﻿using Extensions;

using LoadOrderToolTwo.Domain.Compatibility;
using LoadOrderToolTwo.Domain.Interfaces;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoadOrderToolTwo.Utilities.Managers;

public static class CompatibilityManager
{
	private const string DATA_CACHE_FILE = "CompatibilityDataCache.json";
	private static readonly Dictionary<IPackage, CompatibilityInfo> _cache = new(new Domain.IPackageEqualityComparer());

	public static IndexedCompatibilityData CompatibilityData { get; private set; }

	public static bool FirstLoadComplete { get; set; }

	static CompatibilityManager()
	{
		CompatibilityData = new(null);

		LoadCachedData();

		ConnectionHandler.WhenConnected(() => new BackgroundAction(DownloadData).Run());

		CentralManager.ContentLoaded += () => new BackgroundAction(CacheReport).Run();
		CentralManager.PackageInclusionUpdated += () => new BackgroundAction(CacheReport).Run();
	}

	internal static void CacheReport()
	{
		CacheReport(CentralManager.Packages);
	}

	internal static void CacheReport(IEnumerable<Domain.Package> content)
	{
		foreach (var package in content)
		{
			GetCompatibilityInfo(package, true);
		}

		CentralManager.OnInformationUpdated();
	}

	internal static void LoadCachedData()
	{
		try
		{
			var path = ISave.GetPath(DATA_CACHE_FILE);

			ISave.Load(out CompatibilityData? data, DATA_CACHE_FILE);

			CompatibilityData = new IndexedCompatibilityData(data);
		}
		catch { }
	}

	internal static async void DownloadData()
	{
		try
		{
			var data = await CompatibilityApiUtil.Get<CompatibilityData>("/Catalogue");

			if (data is not null)
			{
				ISave.Save(data, DATA_CACHE_FILE);

				CompatibilityData = new IndexedCompatibilityData(data);

				CacheReport();

				return;
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "Failed to get compatibility data");
		}

		CompatibilityData ??= new IndexedCompatibilityData(new());
	}

	public static bool IsBlacklisted(IPackage package)
	{
		return CompatibilityData.BlackListedIds.Contains(package.SteamId)
			|| CompatibilityData.BlackListedNames.Contains(package.Name ?? string.Empty);
	}

	internal static Domain.Package? FindPackage(IndexedPackage package)
	{
		var localPackage = CentralManager.Packages.FirstOrDefault(x => x.SteamId == package.Package.SteamId)
			?? CentralManager.Mods.FirstOrDefault(x => Path.GetFileName(x.FileName) == package.Package.FileName)?.Package;

		if (localPackage is not null)
		{
			return localPackage;
		}

		if (!package.Interactions.ContainsKey(InteractionType.SucceededBy))
		{
			return null;
		}

		return package.Interactions[InteractionType.SucceededBy]
					.SelectMany(x => x.Packages.Values)
					.Select(FindPackage)
					.FirstOrDefault(x => x is not null);
	}

	internal static CompatibilityInfo GetCompatibilityInfo(this IPackage package, bool noCache = false)
	{
		if (!FirstLoadComplete && !noCache)
		{
			return new CompatibilityInfo(package, null);
		}

		if (!noCache && _cache.TryGetValue(package, out var info))
		{
			return info;
		}

		return _cache[package] = GenerateCompatibilityInfo(package);
	}

