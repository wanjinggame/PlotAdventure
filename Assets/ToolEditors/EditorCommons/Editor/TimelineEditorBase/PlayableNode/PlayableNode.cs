using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TimelineEditor
{
    public class PlayableNode
    {
        public const int DEFAULT_FRAMES_PER_SECOND = 30;
        public const int DEFAULT_LENGTH = 10;
        public virtual string name { get; set; }
        public virtual float startTime { get; set; }
        public virtual float endTime { get; set; }
        [HideInInspector]
        public bool isActive = true;
        public virtual Color color
        {
            set { }
            get
            {
                return Color.white;
            }
        }

        protected int _currentFrame;
        protected int _frameLength = DEFAULT_LENGTH;
        protected int _frameRate { get { return Prefs.frameRate; } }
        //protected int _startFrame;
        //protected int _endFrame;

        public int startFrame
        {
            get
            {
                return (int)(startTime * Prefs.frameRate + 0.5f);
            }
        }

        public int endFrame
        {
            get
            {
                return (int)(endTime * Prefs.frameRate + 0.5f);
            }
        }

        public virtual bool InFrameRange(int frame)
        {
            return frame >= startFrame && frame <= endFrame;
        }


        #region Play

        protected bool _isPlaying = false;
        //protected bool _isSampling = false;
        protected bool _isPaused = false;
        //protected bool _isStopped = true;

        public virtual void Play()
        {
            _isPlaying = true;
            //_isStopped = false; 
            //_isPaused = false;
            //_isSampling = false;
            OnPlay();
        }

        public virtual void Sample(int frame)
        {
            //_isPlaying = false;
            //_isStopped = false;
            //_isPaused = false;
            //_isSampling = true;
            OnSample(frame);
        }

        public void Sample(float time)
        {
            this.Sample((int)(time * _frameRate));
        }

        public virtual void Pause()
        {
            _isPlaying = false;
            //_isStopped = false;
            _isPaused = true;
            //_isSampling = false;
            OnPause();
        }

        public virtual void Stop()
        {
            _isPlaying = false;
            //_isStopped = true;
            _isPaused = false;
            //_isSampling = false;
            OnStop();
        }

        protected virtual void OnPlay() { }
        protected virtual void OnSample(int frame) { }
        protected virtual void OnPause() { }
        protected virtual void OnStop() { }
        #endregion

    }

    public class PlayableNode<T> : PlayableNode where T : PlayableNode
    {
        protected List<T> _subPlayables = new List<T>();

        public void AddNode(T playable)
        {
            if (playable != null)
            {
                _subPlayables.Add(playable);
                OnAddNode(playable);
            }
        }

        public void RemoveNode(T playable)
        {
            if (_subPlayables.Contains(playable))
            {
                _subPlayables.Remove(playable);
            }
        }

        public sealed override void Play()
        {
            base.Play();
            foreach (var sub in _subPlayables)
            {
                sub.Play();
            }
        }

        public sealed override void Sample(int frame)
        {
            base.Sample(frame);
            foreach (var sub in _subPlayables)
            {
                sub.Sample(frame);
            }
        }

        public sealed override void Pause()
        {
            base.Pause();
            foreach (var sub in _subPlayables)
            {
                sub.Pause();
            }
        }

        public sealed override void Stop()
        {
            base.Stop();
            foreach (var sub in _subPlayables)
            {
                sub.Stop();
            }
        }
        protected virtual void OnAddNode(T t) { }
    }
}

