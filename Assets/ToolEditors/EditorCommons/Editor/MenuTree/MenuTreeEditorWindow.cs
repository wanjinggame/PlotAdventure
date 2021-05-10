using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;

namespace UnityEditor
{
    /// <summary>
    /// 使用例子 见Windows.cs的TestMenuEditorWindow类
    /// </summary>
    public class MenuTreeEditorWindow : EditorWindow
    {
        class MenuPopup : PopupWindowContent
        {
            private static MenuPopup popup;

            //private static List<EditorWindow> openWindows = new List<EditorWindow>();

            private static Rect myRect;

            private MenuTreeItem drawer;

            public override Vector2 GetWindowSize() { return new Vector2(myRect.width, myRect.height); }

            public static void Show(Rect rect, MenuTreeItem menuTreeItem)
            {
                myRect = rect;
                if (popup == null)
                {
                    popup = new MenuPopup();
                    Debug.unityLogger.logEnabled = false;
                    UnityEditor.TimelineEditor.ReflectionTools.ShowPopupWindow(new Rect(rect.x, rect.y, 0, 0),
                        popup, UnityEditor.TimelineEditor.ShowMode.NoShadow);
                    Debug.unityLogger.logEnabled = true;
                }
                popup.drawer = menuTreeItem;
                if (menuTreeItem.popupSize != Vector2.zero)
                {
                    myRect.width = menuTreeItem.popupSize.x;
                    myRect.height = menuTreeItem.popupSize.y;
                }
                popup.editorWindow.Repaint();
                popup.editorWindow.Focus();
            }

            Rect position;

            public override void OnGUI(Rect rect)
            {
                position = rect;
                HandleDragRect();
                //CheckSize();
                if (drawer != null)
                {
                    if (drawer.drawTitle)
                    {
                        GUILayoutTools.DrawTitle(drawer.Name, true);
                    }
                    if (drawer.menuTreeItemDraw != null)
                    {
                        drawer.menuTreeItemDraw(rect);
                    }
                }
                else
                {
                    GUI.Label(rect, GUILayoutTools.GetTextSizeOf("内容未定制...", 30), StyleTools.MiddleCenterLab);
                }
                editorWindow.Repaint();
            }

            public override void OnClose()
            {
                base.OnClose();
                popup = null;
            }

            public static void Close()
            {
                if (popup != null && popup.editorWindow != null)
                {
                    popup.editorWindow.Close();
                }
                popup = null;
            }

            public static bool IsFocused(EditorWindow focuser)
            {
                if (popup != null && popup.editorWindow && popup.editorWindow == focuser)
                {
                    return true;
                }
                return false;
            }

