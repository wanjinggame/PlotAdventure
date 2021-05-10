#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using System.Text;
//using LuaInterface;
using System.Collections;
using System.IO;

namespace UnityEditor.TimelineEditor
{
    public static class EditorTools
    {
        private static List<EditorWindow> needCloseWindows = new List<EditorWindow>();
        static EditorTools()
        {
            if (needCloseWindows != null) needCloseWindows.Clear();
            else needCloseWindows = new List<EditorWindow>();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingEditMode)
            {
                foreach (var window in needCloseWindows)
                {
                    if (window) window.Close();
                }
                needCloseWindows.Clear();
            }
        }

        public static void AddWindow(EditorWindow window)
        {
            if (needCloseWindows == null) needCloseWindows = new List<EditorWindow>();
            needCloseWindows.Add(window);
        }
        public static void ClearWindow()
        {
            if (needCloseWindows == null) needCloseWindows.Clear();
        }

        public struct TypeMetaInfo
        {
            public Type type;
            public string name;
            public Type[] attachableTypes;
        }

        //Get all non abstract derived types of base type in the current loaded assemplies
        public static List<TypeMetaInfo> GetTypeMetaDerivedFrom(Type baseType, bool getOld = true)
        {
            var infos = new List<TypeMetaInfo>();
            foreach (var type in ReflectionTools.GetImplementationsOf(baseType))
            {
                if(type.BaseType != baseType)
                {
                    continue;
                }
                if (type.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).FirstOrDefault() != null)
                {
                    continue;
                }
                var nameAtt = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
                if (getOld)
                {
                    if (nameAtt != null && nameAtt.old)
                    {
                        var info = new TypeMetaInfo();
                        info.type = type;

                        info.name = nameAtt != null ? nameAtt.name : type.Name.SplitCamelCase();

                        var attachAtt = type.GetCustomAttributes(typeof(AttachableAttribute), true).FirstOrDefault() as AttachableAttribute;
                        if (attachAtt != null) { info.attachableTypes = attachAtt.types; }

                        infos.Add(info);
                    }
                }
                else
                {
                    if (nameAtt != null && !nameAtt.old)
                    {
                        var info = new TypeMetaInfo();
                        info.type = type;

                        info.name = nameAtt != null ? nameAtt.name : type.Name.SplitCamelCase();

                        var attachAtt = type.GetCustomAttributes(typeof(AttachableAttribute), true).FirstOrDefault() as AttachableAttribute;
                        if (attachAtt != null) { info.attachableTypes = attachAtt.types; }

                        infos.Add(info);
                    }
                }
            }

            infos = infos.OrderBy(i => i.name).ToList();
            return infos;
        }

        public static bool CanCrossBlend(this Clip directable, Clip other)
        {
            if (directable == null || other == null) { return false; }
            if ((directable.canCrossBlend || other.canCrossBlend) && directable.GetType() == other.GetType())
            {
                return true;
            }
            return false;
        }

        public static void DrawLoopedLines(Rect rect, float length, float maxLength, float offset)
        {
            if (length != 0 && maxLength != 0)
            {
                length = Mathf.Abs(length);
                maxLength = Mathf.Abs(maxLength);
                UnityEditor.Handles.color = new Color(0, 0, 0, 0.2f);
                for (var f = offset; f < maxLength; f += length)
                {
                    var posX = (f / maxLength) * rect.width;
                    UnityEditor.Handles.DrawLine(new Vector2(posX, 0), new Vector2(posX, rect.height));
                }
                UnityEditor.Handles.color = Color.white;
            }
        }

        public static T CleanPopup<T>(string prefix, T selected, List<T> options, params GUILayoutOption[] GUIOptions)
        {

            var index = -1;
            if (options.Contains(selected))
            {
                index = options.IndexOf(selected);
            }

            var stringedOptions = options.Select(o => o != null ? o.ToString() : "NONE");

            GUI.enabled = options.Count > 0;
            if (!string.IsNullOrEmpty(prefix)) index = EditorGUILayout.Popup(prefix, index, stringedOptions.ToArray(), GUIOptions);
            else index = EditorGUILayout.Popup(index, stringedOptions.ToArray(), GUIOptions);
            GUI.enabled = true;

            return index == -1 ? default(T) : options[index];
        }

        public static void DrawTitle(string title)
        {
            var rich = GUI.skin.label.richText;
            GUI.skin.label.richText = true;
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUILayout.BeginHorizontal(Styles.headerBoxStyle);
            GUI.color = Color.white;
            GUILayout.Label(string.Format("<size=22><b>{0}</b></size>",title));
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUI.skin.label.richText = rich;
        }

        public static string RunSvnCmd(string cmd)
        {
            string cmdOutput;
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Console.InputEncoding = System.Text.Encoding.UTF8;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
            p.StandardInput.Flush();
            p.StandardInput.Close();

            cmdOutput = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            if (p.StandardError.ReadToEnd().Contains("不是内部或外部命令"))
            {
                if (EditorUtility.DisplayDialog("提示", "未安装SVN命令行，该流程运行失败。\n请下载安装程序并勾选安装command line client tools。", "打开网址", "取消"))
                {
                    Application.OpenURL("https://tortoisesvn.net/downloads.html");
                }
                throw new Exception("请安装svn命令行工具");
            }

            p.Close();
            return cmdOutput;
        }

        static string[] folders = new string[]
        {
            "Assets/Res/Character",
            "Assets/Res/Effect",
            "Assets/ResTemp/Character",
        };
        public static AnimationClip FindAnimationClip(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var GUIDs = AssetDatabase.FindAssets(name, folders);
            if (GUIDs.Length < 1)
            {
                return null;
            }
            for (int i = 0; i < GUIDs.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                if (path.EndsWith(".fbx")  || path.EndsWith(".FBX") || path.EndsWith(".anim"))
                {
                    UnityEngine.Object clip = System.Array.Find(AssetDatabase.LoadAllAssetsAtPath(path), e => e is AnimationClip && e.name == name);
                    if (clip != null)
                    {
                        return (AnimationClip)clip;
                    }
                    return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                }
            }
            return null;
        }


        //这里排序都按照字符串进行排序
        static int LuatableComparison(DictionaryEntry x, DictionaryEntry y)
        {
            var keyX = x.Key.ToString();
            var keyY = y.Key.ToString();
            return keyX.CompareTo(keyY);
        }

        static int LuatableComparisonInt(DictionaryEntry x, DictionaryEntry y)
        {
            var keyX = x.Key.ToString();
            var keyY = y.Key.ToString();
            int numX, numY;
            if (int.TryParse(keyX, out numX) && int.TryParse(keyY, out numY))
            {
                return numX.CompareTo(numY);
            }
            else
            {
                return keyX.CompareTo(keyY);
            }
        }

        public static int GetIntNumberBinaryByIndex(int number, int index)
        {
            return (number >> index) & 1;
        }

        public static int SetIntNumberBinaryByIndex(int number, int index, int val)
        {
            number ^= (number & (1 << index)) ^ (val << index);
            return number;
        }

        public static void GetDirectoryAllFiles(DirectoryInfo directoryInfo, List<FileInfo> files, string nameKeyWord = "")
        {
            if (files == null || directoryInfo == null) return;
            var fileInfos = directoryInfo.GetFiles("*" + nameKeyWord + "*", SearchOption.AllDirectories);
            files.AddRange(fileInfos);
        }
    }
}
#endif

