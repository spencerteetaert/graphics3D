using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Graphics3D
{
    class Rectangle2D
    {
        #region Class parameters
        List<Point2D> points;
        ArrayList lines = new ArrayList();
        List<double> angle = new List<double>();
        #endregion

        #region Class constructors
        public Rectangle2D() { }
        /// <summary>
        /// Creat e anew Rectangle2D
        /// </summary>
        /// <param name="p1">Top left corner</param>
        /// <param name="p2">top right corner</param>
        /// <param name="p3">bottom right corner</param>
        /// <param name="p4">bottom left corner</param>
        public Rectangle2D(List<Point2D> points)
        {
            this.points = points;
            for (int i = 0; i < points.Count; i++)
            {
                lines.Add(new Line2D(points[i % points.Count], points[(i + 1) % points.Count]));
            }
            

            //double maxX = -10000, minX = 10000, maxY = -10000, minY = 10000;
            //foreach (Point2D point in points)
            //{
            //    if (point.X > maxX)
            //        maxX = point.X;
            //    if (point.X < minX)
            //        minX = point.X;
            //    if (point.Y > maxY)
            //        maxY = point.Y;
            //    if (point.Y < minY)
            //        minY = point.Y;
            //}
            //center = new Point2D((maxX + minX)/2, (maxY + minY)/2);
            //foreach (Point2D point in points)
            //{
            //    if (point.X > center.X && point.Y > center.Y)
            //        point.Angle = (360 - (Math.Atan((point.Y - center.Y) / (point.X - center.X))*180/Math.PI));
            //    else if (point.X < center.X && point.Y > center.Y)
            //        point.Angle = ((Math.Atan((point.Y - center.Y) / (center.X - point.X)) * 180 / Math.PI) + 270);
            //    else if (point.X < center.X && point.Y < center.Y)
            //        point.Angle = (180 - (Math.Atan((center.Y - point.Y) / (center.X - point.X)) * 180 / Math.PI));
            //    else
            //        point.Angle = (Math.Atan((center.Y - point.Y) / (point.X - center.X)) * 180 / Math.PI);
            //}
            //ArrayList newPoints = new ArrayList();
            
        }
        #endregion

        #region Class properties
        public List<double> Angles { get { return angle; } }
        public ArrayList Lines {  get { return this.lines; } }
        #endregion

        #region Class methods
        public void Draw(Graphics gr, Pen pen)
        {
            foreach (Line2D line in this.lines) { line.Draw(gr, pen); }
        }
        public void Fill(Graphics gr, Brush brush)
        {
            Point[] pointA = new Point[points.Count];
            int i = 0;
            foreach (Point2D point in this.points)
            {
                pointA[i] = new Point((int)point.X, (int)point.Y);
                i++;
            }
            gr.FillPolygon(brush, pointA);
        }
        public bool Contains(Point2D point)
        {
            return true;//shape.Contains(new PointF((float)point.X, (float)point.Y));
        }
        #endregion
    }
}