	private static CompatibilityInfo GenerateCompatibilityInfo(IPackage package)
	{
		var packageData = GetPackageData(package);
		var info = new CompatibilityInfo(package, packageData);

		if (package.Package?.Mod is not null)
		{
			var modName = Path.GetFileName(package.Package.Mod.FileName);
			var duplicate = CentralManager.Mods.AllWhere(x => x.IsIncluded && modName == Path.GetFileName(x.FileName));

			if (duplicate.Count > 1)
			{
				info.Add(ReportType.Compatibility
					, new PackageInteraction { Type = InteractionType.Identical, Action = StatusAction.SelectOne }
					, LocaleCR.Get($"Interaction_{InteractionType.Identical}")
					, duplicate.Select(x => new PseudoPackage(x)).ToArray());
			}
		}

		if (packageData is null)
		{
			return info;
		}

		info.Add(ReportType.Stability, new StabilityStatus(packageData.Package.Stability, packageData.Package.Note, false), LocaleCR.Get($"Stability_{packageData.Package.Stability}"), new PseudoPackage[0]);

		if (packageData.Package.Stability is not PackageStability.Broken)
		{
			info.Add(ReportType.Stability, new StabilityStatus(PackageStability.Stable, string.Empty, true), ((packageData.Package.Stability is not PackageStability.NotReviewed and not PackageStability.AssetNotReviewed) ? (LocaleCR.LastReviewDate.Format(packageData.Package.ReviewDate.ToReadableString(packageData.Package.ReviewDate.Year != DateTime.Now.Year, ExtensionClass.DateFormat.TDMY)) + "\r\n\r\n") : string.Empty) + LocaleCR.RequestReviewInfo, new PseudoPackage[0]);
		}

		foreach (var status in packageData.Statuses)
		{
			foreach (var item in status.Value)
			{
				HandleStatus(info, item);
			}
		}

		if (package.IsMod && !info.Links.Any(x => x.Type is LinkType.Github))
		{
			HandleStatus(info, new PackageStatus { Type = StatusType.SourceCodeNotAvailable, Action = StatusAction.UnsubscribeThis });
		}

		if (package.IsMod && package.SteamDescription is not null && package.SteamDescription.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length <= 3)
		{
			HandleStatus(info, new PackageStatus { Type = StatusType.IncompleteDescription, Action = StatusAction.UnsubscribeThis });
		}

		foreach (var interaction in packageData.Interactions)
		{
			foreach (var item in interaction.Value)
			{
				HandleInteraction(info, item);
			}
		}

		if (packageData.Package.RequiredDLCs?.Any() ?? false)
		{
			var missing = packageData.Package.RequiredDLCs.Where(x => !SteamUtil.IsDlcInstalledLocally(x));

			if (missing.Any())
			{
				HandleStatus(info, new PackageStatus
				{
					Type = StatusType.MissingDlc,
					Action = StatusAction.NoAction,
					Packages = missing.Select(x => (ulong)x).ToArray()
				});
			}
		}

		return info;
	}

	private static IndexedPackage? GetPackageData(IPackage package)
	{
		if (package.Workshop)
		{
			var packageData = CompatibilityData.Packages.TryGet(package.SteamId);

			if (packageData is null)
			{
				packageData = new IndexedPackage(GetAutomatedReport(package));

				packageData.Load(CompatibilityData.Packages);
			}

			return packageData;
		}

		if (package.Package?.Mod is not null)
		{
			return CompatibilityData.Packages.Values
				.AllWhere(x => x.Package.FileName == Path.GetFileName(package.Package.Mod.FileName))
				.FirstOrAny(x => !x.Statuses.ContainsKey(StatusType.TestVersion));
		}

		return null;
	}

	private static void HandleStatus(CompatibilityInfo info, IndexedPackageStatus status)
	{
		var type = status.Status.Type;

		if (type == StatusType.DependencyMod && ContentUtil.GetReferencingPackage(info.Package.SteamId, true).Any())
		{
			return;
		}

		var reportType = type switch
		{
			StatusType.Deprecated => (status.Status.Packages?.Length ?? 0) == 0 ? ReportType.Stability : ReportType.Successors,
			StatusType.CausesIssues or StatusType.SavesCantLoadWithoutIt => ReportType.Stability,
			StatusType.DependencyMod or StatusType.TestVersion or StatusType.MusicCanBeCopyrighted => ReportType.Status,
			StatusType.SourceCodeNotAvailable or StatusType.IncompleteDescription or StatusType.Reupload => ReportType.Ambiguous,
			StatusType.MissingDlc => ReportType.DlcMissing,
			_ => ReportType.Status,
		};

		var translation = LocaleCR.Get($"Status_{type}");
		var action = LocaleCR.Get($"Action_{status.Status.Action}");
		var text = (status.Status.Packages?.Length ?? 0) switch { 0 => translation.Zero, 1 => translation.One, _ => translation.Plural } ?? translation.One;
		var actionText = (status.Status.Packages?.Length ?? 0) switch { 0 => action.Zero, 1 => action.One, _ => action.Plural } ?? action.One;
		var message = string.Format($"{text}\r\n\r\n{actionText}", info.Package.CleanName(), SteamUtil.GetItem(status.Status.Packages?.FirstOrDefault() ?? 0)?.CleanName() ?? string.Empty).Trim();

		info.Add(reportType, status.Status, message, status.Status.Packages ?? new ulong[0]);
	}

