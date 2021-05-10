using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace UnityEditor.TimelineEditor
{
    public class Track : PlayableNode<Clip>
    {
        private const float BOX_WIDTH = 30f;
        public virtual float defaultHeight { get { return 30f; } set { } }
#if UNITY_EDITOR
        public virtual string iconName { get; set; }

        private Texture2D _icon;
        public Texture2D icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = (Texture2D)Resources.Load(iconName);
                    if (_icon == null)
                    {
                        _icon = ReflectionTools.LoadIcon(iconName);
                    }
                }
                return _icon;
            }
        }
#endif
        public float _height;
        public float finalHeight
        {
            get { return _height + defaultHeight; }
        }
        public bool isLocked;
        public Group parent;

        public List<Clip> clips
        {
            get
            {
                return _subPlayables;
            }
        }

        public void Sort()
        {
            _subPlayables = _subPlayables.OrderBy(a => a.startTime).ToList();
        }

#if UNITY_EDITOR
        public void OnTrackInfoGUI(Rect trackRect)
        {
            var e = Event.current;
            DoDefaultInfoGUI(e, trackRect);
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }
        protected void DoDefaultInfoGUI(Event e, Rect trackRect)
        {
            var iconBGRect = new Rect(0, 0, BOX_WIDTH, defaultHeight);
            var textInfoRect = Rect.MinMaxRect(iconBGRect.xMax + 2, 0, trackRect.width - BOX_WIDTH - 2, defaultHeight);
            var curveButtonRect = new Rect(trackRect.width - BOX_WIDTH, 0, BOX_WIDTH, defaultHeight);


            GUI.backgroundColor = UnityEditor.EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.7f) : new Color(0, 0, 0, 0.2f);
            GUI.Box(iconBGRect, string.Empty);
            GUI.backgroundColor = Color.white;

            if (icon != null)
            {
                var iconRect = new Rect(0, 0, 16, 16);
                iconRect.center = iconBGRect.center;
                GUI.color = ReferenceEquals(CutsceneUtility.selectedObject, this) ? Color.white : new Color(1, 1, 1, 0.8f);
                GUI.DrawTexture(iconRect, icon);
                GUI.color = Color.white;
            }


            var nameString = string.Format("<size=11>{0}</size>", name);
            var infoString = string.Format("<size=9><color=#707070>{0}</color></size>", "");
            GUI.color = isActive ? Color.white : Color.grey;
            GUI.Label(textInfoRect, string.Format("{0}\n{1}", nameString, infoString));
            GUI.color = Color.white;

            var wasEnable = GUI.enabled;
            GUI.enabled = true;
            var curveIconRect = new Rect(0, 0, 16, 16);
            curveIconRect.center = curveButtonRect.center - new Vector2(0, 1);

            GUI.color = UnityEditor.EditorGUIUtility.isProSkin ? Color.grey : Color.grey;
            if (!isActive)
            {
                var hiddenRect = new Rect(0, 0, 16, 16);
                hiddenRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
                if (GUI.Button(hiddenRect, Styles.hiddenIcon, GUIStyle.none)) { isActive = !isActive; }
            }

            if (isLocked)
            {
                var lockRect = new Rect(0, 0, 16, 16);
                lockRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
                if (!isActive) { lockRect.center -= new Vector2(16, 0); }
                if (GUI.Button(lockRect, Styles.lockIcon, GUIStyle.none)) { isLocked = !isLocked; }
            }


            GUI.color = Color.white;
            GUI.enabled = wasEnable;
        }

        public virtual void OnTrackTimelineGUI(Rect posRect, Rect timeRect, float cursorTime, System.Func<float, float> TimeToPos,
            System.Action action, System.Action<Track> onDragUpdated, System.Action<Track, float> onDragPerform)
        {
            var e = Event.current;

            var clipsPosRect = Rect.MinMaxRect(posRect.xMin, posRect.yMin, posRect.xMax, posRect.yMin + defaultHeight);
            DoTrackContextMenu(e, clipsPosRect, cursorTime, action, onDragUpdated, onDragPerform);
        }
        void DoTrackContextMenu(Event e, Rect clipsPosRect, float cursorTime, System.Action action, System.Action<Track> onDragUpdated, System.Action<Track, float> onDragPerform)
        {
            if (e.type == EventType.ContextClick && clipsPosRect.Contains(e.mousePosition))
            {
                var attachableTypeInfos = new List<EditorTools.TypeMetaInfo>();

                foreach (var info in EditorTools.GetTypeMetaDerivedFrom(typeof(Clip), Prefs.isOld))
                {
                    if (info.attachableTypes != null && !info.attachableTypes.Contains(this.GetType()))
                    {
                        continue;
                    }
                    if (info.type != typeof(Clip))
                    {
                        attachableTypeInfos.Add(info);
                    }
                }
                var menu = new UnityEditor.GenericMenu();
                if (attachableTypeInfos.Count > 0)
                {
                    foreach (var _info in attachableTypeInfos)
                    {
                        var info = _info;
                        var tName = info.name;
                        menu.AddItem(new GUIContent(tName), false, () =>
                        {
                            var clip = ReflectionTools.CreateInstance(_info.type) as Clip;
                            clip.startTime = cursorTime;
                            AddNode(clip);
                            CutsceneUtility.selectedObject = clip;
                            if (action!=null) action();
                        });
                    }
                    e.Use();
                }
                if (CutsceneUtility.CanPastClip(attachableTypeInfos))
                {
                    menu.AddItem(new GUIContent("粘贴"), false, () => 
                    {
                        CutsceneUtility.PasteClip(this, cursorTime);
                    });
                }
                if (menu.GetItemCount() > 0)
                {
                    menu.ShowAsContext();
                }
                else
                {
                    menu = null;
                }
            }
            if (clipsPosRect.Contains(e.mousePosition) && e.type == EventType.DragUpdated)
            {
                if (onDragUpdated != null) onDragUpdated(this);
            }

            if (clipsPosRect.Contains(e.mousePosition) && e.type == EventType.DragPerform)
            {
                if (onDragPerform != null) onDragPerform(this, cursorTime);
                //for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                //{
                //    var o = DragAndDrop.objectReferences[i];
                //    if (o is AnimationClip && this is SkillAnimationTrack)
                //    {
                //        var aniClip = new SKillAnimationEvent();
                //        AddNode(aniClip);
                //        aniClip.startTime = cursorTime;
                //        aniClip.length = (o as AnimationClip).length;
                //        aniClip.animationClip = o as AnimationClip;
                //        if (action != null) action();
                //        CutsceneUtility.selectedObject = aniClip;
                //    }
                //}
                SortClips();
            }
        }

        protected void SortClips()
        {
            Sort();
            if (clips.Count > 1)
            {
                for (int i = 0; i < clips.Count - 1; ++i)
                {
                    if (clips[i].InFrameRange(clips[i + 1].startFrame) || clips[i + 1].startTime <= clips[i].startTime)
                    {
                        clips[i + 1].startTime = clips[i].endTime;
                    }
                }
            }
        }

        public void DeleteClip(Clip clip)
        {
            if (clips.Contains(clip))
            {
                if (clip.sequence != null) clip.sequence.dirty = true;
                clips.Remove(clip);
            }
        }
#endif
        protected override void OnAddNode(Clip clip)
        {
            clip.parent = this;
            clips.Sort(delegate (Clip c1, Clip c2) { return c1.startFrame.CompareTo(c2.startFrame); });
            if (clip.sequence != null)
            {
                clip.sequence.dirty = true;
            }
        }
    }
}
