using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;
using UnityEditor.IMGUI.Controls;
using System;

namespace Plot.Tools.PlotEditor
{
    public class CharacterConfigWindow : EditorWindow
    {
        [MenuItem("Polt/��ɫ����")]
        public static void Open()
        {
            GetWindow<CharacterConfigWindow>();
        }

        public List<CharacterInfo> characterInfos;

        public CharacterInfo curChar = null;

        SearchField searchField;
        private Vector2 scPos;
        string search = "";

        private void OnEnable()
        {
            titleContent = new GUIContent("��ɫ����");
            searchField = new SearchField();
            Init();
        }

        public void Init()
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

        private void OnGUI()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
            }

            GUILayoutTools.DrawTitle("��ɫ����");
            GUILayoutTools.Separator_NoSpaceDoubleLine();
            ToolBar();
            GUILayout.Space(5);
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(position.width * 0.4f)))
                {
                    DrawList();
                }
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
                    DrawCharacter(curChar);
                }
                GUILayout.Space(10);
            }
            GUILayoutTools.Separator_DoubleLine();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("�� ��", GUILayout.Width(120), GUILayout.Height(30)))
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
            foreach (var c in characterInfos)
            {
                jsonData[c.Id.ToString()] = c.ToJson();
            }
            DataGenerator.SaveJson(jsonData, Const.CHARACTER_DATA);
        }

        private void ToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(5);

            if (GUILayout.Button("�½���ɫ", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                var c = new CharacterInfo();
                characterInfos.Add(c);
            }

            GUILayout.Label("");
            GUILayout.Space(3);
            GUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            search = searchField.OnGUI(search);
            using (new GUILayout.VerticalScope(StyleTools.Skin.box))
            {
                var r = GUILayoutUtility.GetRect(position.width * 0.6f, 20);
                r.width /= 2;
                GUI.Label(r, "��ɫID", EditorStyles.toolbarButton);
                r.x += r.width;
                GUI.Label(r, "�� ��", EditorStyles.toolbarButton);
                scPos = GUILayout.BeginScrollView(scPos);
                foreach (var character in characterInfos)
                {
                    if (character.Id.ToString().Contains(search) || character.Name.ToLower().Contains(search.ToLower()))
                    {
                        var evt = Event.current;
                        var rect = GUILayoutUtility.GetRect(position.width * 0.6f, 20);
                        var newRect = new Rect(rect);
                        rect.width /= 2;
                        GUI.Label(rect, " " + character.Id, curChar == character ? "SelectionRect" : "Tooltip");
                        rect.x += rect.width;
                        GUI.Label(rect, character.Name, curChar == character ? "SelectionRect" : "Tooltip");
                        var style = curChar == character ? StyleTools.Skin.box : "box";
                        if (newRect.Contains(evt.mousePosition))
                        {
                            if (evt.button == 0)
                            {
                                if (evt.type == EventType.MouseDown)
                                {
                                    GUI.FocusControl(null);
                                    curChar = character;
                                    GUIUtility.ExitGUI();
                                }
                            }
                            if (evt.button == 1)
                            {
                                if (evt.type == EventType.MouseDown)
                                {
                                    GUI.FocusControl(null);
                                    ShowRightClick(character);
                                }
                            }
                            if (character != curChar)
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

        static bool showCharacterInfo = true;

        public static void DrawCharacter(CharacterInfo character)
        {
            if (character == null)
            {
                return;
            }
            if (showCharacterInfo = GUILayoutTools.Header(character.Name + " ��ϸ����", showCharacterInfo, null))
            {
                GUILayout.BeginVertical("helpbox");
                character.Id = EditorGUILayout.IntField("��ɫID", character.Id);

                character.Name = EditorGUILayout.TextField("��ɫ��", character.Name);

                character.characterSex = (CharacterSex)EditorGUILayout.EnumPopup("�Ա�", character.characterSex);

                using (new GUILayout.HorizontalScope())
                {
                    character.model = EditorGUILayout.TextField("ģ��/ͼƬ", character.model);
                    if (GUILayout.Button("ѡ��", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        //todo  ��Դѡ�� 

                    }
                }

                EditorGUILayout.LabelField("������");
                character.otherInfo = EditorGUILayout.TextArea(character.otherInfo);
                GUILayout.EndVertical();
                //to do   ��ԴԤ��
            }
        }

        private void ShowRightClick(CharacterInfo characterInfo)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("ɾ���ý�ɫ"), false, () =>
            {
                if (characterInfos.Contains(characterInfo))
                {
                    characterInfos.Remove(characterInfo);
                }
    
            });
            genericMenu.ShowAsContext();
        }

        private void OnDestroy()
        {

        }
    }
}