	private static void HandleInteraction(CompatibilityInfo info, IndexedPackageInteraction interaction)
	{
		var type = interaction.Interaction.Type;

		if (type is InteractionType.Successor or InteractionType.RequirementAlternative)
		{
			return;
		}

		if (type is InteractionType.SucceededBy && interaction.Interaction.Action is StatusAction.NoAction)
		{
			return;
		}

		var packages = interaction.Interaction.Packages?.ToList() ?? new();

		if (type is InteractionType.SameFunctionality or InteractionType.CausesIssuesWith or InteractionType.IncompatibleWith)
		{
			packages.RemoveAll(x => !(GetPackage(x, false)?.IsIncluded ?? false));
		}
		else if (type is InteractionType.RequiredPackages or InteractionType.OptionalPackages)
		{
			packages.RemoveAll(x => GetPackage(x, true)?.IsIncluded ?? false);
		}

		if (packages.Count == 0)
		{
			return;
		}

		var reportType = type switch
		{
			InteractionType.SucceededBy => ReportType.Successors,
			InteractionType.Alternative => ReportType.Alternatives,
			InteractionType.SameFunctionality or InteractionType.CausesIssuesWith or InteractionType.IncompatibleWith => ReportType.Compatibility,
			InteractionType.RequiredPackages => ReportType.RequiredPackages,
			InteractionType.OptionalPackages => ReportType.OptionalPackages,
			_ => ReportType.Compatibility
		};

		var translation = LocaleCR.Get($"Interaction_{type}");
		var action = LocaleCR.Get($"Action_{interaction.Interaction.Action}");
		var text = packages.Count switch { 0 => translation.Zero, 1 => translation.One, _ => translation.Plural } ?? translation.One;
		var actionText = packages.Count switch { 0 => action.Zero, 1 => action.One, _ => action.Plural } ?? action.One;
		var message = string.Format($"{text}\r\n\r\n{actionText}", info.Package.CleanName(), SteamUtil.GetItem(packages.FirstOrDefault())?.CleanName() ?? string.Empty).Trim();

		if (interaction.Interaction.Action is StatusAction.SelectOne)
		{
			packages.Insert(0, info.Package.SteamId);
		}

		info.Add(reportType, interaction.Interaction, message, packages.ToArray());
	}

	private static IPackage? GetPackage(ulong steamId, bool withAlternatives)
	{
		var indexedPackage = CompatibilityData.Packages.TryGet(steamId);

		if (indexedPackage is null)
		{
			return CentralManager.Packages.FirstOrDefault(x => x.SteamId == steamId);
		}

		Domain.Package? package = null;

		if (withAlternatives)
		{
			foreach (var item in indexedPackage.RequirementAlternatives)
			{
				if (item.Key != steamId)
				{
					package = FindPackage(item.Value);

					if (package is not null)
					{
						return package;
					}
				}
			}
		}

		package = FindPackage(indexedPackage);

		if (package is not null)
		{
			return package;
		}

		foreach (var item in indexedPackage.Group)
		{
			if (item.Key != steamId)
			{
				package = FindPackage(item.Value);

				if (package is not null)
				{
					return package;
				}
			}
		}

		return null;
	}

