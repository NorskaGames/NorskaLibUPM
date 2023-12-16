using UnityEngine;
using UnityEditor;

namespace NorskaLib.Extensions
{
    public struct MenuBarExtensions
    {
        [MenuItem("Tools/Clear Player Preferences")]
        public static void ClearPlayerPrefs()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Unavailable in play mode!");
                return;
            }

            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/Open Pesistent Data location")]
        public static void OpenPersistentDataFolder()
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }
    }
}