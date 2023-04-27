using DG.Tweening;
using NorskaLib.Extensions;
using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TO DO:
// Switch to custom editor to allow add/remove modules

namespace NorskaLib.UI.Widgets
{
	public sealed partial class WidgetAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public float delay;
		public float duration = 0.3f;
		public Ease ease = Ease.Linear;

		[Space]

		[FoldoutGroup("Auto-Animate"), LabelText("On Enable")]
		public bool animateOnEnable;
		[FoldoutGroup("Auto-Animate"), EnableIf("@animateOnEnable")]
		public bool oneShot;
		private bool oneShotDone;

		[Space]

		[FoldoutGroup("Auto-Animate"), LabelText("Pointer Press")]
		public bool animatePress;
		[FoldoutGroup("Auto-Animate"), LabelText("Preset"), EnableIf("@animatePress")]
		public bool presetPress;

		[BoxGroup("Modules"), LabelText("Scale")]
		[SerializeField] ScaleModule scaleModule;
		[BoxGroup("Modules"), LabelText("Anchored Position")]
		[SerializeField] AnchorModule anchorModule;
		[BoxGroup("Modules"), LabelText("Fade")]
		[SerializeField] FadeModule fadeModule;

		private Module[] modules;
		private Module[] Modules
        {
			get
            {
				if (modules is null)
					modules = new Module[]
					{
						scaleModule,
						anchorModule,
						fadeModule
					};

				return modules;
			}
        }
		private List<Tween> tweens = new();

		public Action onAnimationFinished;

        void OnEnable()
        {
			if (animateOnEnable && (!oneShot || !oneShotDone))
            {
				oneShotDone = true;
				AnimateIn(true);
            }

			if (animatePress && presetPress)
				SetIn();
		}

		void OnDisable()
		{
			ClearTweens();
		}

		public void SetIn()
		{
			Set(true);
		}
		public void SetOut()
		{
			Set(false);
		}
		private void Set(bool @in)
		{
			ClearTweens();

			foreach (var module in Modules)
            {
				if (!module.enabled)
					continue;

				module.Set(@in);
            }
		}

		public void AnimateLoop(bool fromIn, bool reset = false)
		{
			ClearTweens();

			CreateTweens(ref tweens, fromIn, reset, true);

			if (tweens.Count <= 0)
				return;

			foreach (var tween in tweens)
				tween.Play();
		}

		public void AnimateIn(bool reset = false)
		{
			Animate(true, reset);
		}
		public void AnimateOut(bool reset = false)
		{
			Animate(false, reset);
		}
		private void Animate(bool @in, bool reset)
		{
			ClearTweens();
			CreateTweens(ref tweens, @in, reset, false);

			if (tweens.Count <= 0)
				return;

			var callbackTween = DOTween.Sequence()
				.AppendInterval(delay + duration)
				.AppendCallback(() => onAnimationFinished?.Invoke());
			tweens.Add(callbackTween);

			foreach (var tween in tweens)
				tween.Play();
		}

		private void CreateTweens(ref List<Tween> collection, bool @in, bool reset, bool loop)
		{
			foreach (var module in Modules)
			{
				if (!module.enabled)
					continue;

				if (reset)
					module.Set(!@in);
				collection.Add(module.CreateTween(@in, duration));
			}

			foreach (var tween in collection)
			{
				tween.SetEase(ease);

				if (loop)
					tween.SetLoops(-1, LoopType.Yoyo);

				if (delay > 0)
					tween.SetDelay(delay);
			}
		}

		private void ClearTweens()
		{
			foreach (var tween in tweens)
				tween?.Kill(complete: true);

			tweens.Clear();
		}

		#region Pointer callbacks

		public void OnPointerDown(PointerEventData eventData)
        {
			if (!animatePress)
				return;

			AnimateOut();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
			if (!animatePress)
				return;

			AnimateIn();
		}

        #endregion
    }
}