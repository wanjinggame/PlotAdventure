using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;
using System.Text;
using System.IO;

namespace Plot.Tools
{
    public class DataGenerator
    {
        [MenuItem("DataGenerator/Generator All")]
        static void CreateJson()
        {
            CreateSceneData();
            //todo ��������...
            Debug.Log("Generator All Suc...");
        }

        static void CreateSceneData()
        {
            var jsonData = new JsonData();
            var subData = new JsonData();
            subData["name"] = "�԰�1";
            subData["poltId"] = 1;
            subData["bgm"] = "aaa";
            jsonData["1"] = subData;
            SaveJson(jsonData, "scene_data");
        }

        static void SaveJson(JsonData data, string dataName)
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter jsonWriter = new JsonWriter(sb);
            jsonWriter.PrettyPrint = true;
            JsonMapper.ToJson(data, jsonWriter);
            var path = string.Format("Assets/Resources/Data/{0}.json", dataName);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
