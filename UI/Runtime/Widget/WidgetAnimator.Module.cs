using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace NorskaLib.UI.Widgets
{
    public sealed partial class WidgetAnimator
    {
        [System.Serializable]
        public abstract class Module
        {
            public bool enabled;

            public abstract void Set(bool @in);

            public abstract Tween CreateTween(bool @in, float duration);
        }
    }
}