using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace FPS.Sheets
{
	public class SheetsApi
	{
		private readonly DTOStorage _dtoStorage;
		private const string ApiURL = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}?key={2}";

		/// <summary>
		/// First list with compressed data
		/// </summary>
		private const string CsvURL = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv";

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

		public SheetsApi(DTOStorage dtoStorage)
		{
			_dtoStorage = dtoStorage;
		}

		public static string SheetInfoURL =>
			$"https://sheets.googleapis.com/v4/spreadsheets/{SheetsConfig.SheetID}?key={SheetsConfig.APIKey}";

		public static string GetListURL(string listName)
		{
			return string.Format(ApiURL, SheetsConfig.SheetID, listName, SheetsConfig.APIKey);
		}

		public static void ParseValue<T>(object rawValue, out T target)
		{
			target = default;

			if (rawValue == null)
				return;

			if (rawValue is string str && str.Trim().StartsWith("{") && str.Trim().EndsWith("}"))
			{
				target = JsonConvert.DeserializeObject<T>(str);
				return;
			}

			target = target switch
			{
				string => (T)(object)rawValue.ToString(),
				_ => (T)Convert.ChangeType(rawValue, typeof(T))
			};
		}

		public async UniTask<string> LoadCompressedData()
		{
			var req = await UnityWebRequest.Get(CsvURL).SendWebRequest();
			return req.downloadHandler.text;
		}

		public async UniTask<string> GetListData(string listName)
		{
			var req = await UnityWebRequest.Get(GetListURL(listName)).SendWebRequest();
			return req.downloadHandler.text;
		}

		public void Parse<T>(string sheetJson) where T : ISheetDTO
		{
			var raws = JsonConvert.DeserializeObject<JObject>(sheetJson)["values"] as JArray;
			var headers = JsonConvert.DeserializeObject<string[]>(raws![0].ToString());
			var dict = headers.ToDictionary(key => key.ToString(), null);

			for (int raw = 0; raw < raws.Count - 1; raw++)
			{
				var valuesRaw = JsonConvert.DeserializeObject<string[]>(raws[raw + 1].ToString());
				for (int column = 0; column < headers.Length; column++)
					dict[headers[column]] = valuesRaw[column];

				var constructor = typeof(T).GetConstructor(new[] { typeof(IDictionary) });
				var dto = (T)constructor!.Invoke(new object[] { dict });
				_dtoStorage.Add(dto);
			}
		}
	}
}