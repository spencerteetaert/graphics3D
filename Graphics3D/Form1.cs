using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics3D
{
    enum Face { front, back };
    public partial class Form1 : Form
    {
        //axis lines
        List<Line3D> axis = new List<Line3D>();
        //the change in angle - reset every tick
        Point3D angle = new Point3D(0, 0, 0),
            lightSource = new Point3D(-1000, -1000, 1000);
        Point2D mouseLastPos = new Point2D(), mouseDown = new Point2D();
        Random ran = new Random();
        List<Cube> cubes = new List<Cube>(), selectedCubes = new List<Cube>();
        List<Polygon3D> floor = new List<Polygon3D>();
        bool canPlace = true, drawPlane = true, drawRectangle = false, shift = false;
        MouseEventArgs mouseD = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        Rectangle selectionBox = new Rectangle();
        double size = 100;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //creates the floor surface
            for (int j = -15; j < 15; j++)
            {
                for (int i = -15; i < 15; i++)
                {
                    Polygon3D aSide = new Polygon3D(this.ClientSize, null);
                    aSide.AddPoint(new Point3D(i * size, 0, j * size));
                    aSide.AddPoint(new Point3D(i * size, 0, j * size + size));
                    aSide.AddPoint(new Point3D(i * size + size, 0, j * size + size));
                    aSide.AddPoint(new Point3D(i * size + size, 0, j * size));
                    aSide.BackColor = Color.Transparent;
                    aSide.FrontColor = Color.Transparent;

                    floor.Add(aSide);
                }
            }
            //foreach (Cube cube in cubes)
            //    Global.AllSides.AddRange(cube.Sides);

            Global.AllSides.AddRange(floor);

            axis.Add(new Line3D(new Point3D(-25, 0, 0), new Point3D(1000, 0, 0)));
            axis.Add(new Line3D(new Point3D(0, 25, 0), new Point3D(0, -1000, 0)));
            axis.Add(new Line3D(new Point3D(0, 0, -25), new Point3D(0, 0, 1000)));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Dispose();
                    break;
                //case Keys.Space:
                //    timer1.Enabled = !timer1.Enabled;
                //    break;
                //case Keys.S:
                //    timer1_Tick(null, null);
                //    break;
                case Keys.X:
                    if (e.Modifiers == Keys.Shift)
                        angle.X = -3;
                    else
                        angle.X = 3;
                    break;
                case Keys.Y:
                    if (e.Modifiers == Keys.Shift)
                        angle.Y = -3;
                    else
                        angle.Y = 3;
                    break;
                case Keys.Z:
                    if (e.Modifiers == Keys.Shift)
                        angle.Z = -3;
                    else
                        angle.Z = 3;
                    break;
                //Ctrl + D to deselect
                case Keys.D:
                    if (e.Modifiers == Keys.Control)
                        UnselectAll();
                    break;
                //R for birds-eye view
                case Keys.R:
                    foreach (Cube cube in cubes)
                        cube.UnRotateDeg(Global.TotalAngle);
                    foreach (Polygon3D plane in floor)
                        plane.UnRotateDeg(Global.TotalAngle);
                    foreach (Line3D a in axis)
                        a.UnRotateDeg(Global.TotalAngle);
                    Global.TotalAngle = new Point3D();
                    angle.X = 90;
                    break;
                //zoom in/out
                case Keys.OemPeriod:
                    if (size * 1.1 < 500)
                    {
                        foreach (Cube cube in cubes)
                            cube.Scale(1.1);
                        foreach (Polygon3D plane in floor)
                            plane.Scale(1.1);
                        size *= 1.1;
                    }
                    break;
                case Keys.Oemcomma:
                    if (size / 1.1 > 1)
                    {
                        foreach (Cube cube in cubes)
                            cube.Scale(1 / 1.1);
                        foreach (Polygon3D plane in floor)
                            plane.Scale(1 / 1.1);
                        size /= 1.1;
                    }
                    break;
                //shifts selected cubes
                case Keys.Left:
                    foreach (Cube cube in selectedCubes)
                        cube.Translate(new Point3D(-size, 0, 0), false);
                    break;
                case Keys.Right:
                    foreach (Cube cube in selectedCubes)
                        cube.Translate(new Point3D(size, 0, 0), false);
                    break;
                case Keys.Up:
                    if (e.Modifiers != Keys.Shift)
                        foreach (Cube cube in selectedCubes)
                            cube.Translate(new Point3D(0, 0, -size), false);
                    else
                        foreach (Cube cube in selectedCubes)
                            cube.Translate(new Point3D(0, -size, 0), false);
                    break;
                case Keys.Down:
                    if (e.Modifiers != Keys.Shift)
                        foreach (Cube cube in selectedCubes)
                            cube.Translate(new Point3D(0, 0, size), false);
                    else
                        foreach (Cube cube in selectedCubes)
                            cube.Translate(new Point3D(0, size, 0), false);
                    break;
                //hides the plane/axis'
                case Keys.H:
                    if (drawPlane)
                        foreach (Polygon3D f in floor)
                            Global.AllSides.Remove(f);
                    else
                        Global.AllSides.AddRange(floor);
                    Global.AllSides.Sort();
                    drawPlane = !drawPlane;
                    break;
                //deletes selected cubes
                case Keys.Delete:
                    foreach (Cube c in selectedCubes)
                    {
                        cubes.Remove(c);
                        foreach (Polygon3D item in c.Sides)
                            Global.AllSides.Remove(item);

                        c.Dispose();
                    }
                    break;
                //ctrl + A = select all
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        UnselectAll();
                        foreach (Cube item in cubes)
                        {
                            selectedCubes.Add(item);
                            foreach (Polygon3D side in item.Sides)
                                side.Flag = true;
                        }
                    }
                    break;
                case Keys.ShiftKey:
                        shift = true;
                        break;
                //control box toggle
                case Keys.OemQuestion:
                    if (groupBox1.Visible)
                    {
                        groupBox1.Hide();
                        label1.Show();
                    }
                    else
                    {
                        groupBox1.Show();
                        label1.Hide();
                    }
                    break;
                //Shows an info box
                case Keys.I:
                    AboutBox1 aboutBox = new AboutBox1();
                    aboutBox.ShowDialog();
                    break;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //checks relative x-location (to last location) to determine a rotation angle on mouse move
            if (mouseD.Button == MouseButtons.Left)
            {
                angle.X = (e.Y - mouseLastPos.Y > 1) ? angle.X = 8 : (e.Y - mouseLastPos.Y < -1) ? angle.X = -8 : angle.X = 0;
                angle.Y = (e.X - mouseLastPos.X > 1) ? angle.Y = -8 : (e.X - mouseLastPos.X < -1) ? angle.Y = 8 : angle.Y = 0;
                if (angle.X > 0 || angle.Y > 0)
                    canPlace = false;
            }

            //toggles drawRectangle if right click
            if (mouseD.Button == MouseButtons.Right)
                drawRectangle = true;

            mouseLastPos = new Point2D(e.Location.X, e.Location.Y);
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseD = e;
            mouseDown = new Point2D(e.Location.X, e.Location.Y);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            //checks for shift - clears selection if not pressed
            if (!shift && e.Button != MouseButtons.Left)
                UnselectAll();
            //if selecting, call the select function
            if (drawRectangle)
                SelectionBoxContains();

            mouseD = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
            bool send = false;
            if (e.Button == MouseButtons.Left)
                send = true;

            //calls a checkClick for all faces
            if (canPlace)
                CheckClick(new Point2D(e.X - ClientSize.Width / 2, e.Y - ClientSize.Height / 2), send);

            drawRectangle = false;
            canPlace = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ShiftKey:
                    {
                        shift = false;
                        break;
                    }
            }
        }

        public void SelectionBoxContains()
        {

            foreach (Polygon3D side in Global.AllSides)
            {
                //filters out floor plane
                if (floor.Contains(side))
                    continue;
                foreach (Point3D vert in side.Vertices)
                {
                    //checks if the polygon contains mousePos & it's corresponding cube isn't already selected
                    if (selectionBox.Contains((int)vert.Projection(Global.Distance).ToPointF.X, (int)vert.Projection(Global.Distance).ToPointF.Y)
                        && !selectedCubes.Contains(side.Cube))
                    {
                        selectedCubes.Add(side.Cube); //add entire cube to selected list
                        foreach (Polygon3D item in side.Cube.Sides)
                            item.Flag = true;
                    }
                }
            }
        }
        /// <summary>
        /// Clears box selection
        /// </summary>
        public void UnselectAll()
        {
            selectedCubes.Clear();
            foreach (Polygon3D item in Global.AllSides)
                item.Flag = false;
        }
        private void CheckClick(Point2D mouseLocation, bool leftClick)
        {
            //reorders sides to check the face you see on top as first
            Global.AllSides.Sort();
            Global.AllSides.Reverse();
            Polygon3D flag = new Polygon3D(this.ClientSize, null);

            foreach (Polygon3D side in Global.AllSides)
            {
                if (side.Contains(mouseLocation))
                {
                    //if left clicking on a face
                    if (leftClick)
                    {
                        //adds a new cube
                        Cube a = side.MakeNewCube(size, cubes, floor);
                        cubes.Add(a);
                        Global.AllSides.AddRange(a.Sides);
                        break;
                    }
                    //if right clicking on a face
                    else
                    {
                        //checks if shift is held and selection box isn't being drawn
                        if (shift && (mouseDown - mouseLastPos).Magnitude < 5)
                            flag = side;
                        break;
                    }
                }
            }
            flag.DeleteCube(cubes, floor);
            //reorders sides to draw
            Global.AllSides.Sort();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.TranslateTransform(ClientSize.Width / 2, ClientSize.Height / 2);
            //label1.Text = Graphics3D.Global.Distance.ToString();

            Global.AllSides.Sort();
            for (int i = 0; i < Global.AllSides.Count; i++)
            {
                //draws all faces
                Global.AllSides[i].Draw(gr, Graphics3D.Global.Distance, Face.back, lightSource);
                Global.AllSides[i].Draw(gr, Graphics3D.Global.Distance, Face.front, lightSource);
                //draws a white frame around selected cubes
                if (Global.AllSides[i].Flag)
                    Global.AllSides[i].Draw(gr, Pens.White, Global.Distance);
            }

            if (drawPlane)
            {
                //draws axis'
                axis[0].Draw(gr, Pens.Red, Global.Distance);
                axis[1].Draw(gr, Pens.Blue, Global.Distance);
                axis[2].Draw(gr, Pens.Green, Global.Distance);
                //Sphere origin = new Sphere(0, 0, 0, 1);
                //origin.Draw(gr, Color.White, Global.Distance);
            }

            if (drawRectangle)
            {
                //draws a selection rectanlge
                selectionBox = FindBox(mouseDown.X - ClientSize.Width / 2, mouseDown.Y - ClientSize.Height / 2, mouseLastPos.X - ClientSize.Width / 2, mouseLastPos.Y - ClientSize.Height / 2);
                gr.DrawRectangle(Pens.White, selectionBox);
            }
        }
        /// <summary>
        /// Allows the selection box to be drawn in any direction
        /// </summary>
        /// <param name="x1">Mousedown X</param>
        /// <param name="y1">Mousedown Y</param>
        /// <param name="x2">Current X</param>
        /// <param name="y2">Current Y</param>
        /// <returns></returns>
        public Rectangle FindBox(double x1, double y1, double x2, double y2)
        {
            double cornerX = Math.Min(x1, x2);
            double cornerY = Math.Min(y1, y2);
            double width = Math.Max(x1, x2) - cornerX;
            double height = Math.Max(y1, y2) - cornerY;
            return new Rectangle((int)cornerX, (int)cornerY, (int)width, (int)height);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Cube cube in cubes)
                cube.UnRotateDeg(Global.TotalAngle);
            foreach (Polygon3D plane in floor)
                plane.UnRotateDeg(Global.TotalAngle);
            foreach (Line3D a in axis)
                a.UnRotateDeg(Global.TotalAngle);

            //if (Global.TotalAngle.X + angle.X < 270 & Global.TotalAngle.X + angle.X > 180)
            Global.TotalAngle.X += angle.X;
            //if (Global.TotalAngle.Y + angle.Y < 90 & Global.TotalAngle.Y + angle.Y > -90)
            Global.TotalAngle.Y += angle.Y;
            Global.TotalAngle.Z += angle.Z;

            angle = new Point3D();
            foreach (Cube cube in cubes)
                cube.RotateDeg(Global.TotalAngle);
            foreach (Polygon3D plane in floor)
                plane.RotateDeg(Global.TotalAngle);
            foreach (Line3D a in axis)
                a.RotateDeg(Global.TotalAngle);
            //repaint
            this.Invalidate();
        }
    }
    class Global
    {
        //I created this global class to easily store and edit whole system variables, as oppose to sending them in functions all the time

        //the hypothetical distance of the viewers eyes from the screen
        private static double distance = 1000;
        //the total change in angle from the start
        private static Point3D totalAngle = new Point3D();
        //a list of every side that exists. This includes both the grid and every cube
        private static List<Polygon3D> allSides = new List<Polygon3D>();
        public static double Distance { get { return distance; } set { distance = value; } }
        public static Point3D TotalAngle { get { return totalAngle; } set { totalAngle = value; } }
        public static List<Polygon3D> AllSides { get { return allSides; } set { allSides = value; } }
    }
}
