﻿using Extensions;

using SkyveApp.Domain;
using SkyveApp.Domain.Systems;
using SkyveApp.Systems.CS1.Utilities;

using SkyveShared;

using SlickControls;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkyveApp.Systems.CS1.Managers;
internal class CentralManager : ICentralManager
{
	private readonly IModLogicManager _modLogicManager;
	private readonly ICompatibilityManager _compatibilityManager;
	private readonly IPlaysetManager _profileManager;
	private readonly ICitiesManager _citiesManager;
	private readonly ILocationManager _locationManager;
	private readonly IUpdateManager _updateManager;
	private readonly ISubscriptionsManager _subscriptionManager;
	private readonly IPackageManager _packageManager;
	private readonly IContentManager _contentManager;
	private readonly ColossalOrderUtil _colossalOrderUtil;
	private readonly ISettings _settings;
	private readonly ILogger _logger;
	private readonly INotifier _notifier;
	private readonly IModUtil _modUtil;
	private readonly IBulkUtil _bulkUtil;
	private readonly IVersionUpdateService _versionUpdateService;

	public CentralManager(IModLogicManager modLogicManager, ICompatibilityManager compatibilityManager, IPlaysetManager profileManager, ICitiesManager citiesManager, ILocationManager locationManager, IUpdateManager updateManager, ISubscriptionsManager subscriptionManager, IPackageManager packageManager, IContentManager contentManager, ColossalOrderUtil colossalOrderUtil, ISettings settings, ILogger logger, INotifier notifier, IModUtil modUtil, IBulkUtil bulkUtil, IVersionUpdateService versionUpdateService)
	{
		_modLogicManager = modLogicManager;
		_compatibilityManager = compatibilityManager;
		_profileManager = profileManager;
		_citiesManager = citiesManager;
		_locationManager = locationManager;
		_updateManager = updateManager;
		_subscriptionManager = subscriptionManager;
		_packageManager = packageManager;
		_contentManager = contentManager;
		_colossalOrderUtil = colossalOrderUtil;
		_settings = settings;
		_logger = logger;
		_notifier = notifier;
		_modUtil = modUtil;
		_bulkUtil = bulkUtil;
		_versionUpdateService = versionUpdateService;
	}

	public void Start()
	{
		if (!_settings.SessionSettings.FirstTimeSetupCompleted)
		{
			try
			{ RunFirstTimeSetup(); }
			catch (Exception ex)
			{
				_logger.Exception(ex, "Failed to complete the First Time Setup");

				MessagePrompt.Show(ex, "Failed to complete the First Time Setup", form: SystemsProgram.MainForm as SlickForm);
			}
		}

		ConnectionHandler.AssumeInternetConnectivity = _settings.UserSettings.AssumeInternetConnectivity;

		ConnectionHandler.Start();

		_logger.Info("Loading packages..");

		var content = _contentManager.LoadContents();

		_logger.Info($"Loaded {content.Count} packages");

		_versionUpdateService.Run(content);

		_packageManager.SetPackages(content);

		_logger.Info($"Loading and applying CR Data..");

		_compatibilityManager.Start(content);

		_logger.Info($"Analyzing packages..");

		try
		{ AnalyzePackages(content); }
		catch (Exception ex) { _logger.Exception(ex, "Failed to analyze packages"); }

		_logger.Info($"Finished analyzing packages..");

		_notifier.OnContentLoaded();

		_subscriptionManager.Start();

		if (CommandUtil.PreSelectedProfile == _profileManager.CurrentPlayset.Name)
		{
			_logger.Info($"[Command] Applying Playset ({_profileManager.CurrentPlayset.Name})..");
			_profileManager.SetCurrentPlayset(_profileManager.CurrentPlayset);
		}

		_colossalOrderUtil.Start();

		if (CommandUtil.LaunchOnLoad)
		{
			_logger.Info($"[Command] Launching Cities..");
			_citiesManager.Launch();
		}

		if (CommandUtil.NoWindow)
		{
			_logger.Info($"[Command] Closing App..");
			return;
		}

		_logger.Info($"Starting Listeners..");

		_contentManager.StartListeners();

		_logger.Info($"Listeners Started");

		if (ConnectionHandler.CheckConnection())
		{
			LoadDlcAndCR();
		}
		else
		{
			_logger.Warning("Not connected to the internet, delaying remaining loads.");

			ConnectionHandler.WhenConnected(() => new BackgroundAction(LoadDlcAndCR).Run());
		}

		_notifier.OnWorkshopInfoUpdated();

		_logger.Info($"Finished.");
	}

	private void LoadDlcAndCR()
	{
		try
		{ SteamUtil.LoadDlcs(); }
		catch { }

		_logger.Info($"Downloading compatibility data..");

		_compatibilityManager.DownloadData();

		_logger.Info($"Compatibility data downloaded");

		_compatibilityManager.CacheReport();

		_logger.Info($"Compatibility report cached");
	}

	private void RunFirstTimeSetup()
	{
		_logger.Info("Running First Time Setup");

		_locationManager.RunFirstTimeSetup();

		_logger.Info("First Time Setup Completed");

		if (CrossIO.CurrentPlatform is Platform.Windows)
		{
			_locationManager.CreateShortcut();
		}

		_settings.SessionSettings.FirstTimeSetupCompleted = true;
		_settings.SessionSettings.Save();

		_logger.Info("Saved Session Settings");

		Directory.CreateDirectory(_locationManager.SkyveAppDataPath);

		File.WriteAllText(CrossIO.Combine(_locationManager.SkyveAppDataPath, "SetupComplete.txt"), "Delete this file if your LOT hasn't been set up correctly and want to try again.\r\n\r\nLaunch the game, enable the mod and open Skyve from the main menu after deleting this file.");
	}

	private void AnalyzePackages(List<ILocalPackageWithContents> content)
	{
		var firstTime = _updateManager.IsFirstTime();
		var blackList = new List<ILocalPackageWithContents>();

		foreach (var package in content)
		{
			if (_compatibilityManager.IsBlacklisted(package))
			{
				blackList.Add(package);
				continue;
			}

			if (package.Mod is not null)
			{
				if (!_settings.UserSettings.AdvancedIncludeEnable)
				{
					if (!_modUtil.IsEnabled(package.Mod) && _modUtil.IsIncluded(package.Mod))
					{
						_modUtil.SetIncluded(package.Mod, false);
					}
				}

				if (_settings.UserSettings.LinkModAssets && package.Assets is not null)
				{
					_bulkUtil.SetBulkIncluded(package.Assets, _modUtil.IsIncluded(package.Mod));
				}

				_modLogicManager.Analyze(package.Mod, _modUtil);
			}
		}

		content.RemoveAll(x => blackList.Contains(x));

		if (blackList.Count > 0)
		{
			BlackListTransfer.SendList(blackList.Select(x => x.Id), false);
		}
		else if (CrossIO.FileExists(BlackListTransfer.FilePath))
		{
			CrossIO.DeleteFile(BlackListTransfer.FilePath);
		}

		foreach (var item in blackList)
		{
			_packageManager.DeleteAll(item.Folder);
		}

		_logger.Info($"Applying analysis results..");

		_modLogicManager.ApplyRequiredStates(_modUtil);
	}
}
