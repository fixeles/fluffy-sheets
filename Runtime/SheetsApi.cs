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
		private readonly DtoStorage _dtoStorage;
		private const string ApiURL = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}?key={2}";
		private const string CsvURL = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv";

		public SheetsApi(DtoStorage dtoStorage)
		{
			_dtoStorage = dtoStorage;
		}

		public static string SheetInfoURL
		{
			get
			{
				var config = Resources.Load<SheetsConfig>(nameof(SheetsConfig));
				var result = $"https://sheets.googleapis.com/v4/spreadsheets/{config.SheetID}?key={config.APIKey}";
				Resources.UnloadAsset(config);
				return result;
			}
		}

		public static string GetListURL(string listName)
		{
			var config = Resources.Load<SheetsConfig>(nameof(SheetsConfig));
			var result = string.Format(ApiURL, config.SheetID, listName, config.APIKey);
			Resources.UnloadAsset(config);
			return result;
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

		public static void GetValue<T>(object rawValue, out T target)
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
	}
}