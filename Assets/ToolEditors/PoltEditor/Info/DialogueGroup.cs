using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Tools.PlotEditor
{
    public class DialogueGroup : IJsonSwitch
    {
        public JsonData jsonData { get; set; }

        public int curIndex = -1;

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

        public List<Dialogue> dialogues;

        //本组对白大纲描述
        public string Desc
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_DESC) ? jsonData[Const.JSON_KEY_DESC].ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_DESC] = value;
            }
        }

        public DialogueGroup()
        {
            this.jsonData = new JsonData();
            dialogues = new List<Dialogue>();
        }

        public void FromJson(JsonData jsonData)
        {
            this.jsonData = jsonData;
            if (jsonData.Keys.Contains(Const.JSON_KEY_DIALOGUES))
            {
                var dialogueDatas = jsonData[Const.JSON_KEY_DIALOGUES];
                for (int i = 0; i < dialogueDatas.Keys.Count; ++i)
                {
                    var dia = new Dialogue();
                    dia.FromJson(dialogueDatas[i.ToString()]);
                    dialogues.Add(dia);
                }
            }
        }

        public JsonData ToJson()
        {
            var dialogueDatas = new JsonData();
            int index = 0;
            foreach (var dia in dialogues)
            {
                dialogueDatas[index.ToString()] = dia.ToJson();
                index++;
            }
            jsonData[Const.JSON_KEY_DIALOGUES] = dialogueDatas;
            return jsonData;
        }
    }
}
