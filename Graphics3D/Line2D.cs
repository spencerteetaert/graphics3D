using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics3D
{
    class Line2D
    {
        #region Parameters
        Point2D p1, p2;
        Ball2D[] endPoints = new Ball2D[2];
        #endregion

        #region Constructors
        public Line2D()
        {
            p1 = new Point2D(); p2 = new Point2D();
            endPoints[0] = new Ball2D();
            endPoints[1] = new Ball2D();
        }
        public Line2D(Point2D p1, Point2D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            endPoints[0] = new Ball2D(p1, new Point2D(0, 0), 0.005);
            endPoints[0].Mass = double.MaxValue;
            endPoints[1] = new Ball2D(p2, new Point2D(0, 0), 0.005);
            endPoints[1].Mass = double.MaxValue;
        }
        public Line2D(double x1, double x2, double y1, double y2)
        {
            p1 = new Point2D(x1, y1);
            p2 = new Point2D(x2, y2);
            endPoints[0] = new Ball2D(p1, new Point2D(0, 0), 0.005);
            endPoints[0].Mass = double.MaxValue;
            endPoints[1] = new Ball2D(p2, new Point2D(0, 0), 0.005);
            endPoints[1].Mass = double.MaxValue;
        }
        #endregion

        #region Properties
        public Point2D P1 { get { return p1; } set { p1 = value; } }
        public Point2D P2 { get { return p2; } set { p2 = value; } }
        public double Length
        { get { return (p1 - p2).Magnitude; } }

        #endregion

        #region Methods
        public void Draw(Graphics gr, Pen pen)
        {
            gr.DrawLine(pen, p1.ToPointF, p2.ToPointF);
        }
        /// <summary>
        /// determine the line that is normal from this line to a ball center
        /// </summary>
        /// <param name="ball"></param>
        /// <returns></returns>
        public Point2D NormalToBall(Ball2D ball)
        {
            Point2D v = p1 - p2;
            Point2D ballToLine = ball - p1;
            Point2D normal = v.Normal;
            //make normal a unit vector
            normal.Normalize();
            if (normal * ballToLine < 0)
                normal *= -1;
            return normal;
        }
        /// <summary>
        /// determine if the bounding rectangle of the line contains point p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point2D p)
        {
            RectangleF boundingRect = new RectangleF(
                (float)Math.Min(p1.X, p2.X) - 0.5f,
                (float)Math.Min(p1.Y, p2.Y) - 0.5f,
                (float)Math.Abs(p1.X - p2.X) + 1f,
                (float)Math.Abs(p1.Y - p2.Y) + 1f);
            return boundingRect.Contains(p.ToPointF);
        }
        /// <summary>
        /// determines the point of intersection between this line and another line (otherLine)
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns></returns>
        public Point2D LineIntersectionPoint(Line2D otherLine)
        {
            // Get A,B,C of first line - points : P1 to P2
            double A1 = P2.Y - P1.Y;
            double B1 = P1.X - P2.X;
            double C1 = A1 * P1.X + B1 * P1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            double A2 = otherLine.P2.Y - otherLine.P1.Y;
            double B2 = otherLine.P1.X - otherLine.P2.X;
            double C2 = A2 * otherLine.P1.X + B2 * otherLine.P1.Y;

            // Get delta and check if the lines are parallel
            double delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return new Point2D(float.MaxValue, float.MaxValue);
            //throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Point2D(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }
        /// <summary>
        /// Computes the reflected segment at a point of a curve
        /// The line is the segment being reflected
        /// The normal is the normal of the reflection line and the ball
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Point2D Reflection(Point2D normal)
        {
            double rx, ry;
            Point2D direction = P2 - P1;
            double dot = direction * normal;
            rx = direction.X - 2 * normal.X * dot;
            ry = direction.Y - 2 * normal.Y * dot;
            return new Point2D(rx, ry);
        }

        public bool Bounce(Ball2D ball)
        {
            //determine the normla vector from the line to the ball
            Point2D normal = NormalToBall(ball);
            //make a temporary line of this line moved one radius towards ball (bounce off center)
            Line2D aLineTemp = new Line2D(P1 + normal * ball.Radius, p2 + normal * ball.Radius);

            //we need to kmow where the ball will be in one step
            Point2D ballNextStep = ball + ball.Velocity;
            //make a line from where the ball is now to the location at the next step
            Line2D ballPath = new Line2D(ball, ballNextStep);

            //find point of intersection between line and path of the ball
            Point2D intersectionPoint = aLineTemp.LineIntersectionPoint(ballPath);
            //perfrom the bounce if necessary
            //bounce off endpoints if necessary
            if (endPoints[0].IsColliding(ball))
            {
                endPoints[0].PerformCollision(ball);
                return true;
            }
            else if (endPoints[1].IsColliding(ball))
            {
                endPoints[1].PerformCollision(ball);
                return true;
            }
            //if the intersection point is within line segment
            //and the ball is moving towards the line
            else if (ball.Velocity.Magnitude < (intersectionPoint - ball).Magnitude && normal * ball.Velocity < 0)
                return false;
            else if (aLineTemp.Contains(intersectionPoint) && normal * ball.Velocity < 0)
            {
                Line2D reflectionLine = new Line2D(ball + ball.Velocity, intersectionPoint);
                Point2D velocityDirection = -1 * reflectionLine.Reflection(NormalToBall(ball));
                velocityDirection.Normalize();
                ball.Velocity = velocityDirection * ball.Velocity.Magnitude * ball.Elasticity;
                Point2D ballLocation = intersectionPoint - reflectionLine.Reflection(NormalToBall(ball)) * ball.Elasticity;
                ball.X = ballLocation.X;
                ball.Y = ballLocation.Y;
                return true;
            }
            return false;
        }
        public void Rotate(double theta)
        {
            p1.Rotate(theta);
            p2.Rotate(theta);
        }
        public void RotateDeg(double angle)
        {
            Rotate(angle * Math.PI / 180);
        }
        #endregion
    }
}
