using System;
using System.Collections.Generic;

namespace TimberAssembly.Entities
{
    public class Dimension
    {

        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Dimension() { }
        public Dimension(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }

        public Dimension(List<double> dimensions)
        {
            Length = dimensions[0];
            Width = dimensions[1];
            Height = dimensions[2];
        }


        public List<double> ToList()
        {
            return new List<double> { Length, Width, Height };
        }

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
