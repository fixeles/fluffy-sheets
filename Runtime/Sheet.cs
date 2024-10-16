using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FPS.Sheets
{
	public class Sheet
	{
		private readonly Dictionary<string, int> _columnIndices = new();
		private readonly string[,] _tab;

		public Sheet(JArray valuesArray)
		{
			var headers = JsonConvert.DeserializeObject<object[]>(valuesArray[0].ToString());
			_tab = new string[headers.Length, valuesArray.Count - 1];

			for (int i = 0; i < headers.Length; i++)
			{
				_columnIndices.Add(headers[i].ToString(), i);
			}

			for (int y = 1; y < valuesArray.Count - 1; y++)
			{
				var array = JsonConvert.DeserializeObject<object[]>(valuesArray[y + 1].ToString());
				for (int x = 0; x < array.Length; x++)
				{
					_tab[x, y] = array[x].ToString();
				}
			}
		}

		public string GetCellValue(string columnName, int rawIndex) => _tab[_columnIndices[columnName], rawIndex];
	}
}