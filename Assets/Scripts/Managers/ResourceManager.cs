using Plot.Core;
using Plot.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plot.Resource
{
    public class ResourceManager : BaseManager
    {
        private List<Object> loadedObjs;
        private AssetsLoadController loadController;
        private AssetsLoadType loadType;

        public AssetsLoadController LoadController 
        {
            get { return loadController; }
        }

        public AssetsLoadType LoadType
        {
            get { return loadType; }
        }

        public override void Init()
        {
            base.Init();
            loadedObjs = new List<Object>();
            InitLoadType();
            if (GameLog.EnableLog(GameLog.LV_DEBUG))
            {
                GameLog.Log("ResourceManager Init Success...");
            }
        }

        public override void CommonUpdate()
        {
            base.CommonUpdate();

        }

        public override void CommonLateUpdate()
        {
            base.CommonLateUpdate();

        }

        public override void Destroy()
        {
            base.Destroy();

        }

        private void InitLoadType()
        {
            loadType = Application.platform == RuntimePlatform.WindowsEditor ? AssetsLoadType.Resources : AssetsLoadType.AssetBundle;
            loadController = new AssetsLoadController(loadType);
        }


        public Object LoadResourceAssets(string name)
        {
            //string path = PathUtils.GetAbsolutePath(ResLoadLocation.Resource, name);
            AssetsData assets = loadController.LoadAssets(name);
            if (assets != null)
                return assets.Assets[0];
            return null;
        }

        public void LoadResourceAsync(string name, Action<Object> callBack)
        {
            //string path = PathUtils.GetAbsolutePath(ResLoadLocation.Resource, name);
            loadController.LoadAsync(name, null, callBack);
        }
        public void LoadResourceAsync(string name, Type resType, Action<Object> callBack)
        {
            //string path = PathUtils.GetAbsolutePath(ResLoadLocation.Resource, name);
            loadController.LoadAsync(name, resType, callBack);
        }

        public T LoadResourceAssets<T>(string name) where T : Object
        {
            T res = null;
            //string path = PathUtils.GetAbsolutePath(ResLoadLocation.Resource, name);
            AssetsData assets = loadController.LoadAssets<T>(name);
            if (assets != null)
                res = assets.GetAssets<T>();
            if(res == null)
            {
                Debug.LogError("Error=> Load Name :" + name + "  Type:" + typeof(T).FullName + "\n" + " Load Object:" + res);
            }
            return res;
        }

            public T Load<T>(string resPath) where T : Object
        {

            return null;
        }
    }
}
