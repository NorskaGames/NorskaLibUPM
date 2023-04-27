using UnityEngine;

namespace NorskaLib.UI.Tabs
{
    public abstract class TabContent : MonoBehaviour
    {
        /// <summary>
        /// Is called on TabGroup.Awake(), so you can use this method to force initialization of content which is inactive by default.
        /// </summary>
        public virtual void Initialize() { }

        public virtual void Show(params object[] parameters) { }

        public virtual void Show() { }
    }
}