using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class MarkerWidget : MonoBehaviour
    {
        public RectTransform Transform { get; private set; }

        public MarkerEntry Entry { get; private set; }

        /// <summary>
        /// Determines which widgets will be rendered on top of other (low values rendered above high).
        /// </summary>
        public virtual int SortingOrder => 0;

        #region MonoBehaviour

        protected virtual void Awake()
        {
            Transform = GetComponent<RectTransform>();
        }

        #endregion

        internal MarkerModes lastMode = MarkerModes.Uninitialized;
        /// <summary>
        /// Is called when object moves in or out of camera frustrum. Override this method to switch UI layout correspondingly.
        /// </summary>
        public virtual void SwitchMode(MarkerModes mode) 
        {
            lastMode = mode;
        }

        public virtual void Bind(MarkerEntry entry)
        {
            Entry = entry;
        }

        public virtual void Unbind()
        {

        }
    }

    public interface IDistanceDisplayerWidget
    {
        public void DisplayDistance(float distance);
    }

    public interface IAngleDisplayerWidget
    {
        /// <summary>
        /// Is called each update frame for markers, which entries is in compas mode (are off screen).
        /// Use it to implement custom compas-marker widget logic (for example, rotating an arrow towards quest target).
        /// </summary>
        /// <param name="angle"> Signed angle in degrees from MarkerOverlay.camera.forward to direction
        /// from MarkerOverlay.CompasPivot.position towards entry.WorldPosition. </param>
        public void DisplayAngle(float angle);
    }

    internal struct MarkerWidgetSortData : IEquatable<MarkerWidgetSortData>, IComparable<MarkerWidgetSortData>
    {
        public MarkerWidget widget;
        public float distance;

        public int CompareTo(MarkerWidgetSortData other)
        {
            if (this.widget.SortingOrder < other.widget.SortingOrder)
                return -1;

            return this.distance.CompareTo(other.distance);
        }

        public bool Equals(MarkerWidgetSortData other)
        {
            return this.widget.Equals(other.widget);
        }
    }
}