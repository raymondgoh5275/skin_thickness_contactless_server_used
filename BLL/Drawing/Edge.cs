using System.Drawing;

namespace BLL.Drawing
{
    class Edge
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public Edge(Color C1, int X1, int Y1, Color C2, int X2, int Y2)
        {
            this.Point1 = new Point();
            this.Point2 = new Point();

            if (Y1 < Y2)
            {
                this.Point1.Color = C1;
                this.Point1.X = X1;
                this.Point1.Y = Y1;

                this.Point2.Color = C2;
                this.Point2.X = X2;
                this.Point2.Y = Y2;
            }
            else
            {
                this.Point2.Color = C1;
                this.Point2.X = X1;
                this.Point2.Y = Y1;

                this.Point1.Color = C2;
                this.Point1.X = X2;
                this.Point1.Y = Y2;
            }
        }
    }
}
