﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityEditor.TimelineEditor
{

    public static class Prefs {

		[System.Serializable]
		class SerializedData{
			public bool showTransforms                 = false;
			public bool compactMode                    = false;
			public TimeStepMode timeStepMode           = TimeStepMode.Seconds;
			public float snapInterval                  = 0.1f;
			public int frameRate                       = 30;
			public bool lockHorizontalCurveEditing     = true;
			public bool showDopesheetKeyValues         = true;
			public bool autoFirstKey                   = false;
			//public TangentMode defaultTangentMode      = TangentMode.Linear;
			public KeyframesStyle keyframesStyle       = KeyframesStyle.PerTangentMode; 
			public bool showShotThumbnails             = true;
			public int thumbnailsRefreshInterval       = 30;
			public bool showRuleOfThirds               = false;
			public bool scrollWheelZooms               = true;
			public bool showDescriptions               = true;
			public float gizmosLightness               = 0f;
			public Color motionPathsColor              = Color.black;
			public Prefs.RenderSettings renderSettings = new Prefs.RenderSettings();
			public bool autoKey                        = true;
			public bool magnetSnapping                 = true;
			public float trackListLeftMargin           = 280f;
            public bool monitor                        = true;
            public Color clipBasicColor                = new Color(0.24f, 0.36f, 0.60f, 1f);
            public bool showPopupWindow                = false;
            public bool old                            = true;
        }

		[System.Serializable]
		public enum KeyframesStyle{
			PerTangentMode,
			AlwaysDiamond
		}

		[System.Serializable]
		public enum TimeStepMode{
			Seconds,
			Frames
		}

		[System.Serializable]
		public class RenderSettings{
			
			public enum FileNameMode{
				UseCutsceneName,
				SpecifyFileName,
			}
		}

        //public static Sequence sequence;
        //public static TreeNode skillNodes;

        private static string dataName;
        public static void SetDataName(string name)
        {
            dataName = name;
        }

        private static SerializedData _data;
        private static SerializedData data
        {
            get
            {
                if (_data == null)
                {
                    _data = JsonUtility.FromJson<SerializedData>(EditorPrefs.GetString(dataName/* "Pangu.SkillEditorPreferences"*/));
                    if (_data == null)
                    {
                        _data = new SerializedData();
                    }
                }
                return _data;
            }
        }

        public static float[] snapIntervals = new float[]{ 0.001f, 0.01f, 0.1f };
		public static int[] frameRates = new int[]{ 15, 30, 60 };

		public static bool showTransforms{
			get {return data.showTransforms;}
			set {if (data.showTransforms != value){ data.showTransforms = value; Save(); } }
		}

		public static bool compactMode{
			get {return data.compactMode;}
			set {if (data.compactMode != value){ data.compactMode = value; Save(); } }
		}

		public static float gizmosLightness{
			get {return data.gizmosLightness;}
			set {if (data.gizmosLightness != value){ data.gizmosLightness = value; Save(); } }
		}

		public static Color gizmosColor{
			get {return new Color(data.gizmosLightness, data.gizmosLightness, data.gizmosLightness);}
		}

		public static bool showShotThumbnails{
			get {return data.showShotThumbnails;}
			set {if (data.showShotThumbnails != value){ data.showShotThumbnails = value; Save(); } }
		}

		public static bool showDopesheetKeyValues{
			get {return data.showDopesheetKeyValues;}
			set {if (data.showDopesheetKeyValues != value){ data.showDopesheetKeyValues = value; Save(); } }
		}

		public static KeyframesStyle keyframesStyle{
			get {return data.keyframesStyle;}
			set {if (data.keyframesStyle != value){ data.keyframesStyle = value; Save(); } }
		}

		public static bool scrollWheelZooms{
			get {return data.scrollWheelZooms;}
			set {if (data.scrollWheelZooms != value){ data.scrollWheelZooms = value; Save(); } }
		}

		public static bool showDescriptions{
			get {return data.showDescriptions;}
			set {if (data.showDescriptions != value){ data.showDescriptions = value; Save(); } }
		}

		public static Color motionPathsColor{
			get {return data.motionPathsColor;}
			set {if (data.motionPathsColor != value){ data.motionPathsColor = value; Save(); } }
		}

        public static Color clipBasicColor
        {
            get { return data.clipBasicColor; }
            set { if (data.clipBasicColor != value) { data.clipBasicColor = value; Save(); } }
        }

        public static int thumbnailsRefreshInterval{
			get {return data.thumbnailsRefreshInterval;}
			set {if (data.thumbnailsRefreshInterval != value){ data.thumbnailsRefreshInterval = value; Save(); } }
		}

		public static bool lockHorizontalCurveEditing{
			get {return data.lockHorizontalCurveEditing;}
			set {if (data.lockHorizontalCurveEditing != value){ data.lockHorizontalCurveEditing = value; Save(); } }
		}

		public static bool showRuleOfThirds{
			get {return data.showRuleOfThirds;}
			set {if (data.showRuleOfThirds != value){ data.showRuleOfThirds = value; Save(); } }			
		}

		public static Prefs.RenderSettings renderSettings{
			get {return data.renderSettings;}
			set {data.renderSettings = value; Save(); }			
		}

		public static bool autoKey{
			get {return data.autoKey;}
			set {if (data.autoKey != value){ data.autoKey = value; Save(); } }			
		}

		public static bool autoFirstKey{
			get {return data.autoFirstKey;}
			set {if (data.autoFirstKey != value){ data.autoFirstKey = value; Save(); } }			
		}

		public static bool magnetSnapping{
			get {return data.magnetSnapping;}
			set {if (data.magnetSnapping != value){ data.magnetSnapping = value; Save(); } }						
		}

		public static float trackListLeftMargin{
			get {return data.trackListLeftMargin;}
			set {if (data.trackListLeftMargin != value){ data.trackListLeftMargin = value; Save(); } }			
		}

        public static TimeStepMode timeStepMode
        {
            get { return data.timeStepMode; }
            set
            {
                if (data.timeStepMode != value)
                {
                    data.timeStepMode = value;
                    frameRate = value == TimeStepMode.Frames ? 30 : 10;
                    Save();
                }
            }
        }

        public static bool isOld
        {
            get { return data.old; }
            set { if (data.old != value) { data.old = value; Save(); } }
        }

        public static bool monitor
        {
            get { return data.monitor; }
            set { if (data.monitor != value) { data.monitor = value; Save(); } }
        }
        public static bool showPopupWindow
        {
            get { return data.showPopupWindow; }
            set { if (data.showPopupWindow != value) { data.showPopupWindow = value; Save(); } }
        }
        public static int monitorTarget = -1;
        public static bool monitorNormalAtk = false;

        public static int frameRate
        {
            get { return 15; }
            set { if (data.frameRate != value) { data.frameRate = value; snapInterval = 1f / value; Save(); } }
        }

        public static float snapInterval
        {
            get { return Mathf.Max(data.snapInterval, 0.001f); }
            set { if (data.snapInterval != value) { data.snapInterval = Mathf.Max(value, 0.001f); Save(); } }
        }

        public static bool USE_SKILLUNIT_MODE = true;

        static void Save()
        {
            EditorPrefs.SetString("Pangu.SkillEditorPreferences", JsonUtility.ToJson(data));
        }
    }
}
#endif