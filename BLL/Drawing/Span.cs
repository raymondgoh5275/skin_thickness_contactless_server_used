using System.Drawing;

namespace BLL.Drawing
{
    public class Span
    {
        public Color Color1, Color2;
        public int X1, X2;

        public Span(Color color1, int x1, Color color2, int x2)
        {
            if (x1 < x2)
            {
                Color1 = color1;
                X1 = x1;
                Color2 = color2;
                X2 = x2;
            }
            else
            {
                Color1 = color2;
                X1 = x2;
                Color2 = color1;
                X2 = x1;
            }
        }
    }
}
