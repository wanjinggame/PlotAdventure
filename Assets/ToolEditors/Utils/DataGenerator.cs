#if UNITY_EDITOR
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
            //todo 手码数据...
            Debug.Log("Generator All Suc...");
        }

        static void CreateSceneData()
        {
            var jsonData = new JsonData();
            var subData = new JsonData();
            subData["name"] = "对白1";
            subData["poltId"] = 1;
            subData["bgm"] = "aaa";
            jsonData["1"] = subData;
            SaveJson(jsonData, "scene_data");
        }

        public static void SaveJson(JsonData data, string dataName)
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
        [MenuItem("DataGenerator/RenameFiles")]
        static void RenameFiles()
        {
            string targetDir = @"C:\Projects\Git\PlotAdventure.git\trunk\Assets\Resources\Character";
            var files = Directory.GetFiles(targetDir, "*.jpg", SearchOption.AllDirectories);
            var resPath = "Assets/Resources/Character/{0}";
            var count = 10682;
            foreach (var file in files)
            {
                if (file.EndsWith(".jpg"))
                {
                    var name = Path.GetFileName(file);
                    Debug.Log(string.Format(resPath, name) + " -->  " + string.Format(resPath + ".jpg", count));
                    File.Copy(string.Format(resPath, name), string.Format(resPath + ".jpg", count));
                    File.Delete(string.Format(resPath, name));
                    count++;
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif