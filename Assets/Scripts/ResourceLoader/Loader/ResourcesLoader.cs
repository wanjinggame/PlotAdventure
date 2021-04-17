using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using Plot.Utility;
namespace Plot.Resource
{
    public class ResourcesLoader : LoaderBase
    {
        public ResourcesLoader() { }
        public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<AssetsData> callback)
        {
            AssetsData assetsData = null;
            string newPath = PathUtils.RemoveExtension(path);
            ResourceRequest resourceRequest = null;
            if(resType != null)
            {
                resourceRequest = Resources.LoadAsync(newPath, resType);
            }
            else
            {
                resourceRequest = Resources.LoadAsync(newPath);
            }
            yield return resourceRequest;
            if (resourceRequest.asset != null)
            {
                assetsData = new AssetsData(path);
                assetsData.Assets = new Object[] { resourceRequest.asset };
            }
            else
            {
                Debug.LogError("º”‘ÿ ß∞‹,Path:" + path);
            }
            if (callback != null)
                callback(assetsData);
            yield return new WaitForEndOfFrame();

        }

        public override AssetsData LoadAssets(string path)
        {
            AssetsData assetsData = null;
            string newPath = PathUtils.RemoveExtension(path);
            Object objs = Resources.Load(newPath);
            if(objs != null)
            {
                assetsData = new AssetsData(path);
                assetsData.Assets = new Object[] { objs };
            }
            else
            {
                Debug.LogError("º”‘ÿ ß∞‹,Path:" + path);
            }

            return assetsData;
        }

        public override AssetsData LoadAssets<T>(string path)
        {
            AssetsData assetsData = null;
            string newPath = PathUtils.RemoveExtension(path);
            T ass = Resources.Load<T>(newPath);
            if(ass != null)
            {
                assetsData = new AssetsData(path);
                assetsData.Assets = new Object[] { ass };
            }
            else
            {
                Debug.LogError("º”‘ÿ ß∞‹,Path:" + path);
            }

            return assetsData;
        }
    }
}