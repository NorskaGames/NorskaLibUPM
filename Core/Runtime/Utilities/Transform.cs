using System;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct TransformUtils
    {
        private struct SortMeta
        {
            public object instance;
            public float distanceDelta;
            public float angleDelta;
            public float weight;

            public void UpdateWeight(float angleMax, float distanceMax, float distanceToAngle)
            {
                var D = distanceToAngle;
                var A = 1 - distanceToAngle;

                weight = A * angleDelta / angleMax + D * distanceDelta / distanceMax;
            }

            public static int Compare(SortMeta a, SortMeta b)
                => a.weight.CompareTo(b.weight);
        }

        private const int DefaultSortMetaBufferLenght = 32;
        private static SortMeta[] sortMetaBuffer = new SortMeta[DefaultSortMetaBufferLenght];

        private class SortMetaComparer : IComparer<SortMeta>
        {
            public int Compare(SortMeta x, SortMeta y)
                => x.weight.CompareTo(y.weight);
        }

        private static SortMetaComparer sortMetaComparer = new SortMetaComparer();

        public static void ReallocateBuffer(int bufferSize)
        {
            sortMetaBuffer = new SortMeta[bufferSize];
        }

        public static void SortByDistance<C>(Vector3 origin, C[] instancesBuffer, int instancesCount) where C : Component
        {
            var deltaMax = default(float);
            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
            {
                var instance = instancesBuffer[i];
                var delta = Vector3.SqrMagnitude(instance.transform.position - origin);
                var meta = new SortMeta()
                {
                    instance = instance,
                    distanceDelta = delta,
                };
                sortMetaBuffer[i] = meta;

                if (delta > deltaMax)
                    deltaMax = delta;
            }

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                sortMetaBuffer[i].UpdateWeight(1, deltaMax, 1);

            Array.Sort(sortMetaBuffer, 0, instancesCount, sortMetaComparer);

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                instancesBuffer[i] = sortMetaBuffer[i].instance as C;
        }

        public static void SortByAngle<C>(Vector3 origin, float angleAxis, C[] instancesBuffer, int instancesCount) where C : Component
        {
            var deltaMax = default(float);
            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
            {
                var instance = instancesBuffer[i];
                var angleDirection = MathUtils.AbsoluteSignedAngleXZ(origin, instance.transform.position);
                var delta = MathUtils.ShortestRotationAngle(angleAxis, angleDirection);
                var meta = new SortMeta()
                {
                    instance = instance,
                    angleDelta = delta,
                };
                sortMetaBuffer[i] = meta;

                if (delta > deltaMax)
                    deltaMax = delta;
            }

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                sortMetaBuffer[i].UpdateWeight(deltaMax, 1, 0);

            Array.Sort(sortMetaBuffer, 0, instancesCount, sortMetaComparer);

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                instancesBuffer[i] = sortMetaBuffer[i].instance as C;
        }

        public static void SortByDistanceAndAngle<C>(Vector3 origin, float angleAxis, C[] instances, int instancesCount, float distanceToAngle = 0.5f) where C : Component
        {
            var aDeltaMax = default(float);
            var dDeltaMax = default(float);
            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
            {
                var instance = instances[i];
                var dDelta = Vector3.SqrMagnitude(instance.transform.position - origin);

                var angleDirection = MathUtils.AbsoluteSignedAngleXZ(origin, instance.transform.position);
                var aDelta = MathUtils.ShortestRotationAngle(angleAxis, angleDirection);
                var meta = new SortMeta()
                {
                    instance = instance,
                    distanceDelta = dDelta,
                    angleDelta = aDelta,
                };
                sortMetaBuffer[i] = meta;

                if (aDelta > aDeltaMax)
                    aDeltaMax = aDelta;
                if (dDelta > dDeltaMax)
                    dDeltaMax = dDelta;
            }

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                sortMetaBuffer[i].UpdateWeight(aDeltaMax, dDeltaMax, distanceToAngle);

            Array.Sort(sortMetaBuffer, 0, instancesCount, sortMetaComparer);

            for (int i = 0; i < instancesCount && i < sortMetaBuffer.Length; i++)
                instances[i] = sortMetaBuffer[i].instance as C;
        }
    }
}
