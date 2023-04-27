using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithR(this Color instance, float r)
        {
            return new Color(r, instance.g, instance.b, instance.a);
        }
        public static Color WithRG(this Color instance, float r, float g)
        {
            return new Color(r, g, instance.b, instance.a);
        }
        public static Color WithRB(this Color instance, float r, float b)
        {
            return new Color(r, instance.g, b, instance.a);
        }
        public static Color WithRA(this Color instance, float r, float a)
        {
            return new Color(r, instance.g, instance.b, a);
        }
        public static Color WithRGB(this Color instance, float r, float g, float b)
        {
            return new Color(r, g, b, instance.a);
        }

        public static Color WithG(this Color instance, float g)
        {
            return new Color(instance.r, g, instance.b, instance.a);
        }
        public static Color WithGB(this Color instance, float g, float b)
        {
            return new Color(instance.r, g, b, instance.a);
        }
        public static Color WithGA(this Color instance, float g, float a)
        {
            return new Color(instance.r, g, instance.b, a);
        }

        public static Color WithB(this Color instance, float b)
        {
            return new Color(instance.r, instance.g, b, instance.a);
        }
        public static Color WithBA(this Color instance, float b, float a)
        {
            return new Color(instance.r, instance.g, b, a);
        }

        public static Color WithA(this Color instance, float a)
        {
            return new Color (instance.r, instance.g, instance.b, a);
        }
    }
}