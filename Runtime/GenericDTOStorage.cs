using System.Collections.Generic;
using System.Linq;

namespace FPS.Sheets
{
	public class GenericDTOStorage<T> where T : ISheetDTO
	{
		private readonly Dictionary<string, T> _dict = new();

		public T GetDTO(string key) => _dict.ContainsKey(key) ? default : _dict[key];
		public T[] GetAll() => _dict.Values.ToArray();
		public string[] GetAllKeys() => _dict.Keys.ToArray();
		public bool HasKey(string key) => _dict.ContainsKey(key);

		public bool IsEmpty() => _dict.Count == 0;

		public void Clear() => _dict.Clear();
		public void Add(T dto) => _dict.Add(dto.Id, dto);
	}
}