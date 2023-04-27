using DG.Tweening;
using UnityEngine;

namespace NorskaLib.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class SimpleWindowAnimator : WindowAnimator
    {
        [SerializeField] CanvasGroup canvasGroup;

        [Space]

        [SerializeField] float fadeInDuration = 0.6f;
        [SerializeField] Ease fadeInEase = Ease.InFlash;

        [SerializeField] float fadeOutDuration = 0.3f;
        [SerializeField] Ease fadeOutEase = Ease.OutFlash;

        private Tween tween;

        public override bool IsAnimatingShow => tween?.IsPlaying() ?? false;
        public override bool IsAnimatingHide => tween?.IsPlaying() ?? false;

        public override void AnimateShow()
        {
            tween?.Kill();
            tween = canvasGroup
                .DOFade(1, fadeInDuration)
                .SetEase(fadeInEase)
                .OnComplete(OnFinishedShow)
                .Play();
        }
        private void OnFinishedShow()
            => onShowAnimationFinished?.Invoke();

        public override void AnimateHide()
        {
            tween?.Kill();
            tween = canvasGroup
                .DOFade(0, fadeOutDuration)
                .SetEase(fadeInEase)
                .OnComplete(OnFinishedHide)
                .Play();
        }
        private void OnFinishedHide()
            => onHideAnimationFinished?.Invoke();
    }
}