#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
    public static class GUILayoutTools
    {
        private static GUISkin skin = StyleTools.Skin;

        #region GUILayout

        #region Button
        public static bool ConfigButton()
        {
            return GUILayout.Button("", StyleTools.ConfigStyle, GUILayout.Width(20), GUILayout.Height(20));
        }

        public static bool ConfigIconButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("d__Popup"), skin.button, GUILayout.Width(40), GUILayout.Height(40));
        }

        public static bool SettingButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("GameManager Icon"), skin.button, GUILayout.Width(40), GUILayout.Height(40));
        }

        public static bool ButtonFixedSize(GUIContent content, float w, float h)
        {
            return GUILayout.Button(content, skin.button, GUILayout.Width(w), GUILayout.Height(h));
        }

        public static bool ButtonFixedSizeYellow(GUIContent content, float w, float h)
        {
            return GUILayout.Button(content, skin.customStyles[2], GUILayout.Width(w), GUILayout.Height(h));
        }

        public static bool ButtonFixedSizeGray(GUIContent content, float w, float h)
        {
            return GUILayout.Button(content, skin.customStyles[3], GUILayout.Width(w), GUILayout.Height(h));
        }

        public static bool ButtonFixedSizeGray_Frame(GUIContent content, float w, float h)
        {
            return GUILayout.Button(content, skin.customStyles[4], GUILayout.Width(w), GUILayout.Height(h));
        }

        public static bool Button80_40(GUIContent content)
        {
            return ButtonFixedSize(content, 80, 40);
        }

        public static bool Button100_50(GUIContent content)
        {
            return ButtonFixedSize(content, 100, 50);
        }

        public static bool SerachButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("d_ViewToolZoom On"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool RefreshButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("d_RotateTool"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool AddButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus More"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool MinusButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool IconButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("GridIcon"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool ListButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("ListIcon"), skin.button, GUILayout.Width(30), GUILayout.Height(30));
        }

        public static bool OKButton()
        {
            return GUILayout.Button(EditorGUIUtility.IconContent("Button Icon"), skin.button, GUILayout.Width(40), GUILayout.Height(40));
        }

        public static bool Button(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, skin.button, options);
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, skin.button, options);
        }

        public static bool RepeatButton(string text, params GUILayoutOption[] options)
        {
            return GUILayout.RepeatButton(text, skin.button, options);
        }

        public static bool RepeatButton(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.RepeatButton(content, skin.button, options);
        }

        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, style, options);
        }

        #endregion

        #region Label
        public static void WhiteLargeLabel(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, StyleTools.GetGUIStyle("WhiteLargeLabel"), options);
        }

        public static void WhiteBoldLabel(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, StyleTools.GetGUIStyle("WhiteBoldLabel"), options);
        }

        public static void ControlLabel(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, StyleTools.GetGUIStyle("ControlLabel"), options);
        }

        public static void TitleTextLabel(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, StyleTools.GetGUIStyle("IN TitleText"), options);
        }

        public static void NotificationTextLabel(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, StyleTools.GetGUIStyle("NotificationText"), options);
        }

        public static void Label(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, skin.label, options);
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, skin.label, options);
        }
        #endregion

        #region 分隔线
        public static void Separator_DoubleLine()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("", skin.customStyles[0], GUILayout.Height(10));
            GUILayout.EndVertical();
        }
        public static void Separator_NoSpaceDoubleLine()
        {
            GUILayout.BeginVertical();
            //GUILayout.Space(5);
            GUILayout.Label("", skin.customStyles[0], GUILayout.Height(10));
            GUILayout.EndVertical();
        }

        public static void Separator_SingleLine()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("", skin.customStyles[1], GUILayout.Height(5));
            GUILayout.EndVertical();
        }


        public static void BreadcrumbsSeparator()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            GUILayout.Label("", skin.customStyles[5], GUILayout.Width(2));
            GUILayout.EndVertical();
        }
        #endregion

        #region Toggle
        public static bool Toggle(bool value, string text, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, text, skin.toggle, options);
        }

        public static bool Toggle(bool value, GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, skin.toggle, options);
        }

        public static bool DotToggle(bool value, GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, StyleTools.DotToggleStyle, options);
        }

        #endregion

        #region other
        public static void Box(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Box(content, skin.box, options);
        }

        public static void Box(string text, params GUILayoutOption[] options)
        {
            GUILayout.Box(text, skin.box, options);
        }

        public static void PasswordField(string password, char maskChar, params GUILayoutOption[] options)
        {
            GUILayout.PasswordField(password, maskChar, skin.textField, options);
        }

        public static void PasswordField(string password, char maskChar, int maxLength, params GUILayoutOption[] options)
        {
            GUILayout.PasswordField(password, maskChar, maxLength, skin.textField, options);
        }

        public static int SelectionGrid(int selected, GUIContent[] contents, int xCount, params GUILayoutOption[] options)
        {
            return GUILayout.SelectionGrid(selected, contents, xCount, options);
        }

        public static int SelectionGrid(int selected, string[] texts, int xCount, params GUILayoutOption[] options)
        {
            return GUILayout.SelectionGrid(selected, texts, xCount, options);
        }

        public static string TextArea(string text, int maxLength = -1, params GUILayoutOption[] options)
        {
            if (maxLength == -1)
            {
                return GUILayout.TextArea(text, skin.textArea, options);
            }
            else
            {
                return GUILayout.TextArea(text, maxLength, skin.textArea, options);
            }
        }

        public static string TextField(string text, string Name = "", int maxLength = 20, params GUILayoutOption[] options)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Name + ":");
                text = GUILayout.TextField(text, maxLength, skin.textField, options);
            }
            else
            {
                text = GUILayout.TextField(text, maxLength, skin.textField, options);
            }
            return text;
        }

        public static int Toolbar(int selected, GUIContent[] contents, params GUILayoutOption[] options)
        {
            return GUILayout.Toolbar(selected, contents, options);
        }

        public static int Toolbar(int selected, string[] texts, params GUILayoutOption[] options)
        {
            return GUILayout.Toolbar(selected, texts, options);
        }

        public static float HorizontalScrollbar(float value, float size, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            return GUILayout.HorizontalScrollbar(value, size, leftValue, rightValue, skin.horizontalScrollbar);
        }

        public static float HorizontalSlider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            return GUILayout.HorizontalSlider(value, leftValue, rightValue, skin.horizontalSlider, skin.horizontalSliderThumb);
        }

        public static float VerticalScrollbar(float value, float size, float topValue, float bottomValue, params GUILayoutOption[] options)
        {
            return GUILayout.VerticalScrollbar(value, size, topValue, bottomValue, skin.verticalScrollbar);
        }

        public static float VerticalSlider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            return GUILayout.VerticalSlider(value, leftValue, rightValue, skin.verticalSlider, skin.verticalSliderThumb);
        }

        public static void Space(float pixels)
        {
            GUILayout.Space(pixels);
        }

        public static void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }
        #endregion

        #region group
        public static bool SerachFiled(ref string search)
        {
            bool serachFlag = false;
            GUILayout.BeginVertical();
            Label(new GUIContent("Serach"));
            using (new HorizontalScope())
            {
                search = TextField(search, "", 100, GUILayout.Height(30));
                if (GUILayoutTools.SerachButton())
                {
                    serachFlag = true;
                }
            }
            GUILayout.EndVertical();
            return serachFlag;
        }

        public static string PathPicker(string pakerName, string path, bool isFile = false, string extension = "")
        {
            GUILayout.BeginVertical();
            Label(new GUIContent(pakerName));
            using (new HorizontalScope())
            {
                path = TextField(path,"", 100, GUILayout.Height(20));
                if (GUILayout.Button("", StyleTools.FloderStyle, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (isFile)
                    {
                        path = EditorUtility.OpenFilePanel(pakerName, path, extension);
                    }
                    else
                    {
                        path = EditorUtility.OpenFolderPanel(pakerName, path, "");
                    }
                }
            }
            GUILayout.EndVertical();
            return path;
        }

        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=14>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        public static void DrawTitle(string title, bool center = false)
        {
            var rich = GUI.skin.label.richText;
            GUI.skin.label.richText = true;
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUILayout.BeginHorizontal(StyleTools.Skin.box);
            GUI.color = Color.white;
            GUILayout.Label(string.Format("<size=22><b>{0}</b></size>", title), center ? StyleTools.MiddleCenterLab : GUI.skin.label);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUI.skin.label.richText = rich;
        }

        public static string GetTextSizeOf(string text, int size)
        {
            if (!GUI.skin.label.richText)
            {
                GUI.skin.label.richText = true;
            }
            return string.Format("<size={0}><b>{1}</b></size>", size, text);
        }

        /// <summary>
        /// 页码绘制接口，参数为当前页和最大页码。 注意：页码是从0开始的  显示上是从1开始的
        /// </summary>
        /// <param name="curPage">当前页</param>
        /// <param name="maxPage">最大页码</param>
        /// <returns>返回操作后的页码</returns>
        public static int DrawPage(int curPage, int maxPage)
        {
            if (maxPage <= 1) return 0;
            GUILayoutTools.Separator_SingleLine();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("第一页", EditorStyles.toolbarButton, GUILayout.Width(40)))
                {
                    return 0;
                }
                if (GUILayout.Button("<<", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    var page = curPage - 1;
                    return Mathf.Max(0, page);
                }
                bool draw = false;
                for (int i = 0; i < maxPage; ++i)
                {
                    if (i == 0 || i == maxPage - 1 || Mathf.Abs(i - curPage) <= 2)
                    {
                        bool select = i == curPage;
                        var color = GUI.color;
                        GUI.color = select ? Color.green : color;
                        if (GUILayout.Button((i + 1).ToString(),
                            EditorStyles.label, GUILayout.Width(20)))
                        {
                            curPage = i;
                        }
                        GUI.color = color;
                        draw = false;
                    }
                    else
                    {
                        if (!draw)
                        {
                            draw = true;
                            GUILayout.Label("...", GUILayout.Width(20));
                        }
                    }
                }
                if (GUILayout.Button(">>", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    var page = curPage + 1;
                    return Mathf.Min(page, maxPage - 1);
                }
                if (GUILayout.Button("最后页", EditorStyles.toolbarButton, GUILayout.Width(40)))
                {
                    return maxPage - 1;
                }
                GUILayout.Space(20);
                GUILayout.FlexibleSpace();
            }
            return Mathf.Clamp(curPage, 0, maxPage - 1);
        }

        #endregion

        #endregion

        #region Scope
        public class VerticalScope : GUI.Scope
        {
            public VerticalScope()
            {
                GUILayout.BeginVertical(skin.box);
            }

            public VerticalScope(params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical("Box", options);
            }

            public VerticalScope(GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(style, options);
            }

            public VerticalScope(string text, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(text, style, options);
            }

            public VerticalScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(image, style, options);
            }

            public VerticalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(content, style, options);
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
            }
        }

        public class HorizontalScope : GUI.Scope
        {
            public HorizontalScope()
            {
                GUILayout.BeginHorizontal("Box");
            }

            public HorizontalScope(params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal("Box", options);
            }

            public HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(style, options);
            }

            public HorizontalScope(string text, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(text, style, options);
            }

            public HorizontalScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(image, style, options);
            }

            public HorizontalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(content, style, options);
            }

            protected override void CloseScope()
            {
                GUILayout.EndHorizontal();
            }
        }

        public class AreaScope : GUI.Scope
        {
            public AreaScope(Rect screenRect)
            {
                GUILayout.BeginArea(screenRect, GUI.skin.textArea);
            }

            public AreaScope(Rect screenRect, string text)
            {
                GUILayout.BeginArea(screenRect, text, GUI.skin.textArea);
            }

            public AreaScope(Rect screenRect, Texture image)
            {
                GUILayout.BeginArea(screenRect, image, GUI.skin.textArea);
            }

            public AreaScope(Rect screenRect, GUIContent content)
            {
                GUILayout.BeginArea(screenRect, content, GUI.skin.textArea);
            }

            public AreaScope(Rect screenRect, string text, GUIStyle style)
            {
                GUILayout.BeginArea(screenRect, text, style);
            }

            public AreaScope(Rect screenRect, Texture image, GUIStyle style)
            {
                GUILayout.BeginArea(screenRect, image, style);
            }

            public AreaScope(Rect screenRect, GUIContent content, GUIStyle style)
            {
                GUILayout.BeginArea(screenRect, content, style);
            }

            protected override void CloseScope()
            {
                GUILayout.EndArea();
            }
        }

        public class ScrollViewScope : GUI.Scope
        {
            public Vector2 scrollPosition
            {
                get;
                private set;
            }

            public bool handleScrollWheel
            {
                get;
                set;
            }

            public ScrollViewScope(Vector2 scrollPosition)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box");
            }

            public ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, "Box", options);
            }

            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
            }

            public ScrollViewScope(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
            }

            public ScrollViewScope(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, style, options);
            }

            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, options);
            }

            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
            }

            protected override void CloseScope()
            {
                GUILayout.EndScrollView();
            }
        }
        #endregion

    }

    public static class GUITools
    {
        static GUISkin skin = StyleTools.Skin;

        #region GUI Need Rect

        #region Box
        public static void Box(Rect position, string text)
        {
            GUI.Box(position, text, skin.box);
        }

        public static void Box(Rect position, GUIContent content)
        {
            GUI.Box(position, content, skin.box);
        }
        #endregion

        #region button
        public static bool ConfigButton()
        {
            return GUILayout.Button("", StyleTools.ConfigStyle, GUILayout.Width(20), GUILayout.Height(20));
        }

        public static bool ConfigIconButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("d__Popup"), skin.button);
        }

        public static bool SettingButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("GameManager Icon"), skin.button);
        }

        public static bool SerachButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("d_ViewToolZoom On"), skin.button);
        }

        public static bool RefreshButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("d_RotateTool"), skin.button);
        }

        public static bool AddButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("d_Toolbar Plus More"), skin.button);
        }

        public static bool MinusButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("d_Toolbar Minus"), skin.button);
        }

        public static bool IconButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("GridIcon"), skin.button);
        }

        public static bool ListButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("ListIcon"), skin.button);
        }

        public static bool OKButton(Rect rect)
        {
            return GUI.Button(rect, EditorGUIUtility.IconContent("Button Icon"), skin.button);
        }

        public static bool Button(Rect position, GUIContent content)
        {
            return GUI.Button(position, content, skin.button);
        }

        public static bool Button(Rect position, string text)
        {
            return GUI.Button(position, text, skin.button);
        }

        public static bool RepeatButton(Rect position, string text)
        {
            return GUI.RepeatButton(position, text, skin.button);
        }

        public static bool RepeatButton(Rect position, GUIContent content)
        {
            return GUI.RepeatButton(position, content, skin.button);
        }

        #endregion

        #region Leabel
        public static void WhiteLargeLabel(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, StyleTools.GetGUIStyle("WhiteLargeLabel"));
        }

        public static void WhiteBoldLabel(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, StyleTools.GetGUIStyle("WhiteBoldLabel"));
        }

        public static void ControlLabel(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, StyleTools.GetGUIStyle("ControlLabel"));
        }

        public static void TitleTextLabel(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, StyleTools.GetGUIStyle("IN TitleText"));
        }

        public static void NotificationTextLabel(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, StyleTools.GetGUIStyle("NotificationText"));
        }

        public static void Label(Rect position, GUIContent content)
        {
            GUI.Label(position, content, skin.label);
        }

        public static void Label(Rect position, string text)
        {
            GUI.Label(position, text, skin.label);
        }
        #endregion

        #region other
        public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue)
        {
            return GUI.HorizontalScrollbar(position, value, size, leftValue, rightValue, skin.horizontalScrollbar);
        }

        public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue)
        {
            return GUI.HorizontalSlider(position, value, leftValue, rightValue, skin.horizontalSlider, skin.horizontalSliderThumb);
        }

        public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue)
        {
            return GUI.VerticalScrollbar(position, value, size, topValue, bottomValue, skin.verticalScrollbar);
        }

        public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue)
        {
            return GUI.VerticalSlider(position, value, topValue, bottomValue, skin.verticalSlider, skin.verticalSliderThumb);
        }

        public static string PasswordField(Rect position, string password, char maskChar, int maxLength)
        {
            return GUI.PasswordField(position, password, maskChar, maxLength, skin.textField);
        }

        public static string PasswordField(Rect position, string password, char maskChar)
        {
            return GUI.PasswordField(position, password, maskChar, skin.textField);
        }

        public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount)
        {
            return GUI.SelectionGrid(position, selected, content, xCount);
        }

        public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount)
        {
            return GUI.SelectionGrid(position, selected, texts, xCount);
        }

        public static int Toolbar(Rect position, int selected, string[] texts)
        {
            return GUI.Toolbar(position, selected, texts);
        }

        public static int Toolbar(Rect position, int selected, GUIContent[] contents)
        {
            return GUI.Toolbar(position, selected, contents);
        }
        #endregion

        #region Text
        public static string TextArea(Rect position, string text, int maxLength)
        {
            return GUI.TextArea(position, text, maxLength, skin.textArea);
        }

        public static string TextArea(Rect position, string text)
        {
            return GUI.TextArea(position, text, skin.textArea);
        }

        public static string TextField(Rect position, string text, int maxLength)
        {
            return GUI.TextField(position, text, maxLength, skin.textField);
        }

        public static string TextField(Rect position, string text)
        {
            return GUI.TextField(position, text, skin.textField);
        }
        #endregion

        #region Toggle
        public static bool Toggle(Rect position, bool value, GUIContent content)
        {
            return GUI.Toggle(position, value, content, skin.toggle);
        }

        public static bool DotToggle(Rect position, bool value, GUIContent content)
        {
            return GUI.Toggle(position, value, content, StyleTools.DotToggleStyle);
        }

        public static bool Toggle(Rect position, bool value, string text)
        {
            return GUI.Toggle(position, value, text, skin.toggle);
        }

        public static bool Toggle(Rect position, int id, bool value, GUIContent content)
        {
            return GUI.Toggle(position, id, value, content, skin.toggle);
        }
        #endregion

        #region 分隔线
        public static void Separator_DoubleLine(Rect rect)
        {
            GUI.Label(rect, "", skin.customStyles[0]);
        }
        public static void Separator_SingleLine(Rect rect)
        {
            GUI.Label(rect, "", skin.customStyles[1]);
        }
        public static void BreadcrumbsSeparator(Rect rect)
        {
            GUI.Label(rect, "", skin.customStyles[5]);
        }
        #endregion

        #region group
        /// <summary>
        /// 建议rect高度:30
        /// </summary>
        public static bool SerachFiled(Rect rect, ref string search)
        {
            bool serachFlag = false;
            search = GUI.TextField(new Rect(rect.x, rect.y, rect.width - 30, rect.height), search, skin.textField);
            if (GUITools.SerachButton(new Rect(rect.x + (rect.width - 30), rect.y, 30, rect.height)))
            {
                serachFlag = true;
            }
            return serachFlag;
        }

        /// <summary>
        /// 建议rect高度:20
        /// </summary>
        public static string PathPicker(Rect rect, string pakerName, string path, bool isFile = false, string extension = "")
        {
            path = GUI.TextField(new Rect(rect.x, rect.y, rect.width - 20, rect.height), path, skin.textField);
            if (GUI.Button(new Rect(rect.x + (rect.width - 20), rect.y, 20, rect.height), "", StyleTools.FloderStyle))
            {
                if (isFile)
                {
                    path = EditorUtility.OpenFilePanel(pakerName, path, extension);
                }
                else
                {
                    path = EditorUtility.OpenFolderPanel(pakerName, path, "");
                }
            }
            return path;
        }
        #endregion

        #endregion

        #region Scope
        public class ClipScope : GUI.Scope
        {
            public ClipScope(Rect position)
            {
                GUI.BeginClip(position);
            }

            protected override void CloseScope()
            {
                GUI.EndClip();
            }
        }

        public class GroupScope : GUI.Scope
        {
            public GroupScope(Rect position)
            {
                GUI.BeginGroup(position, "Box");
            }

            public GroupScope(Rect position, string text)
            {
                GUI.BeginGroup(position, text, "Box");
            }

            public GroupScope(Rect position, Texture image)
            {
                GUI.BeginGroup(position, image, "Box");
            }

            public GroupScope(Rect position, GUIContent content)
            {
                GUI.BeginGroup(position, content, "Box");
            }

            public GroupScope(Rect position, GUIStyle style)
            {
                GUI.BeginGroup(position, style);
            }

            public GroupScope(Rect position, string text, GUIStyle style)
            {
                GUI.BeginGroup(position, text, style);
            }

            public GroupScope(Rect position, Texture image, GUIStyle style)
            {
                GUI.BeginGroup(position, image, style);
            }

            protected override void CloseScope()
            {
                GUI.EndGroup();
            }
        }

        public class ScrollViewScope : GUI.Scope
        {
            public Vector2 scrollPosition
            {
                get;
                private set;
            }

            public bool handleScrollWheel
            {
                get;
                set;
            }

            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
            }

            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
            }

            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
            }

            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
            }

            protected override void CloseScope()
            {
                GUI.EndScrollView(this.handleScrollWheel);
            }
        }
        #endregion

        public static string GetTextSizeOf(string text, int size)
        {
            return string.Format("<size={0}><b>{1}</b></size>", size, text);
        }
    }
}
#endif