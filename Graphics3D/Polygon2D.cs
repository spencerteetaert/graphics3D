using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace Graphics3D
{
    class Polygon2D
    {
        ArrayList lines = new ArrayList();
        List<Point2D> vertices = new List<Point2D>();

        #region Constructor
        public Polygon2D() { }
        public Polygon2D(ArrayList pts)
        {
            foreach (Point2D p in pts)
                vertices.Add(p);
            MakeLines();
        }
        #endregion

        #region Properties
        public Face Face
        {
            get
            {
                if (vertices.Count < 3) return Face.front;
                // calculate, from two vectors, the cross-product
                Point2D v1 = vertices[1] - vertices[0];
                Point2D v2 = vertices[2] - vertices[0];
                if (v1.X * v2.Y - v1.Y * v2.X > 0)
                    return Face.front;
                else
                    return Face.back;
            }
        }
        #endregion

        #region Methods
        public void AddPoint(Point2D p) { vertices.Add(p); }
        public void MakeLines()
        {
            lines.Clear();
            for (int i = 0; i < vertices.Count; i++)
                lines.Add(new Line2D((Point2D)vertices[i], (Point2D)vertices[(i + 1) % vertices.Count]));
        }
        /// <summary>
        /// Rotate the polygon around a given point
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        public void Rotate(double angle, Point2D center)
        {
            foreach(Point2D p in vertices)
            {
                Point2D temp = p - center;
                temp.Rotate(angle);
                p.X = temp.X + center.X;
                p.Y = temp.Y + center.Y;
            }
            MakeLines();
        }
        /// <summary>
        /// Rotate Polygon around the origin
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(double angle)
        {
            foreach (Point2D p in vertices)
            {
                p.Rotate(angle);
            }
            MakeLines();
        }
        public void UnRotate(double angle)
        {
            foreach (Point2D p in vertices)
            {
                p.UnRotate(angle);
            }
            MakeLines();
        }
        public void Bounce(Ball2D ball)
        {
            foreach (Line2D line in lines)
                line.Bounce(ball);
        }

        public void Draw(Graphics gr, Pen pen)
        {
            PointF[] pts = new PointF[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                pts[i] = ((Point2D)vertices[i]).ToPointF;
            gr.DrawPolygon(pen, pts);
        }
        public void Fill(Graphics gr, Brush br)
        {
            PointF[] pts = new PointF[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                pts[i] = ((Point2D)vertices[i]).ToPointF;
            gr.FillPolygon(br, pts);
        }
        public bool IsPointInPolygon(Point2D point)
        {
            int polygonLength = vertices.Count, i = 0;
            bool inside = false;
            // x, y for tested point.
            double pointX = point.X, pointY = point.Y;
            // start / end point for the current polygon segment.
            double startX, startY, endX, endY;
            Point2D endPoint = vertices[polygonLength - 1];
            endX = endPoint.X;
            endY = endPoint.Y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = vertices[i++];
                endX = endPoint.X; endY = endPoint.Y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }
        public bool IsInScreen(Size clientSize)
        {
            int count = 0;
            foreach (Point2D p in vertices)
            {
                if (p.X + clientSize.Width / 2 < 0
                    || p.X - clientSize.Width / 2 > 0
                    || p.Y + clientSize.Height / 2 < 0
                    || p.Y - clientSize.Height / 2 > 0)
                    count++;
            }
            return (count == 4) ? false : true;
        }
        #endregion
    }
}
