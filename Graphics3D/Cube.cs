using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    class Cube : Form1
    {
        #region Cube Variables
        List<Polygon3D> sides = new List<Polygon3D>();
        Point3D min = new Point3D(-.5, -.5, -.5);
        Point3D max = new Point3D(.5, .5, .5);
        Point3D location = new Point3D(); //center of cube - start at origin
        Color inside = Color.Red, outside = Color.Blue;
        Size clientSize;
        #endregion

        #region Constructors
        public Cube(Size clientSize)
        {
            this.clientSize = clientSize;
            CreateFaces();
        }
        public Cube(Point3D translation, double scale, Size clientSize)
        {
            this.clientSize = clientSize;
            CreateFaces();
            Translate(translation, false);
            Scale(scale);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get all the sides of the cube
        /// </summary>
        public List<Polygon3D> Sides { get { return sides; } }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the 6 faces of the cube
        /// </summary>
        private void CreateFaces()
        {
            Polygon3D aFace = new Polygon3D(clientSize, this);
            //front face
            aFace.AddPoint(new Point3D(min.X, min.Y, max.Z));
            aFace.AddPoint(new Point3D(max.X, min.Y, max.Z));
            aFace.AddPoint(new Point3D(max.X, max.Y, max.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, max.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);

            //back face
            aFace = new Polygon3D(clientSize, this);
            aFace.AddPoint(new Point3D(max.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(min.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, min.Z));
            aFace.AddPoint(new Point3D(max.X, max.Y, min.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);

            //right face
            aFace = new Polygon3D(clientSize, this);
            aFace.AddPoint(new Point3D(max.X, min.Y, max.Z));
            aFace.AddPoint(new Point3D(max.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(max.X, max.Y, min.Z));
            aFace.AddPoint(new Point3D(max.X, max.Y, max.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);

            //left face
            aFace = new Polygon3D(clientSize, this);
            aFace.AddPoint(new Point3D(min.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(min.X, min.Y, max.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, max.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, min.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);

            //top face
            aFace = new Polygon3D(clientSize, this);
            aFace.AddPoint(new Point3D(min.X, min.Y, max.Z));
            aFace.AddPoint(new Point3D(min.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(max.X, min.Y, min.Z));
            aFace.AddPoint(new Point3D(max.X, min.Y, max.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);

            //bottom face
            aFace = new Polygon3D(clientSize, this);
            aFace.AddPoint(new Point3D(max.X, max.Y, max.Z));
            aFace.AddPoint(new Point3D(max.X, max.Y, min.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, min.Z));
            aFace.AddPoint(new Point3D(min.X, max.Y, max.Z));
            aFace.FrontBrush = new SolidBrush(outside);
            aFace.BackBrush = new SolidBrush(inside);
            sides.Add(aFace);
        }
        /// <summary>
        /// Size the cube according to the scaling factor
        /// </summary>
        /// <param name="factor"></param>
        public void Scale(double factor)
        {
            foreach(Polygon3D side in sides)
            {
                side.Scale(factor);
            }
        }
        /// <summary>
        /// Rotate the cube by a spcific angle
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(Point3D angle)
        {
            foreach (Polygon3D side in sides)
            {
                side.Rotate(angle);
            }
        }
        public void RotateDeg(Point3D angle)
        {
            Rotate(angle * Math.PI / 180);
        }
        public void UnRotate(Point3D angle)
        {
            foreach (Polygon3D side in sides)
            {
                side.UnRotate(angle);
            }
        }
        public void UnRotateDeg(Point3D angle)
        {
            UnRotate(angle * Math.PI / 180);
        }

        public void Translate(Point3D shift, bool fromCubeCreator)
        {
            foreach (Polygon3D side in sides)
            {
                side.Translate(shift, fromCubeCreator);
            }
        }
        public void Draw(Graphics gr, double distance)
        {
            sides.Sort();
            foreach (Polygon3D side in sides)
            {
                side.Fill(gr, distance);
                side.Draw(gr, Pens.White, distance);
            }
        }
        public Point3D Center()
        {
            Point3D ret = new Point3D();
            ret += sides[0].Center() + sides[1].Center();
            ret /= 2;
            return ret;
        }
        public bool Contains(Polygon3D face)
        {
            foreach (Polygon3D s in sides)
            {
                if (s == face)
                    return true;
            }
            return false;
        }
        public void UnScale(double factor)
        {
            foreach (Polygon3D s in sides)
            {
                s.UnScale(factor);
            }
        }
        #endregion
    }
}
