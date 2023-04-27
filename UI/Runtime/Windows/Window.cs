using NorskaLib.Utilities;
using UnityEngine;

namespace NorskaLib.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class Window : MonoBehaviour
    {
        public new RectTransform transform
        { get; private set; }

        public virtual int DefaultOrder => 0;
        public ReactiveValue<int> Order = new ReactiveValue<int>(-1);

        protected virtual void Awake()
        {
            transform = GetComponent<RectTransform>();
        }
    }
}
