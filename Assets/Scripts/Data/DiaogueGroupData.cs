using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Plot.Model
{
    public enum DialogueType
    {
        Normal_正常对话 = 0,  //正常对话
        Narrator_旁白,    //旁白
        DebutShow_首次出场介绍,   //首次出场
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
