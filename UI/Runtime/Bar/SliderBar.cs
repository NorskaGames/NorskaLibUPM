using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public class SliderBar : Bar
    {
        [SerializeField] protected Image backgroundImage;
        public Image BackgroundImage => backgroundImage;
        [Tooltip("Optional \"second\" fill image.")]
        [SerializeField] protected Image tintImage;
        public Image TintImage => tintImage;
        [SerializeField] protected Image foregroundImage;
        public Image ForegroundImage => foregroundImage;

        [Tooltip("Optional \"second\" fill image.")]
        [SerializeField] Slider tintSlider;
        [SerializeField] Slider foregroundSlider;

        [Space]

        public Ease increaseEase = Ease.InFlash;
        public Ease decreaseEase = Ease.OutFlash;

        private Sequence animationSequence;
        private float lastFill;

        public override void FillImmediate(float fill)
        {
            animationSequence?.Kill(true);
            lastFill = fill;

            if (tintSlider != null)
                tintSlider.value = fill;

            foregroundSlider.value = fill;
        }

        public override void FillAnimated(float fill, float duration = 0.3f, float delay = 0)
        {
            animationSequence?.Kill(true);

            Sequence CreateSequence(Slider slider, bool decrease)
            {
                var ease = decrease ? decreaseEase : increaseEase;
                var sequence = DOTween.Sequence();
                if (delay > 0)
                    sequence.AppendInterval(delay);
                sequence
                    .Append(DOTween.To(() => slider.value, s => slider.value = s, fill, duration))
                    .SetEase(ease);

                return sequence;
            }

            var decrease = lastFill > fill;
            var animatedSlider = tintSlider != null && decrease
                ? tintSlider
                : foregroundSlider;

            if (tintSlider != null)
            {
                var immediateSlider = decrease
                    ? foregroundSlider
                    : tintSlider;
                immediateSlider.value = fill;
            }

            animationSequence = CreateSequence(animatedSlider, decrease).Play();
            lastFill = fill;
        }
    }
}
