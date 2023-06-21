﻿using Extensions;

using SkyveApp.Domain;
using SkyveApp.Domain.Compatibility.Enums;
using SkyveApp.Domain.Interfaces;
using SkyveApp.Domain.Utilities;
using SkyveApp.Services.Interfaces;
using SkyveApp.Utilities;
using SkyveShared;

using SlickControls;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkyveApp.Services;
public class ProfileManager : IProfileManager
{
    internal const string LOCAL_APP_DATA_PATH = "%LOCALAPPDATA%";
    internal const string CITIES_PATH = "%CITIES%";
    internal const string WS_CONTENT_PATH = "%WORKSHOP%";

    private readonly List<Profile> _profiles;
    private bool disableAutoSave;
    private readonly FileSystemWatcher? _watcher;

    public bool ApplyingProfile { get; private set; }
    public Profile CurrentProfile { get; private set; }
    public IEnumerable<Profile> Profiles
    {
        get
        {
            yield return Profile.TemporaryProfile;

            List<Profile> profiles;

            lock (_profiles)
            {
                profiles = new(_profiles);
            }

            foreach (var profile in profiles)
            {
                yield return profile;
            }
        }
    }

    public bool ProfilesLoaded { get; private set; }

    public event Action<Profile>? ProfileChanged;

    public event Action? ProfileUpdated;

    private readonly ILogger _logger;
    private readonly ILocationManager _locationManager;
    private readonly ISettings _settings;
    private readonly IContentManager _contentManager;
    private readonly ICompatibilityManager _compatibilityManager;

	public ProfileManager(ILogger logger, ILocationManager locationManager, ISettings settings, IContentManager contentManager, ICompatibilityManager compatibilityManager)
	{
		_logger = logger;
		_locationManager = locationManager;
		_settings = settings;
		_contentManager = contentManager;

		try
		{
			var current = LoadCurrentProfile();

			if (current != null)
			{
				_profiles = new() { current };
				CurrentProfile = current;
			}
		}
		catch (Exception ex)
		{
			_logger.Exception(ex, "Failed to load the current profile");
		}

		_profiles ??= new();

		CurrentProfile ??= Profile.TemporaryProfile;

		if (Directory.Exists(_locationManager.SkyveAppDataPath))
		{
			Directory.CreateDirectory(_locationManager.SkyveProfilesAppDataPath);

			_watcher = new FileSystemWatcher
			{
				Path = _locationManager.SkyveProfilesAppDataPath,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				Filter = "*.json"
			};
		}

		if (!CommandUtil.NoWindow)
		{
			new BackgroundAction(ConvertLegacyProfiles).Run();
			new BackgroundAction(LoadAllProfiles).Run();
		}
		_compatibilityManager = compatibilityManager;
	}

	private Profile? LoadCurrentProfile()
    {
        if (_settings.SessionSettings.CurrentProfile is null or "" && CommandUtil.PreSelectedProfile is null or "")
        {
            return null;
        }

        var profile = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, (CommandUtil.PreSelectedProfile ?? _settings.SessionSettings.CurrentProfile) + ".json");

        if (!CrossIO.FileExists(profile))
        {
            return null;
        }

        var newProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(File.ReadAllText(profile));

        if (newProfile != null)
        {
            newProfile.Name = Path.GetFileNameWithoutExtension(profile);
            newProfile.LastEditDate = File.GetLastWriteTime(profile);
            newProfile.DateCreated = File.GetCreationTime(profile);

            if (newProfile.LastUsed == DateTime.MinValue)
            {
                newProfile.LastUsed = newProfile.LastEditDate;
            }

            return newProfile;
        }
        else
        {
            _logger.Error($"Could not load the profile: '{profile}'");
        }

