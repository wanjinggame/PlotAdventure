using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UnityEditor.TimelineEditor
{
    public class InspectorWindow : PopupWindowContent
    {
        public static InspectorWindow inspectorWindow;
        private IEditor target;
        private Rect myRect;

        public bool enable;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(myRect.width, myRect.height);
        }

        public static void Show(PlayableNode node, Rect rect, bool enable = true, ShowMode showMode = ShowMode.NormalWindow)
        {
            if (node == null || !Prefs.showPopupWindow) return;
            if (inspectorWindow == null) inspectorWindow = new InspectorWindow();
            var editor = EditorsUtlity.GetEditor(node.GetType());
            if (editor != null)
            {
                inspectorWindow.target = editor;
                inspectorWindow.target.SetTarget(node);
            }
            else
            {
                inspectorWindow.target = null;
                return;
            }
            inspectorWindow.myRect = rect;
            inspectorWindow.enable = enable;
            ReflectionTools.ShowPopupWindow(new Rect(rect.x, rect.y, 0, 0), inspectorWindow, showMode);
        }

        public override void OnClose()
        {
            inspectorWindow = null;
            GUIUtility.hotControl = 0;
        }

        public static void Close()
        {
            if (inspectorWindow != null && inspectorWindow.editorWindow != null)
            {
                inspectorWindow.editorWindow.Close();
            }
            inspectorWindow = null;
        }

        Vector2 pos;
        bool flag = false;
        public override void OnGUI(Rect rect)
        {
            if (target == null) return;
            if (!flag && GUIUtility.hotControl != 0)
            {
                flag = true;
                GUIUtility.hotControl = 0;
            }
            var e = GUI.enabled;
            GUI.enabled = enable;
            target.Draw(rect);
            GUI.enabled = e;
        }
    }
}


