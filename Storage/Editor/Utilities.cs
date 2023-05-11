using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NorskaLib.Storage
{
    public struct EditorUtils
    {
        public class MemberInfoMeta
        {
            private static readonly object[] NoIndex = new object[] { };

            private FieldInfo _fieldInfo;
            private PropertyInfo _propertyInfo;
            private object _obj;

            public Type Type
            { get; private set; }

            public bool Settable
            {
                get
                {

                    return _fieldInfo != null && !_fieldInfo.IsLiteral
                        || _propertyInfo != null && _propertyInfo.CanWrite;
                }
            }

            public object Value
            {
                get
                {
                    if (_fieldInfo != null)
                        return _fieldInfo.GetValue(_obj);
                    return _propertyInfo.GetValue(_obj, NoIndex);
                }
                set
                {
                    if (_fieldInfo != null)
                        _fieldInfo.SetValue(_obj, value);
                    else if (_propertyInfo.CanWrite)
                        _propertyInfo.SetValue(_obj, value, NoIndex);
                }
            }

            public MemberInfo MemberInfo
            {
                get
                {
                    if (_propertyInfo != null)
                    {
                        return _propertyInfo;
                    }
                    else
                    {
                        return _fieldInfo;
                    }
                }
            }

            public string Name
            { get; private set; }

            public MemberInfoMeta(FieldInfo fieldInfo, object obj)
            {
                _obj = obj;
                _fieldInfo = fieldInfo;
                _propertyInfo = null;

                Type = fieldInfo.FieldType;
                Name = fieldInfo.Name;
            }

            public MemberInfoMeta(PropertyInfo propertyInfo, object obj)
            {
                _obj = obj;
                _fieldInfo = null;
                _propertyInfo = propertyInfo;

                Type = propertyInfo.PropertyType;
                Name = propertyInfo.Name;
            }
        }

        private struct EnumerableInfos
        {
            public struct Methods
            {
                public static readonly MethodInfo OfType;
                public static readonly MethodInfo ToList;
                public static readonly MethodInfo ToArray;

                static Methods()
                {
                    var type = typeof(System.Linq.Enumerable);

                    OfType = type.GetMethod("OfType");
                    ToList = type.GetMethod("ToList");
                    ToArray = type.GetMethod("ToArray");
                }
            }
        }

        public static void DrawObject(string Name, object obj, bool disabled, Dictionary<string, bool> foldouts, string path = "", bool zeroIndent = false)
        {
            if (obj == null)
            {
                var indentLevel = EditorGUI.indentLevel;
                if (zeroIndent)
                {
                    EditorGUI.indentLevel = 0;
                }

                EditorGUILayout.BeginHorizontal();

                if (!string.IsNullOrEmpty(Name))
                    DrawLabel(Name + ": ", zeroIndent);
                using (new EditorGUI.DisabledGroupScope(disabled))
                    EditorGUILayout.LabelField("null");

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel = indentLevel;
                return;
            }

            var objType = obj.GetType();
            if (objType == typeof(int))
            {
                DrawPrimitive(Name, obj, disabled, (o) => EditorGUILayout.IntField((int)o), zeroIndent);
                return;
            }
            else if (objType == typeof(long))
            {
                DrawPrimitive(Name, obj, disabled, (o) => EditorGUILayout.LongField((long)o), zeroIndent);
                return;
            }
            else if (objType == typeof(string))
            {
                DrawPrimitive(Name, obj, disabled, (o) => EditorGUILayout.TextField((string)o), zeroIndent);
                return;
            }
            else if (objType == typeof(bool))
            {
                DrawPrimitive(Name, obj, disabled, (o) => EditorGUILayout.Toggle((bool)o), zeroIndent);
                return;
            }
            else if (objType == typeof(float))
            {
                DrawPrimitive(Name, obj, disabled, (o) => EditorGUILayout.FloatField((float)o), zeroIndent);
                return;
            }
            else if (objType == typeof(DateTime))
            {
                DrawPrimitive(Name, obj, disabled, (o) => {
                    var date = (DateTime)o;
                    EditorGUILayout.BeginHorizontal();
                    var year = EditorGUILayout.IntField(date.Year, GUILayout.Width(40));
                    var month = EditorGUILayout.IntField(date.Month, GUILayout.Width(20));
                    var day = EditorGUILayout.IntField(date.Day, GUILayout.Width(20));
                    EditorGUILayout.LabelField("", GUILayout.Width(5));
                    var hour = EditorGUILayout.IntField(date.Hour, GUILayout.Width(20));
                    EditorGUILayout.LabelField(":", GUILayout.Width(7));
                    var minute = EditorGUILayout.IntField(date.Minute, GUILayout.Width(20));
                    var second = EditorGUILayout.IntField(date.Second, GUILayout.Width(20));
                    var result = new DateTime(year, month, day, hour, minute, second);
                    if (GUILayout.Button("Now", GUILayout.Width(40)))
                    {
                        result = DateTime.UtcNow;
                    }
                    EditorGUILayout.EndHorizontal();
                    return result;
                }, zeroIndent);

                return;
            }
            else if (objType.IsEnum)
            {
                EditorGUILayout.EnumPopup(Name + ": ", (Enum)obj);
                return;
            }
            // TO DO:
            // Proper KeyValue pair drawing...
            else if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                //foreach (var pair in obj as IDictionary)
                //{

                //}

                DrawObject(Name, JsonConvert.SerializeObject(obj), true, foldouts, path);
                return;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objType))
            {
                var enumType = objType.GetGenericArguments().Any()
                    ? objType.GetGenericArguments().First()
                    : objType.GetElementType();

                using (new EditorGUI.DisabledGroupScope(disabled))
                {
                    using (new GUILayout.VerticalScope())
                    {
                        var k = path + Name;
                        if (!foldouts.ContainsKey(k))
                            foldouts[k] = false;

                        foldouts[k] = EditorGUILayout.Foldout(foldouts[k], Name + ": ");

                        if (foldouts[k])
                        {
                            var count = 0;
                            ++EditorGUI.indentLevel;
                            foreach (var item in obj as IEnumerable)
                            {
                                var name = string.Format("Element {0}", count);
                                var p = path + Name + count.ToString();
                                DrawObject(name, item, disabled, foldouts, p, zeroIndent);
                                ++count;
                            }

                            if (count == 0)
                                EditorGUILayout.LabelField("Empty");

                            --EditorGUI.indentLevel;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                //TODO
                return;
            }

            var fold = objType.GetProperties().Any() || objType.GetFields().Any();
            if (!fold)
            {
                EditorGUILayout.LabelField("  " + Name);
                return;
            }
            var key = path + objType.FullName;
            if (!foldouts.TryGetValue(key, out fold))
                foldouts[key] = false;
            foldouts[key] = EditorGUILayout.Foldout(foldouts[key], Name);
            if (foldouts[key])
            {
                var membersMetas = GetMembersMetas(obj);
                GUILayout.BeginVertical();
                ++EditorGUI.indentLevel;

                foreach (var m in membersMetas)
                    DrawObject(m.Name, m.Value, disabled, foldouts, key, zeroIndent);

                --EditorGUI.indentLevel;
                GUILayout.EndVertical();
            }

            return;
        }

        private static IEnumerable<MemberInfoMeta> GetMembersMetas(object obj)
        {
            var type = obj.GetType();
            var metas = new List<MemberInfoMeta>();

            var propsMetas = type.GetProperties()
                .Where(p => p.GetIndexParameters().Length == 0)
                .Select(p => new MemberInfoMeta(p, obj));
            metas.AddRange(propsMetas);

            var fieldMetas = type.GetFields()
                .Select(f => new MemberInfoMeta(f, obj));
            metas.AddRange(fieldMetas);

            return metas;
        }

        private static T DrawPrimitive<T>(string Name, object obj, bool disabled, Func<object, T> drawFunc, bool zeroIndent = false)
        {
            var indentLevel = EditorGUI.indentLevel;
            if (zeroIndent)
            {
                EditorGUI.indentLevel = 0;
            }

            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(Name))
                DrawLabel(Name + ":", zeroIndent);
            using (new EditorGUI.DisabledGroupScope(disabled))
            {
                var result = drawFunc(obj);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = indentLevel;
                return result;
            }
        }

        public static bool DrawButton(string label, GUIStyle style = null)
        {
            return style == null ?
                GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label) :
                GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label, style);
        }

        public static bool DrawFoldout(string key, string visibleLabel)
        {
            var fold = EditorGUILayout.Foldout(EditorPrefs.GetBool(key, false), visibleLabel);
            EditorPrefs.SetBool(key, fold);
            return fold;
        }

        public static void DrawLabel(string text, bool zeroIndent)
        {
            if (!zeroIndent)
            {
                EditorGUILayout.LabelField(new GUIContent(text), "label");
            }
            else
            {
                Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(text), "label");
                var aligment = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUI.Label(labelRect, text);
                GUI.skin.label.alignment = aligment;
            }
        }
    }
}