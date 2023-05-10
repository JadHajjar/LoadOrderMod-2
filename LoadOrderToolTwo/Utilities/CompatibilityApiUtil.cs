﻿using Extensions;
using LoadOrderToolTwo.Domain.Compatibility;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoadOrderToolTwo.Utilities;
internal class CompatibilityApiUtil
{
	internal static async Task<bool> IsCommunityManager(ulong steamId)
	{
		return await Get<bool>("/IsCommunityManager", ("steamId", Encryption.Encrypt(steamId.ToString(), KEYS.SALT)));
	}

	internal static async Task<CompatibilityData?> Catalogue()
	{
		return await Get<CompatibilityData>("/Catalogue");
	}

	internal static async Task<CompatibilityData?> Catalogue(object steamId)
	{
		return await Get<CompatibilityData>("/Catalogue", ("steamId", steamId));
	}

	internal static async Task<string?> SaveEntry(PostPackage package)
	{
		return await Post<PostPackage, string>("/SaveEntry", package);
	}

	internal static async Task<T?> Get<T>(string url, params (string, object)[] queryParams)
	{
		using var httpClient = new HttpClient();

		httpClient.DefaultRequestHeaders.Add("API_KEY", KEYS.API_KEY);

		if (queryParams.Length > 0)
		{
			var query = queryParams.Select(x => $"{Uri.EscapeDataString(x.Item1)}={Uri.EscapeDataString(x.Item2.ToString())}");

			url += "?" + string.Join("&", query);
		}

		var httpResponse = await httpClient.GetAsync(KEYS.API_URL + url);

		if (httpResponse.IsSuccessStatusCode)
		{
			var response = await httpResponse.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(response);
		}

		return default;
	}

	internal static async Task<T?> Post<TBody, T>(string url, TBody body, params (string, object)[] queryParams)
	{
		using var httpClient = new HttpClient();

		httpClient.DefaultRequestHeaders.Add("API_KEY", KEYS.API_KEY);

		if (queryParams.Length > 0)
		{
			var query = queryParams.Select(x => $"{Uri.EscapeDataString(x.Item1)}={Uri.EscapeDataString(x.Item2.ToString())}");

			url += "?" + string.Join("&", query);
		}

		var json = JsonConvert.SerializeObject(body);
		var content = new StringContent(json, Encoding.UTF8, "application/json");
		var httpResponse = await httpClient.PostAsync(url, content);

		if (httpResponse.IsSuccessStatusCode)
		{
			var response = await httpResponse.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<T>(response);
		}

		return default;
	}
}
