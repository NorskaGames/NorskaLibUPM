using System;

namespace NorskaLib.UI.Markers
{
    internal struct MarkerSortData<E> : IEquatable<MarkerSortData<E>>, IComparable<MarkerSortData<E>> where E : MarkerEntry
    {
        public MarkerWidget<E> widget;
        public float distance;

        public int CompareTo(MarkerSortData<E> other)
        {
            var layerCmp = this.widget.SortingOrder.CompareTo(other.widget.SortingOrder);

            return layerCmp != 0 
                ? layerCmp 
                : this.distance.CompareTo(other.distance);
        }

        public bool Equals(MarkerSortData<E> other)
        {
            return this.widget.Equals(other.widget);
        }
    }
}