using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;

namespace Plot.Tools.PlotEditor
{
    public class MainWinodws : EditorWindow
    {
        DialogueWindow dialogueWindow;
        CharacterConfigWindow characterConfigWindow;
        DemoShowWinodw demoShowWinodw;

        public static MainWinodws Instance;

        [MenuItem("Polt/小说剧情编辑器")]
        public static void Open()
        {
            GetWindow<MainWinodws>();
        }

        public void OnEnable()
        {
            Instance = this;
            titleContent = new GUIContent("小说剧情编辑器");
            minSize = new Vector2(1172, 745);
            InitDialogueGorupData();
            InitCharacter();
            if (dialogueWindow)
            {
                dialogueWindow.Init(dialogueGroups);
            }
            if (characterConfigWindow)
            {
                characterConfigWindow.Init(characterInfos);
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        public void InitDialogueGorupData()
        {
            dialogueGroups = new List<DialogueGroup>();
            if (File.Exists(string.Format(Const.DATA_PATH, Const.DIALOGUE_GROUP_DATA)))
            {
                var datas = JsonMapper.ToObject(File.ReadAllText(string.Format(Const.DATA_PATH, Const.DIALOGUE_GROUP_DATA)));
                foreach (var key in datas.Keys)
                {
                    var id = int.Parse(key);
                    var ch = new DialogueGroup();
                    ch.FromJson(datas[key]);
                    dialogueGroups.Add(ch);
                }
            }
        }

        public void InitCharacter()
        {
            characterInfos = new List<CharacterInfo>();
            if (File.Exists(string.Format(Const.DATA_PATH, Const.CHARACTER_DATA)))
            {
                var datas = JsonMapper.ToObject(File.ReadAllText(string.Format(Const.DATA_PATH, Const.CHARACTER_DATA)));
                foreach (var key in datas.Keys)
                {
                    var id = int.Parse(key);
                    var ch = new CharacterInfo();
                    ch.FromJson(datas[key]);
                    characterInfos.Add(ch);
                }
            }
        }

        public List<DialogueGroup> dialogueGroups;
        public List<CharacterInfo> characterInfos;

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            using (new GUILayout.VerticalScope(GUILayout.Width(position.width * 0.45f)))
            {
                GUILayout.FlexibleSpace();
                GUI.color = new Color(0, 0, 0, 0.3f);
                GUILayout.BeginHorizontal(StyleTools.Skin.box);
                GUI.color = Color.white;
                GUILayout.Label(GUILayoutTools.GetTextSizeOf("小说剧情编辑器", 65), StyleTools.MiddleCenterLab);
                GUILayout.EndHorizontal();
                GUILayoutTools.Separator_DoubleLine();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("对白编辑器", GUILayout.Height(70), GUILayout.Width(position.width * 0.3f)))
                    {
                        if (dialogueWindow == null)
                        {
                            dialogueWindow = GetWindow<DialogueWindow>(typeof(MainWinodws));
                            dialogueWindow.Init(dialogueGroups);
                        }
                        dialogueWindow.Focus();
                    }
                    GUILayout.FlexibleSpace();
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("小说人物配置", GUILayout.Height(70), GUILayout.Width(position.width * 0.3f)))
                    {
                        if (characterConfigWindow == null)
                        {
                            characterConfigWindow = GetWindow<CharacterConfigWindow>(typeof(MainWinodws));
                            characterConfigWindow.Init(characterInfos);
                        }
                        characterConfigWindow.Focus();
                    }
                    GUILayout.FlexibleSpace();
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("对白演示", GUILayout.Height(70), GUILayout.Width(position.width * 0.3f)))
                    {
                        if(demoShowWinodw == null)
                        {
                            demoShowWinodw = GetWindow<DemoShowWinodw>(typeof(MainWinodws));
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("对白场景布置(待开发)", GUILayout.Height(70), GUILayout.Width(position.width * 0.3f)))
                    {
   
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(position.height * 0.1f);
                GUILayout.FlexibleSpace();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
