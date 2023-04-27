using DG.Tweening;
using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NorskaLib.UI.Widgets
{
    public sealed partial class WidgetAnimator
    {
        [System.Serializable]
        public sealed class ScaleModule : Module
        {
            [EnableIf("@enabled")]
            public RectTransform targetTransform;
            [EnableIf("@enabled")]
            public bool isUniform;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("In"), ShowIf("@isUniform")]
            public float inUniform = 1.00f;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("Out"), ShowIf("@isUniform")]
            public float outUniform = 0.95f;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("In"), ShowIf("@!isUniform")]
            public Vector3 inVector = Vector3.one;
            [EnableIf("@enabled"), BoxGroup("Value"), LabelText("Out"), ShowIf("@!isUniform")]
            public Vector3 outVector = Vector3.one * 0.95f;

            private Vector3 GetTargetValue(bool @in)
            {
                return @in
                    ? isUniform
                        ? Vector3Utils.Uniform(inUniform)
                        : inVector
                    : isUniform
                        ? Vector3Utils.Uniform(outUniform)
                        : outVector;
            }

            public override void Set(bool @in)
            {
                targetTransform.localScale = GetTargetValue(@in);
            }

            public override Tween CreateTween(bool @in, float duration)
            {
                return targetTransform.DOScale(GetTargetValue(@in), duration);
            }
        }
    }
}