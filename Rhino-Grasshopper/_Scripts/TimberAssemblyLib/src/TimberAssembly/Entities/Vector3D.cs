using System;
using System.Collections.Generic;

namespace TimberAssembly.Entities
{
    /// <summary>
    /// <para>Representation of a 3D Vector.</para>
    /// <para>This structure is used throughout TimberAssembly to pass 3D Position, Dimension, Rotation and Scale around.</para>
    /// <para>It also contains functions for doing common vector operations.</para>
    /// </summary>
    public class Vector3D
    {
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Create a Vector3D
        /// </summary>
        public Vector3D() { }

        /// <summary>
        /// Create a Vector3D from it's x, y and z components.
        /// </summary>
        /// <param name="x">X Dimension</param>
        /// <param name="y">Y Dimension</param>
        /// <param name="z">Z Dimension</param>
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create a Vector3D from a list of doubles. The list must have exactly 3 values.
        /// </summary>
        /// <param name="vectors">(x, y, z)</param>
        public Vector3D(List<double> vectors)
        {
            if (vectors.Count != 3)
                throw new ArgumentException($"Vector3D take exactly 3 values, but has {vectors.Count} values.");
            X = vectors[0];
            Y = vectors[1];
            Z = vectors[2];
        }

        /// <summary>
        /// Convert Vector3D to list of doubles.
        /// </summary>
        /// <returns>(x, y, z)</returns>
        public List<double> ToList()
        {
            return new List<double> { X, Y, Z };
        }

        /// <summary>
        /// Convert Vector3D to array of doubles.
        /// </summary>
        /// <returns>(x, y, z)></returns>
        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        /// <summary>
        /// Convert Vector3D to string.
        /// </summary>
        /// <returns>(x, y, z)</returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        /// <summary>
        /// Check if ANY Vector is smaller than the given Vector3D.
        /// </summary>
        /// <param name="vector3D">Input Vector3D to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyGreater(Vector3D vector3D)
        {
            bool larger = X > vector3D.X ||
                          Y > vector3D.Y ||
                          Z > vector3D.Z;
            return larger;
        }

        /// <summary>
        /// Check if ANY Vector is smaller than the given value.
        /// </summary>
        /// <param name="value">Value to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyGreater(double value)
        {
            bool larger = X > value ||
                          Y > value ||
                          Z > value;
            return larger;
        }

        /// <summary>
        /// Check if ANY Vector is larger or equal to the given Vector3D.
        /// </summary>
        /// <param name="vector3D">Input Vector3D to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyGreaterOrEqual(Vector3D vector3D)
        {
            bool largerOrEqual = X >= vector3D.X ||
                                 Y >= vector3D.Y ||
                                 Z >= vector3D.Z;
            return largerOrEqual;
        }

        /// <summary>
        /// Check if ANY Vector is larger or equal to the given value.
        /// </summary>
        /// <param name="value">Value to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyGreaterOrEqual(double value)
        {
            bool largerOrEqual = X >= value ||
                                 Y >= value ||
                                 Z >= value;
            return largerOrEqual;
        }

        /// <summary>
        /// Check if ANY Vector is smaller than the given Vector3D.
        /// </summary>
        /// <param name="vector3D">Input Vector3D to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnySmaller(Vector3D vector3D)
        {
            bool smaller = X < vector3D.X ||
                           Y < vector3D.Y ||
                           Z < vector3D.Z;
            return smaller;
        }

        /// <summary>
        /// Check if ANY Vector is smaller than the given value.
        /// </summary>
        /// <param name="value">Value to compare with</param>
        /// <returns></returns>
        public bool IsAnySmaller(double value)
        {
            bool smaller = X < value ||
                           Y < value ||
                           Z < value;
            return smaller;
        }


        /// <summary>
        /// Check if ANY Vector is smaller or equal to the given Vector3D
        /// </summary>
        /// <param name="vector3D">Input Vector3D to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnySmallerOrEqual(Vector3D vector3D)
        {
            bool smallerOrEqual = X <= vector3D.X ||
                                  Y <= vector3D.Y ||
                                  Z <= vector3D.Z;
            return smallerOrEqual;
        }

        /// <summary>
        /// Check if ANY Vector is smaller or equal to the given value.
        /// </summary>
        /// <param name="value">Value to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnySmallerOrEqual(double value)
        {
            bool smallerOrEqual = X <= value ||
                                  Y <= value ||
                                  Z <= value;
            return smallerOrEqual;
        }

        /// <summary>
        /// Get the absolute value of the Vectors, that means all values are positive.
        /// </summary>
        /// <returns>Resulted absolute Vector3D</returns>
        public Vector3D Absolute()
        {
            Vector3D newVector3D = new Vector3D(
                Math.Abs(X),
                Math.Abs(Y),
                Math.Abs(Z)
            );
            return newVector3D;
        }

        /// <summary>
        /// Get the volume of the Vector3D. (x * y * z)
        /// </summary>
        /// <returns>Resulted volume</returns>
        public double GetVolume()
        {
            double volume = X * Y * Z;
            return volume;
        }

