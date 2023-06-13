using NorskaLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Windows;

namespace NorskaLib.Utilities
{
    public struct MathUtils
    {
        public static float Min(float a, float b, float c)
        {
            return Mathf.Min(Mathf.Min(a, b), c);
        }
        public static int Min(int a, int b, int c)
        {
            return Mathf.Min(Mathf.Min(a, b), c);
        }

        public static double Min(double a, double b)
        {
            return a <= b ? a : b;
        }

        public static float Max(float a, float b, float c)
        {
            return Mathf.Max(Mathf.Max(a, b), c);
        }
        public static int Max(int a, int b, int c)
        {
            return Mathf.Max(Mathf.Max(a, b), c);
        }

        public static double Max(double a, double b)
        {
            return a >= b ? a : b;
        }

        public static double InverseLerp(double a, double b, double value)
        {
            return Clamp01((value - a) / (b - a));
        }

        public static double InverseLerpUnclamped(double a, double b, double value)
        {
            return (value - a) / (b - a);
        }

        public static double Clamp01(double value)
        {
            return value >= 1
                ? 1
                : value <= 0
                    ? 0
                    : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value >= max
                ? max
                : value <= min
                    ? min
                    : value;
        }

        public static List<int> GetRangeList(int min, int max)
        {
            var list = new List<int>(max - min + 1);
            for (int i = min; i <= max; i++)
                list.Add(i);

            return list;
        }
        public static int[] GetRangeArray(int min, int max)
        {
            var array = new int[max - min + 1];
            for (int i = 0; i < array.Length; i++)
                array[i] = min + i;

            return array;
        }

        // TO DO:
        // Refactor allocations
        //
        //public static int[] GetRangeArray(int min, int max, int[] exeptions)
        //{
        //    var pool = new List<int>();
        //    for (int i = min; i < max; i++)
        //    {
        //        var exeption = false;
        //        for (int j = 0; j < exeptions.Length; j++)
        //            if (i == exeptions[j])
        //            {
        //                exeption = true;
        //                break;
        //            }

        //        if (!exeption)
        //            pool.Add(i);
        //    }

        //   return  pool.ToArray();
        //}

