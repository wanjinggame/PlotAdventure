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
        public const string manifestFileName = "StreamingAssets";
        private AssetBundleManifest manifest;
        private Dictionary<string, AssetBundle> assetBundleDics = new Dictionary<string, AssetBundle>();

        public AssetBundleLoader() { }

        public void LoadAssetManifest()
        {
            ResLoadLocation type = ResLoadLocation.Streaming;
            string path = PathUtils.GetAbsolutePath(type, manifestFileName);
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                type = ResLoadLocation.Persistent;
                path = PathUtils.GetAssetsBundlePersistentPath()+ manifestFileName;
            }
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            
            ab.Unload(false);
        }

        public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<AssetsData> callback)
        {
            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
            yield return req;
            AssetBundle ab = req.assetBundle;
            AssetBundleRequest abReq = null;
            if (resType != null)
                abReq = ab.LoadAllAssetsAsync(resType);
            else
                abReq = ab.LoadAllAssetsAsync();
            yield return abReq;
            AssetsData ad = new AssetsData(path);
            if (!assetBundleDics.ContainsKey(path))
            {
                assetBundleDics.Add(path, ab);
            }
            ad.Assets = abReq.allAssets;
            ad.AssetBundle = ab;
            if (callback != null)
                callback(ad);

            yield return 0;
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
