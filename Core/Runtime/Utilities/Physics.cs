using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct PhysicsUtils
    {
        /// <returns> Position along the cast trajectory, where sphere was when it produced the hit. </returns>
        public static Vector3 SphereCastPivot(Vector3 origin, Vector3 direction, float hitDistance)
        {
            return origin + direction * hitDistance;
        }
    }
}
