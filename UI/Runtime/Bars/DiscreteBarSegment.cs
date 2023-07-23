using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI
{
    public abstract class DiscreteBarSegment : MonoBehaviour
    {
        public abstract void SetImmediate(bool filled);

        public abstract void SetAnimated(bool filled, float duration = 0.15f);
    }
}