﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Domain.Enums;
using LoadOrderToolTwo.Domain.Interfaces;
using LoadOrderToolTwo.Domain.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Environment;

namespace LoadOrderToolTwo.Utilities.Managers;
internal static class CentralManager
{
	private static List<Package>? packages;

	public static event Action? ContentLoaded;
	public static event Action? WorkshopInfoUpdated;
	public static event Action? PackageInformationUpdated;
	public static event Action? PackageInclusionUpdated;

	private static readonly DelayedAction _delayedWorkshopInfoUpdated;
	private static readonly DelayedAction _delayedPackageInformationUpdated;
	private static readonly DelayedAction _delayedPackageInclusionUpdated;
	private static readonly DelayedAction _delayedContentLoaded;

	public static Profile CurrentProfile => ProfileManager.CurrentProfile;
	public static bool IsContentLoaded { get; private set; }
	public static SessionSettings SessionSettings { get; }
	public static IEnumerable<Package> Packages
	{
		get
		{
			var currentPackages = packages is null ? new() : new List<Package>(packages);

			foreach (var package in currentPackages)
			{
				if (package.IsPseudoMod && SessionSettings.UserSettings.HidePseudoMods)
				{
					continue;
				}

				yield return package;
			}
		}
	}

	public static IEnumerable<Mod> Mods
	{
		get
		{
			var currentPackages = packages is null ? new() : new List<Package>(packages);

			foreach (var package in currentPackages)
			{
				if (package.IsPseudoMod && SessionSettings.UserSettings.HidePseudoMods)
				{
					continue;
				}

				if (package.Mod != null)
				{
					yield return package.Mod;
				}
			}
		}
	}

	public static IEnumerable<Asset> Assets
	{
		get
		{
			var currentPackages = packages is null ? new() : new List<Package>(packages);

			foreach (var package in currentPackages)
			{
				if (package.IsPseudoMod && SessionSettings.UserSettings.HidePseudoMods)
				{
					continue;
				}

				if (package.Assets == null)
				{
					continue;
				}

				foreach (var asset in package.Assets)
				{
					yield return asset;
				}
			}
		}
	}

	static CentralManager()
	{
		ISave.CustomSaveDirectory = Program.CurrentDirectory;

		try
		{
			var folder = GetFolderPath(SpecialFolder.LocalApplicationData);

			Directory.CreateDirectory(Path.Combine(folder, ISave.AppName));

			if (Directory.Exists(Path.Combine(folder, ISave.AppName)))
			{
				ISave.CustomSaveDirectory = folder;
			}
		}
		catch { }

		SessionSettings = ISave.Load<SessionSettings>(nameof(SessionSettings) + ".json");

		_delayedContentLoaded = new(300, () => ContentLoaded?.Invoke());
		_delayedWorkshopInfoUpdated = new(300, () => WorkshopInfoUpdated?.Invoke());
		_delayedPackageInformationUpdated = new(300, () => PackageInformationUpdated?.Invoke());
		_delayedPackageInclusionUpdated = new(300, () => PackageInclusionUpdated?.Invoke());
	}

	public static void Start()
	{
		if (!SessionSettings.FirstTimeSetupCompleted)
		{
			try
			{ RunFirstTimeSetup(); }
			catch (Exception ex) { Log.Exception(ex, "Failed to complete the First Time Setup", true); }
		}

		ConnectionHandler.Start();

		Log.Info("Loading packages..");

		var content = ContentUtil.LoadContents();

		Log.Info($"Loaded {content.Count} packages");
		Log.Info($"Analyzing packages..");

		try
		{ AnalyzePackages(content); }
		catch (Exception ex) { Log.Exception(ex, "Failed to analyze packages"); }

		Log.Info($"Finished analyzing packages..");

		packages = content;

		CompatibilityManager.LoadCachedData();

		CompatibilityManager.CacheReport(packages);

		CompatibilityManager.FirstLoadComplete = true;

		IsContentLoaded = true;

		OnContentLoaded();

		SubscriptionsUtil.Start();

		if (CommandUtil.PreSelectedProfile == CurrentProfile.Name)
		{
			Log.Info($"[Command] Applying Profile ({CurrentProfile.Name})..");
			ProfileManager.SetProfile(CurrentProfile);
		}

		ColossalOrderUtil.Start();

		if (CommandUtil.LaunchOnLoad)
		{
			Log.Info($"[Command] Launching Cities..");
			CitiesManager.Launch();
		}

		if (CommandUtil.NoWindow)
		{
			Log.Info($"[Command] Closing App..");
			return;
		}

		Log.Info($"Starting Listeners..");

		ContentUtil.StartListeners();

		Log.Info($"Listeners Started");

		if (ConnectionHandler.CheckConnection())
		{
			LoadDlcAndCR();
		}
		else
		{
			Log.Warning("Not connected to the internet, delaying remaining loads.");

			ConnectionHandler.WhenConnected(() => new BackgroundAction(LoadDlcAndCR).Run());
		}

		WorkshopInfoUpdated?.Invoke();

		Log.Info($"Finished.");
	}