        /// <summary>
        /// Returns true if two vectors are approximately equal with tolerance of `1e-5`.
        /// <para>Recommending using operator `==`.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var tolerance = 1e-5;
            if (obj is Vector3D other)
            {
                return Math.Abs(X - other.X) < tolerance && Math.Abs(Y - other.Y) < tolerance && Math.Abs(Z - other.Z) < tolerance;
            }
            return false;
        }

        /// <summary>
        /// Return a hash code of the Vector3D.
        /// </summary>
        /// <returns>Hashcode of Vector3D</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Create a new Vector3D with all values set to zero.
        /// </summary>
        /// <returns>Created Vector3D</returns>
        public static Vector3D Zero()
        {
            Vector3D newVector3D = new Vector3D(0, 0, 0);
            return newVector3D;
        }

        /// <summary>
        /// Get the sum of multiple Vector3D.
        /// </summary>
        /// <param name="vector3Ds">List of Vector3D to sum</param>
        /// <returns>Summed Vector3D</returns>
        public static Vector3D GetSum(List<Vector3D> vector3Ds)
        {
            Vector3D newVector3D = new Vector3D(0, 0, 0);
            foreach (Vector3D vector in vector3Ds)
            {
                newVector3D += vector;
            }
            return newVector3D;
        }

        /// <summary>
        /// Returns a vector that is made from the smallest components of two vectors.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Min vector</returns>
        public static Vector3D Min(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Min(a.Z, b.Z)
                );
        }

        /// <summary>
        /// Returns a vector that is made from the largest components of two vectors.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Max vector</returns>
        public static Vector3D Max(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y), 
                Math.Max(a.Z, b.Z)
            );
        }



        #region Operators

        /// <summary>
        /// Adds two vectors.
        /// <para>Adds each component of `a` to the corresponding component of `b`.</para>
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Added</returns>
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            Vector3D result = new Vector3D(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z
            );
            return result;
        }

        /// <summary>
        /// Subtracts a vector from another vector.
        /// <para>Subtracts each component of `b` from each component of `a`.</para>
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Subtracted</returns>
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            Vector3D result = new Vector3D(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z
            );
            return result;
        }

        /// <summary>
        /// Multiplies a vector by a number.
        /// <para>Multiplies each component of a by `a` number `b`.</para>
        /// </summary>
        /// <param name="a">Vector to be multiply</param>
        /// <param name="b">Number to multiply</param>
        /// <returns>Multiplied</returns>
        public static Vector3D operator *(Vector3D a, double b)
        {
            Vector3D result = new Vector3D(
                a.X * b,
                a.Y * b,
                a.Z * b);
            return result;
        }


        /// <summary>
        /// Divides a vector by a number.
        /// <para>Divides each component of a by `a` number `b`.</para>
        /// </summary>
        /// <param name="a">Vector to be divide</param>
        /// <param name="b">Number too divide</param>    
        /// <returns>Divided</returns>
        public static Vector3D operator /(Vector3D a, double b)
        {
            Vector3D result = new Vector3D(
                a.X / b,
                a.Y / b,
                a.Z / b);
            return result;
        }


        /// <summary>
        /// Returns true if two vectors are approximately equal.
        /// <para>To allow for double point inaccuracies, the two vectors are considered equal if the magnitude of their difference is less than `1e-5`.</para>
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Equality</returns>
        public static bool operator ==(Vector3D a, Vector3D b)
        {
            if (a == null && b == null) return true;
            var tolerance = 1e-5;

            bool equal = b != null &&
                         a != null &&
                         Math.Abs(a.X - b.X) < tolerance &&
                         Math.Abs(a.Y - b.Y) < tolerance &&
                         Math.Abs(a.Z - b.Z) < tolerance;
            return equal;
        }

        /// <summary>
        /// Returns true if two vectors are not approximately equal.
        /// <para>To allow for double point inaccuracies, the two vectors are considered equal if the magnitude of their difference is less than 1e-5.</para>
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Inequality</returns>
        public static bool operator !=(Vector3D a, Vector3D b)
        {
            if (a == null && b == null) return false;
            var tolerance = 1e-5;

            bool equal = b != null &&
                         a != null &&
                         Math.Abs(a.X - b.X) < tolerance &&
                         Math.Abs(a.Y - b.Y) < tolerance &&
                         Math.Abs(a.Z - b.Z) < tolerance;
            return !equal;
        }

        /// <summary>
        /// Return true if the ALL component of vector `a` is greater than vector `b`.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Is greater than</returns>
        public static bool operator >(Vector3D a, Vector3D b)
        {
            return a.X > b.X && a.Y > b.Y && a.Z > b.Z;
        }

        /// <summary>
        /// Return true if the ALL component of vector `a` is less than vector `b`.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Is less than</returns>
        public static bool operator <(Vector3D a, Vector3D b)
        {
            return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
        }

        /// <summary>
        /// Return true if the ALL component of vector `a` is greater than or equal to vector `b`.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Is greater or equal</returns>
        public static bool operator >=(Vector3D a, Vector3D b)
        {
            return a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;
        }

        /// <summary>
        /// Return true if the ALL component of vector `a` is less than or equal to vector `b`.
        /// </summary>
        /// <param name="a">Vector a</param>
        /// <param name="b">Vector b</param>
        /// <returns>Is less than or equal to</returns>
        public static bool operator <=(Vector3D a, Vector3D b)
        {
            return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
        }
        #endregion
    }
}
