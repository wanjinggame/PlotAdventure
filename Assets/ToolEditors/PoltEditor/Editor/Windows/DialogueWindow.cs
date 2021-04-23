using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.IO;
using LitJson;
using System;

namespace Plot.Tools.PlotEditor
{
    // to do
    public class DialogueWindow : EditorWindow
    {
        [MenuItem("Polt/剧情对白编辑器")]
        public static void Open()
        {
            GetWindow<DialogueWindow>();
        }

        public List<DialogueGroup> dialogueGroups;

        public DialogueGroup curDialogueGroup = null;

        SearchField searchField;
        private Vector2 scPos;
        string search = "";
        private bool show = true;

        private void OnEnable()
        {
            titleContent = new GUIContent("对白编辑器");
            searchField = new SearchField();
            Init();
        }

        public void Init()
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

        private void OnGUI()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }
            GUILayoutTools.DrawTitle("对白编辑器");
            GUILayoutTools.Separator_NoSpaceDoubleLine();
            ToolBar();
            GUILayout.Space(5);
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(position.width * 0.3f)))
                {
                    DrawList();
                }
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
                    DrawDialogueGroup(curDialogueGroup);
                }
                GUILayout.Space(10);
            }
            GUILayoutTools.Separator_DoubleLine();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("保 存", GUILayout.Width(120), GUILayout.Height(30)))
                {
                    Save();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Label("");
            Repaint();
        }

        private void Save()
        {
            var jsonData = new JsonData();
            foreach (var dg in dialogueGroups)
            {
                jsonData[dg.Id.ToString()] = dg.ToJson();
            }
            DataGenerator.SaveJson(jsonData, Const.DIALOGUE_GROUP_DATA);
        }

        Vector2 dialoguePos;
        private void DrawDialogueGroup(DialogueGroup dialogueGroup)
        {
            if (dialogueGroup == null)
            {
                return;
            }
            if (show = GUILayoutTools.Header("对白明细", show, null))
            {
                GUILayout.BeginVertical("helpbox");
                dialogueGroup.Id = EditorGUILayout.IntField("id", dialogueGroup.Id);
                dialogueGroup.Desc = EditorGUILayout.TextField("对白组的简单描述", dialogueGroup.Desc);
                GUILayout.EndVertical();
                dialoguePos = GUILayout.BeginScrollView(dialoguePos, "box");
                GUILayoutTools.Separator_SingleLine();
                foreach (var dia in dialogueGroup.dialogues)
                {
                    DrawDialogue(dia);
                    GUILayoutTools.Separator_SingleLine();
                }
                GUILayout.EndScrollView();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("新增一条对白", GUILayout.Width(100)))
                    {
                        dialogueGroup.dialogues.Add(new Dialogue());
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void DrawDialogue(Dialogue dialogue)
        {
            GUILayout.BeginVertical("helpbox");
            dialogue.dialogueType = (DialogueType)EditorGUILayout.EnumPopup("对白类型", dialogue.dialogueType);
            if (dialogue.dialogueType != DialogueType.Narrator_旁白)
            {
                using (new GUILayout.HorizontalScope())
                {
                    dialogue.characterId = EditorGUILayout.IntField("这话谁说的", dialogue.characterId);
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {

                    }
                }
            }
            EditorGUILayout.LabelField("内容");
            dialogue.content = EditorGUILayout.TextArea(dialogue.content);
            GUILayout.EndVertical();
        }

        private void DrawList()
        {
            search = searchField.OnGUI(search);
            using (new GUILayout.VerticalScope(StyleTools.Skin.box))
            {
                var r = GUILayoutUtility.GetRect(position.width * 0.4f, 20);
                r.width /= 2;
                GUI.Label(r, "对白组ID", EditorStyles.toolbarButton);
                r.x += r.width;
                GUI.Label(r, "对白组简单描述", EditorStyles.toolbarButton);
                scPos = GUILayout.BeginScrollView(scPos);
                foreach (var dialogue in dialogueGroups)
                {
                    if (dialogue.Id.ToString().Contains(search) || dialogue.Desc.ToLower().Contains(search.ToLower()))
                    {

                        var evt = Event.current;
                        var rect = GUILayoutUtility.GetRect(position.width * 0.4f, 20);
                        var newRect = new Rect(rect);
                        rect.width /= 2;
                        GUI.Label(rect, " " + dialogue.Id, curDialogueGroup == dialogue ? "SelectionRect" : "Tooltip");
                        rect.x += rect.width;
                        GUI.Label(rect, dialogue.Desc, curDialogueGroup == dialogue ? "SelectionRect" : "Tooltip");
                        var style = curDialogueGroup == dialogue ? StyleTools.Skin.box : "box";
                        if (newRect.Contains(evt.mousePosition))
                        {
                            if (evt.button == 0)
                            {
                                if (evt.type == EventType.MouseDown)
                                {
                                    GUI.FocusControl(null);
                                    curDialogueGroup = dialogue;
                                    GUIUtility.ExitGUI();
                                }
                            }
                            if (evt.button == 1)
                            {
                                if (evt.type == EventType.MouseDown)
                                {
                                    GUI.FocusControl(null);
                                    ShowRightClick(dialogue);
                                }
                            }
                            if (dialogue != curDialogueGroup)
                            {
                                style = "box";
                                GUI.Label(newRect, "", style);
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
        }

        private void ShowRightClick(DialogueGroup dialogue)
        {

        }

        private void ToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(5);

            if (GUILayout.Button("新建一组对白", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                var d = new DialogueGroup();
                dialogueGroups.Add(d);
            }

            GUILayout.Label("");
            GUILayout.Space(3);
            GUILayout.EndHorizontal();
        }
    }
}
