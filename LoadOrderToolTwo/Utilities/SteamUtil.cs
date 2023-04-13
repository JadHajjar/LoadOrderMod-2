using Extensions;

using LoadOrderShared;

using LoadOrderToolTwo.Domain.Interfaces;
using LoadOrderToolTwo.Domain.Steam;
using LoadOrderToolTwo.Utilities.IO;
using LoadOrderToolTwo.Utilities.Managers;

using Newtonsoft.Json;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadOrderToolTwo.Utilities;

public static class SteamUtil
{
	private const string STEAM_CACHE_FILE = "SteamModsCache.json";
	private const string DLC_CACHE_FILE = "SteamDlcsCache.json";
	private static readonly CSCache _csCache;

	public static List<SteamDlc> Dlcs { get; private set; }

	public static event Action? DLCsLoaded;

	static SteamUtil()
	{
		_csCache = CSCache.Deserialize();

		ISave.Load(out List<SteamDlc>? cache, DLC_CACHE_FILE);

		Dlcs = cache ?? new();
	}

	public static bool IsSteamAvailable() => LocationManager.FileExists(LocationManager.SteamPathWithExe);

	private static void SaveCache(Dictionary<ulong, SteamWorkshopItem> list)
	{
		var cache = GetCachedInfo() ?? new();

		foreach (var item in list)
		{
			cache[item.Key] = item.Value;
		}

		ISave.Save(cache, STEAM_CACHE_FILE);
	}

	internal static Dictionary<ulong, SteamWorkshopItem>? GetCachedInfo()
	{
		try
		{
			var path = ISave.GetPath(STEAM_CACHE_FILE);

			if (DateTime.Now - File.GetLastWriteTime(path) > TimeSpan.FromDays(1.5) && ConnectionHandler.IsConnected)
			{
				return null;
			}

			ISave.Load(out Dictionary<ulong, SteamWorkshopItem>? dic, STEAM_CACHE_FILE);

			return dic;
		}
		catch { return null; }
	}

	public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
	{
		while (source.Any())
		{
			yield return source.Take(chunkSize);
			source = source.Skip(chunkSize);
		}
	}

	public static async Task<Dictionary<T1, T2>> ConvertInChunks<T1, T2>(IEnumerable<T1> mainList, int chunkSize, Func<List<T1>, Task<Dictionary<T1, T2>>> converter)
	{
		var chunks = mainList.Chunk(chunkSize); // Split mainList into chunks of chunkSize
		var tasks = new List<Task<Dictionary<T1, T2>>>(); // A list of tasks to convert the chunks
		var results = new Dictionary<T1, T2>(); // A dictionary to store the results

		foreach (var chunk in chunks)
		{
			tasks.Add(converter(chunk.ToList())); // Convert the current chunk and add the task to the list
		}

		await Task.WhenAll(tasks); // Wait for all tasks to complete

		foreach (var task in tasks)
		{
			var chunkResults = await task; // Get the results of the completed task

			foreach (var kvp in chunkResults)
			{
				results[kvp.Key] = kvp.Value; // Add the results to the dictionary
			}
		}

		return results;
	}

	public static void ExecuteSteam(string args)
	{
		var file = LocationManager.SteamPathWithExe;

		if (LocationManager.Platform is Platform.Windows)
		{
			var process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(file));

			if (process.Length == 0)
			{
				Notification.Create("", "",PromptIcons.Info, null).Show(Program.MainForm, 10);
			}
		}

