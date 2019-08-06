using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CustomControls
{
    public struct Vector : IFormattable
    {
        private double x;
        private double y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public double Length
        {
            get { return Math.Sqrt(this.x * this.x + this.y * this.y); }
        }

        public double LengthSquared
        {
            get { return this.x * this.x + this.y * this.y; }
        }

        public void Normalize()
        {
            double maxMagnitude = Math.Abs(this.x);
            double yMagnitude = Math.Abs(this.y);
            if (yMagnitude > maxMagnitude)
            {
                maxMagnitude = yMagnitude;
            }
            this.x /= maxMagnitude;
            this.y /= maxMagnitude;
            double length = Math.Sqrt(this.x * this.x + this.y * this.y);
            this.x /= length;
            this.y /= length;
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double radians;
            vector1.Normalize();
            vector2.Normalize();
            if (DotProduct(vector1, vector2) < 0)
            {
                Vector v = -vector1 - vector2;
                radians = Math.PI - 2 * Math.Asin(v.Length / 2);
            }
            else
            {
                Vector w = vector1 - vector2;
                radians = 2 * Math.Asin(w.Length / 2);
            }
            return radians * 180 / Math.PI;
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.x, -vector.y);
        }

        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x + vector2.x, vector1.y + vector2.y);
        }

        public static Vector Add(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x + vector2.x, vector1.y + vector2.y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x - vector2.x, vector1.y - vector2.y);
        }

        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x - vector2.x, vector1.y - vector2.y);
        }

        public static Vector Subtract(Point point1, Point point2)
        {
            return new Vector(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static Point operator +(Vector vector, Point point)
        {
            return new Point(vector.x + point.X, vector.y + point.Y);
        }

        public static Point operator +(Point point, Vector vector)
        {
            return new Point(vector.x + point.X, vector.y + point.Y);
        }

        public static Point Add(Vector vector, Point point)
        {
            return new Point(vector.x + point.X, vector.y + point.Y);
        }

        public static Point Add(Point point, Vector vector)
        {
            return new Point(vector.x + point.X, vector.y + point.Y);
        }

        public static Point operator -(Vector vector, Point point)
        {
            return new Point(vector.x - point.X, vector.y - point.Y);
        }

        public static Point Subtract(Vector vector, Point point)
        {
            return new Point(vector.x - point.X, vector.y - point.Y);
        }

        public static Point operator -(Point point, Vector vector)
        {
            return new Point(point.X - vector.x, point.Y - vector.y);
        }

        public static Point Subtract(Point point, Vector vector)
        {
            return new Point(point.X - vector.x, point.Y - vector.y);
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector Multiply(Vector vector, double scalar)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector Multiply(double scalar, Vector vector)
        {
            return new Vector(vector.x * scalar, vector.y * scalar);
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return vector * (1 / scalar);
        }

        public static Vector Divide(Vector vector, double scalar)
        {
            return vector * (1 / scalar);
        }

        public static Vector operator *(Vector vector, Matrix matrix)
        {
            return new Vector(matrix.M11 * vector.x + matrix.M12 * vector.y, matrix.M21 * vector.x + matrix.M22 * vector.y);
        }

        public static Vector Multiply(Vector vector, Matrix matrix)
        {
            return new Vector(matrix.M11 * vector.x + matrix.M12 * vector.y, matrix.M21 * vector.x + matrix.M22 * vector.y);
        }

        public static double DotProduct(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.x + vector1.y * vector2.y;
        }

        public static double CrossProduct(Vector vector1, Vector vector2)
        {
            return vector1.x * vector2.y - vector1.y * vector2.x;
        }

        public static explicit operator Point(Vector vector)
        {
            return new Point(vector.x, vector.y);
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.x == vector2.x && vector1.y == vector2.y;
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool Equals(Vector vector1, Vector vector2)
        {
            return vector1.x.Equals(vector2.x) && vector1.y.Equals(vector2.y);
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Vector))
            {
                return false;
            }
            Vector vector = (Vector)o;
            return Equals(this, vector);
        }

        public bool Equals(Vector value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode();
        }

        public override string ToString()
        {
            return this.ConvertToString(null, null);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ConvertToString(null, provider);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return this.ConvertToString(format, provider);
        }

        private string ConvertToString(string format, IFormatProvider provider)
        {
            const char numericListSeparator = ',';
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", numericListSeparator, this.x, this.y);
        }
    }
}
