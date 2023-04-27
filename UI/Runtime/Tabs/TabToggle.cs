using System;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI.Tabs
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class TabToggle : MonoBehaviour
    {
        [SerializeField] 
        private Toggle _toggle;

        [SerializeField] 
        private GameObject _groupOn;
        [SerializeField] 
        private GameObject _groupOff;

        public event Action<TabToggle, bool> onValueChanged;

        public bool IsOn => _toggle.isOn;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool value)
        {
            _groupOn.gameObject.SetActive(value);
            _groupOff.gameObject.SetActive(!value);

            onValueChanged?.Invoke(this, value);
        }

        public void SetValue(bool value)
        {
            _toggle.isOn = value;
        }

        public void SetValueNoCallback(bool value)
        {
            _toggle.SetIsOnWithoutNotify(value);

            _groupOn.gameObject.SetActive(value);
            _groupOff.gameObject.SetActive(!value);
        }
    }
}
