using UnityEngine;

namespace NorskaLib.Tools
{
    public sealed class ReadMeComponent : MonoBehaviour
    {
        [TextArea]
        [SerializeField] string text;
    }
}
