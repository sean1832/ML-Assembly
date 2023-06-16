using System;
using System.Collections.Generic;

namespace TimberAssembly.Entities
{
    /// <summary>
    /// Dimension of an agent
    /// </summary>
    public class Dimension
    {
        /// <summary>
        /// Length of the agent
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Width of the agent
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Height of the agent
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Create a dimension
        /// </summary>
        public Dimension() { }

        /// <summary>
        /// Create a dimension from three doubles
        /// </summary>
        /// <param name="length">Length of the agent</param>
        /// <param name="width">Width of the agent</param>
        /// <param name="height">Height of the agent</param>
        public Dimension(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Create a dimension from a list of doubles. The list must have exactly 3 values.
        /// </summary>
        /// <param name="dimensions">(l, w, h)</param>
        public Dimension(List<double> dimensions)
        {
            if (dimensions.Count != 3)
                throw new ArgumentException($"Dimension take exactly 3 values, but has {dimensions.Count} values.");
            Length = dimensions[0];
            Width = dimensions[1];
            Height = dimensions[2];
        }

        /// <summary>
        /// Convert dimension to list of doubles.
        /// </summary>
        /// <returns>(l, w, h)</returns>
        public List<double> ToList()
        {
            return new List<double> { Length, Width, Height };
        }

        /// <summary>
        /// Convert dimension to string.
        /// </summary>
        /// <returns>(l, w, h)</returns>
        public override string ToString()
        {
            return $"({Length}, {Width}, {Height})";
        }

        /// <summary>
        /// Check if ANY dimension is smaller than the given dimension.
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyLargerThan(Dimension dimension)
        {
            bool larger = Length > dimension.Length ||
                          Width > dimension.Width ||
                          Height > dimension.Height;
            return larger;
        }

        /// <summary>
        /// Check if ANY dimension is larger or equal to the given dimension.
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnyLargerOrEqualThan(Dimension dimension)
        {
            bool largerOrEqual = Length >= dimension.Length ||
                                 Width >= dimension.Width ||
                                 Height >= dimension.Height;
            return largerOrEqual;
        }

        /// <summary>
        /// Check if ANY dimension is smaller than the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnySmallerThan(Dimension dimension)
        {
            bool smaller = Length < dimension.Length ||
                           Width < dimension.Width ||
                           Height < dimension.Height;
            return smaller;
        }

        /// <summary>
        /// Check if ANY dimension is smaller or equal to the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAnySmallerOrEqualThan(Dimension dimension)
        {
            bool smallerOrEqual = Length <= dimension.Length ||
                                  Width <= dimension.Width ||
                                  Height <= dimension.Height;
            return smallerOrEqual;
        }

        /// <summary>
        /// Check if ALL dimensions are larger than the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAllLargerThan(Dimension dimension)
        {
            bool larger = Length > dimension.Length &&
                          Width > dimension.Width &&
                          Height > dimension.Height;
            return larger;
        }

        /// <summary>
        /// Check if ALL dimensions are larger or equal to the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAllLargerOrEqualThan(Dimension dimension)
        {
            bool largerOrEqual = Length >= dimension.Length &&
                                 Width >= dimension.Width &&
                                 Height >= dimension.Height;
            return largerOrEqual;
        }

        /// <summary>
        /// Check if ALL dimensions are smaller than the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAllSmallerThan(Dimension dimension)
        {
            bool smaller = Length < dimension.Length &&
                           Width < dimension.Width &&
                           Height < dimension.Height;
            return smaller;
        }

        /// <summary>
        /// Check if ALL dimensions are smaller or equal to the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <returns>Comparison result</returns>
        public bool IsAllSmallerOrEqualThan(Dimension dimension)
        {
            bool smallerOrEqual = Length <= dimension.Length &&
                                  Width <= dimension.Width &&
                                  Height <= dimension.Height;
            return smallerOrEqual;
        }

        /// <summary>
        /// Check if ALL dimensions are equal to the given dimension
        /// </summary>
        /// <param name="dimension">Input dimension to compare with</param>
        /// <param name="tolerance">This number cannot be larger than the smallest dimension out of the two</param>
        /// <returns>Comparison result</returns>
        public bool Equality(Dimension dimension, double tolerance = 0.01)
        {
            // todo: add a check for tolerance

            bool equal = Math.Abs(Length - dimension.Length) < tolerance &&
                         Math.Abs(Width - dimension.Width) < tolerance &&
                         Math.Abs(Height - dimension.Height) < tolerance;
            return equal;
        }

        /// <summary>
        /// Get the absolute value of the dimension, that means all values are positive.
        /// </summary>
        /// <returns>Resulted absolute dimension</returns>
        public Dimension Absolute()
        {
            Dimension newDimension = new Dimension(
                Math.Abs(Length),
                Math.Abs(Width),
                Math.Abs(Height)
            );
            return newDimension;
        }

        /// <summary>
        /// Get the volume of the dimension. (l * w * h)
        /// </summary>
        /// <returns>Resulted volume</returns>
        public double GetVolume()
        {
            double volume = Length * Width * Height;
            return volume;
        }

        /// <summary>
        /// Subtract the given dimension from the current dimension.
        /// </summary>
        /// <param name="dimension">Dimension to subtract</param>
        public void Subtract(Dimension dimension)
        {
            Length -= dimension.Length;
            Width -= dimension.Width;
            Height -= dimension.Height;
        }

        /// <summary>
        /// Addition of the given dimension to the current dimension.
        /// </summary>
        /// <param name="dimension">Dimension to add</param>
        public void Add(Dimension dimension)
        {
            Length += dimension.Length;
            Width += dimension.Width;
            Height += dimension.Height;
        }

        /// <summary>
        /// Create a new dimension with all values set to zero.
        /// </summary>
        /// <returns>Created dimension</returns>
        public static Dimension Zero()
        {
            Dimension newDimension = new Dimension(0, 0, 0);
            return newDimension;
        }

        /// <summary>
        /// Get the sum of two dimensions.
        /// </summary>
        /// <param name="dimension1">Dimension 1 to sum</param>
        /// <param name="dimension2">Dimension 2 to sum</param>
        /// <returns>Summed dimension</returns>
        public static Dimension GetSum(Dimension dimension1, Dimension dimension2)
        {
            Dimension newDimension = new Dimension(
                dimension1.Length + dimension2.Length,
                dimension1.Width + dimension2.Width,
                dimension1.Height + dimension2.Height
            );
            return newDimension;
        }

        /// <summary>
        /// Get the sum of multiple dimensions.
        /// </summary>
        /// <param name="dimensions">List of dimension to sum</param>
        /// <returns>Summed dimension</returns>
        public static Dimension GetSum(List<Dimension> dimensions)
        {
            Dimension newDimension = new Dimension(0, 0, 0);
            foreach (Dimension dimension in dimensions)
            {
                newDimension.Add(dimension);
            }
            return newDimension;
        }

        /// <summary>
        /// Get the difference between two dimensions.
        /// </summary>
        /// <param name="dimension1">Dimension 1 to subtract from</param>
        /// <param name="dimension2">Dimension 2 to subtract</param>
        /// <returns>Difference between two dimension</returns>
        public static Dimension GetDifference(Dimension dimension1, Dimension dimension2)
        {
            Dimension newDimension = new Dimension(
                dimension1.Length - dimension2.Length,
                dimension1.Width - dimension2.Width,
                dimension1.Height - dimension2.Height
                );
            return newDimension;
        }

        
    }
}
