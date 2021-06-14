using System;
using Chess.Extensions;

namespace Chess.Model
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point()
        {
        }

        public int X { get; }
        public int Y { get; }

        protected bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p2 is not null && p1 is not null && p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p2 is not null && p1 is not null && (p1.X != p2.X || p1.Y != p2.Y);
        }

        public bool IsPointOutsideBoard()
        {
            if (X > 7 || X < 0) return true;
            return Y > 7 || Y < 0;
        }

        public static Point GetPointByNotation(string pos)
        {
            var x = Notation.GetPositionByLetter(pos[0].ToString());
            var y = Convert.ToInt32(pos[1].ToString()) - 1;
            return new Point(y, x);
        }

        public override string ToString()
        {
            return X + "," + Y;
        }

        public double Distance(Point p)
        {
            var v = Math.Sqrt(Math.Pow(p.X - X, 2) + Math.Pow(p.Y - Y, 2));
            return v;
        }

        public string ToNotation()
        {
            var str = string.Empty;
            str += Notation.GetLetterByPosition(Y);
            str += X + 1;
            return str;
        }
    }
}