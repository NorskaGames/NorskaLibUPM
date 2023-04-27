using UnityEngine;

namespace NorskaLib.UI.Tabs
{
    using Toggle = TabToggle;

    [System.Serializable]
    public class Tab
    {
        [SerializeField] private Toggle _toggle;
        public Toggle Toggle => _toggle;

        [SerializeField] private TabContent _content;
        public TabContent Content => _content;

        [SerializeField] private bool _isDefault;
        public bool IsDefault => _isDefault;
    }
}