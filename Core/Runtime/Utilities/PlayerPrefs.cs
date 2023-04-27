using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.Utilities
{
    public struct PlayerPrefsUtils
    {
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
    }
}