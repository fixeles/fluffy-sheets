using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace FPS.Sheets.Editor
{
	public static class SheetsEncoder
	{
		[MenuItem("FPS/Encode Config")]
		private static void Generate()
		{
			GenerateAsync().Forget();
		}

		private static async UniTask GenerateAsync()
		{
			var startTime = Time.time;
			Dictionary<string, string> sheetJsons = new();
			await SheetsApi.LoadEachDTO(sheetJsons);
			string rawJson = JsonConvert.SerializeObject(sheetJsons);
			var encodedJson = GZip.Encode(rawJson);

			EditorGUIUtility.systemCopyBuffer = encodedJson;
			
			//cache default config
			string resourcesPath = Application.dataPath + "/FPS/Resources";
			string filePath = Path.Combine(resourcesPath, "CachedSheetsConfig.txt");
			await File.WriteAllTextAsync(filePath, encodedJson);
			Debug.Log($"Data received in {Time.time - startTime:0.0} " +
			          $"sec. Use Ctrl + V \nDefault config file was created: {filePath}");
		}
	}
}