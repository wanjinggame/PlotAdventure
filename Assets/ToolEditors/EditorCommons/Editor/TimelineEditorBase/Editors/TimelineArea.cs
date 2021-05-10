using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace UnityEditor.TimelineEditor
{

    //public class TestTimelineAreaWindow : EditorWindow
    //{
    //    TestTimelineArea timelineArea;

    //    [MenuItem("Test/Test TimelineArea")]
    //    public static void Test()
    //    {
    //        GetWindow<TestTimelineAreaWindow>();
    //    }

    //    public void OnEnable()
    //    {
    //        var seq = new SequenceBase();
    //        var g = new Group();
    //        seq.AddNode(g);
    //        var t = new Track();
    //        g.AddNode(t);
    //        timelineArea = new TestTimelineArea(seq);
    //    }

    //    private void OnGUI()
    //    {
    //        GUILayoutTools.DrawTitle("Timeline", true);
    //        GUILayoutTools.Separator_SingleLine();
    //        GUILayoutTools.Separator_DoubleLine();
    //        timelineArea.OnGUI(this);
    //    }
    //}

    //public class TestTimelineArea : TimelineArea
    //{
    //    public TestTimelineArea(SequenceBase s)
    //    {
    //        cutscene = s;
    //    }
    //}

    /// <summary>
    /// 布局上还存在一些问题
    /// </summary>
    public class TimelineArea : ITimeline
    {
        protected SequenceBase cutscene;
        public float length
        {
            get { return 100; }
            set { }
        }
        [HideInInspector]
        private List<ClipWrapper> _multiSelection = null;
        public List<ClipWrapper> multiSelection
        {
            get { return _multiSelection; }
            set { _multiSelection = value; }
        }

        [HideInInspector]
        private Dictionary<int, ClipWrapper> clipWrappers;
        private Vector2? multiSelectStartPos = null;
        [System.NonSerialized] private Rect preMultiSelectionRetimeMinMax = default(Rect);
        [System.NonSerialized] private int multiSelectionScaleDirection = 0;
        [System.NonSerialized] private bool movingScrubCarret = false;
        [System.NonSerialized] private bool movingEndCarret = false;
        [HideInInspector]
        private List<TimelineWindowBase.GuideLine> _pendingGuides = new List<TimelineWindowBase.GuideLine>();
        public List<TimelineWindowBase.GuideLine> pendingGuides
        {
            get { return _pendingGuides; }
            set { _pendingGuides = value; }
        }

        public float viewTimeMin;

        public float viewTimeMax = 5;

        public float maxTime
        {
            get { return Mathf.Max(viewTimeMax, length); }
        }

        public float viewTime
        {
            get { return viewTimeMax - viewTimeMin; }
        }

        //Layout variables
        private float leftMargin
        { //margin on the left side. The width of the group/tracks list.
            get { return Prefs.trackListLeftMargin; }
            set { Prefs.trackListLeftMargin = Mathf.Clamp(value, 230, 400); }
        }
        private const float RIGHT_MARGIN = 16; //margin on the right side
        private float TOOLBAR_HEIGHT { get { return 18f; } }//the height of the toolbar
        private const float TOP_MARGIN = 20; //top margin AFTER the toolbar
        private const float GROUP_HEIGHT = 21; //height of group headers
        private const float TRACK_MARGINS = 4;  //margin between tracks of same group (top/bottom)
        private const float GROUP_RIGHT_MARGIN = 4;  //margin at the right side of groups
        private const float TRACK_RIGHT_MARGIN = 4;  //margin at the right side of tracks
        private const float FIRST_GROUP_TOP_MARGIN = 20; //initial top margin
                                                         //

        //Layout Rects
        private Rect topLeftRect;   //for playback controls
        private Rect topMiddleRect; //for time info
        private Rect leftRect;      //for group/track list
        public Rect centerRect;    //for timeline
                                   //private Rect topRightRect;
                                   //private Rect rightRect;
                                   //


        private static readonly Color listSelectionColor = new Color(0.5f, 0.5f, 1, 0.3f);
        private static readonly Color groupColor = new Color(0f, 0f, 0f, 0.2f);
        public bool anyClipDragging { get; set; }
        private Vector2 scrollPos;
        private float totalHeight;
        private int playTimeMin = 0;
        private float playTimeMax = 0;
        private bool willRepaint;
        private bool mouseButton2Down;
        public Vector2 mousePosition { get; set; }
        private Track pickedTrack;
        private Track copyTrack;
        private bool isResizingLeftMargin = false;
        private string searchString = null;

        private Color highlighColor
        {
            get { return isProSkin ? new Color(0.65f, 0.65f, 1) : new Color(0.1f, 0.1f, 0.1f); }
        }
        public float magnetSnapInterval
        {
            get { return viewTime * 0.01f; }
        }

        //SHORTCUTS//
        private static bool isProSkin
        {
            get { return EditorGUIUtility.isProSkin; }
        }

        private static Texture2D whiteTexture
        {
            get { return Styles.whiteTexture; }
        }

        private bool isPrefab
        {
            get { return false; }
        }

        private float screenWidth
        {
            get { return position.width; }
        }

        public float screenHeight
        {
            get { return position.height; }
        }

        private Color sampleColor
        {
            get { return Color.yellow; }
        }

        protected virtual Type CanAddTrackBaseType
        {
            get { return typeof(Track); }
            set { }
        }

        protected virtual string SHOWINFO { get; set; }

        public static TimelineArea current { get; set; }
        Rect position;

        EditorWindow window;

        [NonSerialized]
        bool OnOpen = true;
        public void OnGUI(EditorWindow window, int width = 0, int height = 0)
        {
            if (OnOpen)
            {
                Styles.Load();
                OnOpen = false;
            }
            this.window = window;
            if (!OnPreGUI()) return;
            if (EditorApplication.isCompiling)
            {
                window.ShowNotification(new GUIContent("Compiling\n...Please wait..."));
                return;
            }
            GUI.skin.label.richText = true;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            EditorStyles.label.richText = true;
            EditorStyles.textField.wordWrap = true;
            EditorStyles.foldout.richText = true;
            var e = Event.current;
            mousePosition = e.mousePosition;
            //avoid edit when compiling

            //handle undo/redo shortcuts
            if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
            {
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                e.Use();
                return;
            }

            //remove notifications quickly
            if (e.type == EventType.MouseDown)
            {
                window.RemoveNotification();
            }

            if (e.button == 2 && e.type == EventType.MouseDown) { mouseButton2Down = true; }
            if (e.button == 2 && e.rawType == EventType.MouseUp) { mouseButton2Down = false; }

            if (cutscene != null && !anyClipDragging && e.type == EventType.Layout)
            {
                foreach (var group in cutscene.groups)
                {
                    foreach (var track in group.tracks)
                    {
                        track.Sort();
                    }
                }
            }
            ShowToolbar();
            var r = GUILayoutUtility.GetLastRect();
            //position.height = window.position.height - position.y;
            position = new Rect(0, r.y + 18, window.position.width, window.position.height);
            position.height += 2;
            topLeftRect = new Rect(0, position.y, leftMargin, TOP_MARGIN);
            topMiddleRect = new Rect(0 + leftMargin, position.y, screenWidth - leftMargin - RIGHT_MARGIN, TOP_MARGIN);
            leftRect = new Rect(0, position.y + TOP_MARGIN, leftMargin, position.height + 18 + scrollPos.y);
            centerRect = new Rect(0 + leftMargin, position.y + TOP_MARGIN, screenWidth - leftMargin - RIGHT_MARGIN, position.height + 18 + scrollPos.y);

            if (cutscene == null)
            {
                window.ShowNotification(new GUIContent(SHOWINFO));
                return;
            }
            //...
            DoKeyboardShortcuts();
            //call respective function for each rect
            ShowPlaybackControls(topLeftRect);
            ShowTimeInfo(topMiddleRect);
            //Other functions
            DoScrubControls();
            DoZoomAndPan();

            if (viewTimeMin <= 0)
            {
                viewTimeMin = 0;
            }
            if (viewTimeMax - viewTimeMin <= 0.2f)
            {
                viewTimeMax = viewTimeMin + 0.2f;
            }

            //Dirty and Resample flags?
            if (e.rawType == EventType.MouseUp && e.button == 0)
            {
            }

            //Timelines
            var scrollRect1 = Rect.MinMaxRect(position.x, centerRect.yMin, screenWidth - position.x, centerRect.height);
            var scrollRect2 = Rect.MinMaxRect(position.x, centerRect.yMin, screenWidth - position.x, totalHeight + 150);
            using (var s = new GUI.ScrollViewScope(scrollRect1, scrollPos, scrollRect2))
            {
                scrollPos = s.scrollPosition;
                ShowGroupsAndTracksList(leftRect);
                ShowTimeLines(centerRect);
            }

            ///etc
            DrawGuides();
            AcceptDrops(leftRect);

            if (e.rawType == EventType.MouseUp)
            {
                foreach (var cw in clipWrappers.Values)
                {
                    cw.ResetInteraction();
                }
            }

            //clean selection and hotcontrols
            if (e.type == EventType.MouseDown && e.button == 0 && GUIUtility.hotControl == 0)
            {
                if (centerRect.Contains(mousePosition))
                {
                    CutsceneUtility.selectedObject = null;
                    multiSelection = null;
                }
                GUIUtility.keyboardControl = 0;
            }

            //cleanup
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.skin = null;
            //GUILayoutUtility.GetRect(window.position.width - 20f, window.position.height - position.y);
            if (willRepaint)
            {
                window.Repaint();
            }
        }

        //top left controls
        void ShowPlaybackControls(Rect topLeftRect)
        {
            var autoKeyRect = new Rect(topLeftRect.xMin + 10, topLeftRect.yMin + 4, 32, 32);
            AddCursorRect(autoKeyRect, MouseCursor.Link);
            GUI.backgroundColor = Prefs.autoKey ? (isProSkin ? Color.black.WithAlpha(0.3f) : Color.black.WithAlpha(0.4f)) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //GUI.Box(autoKeyRect, string.Empty, Styles.clipBoxStyle);
            GUI.color = Prefs.autoKey ? new Color(1, 0.4f, 0.4f) : Color.white;
            //if (GUI.Button(autoKeyRect, Styles.keyIcon, (GUIStyle)"box"))
            //{
            //    Prefs.autoKey = !Prefs.autoKey;
            //    ShowNotification(new GUIContent(string.Format("AutoKey {0}", Prefs.autoKey ? "Enabled" : "Disabled"), Styles.keyIcon));
            //}
            var autoKeyLabelRect = autoKeyRect;
            autoKeyLabelRect.yMin += 16;
            //GUI.Label(autoKeyLabelRect, "<color=#AAAAAA>Auto</color>", Styles.centerLabel);
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

            //Cutscene shows the gui
            GUILayout.BeginArea(topLeftRect);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.backgroundColor = isProSkin ? Color.white : Color.grey;

            Rect lastRect;

            //if (GUILayout.Button(!cutscene.isPlaying ? Styles.playIcon : Styles.pauseIcon, (GUIStyle)"box", GUILayout.Width(20), GUILayout.Height(20)))
            //{
            //    if (!cutscene.isPlaying)
            //    {
            //        Play();
            //    }
            //    else
            //    {
            //        Stop(false);
            //    }
            //    Event.current.Use();
            //}
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }
            //if (GUILayout.Button(Styles.stopIcon, (GUIStyle)"box", GUILayout.Width(20), GUILayout.Height(20)))
            //{
            //    Stop(true);
            //    Event.current.Use();
            //}
            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition)) { AddCursorRect(lastRect, MouseCursor.Link); }

            GUI.backgroundColor = Color.white;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void Stop(bool v)
        {
            if (cutscene == null) return;
            if (v)
            {
                cutscene.Stop();
            }
            else
            {
                cutscene.Pause();
            }
        }
        private double _timeLastUpdate = -1;
        private void Play()
        {
            _timeLastUpdate = EditorApplication.timeSinceStartup;
            cutscene.Play();
        }

        const int modulosLenght = 500;
        //top mid - viewTime selection and time info
        void ShowTimeInfo(Rect topMiddleRect)
        {
            GUI.color = Color.white.WithAlpha(0.2f);
            //GUI.Box(topMiddleRect, string.Empty, EditorStyles.toolbarButton);
            GUI.color = Color.black.WithAlpha(0.2f);
            //GUI.Box(topMiddleRect, string.Empty, Styles.timeBoxStyle);
            GUI.color = Color.white;
            var timeInterval = 1000000f;
            var highMod = timeInterval;
            var lowMod = 0.01f;
            var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;
            var timeStep = doFrames ? (1f / Prefs.frameRate) : lowMod;
            var modulos = new float[modulosLenght];
            for (int i = 0; i < modulos.Length; ++i)
            {
                modulos[i] = (i + 1) * timeStep;
            }
            for (var i = 0; i < modulos.Length; i++)
            {
                var count = viewTime / modulos[i];
                if (centerRect.width / count > 50)
                { //50 is approx width of label
                    timeInterval = modulos[i];
                    lowMod = i > 0 ? modulos[i - 1] : lowMod;
                    highMod = i < modulos.Length - 1 ? modulos[i + 1] : highMod;
                    break;
                }
            }

            var start = viewTimeMin;/* (float)Mathf.FloorToInt(viewTimeMin / timeInterval) * timeInterval;*/
            var end = viewTimeMax/*(float)Mathf.CeilToInt(viewTimeMax / timeInterval) * timeInterval*/;
            //start =  /*Mathf.Round(start * 10) / 10;*/
            //end = Mathf.Round(end * 10) / 10;

            //draw vertical guide lines. Do this outside the BeginArea bellow.
            for (var _i = start; _i <= end; _i += timeInterval)
            {
                var i = _i; /*Mathf.Round(_i * 10) / 10;*/
                var linePos = TimeToPos(i);
                DrawGuideLine(linePos, Color.black.WithAlpha(0.4f));
                if (i % highMod == 0)
                {
                    DrawGuideLine(linePos, Color.black.WithAlpha(0.5f));
                }
            }

            using (new GUI.GroupScope(topMiddleRect))
            {
                //the minMax slider
                //var _timeMin = viewTimeMin;
                //var _timeMax = viewTimeMax;
                //var sliderRect = new Rect(5, 0, topMiddleRect.width - 10, 18);
                //EditorGUI.MinMaxSlider(sliderRect, ref _timeMin, ref _timeMax, 0, maxTime);
                //viewTimeMin = _timeMin;
                //viewTimeMax = _timeMax;
                //if (sliderRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
                //{
                //    viewTimeMin = 0;
                //    viewTimeMax = length;
                //}

                GUI.color = Color.white.WithAlpha(0.1f);
                GUI.DrawTexture(Rect.MinMaxRect(0, TOP_MARGIN - 1, topMiddleRect.xMax, TOP_MARGIN), Styles.whiteTexture);
                GUI.color = Color.white;

                //the step interval
                if (centerRect.width / (viewTime / timeStep) > 6)
                {
                    for (var i = start; i <= end; i += timeStep)
                    {
                        var posX = TimeToPos(i);
                        var frameRect = Rect.MinMaxRect(posX - 1, TOP_MARGIN - 2, posX + 1, TOP_MARGIN - 1);
                        GUI.color = isProSkin ? Color.white : Color.black;
                        GUI.DrawTexture(frameRect, whiteTexture);
                        GUI.color = Color.white;
                    }
                }

                //the time interval
                for (var i = start; i <= end; i += timeInterval)
                {
                    var posX = TimeToPos(i);

                    var rounded = /*Mathf.Round(i * 10) / 10*/ i;

                    GUI.color = isProSkin ? Color.white : Color.black;
                    var markRect = Rect.MinMaxRect(posX - 2, TOP_MARGIN - 3, posX + 2, TOP_MARGIN - 1);
                    GUI.DrawTexture(markRect, whiteTexture);
                    GUI.color = Color.white;

                    var text = doFrames ? ((int)(rounded * Prefs.frameRate + 0.5f)).ToString() : rounded.ToString("0.00");
                    var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(text));
                    var stampRect = new Rect(0, 0, size.x, size.y);
                    stampRect.center = new Vector2(posX, TOP_MARGIN - size.y + 4);
                    //GUI.color = rounded % highMod == 0 ? Color.white : Color.white.WithAlpha(0.5f);
                    GUI.Box(stampRect, text, (GUIStyle)"label");
                    GUI.color = Color.white;
                }

                //the number showing current time when scubing
                if (cutscene.currentTime > 0)
                {
                    var label = doFrames ? ((int)(cutscene.currentTime * Prefs.frameRate + 0.5f)).ToString("0") : cutscene.currentTime.ToString("0.00");
                    var text = "<b><size=17>" + label + "</size></b>";
                    var size = Styles.headerBoxStyle.CalcSize(new GUIContent(text));
                    var posX = TimeToPos(cutscene.currentTime);
                    var stampRect = new Rect(0, 0, size.x, size.y);
                    stampRect.center = new Vector2(posX, TOP_MARGIN - size.y / 2);

                    GUI.backgroundColor = isProSkin ? Color.black.WithAlpha(0.4f) : Color.black.WithAlpha(0.7f);
                    GUI.color = sampleColor;
                    GUI.Box(stampRect, text, Styles.headerBoxStyle);
                }

                //the length position carret texture and pre-exit length indication
                var lengthPos = TimeToPos(length);
                var lengthRect = new Rect(0, 0, 16, 16);
                lengthRect.center = new Vector2(lengthPos, TOP_MARGIN - 2);
                GUI.color = isProSkin ? Color.white : Color.black;
                GUI.DrawTexture(lengthRect, Styles.carretIcon);
                GUI.color = Color.white;
            }
        }

        //...
        void DoKeyboardShortcuts()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && GUIUtility.keyboardControl == 0)
            {

                if (e.keyCode == KeyCode.S)
                {
                }

                if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                {
                    DeleteClips();
                }

                if (e.keyCode == KeyCode.Space && !e.shift)
                {
                }

                if (e.keyCode == KeyCode.Comma)
                {
                }

                if (e.keyCode == KeyCode.Period)
                {
                }
            }
        }

        //...
        void DrawGuides()
        {

            //draw a vertical line at 0 time
            DrawGuideLine(TimeToPos(0), isProSkin ? Color.white : Color.black);

            //draw a vertical line at length time
            DrawGuideLine(TimeToPos(length), isProSkin ? Color.white : Color.black);

            //draw a vertical line at current time
            if (cutscene.currentTime > 0)
            {
                DrawGuideLine(TimeToPos(cutscene.currentTime), sampleColor);
            }

            //draw a vertical line at dragging clip start/end time
            if (CutsceneUtility.selectedObject != null && anyClipDragging)
            {
                DrawGuideLine(TimeToPos(CutsceneUtility.selectedObject.startTime), Color.white.WithAlpha(0.05f));
                DrawGuideLine(TimeToPos(CutsceneUtility.selectedObject.endTime), Color.white.WithAlpha(0.05f));
            }

            if (cutscene.isPlaying)
            {
                if (playTimeMin > 0)
                {
                    DrawGuideLine(TimeToPos(playTimeMin), Color.red);
                }
                if (playTimeMax < length)
                {
                    DrawGuideLine(TimeToPos(playTimeMax), Color.red);
                }
            }

            //draw other subscribed TimelineWindowBase.GuideLines
            for (var i = 0; i < pendingGuides.Count; i++) { DrawGuideLine(TimeToPos(pendingGuides[i].time), pendingGuides[i].color); }
            pendingGuides.Clear();
        }

        //...
        protected virtual void AcceptDrops(Rect rect)
        {

            //if (cutscene.currentTime > 0)
            //{
            //    return;
            //}

            //var e = Event.current;
            //if (e.type == EventType.DragUpdated)
            //{
            //    //DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            //}

            //if (e.type == EventType.DragPerform)
            //{
            //    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            //    {
            //        var o = DragAndDrop.objectReferences[i];
            //        if (o is GameObject)
            //        {

            //        }
            //        if(o is AnimationClip)
            //        {
            //        }
            //    }
            //}
        }

        //Scrubing....
        void DoScrubControls()
        {
            if (EditorApplication.isPlaying)
            { //no scrubbing if playing in runtime
                return;
            }

            var e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                if (topMiddleRect.Contains(mousePosition))
                {
                    var carretPos = TimeToPos(length) + leftRect.width;
                    var isEndCarret = Mathf.Abs(mousePosition.x - carretPos) < 10 || e.control;

                    if (e.button == 0)
                    {
                        movingEndCarret = isEndCarret;
                        movingScrubCarret = !movingEndCarret;
                        Stop(false);
                    }
                    e.Use();
                }
                else
                {
                    Stop(true);
                }
            }


            if (e.button == 0 && e.rawType == EventType.MouseUp)
            {
                movingScrubCarret = false;
                movingEndCarret = false;
            }

            var pointerTime = PosToTime(mousePosition.x);
            if (movingScrubCarret)
            {
                cutscene.SetCurrentTime(SnapTime(pointerTime));
                cutscene.SetCurrentTime(Mathf.Clamp(cutscene.currentTime, Mathf.Max(viewTimeMin, 0) + float.Epsilon, viewTimeMax - float.Epsilon));
            }

            if (movingEndCarret)
            {
                length = SnapTime(pointerTime);
                length = Mathf.Clamp(length, viewTimeMin + float.Epsilon, viewTimeMax - float.Epsilon);
            }
        }

        //...
        void DoZoomAndPan()
        {
            if (!centerRect.Contains(mousePosition))
            {
                return;
            }

            var e = Event.current;
            //Zoom or scroll down/up if prefs is set to scrollwheel
            if ((e.type == EventType.ScrollWheel && Prefs.scrollWheelZooms) || (e.alt && !e.shift && e.button == 1))
            {
                this.AddCursorRect(centerRect, MouseCursor.Zoom);
                if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.ScrollWheel)
                {
                    var delta = e.alt ? -e.delta.x * 0.1f : e.delta.y;
                    {
                        var pointerTimeA = PosToTime(mousePosition.x);
                        var t = (Mathf.Abs(delta * 20) / centerRect.width) * viewTime;
                        var vMin = viewTimeMin;
                        vMin += (delta > 0 ? -t : t);
                        var vMax = viewTimeMax;
                        if (!(viewTimeMin <= 0 && e.type == EventType.MouseDrag))
                        {
                            vMax += delta > 0 ? t : -t;
                        }
                        var pointerTimeB = PosToTime(mousePosition.x + e.delta.x);
                        var diff = pointerTimeA - pointerTimeB;
                        vMin += diff;
                        if (!(viewTimeMin <= 0 && e.type == EventType.MouseDrag))
                        {
                            vMax += diff;
                        }
                        viewTimeMin = viewTimeMin == 0 ? 0 : vMin;
                        if (vMax - viewTimeMin >= 0.2f)
                        {
                            viewTimeMax = vMax;
                        }
                        else
                        {
                            viewTimeMax = viewTimeMin + 0.2f;
                        }
                        e.Use();
                    }
                }
            }

            //pan left/right, up/down
            if (mouseButton2Down || (e.alt && !e.shift && e.button == 0))
            {
                this.AddCursorRect(centerRect, MouseCursor.Pan);
                if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp)
                {
                    var t = (Mathf.Abs(e.delta.x) / centerRect.width) * viewTime;
                    viewTimeMin += e.delta.x > 0 ? -t : t;
                    if (!(viewTimeMin <= 0 && e.type == EventType.MouseDrag))
                    {
                        viewTimeMax += e.delta.x > 0 ? -t : t;
                    }
                    scrollPos.y -= e.delta.y;
                    e.Use();
                }
            }
        }

        //UTILITY FUNCS//
        public float TimeToPos(float time)
        {
            return (time - viewTimeMin) / viewTime * centerRect.width;
        }

        public float PosToTime(float pos)
        {
            return (pos - leftMargin) / centerRect.width * viewTime + viewTimeMin;
        }

        public float SnapTime(float time)
        {
            return (Mathf.Round(time / Prefs.snapInterval) * Prefs.snapInterval);
        }

        void DrawGuideLine(float xPos, Color color)
        {
            if (xPos == 0) xPos = 1;
            if (xPos == centerRect.xMax - leftRect.width) xPos = (centerRect.xMax - leftRect.width) - 1;
            if (xPos > 0 && xPos < centerRect.xMax - leftRect.width)
            {
                var guideRect = new Rect(xPos + centerRect.x - 1, centerRect.y, 2f, centerRect.height);
                GUI.color = color;
                GUI.DrawTexture(guideRect, whiteTexture);
                GUI.color = Color.white;
            }
        }

        public void AddCursorRect(Rect rect, MouseCursor type)
        {
            EditorGUIUtility.AddCursorRect(rect, type);
            willRepaint = true;
        }
        //...
        void ShowGroupsAndTracksList(Rect leftRect)
        {
            var e = Event.current;
            //allow resize list width
            var scaleRect = new Rect(leftRect.xMax - 4, leftRect.yMin, 4, leftRect.height);
            AddCursorRect(scaleRect, MouseCursor.ResizeHorizontal);
            if (e.type == EventType.MouseDown && e.button == 0 && scaleRect.Contains(e.mousePosition)) { isResizingLeftMargin = true; e.Use(); }
            if (isResizingLeftMargin) { leftMargin = e.mousePosition.x + 2; }
            if (e.rawType == EventType.MouseUp) { isResizingLeftMargin = false; }

            GUI.enabled = cutscene.currentTime <= 0;

            //starting height && search.
            var nextYPos = FIRST_GROUP_TOP_MARGIN;
            var wasEnabled = GUI.enabled;
            GUI.enabled = true;
            var collapseAllRect = Rect.MinMaxRect(leftRect.x + 5, leftRect.y + 4, 20, leftRect.y + 20 - 1);
            var searchRect = Rect.MinMaxRect(leftRect.x + 20, leftRect.y + 4, leftRect.xMax - 18, leftRect.y + 20 - 1);
            var searchCancelRect = Rect.MinMaxRect(searchRect.xMax, searchRect.y, leftRect.xMax - 4, searchRect.yMax);
            var anyExpanded = cutscene.groups.Any(g => !g.isCollapsed);
            AddCursorRect(collapseAllRect, MouseCursor.Link);
            GUI.color = Color.white.WithAlpha(0.5f);
            if (GUI.Button(collapseAllRect, anyExpanded ? "▼" : "►", (GUIStyle)"label"))
            {
                foreach (var group in cutscene.groups)
                {
                    group.isCollapsed = anyExpanded;
                }
            }
            GUI.color = Color.white;
            searchString = EditorGUI.TextField(searchRect, searchString, (GUIStyle)"ToolbarSeachTextField");
            if (GUI.Button(searchCancelRect, string.Empty, (GUIStyle)"ToolbarSeachCancelButton"))
            {
                searchString = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            GUI.enabled = wasEnabled;


            //begin area for left Rect
            GUI.BeginGroup(leftRect);
            ShowListGroups(e, ref nextYPos);
            GUI.EndGroup();

            totalHeight = nextYPos;


            //Simple button to add empty group for convenience
            //var addButtonY = totalHeight + TOP_MARGIN + TOOLBAR_HEIGHT + 20;
            //var addRect = Rect.MinMaxRect(leftRect.xMin + 10, addButtonY, leftRect.xMax - 10, addButtonY + 20);
            GUI.color = Color.white.WithAlpha(0.5f);
            //if (GUI.Button(addRect, "Add Actor Group"))
            //{
            //    var group = new Group();
            //    group.AddNode(new AnimatorTrack() { name = "AnimatorTrack" });
            //    cutscene.groups.Add(group);
            //    // CutsceneUtility.selectedObject = newGroup;
            //}

            //clear picks
            if (e.rawType == EventType.MouseUp)
            {
                pickedGroup = null;
                pickedTrack = null;
            }

            GUI.enabled = true;
            GUI.color = Color.white;
        }

        void ShowListTracks(Event e, Group group, ref float nextYPos)
        {
            //TRACKS
            for (int t = 0; t < group.tracks.Count; t++)
            {
                var track = group.tracks[t];
                var yPos = nextYPos;

                var trackRect = new Rect(10, yPos, leftRect.width - TRACK_RIGHT_MARGIN - 10, track.finalHeight);
                nextYPos += track.finalHeight + TRACK_MARGINS;

                //GRAPHICS
                GUI.color = Color.white.WithAlpha(0.2f);
                GUI.Box(trackRect, string.Empty, (GUIStyle)"flow node 0");
                GUI.color = track.isActive || !isProSkin ? Color.white : Color.grey;
                GUI.Box(trackRect, string.Empty);
                if (ReferenceEquals(track, CutsceneUtility.selectedObject) || track == pickedTrack)
                {
                    GUI.color = listSelectionColor;
                    GUI.DrawTexture(trackRect, whiteTexture);
                }

                //custom color indicator
                if (track.isActive && track.color != Color.white && track.color.a > 0.2f)
                {
                    GUI.color = track.color;
                    var colorRect = new Rect(trackRect.xMax + 1, trackRect.yMin, 2, track.finalHeight);
                    GUI.DrawTexture(colorRect, whiteTexture);
                }
                GUI.color = Color.white;
                //

                ///
                using (new GUI.GroupScope(trackRect))
                {
                    track.OnTrackInfoGUI(trackRect);
                }
                //GUI.EndGroup();
                ///

                AddCursorRect(trackRect, pickedTrack == null ? MouseCursor.Link : MouseCursor.MoveArrow);

                //CONTEXT
                if (e.type == EventType.ContextClick && trackRect.Contains(e.mousePosition))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Disable Track"), !track.isActive, () => { track.isActive = !track.isActive; });
                    //menu.AddItem(new GUIContent("Lock Track"), track.isLocked, () => { track.isLocked = !track.isLocked; });
                    menu.AddItem(new GUIContent("Copy"), false, () => { copyTrack = track; });
                    //if (track.GetType().RTGetAttribute<UniqueElementAttribute>(true) == null)
                    //{
                    //    menu.AddItem(new GUIContent("Duplicate"), false, () =>
                    //    {
                    //        group.DuplicateTrack(track);
                    //        InitClipWrappers();
                    //    });
                    //}
                    //else
                    //{
                    //    menu.AddDisabledItem(new GUIContent("Duplicate"));
                    //}
                    menu.AddSeparator("/");
                    menu.AddItem(new GUIContent("Delete Track"), false, () =>
                    {
                        if (EditorUtility.DisplayDialog("Delete Track", "Are you sure?", "YES", "NO!"))
                        {
                            group.DeleteTrack(track);
                            InitClipWrappers();
                        }
                    });
                    menu.ShowAsContext();
                    e.Use();
                }

                //REORDERING
                if (e.type == EventType.MouseDown && e.button == 0 && trackRect.Contains(e.mousePosition))
                {
                    CutsceneUtility.selectedObject = !ReferenceEquals(CutsceneUtility.selectedObject, track) ? track : null;
                    pickedTrack = track;
                    e.Use();
                }

                if (pickedTrack != null && pickedTrack != track && ReferenceEquals(pickedTrack.parent, group))
                {
                    if (trackRect.Contains(e.mousePosition))
                    {
                        var markRect = new Rect(trackRect.x, (group.tracks.IndexOf(pickedTrack) < t) ? trackRect.yMax - 2 : trackRect.y, trackRect.width, 2);
                        GUI.color = Color.grey;
                        GUI.DrawTexture(markRect, Styles.whiteTexture);
                        GUI.color = Color.white;
                    }

                    if (e.rawType == EventType.MouseUp && e.button == 0 && trackRect.Contains(e.mousePosition))
                    {
                        group.tracks.Remove(pickedTrack);
                        group.tracks.Insert(t, pickedTrack);
                        cutscene.Validate();
                        pickedTrack = null;
                        e.Use();
                    }
                }
            }
        }

        Group pickedGroup;

        void ShowListGroups(Event e, ref float nextYPos)
        {
            //GROUPS
            for (int g = 0; g < cutscene.groups.Count; g++)
            {
                var group = cutscene.groups[g];


                var groupRect = new Rect(4, nextYPos, leftRect.width - GROUP_RIGHT_MARGIN - 4, GROUP_HEIGHT - 3);
                this.AddCursorRect(groupRect, pickedGroup == null ? MouseCursor.Link : MouseCursor.MoveArrow);
                nextYPos += GROUP_HEIGHT;

                ///highligh?
                var groupSelected = (ReferenceEquals(group, CutsceneUtility.selectedObject) || group == pickedGroup);
                GUI.color = groupSelected ? listSelectionColor : groupColor;
                GUI.Box(groupRect, string.Empty, Styles.headerBoxStyle);
                GUI.color = Color.white;


                //GROUP CONTROLS
                var plusClicked = false;
                GUI.color = isProSkin ? Color.white.WithAlpha(0.5f) : new Color(0.2f, 0.2f, 0.2f);
                var plusRect = new Rect(groupRect.xMax - 14, groupRect.y + 5, 8, 8);
                if (GUI.Button(plusRect, Styles.plusIcon, GUIStyle.none)) { plusClicked = true; }
                if (!group.isActive)
                {
                    var disableIconRect = new Rect(plusRect.xMin - 20, groupRect.y + 1, 16, 16);
                    if (GUI.Button(disableIconRect, Styles.hiddenIcon, GUIStyle.none)) { group.isActive = true; }
                }

                GUI.color = isProSkin ? Color.yellow : Color.white;
                GUI.color = group.isActive ? GUI.color : Color.grey;
                var foldRect = new Rect(groupRect.x + 2, groupRect.y + 1, 20, groupRect.height);
                group.isCollapsed = !EditorGUI.Foldout(foldRect, !group.isCollapsed, string.Format("<b>{0} {1}</b>", group.name, string.Empty));
                GUI.color = Color.white;
                //Actor Object Field
                if (group.GetActor() == null)
                {
                    var oRect = Rect.MinMaxRect(groupRect.xMin + 20, groupRect.yMin + 1, groupRect.xMax - 20, groupRect.yMax - 1);
                    var go = (GameObject)UnityEditor.EditorGUI.ObjectField(oRect, group.GetActor(), typeof(GameObject), true);
                    group.SetActor(go);
                }
                ///---

                ///CONTEXT
                if ((e.type == EventType.ContextClick && groupRect.Contains(e.mousePosition)) || plusClicked)
                {
                    var menu = new GenericMenu();
                    foreach (var _info in UnityEditor.TimelineEditor.EditorTools.GetTypeMetaDerivedFrom(CanAddTrackBaseType, false))
                    {
                        var info = _info;
                        if (info.type != typeof(Track))
                        {
                            var finalPath = info.name;

                            menu.AddItem(new GUIContent("Add Track/" + finalPath), false, () =>
                            {
                                var track = ReflectionTools.CreateInstance(info.type) as Track;
                                if (track != null)
                                {
                                    track.name = finalPath;
                                    group.AddNode(track);
                                    InitClipWrappers();
                                }
                            });
                        }
                    }
                    if (group.CanAddTrack(copyTrack))
                    {
                        menu.AddItem(new GUIContent("Paste Track"), false, () => { group.DuplicateTrack(copyTrack); InitClipWrappers(); });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste Track"));
                    }
                    menu.AddItem(new GUIContent("Disable Group"), !group.isActive, () => { group.isActive = !group.isActive; });
                    //menu.AddItem(new GUIContent("Lock Group"), group.isLocked, () => { group.isLocked = !group.isLocked; });

                    //if (!(group is MainPlayerGroup))
                    //{
                    //    menu.AddSeparator("/");
                    //    menu.AddItem(new GUIContent("Delete Group"), false, () =>
                    //    {
                    //        if (EditorUtility.DisplayDialog("Delete Group", "Are you sure?", "YES", "NO!"))
                    //        {
                    //            cutscene.DeleteGroup(group);
                    //            InitClipWrappers();
                    //        }
                    //    });
                    //}


                    menu.ShowAsContext();
                    e.Use();
                }
                if (e.type == EventType.MouseDown && e.button == 0 && groupRect.Contains(e.mousePosition))
                {
                    CutsceneUtility.selectedObject = !ReferenceEquals(CutsceneUtility.selectedObject, group) ? group : null;
                    //if (!(group is MainPlayerGroup))
                    //{
                    //    pickedGroup = group;
                    //}
                    if (e.clickCount == 2)
                    {
                        Selection.activeGameObject = group.GetActor();
                    }
                    e.Use();
                }

                ///SHOW TRACKS (?)
                if (!group.isCollapsed)
                {
                    ShowListTracks(e, group, ref nextYPos);
                    //draw vertical graphic on left side of nested track rects
                    GUI.color = groupSelected ? listSelectionColor : groupColor;
                    var verticalRect = Rect.MinMaxRect(groupRect.x, groupRect.yMax, groupRect.x + 3, nextYPos - 2);
                    GUI.DrawTexture(verticalRect, Styles.whiteTexture);
                    GUI.color = Color.white;
                }
            }
        }

        private DateTime lastTime;

        private void Update()
        {
            //if (EditorApplication.isPlaying)
            //{
            //    if (Prefs.monitor)
            //    {
            //        var inst = LuaExecuter.GetSequence(Prefs.monitorTarget > 0 ? Prefs.monitorTarget : -1, Prefs.monitorNormalAtk);
            //        if (inst != null)
            //        {
            //            if (cutscene == null || cutscene.curSkillInfo == null || cutscene.curSkillInfo.skillId != inst.curSkillInfo.skillId)
            //            {
            //                Stop(true);
            //                SetSequence(inst);
            //                InitClipWrappers();
            //                lastTime = LuaExecuter.GetGameTiem();
            //                Play();
            //            }
            //            if (cutscene != null && cutscene.curSkillInfo != null && cutscene.curSkillInfo.skillId == inst.curSkillInfo.skillId)
            //            {
            //                if (!cutscene.isPlaying)
            //                {
            //                    lastTime = LuaExecuter.GetGameTiem();
            //                    Play();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            Stop(true);
            //        }

            //        if (cutscene == null) return;

            //        if (!cutscene.isPlaying) return;

            //        var time = cutscene.currentTime;

            //        var curtime = LuaExecuter.GetGameTiem();

            //        float delta = (float)(curtime - lastTime).TotalSeconds;

            //        lastTime = LuaExecuter.GetGameTiem();

            //        float newTime = time + delta;
            //        cutscene.SetCurrentTime(Mathf.Clamp(newTime, 0, length));
            //    }
            //}
            //else
            {
                if (cutscene == null) return;
                if (!cutscene.isPlaying) return;
                var time = cutscene.currentTime;
                var curtime = EditorApplication.timeSinceStartup;
                float delta = (float)(curtime - _timeLastUpdate);
                float newTime = time + delta;
                cutscene.SetCurrentTime(Mathf.Clamp(newTime, 0, length));
                _timeLastUpdate = curtime;
            }
            window.Repaint();
        }

        void ShowToolbar()
        {
            var e = Event.current;

            GUI.backgroundColor = isProSkin ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
            GUI.color = Color.white;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Space(3);

            GUI.enabled = cutscene != null;
            if (GUILayout.Button(cutscene != null && !cutscene.isPlaying ? Styles.playIcon : Styles.pauseIcon, EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                if (!cutscene.isPlaying)
                {
                    Play();
                }
                else
                {
                    Stop(false);
                }
                Event.current.Use();
            }
            GUILayout.Space(2);

            if (GUILayout.Button(Styles.stopIcon, EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                Stop(true);
                Event.current.Use();
            }
            GUI.enabled = true;

            GUILayout.Space(10);

            GUI.color = Color.white.WithAlpha(0.3f);
            GUILayout.Label(string.Format("<size=9>Version {0}</size>", 1.0f.ToString("0.0")));
            GUI.color = Color.white;

            if (GUILayout.Button(Styles.gearIcon, EditorStyles.toolbarButton, GUILayout.Width(26)))
            {
                PreferencesWindow.Show(new Rect(position.x + screenWidth - 5, position.y + 40, 400, 300), OnPreferencesDraw);
            }

            GUI.backgroundColor = new Color(1, 0.8f, 0.8f, 1);

            if (GUILayout.Button("Rest Space", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {

                //var vtime = viewTimeMax - viewTimeMin;
                //viewTimeMin = 0;
                //viewTimeMax = vtime;
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }

        void ShowTimeLines(Rect centerRect)
        {
            //temporary delegate used to call GUI after EndWindows (thus show on top)
            GUI.enabled = cutscene != null && cutscene.currentTime <= 0;
            System.Action postWindowsGUI = null;

            var e = Event.current;

            //bg graphic
            var bgRect = Rect.MinMaxRect(centerRect.xMin, TOP_MARGIN + scrollPos.y + position.y, centerRect.xMax, screenHeight - 4.5f + scrollPos.y);
            GUI.color = Color.black.WithAlpha(0.2f);
            GUI.DrawTextureWithTexCoords(bgRect, Styles.stripes, new Rect(0, 0, bgRect.width / -7, bgRect.height / -7));
            GUI.color = Color.white;
            GUI.Box(bgRect, string.Empty, (GUIStyle)"TextField");


            //Begin Group
            GUI.BeginGroup(centerRect);

            //starting height
            var nextYPos = FIRST_GROUP_TOP_MARGIN;

            //Begin Windows
            window.BeginWindows();

            //GROUPS
            for (int g = 0; g < cutscene.groups.Count; g++)
            {
                var group = cutscene.groups[g];

                if (FilteredOutBySearch(group, searchString))
                {
                    group.isCollapsed = true;
                    continue;
                }

                var groupRect = Rect.MinMaxRect(Mathf.Max(TimeToPos(viewTimeMin), TimeToPos(0)), nextYPos, TimeToPos(viewTimeMax), nextYPos + GROUP_HEIGHT);
                nextYPos += GROUP_HEIGHT;

                //if collapsed, just show a heat minimap of clips.
                if (group.isCollapsed)
                {
                    GUI.color = Color.black.WithAlpha(0.15f);
                    var collapseRect = Rect.MinMaxRect(groupRect.xMin + 2, groupRect.yMin + 2, groupRect.xMax, groupRect.yMax - 4);
                    GUI.DrawTexture(collapseRect, Styles.whiteTexture);
                    GUI.color = Color.white;

                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    foreach (var track in group.tracks)
                    {
                        foreach (var clip in track.clips)
                        {
                            var start = TimeToPos(clip.startTime);
                            var end = TimeToPos(clip.endTime);
                            GUI.DrawTexture(Rect.MinMaxRect(start + 0.5f, collapseRect.y + 2, end - 0.5f, collapseRect.yMax - 2), Styles.whiteTexture);
                        }
                    }
                    GUI.color = Color.white;
                    continue;
                }

                Debug.Log(Event.current.type);
                //TRACKS
                for (int t = 0; t < group.tracks.Count; t++)
                {
                    var track = group.tracks[t];
                    var yPos = nextYPos;
                    var trackPosRect = Rect.MinMaxRect(Mathf.Max(TimeToPos(viewTimeMin), TimeToPos(track.startTime)), yPos, TimeToPos(viewTimeMax), yPos + track.finalHeight);
                    var trackTimeRect = Rect.MinMaxRect(Mathf.Max(viewTimeMin, track.startTime), 0, viewTimeMax, 0);
                    nextYPos += track.finalHeight + TRACK_MARGINS;

                    //GRAPHICS
                    GUI.backgroundColor = isProSkin ? Color.black : Color.black.WithAlpha(0.1f);
                    GUI.Box(trackPosRect, string.Empty);
                    Handles.color = new Color(0.2f, 0.2f, 0.2f);
                    Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.y + 1), new Vector2(trackPosRect.xMax, trackPosRect.y + 1));
                    Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.yMax), new Vector2(trackPosRect.xMax, trackPosRect.yMax));
                    //if (track.showCurves)
                    //{
                    //    Handles.DrawLine(new Vector2(trackPosRect.x, trackPosRect.y + track.defaultHeight), new Vector2(trackPosRect.xMax, trackPosRect.y + track.defaultHeight));
                    //}
                    Handles.color = Color.white;
                    if (viewTimeMin < 0)
                    { //just visual clarity
                        GUI.Box(Rect.MinMaxRect(TimeToPos(viewTimeMin), trackPosRect.yMin, TimeToPos(0), trackPosRect.yMax), string.Empty);
                    }
                    if (track.startTime > track.parent.startTime || track.endTime < track.parent.endTime)
                    {
                        Handles.color = Color.white;
                        GUI.color = Color.black.WithAlpha(0.2f);
                        if (track.startTime > track.parent.startTime)
                        {
                            var tStart = TimeToPos(track.startTime);
                            var r = Rect.MinMaxRect(TimeToPos(0), yPos, tStart, yPos + track.finalHeight);
                            GUI.DrawTexture(r, whiteTexture);
                            GUI.DrawTextureWithTexCoords(r, Styles.stripes, new Rect(0, 0, r.width / 7, r.height / 7));
                            var a = new Vector2(tStart, trackPosRect.yMin);
                            var b = new Vector2(a.x, trackPosRect.yMax);
                            Handles.DrawLine(a, b);
                        }
                        if (track.endTime < track.parent.endTime)
                        {
                            var tEnd = TimeToPos(track.endTime);
                            var r = Rect.MinMaxRect(tEnd, yPos, TimeToPos(length), yPos + track.finalHeight);
                            GUI.DrawTexture(r, whiteTexture);
                            GUI.DrawTextureWithTexCoords(r, Styles.stripes, new Rect(0, 0, r.width / 7, r.height / 7));
                            var a = new Vector2(tEnd, trackPosRect.yMin);
                            var b = new Vector2(a.x, trackPosRect.yMax);
                            Handles.DrawLine(a, b);
                        }
                        GUI.color = Color.white;
                        Handles.color = Color.white;
                    }
                    GUI.backgroundColor = Color.white;

                    if (ReferenceEquals(CutsceneUtility.selectedObject, track))
                    {
                        GUI.color = Color.grey;
                        GUI.Box(trackPosRect, string.Empty, Styles.hollowFrameHorizontalStyle);
                        GUI.color = Color.white;
                    }
                    ///

                    if (track.isLocked)
                    {
                        if (e.isMouse && trackPosRect.Contains(e.mousePosition))
                        {
                            e.Use();
                        }
                    }

                    //...
                    var cursorTime = SnapTime(PosToTime(mousePosition.x));
                    track.OnTrackTimelineGUI(trackPosRect, trackTimeRect, cursorTime, TimeToPos, InitClipWrappers, DragUpdatedOnTrack, DragPerformOnTrack);
                    //...


                    //ACTION CLIPS
                    for (int a = 0; a < track.clips.Count; a++)
                    {
                        var action = track.clips[a];
                        var ID = UID(g, t, a);
                        ClipWrapper clipWrapper = null;

                        if (!clipWrappers.TryGetValue(ID, out clipWrapper) || clipWrapper.action != action)
                        {
                            InitClipWrappers();
                            clipWrapper = clipWrappers[ID];
                        }

                        //find and store next/previous clips to wrapper
                        var nextClip = a < track.clips.Count - 1 ? track.clips[a + 1] : null;
                        var previousClip = a != 0 ? track.clips[a - 1] : null;
                        clipWrapper.nextClip = nextClip;
                        clipWrapper.previousClip = previousClip;


                        //get the action box rect
                        var clipRect = clipWrapper.rect;

                        //modify it
                        clipRect.y = yPos;
                        clipRect.width = Mathf.Max(action.length / viewTime * centerRect.width, 6);
                        clipRect.height = track.defaultHeight;



                        //get the action time and pos
                        var xTime = action.startTime;
                        var xPos = clipRect.x;

                        if (anyClipDragging && ReferenceEquals(CutsceneUtility.selectedObject, action) && track.isActive)
                        {

                            var lastTime = xTime; //for multiSelection drag
                            xTime = PosToTime(xPos + leftRect.width);
                            xTime = SnapTime(xTime);
                            xTime = Mathf.Clamp(xTime, 0, maxTime - 0.1f);

                            //handle multisection. Limit xmin, xmax by their bound rect
                            if (multiSelection != null && multiSelection.Count > 1)
                            {
                                var delta = xTime - lastTime;
                                var boundMin = Mathf.Min(multiSelection.Select(b => b.action.startTime).ToArray());
                                // var boundMax = Mathf.Max( multiSelection.Select(b => b.action.endTime).ToArray() );
                                if (boundMin + delta < 0)
                                {
                                    xTime -= delta;
                                    delta = 0;
                                }

                                foreach (var cw in multiSelection)
                                {
                                    if (cw.action != action)
                                    {
                                        cw.action.startTime += delta;
                                    }
                                }
                            }

                            //clamp and cross blend between other nearby clips
                            if (multiSelection == null || multiSelection.Count < 1)
                            {
                                var preCursorClip = track.clips.LastOrDefault(act => act != action && act.startTime < cursorTime);
                                var postCursorClip = track.clips.FirstOrDefault(act => act != action && act.endTime > cursorTime);

                                if (e.shift)
                                { //when shifting track clips always clamp to previous clip and no need to clamp to next
                                    preCursorClip = previousClip;
                                    postCursorClip = null;
                                }

                                var preTime = preCursorClip != null ? preCursorClip.endTime : 0;
                                var postTime = postCursorClip != null ? postCursorClip.startTime : maxTime + action.length;
                                if (Prefs.magnetSnapping && !e.control)
                                { //magnet snap
                                    if (Mathf.Abs((xTime + action.length) - postTime) <= magnetSnapInterval)
                                    {
                                        xTime = postTime - action.length;
                                    }
                                    if (Mathf.Abs(xTime - preTime) <= magnetSnapInterval)
                                    {
                                        xTime = preTime;
                                    }
                                }

                                if (action.CanCrossBlend(preCursorClip))
                                {
                                    preTime -= Mathf.Min(action.length / 2, preCursorClip.length / 2);
                                }

                                if (action.CanCrossBlend(postCursorClip))
                                {
                                    postTime += Mathf.Min(action.length / 2, postCursorClip.length / 2);
                                }

                                //does it fit?
                                if (action.length > postTime - preTime)
                                {
                                    xTime = lastTime;
                                }

                                if (xTime != lastTime)
                                {
                                    xTime = Mathf.Clamp(xTime, preTime, postTime - action.length);
                                    //Shift all the next clips along with this one if shift is down
                                    if (e.shift)
                                    {
                                        foreach (var cw in clipWrappers.Values.Where(c => c.action.parent == action.parent && c.action != action && c.action.startTime > lastTime))
                                        {
                                            cw.action.startTime += xTime - lastTime;
                                        }
                                    }
                                }
                            }

                            //Apply xTime
                            action.startTime = xTime;
                        }

                        //apply xPos
                        clipRect.x = TimeToPos(xTime);


                        //set crossblendable blend properties
                        if (!anyClipDragging)
                        {
                            var overlap = previousClip != null ? Mathf.Max(previousClip.endTime - action.startTime, 0) : 0;
                            if (overlap > 0)
                            {
                                action.blendIn = overlap;
                                previousClip.blendOut = overlap;
                            }
                        }


                        //dont draw if outside of view range and not selected
                        var isSelected = ReferenceEquals(CutsceneUtility.selectedObject, action) || (multiSelection != null && multiSelection.Select(b => b.action).Contains(action));
                        var isVisible = Rect.MinMaxRect(0, scrollPos.y, centerRect.width, centerRect.height).Overlaps(clipRect);
                        if (!isSelected && !isVisible)
                        {
                            clipWrapper.rect = default(Rect); //we basicaly nullify the rect
                            continue;
                        }

                        //draw selected rect
                        if (isSelected)
                        {
                            var selRect = clipRect.ExpandBy(2);
                            GUI.color = highlighColor;
                            GUI.DrawTexture(selRect, Styles.whiteTexture);
                            GUI.color = Color.white;
                        }

                        //determine color and draw clip
                        var color = track.color;
                        color = new Color(1, 0.3f, 0.3f);
                        color = track.isActive ? action.color : Color.grey;
                        GUI.color = color;
                        GUI.Box(clipRect, string.Empty, Styles.clipBoxHorizontalStyle);
                        GUI.color = Color.white;
                        clipWrapper.e = Event.current;
                        clipWrapper.rect = GUI.Window(ID, clipRect, ActionClipWindow, string.Empty, GUIStyle.none);
                        if (!isProSkin) { GUI.color = Color.white.WithAlpha(0.5f); GUI.Box(clipRect, string.Empty); GUI.color = Color.white; }

                        //forward external Clip GUI
                        var nextPosX = TimeToPos(nextClip != null ? nextClip.startTime : viewTimeMax);
                        var prevPosX = TimeToPos(previousClip != null ? previousClip.endTime : viewTimeMin);
                        var extRectLeft = Rect.MinMaxRect(prevPosX, clipRect.yMin, clipRect.xMin, clipRect.yMax);
                        var extRectRight = Rect.MinMaxRect(clipRect.xMax, clipRect.yMin, nextPosX, clipRect.yMax);
                        action.ShowClipGUIExternal(extRectLeft, extRectRight);

                        var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;
                        //draw info text outside if clip is too small
                        if (clipRect.width <= 40)
                        {
                            GUI.Label(extRectRight, string.Format("<size=9>{0} start:{1}\n{2}</size>",
                                action.name, doFrames ? action.startFrame : action.startTime, action.externalInfo));
                        }
                    }

                    if (!track.isActive || track.isLocked)
                    {

                        postWindowsGUI += () =>
                        {
                            //overlay dark for disabled tracks
                            if (!track.isActive)
                            {
                                GUI.color = Color.black.WithAlpha(0.2f);
                                GUI.DrawTexture(trackPosRect, whiteTexture);
                                GUI.DrawTextureWithTexCoords(trackPosRect, Styles.stripes, new Rect(0, 0, (trackPosRect.width / 5), (trackPosRect.height / 5)));
                                GUI.color = Color.white;
                            }

                            //overlay stripes for locked tracks
                            if (track.isLocked)
                            {
                                GUI.color = Color.black.WithAlpha(0.15f);
                                GUI.DrawTextureWithTexCoords(trackPosRect, Styles.stripes, new Rect(0, 0, trackPosRect.width / 20, trackPosRect.height / 20));
                                GUI.color = Color.white;
                            }

                            if (isProSkin)
                            {
                                string overlayLabel = null;
                                if (!track.isActive && track.isLocked)
                                {
                                    overlayLabel = "DISABLED & LOCKED";
                                }
                                else
                                {
                                    if (!track.isActive) { overlayLabel = "DISABLED"; }
                                    if (track.isLocked) { overlayLabel = "LOCKED"; }
                                }
                                var size = Styles.centerLabel.CalcSize(new GUIContent(overlayLabel));
                                var bgLabelRect = new Rect(0, 0, size.x, size.y);
                                bgLabelRect.center = trackPosRect.center;
                                GUI.Label(trackPosRect, string.Format("<b>{0}</b>", overlayLabel), Styles.centerLabel);
                                GUI.color = Color.white;
                            }
                        };
                    }
                }


                //highligh selected group
                if (ReferenceEquals(CutsceneUtility.selectedObject, group))
                {
                    var r = Rect.MinMaxRect(groupRect.xMin, groupRect.yMin, groupRect.xMax, nextYPos);
                    GUI.color = Color.grey;
                    GUI.Box(r, string.Empty, Styles.hollowFrameHorizontalStyle);
                    GUI.color = Color.white;
                }

            }

            window.EndWindows();

            //call postwindow delegate
            if (postWindowsGUI != null)
            {
                postWindowsGUI();
                postWindowsGUI = null;
            }

            //this is done in the same GUI.Group
            DoMultiSelection();

            GUI.EndGroup();

            //border shadows
            GUI.color = Color.white.WithAlpha(0.2f);
            GUI.Box(bgRect, string.Empty, Styles.shadowBorderStyle);
            GUI.color = Color.white;

            ///darken the time after cutscene length
            if (viewTimeMax > length)
            {
                var endPos = Mathf.Max(TimeToPos(length) + leftRect.width, centerRect.xMin);
                var darkRect = Rect.MinMaxRect(endPos, centerRect.yMin, centerRect.xMax, centerRect.yMax);
                GUI.color = Color.black.WithAlpha(0.3f);
                GUI.Box(darkRect, string.Empty, (GUIStyle)"TextField");
                GUI.color = Color.white;
            }

            ///darken the time before zero
            if (viewTimeMin < 0)
            {
                var startPos = Mathf.Min(TimeToPos(0) + leftRect.width, centerRect.xMax);
                var darkRect = Rect.MinMaxRect(centerRect.xMin, centerRect.yMin, startPos, centerRect.yMax);
                GUI.color = Color.black.WithAlpha(0.3f);
                GUI.Box(darkRect, string.Empty, (GUIStyle)"TextField");
                GUI.color = Color.white;
            }

            if (GUIUtility.hotControl == 0 || e.rawType == EventType.MouseUp)
            {
                anyClipDragging = false;
            }
        }

        bool FilteredOutBySearch(PlayableNode playableNode, string search)
        {
            if (string.IsNullOrEmpty(search)) { return false; }
            if (string.IsNullOrEmpty(playableNode.name)) { return true; }
            return !playableNode.name.ToLower().Contains(search.ToLower());
        }

        int UID(int g, int t, int a)
        {
            var A = g.ToString("D3");
            var B = t.ToString("D3");
            var C = a.ToString("D4");
            return int.Parse(A + B + C);
        }

        void ActionClipWindow(int id)
        {
            ClipWrapper wrapper = null;
            if (clipWrappers.TryGetValue(id, out wrapper))
            {
                GUI.enabled = cutscene.currentTime <= 0;
                wrapper.OnClipGUI(false);
            }
        }

        public void InitClipWrappers()
        {
            if (cutscene == null)
            {
                return;
            }
            multiSelection = null;
            var lastTime = cutscene.currentTime;

            if (!Application.isPlaying)
            {
                Stop(true);
            }
            cutscene.Validate();
            clipWrappers = new Dictionary<int, ClipWrapper>();
            for (int g = 0; g < cutscene.groups.Count; g++)
            {
                for (int t = 0; t < cutscene.groups[g].tracks.Count; t++)
                {
                    for (int a = 0; a < cutscene.groups[g].tracks[t].clips.Count; a++)
                    {
                        var id = UID(g, t, a);
                        if (clipWrappers.ContainsKey(id))
                        {
                            Debug.LogError("Collided UIDs. This should really not happen but it did!");
                            continue;
                        }
                        clipWrappers[id] = new ClipWrapper(cutscene.groups[g].tracks[t].clips[a], this);
                    }
                }
            }

            if (lastTime > 0)
            {
                cutscene.currentTime = lastTime;
            }
        }

        void DoMultiSelection()
        {

            var e = Event.current;

            var r = new Rect();
            var bigEnough = false;
            if (multiSelectStartPos != null)
            {
                var start = (Vector2)multiSelectStartPos;
                if ((start - e.mousePosition).magnitude > 10)
                {
                    bigEnough = true;
                    r.xMin = Mathf.Max(Mathf.Min(start.x, e.mousePosition.x), 0);
                    r.xMax = Mathf.Min(Mathf.Max(start.x, e.mousePosition.x), screenWidth);
                    r.yMin = Mathf.Min(start.y, e.mousePosition.y);
                    r.yMax = Mathf.Max(start.y, e.mousePosition.y);
                    GUI.color = isProSkin ? Color.white : Color.white.WithAlpha(0.3f);
                    GUI.Box(r, string.Empty);
                    foreach (var wrapper in clipWrappers.Values.Where(b => r.Encapsulates(b.rect) && !b.action.isLocked))
                    {
                        GUI.color = new Color(0.5f, 0.5f, 1, 0.5f);
                        GUI.Box(wrapper.rect, string.Empty, Styles.clipBoxStyle);
                        GUI.color = Color.white;
                    }
                }
            }

            if (e.rawType == EventType.MouseUp)
            {
                if (bigEnough)
                {
                    multiSelection = clipWrappers.Values.Where(b => r.Encapsulates(b.rect) && !b.action.isLocked).ToList();
                    if (multiSelection.Count == 1)
                    {
                        CutsceneUtility.selectedObject = multiSelection[0].action;
                        multiSelection = null;
                    }
                }
                multiSelectStartPos = null;
            }

            if (multiSelection != null)
            {
                var boundRect = RectUtility.GetBoundRect(multiSelection.Select(b => b.rect).ToArray()).ExpandBy(4);
                GUI.color = isProSkin ? Color.white : Color.white.WithAlpha(0.3f);
                GUI.Box(boundRect, string.Empty);

                var leftDragRect = new Rect(boundRect.xMin - 6, boundRect.yMin, 4, boundRect.height);
                var rightDragRect = new Rect(boundRect.xMax + 2, boundRect.yMin, 4, boundRect.height);
                AddCursorRect(leftDragRect, MouseCursor.ResizeHorizontal);
                AddCursorRect(rightDragRect, MouseCursor.ResizeHorizontal);
                GUI.color = isProSkin ? new Color(0.7f, 0.7f, 0.7f) : Color.grey;
                GUI.DrawTexture(leftDragRect, Styles.whiteTexture);
                GUI.DrawTexture(rightDragRect, Styles.whiteTexture);
                GUI.color = Color.white;

                if (e.type == EventType.MouseDown && (leftDragRect.Contains(e.mousePosition) || rightDragRect.Contains(e.mousePosition)))
                {
                    multiSelectionScaleDirection = leftDragRect.Contains(e.mousePosition) ? -1 : 1;
                    var minTime = Mathf.Min(multiSelection.Select(b => b.action.startTime).ToArray());
                    var maxTime = Mathf.Max(multiSelection.Select(b => b.action.endTime).ToArray());
                    preMultiSelectionRetimeMinMax = Rect.MinMaxRect(minTime, 0, maxTime, 0);
                    e.Use();
                }

                if (e.type == EventType.MouseDrag && multiSelectionScaleDirection != 0)
                {
                    foreach (var clipWrapper in multiSelection)
                    {
                        var clip = clipWrapper.action;
                        var preClipStartTime = clipWrapper.preScaleStartTime;
                        var preClipEndTime = clipWrapper.preScaleEndTime;
                        var preTimeMin = preMultiSelectionRetimeMinMax.xMin;
                        var preTimeMax = preMultiSelectionRetimeMinMax.xMax;
                        var pointerTime = SnapTime(PosToTime(mousePosition.x));

                        var lerpMin = multiSelectionScaleDirection == -1 ? Mathf.Clamp(pointerTime, 0, preTimeMax) : preTimeMin;
                        var lerpMax = multiSelectionScaleDirection == 1 ? Mathf.Max(pointerTime, preTimeMin) : preTimeMax;

                        var normIn = Mathf.InverseLerp(preTimeMin, preTimeMax, preClipStartTime);
                        clip.startTime = Mathf.Lerp(lerpMin, lerpMax, normIn);

                        var normOut = Mathf.InverseLerp(preTimeMin, preTimeMax, preClipEndTime);
                        clip.endTime = Mathf.Lerp(lerpMin, lerpMax, normOut);

                    }
                    e.Use();
                }

                if (e.rawType == EventType.MouseUp)
                {
                    multiSelectionScaleDirection = 0;
                }
            }

            if (e.type == EventType.MouseDown && e.button == 0 && GUIUtility.hotControl == 0)
            {
                multiSelection = null;
                multiSelectStartPos = e.mousePosition;
            }

            GUI.color = Color.white;
        }

        public void SafeDoAction(System.Action call)
        {
            var time = cutscene.currentTime;
            Stop(true);
            call();
            cutscene.currentTime = time;
        }

        protected virtual void DragUpdatedOnTrack(Track track)
        {

        }

        protected virtual void DragPerformOnTrack(Track track, float cursorTime)
        {

        }

        private void DeleteClips()
        {
            if (multiSelection != null)
            {
                foreach (var act in multiSelection.Select(b => b.action).ToArray())
                {
                    (act.parent as Track).DeleteClip(act);
                }
                InitClipWrappers();
                multiSelection = null;
            }
            else
            {
                if (CutsceneUtility.selectedObject != null && CutsceneUtility.selectedObject is Clip)
                {
                    var action = CutsceneUtility.selectedObject as Clip;
                    (action.parent as Track).DeleteClip(action);
                    InitClipWrappers();
                }
            }
        }

        protected virtual void OnPreferencesDraw()
        {

        }

        public virtual void OnClipDoubleClick(Rect rect, Clip action)
        {

        }

        protected virtual bool OnPreGUI()
        {
            return true;
        }

        protected virtual void OnGUIEnd()
        {

        }
    }
}


