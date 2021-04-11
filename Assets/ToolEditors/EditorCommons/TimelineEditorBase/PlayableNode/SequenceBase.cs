using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TimelineEditor
{
    public class SequenceBase : PlayableNode<Group>
    {
        public List<Group> groups
        {
            get
            {
                return _subPlayables;
            }
        }
        private bool _dirty = false;
        public bool dirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        public string path;

        public float viewTimeMax;
        public float viewTimeMin;

        public override float startTime { get { return 0; } }
        public override float endTime { get { return length; } }

        public float currentTime { get; set; }
        public bool isPlaying { get { return _isPlaying; } }

        private float _lenght = 50;
        public float length { get { return _lenght; } set { _lenght = value == 0 ? 50 : value; } }

        public void SetCurrentTime(float time)
        {
            currentTime = time;
            if (time >= length)
            {
                Stop();
            }
            else
            {
                Sample((int)(time * _frameRate));
            }
        }

        protected override void OnStop()
        {
            currentTime = 0;
        }

        public void Validate()
        {
            //throw new NotImplementedException();
        }

        protected override void OnAddNode(Group t)
        {
            base.OnAddNode(t);
            if (t != null)
            {
                t.parent = this;
            }
        }

        public virtual void DeleteGroup(Group group)
        {

        }

        public virtual void FromSequence(SequenceBase sequence)
        {

        }
    }
}