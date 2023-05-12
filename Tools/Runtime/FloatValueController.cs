using NorskaLib.Extensions;
using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Tools
{
	public class FloatValueController : MonoBehaviour
	{
        #region OdinInspector

        private bool appisPlaying => UnityEngine.Application.isPlaying;

        #endregion

        public float defaultValue;
        public TargetResolving targetResolving = TargetResolving.Minimum;

        public float softingSpeed = 1;
        public float softingReferenceDelta = 0.5f;
        public AnimationCurve softingCurve;

        #region public struct Request
        [Serializable]
		public struct Request : IEquatable<Request>
		{
			public GameObject source;
			public float value;

            #region IEquatable

            bool IEquatable<Request>.Equals(Request other)
            {
                return this.source.Equals(other.source);
            }

            public static bool operator ==(Request x, Request y)
            {
                return x.source == y.source;
            }

            public static bool operator !=(Request x, Request y)
            {
                return !(x == y);
            }

            public override bool Equals(object obj)
            {
                if (obj is Request)
                {
                    return Equals((Request)obj);
                }
                return false;
            }

            #endregion
        }
        #endregion

        public enum TargetResolving
        {
            Minimum,
            Maximum
        }

        [ShowInInspector, ShowIf(nameof(appisPlaying)), DisableIf("@true")]
        private float value;
        public float Value => value;

        private const int DefaultRequestsCapacity = 4;
        [ShowInInspector, ShowIf(nameof(appisPlaying)), DisableIf("@true")]
        private List<Request> requests;

        public event Action<float> OnValueChanged;

		public void RequestValue(GameObject source, float value)
		{
            var cloneIndex = -1;
            for (int i = 0; i < requests.Count; i++)
            {
                var rqst = requests[i];
                if (rqst.source == source)
                {
                    rqst.value = value;
                    cloneIndex = i;
                    requests[i] = rqst;
                    break;
                }
            }

            var newRqst = new Request()
            {
                source = source,
                value = value
            };

            if (cloneIndex == -1)
                requests.Add(newRqst);
            else
                requests[cloneIndex] = newRqst;
        }

        public void UnrequestValue(GameObject source)
        {
            var index = -1;
            for (int i = 0; i < requests.Count; i++)
                if (requests[i].source == source)
                {
                    index = i;
                    break;
                }

            if (index == -1)
                return;

            requests.RemoveAt(index);
        }

        #region MonoBehaviour

        void Awake()
        {
            value = defaultValue;
            requests = new List<Request>(DefaultRequestsCapacity);
        }

        void Update()
        {
            var targetValue = default(float);
            if (requests.Count <= 0)
                targetValue = defaultValue;
            else
            {
                targetValue = requests[0].value;
                for (int i = 1; i < requests.Count; i++)
                {
                    var rqst = requests[i];
                    switch (targetResolving)
                    {
                        case TargetResolving.Maximum:
                            if (rqst.value > targetValue)
                                targetValue = rqst.value;
                            break;

                        default:
                        case TargetResolving.Minimum:
                            if (rqst.value < targetValue)
                                targetValue = rqst.value;
                            break;
                    }
                }
            }

            if (targetValue.Approximately(value))
                return;

            var targetDelta = targetValue - value;
            var targetDeltaAbs = Mathf.Abs(targetDelta);
            var softingFactor = Mathf.InverseLerp(0, softingReferenceDelta, targetDeltaAbs);
            var timeDelta = Time.deltaTime;
            var valueDelta = softingSpeed * softingCurve.Evaluate(softingFactor) * timeDelta;
            value = targetDelta > 0
                ? Mathf.Clamp(value + timeDelta, value, targetValue)
                : Mathf.Clamp(value - timeDelta, targetValue, value);
            OnValueChanged?.Invoke(value);
        }

        #endregion
    } 
}