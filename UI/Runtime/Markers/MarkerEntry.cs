using NorskaLib.Extensions;
using NorskaLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.UI.Markers
{
    public abstract class MarkerEntry : MonoBehaviour
    {
        #region Static

        private static List<MarkerEntry> instances;
        public static List<MarkerEntry> Instances 
        { 
            get
            {
                if (instances == null)
                    instances = new List<MarkerEntry>(100);

                return instances;
            }
        }

        public static event Action<MarkerEntry> onInstanceRegistred;
        public static event Action<MarkerEntry> onInstanceUnregistred;

        #endregion

        /// <summary>
        /// Override this getter to implement custom logic which determines when marker should be visible. 
        /// NOTE: to implement markers, that can be physically obscured by other colliders implement IPhysicallyObscuredMarkerEntry interface.
        /// </summary>
        public virtual bool Show => true;

        public abstract MarkerModes Modes { get; }

        public abstract Vector3 PivotPosition { get; }

        public abstract Vector3 WidgetPosition { get; }

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            Instances.Add(this);
            onInstanceRegistred?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            Instances.Remove(this);
            onInstanceUnregistred?.Invoke(this);
        }

        #endregion

        #region Editor
#if UNITY_EDITOR

        private const float GizmosRadius = 0.1f;
        private static Vector3 GizmosSize_Widget = new Vector3(1, 0.2f, 0);
        private static Vector3 GizmosSize_CrossPoint = new Vector3(0.4f, 0.4f, 0.4f);
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow.WithA(0.3f);
            Gizmos.DrawSphere(PivotPosition, GizmosRadius);
            Gizmos.DrawWireSphere(PivotPosition, GizmosRadius);
            GizmosUtils.DrawCrossPoint(PivotPosition, GizmosSize_CrossPoint);

            var widgetCenterOffset = new Vector3(0, GizmosSize_Widget.y * 0.5f, 0);
            Gizmos.DrawWireCube(WidgetPosition + widgetCenterOffset, GizmosSize_Widget);
            Gizmos.DrawCube(WidgetPosition + widgetCenterOffset, GizmosSize_Widget);
            GizmosUtils.DrawCrossPoint(WidgetPosition, GizmosSize_CrossPoint);

            if (this is IPhysicallyObscuredMarkerEntry poEntry && poEntry.MultipleSamples)
            {
                Gizmos.color = Color.green.WithA(0.3f);
                foreach (var samplePoint in poEntry.SamplePoints)
                {
                    var position = PivotPosition + samplePoint;
                    Gizmos.DrawSphere(position, GizmosRadius);
                    Gizmos.DrawWireSphere(position, GizmosRadius);
                }
            }
        }

#endif
        #endregion
    }

    /// <summary>
    /// World-markers which can be obscured by physical colliders (for example, players' name labels) must implement this interface.
    /// </summary>
    public interface IPhysicallyObscuredMarkerEntry
    {
        public LayerMask CollisionMask { get; }

        /// <summary>
        /// If TRUE - SamplePoints will be used to check collision, else - only MarkerEntry.PivotPosition will be linecasted.
        /// </summary>
        public bool MultipleSamples { get; }

        /// <summary>
        /// NOTE: Points will be considered as in local object space (relative to MarkerEntry.PivotPosition).
        /// </summary>
        public IEnumerable<Vector3> SamplePoints { get; }
    }
}