        public static float Radians(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        /// <summary>
        /// Отображает угол в диапазоне [-180; 180] в диапазон [0; 360]
        /// </summary>
        public static float SignedTo360(float signedAngle)
        {
            return signedAngle < 0
                ? 360 + signedAngle
                : signedAngle;
        }


        /// <returns> Same angle in -180...+180 degrees range.</returns>
        //public static float ToSignedDegrees(float degrees)
        //{
        //    
        //}

        public static float CylindricSpiralLength(float R, float H)
        {
            var b = H / (2 * Mathf.PI);
            return 2 * Mathf.PI * Mathf.Sqrt(R * R + b * b);
        }

        public static float CircleLength(float radius)
        {
            return 2 * Mathf.PI * radius;
        }

        public static float ArcLength(float radius, float degrees)
        {
            var p = degrees * Mathf.PI / 180;
            return p * radius;
        }

        /// <param name="angle"> Signed angle from -180 to 180 degrees measured from Vector2.up (0, 1). </param>
        /// <param name="alpha"> Angle between vertical axis and the diagonal, that splits top-right quadrant of the rectangle into 2 triangles. 45 degrees is the default value for circles and squares. </param>
        /// <returns> Index of 1/8 part of a circle corresponding given angle, which is measured from Vector.up (0, 1) axis clockwise. </returns>
        public static int GetOctantIndex(float angle, float alpha = 45)
        {
            return GetOctantIndex(angle, out var legAngle, alpha);
        }

        /// <param name="angle"> Signed angle from -180 to 180 degrees measured from Vector2.up (0, 1). </param>
        /// <param name="legAngle"> TO DO </param>
        /// <param name="alpha"> Angle between vertical axis and the diagonal, that splits top-right quadrant of the rectangle into 2 triangles. 45 degrees is the default value for circles and squares. </param>
        /// <returns> Index of 1/8 part of a circle corresponding given angle, which is measured from Vector.up (0, 1) axis clockwise. </returns>
        public static int GetOctantIndex(float angle, out float legAngle, float alpha = 45)
        {
            var beta = 90 - alpha;

            float minAngle, maxAngle;
            minAngle = maxAngle = -180;

            var initialIndex = 4;

            for (int i = 0; i < 8; i++)
            {
                var index = (initialIndex + i) % 8;
                var deltaAngle = index.EqualsAny(0, 3, 4, 7)
                    ? alpha
                    : beta;

                maxAngle += deltaAngle;

                legAngle = index.EqualsAny(0, 2, 4, 6)
                    ? angle - minAngle
                    : maxAngle - angle;
                if (angle.IsBetween(minAngle, maxAngle))
                    return index;

                minAngle = maxAngle;
            }

            legAngle = 0;
            return -1;
        }

        public static Vector3 Project(Vector3 position, Vector3 A, Vector3 B)
        {
            var normal = (B - A).normalized;
            var vector = position - A;

            return A + Vector3.Project(vector, normal);
        }

        public static Vector3 PositionOnCylindricSpiral(Vector3 origin, float radius, float height, float t, bool clockwise = true, float initialAngle = 0)
        {
            var sign = clockwise
                ? +1
                : -1;
            var p = (sign * t * 2 + initialAngle / 180) * Mathf.PI;

            var x = radius * Mathf.Sin(p);
            var y = height * t;
            var z = radius * Mathf.Cos(p);

            return origin + new Vector3(x, y, z);
        }

        /// <param name="rectSize"> </param>
        /// <param name="angle"> Signed angle from -180 to 180 degrees measured from Vector2.up (0, 1). </param>
        /// <returns> Position on the rectangle border at given angle from its center. </returns>
        public static Vector2 PositionOnRectangle(Vector2 rectCenter, Vector2 rectSize, float angle)
        {
            var halfsize = rectSize / 2;

            if (angle.Approximately(0))
                return rectCenter + Vector2.up * halfsize.y;
            else if (angle.Approximately(90))
                return rectCenter + Vector2.right * halfsize.x;
            else if (angle.Approximately(-90))
                return rectCenter + Vector2.left * halfsize.x;
            else if (angle.ApproximatelyAny(-180, 180))
                return rectCenter + Vector2.down * halfsize.y;
            else
            {
                var tr = rectCenter + halfsize;

                var alpha = AbsoluteSignedAngleXZ(tr - rectCenter);
                var octanIndex = GetOctantIndex(angle, out var legAngle, alpha);
                (var normalLength, var delta, var normal) = octanIndex.EqualsAny(1, 2, 5, 6)
                    ? (halfsize.x, Vector2.up, Vector2.right)
                    : (halfsize.y, Vector2.right, Vector2.up);
                var deltaSign = octanIndex.EqualsAny(0, 1, 3, 6)
                    ? +1
                    : -1;
                var normalSign = octanIndex.EqualsAny(0, 1, 2, 7)
                    ? +1
                    : -1;

                var deltaLength = normalLength * Mathf.Tan(Radians(legAngle));
                var result = rectCenter + normalSign * normalLength * normal + deltaSign * deltaLength * delta;
                return result;
            }
        }

        /// <param name="angle"> Signed angle from -180 to 180 degrees measured from Vector2.up (0, 1). </param>
        /// <returns> Position on the rectangle border at given angle from its center. </returns>
        public static Vector2 PositionOnRectangle(Rect rect, float angle)
        {
            return PositionOnRectangle(rect.center, rect.size, angle);
        }

        public static Vector3 PositionOnCircle3D(float degrees, float radius = 1)
        {
            var p = degrees * Mathf.PI / 180;

            var z = radius * Mathf.Cos(p);
            var x = radius * Mathf.Sin(p);

            return new Vector3(x, 0, z);
        }
        public static Vector3 PositionOnCircle3D(Vector3 origin, float degrees, float radius = 1)
        {
            var p = degrees * Mathf.PI / 180;

            var z = radius * Mathf.Cos(p);
            var x = radius * Mathf.Sin(p);

            return origin + new Vector3(x, 0, z);
        }
        public static Vector2 PositionOnCircle2D(float degrees, float radius)
        {
            var p = degrees * Mathf.PI / 180;

            var y = radius * Mathf.Cos(p);
            var x = radius * Mathf.Sin(p);

            return new Vector2(x, y);
        }
        public static Vector2 PositionOnCircle2D(Vector2 origin, float degrees, float radius)
        {
            var p = degrees * Mathf.PI / 180;

            var y = radius * Mathf.Cos(p);
            var x = radius * Mathf.Sin(p);

            return origin + new Vector2(x, y);
        }

        // https://ru.wikipedia.org/wiki/Кривая_Безье
        public static Vector3 PositionOnQuadCurve(Vector3 startPos, Vector3 arcPos, Vector3 endPos, float t)
        {
            var b = (1 - t) * (1 - t) * startPos
                + 2 * t * (1 - t) * arcPos
                + t * t * endPos;

            return b;
        }
        public static Vector3 PositionOnQuadCurveClamped(Vector3 startPos, Vector3 arcPos, Vector3 endPos, float t)
        {
            t = Mathf.Clamp01(t);

            var b = (1 - t) * (1 - t) * startPos
                + 2 * t * (1 - t) * arcPos
                + t * t * endPos;

            return b;
        }

        /// <summary>
        /// Возвращает примерную длину квадратичной кривой Безье.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="arcPos"></param>
        /// <param name="endPos"></param>
        /// <param name="divCount"> Кол-во отрезков, на которые разбивается кривая. </param>
        /// <returns></returns>
        public static float QuadCurveLengthSlow(Vector3 startPos, Vector3 arcPos, Vector3 endPos, int divCount)
        {
            var length = 0f;
            var a = startPos;
            for (int i = 0; i < divCount; i++)
            {
                var t = (i + 1) / (float)divCount;
                var b = PositionOnQuadCurve(startPos, arcPos, endPos, t);
                length += Vector3.Distance(a, b);
                a = b;
            }

            return length;
        }

        /// <summary>
        /// Возвращает точку, на которую смотрит точка на квадратичной кривой
        /// </summary>
        /// <param name="startPos"> Начальная точка </param>
        /// <param name="arcPos"> Опорная точка, задающая форму </param>
        /// <param name="endPos"> Конечная точка </param>
        /// <param name="t"> Параметр, определяющий долю от пути по кривой, 
        /// на которой находится точка; лежит в пределах от 0 до 1 </param>
        public static Vector3 LookPositionOnQuad(Vector3 startPos, Vector3 arcPos, Vector3 endPos, float t)
        {
            return arcPos + (endPos - arcPos) * t;
        }

        // By DylanW https://answers.unity.com/questions/556480/rotate-the-shortest-way.html
        public static float ShortestRotationAngle(float from, float to)
        {
            // If from or to is a negative, we have to recalculate them.
            // For an example, if from = -45 then from(-45) + 360 = 315.
            if (from < 0)
            {
                from += 360;
            }

            if (to < 0)
            {
                to += 360;
            }

            // Do not rotate if from == to.
            if (from == to ||
               from == 0 && to == 360 ||
               from == 360 && to == 0)
            {
                return 0;
            }

            // Pre-calculate left and right.
            float left = (360 - from) + to;
            float right = from - to;
            // If from < to, re-calculate left and right.
            if (from < to)
            {
                if (to > 0)
                {
                    left = to - from;
                    right = (360 - to) + from;
                }
                else
                {
                    left = (360 - to) + from;
                    right = to - from;
                }
            }

            // Determine the shortest direction.
            return (left <= right) 
                ? left 
                : (right * -1);
        }

        public static int ShortestRotationDirection(float from, float to)
        {
            var angle = ShortestRotationAngle(from, to);
            if (angle > 0)
                return 1;
            else if (angle < 0)
                return -1;
            else
                return 0;
        }

        public static float RelativeSignedAngleXZ(Transform origin, Vector3 position)
        {
            var origin2D = new Vector2(origin.position.x, origin.position.z);
            var axis2D = new Vector2(origin.forward.x, origin.forward.z);
            var position2D = new Vector2(position.x, position.z);
            var direction2D = position2D - origin2D;

            return Vector2.SignedAngle(axis2D, direction2D);
        }
        public static float RelativeSignedAngleXZ(Vector3 origin, Vector3 positionA, Vector3 positionB)
        {
            var origin2D = new Vector2(origin.x, origin.z);
            var directionA2D = new Vector2(positionA.x, positionA.z) - origin2D;
            var directionB2D = new Vector2(positionB.x, positionB.z) - origin2D;

            return Vector2.SignedAngle(directionA2D, directionB2D);
        }
        public static float RelativeUnsignedAngleXZ(Vector3 directionnA, Vector3 directionB)
        {
            var directionA2D = new Vector2(directionnA.x, directionnA.z);
            var directionB2D = new Vector2(directionB.x, directionB.z);

            return Vector2.Angle(directionA2D, directionB2D);
        }

        /// <summary>
        /// Returns a signed angle (-180, +180) in XZ plane between
        /// position - origin and Vector3.forward.
        /// </summary>
        public static float AbsoluteSignedAngleXZ(Vector3 origin, Vector3 position)
        {
            return -Vector2.SignedAngle(Vector2.up, position.ToXZ() - origin.ToXZ());
        }
        /// <summary>
        /// Returns a signed angle (-180, +180) in XZ plane between
        /// given direction and Vector3.forward.
        /// </summary>
        public static float AbsoluteSignedAngleXZ(Vector3 direction)
        {
            return -Vector2.SignedAngle(Vector2.up, direction.ToXZ());
        }
        /// <summary>
        /// Returns a signed angle (-180, +180) in XZ plane between
        /// given direction and Vector2.up.
        /// </summary>
        public static float AbsoluteSignedAngleXZ(Vector2 direction)
        {
            return -Vector2.SignedAngle(Vector2.up, direction);
        }
        /// <summary>
        /// Returns a signed angle (-180, +180) in XZ plane between
        ///  given Transform.forward and Vector3.forward.
        /// </summary>
        public static float AbsoluteSignedAngleXZ(Transform transform)
        {
            return -Vector2.SignedAngle(Vector2.up, transform.forward.ToXZ());
        }

        public static bool SectorCircleCollision(Vector2 sectorOrigin, float sectorRadius, float sectorFacing, float sectorSpan, Vector2 circleOrigin, float circleRadius,
            out int samplePointsCount, out Vector2 samplePoint0, out Vector2 samplePoint1, out Vector2 samplePoint2)
        {
            samplePointsCount = -1;
            samplePoint2 = new Vector2(-1, -1);

            var direction = circleOrigin - sectorOrigin;
            var distance = direction.magnitude;
            var intersectionCount = CirclesIntersect(sectorOrigin, sectorRadius, circleOrigin, circleRadius, out samplePoint0, out samplePoint1);

            // Too far from each other
            if (intersectionCount == -1)
                return false;

            if (distance < sectorRadius)
            {
                CirclesIntersect(sectorOrigin, distance, circleOrigin, circleRadius, out samplePoint0, out samplePoint1);
                samplePoint2 = sectorOrigin + direction * (1 + circleRadius / distance);
                samplePointsCount = 3;
            }
            else
            {
                samplePointsCount = intersectionCount;
            }

            var halfspan = sectorSpan * 0.5f;
            for (int i = 0; i < samplePointsCount; i++)
            {
                var samplePoint = i == 0 
                    ? samplePoint0 
                    : i == 1
                        ? samplePoint1
                        : samplePoint2;

                var angle = AbsoluteSignedAngleXZ(samplePoint - sectorOrigin);
                if (Mathf.Abs(ShortestRotationAngle(sectorFacing, angle)) <= halfspan)
                    return true;
            }

            return false;
        }
        public static bool SectorCircleCollision(Vector2 sectorOrigin, float sectorRadius, float sectorFacing, float sectorSpan, Vector2 circleOrigin, float circleRadius)
        {
            return SectorCircleCollision(sectorOrigin, sectorRadius, sectorFacing, sectorSpan, circleOrigin, circleRadius, out var spc, out var sp0, out var sp1, out var sp2);
        }

        // From https://planetcalc.ru/8098/
        /// <param name="intersect0"> If circles do not intersect will be set to (-1, -1). </param>
        /// <param name="intersect1"> If circles intersect only once will be set to (-1, -1). </param>
        /// <returns> 
        /// -1 - circles are too far from each other;
        /// 0 - circles overlap but don't intersect (one is inside another);
        /// 1 and 2 - self-explanatory.
        /// </returns>
        public static int CirclesIntersect(Vector2 originA, float radiusA, Vector2 originB, float radiusB, out Vector2 intersect0, out Vector2 intersect1)
        {
            var delta       = originB - originA;
            var distance    = delta.magnitude;
            var radiusSum   = radiusA + radiusB;
            var radiusDelta = Mathf.Abs(radiusA - radiusB);

            intersect0 = new Vector2(-1, -1);
            intersect1 = new Vector2(-1, -1);

            // No intersection
            if (distance > radiusSum)
                return -1;
            // 1 point
            else if (Mathf.Approximately(distance, radiusSum))
            {
                intersect0 = Vector2.Lerp(originA, originB, radiusA / radiusSum);
                return 1;
            }
            // One insude another
            else if (distance < radiusDelta)
                return 0;
            // 2 points
            else
            {
                var radiusSqrA = radiusA * radiusA;
                var radiusSqrB = radiusB * radiusB;
                var distanceSqr = Vector2.SqrMagnitude(delta);
                var a = (radiusSqrA - radiusSqrB + distanceSqr) / (2 * distance);
                var h = Mathf.Sqrt(radiusSqrA - (a * a));
                var H = Vector2.LerpUnclamped(originA, originB, a / distance);

                intersect0 = new Vector2(
                    x: H.x + h / distance * delta.y,
                    y: H.y - h / distance * delta.x);
                intersect1 = new Vector2(
                    x: H.x - h / distance * delta.y,
                    y: H.y + h / distance * delta.x);

                return 2;
            }
        }
    }
}
