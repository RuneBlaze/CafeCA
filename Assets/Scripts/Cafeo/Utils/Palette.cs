using System.Drawing;
using Color = UnityEngine.Color;

namespace Cafeo.Utils
{
    public static class Palette
    {

        public static Color HexColor(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
        // we use pico8 palette
        public static Color black = HexColor(0, 0, 0);
        public static Color deepBlue = HexColor(0x1d, 0x2b, 0x53);
        public static Color purple = HexColor(0x7e, 0x25, 0x53);
        public static Color tealGreen = HexColor(0x00, 0x87, 0x51);
        public static Color brown = HexColor(0xab, 0x52, 0x36);
        public static Color gray = HexColor(0x5f, 0x57, 0x4f);
        public static Color cloudGray = HexColor(0xc2, 0xc3, 0xc7);
        public static Color milkWhite = HexColor(0xff, 0xf1, 0xe8);
        public static Color red = HexColor(0xff, 0x00, 0x4f);
        public static Color orange = HexColor(0xff, 0xa3, 0x00);
        public static Color yellow = HexColor(0xff, 0xec, 0x27);
        public static Color green = HexColor(0x00, 0xe4, 0x36);
        public static Color skyBlue = HexColor(0x29, 0xad, 0xff);
        public static Color clayPurple = HexColor(0x83, 0x76, 0x9c);
        public static Color pink = HexColor(0xff, 0x77, 0xa8);
        public static Color milkYellow = HexColor(0xff, 0xcc, 0xaa);

        public static Color SwapAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}