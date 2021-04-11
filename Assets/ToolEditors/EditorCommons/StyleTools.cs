#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEditor
{
    public class StyleTools
    {
        static StyleTools()
        {
            InitStyle();
        }

        private static readonly string skinPath = "Assets/ToolEditors/EditorCommons/Editor/Asset/EditorSkin.guiskin";

        private static GUISkin skin;
        public static GUISkin Skin
        {
            get
            {
                if (skin == null)
                {
                    skin = AssetDatabase.LoadAssetAtPath<GUISkin>(skinPath);
                }
                return skin;
            }
        }

        private readonly static Dictionary<string, GUIStyle> internalStyles = new Dictionary<string, GUIStyle>();
        private static Dictionary<string, GUIStyle> InternalStyles
        {
            get
            {
                if (internalStyles == null || internalStyles.Count == 0)
                {
                    foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        object o = fi.GetValue(null, null);
                        if (o.GetType() == typeof(GUIStyle))
                        {
                            if (!internalStyles.ContainsKey((o as GUIStyle).name))
                            {
                                internalStyles.Add((o as GUIStyle).name, o as GUIStyle);
                            }
                        }
                    }
                }
                return internalStyles;
            }
        }

        private readonly static Dictionary<string, Texture2D> textrue2Ds = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> Textrue2Ds
        {
            get
            {
                if (textrue2Ds == null || textrue2Ds.Count == 0)
                {
                    Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
                    foreach (Texture2D x in t)
                    {
                        Debug.unityLogger.logEnabled = false;
                        GUIContent gc = EditorGUIUtility.IconContent(x.name);
                        Debug.unityLogger.logEnabled = true;
                        if (gc != null && gc.image != null)
                        {
                            if (!textrue2Ds.ContainsKey(x.name))
                            {
                                textrue2Ds.Add(x.name, x);
                            }
                        }
                    }
                }
                return textrue2Ds;
            }
        }

        public static GUIStyle GetGUIStyle(string styleName)
        {
            if (InternalStyles.ContainsKey(styleName))
            {
                return InternalStyles[styleName];
            }
            return EditorStyles.label;
        }

        public static GUIStyle ConfigStyle;

        public static GUIStyle FloderStyle;

        public static GUIStyle DotToggleStyle;

        public static GUIStyle MiddleCenterLab;

        public static Texture2D GetTexture2D(string name)
        {
            if (Textrue2Ds.ContainsKey(name))
            {
                return Textrue2Ds[name];
            }
            else
            {
                Texture2D textrue = EditorGUIUtility.FindTexture(name);
                if (textrue != null)
                {
                    Textrue2Ds.Add(name, textrue);
                    return textrue;
                }
            }
            return null;
        }

        public static Texture2D gearIcon;

        public static void InitStyle()
        {
            textrue2Ds.Clear();
            ConfigStyle = new GUIStyle();
            ConfigStyle.normal.background = GetTexture2D("d__Popup");

            FloderStyle = new GUIStyle();
            FloderStyle.normal.background = GetTexture2D("Folder Icon");
            FloderStyle.active.background = GetTexture2D("FolderEmpty Icon");

            DotToggleStyle = new GUIStyle(Skin.toggle);
            InitToggleStyle(DotToggleStyle, "radio", "radio on");

            MiddleCenterLab = new GUIStyle(Skin.label);
            MiddleCenterLab.alignment = TextAnchor.MiddleCenter;
            MiddleCenterLab.richText = true;

            gearIcon = (Texture2D)Resources.Load("GearIcon");

            Skin.customStyles[5].normal.background = GetTexture2D("BreadcrumbsSeparator");
            Skin.customStyles[5].active.background = GetTexture2D("BreadcrumbsSeparator");
        }

        private static void InitToggleStyle(GUIStyle style, string off, string on)
        {
            style.normal.background = GetTexture2D(off);
            style.hover.background = GetTexture2D(off);
            style.active.background = GetTexture2D(off);
            style.onActive.background = GetTexture2D(on);
            style.onNormal.background = GetTexture2D(on);
            style.onHover.background = GetTexture2D(on);
        }
    }
}

#endif
