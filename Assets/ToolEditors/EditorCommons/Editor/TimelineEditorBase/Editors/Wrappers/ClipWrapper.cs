using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.Linq;

namespace UnityEditor.TimelineEditor
{
    public class ClipWrapper
    {
        const float CLIP_DOPESHEET_HEIGHT = 13f;
        const float SCALE_RECT_WIDTH = 4;

        public Clip action;
        public bool isScalingStart;
        public bool isScalingEnd;
        public bool isControlBlendIn;
        public bool isControlBlendOut;
        public float preScaleStartTime;
        public float preScaleEndTime;

        public Clip previousClip;
        public Clip nextClip;

        public Event e;
        private float overlapIn;
        private float overlapOut;
        private float blendInPosX;
        private float blendOutPosX;
        private bool hasActiveParameters = false;
        private bool hasParameters;
        private float pointerTime;
        private float snapedPointerTime;
        private bool isScalable;
        private Dictionary<AnimationCurve, Keyframe[]> retimingKeys;

        private ITimeline editor;

        private List<ClipWrapper> multiSelection
        {
            get { return editor.multiSelection; }
            set { editor.multiSelection = value; }
        }

        private Rect _rect;
        public Rect rect
        {
            get { return action.isCollapsed ? default(Rect) : _rect; }
            set { _rect = value; }
        }

        public ClipWrapper(Clip action, ITimeline timelineEditorBase)
        {
            editor = timelineEditorBase;
            this.action = action;
        }

        public void ResetInteraction()
        {
            isControlBlendIn = false;
            isControlBlendOut = false;
            isScalingStart = false;
            isScalingEnd = false;
        }

