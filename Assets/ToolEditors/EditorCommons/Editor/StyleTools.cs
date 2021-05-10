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
namespace UnityEditor.Custom
{
    public static class Styling
    {
        public static readonly GUIStyle smallTickbox;

        static readonly Color splitterDark;
        static readonly Color splitterLight;

        public static Color splitter { get { return EditorGUIUtility.isProSkin ? splitterDark : splitterLight; } }

        static readonly Texture2D paneOptionsIconDark;
        static readonly Texture2D paneOptionsIconLight;

        public static Texture2D paneOptionsIcon { get { return EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight; } }

        public static readonly GUIStyle headerLabel;

        static readonly Color headerBackgroundDark;
        static readonly Color headerBackgroundLight;

        public static Color headerBackground { get { return EditorGUIUtility.isProSkin ? headerBackgroundDark : headerBackgroundLight; } }

        public static GUIStyle header;
        public static GUIStyle headerCheckbox;
        public static GUIStyle headerFoldout;

        public static readonly GUIStyle wheelLabel;

        public static readonly GUIStyle wheelThumb;

        public static readonly Vector2 wheelThumbSize;

        public static Texture2D m_TransparentTexture;
        public static Texture2D transparentTexture
        {
            get
            {
                if (m_TransparentTexture == null)
                {
                    m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = "Transparent Texture" };
                    m_TransparentTexture.SetPixel(0, 0, Color.clear);
                    m_TransparentTexture.Apply();
                }

                return m_TransparentTexture;
            }
        }

        public static readonly GUIStyle toggleOnStyle;
        public static readonly GUIStyle toggleOffStyle;

        public static readonly GUIStyle gridOnStyle;
        public static readonly GUIStyle gridOffStyle;

        public static readonly GUIStyle horizontalScrollbar;
        public static readonly GUIStyle verticalScrollbar;

        public static readonly GUIStyle richLargeButton;
        public static readonly GUIStyle richButton;

        static Styling()
        {
            var skin = GUI.skin;
            var customStyles = skin.customStyles;
            int currentId = customStyles.Length;
            System.Array.Resize(ref customStyles, customStyles.Length + 4);

            horizontalScrollbar = new GUIStyle("PreHorizontalScrollbar");
            customStyles[currentId++] = new GUIStyle(GUIStyle.none) { name = horizontalScrollbar.name + "leftbutton" };
            customStyles[currentId++] = new GUIStyle(GUIStyle.none) { name = horizontalScrollbar.name + "rightbutton" };

            verticalScrollbar = new GUIStyle("PreVerticalScrollbar");
            customStyles[currentId++] = new GUIStyle(GUIStyle.none) { name = verticalScrollbar.name + "upbutton" };
            customStyles[currentId++] = new GUIStyle(GUIStyle.none) { name = verticalScrollbar.name + "downbutton" };
            skin.customStyles = customStyles;

            smallTickbox = new GUIStyle("ShurikenToggle");

            splitterDark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
            splitterLight = new Color(0.6f, 0.6f, 0.6f, 1.333f);

            headerBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            headerBackgroundLight = new Color(1f, 1f, 1f, 0.2f);

            paneOptionsIconDark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            paneOptionsIconLight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");

            headerLabel = new GUIStyle(EditorStyles.miniLabel);

            wheelThumb = new GUIStyle("ColorPicker2DThumb");

            wheelThumbSize = new Vector2(
                !Mathf.Approximately(wheelThumb.fixedWidth, 0f) ? wheelThumb.fixedWidth : wheelThumb.padding.horizontal,
                !Mathf.Approximately(wheelThumb.fixedHeight, 0f) ? wheelThumb.fixedHeight : wheelThumb.padding.vertical
            );

            wheelLabel = new GUIStyle(EditorStyles.miniLabel);

            header = new GUIStyle("ShurikenModuleTitle")
            {
                font = (new GUIStyle("Label")).font,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = 22,
                contentOffset = new Vector2(20f, -2f),
                richText = true,
            };

            headerCheckbox = new GUIStyle("ShurikenCheckMark");
            headerFoldout = new GUIStyle("Foldout");

            richLargeButton = new GUIStyle("LargeButton") { richText = true };
            richButton = new GUIStyle("Button") { richText = true };

            toggleOnStyle = new GUIStyle("ProgressBarBar");
            toggleOnStyle.stretchWidth = true;
            toggleOnStyle.wordWrap = false;
            toggleOnStyle.margin.top -= 3;
            toggleOnStyle.margin.bottom += 3;

            toggleOffStyle = new GUIStyle("ProgressBarBack");
            toggleOffStyle.stretchWidth = true;
            toggleOffStyle.wordWrap = false;
            toggleOffStyle.margin.top -= 3;
            toggleOffStyle.margin.bottom += 3;

            gridOnStyle = new GUIStyle("ProgressBarBar");
            gridOnStyle.stretchWidth = true;
            gridOnStyle.wordWrap = false;
            gridOnStyle.margin.top -= 2;
            gridOnStyle.margin.bottom -= 2;

            gridOffStyle = new GUIStyle("ProgressBarBack");
            gridOffStyle.stretchWidth = true;
            gridOffStyle.wordWrap = false;
            gridOffStyle.margin.top -= 2;
            gridOffStyle.margin.bottom -= 2;
        }
    }
}
#endif
