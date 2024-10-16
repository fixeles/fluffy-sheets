using UnityEngine;

namespace FPS.Sheets
{
	public class SheetsConfig : ScriptableObject
	{
		[field: SerializeField] public string APIKey { get; private set; } = "AIzaSyA3c-QGe164O2iScqQzzczZvPWZKbv6IMc";

		[field: SerializeField]
		public string SheetID { get; private set; } = "1_wh02i8ces2mzsFZWb5jXyKLTgmbxtPptVLfxevZg9s";

#if UNITY_EDITOR
		[field: SerializeField] public SheetLoadType ReceiveType { get; private set; }

		public enum SheetLoadType
		{
			EachList,
			Compressed
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