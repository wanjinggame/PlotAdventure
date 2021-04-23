using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Plot.Tools.PlotEditor
{
    public enum CharacterSex
    {
        Man = 0,
        WoMan,
        Girl,
        Boy,
    }

    public class CharacterInfo : IJsonSwitch
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

        public CharacterSex characterSex
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_SEX) ? (CharacterSex)int.Parse(jsonData[Const.JSON_KEY_SEX].ToString()) : CharacterSex.Man;
            }
            set
            {
                jsonData[Const.JSON_KEY_SEX] = (int)value;
            }
        }

        public string Name
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_NAME) ? jsonData[Const.JSON_KEY_NAME].ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_NAME] = value;
            }
        }

        public string model
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_MODEL) ? jsonData[Const.JSON_KEY_MODEL].ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_MODEL] = value;
            }
        }

        public string showIcon
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_SHOW_ICON) ? jsonData[Const.JSON_KEY_SHOW_ICON].ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_SHOW_ICON] = value;
            }
        }

        public string otherInfo
        {
            get
            {
                return jsonData.Keys.Contains(Const.JSON_KEY_OTHER_INFO) ? jsonData[Const.JSON_KEY_OTHER_INFO].ToString() : "";
            }
            set
            {
                jsonData[Const.JSON_KEY_OTHER_INFO] = value;
            }
        }

        public CharacterInfo()
        {
            jsonData = new JsonData();
        }

        public void FromJson(JsonData jsonData)
        {
            this.jsonData = jsonData;
        }

        public JsonData ToJson()
        {
            return jsonData;
        }
    }
}
