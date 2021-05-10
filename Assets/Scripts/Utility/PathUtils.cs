using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using Plot.Resource;
using System.Text;

namespace Plot.Utility
{

    public enum ResLoadLocation
    {
        Resource,
        Streaming,
        Persistent,
        Catch,
    }

    public class PathUtils
    {

        public static string GetPath(ResLoadLocation loadType)
        {
            StringBuilder path = new StringBuilder();
            switch(loadType)
            {
                case ResLoadLocation.Resource:
#if UNITY_EDITOR
                    path.Append(Application.dataPath);
                    path.Append("/Resources/");
                    break;

#endif
                case ResLoadLocation.Streaming:
#if UNITY_ANDROID && !UNITY_EDITOR
                    path.Append(Application.dataPath );
                    path.Append("!assets/");
#else
                    path.Append(Application.streamingAssetsPath);
                    path.Append("/");
#endif
                    break;
                case ResLoadLocation.Persistent:
                    path.Append(Application.persistentDataPath);
                    path.Append("/");
                    break;
                case ResLoadLocation.Catch:
                    path.Append(Application.temporaryCachePath);
                    path.Append("/");
                    break;
                default:
                    Debug.LogError("Type Error !" + loadType);
                    break;
            }
            return path.ToString();
        }

        /// <summary>
        /// 绝对路径
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="relativelyPath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(ResLoadLocation loadType, string relativelyPath)
        {
            return GetPath(loadType) + relativelyPath;
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="expandName"></param>
        /// <returns></returns>
        public static string GetRelativelyPath(string path, string fileName, string expandName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(path);
            builder.Append("/");
            builder.Append(fileName);
            builder.Append(".");
            builder.Append(expandName);

            return builder.ToString();
        }

        public static string GetFileName(string path)
        {
            string name = "";
            try
            {
                name = Path.GetFileName(path);
            }
            catch 
            {
                path = path.Replace("\\", "/");
                if (!path.Contains("/"))
                {
                    return path;
                }
                string[] ss = path.Split(new string[] { "/" }, StringSplitOptions.None);
                if (ss.Length > 0)
                    return ss[ss.Length - 1];
            }
            return name; 
        }
        public static string GetAssetsBundlePersistentPath()
        {
            return Application.persistentDataPath + "/Resources/";
        }

        public static string GetLoadPath(AssetsLoadType assetsloadType, string name)
        {
            string path = string.Empty;
            if (assetsloadType == AssetsLoadType.Resources)
            {
                path = GetAbsolutePath(ResLoadLocation.Resource, name);
            }
            else
            {
                path = GetAssetsBundlePersistentPath() + path;
            }
            return path;

        }


        /// <summary>
        /// 移除扩展名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveExtension(string path)
        {
            string ss = path;
            if (Path.HasExtension(path))
                ss = Path.ChangeExtension(path, null);
            return ss;
        }


    }
}