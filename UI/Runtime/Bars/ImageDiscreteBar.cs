using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI
{
    public class ImageDiscreteBar : DiscreteBar
    {
        [SerializeField] protected List<DiscreteBarSegment> segments;
        public override List<DiscreteBarSegment> Segements => segments;
    }
}