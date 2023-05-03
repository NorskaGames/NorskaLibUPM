using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI.Markers
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class MarkerWidget<E> : MonoBehaviour where E : MarkerEntry
    {
        public RectTransform Transform { get; private set; }

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

        public virtual void Bind(E entry) { }

        public virtual void Unbind(E entry) { }
    }

    public interface IDistanceDisplayerWidget
    {
        public void Display(float distance);
    }

    public interface IAngleDisplayerWidget
    {
        /// <summary>
        /// Use it to implement custom compas-marker widget logic (for example, rotating an arrow towards quest target).
        /// </summary>
        /// <param name="angle"> Signed angle in degrees from 'MarkerOverlay.camera.forward' to direction
        /// from 'MarkerOverlay.CompasPivot.position' towards 'MarkerEntry.WorldPosition'. </param>
        public void Display(float angle);
    }

    public interface IFacingDisplayerWidget
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facingAngle"> Absolute facing angle of a corresponding entry. </param>
        public void Display(float facingAngle);
    }
}