using Plot.Core;
using Plot.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Plot.Resource
{
    public class AssetsManifestManager : BaseManager
    {
        private bool isInit = false;
        private AssetBundleManifest manifest;
        public const string manifestFileName = "StreamingAssets";
        private Dictionary<string, AssetBundle> assetBundleDics = new Dictionary<string, AssetBundle>();
        private Dictionary<string, string[]> dependenciePathsDic = new Dictionary<string, string[]>();
        private List<string> hasDependenciesPathList = new List<string>();
        public override void Init()
        {
            
            if(!isInit)
            { 
                base.Init();
                isInit = true;
                LoadAssetsManifest();
            }
        }
        
        public void LoadAssetsManifest() 
        {
            ResLoadLocation type = ResLoadLocation.Streaming;
            string path = PathUtils.GetAbsolutePath(type, manifestFileName);
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                type = ResLoadLocation.Persistent;
                path = PathUtils.GetAssetsBundlePersistentPath() + manifestFileName;
            }
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            ab.Unload(false);
        }

        public AssetBundleManifest GetManifest()
        {
            Init();
            return manifest;
        }

        public Dictionary<string, string[]> GetDependencieNamesDic()
        {
            Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
            foreach (var item in dependenciePathsDic)
            {
                List<string> names = new List<string>();
                foreach (var pathArr in item.Value)
                {
                    string name = PathUtils.GetFileName(pathArr);
                    names.Add(name);
                }
                dic.Add(PathUtils.GetFileName(item.Key), names.ToArray());
            }
            return dic;
        }

        private void LoadDependenciePaths()
        {
            dependenciePathsDic.Clear();
            string[] sArr = manifest.GetAllAssetBundles();
            for (int i = 0; i < sArr.Length; i++)
            {
                string assetPath = sArr[i];
                string[] dependenPaths = manifest.GetDirectDependencies(assetPath);
                string[] dependens = new string[dependenPaths.Length];
                for (int j = 0; j < dependenPaths.Length; j++)
                {
                    dependens[j] = PathUtils.GetLoadPath(AssetsLoadType.AssetBundle, dependenPaths[j]);
                }
                dependenciePathsDic.Add(PathUtils.GetLoadPath(AssetsLoadType.AssetBundle, assetPath), dependens);
            }
            hasDependenciesPathList.Clear();
            foreach (var assetPath in dependenciePathsDic.Keys)
            {
                bool hasDep = false;
                foreach (var depList in dependenciePathsDic.Values)
                {
                    foreach (var item in depList)
                    {
                        if (item == assetPath)
                        {
                            hasDep = true;
                            hasDependenciesPathList.Add(assetPath);
                            break;
                        }
                    }
                    if (hasDep)
                        break;
                }
            }


        }
        public string[] GetAllDependenciesPaths(string path)
        {
            if (!isInit)
            {
                Init();
            }
            if (dependenciePathsDic.ContainsKey(path))
                return dependenciePathsDic[path];
            return new string[0];
        }

        public bool IsHaveDependencies(string path)
        {
            if (!isInit)
            {
                Init();
            }
            if (hasDependenciesPathList.Contains(path))
            {
                return true;
            }
            return false;
        }

        public override void CommonUpdate()
        {
            base.CommonUpdate();
        }

        public override void CommonLateUpdate()
        {
            base.CommonLateUpdate();
        }

        public override void SetFPS(int fps)
        {
            base.SetFPS(fps);
        }

        public override void SetInfo(ManagerInfo info)
        {
            base.SetInfo(info);
        }
    }
}
