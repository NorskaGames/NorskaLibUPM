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

        /// <summary>
        /// Casts multiple spheres from 'origin' towards corresponding angle, 
        /// where 0 degress match global North (Vector3.one); if sphere DID hit
        /// anything the angle is stored in the 'results' buffer.
        /// </summary>
        /// <param name="results"> Array of angles in degrees, where cast hit something. </param>
        /// <param name="spheresCount"></param>
        /// <returns> Results count. </returns>
        public static int CircleSphereCastNonAlloc(
            Vector3 origin, 
            float sphereRadius, 
            float castRadius, 
            LayerMask castMask,
            RaycastHit[] hitsBuffer,
            float[] results,
            int spheresCount = 8)
            => CircleSphereCastNonAllocInternal(origin, sphereRadius, castRadius, castMask, hitsBuffer, results, spheresCount, false);

        /// <summary>
        /// Casts multiple spheres from 'origin' towards corresponding angle, 
        /// where 0 degress match global North (Vector3.one); if sphere DIDN'T hit
        /// anything the angle is stored in the 'results' buffer.
        /// </summary>
        /// <param name="results"> Array of angles in degrees, where cast hit nothing. </param>
        /// <param name="spheresCount"></param>
        /// <returns> Results count. </returns>
        public static int CircleSphereCastInverseNonAlloc(
            Vector3 origin,
            float sphereRadius,
            float castRadius,
            LayerMask castMask,
            RaycastHit[] hitsBuffer,
            float[] results,
            int spheresCount = 8)
            => CircleSphereCastNonAllocInternal(origin, sphereRadius, castRadius, castMask, hitsBuffer, results, spheresCount, true);

        private static int CircleSphereCastNonAllocInternal(
            Vector3 origin,
            float sphereRadius,
            float castRadius,
            LayerMask castMask,
            RaycastHit[] hitsBuffer,
            float[] results,
            int spheresCount,
            bool inverse = false)
        {
            var resultsCount = 0;
            spheresCount = Mathf.Clamp(spheresCount, 2, spheresCount);
            var angularDelta = 360 / spheresCount;
            for (int i = 0; i < spheresCount; i++)
            {
                var angle = i * angularDelta;
                var direction = MathUtils.PositionOnCircle3D(angle);
                var hitsCount = Physics.SphereCastNonAlloc(origin, sphereRadius, direction, hitsBuffer, castRadius, castMask);
                if (inverse && hitsCount > 0 || !inverse && hitsCount <= 0)
                    continue;

                results[i] = angle;
                resultsCount++;
            }

            return resultsCount;
        }
    }
}
