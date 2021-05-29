using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plot.Core;
using Plot.Model;
using LitJson;
namespace Plot.UI
{
    public class UIMainModel : UIModel
    {
        public List<DiaogueGroupData> diaogueGroup;
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void Init()
        {
            base.Init();
            diaogueGroup = new List<DiaogueGroupData>();
            GetDialogueGroup();
        }

        public void GetDialogueGroup()
        {
            JsonData json = GameApp.instance.dataManager.Load("dialogue_groups");
            for (int i = 0; i < json.Count; i++)
            {
                
                var data = json[i];
                DiaogueGroupData group = new DiaogueGroupData();
                group.groupId = int.Parse(data["id"].ToString());
                group.desc = data["desc"].ToString();
                JsonData dialogues = data["dialogues"];
                int count = dialogues.Count;
                group.dialogueInfos = new DialogueInfo[count];

                for (int j = 0; j < count; j++)
                {
                    JsonData dialog = dialogues[j.ToString()];
                    group.dialogueInfos[i].dialogueType = (DialogueType)int.Parse(dialog["dialogueType"].ToString());
                    group.dialogueInfos[i].characterId = int.Parse(dialog["characterId"].ToString());
                    group.dialogueInfos[i].content = dialog["content"].ToString();
                }
                diaogueGroup.Add(group);
            }
        }

        public override void Destory()
        {
            base.Destory();
        }
    }
}