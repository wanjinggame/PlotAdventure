using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using Plot.Utility;
using System.Data;
using UnityEditor;

namespace Plot.Resource
{
    public class AssetsLoadController
    {
        private Dictionary<string, AssetsData> assetsDics = new Dictionary<string, AssetsData>();
        private LoaderBase loader;
        private AssetsLoadType loadType;


        public AssetsLoadController(AssetsLoadType loadType)
        {
            this.loadType = loadType;
            if (loadType == AssetsLoadType.Resources)
            {
                loader = new ResourcesLoader();
            }
            else
            {
                loader = new AssetBundleLoader();
            }
        }

        public Dictionary<string, AssetsData> GetLoadAssets()
        {
            return assetsDics;
        }

        #region 加载资源
        private void LoadAssetsDependencie(string path)
        {
            string[] dependenciesNames = loader.GetAllDependenciesName(path);
            foreach (var item in dependenciesNames)
            {
                LoadAssets(item);
            }
        }
        public AssetsData LoadAssetsLogic(string path, Func<bool> checkContainsAssets, Func<string, AssetsData> loadFunc)
        {
            LoadAssetsDependencie(path);
            AssetsData assets = null;
            if (checkContainsAssets())
            {
                assets = assetsDics[path];
            }
            else
            {
                assets = loadFunc(path);
                if (assets == null)
                {
                    Debug.LogError("资源加载失败：" + path);
                    return assets;
                }
                else
                {
                    if (assetsDics.ContainsKey(path))
                    {
                        List<Object> list = new List<Object>(assetsDics[path].Assets);
                        foreach (var item in assets.Assets)
                        {
                            if (!list.Contains(item))
                            {
                                list.Add(item);
                            }
                        }
                        assetsDics[path].Assets = list.ToArray();
                        assets = assetsDics[path];
                    }
                    else
                    {
                        assetsDics.Add(path, assets);
                    }
                }
            }
            return assets;
        }
        public AssetsData LoadAssets(string path)
        {
            return LoadAssetsLogic(path, () => {
                if (assetsDics.ContainsKey(path))
                {
                    return true;
                }
                return false;
            },
            (p) =>
            {
                return loader.LoadAssets(p);
            });
        }

        public AssetsData LoadAssets<T>(string path) where T : Object
        {
            return LoadAssetsLogic(path, () =>
            {
                if (assetsDics.ContainsKey(path))
                {
                    T res = assetsDics[path].GetAssets<T>();
                    if (res != null)
                        return true;
                }
                return false;
            },
            (p) =>
            {
                return loader.LoadAssets<T>(p);
            });
        }


        private IEnumerator LoadAssetsIDependencieEnumerator(string path)
        {
            string[] dependenciesNames = loader.GetAllDependenciesName(path);

            foreach (var item in dependenciesNames)
            {
                yield return LoadAssetsIEnumerator(item, null, null);
            }
        }

        private IEnumerator LoadAssetsIEnumerator(string path, Type assetType, Action<Object> callback)
        {
            if (assetsDics.ContainsKey(path))
            {
                AssetsData assets = assetsDics[path];
                assets.refCount++;

                if (callback != null)
                {
                    callback(assets.Assets[0]);
                }
            }
            else
            {
                yield return loader.LoadAssetsIEnumerator(path, assetType, (assets) =>
                {
                    assetsDics.Add(path, assets);
                    if (callback != null)
                        callback(assets.Assets[0]);
                });
            }

            yield return 0;
        }



        public void LoadAsync(string path, Type assetType, Action<Object> callback)
        {
            MonoRuntime.Instance.StartCoroutine(LoadAssetsIEnumerator(path, assetType, callback));
        }


        #endregion


        #region 卸载资源

        public void DestoryAssetsCounter(string path)
        {
            if (assetsDics.ContainsKey(path))
            {
                AssetsData assets = assetsDics[path];
                assets.refCount--;

                if (assets.refCount < 0)
                {
                    Debug.LogError("资源引用计数错误：(" + assets.refCount + ") " + assets.assetPath);
                    assets.refCount = 0;
                }
                if (assets.refCount == 0)
                {
                    //TO DO
                }
                string[] dependenciesNames = loader.GetAllDependenciesName(path);
                foreach (var item in dependenciesNames)
                {
                    DestoryAssetsCounter(item);
                }
            }
            else
            {
                Debug.LogError("未加载资源，不能Destroy ：" + path);
            }
        }

        public void ReleaseAll(bool isForceAB)
        {
            foreach (var item in assetsDics)
            {
                UnloadAssets(item.Value, isForceAB);
            }
            assetsDics.Clear();
        }

        public void Release(string path)
        {
            AssetsData assets = null;
            if(assetsDics.TryGetValue(path, out assets))
            {
                if(assets.refCount <= 0)
                {
                    UnloadAssets(assets, true);
                    assetsDics.Remove(path);
                    Debug.LogWarning("彻底释放" + path);
                }
            }
        }

        public void UnloadAssets(AssetsData assets, bool isForceAB)
        {
            if (assets.Assets != null && isForceAB)
            {
                foreach (var item in assets.Assets)
                {
                    Debug.LogWarning("释放资源" + item);
                    UnloadObject(item);
                }
                assets.Assets = null;
            }
            if (assets.AssetBundle != null)
                assets.AssetBundle.Unload(isForceAB);
        }

        public void UnloadObject(Object obj)
        {
            if (obj == null)
                return;
            if (obj is Shader)
                return;
            if (!(obj is GameObject) && !(obj is Component) && !(obj is AssetBundle))
            {
                Resources.UnloadAsset(obj);
            } else if ((obj is GameObject) || (obj is Component))
            {
                if (loadType == AssetsLoadType.AssetBundle)
                    Object.DestroyImmediate(obj, true);
            }
            else
            {
                AssetBundle ab = (AssetBundle)obj;
                ab.Unload(true);
            }
        }

        #endregion

    }
}
