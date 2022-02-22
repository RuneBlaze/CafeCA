using Drawing;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Cafeo.Utils
{
    public static class ImDraw
    {
        public static void DrawGauge(CommandBuilder cb, float x, float y, float width, float height, 
            int v, int maxV, Color bg, Color fg, float fontSize = 55)
        {
            cb.SolidRectangle(new Rect(x, y, width, height), bg);
            cb.SolidRectangle(new Rect(x, y, (width * v / maxV), height), fg);
            if (fontSize > 0)
            {
                cb.Label2D(float3(x + 5, y + 10, 0), $"{v}/{maxV}", fontSize, Palette.milkWhite);
            }
        }
    }
}