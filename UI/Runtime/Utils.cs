using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NorskaLib.UI
{
    public struct TMProUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns> A markup symbols matching TMPro sprite inlining syntax like "sprite name=\spriteName\" </returns>
        public static string WrapInlineSprite(string spriteName)
        {
            return $"<sprite name=\"{spriteName}\">";
        }
    }

    public struct UIUtils
    {
        [Serializable]
        public struct Anchors
        {
            public Vector2 min;
            public Vector2 max;
        }

        [Flags]
        public enum RectKeyPoints : byte
        {
            Top     = 1 << 0,
            Bottom  = 1 << 1,
            Left    = 1 << 2,
            Right   = 1 << 3
        }

        public static Dictionary<byte, Vector2> KeyPointsMasksToAnchors = new Dictionary<byte, Vector2>()
        {
            { 
                (byte)RectKeyPoints.Top, 
                new Vector2(0.5f, 1.0f)
            },

            {
                (byte)RectKeyPoints.Bottom,
                new Vector2(0.5f, 0.0f)
            },

            {
                (byte)RectKeyPoints.Left,
                new Vector2(0.0f, 0.5f)
            },

            {
                (byte)RectKeyPoints.Right,
                new Vector2(1.0f, 0.5f)
            },

            {
                (byte)RectKeyPoints.Top + (byte)RectKeyPoints.Left,
                new Vector2(0.0f, 1.0f)
            },

            {
                (byte)RectKeyPoints.Top + (byte)RectKeyPoints.Right,
                new Vector2(1.0f, 1.0f)
            },

            {
                (byte)RectKeyPoints.Bottom + (byte)RectKeyPoints.Left,
                new Vector2(0.0f, 0.0f)
            },

            {
                (byte)RectKeyPoints.Bottom + (byte)RectKeyPoints.Right,
                new Vector2(1.0f, 0.0f)
            },
        };

        public static GameObject GetObject(Vector2 screenPosition)
        {
            var hitObjects = GetObjects(screenPosition);

            return hitObjects.Count > 0
                ? hitObjects[0].gameObject
                : null;
        }

        public static List<GameObject> GetObjects(Vector2 screenPosition)
        {
            var pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = screenPosition;

            var hitObjects = new List<RaycastResult>();
            var list = new List<GameObject>();

            EventSystem.current.RaycastAll(pointerData, hitObjects);

            foreach (var item in hitObjects)
                list.Add(item.gameObject);

            return list;
        }

        public static byte OverlapRect(Vector2 size, Vector2 center, Rect targetRect)
        {
            Dictionary<RectKeyPoints, Vector2> keyPoints = new Dictionary<RectKeyPoints, Vector2>()
            {
                { RectKeyPoints.Top,    center + new Vector2(0, +size.y)},
                { RectKeyPoints.Bottom, center + new Vector2(0, -size.y)},
                { RectKeyPoints.Left,   center + new Vector2(+size.x, 0)},
                { RectKeyPoints.Right,  center + new Vector2(-size.x, 0)}
            };

            byte mask = 0;
            foreach (var item in keyPoints)
                if (!targetRect.Contains(item.Value))
                    mask += (byte)item.Key;

            return mask;
        }

        public static Vector2 WorldToRectPosition(Camera camera, RectTransform rectTtansform, Vector3 worldPosition)
        {
            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = (Vector2)camera.WorldToScreenPoint(worldPosition);
            var normalizedPosition = cameraPosition / cameraSize;

            var rectSize = rectTtansform.rect.size;

            return normalizedPosition * rectSize;
        }
        public static Vector2 WorldToScreenPosition(Camera camera, Vector3 worldPosition)
        {
            var cameraSize = new Vector2Int(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = (Vector2)camera.WorldToScreenPoint(worldPosition);
            var normalizedPosition = cameraPosition / cameraSize;

            return cameraSize * normalizedPosition;
        }
        public static Vector2 WorldToScreenPosition(Camera camera, RectTransform renderedTextureRect, Vector3 worldPosition)
        {
            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = (Vector2)camera.WorldToScreenPoint(worldPosition);
            var normalizedPosition = cameraPosition / cameraSize;

            var screenRect = RectTransformToScreenSpace(renderedTextureRect);

            return screenRect.min + normalizedPosition * screenRect.size;
        }
        
        public static Rect WorldToScreenRect(Camera camera, RectTransform renderedTextureRect, Vector3 worldPositionA, Vector3 worldPositionB)
        {
            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPositionA = (Vector2)camera.WorldToScreenPoint(worldPositionA);
            var cameraPositionB = (Vector2)camera.WorldToScreenPoint(worldPositionB);
            var normalizedPositionA = cameraPositionA / cameraSize;
            var normalizedPositionB = cameraPositionB / cameraSize;

            var screenRect = RectTransformToScreenSpace(renderedTextureRect);

            var screenPosA = screenRect.min + normalizedPositionA * screenRect.size;
            var screenPosB = screenRect.min + normalizedPositionB * screenRect.size;

            var min = new Vector2(
                screenPosA.x > screenPosB.x 
                    ? screenPosB.x
                    : screenPosA.x,
                screenPosA.y > screenPosB.y
                    ? screenPosB.y
                    : screenPosA.y);
            var max = new Vector2(
                screenPosA.x < screenPosB.x
                    ? screenPosB.x
                    : screenPosA.x,
                screenPosA.y < screenPosB.y
                    ? screenPosB.y
                    : screenPosA.y);

            return new Rect(min, max - min);
        }

        public static Vector2 AdjustPositionInRect(Vector2 screenPosition, Vector2 defaultOffset, RectTransform transform, bool rightHand)
        {
            var inBounds = RectTransformUtility.RectangleContainsScreenPoint(transform, screenPosition);
            if (!inBounds)
                return screenPosition;

            var rect = RectTransformToScreenSpace(transform);

            var borders = new Vector2(
                rightHand
                    ? rect.xMax
                    : rect.xMin,
                rect.yMin);

            return screenPosition + CalculateAdjustedOffset(screenPosition, defaultOffset, borders, rightHand);
        }
        public static Vector2 AdjustPositionInScreen(Vector2 screenPosition, Vector2 defaultOffset, Vector2 screenResolution, bool rightHand)
        {
            var borders = new Vector2(
                rightHand
                    ? 0
                    : screenResolution.x,
                screenResolution.y);

            return screenPosition + CalculateAdjustedOffset(screenPosition, defaultOffset, borders, rightHand);
        }

        private static Vector2 CalculateAdjustedOffset(Vector2 screenPosition, Vector2 defaultOffset, Vector2 borders, bool rightHand)
        {
            var delta = new Vector2(
                    Mathf.Abs(screenPosition.x - borders.x),
                    Mathf.Abs(screenPosition.y - borders.y))
                / defaultOffset;

            if (rightHand)
                defaultOffset.x = -1 * defaultOffset.x;

            var adjustedOffset = Vector2.zero;

            if (delta.x < 1)
                adjustedOffset.x = defaultOffset.x * delta.x;
            else if (delta.x >= 1)
                adjustedOffset.x = defaultOffset.x;

            if (delta.y < 1)
                adjustedOffset.y = defaultOffset.y * delta.y;
            else if (delta.y >= 1)
                adjustedOffset.y = defaultOffset.y;

            return adjustedOffset;
        }

        // by Malkyne and Zmeyk
        // https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * transform.pivot), size);
        }

        // TO DO:
        // Надо как-то унифицировать
        public static Vector3 RectToWorldPosition(RectTransform rect, Vector2 pointerPosition, Camera camera, LayerMask raycastMask, out Vector3 rayOrigin, out Vector3 rayEnd, float linecastLength = 100)
        {
            var contains = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, pointerPosition, null, out var rectPosition);
            //if (!contains)


            var size = rect.rect.size;

            var normalizedPosition = (rectPosition + size / 2) / size;

            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = cameraSize * normalizedPosition;

            var ray = camera.ScreenPointToRay(cameraPosition);

            rayOrigin = ray.origin;
            rayEnd = ray.origin + ray.direction * linecastLength;

            if (Physics.Linecast(rayOrigin, rayEnd, out var hitInfo, raycastMask))
                return hitInfo.point;
            else
                return rayEnd;
        }
        public static Vector3 RectToWorldPosition(RectTransform rect, Vector2 pointerPosition, Camera camera, LayerMask raycastMask, float linecastLength = 100)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, pointerPosition, null, out var rectPosition);

            var rectSize = rect.sizeDelta;

            var normalizedPosition = (rectPosition + rectSize / 2) / rectSize;

            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = cameraSize * normalizedPosition;

            var ray = camera.ScreenPointToRay(cameraPosition);

            var rayEnd = ray.origin + ray.direction * linecastLength;
            if (Physics.Linecast(ray.origin, rayEnd, out var hitInfo, raycastMask))
                return hitInfo.point;
            else
                return rayEnd;
        }
        public static Vector3 RectToWorldPosition(RectTransform rect, Vector2 pointerPosition, Camera camera, float linecastLength = 100)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, pointerPosition, null, out var rectPosition);

            var rectSize = rect.sizeDelta;

            var normalizedPosition = (rectPosition + rectSize / 2) / rectSize;

            var cameraSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
            var cameraPosition = cameraSize * normalizedPosition;

            var ray = camera.ScreenPointToRay(cameraPosition);

            var rayEnd = ray.origin + ray.direction * linecastLength;
            if (Physics.Linecast(ray.origin, rayEnd, out var hitInfo))
                return hitInfo.point;
            else
                return rayEnd;
        }

        public static Rect FittingScreenRect(RectTransform[] rects)
        {
            var screenSpaceRect0 = RectTransformToScreenSpace(rects[0]);
            var min = screenSpaceRect0.min;
            var max = screenSpaceRect0.max;

            for (int i = 1; i < rects.Length; i++)
            {
                var screenSpaceRect = RectTransformToScreenSpace(rects[i]);
                if (screenSpaceRect.xMin < min.x)
                    min.x = screenSpaceRect.xMin;
                if (screenSpaceRect.xMax > max.x)
                    max.x = screenSpaceRect.xMax;

                if (screenSpaceRect.yMin < min.y)
                    min.y = screenSpaceRect.yMin;
                if (screenSpaceRect.yMax > max.y)
                    max.y = screenSpaceRect.yMax;
            }

            return new Rect(min, max - min);
        }

        /// <summary>
        /// Растягивает и позиционирует sumRect так, чтобы он охватывал все rects[].
        /// </summary>
        /// <param name="sumRect"> Будет изменен. </param>
        /// <param name="rects"></param>
        public static void Fit(RectTransform sumRect, RectTransform[] rects)
        {
            var screenRect = FittingScreenRect(rects);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)sumRect.parent, 
                screenRect.min, 
                null, 
                out var localMin);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)sumRect.parent, 
                screenRect.max, 
                null, 
                out var localMax);

            var size = localMax - localMin;
            var center = localMin + size / 2;
            var anchors = 0.5f;

            sumRect.anchorMin = sumRect.anchorMax = new Vector2(anchors, anchors);
            sumRect.localPosition = center;
            sumRect.sizeDelta = size;
        }

        public static void Fit(RectTransform rectTransform, Rect screenRect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)rectTransform.parent, screenRect.min, null, out var localMin);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)rectTransform.parent, screenRect.max, null, out var localMax);

            var size = localMax - localMin;
            var center = localMin + size / 2;

            var anchors = 0.5f;
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchors, anchors);
            rectTransform.localPosition = center;
            rectTransform.sizeDelta = size;
        }
    }
}
