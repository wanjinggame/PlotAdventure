using Plot.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;
using System.Text;
using System.IO;

namespace Plot.Data
{

    public class DataManager : BaseManager
    {
        [MenuItem("Test/CreateJson")]
        static void CreateJson()
        {
            var jsonData = new JsonData();
            var subData = new JsonData();
            subData["name"] = "¶Ô°×1";
            subData["poltId"] = 1;
            subData["bgm"] = "aaa";
            jsonData["1"] = subData;

            StringBuilder sb = new StringBuilder();
            JsonWriter jsonWriter = new JsonWriter(sb);
            jsonWriter.PrettyPrint = true;
            JsonMapper.ToJson(jsonData, jsonWriter);

            var path = "Assets/Resources/Data/scene_data.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

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
            if (loadedData.ContainsKey(dataName))
            {
                return loadedData[dataName];
            }
            return null;
        }
    }
}
