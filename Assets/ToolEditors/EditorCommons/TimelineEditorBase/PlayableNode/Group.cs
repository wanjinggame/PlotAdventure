using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TimelineEditor
{
    public class Group : PlayableNode<Track>
    {
        public SequenceBase parent { get; set; }

        public override string name
        {
            get
            {
                return actor == null ? base.name : actor.name + " Group";
            }
        }

        public List<Track> tracks
        {
            get
            {
                return _subPlayables;
            }
        }

        public void DuplicateTrack(Track track)
        {
            var type = track.GetType();
            var newTrack = System.Activator.CreateInstance(type) as Track;
            AddNode(newTrack);
            foreach (var clip in track.clips)
            {
                newTrack.AddNode(ReflectionTools.Clone(clip) as Clip);
            }
        }
        public bool isCollapsed;
        /// <summary>
        /// 对应到游戏里应该是Entity，先用GO
        /// </summary>
        private GameObject actor;

        public void SetActor(GameObject go)
        {
            actor = go;
        }
        public GameObject GetActor()
        {
            return actor;
        }

        public bool CanAddTrack(Track track)
        {
            return track != null;
        }

        public void DeleteTrack(Track track)
        {
            if (track != null && _subPlayables.Contains(track))
            {
                _subPlayables.Remove(track);
            }
        }

        protected override void OnAddNode(Track track)
        {
            track.parent = this;
        }
    }
}

