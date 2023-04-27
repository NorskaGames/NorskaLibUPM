using DG.Tweening;
using NorskaLib.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace NorskaLib.UI.Widgets
{
    public sealed partial class WidgetAnimator
    {
        [System.Serializable]
        public sealed class FadeModule : Module
        {
            public enum TargetType
            {
                CanvasGroup,
                Graphic
            }

            [EnableIf("@enabled")]
            public TargetType targetType = TargetType.CanvasGroup;

            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.Graphic")]
            public Graphic targetGraphic;
            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.Graphic")]
            [BoxGroup("Color"), LabelText("In")]
            public Color graphicColorIn = Color.white;
            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.Graphic")]
            [BoxGroup("Color"), LabelText("Out")]
            public Color graphicColorOut = Color.white.WithA(0);

            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.CanvasGroup")]
            public CanvasGroup targetCanvasGroup;
            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.CanvasGroup")]
            [BoxGroup("Alpha"), LabelText("In")]
            public float canvasAlphaIn = 1;
            [EnableIf("@enabled"), ShowIf("@targetType == TargetType.CanvasGroup")]
            [BoxGroup("Alpha"), LabelText("Out")]
            public float canvasAlphaOut = 0;

            private float GetCanvasTragetAlpha(bool @in)
            {
                return @in ? canvasAlphaIn : canvasAlphaOut;
            }
            private Color GetGraphicTargetColor(bool @in)
            {
                return @in ? graphicColorIn : graphicColorOut;
            }

            public override void Set(bool @in)
            {
                switch (targetType)
                {
                    default:
                    case TargetType.CanvasGroup:
                        targetCanvasGroup.alpha = GetCanvasTragetAlpha(@in);
                        break;
                    case TargetType.Graphic:
                        targetGraphic.color = GetGraphicTargetColor(@in);
                        break;
                }
            }

            public override Tween CreateTween(bool @in, float duration)
            {
                switch (targetType)
                {
                    default:
                    case TargetType.CanvasGroup:
                        return targetCanvasGroup.DOFade(GetCanvasTragetAlpha(@in), duration);
                    case TargetType.Graphic:
                        return targetGraphic.DOColor(GetGraphicTargetColor(@in), duration);
                }
            }
        }
    }
}