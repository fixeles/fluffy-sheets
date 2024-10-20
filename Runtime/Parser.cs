using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FPS.Sheets
{
	public static class Parser
	{
		public static void ParseSheet<T>(DTOStorage dtoStorage, string sheetJson) where T : ISheetDTO
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
				dtoStorage.Add(dto);
			}
		}

		public static void GetValue<T>(object rawValue, out T target, params JsonConverter[] converters)
		{
			target = default;

			if (rawValue == null)
				return;

			if (rawValue is string str)
			{
				if (str[0] is '{' or '[' && str[^1] is '}' or ']')
				{
					target = JsonConvert.DeserializeObject<T>(str, converters);
					return;
				}
			}

			target = target switch
			{
				string => (T)(object)rawValue.ToString(),
				_ => (T)Convert.ChangeType(rawValue, typeof(T))
			};
		}
	}
}