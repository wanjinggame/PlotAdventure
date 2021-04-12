using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plot.Utility;
using Plot.Const;
using System;
using Plot.Core;

namespace Plot.UI
{
    public class UIManager : BaseManager
    {
        public Dictionary<int, string> uiPrefabs;
        public Dictionary<int, Type> uiTypes;

        public Dictionary<int, UIBase> openUIList;

        public Transform canvs;

        public override void Init()
        {
            base.Init();

            openUIList = new Dictionary<int, UIBase>();

            SetupUITypes();
            SetupUI();

            InitCanvas();

            if (GameLog.EnableLog(GameLog.LV_DEBUG))
            {
                GameLog.Log("UIManager Init Success...");
            }
        }

        public void InitCanvas()
        {

        }

        public void SetupUITypes()
        {
            uiTypes = new Dictionary<int, Type>();
            uiTypes.Add(UIConst.UI_ID_INIT, typeof(UIInitCtrl));
        }

        public void SetupUI()
        {
            uiPrefabs = new Dictionary<int, string>();
            //���ID   prefab��Դ��
            uiPrefabs.Add(UIConst.UI_ID_INIT, "Init_Panel");
        }

        public UIBase OpenUI(int uiId)
        {
            return OpenUI<UIBase>(uiId);
        }

        public T OpenUI<T>(int uiID) where T : UIBase
        {
            var path = GetPrefabPath(uiID);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            //to do  resourceManager load ui prefab
            var uiPrefab = GameApp.instance.resourceManager.Load<GameObject>(path);
            if (uiPrefab)
            {
                var go = UnityEngine.Object.Instantiate(uiPrefab);
                SetInCanvas(go);
                var ctrl = go.AddComponent<T>();
            }
            return null;
        }

        public bool IsOpen(int uiId)
        {

            return openUIList.ContainsKey(uiId);
        }

        public void CloseUI(int uiId)
        {
            if (!IsOpen(uiId))
            {
                return;
            }
        }

        public UIBase TryGetUI(int uiID)
        {
            if (openUIList.ContainsKey(uiID))
            {
                return openUIList[uiID];
            }
            return null;
        }

        public string GetPrefabPath(int uiId)
        {
            if (uiPrefabs.ContainsKey(uiId))
            {
                return uiPrefabs[uiId];
            }
            if (GameLog.EnableLog(GameLog.LV_ERROR))
            {
                GameLog.LogError(string.Format("no ui:{0}  config",uiId));
            }
            return "";
        }

        public void SetInCanvas(GameObject gameObject)
        {

        }
    }
}