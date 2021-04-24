using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Plot.Tools.PlotEditor
{
    public enum DialogueType
    {
        Normal_正常对话 = 0,  //正常对话
        Narrator_旁白,    //旁白
        DebutShow_首次出场介绍,   //首次出场
    }

    public class Dialogue : IJsonSwitch
    {
        public JsonData jsonData { get; set; }

        public int Id
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_ID) ? int.Parse(jsonData[Const.JSON_KEY_ID].ToString()) : 0;
            }
            set
            {
                jsonData[Const.JSON_KEY_ID] = value;
            }
        }

        public int NextId
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_NEXT_ID) ? int.Parse(jsonData[Const.JSON_KEY_NEXT_ID].ToString()) : 0;
            }
            set
            {
                jsonData[Const.JSON_KEY_NEXT_ID] = value;
            }
        }

        public DialogueType dialogueType
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_DIALOGUE_TYPE) ? (DialogueType)int.Parse(jsonData[Const.JSON_KEY_DIALOGUE_TYPE].ToString()) : DialogueType.Normal_正常对话;
            }
            set
            {
                jsonData[Const.JSON_KEY_DIALOGUE_TYPE] = (int)value;
            }
        }

        public int characterId
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_CHARACTER_ID) ? int.Parse(jsonData[Const.JSON_KEY_CHARACTER_ID].ToString()) : 0;
            }
            set
            {
                jsonData[Const.JSON_KEY_CHARACTER_ID] = value;
            }
        }

        public string content
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_CONTENT) ? jsonData[Const.JSON_KEY_CONTENT].ToString().ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_CONTENT] = value;
            }
        }

        public Dialogue()
        {
            this.jsonData = new JsonData();
        }

        public void FromJson(JsonData jsonData)
        {
            this.jsonData = jsonData;
        }

        public JsonData ToJson()
        {
            return jsonData;
        }

        public string ShowText()
        {
            if(dialogueType == DialogueType.Narrator_旁白)
            {
                return content;
            }
            else
            {
                return string.Format("{0}：{1}", characterId, content);
            }
        }
    }
}