        public void OnClipGUI(bool useCurEvent = true)
        {
            if (useCurEvent)
            {
                e = Event.current;
            }
            overlapIn = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
            overlapOut = nextClip != null ? Mathf.Max(action.endTime - nextClip.startTime, 0) : 0;
            blendInPosX = (action.blendIn / action.length) * rect.width;
            blendOutPosX = ((action.length - action.blendOut) / action.length) * rect.width;

            pointerTime = editor.PosToTime(editor.mousePosition.x);
            snapedPointerTime = editor.SnapTime(pointerTime);

            var lengthProp = action.GetType().GetProperty("length", BindingFlags.Instance | BindingFlags.Public);
            isScalable = lengthProp != null && lengthProp.DeclaringType != typeof(Clip) && lengthProp.CanWrite && action.length > 0;

            //...
            var localRect = new Rect(0, 0, rect.width, rect.height);
            if (action.isLocked)
            {
                if (e.isMouse && localRect.Contains(e.mousePosition))
                {
                    e.Use();
                }
            }

            action.ShowClipGUI(localRect);
            if (hasActiveParameters && action.length > 0)
            {
                ShowClipDopesheet(localRect);
            }
            //...


            //BLEND GRAPHICS
            if (action.blendIn > 0)
            {
                Handles.color = Color.black.WithAlpha(0.5f);
                Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(0, rect.height), new Vector2(blendInPosX, 0) });
                Handles.color = Color.black.WithAlpha(0.3f);
                Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(0, 0), new Vector3(0, rect.height), new Vector3(blendInPosX, 0) });
            }

            if (action.blendOut > 0 && overlapOut == 0)
            {
                Handles.color = Color.black.WithAlpha(0.5f);
                Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(blendOutPosX, 0), new Vector2(rect.width, rect.height) });
                Handles.color = Color.black.WithAlpha(0.3f);
                Handles.DrawAAConvexPolygon(new Vector3[] { new Vector3(rect.width, 0), new Vector2(blendOutPosX, 0), new Vector2(rect.width, rect.height) });
            }

            if (overlapIn > 0)
            {
                Handles.color = Color.black;
                Handles.DrawAAPolyLine(2, new Vector3[] { new Vector2(blendInPosX, 0), new Vector2(blendInPosX, rect.height) });
            }

            Handles.color = Color.white;

            //SCALING IN/OUT, DRAG RECTS
            var allowScaleIn = isScalable && rect.width > SCALE_RECT_WIDTH * 2;
            var dragRect = new Rect((allowScaleIn ? SCALE_RECT_WIDTH : 0), 0, (isScalable ? rect.width - (allowScaleIn ? SCALE_RECT_WIDTH * 2 : SCALE_RECT_WIDTH) : rect.width), rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
            editor.AddCursorRect(dragRect, MouseCursor.Link);

            var controlRectIn = new Rect(0, 0, SCALE_RECT_WIDTH, rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
            var controlRectOut = new Rect(rect.width - SCALE_RECT_WIDTH, 0, SCALE_RECT_WIDTH, rect.height - (hasActiveParameters ? CLIP_DOPESHEET_HEIGHT : 0));
            if (isScalable && action.parent.isActive)
            {
                GUI.color = new Color(0, 1, 1, 0.3f);
                if (overlapOut <= 0)
                {
                    editor.AddCursorRect(controlRectOut, MouseCursor.ResizeHorizontal);
                    if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
                    {
                        if (controlRectOut.Contains(e.mousePosition))
                        {
                            isScalingEnd = true;
                            preScaleStartTime = action.startTime;
                            preScaleEndTime = action.endTime;
                            e.Use();
                        }
                    }
                }

                if (overlapIn <= 0 && allowScaleIn)
                {
                    editor.AddCursorRect(controlRectIn, MouseCursor.ResizeHorizontal);
                    if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
                    {
                        if (controlRectIn.Contains(e.mousePosition))
                        {
                            isScalingStart = true;
                            preScaleStartTime = action.startTime;
                            preScaleEndTime = action.endTime;
                            e.Use();
                        }
                    }
                }
                GUI.color = Color.white;
            }

            //BLENDING IN/OUT
            if (e.type == EventType.MouseDown && e.button == 0 && e.control)
            {
                var blendInProp = action.GetType().GetProperty("blendIn", BindingFlags.Instance | BindingFlags.Public);
                var isBlendableIn = blendInProp != null && blendInProp.DeclaringType != typeof(Clip) && blendInProp.CanWrite;
                var blendOutProp = action.GetType().GetProperty("blendOut", BindingFlags.Instance | BindingFlags.Public);
                var isBlendableOut = blendOutProp != null && blendOutProp.DeclaringType != typeof(Clip) && blendOutProp.CanWrite;
                if (isBlendableIn && controlRectIn.Contains(e.mousePosition))
                {
                    isControlBlendIn = true;
                    e.Use();
                }
                if (isBlendableOut && controlRectOut.Contains(e.mousePosition))
                {
                    isControlBlendOut = true;
                    e.Use();
                }
            }


            if (isControlBlendIn)
            {
                action.blendIn = Mathf.Clamp(pointerTime - action.startTime, 0, action.length - action.blendOut);
            }

            if (isControlBlendOut)
            {
                action.blendOut = Mathf.Clamp(action.endTime - pointerTime, 0, action.length - action.blendIn);
            }

            if (isScalingStart)
            {
                var prev = previousClip != null ? previousClip.endTime : 0;
                if (Prefs.magnetSnapping && !e.control)
                { //magnet snap
                    if (Mathf.Abs(snapedPointerTime - prev) <= editor.magnetSnapInterval)
                    {
                        snapedPointerTime = prev;
                    }
                }

                if (action.CanCrossBlend(previousClip))
                {
                    prev -= Mathf.Min(action.length / 2, previousClip.length / 2);
                }
                action.startTime = snapedPointerTime;
                action.startTime = Mathf.Clamp(action.startTime, prev, preScaleEndTime);
                action.endTime = preScaleEndTime;
                editor.pendingGuides.Add(new TimelineWindowBase.GuideLine(action.startTime, Color.white.WithAlpha(0.05f)));
            }

            if (isScalingEnd)
            {
                var next = nextClip != null ? nextClip.startTime : editor.maxTime;
                if (Prefs.magnetSnapping && !e.control)
                { //magnet snap
                    if (Mathf.Abs(snapedPointerTime - next) <= editor.magnetSnapInterval)
                    {
                        snapedPointerTime = next;
                    }
                }

                if (action.CanCrossBlend(nextClip))
                {
                    next += Mathf.Min(action.length / 2, nextClip.length / 2);
                }
                action.endTime = snapedPointerTime;
                action.endTime = Mathf.Clamp(action.endTime, 0, next);
                editor.pendingGuides.Add(new TimelineWindowBase.GuideLine(action.endTime, Color.white.WithAlpha(0.05f)));
            }

            if (e.type == EventType.MouseDrag && e.button == 0 && dragRect.Contains(e.mousePosition))
            {
                editor.anyClipDragging = true;
            }

            if (e.type == EventType.MouseDown)
            {
                if (e.control)
                {
                    if (multiSelection == null)
                    {
                        multiSelection = new List<ClipWrapper>() { this };
                    }
                    if (multiSelection.Contains(this))
                    {
                        multiSelection.Remove(this);
                    }
                    else
                    {
                        multiSelection.Add(this);
                    }
                }
                else
                {
                    CutsceneUtility.selectedObject = action;
                    if (multiSelection != null && !multiSelection.Select(cw => cw.action).Contains(action))
                    {
                        multiSelection = null;
                    }
                }
                if (e.clickCount == 2)
                {
                    editor.OnClipDoubleClick(rect, action);
                }
            }


            if (e.rawType == EventType.ContextClick)
            {
                DoClipContextMenu();
            }

            if (e.rawType == EventType.MouseUp)
            {
                ResetInteraction();
            }

            if (e.button == 0)
            {
                GUI.DragWindow(dragRect);
            }
            var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;
            //Draw info text if big enough
            if (rect.width > 40)
            {
                var r = new Rect(0, 0, rect.width, rect.height);
                if (overlapIn > 0) { r.xMin = blendInPosX; }
                if (overlapOut > 0) { r.xMax = blendOutPosX; }
                var label = string.Format("<size=10>{0} start:{1}\n{2}</size>",
                    action.name, doFrames ? action.startFrame : action.startTime, action.externalInfo);
                GUI.color = action.color.grayscale >= 0.6 ? Color.black : Color.white;
                GUI.Label(r, label);
                GUI.color = Color.white;
            }
        }

        //Show the clip dopesheet
        void ShowClipDopesheet(Rect rect)
        {
            var dopeRect = new Rect(0, rect.height - CLIP_DOPESHEET_HEIGHT, rect.width, CLIP_DOPESHEET_HEIGHT);
            GUI.color = EditorGUIUtility.isProSkin ? new Color(0, 0.2f, 0.2f, 0.5f) : new Color(0, 0.8f, 0.8f, 0.5f);
            GUI.Box(dopeRect, string.Empty, Styles.clipBoxHorizontalStyle);
            GUI.color = Color.white;
            //DopeSheetEditor.DrawDopeSheet(action.animationData, action, dopeRect, 0, action.length, false);
        }

        //CONTEXT
        void DoClipContextMenu()
        {

            var menu = new GenericMenu();

            if (multiSelection != null && multiSelection.Contains(this))
            {
                menu.AddItem(new GUIContent("Delete Clips"), false, () =>
                {
                    editor.SafeDoAction(() =>
                    {
                        foreach (var act in multiSelection.Select(b => b.action).ToArray())
                        {
                            (act.parent as Track).DeleteClip(act);
                        }
                        editor.InitClipWrappers();
                        multiSelection = null;
                    });
                });

                menu.ShowAsContext();
                e.Use();
                return;
            }


            menu.AddItem(new GUIContent("Copy Clip"), false, () =>
            {
                editor.SafeDoAction(() => { CutsceneUtility.CopyClip(action); });
            });

            var clips = action.parent.clips.FindAll((t) => { return t.startTime == action.startTime && t.length <= action.length; });
            if (clips != null && clips.Count > 1)
            {
                int index = 0;
                foreach (var clip in clips)
                {
                    menu.AddItem(new GUIContent("重叠的clip/" + clip.name + "  " + index), false, () =>
                    {
                        CutsceneUtility.selectedObject = clip;
                    });
                    index++;
                }
            }

            menu.AddSeparator("/");

            menu.AddItem(new GUIContent("Delete Clip"), false, () =>
            {
                editor.SafeDoAction(() => { (action.parent as Track).DeleteClip(action); editor.InitClipWrappers(); });
            });

            menu.ShowAsContext();
            e.Use();
        }
    }
}
    

