using System.Drawing;
using System.Drawing.Imaging;

namespace BLL.Drawing
{
    public class Rasterizer
    {
        private Bitmap BM;
        public Bitmap Result { get { return BM; } }

        private int Width, Height;
        private int offSetX, offSetY;

        public Rasterizer(int MinX, int MaxX, int MinY, int MaxY)
        {
            this.Width = (MaxX - MinX) + 1;
            this.Height = (MaxY - MinY) + 1;

            if (MinX < 0)
                MinX = -MinX;

            offSetX = MinX;

            offSetY = MinY;

            BM = new Bitmap(Width, Height);
        }

        public void DrawTriangle(Color C1, int X1, int Y1, Color C2, int X2, int Y2, Color C3, int X3, int Y3, bool Traficlights)
        {
            this.DrawTriangle(C1, X1, Y1, C2, X2, Y2, C3, X3, Y3);
            if (Traficlights)
            {
                SetPixel(X1 + offSetX, Y1 - offSetY, Color.Black);
                SetPixel(X2 + offSetX, Y2 - offSetY, Color.Black);
                SetPixel(X3 + offSetX, Y3 - offSetY, Color.Black);
            }
        }

        public void DrawTriangle(Color C1, int X1, int Y1, Color C2, int X2, int Y2, Color C3, int X3, int Y3)
        {
            // Create edges for the triangle
            Edge[] edges = {
                new Edge(C1, X1+offSetX, Y1-offSetY, C2, X2+offSetX, Y2-offSetY),
                new Edge(C2, X2+offSetX, Y2-offSetY, C3, X3+offSetX, Y3-offSetY),
                new Edge(C3, X3+offSetX, Y3-offSetY, C1, X1+offSetX, Y1-offSetY)
            };

            int maxLength = 0;
            int longEdge = 0;

            // Find edge with the greatest length in the y axis
            for (int i = 0; i < 3; i++)
            {
                int length = edges[i].Point2.Y - edges[i].Point1.Y;
                if (length > maxLength)
                {
                    maxLength = length;
                    longEdge = i;
                }
            }

            int shortEdge1 = (longEdge + 1) % 3;
            int shortEdge2 = (longEdge + 2) % 3;

            // draw spans between edges; the long edge can be drawn
            // with the shorter edges to draw the full triangle
            DrawSpansBetweenEdges(edges[longEdge], edges[shortEdge1]);
            DrawSpansBetweenEdges(edges[longEdge], edges[shortEdge2]);
        }

        private void DrawSpansBetweenEdges(Edge e1, Edge e2)
        {
            // calculate difference between the y coordinates
            // of the first edge and return if 0
            float e1ydiff = (float)(e1.Point2.Y - e1.Point1.Y);
            if (e1ydiff == 0.0f)
                return;

            // calculate difference between the y coordinates
            // of the second edge and return if 0
            float e2ydiff = (float)(e2.Point2.Y - e2.Point1.Y);
            if (e2ydiff == 0.0f)
                return;

            // calculate differences between the x coordinates
            // and colors of the points of the edges
            float e1xdiff = (float)(e1.Point2.X - e1.Point1.X);
            float e2xdiff = (float)(e2.Point2.X - e2.Point1.X);
            //Color e1colordiff = Color.sub .FromArgb( (e1.Point2.Color.ToArgb() - e1.Point1.Color.ToArgb()) );
            //Color e2colordiff = Color.FromArgb( (e2.Point2.Color.ToArgb() - e2.Point1.Color.ToArgb()) );

            // calculate factors to use for interpolation
            // with the edges and the step values to increase
            // them by after drawing each span
            float factor1 = (float)(e2.Point1.Y - e1.Point1.Y) / e1ydiff;
            float factorStep1 = 1.0f / e1ydiff;
            float factor2 = 0.0f;
            float factorStep2 = 1.0f / e2ydiff;

            // loop through the lines between the edges and draw spans
            for (int y = e2.Point1.Y; y < e2.Point2.Y; y++)
            {
                // create and draw span
                Span span = new Span(ColorInterpolator.InterpolateBetween(e1.Point1.Color, e1.Point2.Color, factor1),
                          e1.Point1.X + (int)(e1xdiff * factor1),
                          ColorInterpolator.InterpolateBetween(e2.Point1.Color, e2.Point2.Color, factor2),
                          e2.Point1.X + (int)(e2xdiff * factor2));
                DrawSpan(span, y);

                // increase factors
                factor1 += factorStep1;
                factor2 += factorStep2;
            }

        }

        public void DrawSpan(Span span, int y)
        {
            int xdiff = span.X2 - span.X1;
            if (xdiff == 0)
                return;

            Color colordiff = Color.FromArgb(span.Color2.ToArgb() - span.Color1.ToArgb());

            float factor = 0.0f;
            float factorStep = 1.0f / (float)xdiff;

            // draw each pixel in the span
            for (int x = span.X1; x < span.X2; x++)
            {
                SetPixel(x, y, ColorInterpolator.InterpolateBetween(span.Color1, span.Color2, factor));
                factor += factorStep;
            }
        }

        private void SetPixel(int x, int y, Color color)
        {
            BM.SetPixel(x, y, color);
        }
    }
}
