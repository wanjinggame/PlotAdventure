using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Plot.Model
{
    public enum DialogueType
    {
        Normal_�����Ի� = 0,  //�����Ի�
        Narrator_�԰�,    //�԰�
        DebutShow_�״γ�������,   //�״γ���
    }

    public class DiaogueGroupData
    {
        public int groupId;
        public string desc;
        public DialogueInfo[] dialogueInfos;
    }
    public class DialogueInfo
    {
        public int characterId;
        public DialogueType dialogueType;
        public string content;
    }


}
