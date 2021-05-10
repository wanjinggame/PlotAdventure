#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace UnityEditor.TimelineEditor
{
    ///Utilities specific to Cutscenes
    public static class CutsceneUtility
    {
        [System.NonSerialized]
        private static System.Type copyType;
        [System.NonSerialized]
        private static PlayableNode _selectedObject;

        private static Clip copyClip;

        ///Raised when directable selection change
        public static event System.Action<PlayableNode> onSelectionChange;

        public struct ChangedParameterCallbacks
        {
            public System.Action Restore;
            public System.Action Commit;
            public ChangedParameterCallbacks(System.Action restore, System.Action commit)
            {
                Restore = restore;
                Commit = commit;
            }
        }

        ///Currently selected directable element
        public static PlayableNode selectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                if (onSelectionChange != null)
                {
                    onSelectionChange(value);
                }
            }
        }

        public static System.Type GetCopyType()
        {
            return copyType;
        }

        public static void FlushCopy()
        {
            copyType = null;
            copyClip = null;
        }

        public static bool CanPastClip(List<EditorTools.TypeMetaInfo> types)
        {
            if (copyClip == null) return false;
            foreach (var t in types)
            {
                if (t.type == copyType) return true;
            }
            return false;
        }

        public static void CopyClip(Clip action)
        {
            copyClip = ReflectionTools.Clone(action) as Clip;
            copyType = action.GetType();
        }

        public static Clip PasteClip(Track track, float time)
        {
            if (copyClip != null && track != null)
            {
                var newAction = ReflectionTools.Clone(copyClip) as Clip;
                newAction.startTime = time;
                track.AddNode(newAction);
                var nextAction = track.clips.FirstOrDefault(a => a.startTime > newAction.startTime);
                if (nextAction != null && newAction.endTime > nextAction.startTime)
                {
                    newAction.endTime = nextAction.startTime;
                }
                return newAction;
            }
            return null;
        }
    }

    public static class ColorUtility
    {
        ///A greyscale color
        public static Color Grey(float value)
        {
            return new Color(value, value, value);
        }

        ///The color, with alpha
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }

}


#endif