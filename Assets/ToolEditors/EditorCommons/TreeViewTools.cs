#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;
using UnityEditor;
using System.Reflection;

namespace Pangu.EditorTools
{
    public class GameObjectRootTreeView : TreeView
    {
        private SearchField _searchField = new SearchField();

        private GameObject root;

        public Action<int> OnSelectionChange;

        public GameObjectRootTreeView(TreeViewState state, GameObject root) : base(state)
        {
            rowHeight = 20;
            showBorder = true;
            this.root = root;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "root" };
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (this.root == null)
            {
                return base.BuildRows(root);
            }
            root.children = new List<TreeViewItem>();
            BuildSubTree(root, this.root);
            SetupDepthsFromParentsAndChildren(root);
            return base.BuildRows(root);
        }

        void BuildSubTree(TreeViewItem root, GameObject rootObject)
        {
            if ((rootObject.hideFlags & HideFlags.HideInHierarchy) == 0)
            {
                var subTree = new TreeViewItem() { id = rootObject.GetInstanceID(), children = new List<TreeViewItem>(), displayName = rootObject.name };
                root.AddChild(subTree);
                for (int i = 0; i < rootObject.transform.childCount; ++i)
                {
                    BuildSubTree(subTree, rootObject.transform.GetChild(i).gameObject);
                }
            }
        }

        Color rowColorTypeOne = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        Color rowColorTypeTwo = new Color(60f / 255f, 60f / 255f, 60f / 255f);

        protected override void BeforeRowsGUI()
        {
            for (int i = -1; i < Mathf.Max(treeViewRect.height, totalHeight) / rowHeight; ++i)
            {
                EditorGUI.DrawRect(new Rect(treeViewRect.x, treeViewRect.y + 1 + i * rowHeight, treeViewRect.width, rowHeight), 
                    i % 2 == 0 ? rowColorTypeOne : rowColorTypeTwo);
            }
            base.BeforeRowsGUI();
        }

        public override void OnGUI(Rect rect)
        {
            Rect srect = rect;
            srect.height = 18f;
            searchString = _searchField.OnGUI(srect, searchString);
            rect.y += srect.height;
            rect.height -= srect.height;
            GUI.BeginClip(rect);
            base.OnGUI(new Rect(0f, 0f, rect.width, rect.height));
            GUI.EndClip();
        }

        public void RebuildTree()
        {
            Reload();
        }

        public void RebuildTree(GameObject go)
        {
            this.root = go;
            RebuildTree();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count > 0 && OnSelectionChange != null)
            {
                OnSelectionChange.Invoke(selectedIds[0]);
            }
        }
    }

    public class FolderRootTreeView : TreeView
    {
        private SearchField _searchField = new SearchField();

        private string rootPath;

        private TreeViewItem rootFolderItem;

        public Action<int> OnSelectionChange;

        public FolderRootTreeView(TreeViewState state, string path) : base(state)
        {
            rowHeight = 20;
            showBorder = true;
            this.rootPath = path;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "root" };
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                return base.BuildRows(root);
            }

            root.children = new List<TreeViewItem>();

            var rootFolder = AssetDatabase.LoadMainAssetAtPath(rootPath);
            rootFolderItem = new TreeViewItem() { id = rootFolder.GetInstanceID(), children = new List<TreeViewItem>(), displayName = rootFolder.name };

            root.children.Add(rootFolderItem);

            Stack<TreeViewItem> folderStack = new Stack<TreeViewItem>();
            folderStack.Push(rootFolderItem);
            string[] assets = AssetDatabase.FindAssets("t:folder", new string[] { rootPath });
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var curFolder = folderStack.Peek();
                while (!path.Contains(AssetDatabase.GetAssetPath(curFolder.id)))
                {
                    folderStack.Pop();
                    curFolder = folderStack.Peek();
                }
                var folder = AssetDatabase.LoadMainAssetAtPath(path);
                var subFolder = new TreeViewItem()
                { id = folder.GetInstanceID(), children = new List<TreeViewItem>(), displayName = folder.name };
                curFolder.children.Add(subFolder);
                folderStack.Push(subFolder);
            }

            SetupDepthsFromParentsAndChildren(root);
            return base.BuildRows(root);
        }

        Color rowColorTypeOne = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        Color rowColorTypeTwo = new Color(60f / 255f, 60f / 255f, 60f / 255f);

        protected override void BeforeRowsGUI()
        {
            for (int i = -1; i < Mathf.Max(treeViewRect.height, totalHeight) / rowHeight; ++i)
            {
                EditorGUI.DrawRect(new Rect(treeViewRect.x, treeViewRect.y + 1 + i * rowHeight, treeViewRect.width, rowHeight), i % 2 == 0 ? rowColorTypeOne : rowColorTypeTwo);
            }

            base.BeforeRowsGUI();
        }

        public override void OnGUI(Rect rect)
        {
            Rect srect = rect;
            srect.height = 18f;
            searchString = _searchField.OnGUI(srect, searchString);
            rect.y += srect.height;
            rect.height -= srect.height;
            GUI.BeginClip(rect);
            base.OnGUI(new Rect(0f, 0f, rect.width, rect.height));
            GUI.EndClip();
        }

        public void RebuildTree()
        {
            Reload();
        }

        public void RebuildTree(string rootPath)
        {
            this.rootPath = rootPath;
            RebuildTree();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count > 0 && OnSelectionChange != null)
            {
                OnSelectionChange.Invoke(selectedIds[0]);
            }
        }
    }
}
#endif