        return null;
    }

    public void ConvertLegacyProfiles()
    {
        if (!_settings.SessionSettings.FirstTimeSetupCompleted)
        {
            return;
        }

        _logger.Info("Checking for Legacy profiles");

        var legacyProfilePath = CrossIO.Combine(_locationManager.AppDataPath, "LoadOrder", "LOMProfiles");

        if (!Directory.Exists(legacyProfilePath))
        {
            return;
        }

        _logger.Info("Checking for Legacy profiles");

        foreach (var profile in Directory.EnumerateFiles(legacyProfilePath, "*.xml"))
        {
            var newName = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{Path.GetFileNameWithoutExtension(profile)}.json");

            if (CrossIO.FileExists(newName))
            {
                _logger.Info($"Profile '{newName}' already exists, skipping..");
                continue;
            }

            _logger.Info($"Converting profile '{newName}'..");

            ConvertLegacyProfile(profile);
        }
    }

    internal Profile? ConvertLegacyProfile(string profilePath, bool removeLegacyFile = true)
    {
        var legacyProfilePath = CrossIO.Combine(_locationManager.AppDataPath, "LoadOrder", "LOMProfiles");
        var legacyProfile = LoadOrderTool.Legacy.LoadOrderProfile.Deserialize(profilePath);
        var newProfile = legacyProfile?.ToLot2Profile(Path.GetFileNameWithoutExtension(profilePath));

        if (newProfile != null)
        {
            newProfile.LastEditDate = File.GetLastWriteTime(profilePath);
            newProfile.DateCreated = File.GetCreationTime(profilePath);
            newProfile.LastUsed = newProfile.LastEditDate;

            lock (_profiles)
            {
                _profiles.Add(newProfile);
            }

            Save(newProfile, true);

            if (removeLegacyFile)
            {
                Directory.CreateDirectory(CrossIO.Combine(legacyProfilePath, "Legacy"));

                File.Move(profilePath, CrossIO.Combine(legacyProfilePath, "Legacy", Path.GetFileName(profilePath)));
            }
        }
        else
        {
            _logger.Error($"Could not load the profile: '{profilePath}'");
        }

        return newProfile;
    }

    private void CentralManager_ContentLoaded()
    {
        new BackgroundAction(() =>
        {
            List<Profile> profiles;

            lock (_profiles)
            {
                profiles = new(_profiles);
            }

            foreach (var profile in profiles)
            {
                profile.IsMissingItems = profile.Mods.Any(x => GetMod(x) is null) || profile.Assets.Any(x => GetAsset(x) is null);
#if DEBUG
                if (profile.IsMissingItems)
                {
                    _logger.Debug($"Missing items in the profile: {profile}\r\n" +
                        profile.Mods.Where(x => GetMod(x) is null).ListStrings(x => $"{x.Name} ({ToLocalPath(x.RelativePath)})", "\r\n") + "\r\n" +
                        profile.Assets.Where(x => GetAsset(x) is null).ListStrings(x => $"{x.Name} ({ToLocalPath(x.RelativePath)})", "\r\n"));
                }
#endif
            }

            ProfileUpdated?.Invoke();
        }).Run();
    }

	public void MergeProfile(Profile profile)
    {
        new BackgroundAction("Applying profile", apply).Run();

        void apply()
        {
            try
            {
                var unprocessedMods = _contentManager.Mods.ToList();
                var unprocessedAssets = _contentManager.Assets.ToList();
                var missingMods = new List<Profile.Mod>();
                var missingAssets = new List<Profile.Asset>();

                ApplyingProfile = true;

                foreach (var mod in profile.Mods)
                {
                    var localMod = GetMod(mod);

                    if (localMod != null)
                    {
                        localMod.IsIncluded = true;
                        localMod.IsEnabled |= mod.Enabled;

                        unprocessedMods.Remove(localMod);
                    }
                    else
                    {
                        missingMods.Add(mod);
                    }
                }

                foreach (var asset in profile.Assets)
                {
                    var localAsset = GetAsset(asset);

                    if (localAsset != null)
                    {
                        localAsset.IsIncluded = true;

                        unprocessedAssets.Remove(localAsset);
                    }
                    else
                    {
                        missingAssets.Add(asset);
                    }
                }

                if ((missingMods.Count > 0 || missingAssets.Count > 0) && Program.MainForm is not null)
                {
                    UserInterface.Panels.PC_MissingPackages.PromptMissingPackages(Program.MainForm, missingMods, missingAssets);
                }

                ApplyingProfile = false;
                disableAutoSave = true;

                ModsUtil.SavePendingValues();
                AssetsUtil.SaveChanges();

                disableAutoSave = false;

                ProfileChanged?.Invoke(CurrentProfile);

                TriggerAutoSave();
            }
            catch (Exception ex)
            {
                MessagePrompt.Show(ex, "Failed to merge your profiles", form: Program.MainForm);
            }
            finally
            {
                ApplyingProfile = false;
                disableAutoSave = false;
            }
        }
    }

	public void ExcludeProfile(Profile profile)
    {
        new BackgroundAction("Applying profile", apply).Run();

        void apply()
        {
            try
            {
                var unprocessedMods = _contentManager.Mods.ToList();
                var unprocessedAssets = _contentManager.Assets.ToList();
                var missingMods = new List<Profile.Mod>();
                var missingAssets = new List<Profile.Asset>();

                ApplyingProfile = true;

                foreach (var mod in profile.Mods)
                {
                    var localMod = GetMod(mod);

                    if (localMod != null)
                    {
                        localMod.IsIncluded = false;
                        localMod.IsEnabled = false;
                    }
                }

                foreach (var asset in profile.Assets)
                {
                    var localAsset = GetAsset(asset);

                    if (localAsset != null)
                    {
                        localAsset.IsIncluded = false;
                    }
                }

                ApplyingProfile = false;
                disableAutoSave = true;

                ModsUtil.SavePendingValues();
                AssetsUtil.SaveChanges();

                disableAutoSave = false;

                ProfileChanged?.Invoke(CurrentProfile);

                TriggerAutoSave();
            }
            catch (Exception ex)
            {
                MessagePrompt.Show(ex, "Failed to exclude items from your profile", form: Program.MainForm);
            }
            finally
            {
                ApplyingProfile = false;
                disableAutoSave = false;
            }
        }
    }

	public void DeleteProfile(Profile profile)
    {
        CrossIO.DeleteFile(CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{profile.Name}.json"));

        lock (_profiles)
        {
            _profiles.Remove(profile);
        }

        if (profile == CurrentProfile)
        {
            SetProfile(Profile.TemporaryProfile);
        }

        ProfileUpdated?.Invoke();
    }

    public void SetProfile(Profile profile)
    {
        CurrentProfile = profile;

        if (profile.Temporary)
        {
            ProfileChanged?.Invoke(profile);

            _settings.SessionSettings.CurrentProfile = null;
			_settings.SessionSettings.Save();

            return;
        }

        if (Program.MainForm is null)
        {
            apply();
        }
        else
        {
            new BackgroundAction("Applying profile", apply).Run();
        }

        void apply()
        {
            try
            {
                var unprocessedMods = _contentManager.Mods.ToList();
                var unprocessedAssets = _contentManager.Assets.ToList();
                var missingMods = new List<Profile.Mod>();
                var missingAssets = new List<Profile.Asset>();

                ApplyingProfile = true;

                foreach (var mod in profile.Mods)
                {
                    var localMod = GetMod(mod);

                    if (localMod != null)
                    {
                        localMod.IsIncluded = true;
                        localMod.IsEnabled = mod.Enabled;

                        unprocessedMods.Remove(localMod);
                    }
                    else if (!_compatibilityManager.IsBlacklisted(mod))
                    {
                        missingMods.Add(mod);
                    }
                }

                foreach (var asset in profile.Assets)
                {
                    var localAsset = GetAsset(asset);

                    if (localAsset != null)
                    {
                        localAsset.IsIncluded = true;

                        unprocessedAssets.Remove(localAsset);
                    }
                    else if (!_compatibilityManager.IsBlacklisted(asset))
                    {
                        missingAssets.Add(asset);
                    }
                }

                foreach (var mod in unprocessedMods)
                {
                    mod.IsIncluded = false;
                    mod.IsEnabled = false;
                }

                foreach (var asset in unprocessedAssets)
                {
                    asset.IsIncluded = false;
                }

#if DEBUG
                _logger.Debug($"unprocessedMods: {unprocessedMods.Count}\r\n" +
                    $"unprocessedAssets: {unprocessedAssets.Count}\r\n" +
                    $"missingMods: {missingMods.Count}\r\n" +
                    $"missingAssets: {missingAssets.Count}");
#endif

                if ((missingMods.Count > 0 || missingAssets.Count > 0) && Program.MainForm is not null)
                {
                    UserInterface.Panels.PC_MissingPackages.PromptMissingPackages(Program.MainForm, missingMods, missingAssets);
                }

                AssetsUtil.SetDlcsExcluded(profile.ExcludedDLCs.ToArray());

                ApplyingProfile = false;
                disableAutoSave = true;

                ModsUtil.SavePendingValues();
                AssetsUtil.SaveChanges();

                profile.LastUsed = DateTime.Now;
                Save(profile);

                ProfileChanged?.Invoke(profile);

                try
                { SaveLsmSettings(profile); }
                catch (Exception ex) { _logger.Exception(ex, "Failed to apply the LSM settings for profile " + profile.Name); }

                if (!CommandUtil.NoWindow)
                {
                    _settings.SessionSettings.CurrentProfile = profile.Name;
                    _settings.SessionSettings.Save();
                }

                disableAutoSave = false;
            }
            catch (Exception ex)
            {
                MessagePrompt.Show(ex, "Failed to apply your profile", form: Program.MainForm);

                ProfileChanged?.Invoke(profile);
            }
            finally
            {
                ApplyingProfile = false;
                disableAutoSave = false;
            }
        }
    }

	public void TriggerAutoSave()
    {
        if (!disableAutoSave && !ApplyingProfile && _contentManager.IsContentLoaded && !ContentUtil.BulkUpdating)
        {
            Task.Run(() =>
            {
                if (CurrentProfile.AutoSave)
                {
                    CurrentProfile.Save();
                }
                else if (!CurrentProfile.Temporary)
                {
                    CurrentProfile.UnsavedChanges = true;
                    Save(CurrentProfile);
                }
            });
        }
    }

    private void LoadAllProfiles()
    {
        try
        {
            foreach (var profile in Directory.EnumerateFiles(_locationManager.SkyveProfilesAppDataPath, "*.json"))
            {
                if (Path.GetFileNameWithoutExtension(profile).Equals(_settings.SessionSettings.CurrentProfile, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var newProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(File.ReadAllText(profile));

                if (newProfile != null)
                {
                    newProfile.Name = Path.GetFileNameWithoutExtension(profile);
                    newProfile.LastEditDate = File.GetLastWriteTime(profile);
                    newProfile.DateCreated = File.GetCreationTime(profile);

                    if (newProfile.LastUsed == DateTime.MinValue)
                    {
                        newProfile.LastUsed = newProfile.LastEditDate;
                    }

                    lock (_profiles)
                    {
                        _profiles.Add(newProfile);
                    }
                }
                else
                {
                    _logger.Error($"Could not load the profile: '{profile}'");
                }
            }
        }
        catch { }

        if (_contentManager.IsContentLoaded)
        {
            CentralManager_ContentLoaded();
        }

		_contentManager.ContentLoaded += CentralManager_ContentLoaded;

        ProfilesLoaded = true;

        ProfileUpdated?.Invoke();

        if (_watcher is not null)
        {
            _watcher.Changed += new FileSystemEventHandler(FileChanged);
            _watcher.Created += new FileSystemEventHandler(FileChanged);
            _watcher.Deleted += new FileSystemEventHandler(FileChanged);

            try
            { _watcher.EnableRaisingEvents = true; }
            catch (Exception ex) { _logger.Exception(ex, $"Failed to start profile watcher ({_locationManager.SkyveProfilesAppDataPath})"); }
        }
    }

    private void FileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            if (!Path.GetExtension(e.FullPath).Equals(".json", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (!CrossIO.FileExists(e.FullPath))
            {
                lock (_profiles)
                {
                    var profile = _profiles.FirstOrDefault(x => x.Name?.Equals(Path.GetFileNameWithoutExtension(e.FullPath), StringComparison.OrdinalIgnoreCase) ?? false);

                    if (profile != null)
                    {
                        _profiles.Remove(profile);
                    }
                }

                ProfileUpdated?.Invoke();

                return;
            }

            var newProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(File.ReadAllText(e.FullPath));

            if (newProfile != null)
            {
                newProfile.Name = Path.GetFileNameWithoutExtension(e.FullPath);
                newProfile.LastEditDate = File.GetLastWriteTime(e.FullPath);
                newProfile.DateCreated = File.GetCreationTime(e.FullPath);

                if (newProfile.LastUsed == DateTime.MinValue)
                {
                    newProfile.LastUsed = newProfile.LastEditDate;
                }

                lock (_profiles)
                {
                    var currentProfile = _profiles.FirstOrDefault(x => x.Name?.Equals(Path.GetFileNameWithoutExtension(e.FullPath), StringComparison.OrdinalIgnoreCase) ?? false);

                    _profiles.Remove(currentProfile);

                    _profiles.Add(newProfile);
                }

                ProfileUpdated?.Invoke();
            }
            else
            {
                _logger.Error($"Could not load the profile: '{e.FullPath}'");
            }
        }
        catch (Exception ex) { _logger.Exception(ex, "Failed to refresh changes to profiles"); }
    }

    public void GatherInformation(Profile? profile)
    {
        if (profile == null || profile.Temporary || !_contentManager.IsContentLoaded)
        {
            return;
        }

        profile.Assets = _contentManager.Assets.Where(x => x.IsIncluded).Select(x => new Profile.Asset(x)).ToList();
        profile.Mods = _contentManager.Mods.Where(x => x.IsIncluded).Select(x => new Profile.Mod(x)).ToList();
        profile.ExcludedDLCs = SkyveConfig.Deserialize()?.RemovedDLCs.ToList() ?? new();
    }

    public bool Save(Profile? profile, bool forced = false)
    {
        if (profile == null || !forced && (profile.Temporary || !_contentManager.IsContentLoaded))
        {
            return false;
        }

        try
        {
            if (_watcher is not null)
            {
                _watcher.EnableRaisingEvents = false;
            }

            Directory.CreateDirectory(_locationManager.SkyveProfilesAppDataPath);

            File.WriteAllText(
                CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{profile.Name}.json"),
                Newtonsoft.Json.JsonConvert.SerializeObject(profile, Newtonsoft.Json.Formatting.Indented));

            profile.IsMissingItems = profile.Mods.Any(x => GetMod(x) is null) || profile.Assets.Any(x => GetAsset(x) is null);
#if DEBUG
            if (profile.IsMissingItems)
            {
                _logger.Debug($"Missing items in the profile: {profile}\r\n" +
                    profile.Mods.Where(x => GetMod(x) is null).ListStrings(x => $"{x.Name} ({ToLocalPath(x.RelativePath)})", "\r\n") + "\r\n" +
                    profile.Assets.Where(x => GetAsset(x) is null).ListStrings(x => $"{x.Name} ({ToLocalPath(x.RelativePath)})", "\r\n"));
            }
#endif

            return true;
        }
        catch (Exception ex)
        {
            _logger.Exception(ex, $"Failed to save profile ({profile.Name}) to {CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{profile.Name}.json")}");
        }
        finally
        {
            if (_watcher is not null)
            {
                _watcher.EnableRaisingEvents = true;
            }
        }

        return false;
    }

    internal Mod GetMod(Profile.Mod mod)
    {
        return ModsUtil.FindMod(ToLocalPath(mod.RelativePath));
    }

    internal Asset GetAsset(Profile.Asset asset)
    {
        return AssetsUtil.GetAsset(ToLocalPath(asset.RelativePath));
    }

    internal string ToRelativePath(string? localPath)
    {
        if (localPath is null or "")
        {
            return string.Empty;
        }

        return localPath
            .Replace(_locationManager.AppDataPath, LOCAL_APP_DATA_PATH)
            .Replace(_locationManager.GamePath, CITIES_PATH)
            .Replace(_locationManager.WorkshopContentPath, WS_CONTENT_PATH)
            .FormatPath();
    }

    internal string ToLocalPath(string? relativePath)
    {
        if (relativePath is null or "")
        {
            return string.Empty;
        }

        return relativePath
            .Replace(LOCAL_APP_DATA_PATH, _locationManager.AppDataPath)
            .Replace(CITIES_PATH, _locationManager.GamePath)
            .Replace(WS_CONTENT_PATH, _locationManager.WorkshopContentPath)
            .FormatPath();
    }

    internal bool RenameProfile(Profile profile, string text)
    {
        if (profile == null || profile.Temporary)
        {
            return false;
        }

        text = text.EscapeFileName();

        var newName = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{text}.json");
        var oldName = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{profile.Name}.json");

        try
        {
            if (newName == oldName)
            {
                return true;
            }

            if (CrossIO.FileExists(newName))
            {
                return false;
            }

            if (CrossIO.FileExists(oldName))
            {
                File.Move(oldName, newName);

                profile.Name = text;
            }
            else
            {
                profile.Name = text;

                Save(profile);
            }
        }
        finally
        {
            _settings.SessionSettings.CurrentProfile = text;
            _settings.SessionSettings.Save();
        }

        return true;
    }

    internal string GetNewProfileName()
    {
        var startName = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, "New Profile.json");

        // Check if the file with the proposed name already exists
        if (CrossIO.FileExists(startName))
        {
            var extension = ".json";
            var nameWithoutExtension = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, "New Profile");
            var counter = 1;

            // Loop until a valid file name is found
            while (CrossIO.FileExists(startName))
            {
                // Generate the new file name with the counter appended
                startName = $"{nameWithoutExtension} ({counter}){extension}";

                // Increment the counter
                counter++;
            }
        }

        // Return the valid file name
        return Path.GetFileNameWithoutExtension(startName);
    }

    internal List<Package> GetInvalidPackages(PackageUsage usage)
    {
        if ((int)usage == -1)
        {
            return new();
        }

        return _contentManager.Packages.AllWhere(x =>
        {
            var cr = x.GetCompatibilityInfo().Data;

            if (cr is null)
            {
                return false;
            }

            if (cr.Package.Usage.HasFlag(usage))
            {
                return false;
            }

            return x.IsIncluded;
        });
    }

    public void SaveLsmSettings(Profile profile)
    {
        var current = LsmSettingsFile.Deserialize();

        if (current == null)
        {
            return;
        }

        current.loadEnabled = profile.LsmSettings.LoadEnabled;
        current.loadUsed = profile.LsmSettings.LoadUsed;
        current.skipFile = profile.LsmSettings.SkipFile;
        current.skipPrefabs = profile.LsmSettings.UseSkipFile;

        current.SyncAndSerialize();
    }

    internal void AddProfile(Profile newProfile)
    {
        lock (_profiles)
        {
            _profiles.Add(newProfile);
        }

        ProfileUpdated?.Invoke();
    }

    internal Profile? ImportProfile(string obj)
    {
        if (_watcher is not null)
        {
            _watcher.EnableRaisingEvents = false;
        }

        var newPath = CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, Path.GetFileName(obj));

        File.Move(obj, newPath);

        if (_watcher is not null)
        {
            _watcher.EnableRaisingEvents = true;
        }

        var newProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(File.ReadAllText(newPath));

        if (newProfile != null)
        {
            newProfile.Name = Path.GetFileNameWithoutExtension(newPath);
            newProfile.LastEditDate = File.GetLastWriteTime(newPath);
            newProfile.DateCreated = File.GetCreationTime(newPath);

            if (newProfile.LastUsed == DateTime.MinValue)
            {
                newProfile.LastUsed = newProfile.LastEditDate;
            }

            lock (_profiles)
            {
                var currentProfile = _profiles.FirstOrDefault(x => x.Name?.Equals(Path.GetFileNameWithoutExtension(newPath), StringComparison.OrdinalIgnoreCase) ?? false);

                _profiles.Remove(currentProfile);

                _profiles.Add(newProfile);
            }

            ProfileUpdated?.Invoke();

            return newProfile;
        }
        else
        {
            _logger.Error($"Could not load the profile: '{obj}' / '{newPath}'");
        }

        return null;
    }

    internal void SetIncludedForAll<T>(T item, bool value) where T : IPackage
    {
        try
        {
            if (_watcher is not null)
            {
                _watcher.EnableRaisingEvents = false;
            }

            if (item is Mod mod)
            {
                var profileMod = new Profile.Mod(mod);

                foreach (var profile in Profiles.Skip(1))
                {
                    SetIncludedFor(value, profileMod, profile);
                }
            }
            else if (item is Asset asset)
            {
                var profileAsset = new Profile.Asset(asset);

                foreach (var profile in Profiles.Skip(1))
                {
                    SetIncludedFor(value, profileAsset, profile);
                }
            }
            else if (item is Package package)
            {
                var profileMod = package.Mod is null ? null : new Profile.Mod(package.Mod);
                var assets = package.Assets?.Select(x => new Profile.Asset(x)).ToList() ?? new();

                foreach (var profile in Profiles.Skip(1))
                {
                    SetIncludedFor(value, profileMod, assets, profile);
                }
            }
        }
        catch (Exception ex) { _logger.Exception(ex, $"Failed to apply included status '{value}' to package: '{item}'"); }
        finally
        {
            if (_watcher is not null)
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }

    private void SetIncludedFor(bool value, Profile.Mod? profileMod, List<Profile.Asset> assets, Profile profile)
    {
        if (value)
        {
            if (profileMod is not null)
            {
                if (!profile.Mods.Any(x => x.RelativePath?.Equals(profileMod.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    profile.Mods.Add(profileMod);
                }
            }

            if (assets.Count > 0)
            {
                var assetsToAdd = new List<Profile.Asset>(assets);

                foreach (var pa in profile.Assets)
                {
                    foreach (var profileAsset in assetsToAdd)
                    {
                        if (pa.RelativePath?.Equals(profileAsset.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false)
                        {
                            assetsToAdd.Remove(profileAsset);
                            break;
                        }
                    }
                }

                profile.Assets.AddRange(assetsToAdd);
            }
        }
        else
        {
            if (profileMod is not null)
            {
                profile.Mods.RemoveAll(x => x.RelativePath?.Equals(profileMod.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (assets.Count > 0)
            {
                profile.Assets.RemoveAll(x => assets.Any(profileAsset => x.RelativePath?.Equals(profileAsset.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false));
            }
        }

        Save(profile);
    }

    private void SetIncludedFor(bool value, Profile.Asset profileAsset, Profile profile)
    {
        if (value)
        {
            if (!profile.Assets.Any(x => x.RelativePath?.Equals(profileAsset.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                profile.Assets.Add(profileAsset);
            }
        }
        else
        {
            profile.Assets.RemoveAll(x => x.RelativePath?.Equals(profileAsset.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        Save(profile);
    }

    private void SetIncludedFor(bool value, Profile.Mod profileMod, Profile profile)
    {
        if (value)
        {
            if (!profile.Mods.Any(x => x.RelativePath?.Equals(profileMod.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                profile.Mods.Add(profileMod);
            }
        }
        else
        {
            profile.Mods.RemoveAll(x => x.RelativePath?.Equals(profileMod.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        Save(profile);
    }

    internal bool IsPackageIncludedInProfile(IPackage ipackage, Profile profile)
    {
        if (ipackage is Package package)
        {
            var profileMod = package.Mod is null ? null : new Profile.Mod(package.Mod);
            var assets = package.Assets?.Select(x => new Profile.Asset(x)).ToList() ?? new();

            if (profileMod is not null)
            {
                if (!profile.Mods.Any(x => x.RelativePath?.Equals(profileMod.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    return false;
                }
            }

            if (assets.Count > 0)
            {
                if (!assets.All(profileAsset => profile.Assets.Any(x => x.RelativePath?.Equals(profileAsset.RelativePath, StringComparison.OrdinalIgnoreCase) ?? false)))
                {
                    return false;
                }
            }
        }
        else
        {
            if (ipackage.IsMod)
            {
                return profile.Mods.Any(x => x.SteamId == ipackage.SteamId);
            }

            return profile.Assets.Any(x => x.SteamId == ipackage.SteamId);
        }

        return true;
    }

    internal void SetIncludedFor(IPackage ipackage, Profile profile, bool value)
    {
        if (ipackage is Package package)
        {
            var profileMod = package.Mod is null ? null : new Profile.Mod(package.Mod);
            var assets = package.Assets?.Select(x => new Profile.Asset(x)).ToList() ?? new();

            SetIncludedFor(value, profileMod, assets, profile);
        }
        else
        {
            var profileMod = profile.Mods.FirstOrDefault(x => x.SteamId == ipackage.SteamId) ?? new Profile.Mod(ipackage);

            SetIncludedFor(value, profileMod, new(), profile);
        }
    }

    internal string GetFileName(Profile profile)
    {
        return CrossIO.Combine(_locationManager.SkyveProfilesAppDataPath, $"{profile.Name}.json");
    }

    internal void CreateShortcut(Profile item)
    {
        try
        {
            var launch = MessagePrompt.Show(Locale.AskToLaunchGameForShortcut, PromptButtons.YesNo, PromptIcons.Question) == DialogResult.Yes;

            ExtensionClass.CreateShortcut(CrossIO.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), item.Name + ".lnk")
                , Program.ExecutablePath
                , (launch ? "-launch " : "") + $"-profile {item.Name}");
        }
        catch (Exception ex)
        {
            _logger.Exception(ex, "Failed to create shortcut");
        }
    }

    internal async Task Share(Profile item)
    {
        try
        {
            var result = await SkyveApiUtil.SaveUserProfile(new()
            {

                Author = SteamUtil.GetLoggedInSteamId(),
                Banner = item.BannerBytes,
                Color = item.Color?.ToArgb(),
                Name = item.Name,
                ProfileUsage = (int)item.Usage,
                ProfileId = item.ProfileId,
                Contents = item.Assets.Concat(item.Mods).Select(x => x.AsProfileContent()).ToArray()
            });

            if (result.Success)
            {
                item.ProfileId = (int)Convert.ChangeType(result.Data, typeof(int));
                item.Author = SteamUtil.GetLoggedInSteamId();

                Save(item);
            }
            else
            {
                Program.MainForm.TryInvoke(() => MessagePrompt.Show((item.ProfileId == 0 ? Locale.FailedToUploadProfile : Locale.FailedToUpdateProfile) + "\r\n\r\n" + LocaleHelper.GetGlobalText(result.Message), PromptButtons.OK, PromptIcons.Error, form: Program.MainForm));
            }
        }
        catch (Exception ex) { Program.MainForm.TryInvoke(() => MessagePrompt.Show(ex, item.ProfileId == 0 ? Locale.FailedToUploadProfile : Locale.FailedToUpdateProfile, form: Program.MainForm)); }
    }

    internal async Task<bool> DownloadProfile(IProfile item)
    {
        try
        {
            var profile = await SkyveApiUtil.GetUserProfileContents(item.ProfileId);

            if (profile == null)
            {
                return false;
            }

            var generatedProfile = Profiles.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase)) ?? profile.CloneTo<IProfile, Profile>();

            generatedProfile.Color = ((IProfile)profile).Color;
            generatedProfile.Author = profile.Author;
            generatedProfile.ProfileId = profile.ProfileId;
            generatedProfile.Usage = profile.Usage;
            generatedProfile.BannerBytes = profile.Banner;
            generatedProfile.Assets = profile.Contents.Where(x => !x.IsMod).ToList(x => new Profile.Asset(x));
            generatedProfile.Mods = profile.Contents.Where(x => x.IsMod).ToList(x => new Profile.Mod(x));

            return Save(generatedProfile);
        }
        catch (Exception ex)
        {
            _logger.Exception(ex, "Failed to download profile");

            return false;
        }
    }

    internal async Task<bool> DownloadProfile(string link)
    {
        try
        {
            var profile = await SkyveApiUtil.GetUserProfileByLink(link);

            if (profile == null)
            {
                return false;
            }

            var generatedProfile = profile.CloneTo<IProfile, Profile>();

            generatedProfile.Assets = profile.Contents.Where(x => !x.IsMod).ToList(x => new Profile.Asset(x));
            generatedProfile.Mods = profile.Contents.Where(x => x.IsMod).ToList(x => new Profile.Mod(x));

            return Save(generatedProfile);
        }
        catch (Exception ex)
        {
            _logger.Exception(ex, "Failed to download profile");

            return false;
        }
    }

    internal async Task<bool> SetVisibility(Profile profile, bool @public)
    {
        try
        {
            var result = await SkyveApiUtil.SetProfileVisibility(profile.ProfileId, @public);

            if (!result.Success)
            {
                Program.MainForm.TryInvoke(() => MessagePrompt.Show(Locale.FailedToUpdateProfile + "\r\n\r\n" + LocaleHelper.GetGlobalText(result.Message), PromptButtons.OK, PromptIcons.Error, form: Program.MainForm));
            }
            else
            {
                profile.Public = @public;
                return Save(profile);
            }

            return result.Success;
        }
        catch (Exception ex) { Program.MainForm.TryInvoke(() => MessagePrompt.Show(ex, Locale.FailedToUpdateProfile, form: Program.MainForm)); return false; }
    }

    internal async Task<bool> DeleteOnlineProfile(IProfile profile)
    {
        try
        {
            var result = await SkyveApiUtil.DeleteUserProfile(profile.ProfileId);

            if (!result.Success)
            {
                Program.MainForm.TryInvoke(() => MessagePrompt.Show(Locale.FailedToDeleteProfile + "\r\n\r\n" + LocaleHelper.GetGlobalText(result.Message), PromptButtons.OK, PromptIcons.Error, form: Program.MainForm));
            }

            return result.Success;
        }
        catch (Exception ex) { Program.MainForm.TryInvoke(() => MessagePrompt.Show(ex, Locale.FailedToDeleteProfile, form: Program.MainForm)); return false; }
    }
}
