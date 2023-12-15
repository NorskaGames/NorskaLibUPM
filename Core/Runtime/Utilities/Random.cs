using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NorskaLib.Extensions;

namespace NorskaLib.Utilities
{
    public interface IRandomizable
    {
        /// <summary>
        /// NOTE: Must be a positive number!
        /// </summary>
        public float Weight { get; }
    }

    public struct RandomUtils
    {
        /// <summary>
        /// Returns random bool value, based on chance; for chance <= 0.00 always returns FALSE; for chance >= 1.00 always returns TRUE.
        /// </summary>
        /// <param name="dice"> Randomly generated "roll" value. </param>
        public static bool Bool(float chance, out float dice)
        {
            dice = Random.Range(0, 0.99f);
            return Mathf.Clamp01(chance) >= dice;
        }
        /// <summary>
        /// Returns random bool value, based on chance; for chance <= 0.00 always returns FALSE; for chance >= 1.00 always returns TRUE.
        /// </summary>
        public static bool Bool(float chance)
        {
            return Bool(chance, out var dice);
        }

        public static int Range(int min, int max, int exeption)
        {
            var pool = new List<int>();
            for (int i = min; i < max; i++)
                if (i != exeption)
                    pool.Add(i);

            var index = UnityEngine.Random.Range(0, pool.Count);

            return pool[index];
        }
        public static int Range(int min, int max, int[] exeptions)
        {
            var pool = new List<int>();
            for (int i = min; i < max; i++)
            {
                var exclude = false;
                for (int j = 0; j < exeptions.Length; j++)
                    if (i == exeptions[j])
                    {
                        exclude = true;
                        break;
                    }

                if (!exclude)
                    pool.Add(i);
            }

            var index = Random.Range(0, pool.Count);

            return pool[index];
        }
        public static int RangeInclusive(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        public static Vector3 Vector3Normalized()
        {
            return Vector3(-1, 1, -1, 1, -1, 1).normalized;
        }
        public static Vector3 Vector3(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            var x = Random.Range(minX, maxX);
            var y = Random.Range(minY, maxY);
            var z = Random.Range(minZ, maxZ);

            return new Vector3(x, y, z);
        }
        public static Vector3 Vector3(Bounds bounds)
        {
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            var z = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(x, y, z);
        }

        public static int Index(float[] weigths)
        {
            float roll = Random.Range(0, weigths.Sum());

            float lastMin = 0;
            float lastMax = 0;

            for (int i = 0; i < weigths.Length; i++)
            {
                if (i > 0)
                    lastMin += weigths[i - 1];

                lastMax += weigths[i];

                if (roll >= lastMin && roll < lastMax && !weigths[i].ApproximatelyZero())
                    return i;
            }

            return -1;
        }
        public static int Index<T>(IEnumerable<T> collection) where T : IRandomizable
        {
            var W = 0f;
            foreach (var randomizable in collection)
                W += randomizable.Weight;

            var roll = Random.Range(0, W);

            var min = 0f;
            var index = 0;
            foreach (var randomizable in collection)
            {
                var weight = randomizable.Weight;
                var max = min + weight;
                if (roll.IsBetween(min, max, false, true) && !weight.ApproximatelyZero())
                    return index;

                min = max;
                index++;
            }

            return -1;
        }

        public static T Element<T>(IEnumerable<T> collection) where T : IRandomizable
        {
            var W = 0f;
            foreach (var randomizable in collection)
                W += randomizable.Weight;

            var roll = Random.Range(0, W);

            var min = 0f;
            foreach (var randomizable in collection)
            {
                var weight = randomizable.Weight;
                var max = min + weight;
                if (roll.IsBetween(min, max, false, true) && !weight.ApproximatelyZero())
                    return randomizable;

                min = max;
            }

            return default;
        }
    }
}
