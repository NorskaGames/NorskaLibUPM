using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct TextUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns> Given 'text' wrapped into markup symbols matching TMPro character colorizing syntax like "color=#FF00FF00 text /color" </returns>
        public static string Format(string text, Color color)
        {
            return $"<color=#{ColorUtils.GetHEXA(color)}>{text}</color>";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns> Given 'spriteName' wrapped into markup symbols matching TMPro sprite inlining syntax like "sprite name=\spriteName\" </returns>
        public static string Format(string spriteName)
        {
            return $"<sprite name=\"{spriteName}\">";
        }
    } 
}