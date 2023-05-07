using UnityEngine;

namespace NorskaLib.Spreadsheets
{
    public abstract class SpreadsheetsContainerBase : ScriptableObject
    {
        // TO DO:
        // 1. Multiple source support:
        // - Google Spreadsheets
        // - Yandex Spreadsheets

        // 2. Serialization support:
        // - *.json 
        // - *.bin

        [SerializeField] [HideInInspector]
        public string documentID;
    }
}