	private static void LoadDlcAndCR()
	{
		try
		{ SteamUtil.LoadDlcs(); }
		catch { }

		Log.Info($"Downloading compatibility data..");

		CompatibilityManager.DownloadData();

		Log.Info($"Compatibility data downloaded");

		CompatibilityManager.CacheReport();

		CompatibilityManager.FirstLoadComplete = true;

		Log.Info($"Compatibility report cached");
	}

	private static void RunFirstTimeSetup()
	{
		Log.Info("Running First Time Setup");

		LocationManager.RunFirstTimeSetup();

		Log.Info("First Time Setup Completed");

		if (LocationManager.Platform is Platform.Windows)
		{
			ContentUtil.CreateShortcut();
		}

		SessionSettings.FirstTimeSetupCompleted = true;
		SessionSettings.Save();

		Log.Info("Saved Session Settings");

		Directory.CreateDirectory(LocationManager.LotAppDataPath);

		File.WriteAllText(LocationManager.Combine(LocationManager.LotAppDataPath, "SetupComplete.txt"), "Delete this file if your LOT hasn't been set up correctly and want to try again.\r\n\r\nLaunch the game, enable the mod and open Load Order Tool from the main menu after deleting this file.");

		ProfileManager.ConvertLegacyProfiles();
	}

	private static void AnalyzePackages(List<Package> content)
	{
		var firstTime = UpdateManager.IsFirstTime();

		foreach (var package in content)
		{
			if (!firstTime)
			{
				HandleNewPackage(package);
			}

			if (package.Mod is not null)
			{
				if (!SessionSettings.UserSettings.AdvancedIncludeEnable)
				{
					if (package.Mod.IsIncluded && !package.Mod.IsEnabled)
					{
						package.Mod.IsIncluded = false;
					}
				}

				if (!SessionSettings.UserSettings.LinkModAssets && package.Assets is not null)
				{
					foreach (var asset in package.Assets)
					{
						asset.IsIncluded = package.Mod.IsIncluded;
					}
				}

				ModLogicManager.Analyze(package.Mod);
			}
		}

		Log.Info($"Applying analysis results..");

		ModLogicManager.ApplyRequiredStates();
	}

	private static void HandleNewPackage(Package package)
	{
		if (UpdateManager.IsPackageKnown(package))
		{
			return;
		}

		if (package.Mod is not null)
		{
			package.Mod.IsIncluded = !SessionSettings.UserSettings.DisableNewModsByDefault;

			if (SessionSettings.UserSettings.AdvancedIncludeEnable)
			{
				package.Mod.IsEnabled = !SessionSettings.UserSettings.DisableNewModsByDefault;
			}
		}

		if (package.Assets is not null)
		{
			foreach (var asset in package.Assets)
			{
				asset.IsIncluded = !SessionSettings.UserSettings.DisableNewAssetsByDefault;
			}
		}
	}

	internal static void AddPackage(Package package)
	{
		var info = SteamUtil.GetItem(package.SteamId);

		if (info is not null)
		{
			package.SetSteamInformation(info, true);
		}

		if (packages is null)
		{
			packages = new List<Package>() { package };
		}
		else
		{
			packages.Add(package);
		}

		HandleNewPackage(package);

		if (package.Mod is not null)
		{
			ModLogicManager.Analyze(package.Mod);
		}

		OnInformationUpdated();
		OnContentLoaded();
	}

	internal static void RemovePackage(Package package)
	{
		packages?.Remove(package);

		if (package.Mod is not null)
		{
			ModLogicManager.ModRemoved(package.Mod);
		}

		package.Status = DownloadStatus.NotDownloaded;
		OnContentLoaded();
		_delayedWorkshopInfoUpdated.Run();
	}

	internal static void OnInformationUpdated()
	{
		_delayedPackageInformationUpdated.Run();
	}

	internal static void OnInclusionUpdated()
	{
		_delayedPackageInclusionUpdated.Run();
		_delayedPackageInformationUpdated.Run();
	}

	internal static void OnWorkshopInfoUpdated()
	{
		_delayedWorkshopInfoUpdated.Run();
	}

	internal static void OnContentLoaded()
	{
		AssetsUtil.BuildAssetIndex();

		_delayedContentLoaded.Run();
	}
}
