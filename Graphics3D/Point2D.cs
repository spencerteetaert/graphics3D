using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    public class Point2D
    {

        #region Class parameters 
        double x = 0, y = 0;

        #endregion

        #region Class constructors 
        //Fcns that are executed when you initialize (new Point2D();)
        /// <summary>
        /// Create a default instance of Point2D
        /// </summary>
        public Point2D()
        {

        }
        /// <summary>
        /// Create an instance of Point2D with a position
        /// </summary>
        /// <param name="x">Initial x-value</param>
        /// <param name="y">Initial y-value</param>
        public Point2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Class operators
        /// <summary>
        /// Add together the x/y values of the two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The sum of the points</returns>
        public static Point2D operator +(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.x + p2.x, p1.y + p2.y);
        }
        /// <summary>
        /// Subtract the x/y values of the two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The difference of the points</returns>
        public static Point2D operator -(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.x - p2.x, p1.y - p2.y);
        }
        /// <summary>
        /// Multiply a point by a factor
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="factor">Factor</param>
        /// <returns>Scaled point</returns>
        public static Point2D operator *(Point2D p1, double factor)
        {
            return new Point2D(p1.x * factor, p1.y * factor);
        }
        public static Point2D operator *(double factor, Point2D p1)
        {
            return new Point2D(p1.x * factor, p1.y * factor);
        }
        /// <summary>
        /// Divide a point by a divisor
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="factor">Divisor</param>
        /// <returns>Scaled point</returns>
        public static Point2D operator /(Point2D p1, double divisor)
        {
            return new Point2D(p1.x / divisor, p1.y / divisor);
        }
        public static double operator *(Point2D p1, Point2D p2)
        {
            return p1.x * p2.x + p1.y * p2.y;
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
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }
        /// <summary>
        /// Returns the line normal to this point (when used as a vector)
        /// </summary>
        public Point2D Normal
        {
            get { return new Point2D(-y, x); }
        }
        public PointF ToPointF { get { return new PointF((float)x, (float)y); } }
        #endregion

        #region Class methods
        public void Draw(Graphics gr, Color color, int size)
        {
            gr.FillEllipse(new SolidBrush(color), (float)x - size / 2, (float)y - size / 2, size, size);
        }
        /// <summary>
        /// Normalizes this point/vector to have length 1
        /// </summary>
        public void Normalize()
        {
            double magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
        }
        public void Rotate (double theta)
        {
            double x2 = x * Math.Cos(theta) - y * Math.Sin(theta);
            double y2 = x * Math.Sin(theta) + y * Math.Cos(theta);
            x = x2;
            y = y2;
        }
        public void RotateDeg(double angle)
        {
            Rotate(angle * Math.PI / 180);
        }
        public void UnRotate(double theta)
        {
            theta *= -1;
            double x2 = x * Math.Cos(theta) - y * Math.Sin(theta);
            double y2 = x * Math.Sin(theta) + y * Math.Cos(theta);
            x = x2;
            y = y2;
        }
        public void UnRotateDeg(double angle)
        {
            UnRotate(angle * Math.PI / 180);
        }
        #endregion
    }
}
