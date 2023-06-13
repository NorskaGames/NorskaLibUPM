using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public enum Modes { Custom, OnUpdate, OnFixedUpdate }

    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform instance)
        {
            var childCount = instance.childCount;
            for (var i = childCount - 1; i > -1; i--)
            {
                Object.Destroy(instance.GetChild(i).gameObject);
            }
        }

        [System.Obsolete("Use Transform.localScale = Vector3Utils.Uniform instead.")]
        public static void SetScale(this Transform instance, float scale)
        {
            instance.localScale = new Vector3(scale, scale, scale);
        }

        public static void SetPositionAndRotation(this Transform transform, Transform other)
        {
            transform.SetPositionAndRotation(other.transform.position, other.transform.rotation);
        }

        public static void SetLocalPositionAndRotation(this Transform transform, Transform other)
        {
            transform.SetLocalPositionAndRotation(other.transform.position, other.transform.rotation);
        }
    }
}