		IOUtil.Execute(file, args);
	}

	public static async Task<Dictionary<string, SteamUserEntry>> GetSteamUsers(List<string> steamId64s)
	{
		var idString = string.Join(",", steamId64s);
		var url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=706303B24FA0E63B1FB25965E081C2E1&steamids={idString}";

		try
		{
			using var httpClient = new HttpClient();
			var httpResponse = await httpClient.GetAsync(url);

			if (httpResponse.IsSuccessStatusCode)
			{
				var response = await httpResponse.Content.ReadAsStringAsync();

				return Newtonsoft.Json.JsonConvert.DeserializeObject<SteamUserRootResponse>(response)?.response.players.ToDictionary(x => x.steamid) ?? new();
			}

			Log.Error("failed to get steam author data: " + httpResponse.RequestMessage);
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "Failed to get steam author information");
		}

		return new();
	}

	public static async Task<Dictionary<ulong, SteamWorkshopItem>> GetWorkshopInfoAsync(ulong[] ids)
	{
		var results = await ConvertInChunks(ids, 1000, GetWorkshopInfoImplementationAsync);

		SaveCache(results);

		return results;
	}

	private static async Task<Dictionary<ulong, SteamWorkshopItem>> GetWorkshopInfoImplementationAsync(List<ulong> ids)
	{
		var url = @"https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

		ids.Remove(0);

		var bodyDictionary = new Dictionary<string, string>
		{
			["itemcount"] = ids.Count.ToString()
		};

		for (var i = 0; i < ids.Count; ++i)
		{
			bodyDictionary[$"publishedfileids[{i}]"] = ids[i].ToString();
		}

		try
		{
			using var httpClient = new HttpClient();
			var body = new FormUrlEncodedContent(bodyDictionary);
			var httpResponse = await httpClient.PostAsync(url, body);

			if (httpResponse.IsSuccessStatusCode)
			{
				var response = await httpResponse.Content.ReadAsStringAsync();

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SteamWorkshopItemRootResponse>(response)?.response.publishedfiledetails
					.Where(x => x.result is 1 or 17 or 86 or 9)
					.Select(x => new SteamWorkshopItem(x))
					.ToList() ?? new();

				var users = await ConvertInChunks(data.Select(x => x.AuthorID ?? string.Empty).Distinct(), 100, GetSteamUsers);

				foreach (var item in data)
				{
					if (!string.IsNullOrEmpty(item.AuthorID) && users.ContainsKey(item.AuthorID!))
					{
						item.Author = new(users[item.AuthorID!]);
					}
				}

				foreach (var item in data.Where(x => x.Private))
				{
					var info = await GetUnlistedWorkshopEntryAsync("https://steamcommunity.com/workshop/filedetails?id="+item.SteamId);

					if (info.Item1 is not null)
					{
						item.Title = info.Item1;
						item.PreviewURL = info.Item2;
					}
				}

				return data.ToDictionary(x => ulong.Parse(x.PublishedFileID));
			}

			Log.Error("failed to get steam data: " + httpResponse.RequestMessage);
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "Failed to get steam information");
		}

		return new();
	}

	public static async Task<(string?, string?)> GetUnlistedWorkshopEntryAsync(string entryUrl)
	{
		using var client = new HttpClient();
		var response = await client.GetAsync(entryUrl);
		var entryHtml = await response.Content.ReadAsStringAsync();

		// Extract the entry name from the HTML using a regular expression
		var nameMatch = Regex.Match(entryHtml, "<div class=\"workshopItemTitle\">\\s*(.*?)\\s*</div>");
		var entryName = nameMatch.Success ? nameMatch.Groups[1].Value : null;

		// Extract the thumbnail URL from the HTML using a regular expression
		var thumbnailMatch = Regex.Match(entryHtml, "<img .+? class=\"workshopItemPreviewImageMain\" src=\"(.*?)\"");
		var thumbnailUrl = thumbnailMatch.Success ? Regex.Replace(thumbnailMatch.Groups[1].Value, @"imw=\d+&imh=\d+", "imw=5000&imh=5000").Replace("letterbox=true", "letterbox=false") : null;

		return (entryName, thumbnailUrl);
	}

	public static async Task<Dictionary<ulong, SteamWorkshopItem>> GetCollectionContentsAsync(string collectionId)
	{
		var url = @"https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/";

		var bodyDictionary = new Dictionary<string, string>
		{
			["collectioncount"] = "1",
			[$"publishedfileids[0]"] = collectionId
		};

		try
		{
			using var httpClient = new HttpClient();
			var body = new FormUrlEncodedContent(bodyDictionary);
			var httpResponse = await httpClient.PostAsync(url, body);

			if (httpResponse.IsSuccessStatusCode)
			{
				var response = await httpResponse.Content.ReadAsStringAsync();

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SteamWorkshopCollectionRootResponse>(response)?.response.collectiondetails?.FirstOrDefault()?.children?
					.Where(x => x.filetype == 0)
					.Select(x => ulong.Parse(x.publishedfileid))
					.ToList() ?? new();

				if (data.Count == 0)
				{
					return new();
				}

				data.Insert(0, ulong.Parse(collectionId));

				return await GetWorkshopInfoAsync(data.ToArray());
			}

			Log.Error("failed to get steam data: " + httpResponse.RequestMessage);
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "Failed to get steam collection information");
		}

		return new();
	}

	public static async Task<Dictionary<string, SteamAppInfo>> GetSteamAppInfoAsync(uint steamId)
	{
		var url = $"https://store.steampowered.com/api/appdetails?appids={steamId}";

		try
		{
			using var httpClient = new HttpClient();
			var httpResponse = await httpClient.GetAsync(url);

			if (httpResponse.IsSuccessStatusCode)
			{
				var response = await httpResponse.Content.ReadAsStringAsync();

				return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SteamAppInfo>>(response);
			}
		}
		catch (Exception ex) { Log.Exception(ex, "Failed to get the steam information for appid " + steamId); }

		return new();
	}

	public static void ReDownload(params IPackage[] packages)
	{
		if (packages.Any(x => x.Folder.PathEquals(Path.GetDirectoryName(LocationManager.CurrentDirectory))))
		{
			if (MessagePrompt.Show(Locale.LOTWillRestart, PromptButtons.OKCancel, PromptIcons.Info, Program.MainForm) == DialogResult.Cancel)
				return;

			IOUtil.WaitForUpdate();

			Application.Exit();
		}

		ReDownload(packages.Select(x => x.SteamId).ToArray());
	}

	public static void ReDownload(params ulong[] ids)
	{
		try
		{
			var steamArguments = new StringBuilder("steam://open/console");

			if (LocationManager.Platform is not Platform.Windows)
			{
				steamArguments.Append(" \"");
			}

			for (var i = 0; i < ids.Length; i++)
			{
				steamArguments.AppendFormat(" +workshop_download_item 255710 {0}", ids[i]);
			}

			if (LocationManager.Platform is not Platform.Windows)
			{
				steamArguments.Append("\"");
			}

			ExecuteSteam(steamArguments.ToString());

			Thread.Sleep(150);

			ExecuteSteam("steam://open/downloads");
		}
		catch (Exception) { }
	}

	public static bool IsDlcInstalledLocally(uint dlcId)
	{
		return _csCache?.Dlcs?.Contains(dlcId) ?? false;
	}

	public static void SetSteamInformation(this IPackage package, SteamWorkshopItem steamWorkshopItem, bool cache)
	{
		if (package.Private = steamWorkshopItem.Private)
		{
			package.Name = steamWorkshopItem.Title ?? package.Name;

			if (package.IconUrl != steamWorkshopItem.PreviewURL)
			{
				package.IconUrl = steamWorkshopItem.PreviewURL ?? package.IconUrl;
			}

			return;
		}

		if (package.RemovedFromSteam = steamWorkshopItem.Removed)
		{
			return;
		}

		package.SteamInfoLoaded = true;
		package.Name = package.SteamId == 2040656402ul ? "Harmony" : steamWorkshopItem.Title ?? package.Name;
		package.Author = steamWorkshopItem.Author;
		package.ServerTime = steamWorkshopItem.UpdatedUTC;
		package.WorkshopTags = steamWorkshopItem.Tags;
		package.ServerSize = steamWorkshopItem.Size;
		package.SteamDescription = steamWorkshopItem.Description;

		if (package.IconUrl != steamWorkshopItem.PreviewURL)
		{
			package.IconUrl = steamWorkshopItem.PreviewURL;
		}

		if (!cache)
		{
			package.Status = ModsUtil.GetStatus(package, out var reason);
			package.StatusReason = reason;

			CentralManager.InformationUpdate(package);
		}
	}

	internal static void LoadDlcs()
	{
		new BackgroundAction("Loading DLCs", async () =>
		{
			Log.Info($"Loading DLCs..");

			var dlcs = await GetSteamAppInfoAsync(255710);

			if (!dlcs.ContainsKey("255710"))
			{
				Log.Info($"Failed to load DLCs, steam info returned invalid content..");
				return;
			}

			var newDlcs = new List<SteamDlc>(Dlcs);

			foreach (var dlc in dlcs["255710"].data.dlc.Where(x => !Dlcs.Any(y => y.Id == x)))
			{
				var data = await GetSteamAppInfoAsync(dlc);

				if (data.ContainsKey(dlc.ToString()))
				{
					var info = data[dlc.ToString()].data;

					newDlcs.Add(new SteamDlc
					{
						Id = dlc,
						Name = info.name,
						Description = info.short_description,
						ReleaseDate = DateTime.TryParseExact(info.release_date?.date, "dd MMM, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DateTime.MinValue
					});
				}
			}

			Log.Info($"DLCs ({newDlcs.Count}) loaded..");

			ISave.Save(Dlcs = newDlcs, DLC_CACHE_FILE);

			DLCsLoaded?.Invoke();

			AssetsUtil.SetAvailableDlcs(Dlcs.Select(x => x.Id));

			foreach (var dlc in Dlcs)
			{
				await ImageManager.Ensure(dlc.ThumbnailUrl, false, $"{dlc.Id}.png", false);

				DLCsLoaded?.Invoke();
			}
		}).Run();
	}
}