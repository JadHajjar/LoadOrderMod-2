﻿using SkyveApp.Domain.Systems;

namespace SkyveApp.Domain.Steam;

internal class SteamPackageRequirement : IPackageRequirement
{

	public SteamPackageRequirement(ulong id, bool optional)
	{
		Id = id;
		Optional = optional;
		Url = $"https://steamcommunity.com/workshop/filedetails/?id={Id}";
	}

	public bool Optional { get; }
	public ulong Id { get; }
	public string? Url { get; }
	public string Name => this.GetWorkshopInfo()?.Title ?? Id.ToString();
}