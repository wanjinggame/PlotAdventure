using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEditor.TimelineEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AttachableAttribute : Attribute
    {
        public Type[] types;
        public AttachableAttribute(params Type[] types)
        {
            this.types = types;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public string name;
        /// <summary>
        /// 标识是否是老技能框架上的轨道和Clip
        /// </summary>
        public bool old;
        public NameAttribute(string name, bool isOld = true)
        {
            this.name = name;
            old = isOld;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EffectTypeAttribute : Attribute
    {
        public Type type;
        public EffectTypeAttribute(Type type)
        {
            this.type = type;
        }
    }
}