	internal static Package GetAutomatedReport(IPackage package)
	{
		var info = new Package
		{
			Stability = package.IsMod ? PackageStability.NotReviewed : PackageStability.AssetNotReviewed,
			SteamId = package.SteamId,
			Name = package.Name,
			FileName = package.Package?.Mod?.FileName,
			Links = new(),
			Interactions = new(),
			Statuses = new(),
		};

		if (package.RequiredPackages?.Any() ?? false)
		{
			info.Interactions.Add(new PackageInteraction { Type = package.IsMod ? InteractionType.RequiredPackages : InteractionType.OptionalPackages, Action = StatusAction.SubscribeToPackages, Packages = package.RequiredPackages });
		}

		package.ToString().RemoveVersionText(out var titleTags);

		foreach (var tag in titleTags)
		{
			if (tag.ToLower() is "obsolete" or "deprecated" or "abandoned" or "broken")
			{
				info.Stability = PackageStability.Broken;
			}
			else if (tag.ToLower() is "alpha" or "experimental" or "beta" or "test" or "testing")
			{
				info.Statuses.Add(new PackageStatus { Type = StatusType.TestVersion });
			}
		}

		const ulong MUSIC_MOD_ID = 2474585115;

		if (package.RequiredPackages?.Contains(MUSIC_MOD_ID) ?? false)
		{
			info.Statuses.Add(new PackageStatus { Type = StatusType.MusicCanBeCopyrighted });
		}

		if (package.SteamDescription is not null)
		{
			var matches = Regex.Matches(package.SteamDescription, @"\[url\=(https://(?:www\.)?(.+?)/.*?)\]");

			foreach (Match match in matches)
			{
				var type = match.Groups[2].Value.ToLower() switch
				{
					"github.com" => LinkType.Github,
					"discord.com" or "discord.gg" => LinkType.Discord,
					"crowdin.com" => LinkType.Crowdin,
					"buymeacoffee.com" or "patreon.com" or "ko-fi.com" or "paypal.com" => LinkType.Donation,
					_ => LinkType.Other
				};

				if (type is not LinkType.Other)
				{
					info.Links.Add(new PackageLink
					{
						Url = match.Groups[1].Value,
						Type = type,
					});
				}
			}
		}

		if (package.IsMod && DateTime.UtcNow - package.ServerTime > TimeSpan.FromDays(365))
		{
			info.Statuses.Add(new PackageStatus { Type = StatusType.Deprecated });
		}

		return info;
	}

	public static DynamicIcon GetIcon(this LinkType link)
	{
		return link switch
		{
			LinkType.Website => "I_Globe",
			LinkType.Github => "I_Github",
			LinkType.Crowdin => "I_Translate",
			LinkType.Donation => "I_Donate",
			LinkType.Discord => "I_Discord",
			_ => "I_Share",
		};
	}

	public static DynamicIcon GetIcon(this NotificationType notification, bool status)
	{
		return notification switch
		{
			NotificationType.Info => "I_Info",
			NotificationType.MissingDependency => "I_MissingMod",
			NotificationType.Caution => "I_Remarks",
			NotificationType.Warning => "I_MinorIssues",
			NotificationType.AttentionRequired => "I_MajorIssues",
			NotificationType.Switch => "I_Switch",
			NotificationType.Unsubscribe => "I_Broken",
			NotificationType.Exclude => "I_Disposable",
			NotificationType.None or _ => status ? "I_Ok" : "I_Info",
		};
	}

	public static Color GetColor(this NotificationType notification)
	{
		return notification switch
		{
			NotificationType.Info => FormDesign.Design.InfoColor,

			NotificationType.MissingDependency or
			NotificationType.Caution => FormDesign.Design.YellowColor,

			NotificationType.Warning or
			NotificationType.AttentionRequired => FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.RedColor),

			NotificationType.Exclude or
			NotificationType.Unsubscribe => FormDesign.Design.RedColor,

			_ => FormDesign.Design.GreenColor
		};
	}
}
