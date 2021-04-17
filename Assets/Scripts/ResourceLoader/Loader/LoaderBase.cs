using System;
using System.Collections;
using System.Collections.Generic;

namespace Plot.Resource
{
    public abstract class LoaderBase
    {
        public LoaderBase(){ }
        public abstract IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<AssetsData> callback);
        public abstract AssetsData LoadAssets(string path);
        public abstract AssetsData LoadAssets<T>(string path) where T : UnityEngine.Object;
        public virtual string[] GetAllDependenciesName(string name) { return new string[0]; }
        public virtual bool IsHaveDependencies(string name) { return false; }
    }
}