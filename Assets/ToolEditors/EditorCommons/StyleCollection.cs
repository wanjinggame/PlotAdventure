using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
    [CreateAssetMenu()]
    public class StyleCollection : ScriptableObject
    {
        private static StyleCollection _instance;
        public static StyleCollection instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<StyleCollection>("Assets/ToolEditors/EditorCommons/Editor/Asset/StyleCollection.asset");
                }
                return _instance;
            }
        }

        public Texture2D menuTreeBg;
        public Color menuTreeBgColor;
    }
}

