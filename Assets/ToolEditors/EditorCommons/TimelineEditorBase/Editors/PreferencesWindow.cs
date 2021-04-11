using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UnityEditor.TimelineEditor
{
    public class PreferencesWindow : PopupWindowContent
    {
        private static System.Action onDraw;
        private static Rect myRect;
        Vector2 scorllPos;

        public static void Show(Rect rect, System.Action onDraw = null)
        {
            PreferencesWindow.onDraw = onDraw;
            myRect = rect;
            PopupWindow.Show(new Rect(rect.x, rect.y, 0, 0), new PreferencesWindow());
        }

        public override Vector2 GetWindowSize() { return new Vector2(myRect.width, myRect.height); }
        public override void OnGUI(Rect rect)
        {

            GUILayout.BeginVertical("box");
            using (var sc=new GUILayout.ScrollViewScope(scorllPos))
            {
                EditorTools.DrawTitle("Preferences");
                GUILayout.BeginVertical("box");
                Prefs.timeStepMode = (Prefs.TimeStepMode)EditorGUILayout.EnumPopup("Time Step Mode", Prefs.timeStepMode);
                if (Prefs.timeStepMode == Prefs.TimeStepMode.Seconds)
                {
                    Prefs.snapInterval = EditorTools.CleanPopup<float>("Working Snap Interval", Prefs.snapInterval, Prefs.snapIntervals.ToList());
                }
                else
                {
                    Prefs.frameRate = EditorTools.CleanPopup<int>("Working Frame Rate", Prefs.frameRate, Prefs.frameRates.ToList());
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                Prefs.clipBasicColor = EditorGUILayout.ColorField("Clip的基础色", Prefs.clipBasicColor);
                Prefs.scrollWheelZooms = EditorGUILayout.Toggle("Scroll Wheel Zooms", Prefs.scrollWheelZooms);
                if (onDraw != null)
                {
                    onDraw.Invoke();
                }
                GUILayout.EndVertical();
                scorllPos = sc.scrollPosition;
            }
            GUILayout.EndVertical();

        }
    }
}

