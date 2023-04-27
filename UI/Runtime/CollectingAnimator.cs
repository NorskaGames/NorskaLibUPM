using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NorskaLib.Utilities; 

namespace NorskaLib.UI
{
    using Random = UnityEngine.Random;

    public class CollectingAnimator : MonoBehaviour
    {
        public Canvas parentCanvas;

        [Tooltip("Toggle this to automatically detect parent Canvas.")]
        [SerializeField] bool autoDetectCanvas;

        #region MonoBehaviour

        void Start()
        {
            if (autoDetectCanvas)
                parentCanvas = GetComponentInParent<Canvas>();
        }

        #endregion

        #region Callbacks

        public Action<RectTransform> onFinishSequence;
        void OnHandlerFinish(SequenceHandler handler)
        {
            handler.onFinish -= OnHandlerFinish;

            onFinishSequence?.Invoke(handler.item);
        }

        #endregion

        public void Animate(RectTransform instance, RectTransform target, float duration = 1f, float transitScale = 1.2f)
        {
            var clone = Instantiate(instance, instance.parent);
            clone.SetParent(target);

            new SequenceHandler(clone, CreateSequences(clone, duration, transitScale));
        }

        public void Animate(RectTransform prefab, Vector2 screenPos, RectTransform target, float duration = 1f, float transitScale = 1.2f)
        {
            var clone = Instantiate(prefab, target);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target, 
                screenPos,
                parentCanvas?.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : parentCanvas.worldCamera,
                out var anchoredPos);

            clone.anchoredPosition = anchoredPos;

            var handler = new SequenceHandler(clone, CreateSequences(clone, duration, transitScale));
            handler.onFinish += OnHandlerFinish;
        }

        private IEnumerable<Sequence> CreateSequences(RectTransform clone, float duration, float transitScale)
        {
            var W = UnityEngine.Screen.width;

            #region Position

            var posSequence = DOTween.Sequence();

            // Moving to transit position
            var radius = Random.Range(0.1f * W, 0.2f * W);
            var degrees = Random.Range(-180, 180);
            var transitPosition = MathUtils.PositionOnCircle2D(clone.anchoredPosition, degrees, radius);

            posSequence.Append(
                DOTween.To(
                    () => clone.anchoredPosition,
                    p => clone.anchoredPosition = p,
                    transitPosition,
                    0.5f * duration)
                .SetEase(Ease.OutExpo));

            // Moving to target
            posSequence.Append(
                DOTween.To(
                    () => clone.anchoredPosition,
                    p => clone.anchoredPosition = p,
                    Vector2.zero,
                    0.5f * duration)
                .SetEase(Ease.OutExpo));

            posSequence.Play();

            #endregion

            #region Scale

            var scaleSequence = DOTween.Sequence();

            scaleSequence.Append(
                DOTween.To(
                    () => clone.localScale,
                    s => clone.localScale = s,
                    new Vector3(transitScale, transitScale, transitScale),
                    0.5f * duration)
                .SetEase(Ease.OutExpo));

            scaleSequence.Append(
                DOTween.To(
                    () => clone.localScale,
                    s => clone.localScale = s,
                    Vector3.one,
                    0.5f * duration)
                .SetEase(Ease.OutExpo));

            scaleSequence.Play();

            #endregion

            return new Sequence[]
            {
                posSequence,
                scaleSequence
            };
        }

        private class SequenceHandler
        {
            public readonly RectTransform item;
            private IEnumerable<Sequence> sequences;

            private bool isExecuted;

            public SequenceHandler(RectTransform item, IEnumerable<Sequence> sequences)
            {
                this.item = item;
                this.sequences = sequences;

                foreach (var s in sequences)
                    s.OnComplete(OnSequenceFinish);
            }

            public event Action<SequenceHandler> onFinish;
            void OnSequenceFinish()
            {
                if (isExecuted || sequences.Any(s => s.IsPlaying()))
                    return;

                onFinish?.Invoke(this);

                Destroy(item.gameObject);
                sequences = null;
                isExecuted = true;
            }
        }
    }
}