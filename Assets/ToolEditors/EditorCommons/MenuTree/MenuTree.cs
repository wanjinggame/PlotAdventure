using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor.AnimatedValues;

namespace UnityEditor
{
    public class MenuTreeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Action<Rect> menuTreeItemDraw;
        public Vector2 popupSize = Vector2.zero;
        public AnimBool state = new AnimBool(false);
        public bool drawTitle = true;
        public virtual void OnEnable()
        {

        }

        public virtual void OnShow()
        {

        }

        public virtual void OnHide()
        {

        }

        public virtual void OnDestroy()
        {

        }
    }

    public class MenuTree : TreeView, IEnumerable
    {
        private List<MenuTreeItem> menuTreeItems = new List<MenuTreeItem>();

        private Action OnselectClick;

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "root" };
        }

        public MenuTree(TreeViewState state, float rowHeight, List<MenuTreeItem> menuTreeItems, Action OnselectClick = null, bool select = true) : base(state)
        {
            this.menuTreeItems = menuTreeItems;
            this.OnselectClick = OnselectClick;
            this.rowHeight = rowHeight;
            Reload();
            if (select)
            {
                SetSelection(new List<int>() { 0, }, TreeViewSelectionOptions.RevealAndFrame);
            }
        }

        public void AddMenu(string menu, bool needRefersh = true)
        {
            menuTreeItems.Add(new MenuTreeItem() { Name = menu });
            if (needRefersh) Reload();
        }

        public void AddMenu(MenuTreeItem menu, bool needRefersh = true)
        {
            if (menu != null)
            {
                menuTreeItems.Add(menu);
            }
            if (needRefersh) Reload();
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (menuTreeItems == null || menuTreeItems.Count == 0)
            {
                return base.BuildRows(root);
            }
            root.children = new List<TreeViewItem>();
            int id = 0;
            foreach (var item in menuTreeItems)
            {
                item.Id = id++;
                var subTree = new TreeViewItem
                {
                    id = item.Id,
                    displayName = item.Name,
                };
                root.AddChild(subTree);
            }
            SetupDepthsFromParentsAndChildren(root);
            return base.BuildRows(root);
        }

        public void OnGUI(Rect rect, EditorWindow owner)
        {
            EditorGUI.DrawRect(rect, StyleCollection.instance.menuTreeBgColor);
            OnGUI(rect);
            if (owner) owner.Repaint();
        }

        Color hoverColor = new Color(60f / 255f, 60f / 255f, 60f / 255f);
        public void RebuildTree()
        {
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            Rect rect = args.rowRect;
            var id = args.item.id;
            bool isSelect = IsSelected(id);
            if (!isSelect)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(rect, hoverColor);
                }
            }
            GUI.Label(rect, GUILayoutTools.GetTextSizeOf(args.item.displayName, 16), StyleTools.MiddleCenterLab);
            if (menuTreeItems.FindIndex(t => { return t.Id == id; }) != menuTreeItems.Count - 1)
            {
                GUITools.Separator_SingleLine(new Rect(rect.x, rect.y + rect.height - 1, rect.width, 10));
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {

        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return false;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return false;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
        }

        protected override void SingleClickedItem(int id)
        {
            if (IsSelected(id) && OnselectClick != null)
            {
                OnselectClick();
            }
        }

        public IEnumerator GetEnumerator()
        {
            return menuTreeItems.GetEnumerator();
        }

        public MenuTreeItem this[int i]
        {
            get { return menuTreeItems[i]; }
        }

        public MenuTreeItem GetSelectItem()
        {
            var ids = GetSelection();
            return menuTreeItems.Find(t => { return t.Id == ids[0]; });
        }
    }
}



