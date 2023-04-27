using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NorskaLib.UI.Widgets
{
    [RequireComponent(typeof(RectTransform))]
	public abstract class Widget : MonoBehaviour
	{
        /// <summary>
        /// Params: Widget - instance, string - widget id
        /// </summary>
        public static event Action<Widget, string> onClicked;

        public abstract string Id { get;  }

        public RectTransform RectTransform { get; private set; }
        public WidgetEvents Events { get; private set; }

        #region MonoBehaviour

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Events = GetComponent<WidgetEvents>();

            WidgetProvider.Instance.RegisterInstance(this);
        }

        protected virtual void OnEnable()
        {
            WidgetProvider.Instance.OnWidgetEnabled(this);
            if (Events != null)
            {
                Events.onClick += OnClick;
            }
        }

        protected virtual void OnDisable()
        {
            WidgetProvider.Instance.OnWidgetDisabled(this);
            if (Events != null)
            {
                Events.onClick -= OnClick;      
            }
        }

        protected virtual void OnDestroy()
        {
            WidgetProvider.Instance.UnregisterInstance(this);
        }

        #endregion

        void OnClick(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(Id))
                return;

            onClicked?.Invoke(this, Id);
        }
    }
}