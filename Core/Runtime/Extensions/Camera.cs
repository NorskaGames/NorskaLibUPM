using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class CameraExtensions
    {
        public static Vector2 WorldToScreenPointNormalized(this Camera camera, Vector3 worldPosition)
        {
            return camera.WorldToScreenPoint(worldPosition) / new Vector2(camera.pixelWidth, camera.pixelHeight);
        }

        public static Vector2 WorldToScreenPointNormalized(this Camera camera, Vector3 worldPosition, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
        {
            return camera.WorldToScreenPoint(worldPosition, eye) / new Vector2(camera.pixelWidth, camera.pixelHeight);
        }

        public static bool PointIsInsideViewport(this Camera camera, Vector3 worldPosition)
        {
            return camera.PointIsInsideViewport(worldPosition, Vector2.zero, Vector2.one);
        }

        public static bool PointIsInsideViewport(this Camera camera, Vector3 worldPosition, out Vector2 normalizedPosition)
        {
            return camera.PointIsInsideViewport(worldPosition, Vector2.zero, Vector2.one, out normalizedPosition);
        }

        public static bool PointIsInsideViewport(this Camera camera, Vector3 worldPosition, Vector2 offscreenMarginsMin, Vector2 offscreenMarginsMax)
        {
            var normalizedPosition = camera.WorldToScreenPointNormalized(worldPosition);
            return normalizedPosition.y.IsBetween(offscreenMarginsMin.y, offscreenMarginsMax.y) 
                && normalizedPosition.x.IsBetween(offscreenMarginsMin.x, offscreenMarginsMax.x);
        }

        public static bool PointIsInsideViewport(this Camera camera, Vector3 worldPosition, Vector2 offscreenMarginsMin, Vector2 offscreenMarginsMax, out Vector2 normalizedPosition)
        {
            normalizedPosition = camera.WorldToScreenPointNormalized(worldPosition);
            return normalizedPosition.y.IsBetween(offscreenMarginsMin.y, offscreenMarginsMax.y)
                && normalizedPosition.x.IsBetween(offscreenMarginsMin.x, offscreenMarginsMax.x);
        }
    }
}