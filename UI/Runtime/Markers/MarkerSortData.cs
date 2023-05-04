using System;

namespace NorskaLib.UI.Markers
{
    internal struct MarkerSortData : IEquatable<MarkerSortData>, IComparable<MarkerSortData>
    {
        public MarkerWidget widget;
        public float distance;

        public int CompareTo(MarkerSortData other)
        {
            var layerCmp = this.widget.SortingOrder.CompareTo(other.widget.SortingOrder);

            return layerCmp != 0 
                ? layerCmp 
                : this.distance.CompareTo(other.distance);
        }

        public bool Equals(MarkerSortData other)
        {
            return this.widget.Equals(other.widget);
        }
    }
}