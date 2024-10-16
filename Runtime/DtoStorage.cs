using System;
using System.Collections.Generic;

namespace FPS.Sheets
{
	public class DtoStorage
	{
		private readonly Dictionary<Type, object> _dtoByType = new();

		public bool Has<T>(string key) where T : ISheetDTO => GetGenericStorage<T>().HasKey(key);

		private GenericDTOStorage<T> GetGenericStorage<T>() where T : ISheetDTO
		{
			var type = typeof(T);
			if (_dtoByType.TryGetValue(type, out var value))
				return value as GenericDTOStorage<T>;

			var storage = new GenericDTOStorage<T>();
			_dtoByType.Add(type, storage);
			return _dtoByType[type] as GenericDTOStorage<T>;
		}

		public void Add<T>(T obj) where T : ISheetDTO => GetGenericStorage<T>().Add(obj);

		public T GetDTO<T>(string key) where T : ISheetDTO => GetGenericStorage<T>().GetDTO(key);

		public T[] GetAll<T>() where T : ISheetDTO => GetGenericStorage<T>().GetAll();

		// public T[] GetByContains<T>(string content) where T : ISheetDTO
		// {
		// 	var allObjects = GetGenericStorage<T>().GetAll();
		// 	var filteredObjects = allObjects.Where(obj => obj.Id.Contains(content)).ToArray();
		// 	return filteredObjects;
		// }

		public T GetSingle<T>() where T : ISheetDTO => GetGenericStorage<T>().GetAll()[0];

		public int GetIndex<T>(string id) where T : ISheetDTO
		{
			var array = GetAll<T>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Id == id)
					return i;
			}

			return -1;
		}
	}
}