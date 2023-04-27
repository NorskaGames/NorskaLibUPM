using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct ColorUtils
    {
        public static Color GetRGB(string RRGGBB)
        {
            var RR = new string(new char[] { RRGGBB[0], RRGGBB[1] });
            var GG = new string(new char[] { RRGGBB[2], RRGGBB[3] });
            var BB = new string(new char[] { RRGGBB[4], RRGGBB[5] });

            var r = int.Parse(RR, System.Globalization.NumberStyles.HexNumber) / 255f;
            var g = int.Parse(GG, System.Globalization.NumberStyles.HexNumber) / 255f;
            var b = int.Parse(BB, System.Globalization.NumberStyles.HexNumber) / 255f;

            return new Color(r, g, b);
        }

        public static Color GetRGBA(string RRGGBBAA)
        {
            var RR = new string(new char[] { RRGGBBAA[0], RRGGBBAA[1] });
            var GG = new string(new char[] { RRGGBBAA[2], RRGGBBAA[3] });
            var BB = new string(new char[] { RRGGBBAA[4], RRGGBBAA[5] });
            var AA = new string(new char[] { RRGGBBAA[6], RRGGBBAA[7] });

            var r = int.Parse(RR, System.Globalization.NumberStyles.HexNumber) / 255f;
            var g = int.Parse(GG, System.Globalization.NumberStyles.HexNumber) / 255f;
            var b = int.Parse(BB, System.Globalization.NumberStyles.HexNumber) / 255f;
            var a = int.Parse(AA, System.Globalization.NumberStyles.HexNumber) / 255f;

            return new Color(r, g, b, a);
        }

        public static string GetHEX(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);

            var RR = Convert.ToString(r, 16);
            var GG = Convert.ToString(g, 16);
            var BB = Convert.ToString(b, 16);

            return RR + GG + BB;
        }

        public static string GetHEXA(Color color)
        {
            var components = new int[]
            {
                Mathf.RoundToInt(color.r * 255),
                Mathf.RoundToInt(color.g * 255),
                Mathf.RoundToInt(color.b * 255),
                Mathf.RoundToInt(color.a * 255)
            };

            var stringBuilder = new StringBuilder(components.Length);
            foreach (var c in components)
                stringBuilder.Append(c > 0 ? Convert.ToString(c, 16) : "00");

            return stringBuilder.ToString();
        }

        public static string Format(string text, Color color)
        {
            return $"<color=#{GetHEXA(color)}>{text}</color>";
        }
    }
}
