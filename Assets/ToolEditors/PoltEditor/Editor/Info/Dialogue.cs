using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Plot.Tools.PlotEditor
{
    public enum DialogueType
    {
        Normal_�����Ի� = 0,  //�����Ի�
        Narrator_�԰�,    //�԰�
        DebutShow_�״γ�������,   //�״γ���
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
                return jsonData.Keys.Contains(Const.JSON_KEY_DIALOGUE_TYPE) ? (DialogueType)int.Parse(jsonData[Const.JSON_KEY_DIALOGUE_TYPE].ToString()) : DialogueType.Normal_�����Ի�;
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
            if(dialogueType == DialogueType.Narrator_�԰�)
            {
                return content;
            }
            else
            {
                return string.Format("{0}��{1}", characterId, content);
            }
        }
    }
}

