using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    class Point3D : IComparable<Point3D>
    {
        #region Class parameters 
        double x = 0, y = 0, z = 0;

        #endregion

        #region Class constructors 
        //Fcns that are executed when you initialize (new Point3D();)
        /// <summary>
        /// Create a default instance of Point3D
        /// </summary>
        public Point3D()
        {

        }
        /// <summary>
        /// Create an instance of Point3D with a position
        /// </summary>
        /// <param name="x">Initial x-value</param>
        /// <param name="y">Initial y-value</param>
        /// <param name="z">Initial z-value</param>
        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        #endregion

        #region Class operators
        /// <summary>
        /// Add together the x/y values of the two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The sum of the points</returns>
        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }
        /// <summary>
        /// Subtract the x/y values of the two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The difference of the points</returns>
        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }
        /// <summary>
        /// Multiply a point by a factor
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="factor">Factor</param>
        /// <returns>Scaled point</returns>
        public static Point3D operator *(Point3D p1, double factor)
        {
            return new Point3D(p1.x * factor, p1.y * factor, p1.z * factor);
        }
        public static Point3D operator *(double factor, Point3D p1)
        {
            return new Point3D(p1.x * factor, p1.y * factor, p1.z * factor);
        }
        /// <summary>
        /// Divide a point by a divisor
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="factor">Divisor</param>
        /// <returns>Scaled point</returns>
        public static Point3D operator /(Point3D p1, double divisor)
        {
            return new Point3D(p1.x / divisor, p1.y / divisor, p1.z / divisor);
        }
        public static double operator *(Point3D p1, Point3D p2)
        {
            return p1.x * p2.x + p1.y * p2.y;
        }

        public static Point3D operator ^ (Point3D p1, Point3D p2)
        {
            return new Point3D(
                p1.y * p2.z - p1.z * p2.y, 
                p1.z * p2.x - p1.x * p2.z,
                p1.x * p2.y - p1.y * p2.x
                );
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Get/Set the x-value
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        /// <summary>
        /// Get/Set the y-value
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }
        /// <summary>
        /// Returns the line normal to this point (when used as a vector)
        /// </summary>
        #endregion

        #region Class methods
            public Point3D toInt()
        {
            return new Point3D(Math.Round(x), Math.Round(y), Math.Round(z));
        }
        public void Draw(Graphics gr, Color color, double distance)
        {
            Projection(distance).Draw(gr, color, 2);
        }
        /// <summary>
        /// Project the 3D point onto the 2D surface where the observor is a distance from the origin
        /// </summary>
        /// <param name="distance">Observer distance from the origin</param>
        /// <returns></returns>
        public Point2D Projection (double distance)
        {
            return new Point2D(
                distance * x / (distance - z),
                distance * y / (distance - z));
        }
        /// <summary>
        /// Normalizes this point/vector to have length 1
        /// </summary>
        public void Normalize()
        {
            double magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
            Z /= magnitude;
        }
        /// <summary>
        /// Rotates 3D point 
        /// </summary>
        /// <param name="theta">Point3D rotation angles in radians</param>
        public void Rotate(Point3D theta)
        {
            //x-axis rotation
            double z2 = z * Math.Cos(theta.X) - y * Math.Sin(theta.X);
            double y2 = z * Math.Sin(theta.X) + y * Math.Cos(theta.X);
            z = z2;
            y = y2;
            //y-axis rotation
            double x2 = x * Math.Cos(theta.Y) - z * Math.Sin(theta.Y);
            z2 = x * Math.Sin(theta.Y) + z * Math.Cos(theta.Y);
            x = x2;
            z = z2;
            //z-axis rotation
            x2 = x * Math.Cos(theta.Z) - y * Math.Sin(theta.Z);
            y2 = x * Math.Sin(theta.Z) + y * Math.Cos(theta.Z);
            x = x2;
            y = y2;
        }
        public void RotateDeg(Point3D angle)
        {
            Rotate(angle * Math.PI / 180);
        }
        public void UnRotate(Point3D theta)
        {
            theta *= -1;
            //z-axis rotation
            double x2 = x * Math.Cos(theta.Z) - y * Math.Sin(theta.Z);
            double y2 = x * Math.Sin(theta.Z) + y * Math.Cos(theta.Z);
            x = x2;
            y = y2;
            //y-axis rotation
            x2 = x * Math.Cos(theta.Y) - z * Math.Sin(theta.Y);
            double z2 = x * Math.Sin(theta.Y) + z * Math.Cos(theta.Y);
            x = x2;
            z = z2;
            //x-axis rotation
            z2 = z * Math.Cos(theta.X) - y * Math.Sin(theta.X);
            y2 = z * Math.Sin(theta.X) + y * Math.Cos(theta.X);
            z = z2;
            y = y2;     
        }
        public void UnRotateDeg(Point3D angle)
        {
            UnRotate(angle * Math.PI / 180);
        }
        #endregion

        #region Comparer
        private double Depth { get { return Z; } }
        public int CompareTo(Point3D otherPoint)
        {
            if (this.Depth > otherPoint.Depth)
                return 1;
            else if (this.Depth == otherPoint.Depth)
                return 0;
            else
                return -1;
        }
        #endregion
    }
}
