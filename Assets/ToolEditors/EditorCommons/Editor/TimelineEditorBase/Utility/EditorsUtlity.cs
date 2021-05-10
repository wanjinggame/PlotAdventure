#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace UnityEditor.TimelineEditor
{
    public class EditorsUtlity 
    {
        private static Dictionary<Type, IEditor> editors;

        public static IEditor GetEditor(Type targetType)
        {
            if (editors == null)
            {
                editors = new Dictionary<Type, IEditor>();
            }
            if (editors.ContainsKey(targetType))
            {
                return editors[targetType];
            }
            var editorType = EditorTypeCollection.GetEditorType(targetType);
            if (editorType != null)
            {
                return editors[targetType] = Activator.CreateInstance(editorType) as IEditor;
            }
            else
            {
                return editors[typeof(Clip)] = Activator.CreateInstance(EditorTypeCollection.GetEditorType(typeof(Clip))) as IEditor;
            }
        }

        public static void ClearEditor()
        {
            if (editors == null) return;
            foreach(var editor in editors)
            {
                if (editor.Value != null)
                {
                    var editorBase = editor.Value;
                    editorBase.Clear();
                }
            }
        }
    }
}
#endif
