using UnityEngine;

namespace NorskaLib.Tools
{
    [CreateAssetMenu(fileName = "ReadMe", menuName = "Editor/ReadMe", order = 1)]
    public sealed class ReadMeAsset : ScriptableObject
    {
        [TextArea]
        [SerializeField] string text;
    }
}
