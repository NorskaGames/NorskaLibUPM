using DG.Tweening;
using NorskaLib.Extensions;
using Sirenix.OdinInspector;
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
        private struct KeyWords
        {
            public const string Windows     = "Windows";

            public const string MaskFull    = "MaskFull";
            public const string MaskScene   = "MaskScene";
        }

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

        #region Masks properties

        /// <summary>
        /// Masks are images that covers whole screen.
        /// </summary>
        public enum MaskType
        {
            Full,
            Scene
        }

        private Dictionary<MaskType, Image> maskImages;
        private Dictionary<MaskType, Tween> maskTweens;

        #endregion

        #region Windows properties

        private RectTransform windowsContainer;
        private Dictionary<System.Type, Window> windows;

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
            #region Common initialization

            var children = new Dictionary<string, RectTransform>()
            {
                { KeyWords.MaskScene,   null },
                { KeyWords.Windows,     null },
                { KeyWords.MaskFull,    null }
            };
            for (int i = 0; i < children.Count; i++)
            {
                var name = children.ElementAt(i).Key;

                var obj = new GameObject(name, typeof(RectTransform));
                var rect = obj.GetComponent<RectTransform>();
                rect.SetParent(this.transform);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;

                children[name] = rect;
            }

            #endregion

            #region Masks initialization

            maskImages = new ()
            {
                { MaskType.Full,    children[KeyWords.MaskFull].gameObject.AddComponent<Image>() },
                { MaskType.Scene,   children[KeyWords.MaskScene].gameObject.AddComponent<Image>() }
            };
            maskTweens = new ()
            {
                { MaskType.Full,    null },
                { MaskType.Scene,   null },
            };
            foreach (var pair in maskImages)
            {
                SetMaskAlpha(pair.Key, 0);
                SetMaskSprite(pair.Key, null);
                pair.Value.raycastTarget = false;
            }

            #endregion

            #region Windows initialization

            windowsContainer    = children[KeyWords.Windows];
            windows             = new();

            #endregion
        }

        protected virtual void Update()
        {
            if (wantUpdateOrder)
            {
                UpdateWindowsOrder();
                wantUpdateOrder = false;
            }
        }

        protected virtual void OnDestroy()
        {
            // Masks deinitialization
            if (maskTweens != null)
                foreach (var pair in maskTweens)
                    pair.Value?.Kill();
        }

        #region Windows 

        protected abstract W GetWindowPrefab<W>() where W : Window;

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
                    foreach (var s in windows)
                        if (s.Value != window)
                            HideWindow(s.Value);
                    break;

                case ShowWindowMode.SoloInLayer:
                    foreach (var s in windows)
                        if (s.Value != window && s.Value.Order.Value == order)
                            HideWindow(s.Value);
                    break;

                default:
                case ShowWindowMode.Additive:
                    break;
            }

            window.Order.Value = order == -1
                ? window.DefaultOrder
                : order;

            window.gameObject.SetActive(true);
            onWindowShows?.Invoke(window);

            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
                animator.AnimateShow();

            return window as W;
        }
        public void ShowWindow(Window window, bool animated = false)
        {
            if (!windows.ContainsValue(window) || window.gameObject.activeSelf)
                return;

            window.gameObject.SetActive(true);
            onWindowShows?.Invoke(window);

            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
                animator.AnimateShow();
        }

        public void HideWindow<W>(bool animated = false, bool destroy = false) where W : Window
        {
            if (!windows.TryGetValue(typeof(W), out var window) || !window.gameObject.activeSelf)
                return;

            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
            {
                animator.AnimateHide();
                if (destroy)
                    StartCoroutine(DestroyWindowRoutine(window, animator));
                else
                    StartCoroutine(HideWindowRoutine(window, animator));
            }
            else
            {
                if (destroy)
                    DestroyWindow(window);
                else
                    HideWindow(window);
            }
        }
        public void HideWindow(Window window, bool animated = false, bool destroy = false)
        {
            if (animated && window.TryGetComponent<WindowAnimator>(out var animator))
            {
                animator.AnimateHide();
                if (destroy)
                    StartCoroutine(DestroyWindowRoutine(window, animator));
                else
                    StartCoroutine(HideWindowRoutine(window, animator));
            }
            else
            {
                if (destroy)
                    DestroyWindow(window);
                else
                    HideWindow(window);
            }
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
        
        private IEnumerator HideWindowRoutine(Window window, WindowAnimator animator)
        {
            while (animator.IsAnimatingHide)
                yield return null;

            HideWindow(window);
        }
        private void HideWindow(Window window)
        {
            onWindowHides?.Invoke(window);
            window.gameObject.SetActive(false);
        }

        private IEnumerator DestroyWindowRoutine(Window window, WindowAnimator animator)
        {
            while(animator.IsAnimatingHide)
                yield return null;

            DestroyWindow(window);
        }
        private void DestroyWindow(Window window)
        {
            var type = window.GetType();

            window.Order.onChanged -= OnWindowOrderChanged;
            windows.Remove(type);

            onWindowHides?.Invoke(window);
            onWindowDestroyed?.Invoke(window);
            Destroy(window.gameObject);
        }

        private bool wantUpdateOrder;
        void OnWindowOrderChanged(int order)
        {
            wantUpdateOrder |= true;
        }

        #endregion

        #region Masks

        public void SetMaskSprite(MaskType maskType, Sprite sprite)
        {
            maskImages[maskType].sprite = sprite;
        }

        public void SetMaskAlpha(MaskType maskType, float alpha, float duration = 0)
        {
            SetMaskColor(maskType, maskImages[maskType].color.WithA(alpha), duration);
        }

        public void SetMaskColor(MaskType maskType, Color color, float duration = 0)
        {
            if (duration > 0)
            {
                maskTweens[maskType]?.Kill();
                maskTweens[maskType] = maskImages[maskType]
                    .DOColor(color, duration)
                    .Play();
            }
            else
            {
                maskImages[maskType].color = color;
            }
        }

        #endregion
    }
}