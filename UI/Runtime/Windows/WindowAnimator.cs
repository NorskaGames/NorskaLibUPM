using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI
{
    public abstract class WindowAnimator : MonoBehaviour
    {
        public abstract bool IsAnimatingShow { get; }
        public Action onShowAnimationFinished;
        public abstract void AnimateShow();

        public Action onHideAnimationFinished;
        public abstract bool IsAnimatingHide { get; }
        public abstract void AnimateHide();
    }
}