using Plot.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.Text;
using System.IO;
using Plot.Utility;

namespace Plot.Data
{
    public class DataManager : BaseManager
    {
        const string PATH_FORMAT = "Assets/Resources/Data/{0}.json";

        private List<string> datas;
        private Dictionary<string, JsonData> loadedData = new Dictionary<string, JsonData>();

        public override void Init()
        {
            //dataName
            datas = new List<string>();
            datas.Add("scene_data");
        }

        public JsonData Load(string dataName)
        {
            if (!datas.Contains(dataName))
            {
                if (GameLog.EnableLog(GameLog.LV_ERROR))
                {
                    GameLog.LogError(dataName + " No Exists!");
                }
                return null;
            }
            if (loadedData.ContainsKey(dataName))
            {
                return loadedData[dataName];
            }
#if UNITY_EDITOR
            var path = string.Format(PATH_FORMAT, dataName);
            var text = File.ReadAllText(path, Encoding.UTF8);
            var data = JsonMapper.ToObject(text);
            loadedData.Add(dataName, data);
            return data;
#else
            //todo   by resourceMgr Load Data File

            return null;
#endif
        }
    }
}
