using System;
using System.Collections.Generic;

namespace TimberAssembly.Entities
{
    public class Dimension
    {

        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Dimension(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }

        public List<double> ToList()
        {
            return new List<double> { Length, Width, Height };
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


        public static Dimension Zero()
        {
            Dimension newDimension = new Dimension(0, 0, 0);
            return newDimension;
        }

        public static Dimension Addition(Dimension dimension1, Dimension dimension2)
        {
            Dimension newDimension = new Dimension(
                dimension1.Length + dimension2.Length,
                dimension1.Width + dimension2.Width,
                dimension1.Height + dimension2.Height
            );
            return newDimension;
        }

        public static Dimension Subtract(Dimension dimension1, Dimension dimension2)
        {
            Dimension newDimension = new Dimension(
                dimension1.Length - dimension2.Length,
                dimension1.Width - dimension2.Width,
                dimension1.Height - dimension2.Height
                );
            return newDimension;
        }

        

        public static bool Equality(Dimension dimension1, Dimension dimension2, double tolerance)
        {
            bool equal = Math.Abs(dimension1.Length - dimension2.Length) < tolerance &&
                         Math.Abs(dimension1.Width - dimension2.Width) < tolerance &&
                         Math.Abs(dimension1.Height - dimension2.Height) < tolerance;
            return equal;
        }
    }
}
