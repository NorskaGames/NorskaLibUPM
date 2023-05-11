using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public class ImageBar : Bar
    {
        [SerializeField] protected Image backgroundImage;
        public Image BackgroundImage => backgroundImage;
        [Tooltip("Optional \"second\" fill image.")]
        [SerializeField] protected Image tintImage;
        public Image TintImage => tintImage;
        [SerializeField] protected Image foregroundImage;
        public Image ForegroundImage => foregroundImage;

        [Space]

        public Ease increaseEase = Ease.InFlash;
        public Ease decreaseEase = Ease.OutFlash;

        private Sequence animationSequence;
        private float lastFill;

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            animationSequence?.Kill(true);
        }

        #endregion

        public override void FillImmediate(float fill)
        {
            animationSequence?.Kill(true);

            foregroundImage.fillAmount = fill;
            lastFill = fill;

            if (tintImage != null)
                tintImage.fillAmount = fill;
        }

        public override void FillAnimated(float fill, float duration = 0.3f, float delay = 0.0f)
        {
            Sequence CreateSequence(Image image, bool decrease)
            {
                var ease = decrease ? decreaseEase : increaseEase;
                var sequence = DOTween.Sequence();
                if (delay > 0)
                    sequence.AppendInterval(delay);
                sequence.Append(image
                    .DOFillAmount(fill, duration))
                    .SetEase(ease);

                return sequence;
            }

            animationSequence?.Kill(true);

            var decrease = lastFill > fill;
            var animatedImage = tintImage != null && decrease
                ? tintImage
                : foregroundImage;

            if (tintImage != null)
            {
                var immediateImage = decrease
                    ? foregroundImage 
                    : tintImage;
                immediateImage.fillAmount = fill;
            }

            animationSequence = CreateSequence(animatedImage, decrease).Play();
            lastFill = fill;
        }
    }
}
