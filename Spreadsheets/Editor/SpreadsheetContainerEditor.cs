using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NorskaLib.Spreadsheets
{
    [CustomEditor(typeof(SpreadsheetsContainerBase), true)]
    public class SpreadsheetContainerEditor : Editor
    {
        bool foldout = true;
        Dictionary<string, bool> pagesToggles;

        ImportQueue importQueue;

        public override void OnInspectorGUI()
        {
            var container = (SpreadsheetsContainerBase)target;

            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Import settings");
            if (foldout)
                DrawGUI(container);
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(16);

            base.OnInspectorGUI();
        }

        void DrawGUI(SpreadsheetsContainerBase container)
        {
            var listsInfos = container.GetType().GetFields()
                .Where(fi => Attribute.IsDefined(fi, typeof(PageNameAttribute)))
                .OrderBy(i => i.Name).ToArray();

            if (pagesToggles == null)
            {
                pagesToggles = new Dictionary<string, bool>();
                for (int i = 0; i < listsInfos.Length; i++)
                    pagesToggles.Add(listsInfos[i].Name, EditorPrefs.GetBool(listsInfos[i].Name));
            }

            EditorGUILayout.BeginVertical("box");

            #region Draw doc ID field

            container.documentID = EditorGUILayout.TextField(
                new GUIContent(
                    "Document Id",
                    "The XXXX part in 'https://docs.google.com/spreadsheets/d/XXXX/edit' URL. NOTE: The document must be accessable by link."),
                container.documentID);

            #endregion

            #region Draw controll buttons

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All"))
                SelectAll(true);

            if (GUILayout.Button("None"))
                SelectAll(false);

            if (GUILayout.Button("Import"))
            {
                if (pagesToggles.Any(t => t.Value == true))
                {
                    if (string.IsNullOrEmpty(container.documentID))
                    {
                        Debug.LogError($"Document ID is not specified!");
                        return;
                    }

                    EditorUtility.DisplayProgressBar("Downloading definitions", "Initializing...", 0);

                    importQueue = new ImportQueue(container, listsInfos.Where(i => pagesToggles[i.Name] == true).ToArray());

                    importQueue.onComplete += OnImportQueueComplete;
                    importQueue.onOutputChanged += OnOutputChanged;
                    importQueue.onProgressChanged += OnProgressChanged;

                    importQueue.Run();
                }
                else
                    Debug.LogWarning("Nothing is selected to import");
            }

            EditorGUILayout.EndHorizontal();

            #endregion

            #region Draw import flags

            EditorGUILayout.LabelField("Pages to import:");

            EditorGUI.indentLevel += 1;

            var keys = new List<string>(pagesToggles.Keys);
            foreach (var key in keys)
            {
                pagesToggles[key] = EditorGUILayout.Toggle($"{key}", pagesToggles[key]);
                EditorPrefs.SetBool(key, pagesToggles[key]);
            }

            EditorGUI.indentLevel -= 1;

            #endregion

            EditorGUILayout.EndVertical();
        }

        void SelectAll(bool mode)
        {
            var keys = new List<string>(pagesToggles.Keys);
            foreach (var key in keys)
                pagesToggles[key] = mode;
        }

        void OnProgressChanged()
        {
            EditorUtility.DisplayProgressBar("Downloading definitions", importQueue.Output, importQueue.Progress);
        }

        void OnOutputChanged()
        {
            EditorUtility.DisplayProgressBar("Downloading definitions", importQueue.Output, importQueue.Progress);
        }

        void OnImportQueueComplete(SpreadsheetsContainerBase container)
        {
            EditorUtility.SetDirty(container);

            EditorUtility.ClearProgressBar();

            importQueue.onComplete -= OnImportQueueComplete;
            importQueue.onOutputChanged -= OnOutputChanged;
            importQueue.onProgressChanged -= OnProgressChanged;
            importQueue = null;
        }
    } 
}
