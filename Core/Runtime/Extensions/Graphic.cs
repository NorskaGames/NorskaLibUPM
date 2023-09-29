using UnityEngine.UI;

namespace NorskaLib.Extensions
{
    public static class GraphicExtensions
    {
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = graphic.color.WithA(alpha);
        }
    } 
}