            private void HandleDragRect()
            {
                var dragR = new Rect(position.width - 6, 0, 6, position.height);
                EditorGUIUtility.AddCursorRect(dragR, MouseCursor.ResizeHorizontal);
                var e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseUp:
                        Release();
                        break;
                    case EventType.MouseDown:
                        if (dragR.Contains(e.mousePosition))
                        {
                            Drag(e);
                        }
                        break;
                    case EventType.MouseDrag:
                        if (dragThis)
                        {
                            var w = e.mousePosition.x - oriX;
                            myRect.width = Mathf.Clamp(myRect.width + w, 200, 800);
                            if (drawer != null)
                            {
                                drawer.popupSize = new Vector2(myRect.width, myRect.height);
                            }
                            oriX = e.mousePosition.x;
                        }
                        break;
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

        public MenuTree menuTree;

        public MenuTreeItem curMenuTreeItem;

        protected bool isPopup
        {
            get
            {
                if (AwaysPopup) return true;
                return  EditorPrefs.GetBool(titleContent.text, false);
            }
            set
            {
                if (!AwaysPopup)
                {
                    EditorPrefs.SetBool(titleContent.text, value);
                    SetSize(value);
                    if (value) SetSelect(-1);
                    else
                    {
                        MenuPopup.Close();
                        SetSelect(0);
                    }
                }
            }
        }

        public void OnEnable()
        {
            SetSize(isPopup);
            menuTree = new MenuTree(new TreeViewState(), GetMenuHeight(), new List<MenuTreeItem>(), OnSelectItemClick, !isPopup);
            InitOther();
            BuildMenuTree();
            foreach (var item in menuTree)
            {
                (item as MenuTreeItem).OnEnable();
            }
        }

        private void OnSelectItemClick()
        {
            var item = menuTree.GetSelectItem();
            if (item != null)
            {
                if (curMenuTreeItem != item)
                {
                    if (curMenuTreeItem != null) curMenuTreeItem.OnHide();
                    curMenuTreeItem = item;
                    curMenuTreeItem.OnShow();
                    OnMenuItemSelectChange(curMenuTreeItem);
                    if (isPopup)
                    {
                        MenuPopup.Show(new Rect(150, -18, GetPopupWidth(), position.height), curMenuTreeItem);
                    }
                }
                else
                {
                    if (isPopup)
                    {
                        MenuPopup.Close();
                        SetSelect(-1);
                        if (curMenuTreeItem != null) curMenuTreeItem.OnHide();
                        curMenuTreeItem = null;
                    }
                }
            }
        }

        public void OnDestroy()
        {
            Destroy();
            foreach (var item in menuTree)
            {
                (item as MenuTreeItem).OnDestroy();
            }
            menuTree = null;
            MenuPopup.Close();
        }

        private void OnGUI()
        {
            OnPreDraw();
            OnGUIDraw(position);
        }

        private void Update()
        {
            OnUpdata();
        }

        public void OnGUIDraw(Rect position)
        {
            GUI.Label(new Rect(0, 0, 124, 18), "", EditorStyles.toolbar);
            if (GUI.Button(new Rect(124, 0, 26, 18), StyleTools.gearIcon, EditorStyles.toolbarButton))
            {
                ShowGear();
            }
            Rect menuTreeRect = new Rect(0, 18, 150, position.height);
            if (menuTree != null)
            {
                menuTree.OnGUI(menuTreeRect, this);
            }
            if (!isPopup)
            {
                if (curMenuTreeItem != null)
                {
                    Rect mentTreeItemDrawRecr = new Rect(151, 1, position.width - 152, position.height - 3);
                    if (curMenuTreeItem.menuTreeItemDraw != null)
                    {
                        GUILayout.BeginArea(mentTreeItemDrawRecr, StyleTools.Skin.box);
                        curMenuTreeItem.menuTreeItemDraw.Invoke(new Rect(0, 0, mentTreeItemDrawRecr.width, mentTreeItemDrawRecr.height));
                        GUILayout.EndArea();
                    }
                    else
                    {
                        GUI.Label(mentTreeItemDrawRecr, GUILayoutTools.GetTextSizeOf(curMenuTreeItem.Name + " 内容未定制...", 30), StyleTools.MiddleCenterLab);
                    }
                }
            }
        }
        private void SetSize(bool popup)
        {
            if (!popup)
            {
                minSize = new Vector2(1100f, 100f);
                maxSize = new Vector2(5000f, 4000f);
            }
            else
            {
                minSize = new Vector2(150f, 100f);
                maxSize = new Vector2(150f, 4000f);
            }
        }

        protected virtual bool AwaysPopup { get { return false; } set { } }

        /// <summary>
        /// 在此函数中调用AddMenuItem方法 添加Menu
        /// </summary>
        public virtual void BuildMenuTree()
        {

        }

        public virtual void InitOther()
        {
        }

        public virtual void Destroy()
        {

        }

        protected virtual void OnMenuItemSelectChange(MenuTreeItem selectItem)
        {

        }

        protected virtual void OnPreDraw()
        {

        }

        public virtual void OnUpdata()
        {

        }

        protected virtual float GetPopupWidth()
        {
            return 800f;
        }

        protected virtual float GetMenuHeight()
        {
            return 40f;
        }

        private void ShowGear()
        {
            GenericMenu genericMenu = new GenericMenu();
            OnShowConfig(genericMenu);
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent(isPopup ? "正常模式" : "弹窗模式"), false, () =>
            {
                isPopup = !isPopup;
            });
            genericMenu.ShowAsContext();
        }

        public virtual void OnShowConfig(GenericMenu genericMenu)
        {

        }

        public void AddMenuItem(string menuName, Action<Rect> drawer)
        {
            MenuTreeItem menuTreeItem = new MenuTreeItem()
            {
                Name = menuName,
                menuTreeItemDraw = drawer
            };
            menuTree.AddMenu(menuTreeItem);
        }

        public void AddMenuItem(string menuName, Action<Rect> drawer, Vector2 size)
        {
            MenuTreeItem menuTreeItem = new MenuTreeItem()
            {
                Name = menuName,
                menuTreeItemDraw = drawer,
                popupSize = size
            };
            menuTree.AddMenu(menuTreeItem);
        }

        public void AddMenuItem(MenuTreeItem menuTreeItem)
        {
            if (menuTreeItem != null)
            {
                menuTree.AddMenu(menuTreeItem);
            }
        }

        /// <summary>
        /// 选中某个Menu 参数传-1表示都不选中
        /// </summary>
        /// <param name="index">-1 表示清空选中情况</param>
        public void SetSelect(int index)
        {
            if (index < 0)
            {
                menuTree.SetSelection(new List<int>() { }, TreeViewSelectionOptions.RevealAndFrame);
                curMenuTreeItem = null;
                if (isPopup)
                {
                    MenuPopup.Close();
                }
            }
            else
            {
                menuTree.SetSelection(new List<int>() { index }, TreeViewSelectionOptions.RevealAndFrame);
                var item = menuTree[index];
                if (curMenuTreeItem != item)
                {
                    curMenuTreeItem = item;
                    OnMenuItemSelectChange(curMenuTreeItem);
                    if (isPopup)
                    {
                        MenuPopup.Show(new Rect(150, 0, GetPopupWidth(), position.height), curMenuTreeItem);
                    }
                }
            }
        }
    }
}
