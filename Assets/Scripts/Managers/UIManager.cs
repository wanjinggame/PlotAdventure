using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plot.Utility;
using Plot.Const;
using System;
using Plot.Core;
using UnityEngine.SceneManagement;
using System.Linq;

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
            canvs = GameObject.Find("Canvas").transform;
            GameObject.DontDestroyOnLoad(canvs);
        }

        public void SetupUITypes()
        {
            uiTypes = new Dictionary<int, Type>();
            uiTypes.Add(UIConst.UI_ID_INIT, typeof(UIInitCtrl));
            uiTypes.Add(UIConst.UI_ID_MAIN, typeof(UIMainCtrl));
        }

        public void SetupUI()
        {
            uiPrefabs = new Dictionary<int, string>();
            //面板ID   prefab资源名
            uiPrefabs.Add(UIConst.UI_ID_INIT, "Init_Panel");
            uiPrefabs.Add(UIConst.UI_ID_MAIN, "MainPlane");
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
            path = string.Concat("Prefab/UIPrefab/", path);
            var uiPrefab = GameApp.instance.resourceManager.LoadResourceAssets<GameObject>(path);
            if (uiPrefab)
            {
                var go = UnityEngine.Object.Instantiate(uiPrefab);
                SetInCanvas(go.transform);
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
                GameLog.LogError(string.Format("no ui:{0}  config", uiId));
            }
            return "";
        }

        public void SetInCanvas(Transform tran)
        {
            tran.SetParent(canvs);
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one;
            RectTransform rect = tran.GetComponent<RectTransform>();
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
    }
}
