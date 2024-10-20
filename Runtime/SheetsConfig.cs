using UnityEngine;

namespace FPS.Sheets
{
	public class SheetsConfig : ScriptableObject
	{
		[field: SerializeField] public string APIKey { get; private set; }

		[field: SerializeField] public string SheetID { get; private set; }

#if UNITY_EDITOR
		[field: SerializeField] public SheetLoadType LoadType { get; private set; }

		public enum SheetLoadType
		{
			Encoded,
			EachSheet
		}

		private class Creator : ScriptableObjectCreator<SheetsConfig>
		{
			[UnityEditor.InitializeOnLoadMethod]
			private static void Create()
			{
				TryCreate();
			}
		}
#endif
	}
}