using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace Graphics3D
{
    class Ball2D : Circle2D
        {
            #region Class parameters
            Point2D velocity = new Point2D();
            Color penColor = Color.White;
            Color brushColor = Color.Black;
            Pen pen = new System.Drawing.Pen(Color.White);
            Brush brush = new SolidBrush(Color.Black);

            //ball physics
            double mass = 1.0;
            double elasticity = .3;
            #endregion

            #region Class constructors
            public Ball2D() { }
            public Ball2D(Point2D center, Point2D velocity, double radius)
            {
                this.Radius = radius;
                this.velocity = velocity;
                this.X = center.X;
                this.Y = center.Y;
                this.mass = .004 * Math.Pow(radius, 2);
            }
            #endregion

            #region Class properties
            public Color Brush
            {
                set { brushColor = value; brush = new SolidBrush(brushColor); }
            }
            public Color Pen
            {
                set { penColor = value; pen = new System.Drawing.Pen(penColor); }
            }
            public double Mass { get { return mass; } set { this.mass = value; } }
            public Point2D Velocity { get { return velocity; } set { this.velocity = value; } }
            public double Elasticity { get { return this.elasticity; } }
            #endregion

            #region Class methods
            public void Draw(Graphics gr)
            {
                Draw(gr, penColor);
            }
            public void Fill(Graphics gr)
            {
                Fill(gr, brushColor);
            }
            public void Move(Size ClientSize)
            {
                this.X += velocity.X;
                this.Y += velocity.Y;
                if (mass != double.PositiveInfinity)
                    velocity *= 1 - .005 * mass / 5;
            }
            /// <summary>
            /// Check to see if this ball is colliding with another ball
            /// </summary>
            /// <param name="otherBall"></param>
            /// <returns></returns>
            public bool IsColliding(Ball2D otherBall)
            {
                Point2D distance = this - otherBall;
                return (distance.Magnitude < this.Radius + otherBall.Radius);
            }
            /// <summary>
            /// Perform the collision between two ball objects, this and other ball
            /// </summary>
            /// <param name="otherBall"></param>
            public void PerformCollision(Ball2D otherBall)
            {
                //nothing to do if not colliding
                if (!IsColliding(otherBall))
                    return;
                Point2D difference = this - otherBall;
                double distance = difference.Magnitude;
                //minimum translation distance
                //fudge by a small factor of 1.1 to force them to move apart by at least a slight gap
                Point2D mtd = difference * (this.Radius + otherBall.Radius - distance) / distance * 1.1;

                //get the reciprocal of the masses
                double thisMassReciprocal = 1 / mass;
                double otherMassReciprocal = 1 / otherBall.Mass;

                //push the balls apart by the minimum translation difference
                Point2D center = mtd * (thisMassReciprocal / (thisMassReciprocal + otherMassReciprocal));
                this.X += center.X;
                this.Y += center.Y;

                Point2D otherBallCenter = mtd * (otherMassReciprocal / (thisMassReciprocal + otherMassReciprocal));
                otherBall.X -= otherBallCenter.X;
                otherBall.Y -= otherBallCenter.Y;

                //now we "normalize" the mtd to get a unit vector of length 1 in the direction of the mtd
                mtd.Normalize();

                //impact the velocity due to the collision
                Point2D v = this.velocity - otherBall.velocity;
                double vDotMtd = v * mtd;
                if (double.IsNaN(vDotMtd))
                    return;
                if (vDotMtd > 0)
                    return; //the balls are already moving in opposite directions

                //work out collision effect
                double i = -(1 + elasticity) * vDotMtd / (thisMassReciprocal + otherMassReciprocal);
                Point2D impulse = mtd * i;

                //change the balls velocities
                this.velocity += impulse * thisMassReciprocal;
                otherBall.velocity -= impulse * otherMassReciprocal;

            }
            /// <summary>
            /// Bounce off the bounds of a ball inside the rectangle
            /// </summary>
            /// <param name="bounds"></param>
            public void Bounce(RectangleF bounds)
            {

                    if (X - Radius < bounds.Left)
                    {
                        X += (bounds.Left - X + Radius) * 2;
                        velocity.X *= -1;
                        velocity *= elasticity;
                    }
                    if (Y - Radius < bounds.Top)
                    {
                        Y += (bounds.Top - Y + Radius) * 2;
                        velocity.Y *= -1;
                        velocity *= elasticity;
                    }
                    if (X + Radius > bounds.Right)
                    {
                        X -= (X - bounds.Right + Radius) * 2;
                        velocity.X *= -1;
                        velocity *= elasticity;
                    }
                    if (Y + Radius > bounds.Bottom)
                    {
                        Y -= (Y - bounds.Bottom + Radius) * 2;
                        velocity.Y *= -1;
                        velocity *= elasticity;
                    }
                
            }
            #endregion
        } 
}
