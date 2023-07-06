﻿using Extensions;

using SkyveApp.Domain;
using SkyveApp.Domain.CS1.Utilities;
using SkyveApp.Domain.Enums;
using SkyveApp.Domain.Systems;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyveApp.Systems.CS1.Managers;
internal class ModLogicManager : IModLogicManager
{
	private const string HARMONY_ASSEMBLY = "CitiesHarmony.dll";
	private const string PATCH_ASSEMBLY = "PatchLoaderMod.dll";
	private const string Skyve_ASSEMBLY = "SkyveMod.dll";
	private const string LOM2_ASSEMBLY = "LoadOrderModTwo.dll";
	private const string LOM1_ASSEMBLY = "LoadOrderMod.dll";

	private readonly ModCollection _modCollection = new(GetGroupInfo());

	private static Dictionary<string, CollectionInfo> GetGroupInfo()
	{
		return new(StringComparer.OrdinalIgnoreCase)
		{
			[HARMONY_ASSEMBLY] = new() { Required = true },
			[PATCH_ASSEMBLY] = new() { Required = true },
			[Skyve_ASSEMBLY] = new() { Required = true },
			[LOM2_ASSEMBLY] = new() { Required = false },
			[LOM1_ASSEMBLY] = new() { Required = false },
		};
	}

	private readonly ISettings _settings;

	public ModLogicManager(ISettings settings)
	{
		_settings = settings;
	}

	public void Analyze(IMod mod, IModUtil modUtil)
	{
		_modCollection.CheckAndAdd(mod);

		if (IsForbidden(mod))
		{
			modUtil.SetIncluded(mod, false);
			modUtil.SetEnabled(mod, false);
		}
		else if (IsPseudoMod(mod) && _settings.UserSettings.HidePseudoMods)
		{
			modUtil.SetIncluded(mod, true);
			modUtil.SetEnabled(mod, true);
		}
	}

	public bool IsRequired(IMod mod, IModUtil modUtil)
	{
		var list = _modCollection.GetCollection(mod, out var collection);

		if (!(collection?.Required ?? false) || list is null)
		{
			return false;
		}

		foreach (var modItem in list)
		{
			if (modItem != mod && modUtil.IsIncluded(modItem) && modUtil.IsEnabled(modItem))
			{
				return false;
			}
		}

		return true;
	}

	public bool IsForbidden(IMod mod)
	{
		var list = _modCollection.GetCollection(mod, out var collection);

		if (!(collection?.Forbidden ?? false) || list is null)
		{
			return false;
		}

		return true;
	}

	public void ModRemoved(IMod mod)
	{
		_modCollection.RemoveMod(mod);
	}

	public void ApplyRequiredStates(IModUtil modUtil)
	{
		foreach (var item in _modCollection.Collections)
		{
			if (item.Any(mod => modUtil.IsIncluded(mod) && modUtil.IsEnabled(mod)))
			{
				continue;
			}

			modUtil.SetIncluded(item[0], true);
			modUtil.SetEnabled(item[0], true);
		}
	}

	public bool IsPseudoMod(IPackage package)
	{
		if (package is ILocalPackage localPackage && CrossIO.FileExists(CrossIO.Combine(localPackage.Folder, "ThemeMix.xml")))
		{
			return true;
		}

		if (package.GetCompatibilityInfo().Info?.Type is not PackageType.GenericPackage and not PackageType.MusicPack and not PackageType.CSM and not PackageType.ContentPackage)
		{
			return true;
		}

		return false;
	}

	public bool AreMultipleSkyvesPresent()
	{
		return (_modCollection.GetCollection(Skyve_ASSEMBLY, out _)?.Count ?? 0) + (_modCollection.GetCollection(LOM1_ASSEMBLY, out _)?.Count ?? 0) + (_modCollection.GetCollection(LOM2_ASSEMBLY, out _)?.Count ?? 0) > 1;
	}
}
