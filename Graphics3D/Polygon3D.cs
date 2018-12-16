using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Graphics3D
{
    class Polygon3D : IComparable<Polygon3D>
    {
        ArrayList lines = new ArrayList();
        List<Point3D> vertices = new List<Point3D>();
        Brush frontBr = Brushes.Purple, backBr = Brushes.Yellow;
        Color frontColor = Color.DarkRed, backColor = Color.Blue;
        Size clientSize;
        Cube parent = null;
        bool selected;

        #region Constructor
        public Polygon3D(Size clientSize, Cube parent)
        {
            this.clientSize = clientSize;
            this.parent = parent;
        }
        public Polygon3D(ArrayList pts, Size clientSize)
        {
            foreach (Point3D p in pts)
                vertices.Add(p);
            this.clientSize = clientSize;
        }
        #endregion

        #region Properties
        public Brush FrontBrush { set { frontBr = value; } }
        public Brush BackBrush { set { backBr = value; } }
        public Color FrontColor { set { frontColor = value; } get { return frontColor; } }
        public Color BackColor { set { backColor = value; } get { return backColor; } }
        public List<Point3D> Vertices { get { return vertices; } }
        public Cube Cube { get { return parent; } }
        public bool Flag { get { return selected; } set { selected = value; } }
        #endregion

        #region Methods
        public void AddPoint(Point3D p) { vertices.Add(p); }
        public void MakeLines()
        {
            lines.Clear();
            for (int i = 0; i < vertices.Count; i++)
                lines.Add(new Line3D((Point3D)vertices[i], (Point3D)vertices[(i + 1) % vertices.Count]));
        }

        public void Rotate(Point3D angle, Point3D center)
        {
            foreach (Point3D p in vertices)
            {
                Point3D temp = p - center;
                temp.Rotate(angle);
                p.X = temp.X;
                p.Y = temp.Y;
            }
            MakeLines();
        }
        public void RotateDeg(Point3D angle, Point3D center)
        {
            Rotate(angle * Math.PI / 180, center);
        }
        /// <summary>
        /// Rotate the polygon about the origin
        /// </summary>
        /// <param name="angle">The angle of rotation</param>
        public void Rotate(Point3D angle)
        {
            foreach (Point3D p in vertices)
            {
                p.Rotate(angle);
            }
            MakeLines();
        }

        public void RotateDeg(Point3D angle)
        {
            Rotate(angle * Math.PI / 180);
        }
        public void UnRotate(Point3D angle)
        {
            foreach (Point3D p in vertices)
            {
                p.UnRotate(angle);
            }
            MakeLines();
        }
        public void UnRotateDeg(Point3D angle)
        {
            UnRotate(angle * Math.PI / 180);
        }

        public void Draw(Graphics gr, Pen pen, double distance)
        {
            Polygon2D polyProjected = Projection(distance);
            // draw the 2D polygon
            if (polyProjected != null)
            polyProjected.Draw(gr, pen);
        }
        public void Draw(Graphics gr, double distance, Face whichFace, Point3D lightSource)
        {
            Polygon2D polyProjected = Projection(distance);

            if (polyProjected != null)
            {
                if (polyProjected.Face != whichFace)
                    return;
                Color tempColor = (whichFace == Face.front) ? frontColor : backColor;

                int reflectivity = Reflectivity(lightSource);
                int alpha = (whichFace == Face.front) ? alpha = reflectivity : 255 - reflectivity;
                alpha = 255;
                Brush brush = new SolidBrush(Color.FromArgb(alpha, tempColor));

                double z = 0;
                foreach (Point3D p in vertices)
                    if (p.Z > z)
                        z = p.Z;

                if (polyProjected.IsInScreen(clientSize) && z < distance)
                {
                    //polyProjected.Fill(gr, Brushes.Black);
                    if (this.frontColor != Color.Transparent)
                        polyProjected.Fill(gr, brush);
                    polyProjected.Draw(gr, Pens.Gray);
                }
            }
        }
        public void Fill(Graphics gr, double distance)
        {
            Polygon2D polyProjected = Projection(distance);
            // draw the 2D polygon
            // determine the brush to use based on the projected
            // 2D polygon Face
            if (polyProjected.Face == Face.front)
                polyProjected.Fill(gr, frontBr);
            else
                polyProjected.Fill(gr, backBr);
        }
        private int Reflectivity(Point3D lightSource)
        {
            Point3D v1 = vertices[1] - vertices[0];
            Point3D v2 = vertices[2] - vertices[0];
            Point3D normal = v1 ^ v2;

            double dotProduct = (normal * lightSource) / (lightSource.Magnitude * normal.Magnitude);

            dotProduct = (dotProduct + 1) / 2;

            return (int)(dotProduct * 255);
        }
        private Polygon2D Projection(double distance)
        {
            Polygon2D polyProjected = new Polygon2D();
            // project each point into 2D and add it to the 2D polygon
            foreach (Point3D p in vertices)
                polyProjected.AddPoint(p.Projection(distance));
            // construct the 2D polygon's lines
            polyProjected.MakeLines();

            if (polyProjected.IsInScreen(clientSize))
                return polyProjected;
            else
                return null;
        }
        /// <summary>
        /// Scales the polygon by the amount of the factor
        /// </summary>
        /// <param name="factor"></param>
        public void Scale(double factor)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] *= factor;
            MakeLines();
        }
        public void UnScale(double factor)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] /= factor;
        }
        /// <summary>
        /// Shift a polygon by the shift amount
        /// </summary>
        /// <param name="shift"></param>
        public void Translate(Point3D shift, bool fromCubeCreator)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (!fromCubeCreator)
                    vertices[i].UnRotateDeg(Global.TotalAngle);
                vertices[i] += shift;
                vertices[i].RotateDeg(Global.TotalAngle);
            }
        }
        public Point3D Center()
        {
            Point3D ret = new Point3D();
            foreach (Point3D p in vertices)
                ret += p;
            ret /= vertices.Count();
            return ret;
        }
        public Cube MakeNewCube(double size, List<Cube> cubes, List<Polygon3D> floor)
        {
            Cube ret = new Cube(clientSize); //click to add cube grrrrr

            UnRotateDeg(Global.TotalAngle);
            UnScale(size);
            Point3D thisCenter = new Point3D();
            thisCenter = this.Center();
            Scale(size);
            RotateDeg(Global.TotalAngle);

            Point3D cubeCenter = new Point3D();
            cubeCenter = thisCenter;
            //cubeCenter.Y += .25;

            bool flag = false;
            if (cubes.Count > 0)
                foreach (Cube c in cubes)
                {
                    if (c.Contains(this))
                    {
                        c.UnScale(size);
                        c.UnRotateDeg(Global.TotalAngle);
                        cubeCenter = c.Center();
                        c.RotateDeg(Global.TotalAngle);
                        c.Scale(size);
                        flag = true;
                        continue;
                    }
                }
            if (!flag)
            {
                if (Projection(Global.Distance).Face == Face.front)
                    cubeCenter.Y += .5;
                else
                    cubeCenter.Y -= .5;
            }

            Point3D translation = thisCenter;

            translation = (thisCenter - cubeCenter) * 2 + cubeCenter;

            ret.Scale(size);
            ret.Translate(translation * size, true);
            //ret.RotateDeg(Global.TotalAngle);

            return ret;
        }
   
        public void DeleteCube(List<Cube> cubes, List<Polygon3D> floor)
        {
            Cube flag = new Cube(clientSize);

            foreach (Cube c in cubes)
            {
                if (c.Contains(this))
                {
                    flag = c; //add floor tile after deleting cube
                    continue;
                }
            }
            if (floor.Contains(this))

            cubes.Remove(flag);
            foreach (Polygon3D side in flag.Sides)
                Global.AllSides.Remove(side);
        }
        #endregion

        #region Comparer
        /// <summary>
        /// Retruns te distance between the polygon and the viewer
        /// </summary>
        /// <returns></returns>
        private double getDepth()
        {
            //finds center of polygon
            Point3D avg = new Point3D();
            foreach (Point3D p in vertices)
                avg += p;
            avg /= vertices.Count;

            double xPlaneHyp, yPlaneHyp;
            //calculates the distance between the center point and the view point (0, 0, distance)
            xPlaneHyp = Math.Sqrt(Math.Pow(Global.Distance - avg.Z, 2) + Math.Pow(avg.X, 2));
            yPlaneHyp = Math.Sqrt(Math.Pow(xPlaneHyp, 2) + Math.Pow(avg.Y, 2));

            return yPlaneHyp * -1;
        }
        public int CompareTo(Polygon3D otherPolygon)
        {
            if (this.getDepth() > otherPolygon.getDepth())
                return 1;
            else if (this.getDepth() == otherPolygon.getDepth())
                return 0;
            else
                return -1;
        }
        public bool Contains(Point2D mouseLocation)
        {
            if (Projection(Global.Distance) != null)
                return Projection(Global.Distance).IsPointInPolygon(mouseLocation);
            else
                return false;
        }
        #endregion
    }
}
