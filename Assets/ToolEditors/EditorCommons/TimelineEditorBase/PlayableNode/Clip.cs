using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TimelineEditor
{
    public class Clip : PlayableNode<Clip>
    {
        private bool _inRange = false;

        public virtual float length
        {
            get;
            set;
        }

        public virtual string externalInfo { get; set; }

        [HideInInspector]
        public bool isLocked;
        [HideInInspector]
        public bool isValid = true;
        [HideInInspector]
        public Track parent;

        public override Color color
        {
            get
            {
#if UNITY_EDITOR
                return Prefs.clipBasicColor;
#else
                return new Color(1, 0.3f, 0.3f);
#endif
            }
        }

        float lastEndTime = -1; 
        public override float endTime
        {
            get
            {
                if (lastEndTime == -1) lastEndTime = startTime + length;
                var _endTime = startTime + length;
                if (_endTime != lastEndTime && sequence != null /*&& !sequence.dirty*/)
                {
                    sequence.dirty = true;
                }
                lastEndTime = _endTime;
                return _endTime;
            }
            set
            {
                if (startTime + length != value)
                {
                    length = Mathf.Max(value - startTime, 0);
                    blendOut = Mathf.Clamp(blendOut, 0, length - blendIn);
                    blendIn = Mathf.Clamp(blendIn, 0, length - blendOut);
                }
            }
        }

        public SequenceBase sequence { get { return parent.parent.parent; } }

        public virtual bool canCrossBlend { get; set; }

        public virtual float blendIn { get { return 0; } set { } }

        public virtual float blendOut { get { return 0; } set { } }

        protected override void OnSample(int frame)
        {
            if (!this.InFrameRange(frame))
            {
                if (_inRange)
                {
                    OnExit();
                }
                return;
            }

            if (!_inRange)
            {
                if (OnTrigger(frame))
                {
                    _inRange = true;
                }
            }
            OnUpdate(frame);
        }

        [HideInInspector]
        public bool isCollapsed;

        protected virtual bool OnTrigger(int frame) { return true; }
        protected virtual void OnUpdate(int frame) { }
        protected virtual void OnExit() { }
        virtual protected void OnEnter() { }
#if UNITY_EDITOR
        protected virtual void OnClipGUIExternal(Rect left, Rect right)
        {

        }
        protected virtual void OnClipGUI(Rect rect)
        {

        }
        public void ShowClipGUIExternal(Rect left, Rect right) { OnClipGUIExternal(left, right); }
        public void ShowClipGUI(Rect rect) { OnClipGUI(rect); }
#endif
    }
}

