using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    class Sphere : Point3D
    {
        #region Parameters
        double radius = 0, mass = 1;
        Color color = Color.White;
        #endregion

        #region Constructors
        public Sphere() { }
        public Sphere(Point3D center, double radius)
        {
            this.X = center.X;
            this.Y = center.Y;
            this.Z = center.Z;
            this.radius = radius;
        }
        public Sphere(double x, double y, double z, double radius)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.radius = radius;
        }
        #endregion

        public Color Color { get { return this.color; } set { this.color = value; } }
        public double Mass { get { return this.mass; } set { this.mass = value; } }

        #region Methods
        public void Draw(Graphics gr, Color color, double distance)
        {
            //make a 2D circle (projection of the center), adjust radius
            Point2D center = Projection(distance);
            double radiusProjected = distance * radius / (distance - Z);
            Circle2D c = new Circle2D(center, radiusProjected);
            c.Draw(gr, color);
        }
        public void Fill(Graphics gr, Color color, double distance)
        {
            //make a 2D circle (projection of the center), adjust radius
            Point2D center = Projection(distance);
            double radiusProjected = distance * radius / (distance - Z);
            Circle2D c = new Circle2D(center, radiusProjected);
            c.Fill(gr, color);
        }
        public void Fill(Graphics gr, double distance)
        {
            Fill(gr, color, distance);
        }
        #endregion
    }
}
