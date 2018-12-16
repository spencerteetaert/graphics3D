using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    class Circle2D : Point2D
    {
        #region Class parameters
        double radius = 0;
        #endregion

        #region Class constructors
        public Circle2D() { }
        public Circle2D(Point2D center, double radius)
        {
            this.X = center.X;
            this.Y = center.Y;
            this.radius = radius;
        }
        public Circle2D(double x, double y, double radius)
        {
            this.X = x;
            this.Y = y;
            this.radius = radius;
        }
        #endregion

        #region Class properties
        public double Radius { get { return radius; } set { radius = value; } }
        #endregion

        #region Class methods
        public void Draw(Graphics gr, Color color)
        {
            gr.DrawEllipse(new Pen(color), (float)(this.X - radius), (float)(this.Y - radius), (float)radius * 2, (float)radius * 2);
        }
        public void Fill(Graphics gr, Color color)
        {
            gr.FillEllipse(new SolidBrush(color), (float)(this.X - radius), (float)(this.Y - radius), (float)radius * 2, (float)radius * 2);
        }
        #endregion
    }
}
