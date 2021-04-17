using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Plot.Resource
{
    /// <summary>
    /// ������Դ��Ϣ��
    /// </summary>
    public class AssetsData
    {
        public int refCount = 0;            //��Դ���ô���
        public string assetName = "";       //��Դ��
        public string assetPath;            //��Դ·��
        private Object[] assets;
        private AssetBundle assetBundle;
        private long objectsSize = -1;
        private long bundleSize = -1;

        public AssetsData(string path)
        {
            assetPath = path;
            assetName = Path.GetFileNameWithoutExtension(assetPath);
            objectsSize = -1;
            bundleSize = -1;
        }

        public Object[] Assets 
        {
            get { return assets; }
            set
            {
                assets = value;
                objectsSize = 0;
                if(assets != null)
                {
                    foreach (var item in assets)
                    {
                        objectsSize += Profiler.GetRuntimeMemorySizeLong(item);
                    }
                }
            }
        }


        public T GetAssets<T>() where T : Object
        {
            foreach (var item in assets)
            {
                if (item.GetType() == typeof(T))
                    return (T)item;
            }
            return default(T);
        }


        public AssetBundle AssetBundle
        {
            get
            {
                return assetBundle;
            }
            set
            {
                assetBundle = value;
                bundleSize = 0;
                if (assetBundle != null)
                {
                    bundleSize = Profiler.GetRuntimeMemorySizeLong(assetBundle);
                }
            }
        }

        /// <summary>
        /// ��ȡ��Դ��ռ���ڴ�Ĵ�С
        /// </summary>
        /// <returns></returns>
        public long GetObjectsMemorySize()
        {
            return objectsSize;
        }
        public long GetBundleMemorySize()
        {
            return bundleSize;
        }
        public long GetTotalMemorySize()
        {
            return GetObjectsMemorySize() + GetBundleMemorySize();
        }




    }
}