using NorskaLib.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NorskaLib.Storage
{
    public class LocalStorageEditorWindow : EditorWindow
    {
        #region Dependencies

        private LocalStorage LocalStorage;
        //private LocalStorageManager

        #endregion

        private Dictionary<string, bool> foldoutObjects = new Dictionary<string, bool>();
        private Dictionary<Type, bool> foldoutModules = new Dictionary<Type, bool>();
        private bool foldoutFiles = false;

        GUIStyle labelStyleItalic;
        GUIStyle labelStyleBold;

        private struct StorageMembersNames
        {
            public const string modulesShared = "modulesShared";
            public const string modulesSlot = "modulesSlot";
        }

        private struct ModulesMembersNames
        {
            public const string State = "State";
        }

        [MenuItem("Norska/Local Storage/View State")]
        public static new void Show()
        {
            var window = GetWindow<LocalStorageEditorWindow>("Local Storage");
        }

        private void OnGUI()
        {
            TryInitStyles();

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Only available in play mode!");
                return;
            }

            if (DependenciesContainer.Instance is null)
            {
                EditorGUILayout.LabelField("Dependency injection is uninitialized...");
                return;
            }

            if (LocalStorage is null)
            {
                LocalStorage = DependenciesContainer.Instance.Resolve<LocalStorage>();
                EditorGUILayout.LabelField("No LocalStorage instance registred...");
                return;
            }

            if (!LocalStorage.IsInitialized)
            {
                EditorGUILayout.LabelField("Instance is not initialized...");
                return;
            }

            //TryDrawFiles(instance);

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var modulesSharedFieldInfo = typeof(BinaryLocalStorage).GetField(StorageMembersNames.modulesShared, bindingFlags);
            var modulesShared = modulesSharedFieldInfo.GetValue(LocalStorage) as IStorageModule[];
            TryDrawModules(modulesShared, "Shared state:");

            if (!string.IsNullOrEmpty(LocalStorage.ActiveSlot))
            {
                var modulesSlotFieldInfo = typeof(BinaryLocalStorage).GetField(StorageMembersNames.modulesSlot, bindingFlags);
                var modulesSlot = modulesSlotFieldInfo.GetValue(LocalStorage) as IStorageModule[];
                TryDrawModules(modulesSlot, $"Slot '{LocalStorage.ActiveSlot}':");
            }
            else
            {
                EditorGUILayout.LabelField("No slot loaded", labelStyleItalic);
            }
        }

        private void TryDrawModules(IEnumerable<IStorageModule> collection, string headerText)
        {
            EditorGUILayout.LabelField(headerText);

            if (collection is null || !collection.Any())
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("-No modules registred-", labelStyleItalic);

                EditorGUI.indentLevel--;
            }
            else
            {
                foreach (var module in collection)
                    DrawModule(module);
            }
        }

        private void DrawModule(IStorageModule module)
        {
            var type = module.GetType();
            if (!foldoutModules.ContainsKey(type))
                foldoutModules.Add(type, false);

            foldoutModules[type] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutModules[type], type.Name);
            if (foldoutModules[type])
            {
                EditorGUI.indentLevel++;

                if (!module.HasState)
                {
                    EditorGUILayout.LabelField("-No state loaded-", labelStyleItalic);
                }
                else
                {
                    #region Draw state
                    EditorGUI.BeginDisabledGroup(true);

                    var stateInfo = type.GetProperty(ModulesMembersNames.State, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var stateValue = stateInfo.GetValue(module);
                    var stateType = stateValue.GetType();

                    EditorUtils.DrawObject("State", stateValue, true, foldoutObjects, stateType.Name);

                    EditorGUI.EndDisabledGroup();
                    #endregion

                    #region Draw controlls

                    #endregion
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void TryInitStyles()
        {
            if (labelStyleItalic is null)
            {
                labelStyleItalic = new GUIStyle(GUI.skin.label);
                labelStyleItalic.fontStyle = FontStyle.Italic;
            }

            if (labelStyleBold is null)
            {
                labelStyleBold = new GUIStyle(GUI.skin.label);
                labelStyleBold.fontStyle = FontStyle.Bold;
            }
        }
    } 
}