using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace FPS.Sheets
{
	public static class SheetsApi
	{
		private static SheetsConfig _sheetsConfig;

		private static SheetsConfig SheetsConfig
		{
			get
			{
				if (_sheetsConfig == null)
				{
					var asset = Resources.Load<SheetsConfig>(nameof(SheetsConfig));
					_sheetsConfig = asset;
					Resources.UnloadAsset(asset);
				}

				return _sheetsConfig;
			}
		}

		/// <summary>
		/// First list with compressed data
		/// </summary>
		private static string CsvURL =>
			$"https://docs.google.com/spreadsheets/d/{SheetsConfig.SheetID}/export?format=csv";

		private static string SheetInfoURL =>
			$"https://sheets.googleapis.com/v4/spreadsheets/{SheetsConfig.SheetID}?key={SheetsConfig.APIKey}";

		private static string GetListURL(string listName) =>
			$"https://sheets.googleapis.com/v4/spreadsheets/{SheetsConfig.SheetID}/values/{listName}?key={SheetsConfig.APIKey}";


		public static async UniTask<string> LoadEncodedData()
		{
			var req = await UnityWebRequest.Get(CsvURL).SendWebRequest();
			return req.downloadHandler.text;
		}

		public static async UniTask LoadEachDTO(Dictionary<string, string> result)
		{
			foreach (var dtoType in Utils.Reflection.FindAllDerivedTypes(typeof(ISheetDTO)))
			{
				var request = UnityWebRequest.Get(GetListURL(dtoType.Name));
				await request.SendWebRequest();
				result.Add(dtoType.Name, request.downloadHandler.text);
			}
		}

		public static async UniTask LoadEachSheet(Dictionary<string, string> result)
		{
			Dictionary<string, string> sheetGIDs = new();

			UnityWebRequest request = UnityWebRequest.Get(SheetInfoURL);
			await request.SendWebRequest();

			var data = JObject.Parse(request.downloadHandler.text);
			foreach (var sheet in data["sheets"]!)
			{
				var properties = sheet["properties"];
				var sheetName = properties!["title"]!.ToString();

				if (sheetName is "Encoded" or "Local")
					continue;

				sheetGIDs.Add(sheetName, properties!["sheetId"]!.ToString());
			}

			foreach (var kvp in sheetGIDs)
			{
				request = UnityWebRequest.Get(GetListURL(kvp.Key));
				await request.SendWebRequest();
				result.Add(kvp.Key, request.downloadHandler.text);
			}
		}
	}
}