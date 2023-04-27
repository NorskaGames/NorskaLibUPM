using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI.Tabs
{
    using Toggle = TabToggle;

    [RequireComponent(typeof(UnityEngine.UI.ToggleGroup))]
    public class TabGroup : MonoBehaviour
    {
        [SerializeField]
        private Tab[] _tabs;

        [SerializeField]
        private bool _switchDefaultOnEnable;

        private object[] _showParamsBuffer;

        #region MonoBehaviour

        private void Awake()
        {
            Cache();

            foreach (var tab in _tabs)
                tab.Content.Initialize();
        }

        private void Start()
        {
            TrySetDefaultState();
        }

        private void OnEnable()
        {
            if (_switchDefaultOnEnable)
                TrySetDefaultState();
        }

        #endregion

        private void Cache()
        {
            foreach (var pair in _tabs)
                pair.Toggle.onValueChanged += OnToggleValueChanged;
        }

        private void OnToggleValueChanged(Toggle toggle, bool isOn)
        {
            var target = _tabs.First(p => p.Toggle == toggle);

            if (isOn)
            {
                target.Content.gameObject.SetActive(true);

                if (_showParamsBuffer is null || _showParamsBuffer.Length == 0)
                    target.Content.Show();
                else
                {
                    target.Content.Show(_showParamsBuffer);
                    _showParamsBuffer = null;
                }
            }
            else
            {
                target.Content.gameObject.SetActive(false);
            }
        }

        public void SwitchTab<T>(params object[] parameters) where T : TabContent
        {
            if (parameters is null || parameters.Length == 0)
            {
                Debug.LogError("Parameters array shouldn't be null or empty!");
                return;
            }

            var target = _tabs.FirstOrDefault(t => t.Content is T);
            if (target == null)
            {
                Debug.LogError($"There is no tab content of type '{typeof(T).Name}'");
                return;
            }

            if (!target.Toggle.IsOn)
            {
                _showParamsBuffer = parameters;
                target.Toggle.SetValue(true);
            }
            else
            {
                target.Content.Show(parameters);
                _showParamsBuffer = null;
            }
        }

        public void SwitchTab<T>() where T : TabContent
        {
            var target = _tabs.FirstOrDefault(t => t.Content is T);
            if (target == null)
            {
                Debug.LogError($"There is no tab content of type '{typeof(T).Name}'");
                return;
            }

            if (!target.Toggle.IsOn)
            {
                target.Toggle.SetValue(true);
            }
            else
            {
                target.Content.Show();
                _showParamsBuffer = null;
            }
        }

        private void TrySetDefaultState()
        {
            foreach (var tab in _tabs)
            {
                if (tab.IsDefault)
                {
                    if (!tab.Toggle.IsOn)
                        tab.Toggle.SetValueNoCallback(true);

                    if (!tab.Content.gameObject.activeInHierarchy)
                        tab.Content.gameObject.SetActive(true);

                    tab.Content.Show();
                }
                else
                {
                    if (tab.Toggle.IsOn)
                        tab.Toggle.SetValueNoCallback(false);

                    if (tab.Content.gameObject.activeSelf)
                        tab.Content.gameObject.SetActive(false);
                }
            }
        }

        #region Editor only
#if UNITY_EDITOR

        private void OnValidate()
        {
            if (TryGetComponent<ToggleGroup>(out var toggleGroup))
                toggleGroup.allowSwitchOff = false;
        }

#endif
    #endregion
    }
}