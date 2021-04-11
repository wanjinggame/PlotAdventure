#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UnityEditor.TimelineEditor
{
    public interface IEditor
    {
        void Draw(Rect rect);
        void SetTarget(object target);
        object GetTarget();
        void Clear();
    }

    public class EditorBase<T> : IEditor, IDisposable where T : Clip
    {
        protected virtual T target { get; set; }

        protected virtual bool ShowFrame()
        {
            return true;
        }

        protected void SetTarget(T t)
        {
            target = t;
            OnSetTarget(target);
        }

        public virtual void Draw(Rect rect)
        {
            if (target == null) return;
            using (new GUILayout.VerticalScope("Box"))
            {
                GUI.color = new Color(0, 0, 0, 0.3f);
                using (new GUILayout.HorizontalScope(UnityEditor.TimelineEditor.Styles.headerBoxStyle))
                {
                    GUI.color = Color.white;
                    GUILayout.Label(string.Format("<size=22><b>{0}</b></size>", target.name));
                }
                using (new GUILayout.VerticalScope("Box"))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("StartTime", GUILayout.Width(rect.width / 3f - 5));
                        EditorGUILayout.LabelField("Lenght", GUILayout.Width(rect.width / 3f - 5));
                        EditorGUILayout.LabelField("EndTime", GUILayout.Width(rect.width / 3f - 5));
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        if (ShowFrame())
                        {
                            target.startTime = (float)EditorGUILayout.IntField(target.startFrame) / Prefs.frameRate;
                            target.length = (float)EditorGUILayout.IntField(target.endFrame - target.startFrame) / Prefs.frameRate;
                            target.endTime = (float)EditorGUILayout.IntField(target.endFrame) / Prefs.frameRate;
                        }
                        else
                        {
                            target.startTime = EditorGUILayout.FloatField(target.startTime);
                            target.length = EditorGUILayout.FloatField(target.length);
                            target.endTime = EditorGUILayout.FloatField(target.endTime);
                        }
                    }
                }
                if (target.length != 0)
                {
                    //using (new GUILayout.VerticalScope("Box"))
                    //{
                    //    target.blendIn = EditorGUILayout.Slider("BlendIn", target.blendIn, 0, target.length);
                    //    target.blendOut = EditorGUILayout.Slider("BlendOut", target.blendOut, 0, target.length);
                    //}
                }
            }
        }

        protected virtual void OnSetTarget(T t)
        {

        }

        protected virtual void OnDispose()
        {

        }

        public void SetTarget(object target)
        {
            if(target is T)
            {
                SetTarget(target as T);
            }
        }

        public virtual object GetTarget()
        {
            return target;
        }

        public void Dispose()
        {
            OnDispose();
            target = null;
        }

        public void Clear()
        {
            Dispose();
        }
    }
}
#endif
