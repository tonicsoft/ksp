using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KspLaunchToLko
{
    public struct Vector2
    {
        public readonly double x;
        public readonly double y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double dot(Vector2 other) => x * other.x + y * other.y;

        public double squareMagnitude() => x * x + y * y;

        public double magnitude() => Math.Sqrt(this.squareMagnitude());

        public static Vector2 operator *(double scalar, Vector2 vector) => new Vector2(scalar * vector.x, scalar * vector.y);

        public static Vector2 operator *(Vector2 vector, double scalar) => scalar * vector;

        public static Vector2 operator /(Vector2 vector, double scalar) => new Vector2(vector.x / scalar, vector.y / scalar);

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x -b.x, a.y - b.y);

        public Vector2 unit() => this / magnitude();
    }
}
