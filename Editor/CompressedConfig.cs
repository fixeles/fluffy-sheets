using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace FPS.Sheets.Editor
{
	public static class CompressedConfig
	{
		[MenuItem("FPS/Generate Compressed Config")]
		private static void Generate()
		{
			GenerateAsync(true).Forget();
		}

		public static async UniTask GenerateAsync(bool copyToBuffer = false)
		{
			var startTime = Time.time;

			Dictionary<string, string> sheetGIDs = new();
			Dictionary<string, string> sheetJsons = new();

			UnityWebRequest request = UnityWebRequest.Get(SheetsApi.SheetInfoURL);
			await request.SendWebRequest();

			var data = JObject.Parse(request.downloadHandler.text);
			foreach (var sheet in data["sheets"]!)
			{
				var properties = sheet["properties"];
				var sheetName = properties!["title"]!.ToString();

				if (sheetName is "Compressed" or "Local")
					continue;

				sheetGIDs.Add(sheetName, properties!["sheetId"]!.ToString());
			}

			foreach (var kvp in sheetGIDs)
			{
				request = UnityWebRequest.Get(SheetsApi.GetListURL(kvp.Key));
				await request.SendWebRequest();
				sheetJsons.Add(kvp.Key, request.downloadHandler.text);
			}

			string rawJson = JsonConvert.SerializeObject(sheetJsons);
			var encodedJson = GZip.Encode(rawJson);

			if (copyToBuffer) 
				EditorGUIUtility.systemCopyBuffer = encodedJson;

			string resourcesPath = Application.dataPath + "/FPS/Resources";
			string filePath = Path.Combine(resourcesPath, "CachedSheetsData.txt");
			await File.WriteAllTextAsync(filePath, encodedJson);
			Debug.Log($"Data received in {Time.time - startTime:0.0} " +
			          $"sec. Use Ctrl + V \nDefault config file was created: {filePath}");
		}
	}
}