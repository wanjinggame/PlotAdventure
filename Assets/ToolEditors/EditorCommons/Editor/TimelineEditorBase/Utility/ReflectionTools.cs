using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.TimelineEditor
{
    public enum ShowMode
    {
        NormalWindow,
        PopupMenu,
        Utility,
        NoShadow,
        MainWindow,
        AuxWindow,
        PopupMenuWithKeyboardFocus
    }
    public static class ReflectionTools
    {
        private static List<Assembly> _loadedAssemblies;
        private static List<Assembly> loadedAssemblies
        {
            get
            {
                if (_loadedAssemblies == null)
                {
                    _loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                }
                return _loadedAssemblies;
            }
        }

        private static Type[] RTGetExportedTypes(this Assembly asm)
        {
            return asm.GetExportedTypes();
        }

        public static bool RTIsAssignableFrom(this Type type, Type second)
        {
            return type.IsAssignableFrom(second);
        }

        public static bool RTIsAbstract(this Type type)
        {
            return type.IsAbstract;
        }

        private static Dictionary<Type, Type[]> subTypesMap = new Dictionary<Type, Type[]>();
        ///Get a collection of types assignable to provided type, excluding Abstract types
        public static Type[] GetImplementationsOf(Type type)
        {

            Type[] result = null;
            if (subTypesMap.TryGetValue(type, out result))
            {
                return result;
            }

            var temp = new List<Type>();
            foreach (var asm in loadedAssemblies)
            {
                try { temp.AddRange(asm.RTGetExportedTypes().Where(t => type.RTIsAssignableFrom(t) && !t.RTIsAbstract())); }
                catch { continue; }
            }
            return subTypesMap[type] = temp.ToArray();
        }

        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static string SplitCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = char.ToUpper(s[0]) + s.Substring(1);
            return System.Text.RegularExpressions.Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
        }

        public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute
        {
            return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
        }

        private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
        public static Type GetType(string typeName)
        {

            Type type = null;

            if (typeMap.TryGetValue(typeName, out type))
            {
                return type;
            }

            type = Type.GetType(typeName);
            if (type != null)
            {
                return typeMap[typeName] = type;
            }

            foreach (var asm in loadedAssemblies)
            {
                try { type = asm.GetType(typeName); }
                catch { continue; }
                if (type != null)
                {
                    return typeMap[typeName] = type;
                }
            }

            //worst case scenario
            foreach (var t in GetAllTypes())
            {
                if (t.Name == typeName)
                {
                    return typeMap[typeName] = t;
                }
            }

            UnityEngine.Debug.LogError(string.Format("Requested Type with name '{0}', could not be loaded", typeName));
            return null;
        }

        public static Type[] GetAllTypes()
        {
            var result = new List<Type>();
            foreach (var asm in loadedAssemblies)
            {
                try { result.AddRange(asm.RTGetExportedTypes()); }
                catch { continue; }
            }
            return result.ToArray();
        }

        public static object Clone(object oModel)
        {
            var type = oModel.GetType();
            if (type.IsValueType || oModel is string) return oModel;
            var oRes = Activator.CreateInstance(type);
            var lstPro = type.GetProperties();
            foreach (var oPro in lstPro)
            {
                if (oPro.Name == "endTime" || oPro.Name == "sequence" || oPro.Name == "parent") continue;
                var oValue = oPro.GetValue(oModel, null);
                if (oPro.PropertyType.IsArray)
                {
                    var array = oValue as Array;
                    var elementType = oPro.PropertyType.GetElementType();
                    Array newArr = Array.CreateInstance(elementType, array.Length);
                    int index = 0;
                    foreach (var e in array)
                    {
                        newArr.SetValue(Clone(e), index);
                        index++;
                    }
                    oPro.SetValue(oRes, newArr, null);
                }
                else if (oValue is System.Collections.IList)
                {
                    var array = oValue as System.Collections.IList;
                    var proType = oPro.PropertyType;
                    var newList = Activator.CreateInstance(proType) as System.Collections.IList;
                    foreach (var e in array)
                    {
                        newList.Add(Clone(e));
                    }
                    oPro.SetValue(oRes, newList, null);
                }
                else if (oPro.PropertyType.IsClass)
                {
                    try
                    {
                        if (oValue is UnityEngine.Object)
                        {
                            oPro.SetValue(oRes, oValue, null);
                        }
                        else
                        {
                            oPro.SetValue(oRes, Clone(oValue), null);
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        oPro.SetValue(oRes, oValue, null);
                    }
                    catch { }
                }
            }
            var fileds = type.GetFields();
            foreach (var filed in fileds)
            {
                if (filed.Name == "endTime" || filed.Name == "sequence" || filed.Name == "parent") continue;
                var oValue = filed.GetValue(oModel);
                if (filed.FieldType.IsArray)
                {
                    var array = oValue as Array;
                    var elementType = filed.FieldType.GetElementType();
                    Array newArr = Array.CreateInstance(elementType, array.Length);
                    int index = 0;
                    foreach (var e in array)
                    {
                        newArr.SetValue(Clone(e), index);
                        index++;
                    }
                    filed.SetValue(oRes, newArr);
                }
                else if (oValue is System.Collections.IList)
                {
                    var array = oValue as System.Collections.IList;
                    var filedType = filed.FieldType;
                    var newList = Activator.CreateInstance(filedType) as System.Collections.IList;
                    foreach (var e in array)
                    {
                        newList.Add(Clone(e));
                    }
                    filed.SetValue(oRes, newList);
                }
                else if (filed.FieldType.IsClass)
                {
                    try
                    {
                        if (oValue is UnityEngine.Object)
                        {
                            filed.SetValue(oRes, oValue);
                        }
                        else
                        {
                            filed.SetValue(oRes, Clone(oValue));
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        filed.SetValue(oRes, oValue);
                    }
                    catch { }
                }
            }
            return oRes;
        }

        private const BindingFlags flagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

#if UNITY_EDITOR
        public static void ShowPopupWindow(Rect rect, PopupWindowContent windowContent,ShowMode showMode)
        {
            Type popupWindow = typeof(PopupWindow);
            foreach (var method in popupWindow.GetMethods(flagsEverything))
            {
                if (method.Name == "Show" && method.GetParameters().Length == 4)
                {
                    try
                    {
                        method.Invoke(null, new object[] { rect, windowContent, null, (int)showMode });
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }


        public static Rect GUIToScreenRect(Rect rect)
        {
            Type type = typeof(GUIUtility);
            var m = type.GetMethod("GUIToScreenRect", flagsEverything);
            return (Rect)m.Invoke(null, new object[] { rect });
        }

        public static Texture2D LoadIcon(string name)
        {
            Type type = typeof(EditorGUIUtility);
            var m = type.GetMethod("LoadIcon", flagsEverything);
            return (Texture2D)m.Invoke(null, new object[] { name });
        }
#endif
    }
}

