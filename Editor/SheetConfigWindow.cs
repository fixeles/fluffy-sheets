using UnityEditor;

namespace FPS.Sheets.Editor
{
	public class SheetConfigWindow
	{
		[MenuItem("FPS/Sheet Config")]
		private static void ShowWindow()
		{
			var description = UnityEngine.Resources.Load<SheetsConfig>(nameof(SheetsConfig));
			EditorUtility.OpenPropertyEditor(description);
		}
	}
}