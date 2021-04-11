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

        public override void Init()
        {
            base.Init();

            openUIList = new Dictionary<int, UIBase>();

            SetupUITypes();
            SetupUI();
        }

        public void SetupUITypes()
        {
            uiTypes = new Dictionary<int, Type>();
            uiTypes.Add(UIConst.UI_ID_INIT, typeof(UIInitCtrl));
        }

        public void SetupUI()
        {
            uiPrefabs = new Dictionary<int, string>();
            //面板ID   prefab资源名
            uiPrefabs.Add(UIConst.UI_ID_INIT, "Init_Panel");
        }

        public void OpenUI(int uiId)
        {

        }

        public T OpenUI<T>(int uiID) where T : UIBase
        {

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
    }
}
