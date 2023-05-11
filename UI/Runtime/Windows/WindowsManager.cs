using DG.Tweening;
using NorskaLib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public abstract class WindowsManager : MonoBehaviour
    {
        [SerializeField] Canvas canvas;

        #region Scale properties

        [SerializeField] CanvasScaler canvasScaler;

        // TO DO: 
        //[DisableInPlayMode][Tooltip("May be useful in mobile games, to specify ")]
        //[SerializeField] Vector2 pointerFingerOffset = new Vector2(22, 22);
        //public Vector2 PointerFingerOffset => pointerFingerOffset * CurrentScale;

        public Vector2 CurrentScreenResolution
            => new Vector2(Screen.width, Screen.height);

        public float ReferenceScreenAspect
            => canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;

        public float CurrentScreenAspect
            => CurrentScreenResolution.x / CurrentScreenResolution.y;

        /// <summary>
        /// The factor of current screen resolution to reference one.
        /// </summary>
        public Vector2 CurrentScale
            => CurrentScreenResolution / canvasScaler.referenceResolution;

        #endregion

        #region Windows properties

        private string WindowsParentName = "Windows";

        private RectTransform windowsContainer;
        private Dictionary<Type, Window> windows;

        /// <summary>
        /// Called ones per window lifetime, AFTER it has been instantiated.
        /// </summary>
        public Action<Window> onWindowCreated;
        /// <summary>
        /// Invoked each time during ShowWindow() method call, AFTER window becomes active 
        /// (and BEFORE animation starts, if the window has a WindowAnimator component).
        /// </summary>
        public Action<Window> onWindowShows;
        /// <summary>
        /// Invoked each time during HideWindow() method call, BEFORE window becomes inactive 
        /// (and BEFORE animation starts, if the window has a WindowAnimator component).
        /// </summary>
        public Action<Window> onWindowHides;
        /// <summary>
        /// Called ones per window lifetime, BEFORE instance destroyed.
        /// </summary>
        public Action<Window> onWindowDestroyed;

        #endregion

        protected virtual void Awake()
        {
            var obj = new GameObject(WindowsParentName, typeof(RectTransform));
            windowsContainer = obj.GetComponent<RectTransform>();
            windowsContainer.SetParent(this.transform);
            windowsContainer.anchorMin = Vector2.zero;
            windowsContainer.anchorMax = Vector2.one;
            windowsContainer.pivot = new Vector2(0.5f, 0.5f);
            windowsContainer.anchoredPosition = Vector2.zero;
            windowsContainer.offsetMin = Vector2.zero;
            windowsContainer.offsetMax = Vector2.zero;
            windowsContainer.localScale = Vector3.one;
            windowsContainer.localPosition = Vector3.zero;
            windowsContainer.localRotation = Quaternion.identity;

            windows = new();
        }

        protected virtual void Update()
        {
            if (!wantUpdateOrder)
                return;

            UpdateWindowsOrder();
            wantUpdateOrder = false;
        }

        #region Windows 

        protected abstract W GetWindowPrefab<W>() where W : Window;

        private bool wantUpdateOrder;
        void OnWindowOrderChanged(int order)
        {
            wantUpdateOrder |= true;
        }

        public void UpdateWindowsOrder()
        {
            var windowsSorted = windows.Values.OrderBy(s => s.Order.Value);

            var index = 0;
            foreach (var window in windowsSorted)
            {
                window.transform.SetSiblingIndex(index);
                index++;
            }
        }

        public bool TryGetWindow<W>(out W window) where W : Window
        {
            var type = typeof(W);
            var check = windows.TryGetValue(type, out var result);
            window = result as W;
            return check;
        }
        public W GetWindow<W>() where W : Window
        {
            var type = typeof(W);

            if (windows.ContainsKey(type))
                return windows[type] as W;
            else
                return null;
        }

        public W ShowWindow<W>(ShowWindowMode mode = ShowWindowMode.Additive, int order = -1, bool animated = false) where W : Window
        {
            var type = typeof(W);

            if (!windows.TryGetValue(type, out var window) || window == null)
            {
                var prefab = GetWindowPrefab<W>();
                window = Instantiate(prefab, windowsContainer);

                #region Editor
#if UNITY_EDITOR
                window.name = $"{type.Name}";
#endif
                #endregion

                window.Order.onChanged += OnWindowOrderChanged;
                windows.Add(type, window);

                onWindowCreated?.Invoke(window);
            }

            switch (mode)
            {
                case ShowWindowMode.Single:
                    foreach (var pair in windows)
                        if (pair.Value != window)
                            HideWindowInternal(pair.Value, animated, false);
                    break;

                case ShowWindowMode.SoloInLayer:
                    foreach (var pair in windows)
                        if (pair.Value != window && pair.Value.Order.Value == order)
                            HideWindowInternal(pair.Value, animated, false);
                    break;

                default:
                case ShowWindowMode.Additive:
                    break;
            }

            return ShowWindowInternal(window, order, animated) as W;
        }
        public Window ShowWindow(Window window, int order = -1, bool animated = false)
        {
            if (!windows.ContainsValue(window) || window.gameObject.activeSelf)
                return default;

            return ShowWindowInternal(window, order, animated);
        }
        private Window ShowWindowInternal(Window window, int order, bool animated)
        {
            window.Order.Value = order == -1
                ? window.DefaultOrder
                : order;

            window.gameObject.SetActive(true);
            onWindowShows?.Invoke(window);

            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
                animator.AnimateShow();

            return window;
        }

        public void HideWindow<W>(bool animated = false, bool destroy = false) where W : Window
        {
            if (!windows.TryGetValue(typeof(W), out var window))
                return;

            HideWindowInternal(window, animated, destroy);
        }
        public void HideWindow(Window window, bool animated = false, bool destroy = false)
        {
            HideWindowInternal(window, animated, destroy);
        }
        private void HideWindowInternal(Window window, bool animated, bool destroy)
        {
            IEnumerator Routine(Window window, WindowAnimator animator, bool destroy)
            {
                animator.AnimateHide();
                while (animator.IsAnimatingHide)
                    yield return null;

                Finish(window, destroy);
            }

            void Finish(Window window, bool destroy)
            {
                if (destroy)
                {
                    window.Order.onChanged -= OnWindowOrderChanged;
                    windows.Remove(window.GetType());

                    onWindowHides?.Invoke(window);
                    onWindowDestroyed?.Invoke(window);
                    Destroy(window.gameObject);
                }
                else
                {
                    onWindowHides?.Invoke(window);
                    window.gameObject.SetActive(false);
                }
            }

            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
                StartCoroutine(Routine(window, animator, destroy));
            else
                Finish(window, destroy);
        }

        #endregion
    }
}