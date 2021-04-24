using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using System;

namespace Plot.Tools.PlotEditor
{
    public class DemoShowWinodw : EditorWindow
    {
        private void OnEnable()
        {
            titleContent = new GUIContent("��ʾ��");
            minSize = new Vector2(1172, 745);
        }

        public DialogueGroup curShow = null;
        private bool auto;

        public void OnGUI()
        {
            GUILayoutTools.DrawTitle("��ʾ��");
            GUILayoutTools.Separator_NoSpaceDoubleLine();
            ToolBar();
            GUILayout.Label("��ǰ���ж԰�", StyleTools.MiddleCenterLab);
            var dgs = MainWinodws.Instance.dialogueGroups;
            using (new GUILayout.HorizontalScope())
            {
                foreach (var dg in dgs)
                {
                    if (GUILayout.Button(dg.Id + ":" + dg.Desc))
                    {
                        if (curShow != dg)
                        {
                            curShow = dg;
                            auto = false;
                        }
                    }
                }
            }
            GUILayoutTools.Separator_SingleLine();
            ShowDialogue();
            Repaint();
        }

        private void Update()
        {
            AutoShow();
        }

        double lastTime = 0;
        private void AutoShow()
        {
            if (auto && curShow != null)
            {
                if (EditorApplication.timeSinceStartup - lastTime > 2)
                {
                    Next();
                    lastTime = EditorApplication.timeSinceStartup;
                }
            }
            else
            {
                lastTime = EditorApplication.timeSinceStartup;
            }
        }

        Vector2 scPos;
        private void ShowDialogue()
        {
            if (curShow == null)
            {
                ShowNotification(new GUIContent("��ѡ��һ��԰׿�ʼ�鿴"));
            }
            else
            {
                RemoveNotification();
                GUILayout.Label("������ʾ " + curShow.Id + ":" + curShow.Desc);
                using (var sc = new GUILayout.ScrollViewScope(scPos, "box"))
                {
                    for (int i = 0; i <= curShow.curIndex; ++i)
                    {
                        var dia = curShow.dialogues[i];
                        if (dia.dialogueType == DialogueType.Narrator_�԰�)
                        {
                            GUILayout.Label(GUILayoutTools.GetTextSizeOf(dia.ShowText(), 13));
                        }
                        else
                        {
                            GUILayout.Label(GUILayoutTools.GetTextSizeOf(string.Format("��{0}��: {1}", GetCharacterName(dia.characterId), dia.content), 13));
                        }
                        GUILayout.Space(10);
                    }
                    scPos = sc.scrollPosition;
                }
            }
        }

        private void ToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(5);
            var enable = GUI.enabled;
            GUI.enabled = curShow != null;
            if (GUILayout.Button("���¿�ʼ", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                curShow.curIndex = -1;
                auto = false;
            }
            if (GUILayout.Button(auto ? "�ر��Զ�" : "�Զ�", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                auto = !auto;
            }
            if (GUILayout.Button("��һ��", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                Pre();
                auto = false;
            }
            if (GUILayout.Button("��һ��", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                Next();
                auto = false;
            }
            GUI.enabled = enable;
            GUILayout.Label("");
            GUILayout.Space(3);
            GUILayout.EndHorizontal();
        }

        private void Next()
        {
            var next = curShow.curIndex + 1;
            curShow.curIndex = next >= curShow.dialogues.Count ? curShow.dialogues.Count - 1 : next;
        }

        private void Pre()
        {
            var pre = curShow.curIndex - 1;
            curShow.curIndex = pre <= -1 ? -1 : pre;
        }

        private string GetCharacterName(int id)
        {
            var ch = MainWinodws.Instance.characterInfos.Find((c) => { return c.Id == id; });
            return ch == null ? "" : ch.Name;
        }
    }
}
