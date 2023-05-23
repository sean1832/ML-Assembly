﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimberAssembly.Component
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

        public static Dimension Absolute(Dimension dimension)
        {
            Dimension newDimension = new Dimension(
                               Math.Abs(dimension.Length),
                               Math.Abs(dimension.Width),
                               Math.Abs(dimension.Height)
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