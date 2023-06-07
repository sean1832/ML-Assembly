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

        public bool IsAnyLargerThan(Dimension dimension)
        {
            bool larger = Length > dimension.Length ||
                          Width > dimension.Width ||
                          Height > dimension.Height;
            return larger;
        }
        public bool IsAnyLargerOrEqualThan(Dimension dimension)
        {
            bool largerOrEqual = Length >= dimension.Length ||
                                 Width >= dimension.Width ||
                                 Height >= dimension.Height;
            return largerOrEqual;
        }

        public bool IsAnySmallerThan(Dimension dimension)
        {
            bool smaller = Length < dimension.Length ||
                           Width < dimension.Width ||
                           Height < dimension.Height;
            return smaller;
        }

        public bool IsAnySmallerOrEqualThan(Dimension dimension)
        {
            bool smallerOrEqual = Length <= dimension.Length ||
                                  Width <= dimension.Width ||
                                  Height <= dimension.Height;
            return smallerOrEqual;
        }

        public bool IsAllLargerThan(Dimension dimension)
        {
            bool larger = Length > dimension.Length &&
                          Width > dimension.Width &&
                          Height > dimension.Height;
            return larger;
        }

        public bool IsAllLargerOrEqualThan(Dimension dimension)
        {
            bool largerOrEqual = Length >= dimension.Length &&
                                 Width >= dimension.Width &&
                                 Height >= dimension.Height;
            return largerOrEqual;
        }

        public bool IsAllSmallerThan(Dimension dimension)
        {
            bool smaller = Length < dimension.Length &&
                           Width < dimension.Width &&
                           Height < dimension.Height;
            return smaller;
        }

        public bool IsAllSmallerOrEqualThan(Dimension dimension)
        {
            bool smallerOrEqual = Length <= dimension.Length &&
                                  Width <= dimension.Width &&
                                  Height <= dimension.Height;
            return smallerOrEqual;
        }

        public bool Equality(Dimension dimension, double tolerance = 0.01)
        {
            bool equal = Math.Abs(Length - dimension.Length) < tolerance &&
                         Math.Abs(Width - dimension.Width) < tolerance &&
                         Math.Abs(Height - dimension.Height) < tolerance;
            return equal;
        }

        public Dimension Absolute()
        {
            Dimension newDimension = new Dimension(
                Math.Abs(Length),
                Math.Abs(Width),
                Math.Abs(Height)
            );
            return newDimension;
        }

        public double GetVolume()
        {
            double volume = Length * Width * Height;
            return volume;
        }

        public void Subtract(Dimension dimension)
        {
            Length -= dimension.Length;
            Width -= dimension.Width;
            Height -= dimension.Height;
        }

        public void Add(Dimension dimension)
        {
            Length += dimension.Length;
            Width += dimension.Width;
            Height += dimension.Height;
        }

        public static Dimension Zero()
        {
            Dimension newDimension = new Dimension(0, 0, 0);
            return newDimension;
        }

        public static Dimension GetSum(Dimension dimension1, Dimension dimension2)
        {
            Dimension newDimension = new Dimension(
                dimension1.Length + dimension2.Length,
                dimension1.Width + dimension2.Width,
                dimension1.Height + dimension2.Height
            );
            return newDimension;
        }

        public static Dimension GetSum(List<Dimension> dimensions)
        {
            Dimension newDimension = new Dimension(0, 0, 0);
            foreach (Dimension dimension in dimensions)
            {
                newDimension.Add(dimension);
            }
            return newDimension;
        }

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
