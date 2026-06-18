using UnityEditor;

namespace Plugins.Editor
{
	public static class DebugUIEventSystemWindowMenuItem
	{
		[MenuItem("Window/" + DebugUIEventSystemWindow.TITLE)]
		public static void OpenWindow()
		{
			EditorWindow.GetWindow<DebugUIEventSystemWindow>(DebugUIEventSystemWindow.TITLE).ShowUtility();
		}
	}
}