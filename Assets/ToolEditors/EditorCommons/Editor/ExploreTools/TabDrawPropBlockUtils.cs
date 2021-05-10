using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace UnityEditor
{
    public class TabDrawer<T> where T : UnityEngine.Object
    {
        public List<T> objectList = new List<T>();
        public List<PropBlock<T>> tabList = new List<PropBlock<T>>();
        private Vector2 scroll;
        public Action repaint;

        Color rowColorTypeOne = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        Color rowColorTypeTwo = new Color(60f / 255f, 60f / 255f, 60f / 255f);

        public void OnGUI(Rect position)
        {
            TryInitPropTab();
            var objLenth = objectList.Count;
            using (new GUILayout.HorizontalScope())
            {
                foreach (var tab in tabList)
                {
                    if (tab.DrawTitle())
                    {
                        repaint();
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayoutTools.Separator_DoubleLine();
            var bgRect = GUILayoutUtility.GetLastRect();
            using (var sc = new GUILayout.ScrollViewScope(scroll))
            {
                for (var i = 0; i < objLenth; i++)
                {
                    var m = objectList[i];
                    var selected = Selection.activeObject == m;
                    var style = selected ? "SelectionRect" : GUI.skin.label;
                    var rowRect = new Rect(bgRect.x - 10, (i * 20) - 2, bgRect.width, 20);
                    if (rowRect.Contains(Event.current.mousePosition) && !selected)
                    {
                        style = "box";
                    }
                    EditorGUI.DrawRect(rowRect, i % 2 == 0 ? rowColorTypeOne : rowColorTypeTwo);
                    GUI.Label(rowRect, "", style);
                    using (new GUILayout.HorizontalScope())
                    {
                        foreach (var tab in tabList)
                        {
                            using (new GUILayout.VerticalScope(GUILayout.Width(tab.width)))
                            {
                                tab.DrawProp(m, position);
                            }
                            GUILayoutUtility.GetRect(10, 20);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Ping Object", EditorStyles.toolbarButton))
                        {
                            PingObject(m);
                            Event.current.Use();
                        }
                    }
                }
                scroll = sc.scrollPosition;
            }
            DrawExtend();
        }

        protected virtual void PingObject(T m)
        {
            var ms = GameObject.FindObjectsOfType<T>().Select(
                r =>
                {
                    return r;
                }).Distinct().ToList();
            ms.RemoveAll(g => { return g == null; });
            Selection.objects = ms.ToArray();
            EditorGUIUtility.PingObject(Selection.activeGameObject);
            SceneView.FrameLastActiveSceneView();
        }

        private void TryInitPropTab()
        {
            foreach (var tab in tabList)
            {
                tab.Register(UpdatObj);
            }
        }

        private void UpdatObj(Comparison<T> comparison)
        {
            objectList.Sort(comparison);
        }

        protected virtual void DrawExtend()
        {

        }

        public virtual void RefreshObjs()
        {

        }

        public void Sort()
        {
            foreach (var tab in tabList)
            {
                tab.Sort();
            }
        }
    }

    public class PropBlock<T> where T : UnityEngine.Object
    {
        internal delegate void SortFunc(Comparison<T> comparison);
        internal enum SortType
        {
            Up = 1,
            None = 0,
            Down = -1
        }

        internal string title;
        internal float width;
        internal SortType sortType = SortType.None;
        internal string propPath;
        internal SerializedProperty[] propArray = new SerializedProperty[0];
        internal SortFunc sortFunc;
        private string display;

        internal PropBlock(string title, float width, string path)
        {
            this.title = title;
            this.display = title;
            this.width = width;
            this.propPath = path;
        }

        internal void Register(SortFunc sortFunc)
        {
            this.sortFunc = sortFunc;
        }

        internal void DrawProp(T m, Rect position)
        {
            if (m)
            {
                OnDrawProp(m, position);
                SelectionBlock(m, position.width);
            }
            else
            {
                GUILayout.Label("Null", GUILayout.Width(width));
            }
        }

        private void SelectionBlock(T m, float width)
        {
            var r = GUILayoutUtility.GetLastRect();
            r.x = 0;
            r.width = width;
            var e = Event.current;
            if (r.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    Selection.activeObject = m;
                }
            }
        }

        protected virtual void OnDrawProp(T m, Rect position)
        {

        }

        protected virtual int SortFunction(T x, T y)
        {
            return x.name.CompareTo(y.name);
        }

        internal bool DrawTitle()
        {
            var change = GUILayout.Button(display, GUI.skin.label, GUILayout.Width(width));
            if (change)
            {
                ChangeSortType();
            }
            var r = GUILayoutUtility.GetRect(10, 20);
            GUI.Label(r, "|");
            EditorGUIUtility.AddCursorRect(r, MouseCursor.ResizeHorizontal);
            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseUp:
                    Release();
                    break;
                case EventType.MouseDown:
                    if (r.Contains(e.mousePosition))
                    {
                        Drag(e);
                    }
                    break;
                case EventType.MouseDrag:
                    if (dragThis)
                    {
                        change = true;
                        var w = e.mousePosition.x - oriX;
                        width = Mathf.Max(width + w, 80);
                        oriX = e.mousePosition.x;
                    }
                    break;
            }
            return change;
        }

        protected virtual void TitleEx()
        {

        }

        private void ChangeSortType()
        {
            sortType = ((int)sortType + 1) > (int)SortType.Up ? SortType.Down : (sortType + 1);
            if (sortFunc != null)
            {
                sortFunc.Invoke(SortFunction);
                EditorGUIUtility.editingTextField = false;
                if (sortType == SortType.Up)
                {
                    display = "↑" + title;
                }
                else if (sortType == SortType.Down)
                {
                    display = "↓" + title;
                }
                else
                {
                    display = title;
                }
            }
        }

        public void Sort()
        {
            if (sortFunc != null && sortType != SortType.None)
            {
                sortFunc.Invoke(SortFunction);
            }
        }

        private float oriX;
        private bool dragThis;
        private void Drag(Event e)
        {
            dragThis = true;
            oriX = e.mousePosition.x;
        }
        private void Release()
        {
            dragThis = false;
            oriX = 0;
        }
    }
}


