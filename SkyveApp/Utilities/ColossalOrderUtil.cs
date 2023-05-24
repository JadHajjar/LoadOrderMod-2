﻿using SkyveApp.ColossalOrder;
using SkyveApp.Domain;
using SkyveApp.Domain.Utilities;
using SkyveApp.Utilities.Managers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace SkyveApp.Utilities;
internal static class ColossalOrderUtil
{
	private const string GAME_SETTINGS_FILE_NAME = "userGameState";
	private static SettingsFile _settingsFile;
	private static bool _initialized;
	private static readonly Dictionary<Mod, SavedBool> _settingsDictionary = new();
	private static readonly FileSystemWatcher _watcher;
	private static readonly DelayedAction _delayedAction = new(500);

	static ColossalOrderUtil()
	{
		_settingsFile = new SettingsFile() { fileName = GAME_SETTINGS_FILE_NAME };
		_settingsFile.Load();

		_watcher = CreateWatcher();
	}

	private static FileSystemWatcher CreateWatcher()
	{
		var watcher = new FileSystemWatcher
		{
			Path = LocationManager.AppDataPath,
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
			Filter = GAME_SETTINGS_FILE_NAME + SettingsFile.extension
		};

		watcher.Changed += new FileSystemEventHandler(FileChanged);
		watcher.Created += new FileSystemEventHandler(FileChanged);
		watcher.Deleted += new FileSystemEventHandler(FileChanged);

		return watcher;
	}

	public static void Start()
	{
		_initialized = true;
		SaveSettings();
	}

	private static void FileChanged(object sender, FileSystemEventArgs e)
	{
		_delayedAction.Run(SettingsFileChanged);
	}

	private static void SettingsFileChanged()
	{
		if (CentralManager.SessionSettings.UserSettings.OverrideGameChanges)
		{
			var currentState = _settingsDictionary.ToDictionary(x => x.Key, x => x.Value.value);

			_settingsFile = new SettingsFile() { fileName = GAME_SETTINGS_FILE_NAME };
			_settingsFile.Load();
			_settingsDictionary.Clear();

			foreach (var kvp in currentState)
			{
				SetEnabled(kvp.Key, kvp.Value);
			}

			SaveSettings();
		}
		else
		{
			_settingsFile = new SettingsFile() { fileName = GAME_SETTINGS_FILE_NAME };
			_settingsFile.Load();
			_settingsDictionary.Clear();

			foreach (var mod in CentralManager.Mods)
			{
				CentralManager.OnInformationUpdated();
			}
		}
	}

	public static bool IsEnabled(Mod mod)
	{
		if (!_settingsDictionary.ContainsKey(mod))
		{
			_settingsDictionary[mod] = GetEnabledSetting(mod);
		}

		return _settingsDictionary[mod].value;
	}

	public static void SetEnabled(Mod mod, bool value)
	{
		if (!_settingsDictionary.ContainsKey(mod))
		{
			_settingsDictionary[mod] = GetEnabledSetting(mod);
		}

		_settingsDictionary[mod].value = value;
	}

	public static void SaveSettings()
	{
		if (_initialized)
		{
			_watcher.EnableRaisingEvents = false;
			_settingsFile.Save();
			_watcher.EnableRaisingEvents = true;
		}
	}

	private static SavedBool GetEnabledSetting(Mod mod)
	{
		var savedEnabledKey_ = $"{Path.GetFileNameWithoutExtension(mod.Folder)}{GetLegacyHashCode(mod.Folder)}.enabled";

		return new SavedBool(savedEnabledKey_, GAME_SETTINGS_FILE_NAME, def: false, autoUpdate: false) { settingsFile = _settingsFile };
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public static unsafe int GetLegacyHashCode(string str)
	{
		//fixed (char* ptr = str + (RuntimeHelpers.OffsetToStringData / 2))
		fixed (char* ptr = str + (12 / 2))
		{
			var ptr2 = ptr;
			var ptr3 = ptr2 + str.Length - 1;
			var num = 0;
			while (ptr2 < ptr3)
			{
				num = (num << 5) - num + *ptr2;
				num = (num << 5) - num + ptr2[1];
				ptr2 += 2;
			}
			ptr3++;
			if (ptr2 < ptr3)
			{
				num = (num << 5) - num + *ptr2;
			}
			return num;
		}
	}
}
