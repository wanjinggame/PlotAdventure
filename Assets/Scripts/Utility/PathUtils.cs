using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
namespace Plot.Utility
{

    public class PathUtils
    {

        /// <summary>
        /// ÒÆ³ýÀ©Õ¹Ãû
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