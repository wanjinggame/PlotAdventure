#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Pangu.EditorTools
{
    public class IconWIndow : EditorWindow
    {
        [MenuItem("Window/Editor/Icons Window")]
        public static void OpenMyWindow()
        {
            EditorWindow.GetWindow<IconWIndow>("icons");
        }

        Action<string> action;

        public static void Open(Action<string> cb)
        {
            var window = EditorWindow.GetWindow<IconWIndow>("icons");
            window.action = cb;
        }

        private Vector2 m_Scroll;
        private List<string> m_Icons = null;
        void Awake()
        {
            m_Icons = new List<string>(); ;
            Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D x in t)
            {
                Debug.unityLogger.logEnabled = false;
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                Debug.unityLogger.logEnabled = true;
                if (gc != null && gc.image != null)
                {
                    m_Icons.Add(x.name);
                }
            }
        }
        void OnGUI()
        {
            if (m_Icons == null || m_Icons.Count == 0)
            {
                Awake();
            }
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            float width = 50f;
            int count = (int)(position.width / width);
            count--;
            for (int i = 0; i < m_Icons.Count; i += count)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < count; j++)
                {
                    int index = i + j;
                    if (index < m_Icons.Count)
                    {
                        if (GUILayout.Button(EditorGUIUtility.IconContent(m_Icons[index]),
                            GUILayout.Width(width), GUILayout.Height(30)))
                        {
                            Debug.Log(m_Icons[index]);
                            if (action != null)
                            {
                                action(m_Icons[index]);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    public class TestMenuEditorWindow : MenuTreeEditorWindow
    {
        public override void InitOther()
        {
            m_Icons = new List<string>(); ;
            Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D x in t)
            {
                Debug.unityLogger.logEnabled = false;
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                Debug.unityLogger.logEnabled = true;
                if (gc != null && gc.image != null)
                {
                    m_Icons.Add(x.name);
                }
            }
        }

        public override void BuildMenuTree()
        {
            AddMenuItem("Icon预览", DrawIcon);
            AddMenuItem("控件预览", EditorCommondPreview);
            AddMenuItem("空白页签", null);
            AddMenuItem("测试页签", r =>
            {
                GUI.Label(r, GUILayoutTools.GetTextSizeOf("asdasdcvasdv\nasdvasdv", 15), StyleTools.MiddleCenterLab);
            });
        }

        [MenuItem("Window/Editor/MenuEditorWindow")]
        public static void Open()
        {
            GetWindow<TestMenuEditorWindow>();
        }

        public Vector2 scrollPosition = Vector2.zero;
        bool t = false;
        bool t1 = false;
        string s = "";
        string s1 = "";
        public void EditorCommondPreview(Rect r)
        {
            if (GUILayoutTools.DrawHeader("控 件 预 览", "asc", false, false))
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, StyleTools.Skin.box);
                GUILayoutTools.NotificationTextLabel(new GUIContent("     控  件  预   览"));
                GUILayoutTools.Separator_DoubleLine();
                GUILayoutTools.TextArea("这是一个TextArea!\n" +
                    "sdfbsdvvsdvsssssssssss" +
                    "sdv" +
                    "sdv");
                GUILayoutTools.TextField("这是一个TextField!");
                GUILayoutTools.Separator_SingleLine();

                s = GUILayoutTools.PathPicker("Path Picker", s);
                if (GUILayoutTools.SerachFiled(ref s1))
                {
                    Debug.Log(s1);
                }
                GUILayoutTools.Separator_SingleLine();

                GUILayout.BeginHorizontal();
                GUILayoutTools.Button100_50(new GUIContent("100*50"));
                GUILayoutTools.ButtonFixedSizeGray_Frame(new GUIContent("Button"), 100, 50);
                GUILayoutTools.ButtonFixedSizeGray(new GUIContent("Button"), 100, 50);
                GUILayoutTools.ButtonFixedSizeYellow(new GUIContent("Button"), 100, 50);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayoutTools.Button80_40(new GUIContent("80*40"));
                GUILayoutTools.ButtonFixedSizeGray_Frame(new GUIContent("Button"), 80, 40);
                GUILayoutTools.ButtonFixedSizeGray(new GUIContent("Button"), 80, 40);
                GUILayoutTools.ButtonFixedSizeYellow(new GUIContent("Button"), 80, 40);
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayoutTools.Button("Button160*40", GUILayout.Width(160), GUILayout.Height(40));
                GUILayoutTools.ButtonFixedSizeGray_Frame(new GUIContent("Button"), 160, 40);
                GUILayoutTools.ButtonFixedSizeGray(new GUIContent("Button"), 160, 40);
                GUILayoutTools.ButtonFixedSizeYellow(new GUIContent("Button"), 160, 40);
                GUILayout.EndHorizontal();

                GUILayoutTools.Button("Button", GUILayout.Height(30));
                GUILayout.BeginHorizontal();
                GUILayoutTools.SerachButton();
                GUILayoutTools.ConfigIconButton();
                GUILayoutTools.OKButton();
                GUILayoutTools.IconButton();
                GUILayoutTools.ListButton();
                GUILayoutTools.SettingButton();
                GUILayoutTools.RefreshButton();
                GUILayoutTools.AddButton();
                GUILayoutTools.MinusButton();
                GUILayoutTools.ConfigButton();
                GUILayout.EndHorizontal();
                GUILayoutTools.Separator_SingleLine();

                t = GUILayoutTools.Toggle(t, "NormalToggle");
                t1 = GUILayoutTools.DotToggle(t1, new GUIContent("DotToggle"));
                GUILayoutTools.Separator_SingleLine();

                GUILayoutTools.WhiteBoldLabel(new GUIContent("WhiteBoldLabel"));
                GUILayoutTools.WhiteLargeLabel(new GUIContent("WhiteLargeLabel"));
                GUILayoutTools.TitleTextLabel(new GUIContent("TitleText"));
                GUILayoutTools.Separator_DoubleLine();
                GUILayout.EndScrollView();
            }
        }

        private Vector2 m_Scroll;
        private List<string> m_Icons = null;
        void DrawIcon(Rect r)
        {
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            float width = 50f;
            int count = (int)(r.width / width);
            count--;
            for (int i = 0; i < m_Icons.Count; i += count)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < count; j++)
                {
                    int index = i + j;
                    if (index < m_Icons.Count)
                    {
                        if (GUILayout.Button(EditorGUIUtility.IconContent(m_Icons[index]),
                            GUILayout.Width(width), GUILayout.Height(30)))
                        {
                            Debug.Log(m_Icons[index]);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif