using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using Plot.Utility;
namespace Plot.Resource
{
    public class AssetBundleLoader : LoaderBase
    {
        public AssetBundleLoader() { }
        public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<AssetsData> callback)
        {
            throw new NotImplementedException();
        }

        public override AssetsData LoadAssets(string path)
        {
            throw new NotImplementedException();
        }

        public override AssetsData LoadAssets<T>(string path)
        {
            throw new NotImplementedException();
        }

       
    }
}
