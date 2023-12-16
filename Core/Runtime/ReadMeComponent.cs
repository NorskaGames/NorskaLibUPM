using UnityEngine;

namespace NorskaLib.Utilities
{
    public sealed class ReadMeComponent : MonoBehaviour
    {
        [TextArea]
        [SerializeField] string text;
    }
}
