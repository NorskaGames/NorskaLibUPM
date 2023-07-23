using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI
{
    public class ImageDiscreteBarSegment : DiscreteBarSegment
    {
        [SerializeField] protected Image image;

        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [SerializeField] protected Sprite spriteFilled;
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [SerializeField] protected Sprite spriteEmpty;

        public float animationScale = 1.15f;
        private Tween animationTween;
        private Tween callbackTween;
        private IEnumerable<Tween> Tweens
        {
            get
            {
                yield return animationTween;
                yield return callbackTween;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var tween in Tweens)
                tween?.Kill();
        }

        public override void SetAnimated(bool filled, float duration = 0.15F)
        {
            foreach (var tween in Tweens)
                tween?.Kill();

            animationTween = transform
                .DOScale(animationScale, duration)
                .SetEase(Ease.InOutBounce)
                .Play();

            callbackTween = filled
                ? DOVirtual.DelayedCall(duration * 0.5f, SetFilled)
                : DOVirtual.DelayedCall(duration * 0.5f, SetEmpty);
            callbackTween.Play();
        }

        public override void SetImmediate(bool filled)
        {
            foreach (var tween in Tweens)
                tween?.Kill();

            if (filled)
                SetFilled();
            else 
                SetEmpty();
        }

        void SetFilled() => image.sprite = spriteFilled;
        void SetEmpty() => image.sprite = spriteEmpty;
    }
}