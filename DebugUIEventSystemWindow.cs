using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Plugins.Editor
{
    [EditorWindowTitle(title = TITLE)]
    public class DebugUIEventSystemWindow : EditorWindow
    {
        public const string TITLE = "Debug UI.EventSystem";
        static GUIStyle STYLE = new GUIStyle();

        [MenuItem("Window/" + TITLE)]
        public static void OpenWindow()
        {
            var window = GetWindow<DebugUIEventSystemWindow>(TITLE);

            if (!window) window = CreateInstance<DebugUIEventSystemWindow>();
            window.ShowUtility();
        }

        EventSystem eSystem;
        Vector2 _scroll;

        protected void OnEnable()
        {
            titleContent = new GUIContent(TITLE);
        }

        void FindSystem()
        {
            if (eSystem) return;
            eSystem = FindObjectOfType<EventSystem>();
        }

        string content;

        protected void OnGUI()
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

        void Update()
        {
            FindSystem();

            if (!eSystem)
            {
                content = "<b>EventSystem не найдена! </b>";
                return;
            }

            content = eSystem.ToString().Replace("(UnityEngine.GameObject)", string.Empty)
                .Replace("(UnityEngine.Camera)", string.Empty);

            Repaint();
        }
    }
}
