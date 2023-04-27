using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public class Bar : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image background;
        [SerializeField] Image tint;
        [SerializeField] Image foreground;
        private Sequence fillTween;

        private RectTransform rect;
        public RectTransform RectTransform => rect;

        /// <summary>
        /// Override this list to deinitialize tweens properly.
        /// </summary>
        protected virtual IEnumerable<Tween> tweens
            => new Tween[]
            {
                    fillTween,
            };

        #region MonoBehaviour

        protected virtual void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        protected virtual void OnDestroy()
        {
            foreach (var tween in tweens)
                tween?.Kill();
        }

        #endregion

        #region Callbacks

        public event Action<Bar> onClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this);
        }

        #endregion

        public void SetForegroundColor(Color color)
        {
            foreground.color = color;
        }
        public void SetTintColor(Color color)
        {
            tint.color = color;
        }
        public void SetBackgroundColor(Color color)
        {
            background.color = color;
        }

        private float fill;
        public virtual void FillImmediate(float fill)
        {
            this.fill = fill;
            fillTween?.Kill(true);

            foreground.fillAmount = fill;
            tint.fillAmount = fill;
        }
        public virtual void FillAnimated(float fill, float duration = 0.3f, float delay = 0.0f)
        {
            fillTween?.Kill(true);

            Sequence CreateSequence(Image targetGraphic)
            {
                var sequence = DOTween.Sequence();

                if (delay > 0)
                    sequence.Append(targetGraphic.DOFillAmount(this.fill, delay));

                sequence.Append(targetGraphic.DOFillAmount(fill, duration)).SetEase(Ease.Flash);

                return sequence;
            }

            // i. g. animating decrease 
            if (this.fill > fill)
            {
                foreground.fillAmount = fill;
                fillTween = CreateSequence(tint).Play();
            }
            // i. g. animating increase 
            else
            {
                tint.fillAmount = fill;
                fillTween = CreateSequence(foreground).Play();
            }

            this.fill = fill;
        }
    }
}
