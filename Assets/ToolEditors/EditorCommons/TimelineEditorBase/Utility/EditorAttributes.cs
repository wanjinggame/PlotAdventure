using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEditor.TimelineEditor
{
    public class EditorTypeCollection
    {
        public static Dictionary<Type, Type> targetsTypes;

        public static Type GetEditorType(Type targetType)
        {
            if (targetsTypes.ContainsKey(targetType))
            {
                return targetsTypes[targetType];
            }
            return null;
        }

        public static void Init()
        {
            targetsTypes = new Dictionary<Type, Type>();
            foreach (var type in ReflectionTools.GetImplementationsOf(typeof(IEditor)))
            {
                var editorTargetAttribute = type.RTGetAttribute<EditorTargetAttribute>(false);
                if (editorTargetAttribute != null)
                {
                    targetsTypes[editorTargetAttribute.type] = type;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EditorTargetAttribute : Attribute
    {
        public Type type;
        public EditorTargetAttribute(Type targetType)
        {
            this.type = targetType;
        }
    }
}

