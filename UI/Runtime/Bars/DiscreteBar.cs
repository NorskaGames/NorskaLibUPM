using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI
{
	public abstract class DiscreteBar : Bar
	{
		public abstract List<DiscreteBarSegment> Segements { get; }

		[ShowInInspector, ReadOnly]
		protected int lastFilledCount = 0;

        private Coroutine animationRoutine;

        protected virtual void OnDisable()
        {
            if (animationRoutine == null)
                return;

            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        public override void FillImmediate(float fill)
        {
            var segmentsCount = Mathf.CeilToInt(fill * Segements.Count);
            FillImmediate(segmentsCount);
        }

        public void FillImmediate(int segmentsCount)
        {
            if (segmentsCount > lastFilledCount)
                for (int i = lastFilledCount; i < segmentsCount; i++)
                    Segements[i].SetImmediate(true);
            else if (segmentsCount < lastFilledCount)
                for (int i = lastFilledCount - 1; i > segmentsCount; i--)
                    Segements[i].SetImmediate(false);

            lastFilledCount = segmentsCount;
        }

        public override void FillAnimated(float fill, float duration = 0.3f, float delay = 0)
        {
            var segmentsCount = Mathf.CeilToInt(fill * Segements.Count);
            FillAnimated(segmentsCount, duration, delay);
        }

        public void FillAnimated(int segmentsCount, float duration = 0.3f, float delay = 0)
        {
            if (animationRoutine != null)
                StopCoroutine(animationRoutine);

            animationRoutine = StartCoroutine(AnimationRoutine(segmentsCount, duration, delay));
            lastFilledCount = segmentsCount;
        }

        // TO DO:
        // delay
        private IEnumerator AnimationRoutine(int segmentsCount, float duration = 0.3f, float delay = 0)
        {
            var segmentAnimationTime = duration / segmentsCount;
            var timer = 0.0f;
            if (segmentsCount > lastFilledCount)
                for (int i = lastFilledCount; i < segmentsCount; i++)
                {
                    Segements[i].SetImmediate(true);

                    timer += Time.deltaTime;
                    if (timer <= segmentAnimationTime)
                        yield return null;

                    timer -= segmentAnimationTime;
                }
            else if (segmentsCount < lastFilledCount)
                for (int i = lastFilledCount - 1; i > segmentsCount; i--)
                {
                    Segements[i].SetImmediate(false);

                    timer += Time.deltaTime;
                    if (timer <= segmentAnimationTime)
                        yield return null;

                    timer -= segmentAnimationTime;
                }

            animationRoutine = null;
        }
    }
}