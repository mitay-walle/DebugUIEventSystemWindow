using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

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

			eSystem = FindAnyObjectByType<EventSystem>();
		}

		private void OnGUI()
		{
			STYLE = new GUIStyle(UnityEngine.GUI.skin.label)
			{
				richText = true,
			};

			EditorGUILayout.ObjectField(EventSystem.current, typeof(EventSystem), true);

			EditorGUILayout.ObjectField(EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null, typeof(GameObject), true);

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
                content = eSystem.ToString().Replace("(UnityEngine.GameObject)", string.Empty)
                    .Replace("(UnityEngine.Camera)", string.Empty);
            }
#else
			content = eSystem.ToString().Replace("(UnityEngine.GameObject)", string.Empty).Replace("(UnityEngine.Camera)", string.Empty);
#endif

			Repaint();
		}

   #if ENABLE_INPUT_SYSTEM
        private void ProcessInputSystem(InputSystemUIInputModule module)
        {
            if (!module) return;
            //try
            {
                var sb = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType());
                sb.AppendLine();
                var type = module.GetType().Assembly.GetType("UnityEngine.InputSystem.UI.PointerModel");
                var field = type.GetField("eventData");
                var pointers =
 (IEnumerable)module.GetType().GetField("m_PointerStates",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(module);
                foreach (var pointer in pointers)
                {
                    ExtendedPointerEventData eventData = field.GetValue(pointer) as ExtendedPointerEventData;
                    if (eventData == null)
                        continue;
                    sb.AppendLine("<B>Pointer:</b> " + eventData.touchId);
                    sb.AppendLine(eventData.ToString());
                }

                content = sb.ToString();
            }
            //catch (Exception e)
            {
            }
        }
#endif

		[MenuItem("Window/" + TITLE)]
		public static void OpenWindow()
		{
			var window = GetWindow<DebugUIEventSystemWindow>(TITLE);

			if (!window) window = CreateInstance<DebugUIEventSystemWindow>();
			window.ShowUtility();
		}
	}
}
