using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Plugins.Editor
{
	[EditorWindowTitle(title = TITLE)]
	public sealed class DebugUIEventSystemWindow : EditorWindow
	{
		public const string TITLE = "Debug UI.EventSystem";
		private static GUIStyle STYLE = new GUIStyle();

		private string content;
		private EventSystem eSystem;
		private Vector2 _scroll;

		private void OnEnable() => titleContent = new GUIContent(TITLE);

		private void FindSystem()
		{
			if (eSystem) return;
			eSystem = FindObjectOfType<EventSystem>();
		}

		private void OnGUI()
		{
			STYLE = new GUIStyle(GUI.skin.label)
			{
				richText = true,
			};

			EditorGUILayout.ObjectField(EventSystem.current, typeof(EventSystem), true);

			EditorGUILayout.ObjectField(
				EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null,
				typeof(GameObject),
				true);

			_scroll = GUILayout.BeginScrollView(_scroll);
			GUILayout.Label(content, STYLE);
			GUILayout.EndScrollView();
		}

		private void Update()
		{
			FindSystem();

			if (!eSystem)
			{
				content = "<b>EventSystem не найдена! </b>";
				return;
			}

			#if ENABLE_INPUT_SYSTEM
			if (eSystem.TryGetComponent<InputSystemUIInputModule>(out var module))
			{
				ProcessInputSystem(module);
			}
			else
			{
				content = eSystem.ToString().Replace("(UnityEngine.GameObject)", string.Empty).Replace("(UnityEngine.Camera)", string.Empty);
			}
			#else
			content = eSystem.ToString().Replace("(UnityEngine.GameObject)", string.Empty).Replace("(UnityEngine.Camera)", string.Empty);
			#endif

			Repaint();
		}

		private void ProcessInputSystem(InputSystemUIInputModule module)
		{
			var sb = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType());
			sb.AppendLine();
			foreach (var pointer in module.m_PointerStates)
			{
				if (pointer.eventData == null)
					continue;
				sb.AppendLine("<B>Pointer:</b> " + pointer.eventData.touchId);
				sb.AppendLine(pointer.eventData.ToString());
			}
			content = sb.ToString();
		}

		[MenuItem("Window/" + TITLE)]
		public static void OpenWindow()
		{
			var window = GetWindow<DebugUIEventSystemWindow>(TITLE);

			if (!window) window = CreateInstance<DebugUIEventSystemWindow>();
			window.ShowUtility();
		}
	}
}