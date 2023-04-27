using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NorskaLib.UI.Widgets
{
    public sealed partial class WidgetAnimator
    {
        [System.Serializable]
        public sealed class AnchorModule : Module
        {
            [EnableIf("@enabled")]
            public RectTransform targetTransform;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("In")]
            public Vector2 anchorIn;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("Out")]
            public Vector2 anchorOut;

            private Vector2 GetTargetValue(bool @in)
            {
                return @in ? anchorIn : anchorOut;
            }

            public override void Set(bool @in)
            {
                targetTransform.anchoredPosition = GetTargetValue(@in);
            }

            public override Tween CreateTween(bool @in, float duration)
            {
                return targetTransform.DOAnchorPos(GetTargetValue(@in), duration);
            }
        }
    }
}