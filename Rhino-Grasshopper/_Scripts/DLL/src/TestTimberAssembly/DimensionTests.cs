using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberAssembly.Entities;

namespace TestTimberAssembly
{
    [TestFixture]
    public class DimensionTests
    {
        private readonly double _tolerance = 0.1;

        [Test]
        public void Test_Addition_ReturnsCorrectDimension_WhenPassedTwoDimensions()
        {
            var dimension1 = new Dimension(5, 5, 5);

            var dimension2 = new Dimension(10, 10, 10);

            var expected = new Dimension(15, 15, 15);

            var result = Dimension.GetSum(dimension1, dimension2);

            Assert.That(Math.Abs(expected.Height - result.Height) < 0.001);
            Assert.That(Math.Abs(expected.Length - result.Length) < 0.001);
            Assert.That(Math.Abs(expected.Width - result.Width) < 0.001);
        }

        [Test]
        public void Test_Subtract_ReturnsCorrectDimension_WhenPassedTwoDimensions()
        {
            var dimension1 = new Dimension(100, 100, 100);

            var dimension2 = new Dimension(20, 20, 20);

            var expected = new Dimension(80, 80, 80);

            var result = Dimension.GetDifference(dimension1, dimension2);

            Assert.That(Math.Abs(expected.Height - result.Height) < 0.001);
            Assert.That(Math.Abs(expected.Length - result.Length) < 0.001);
            Assert.That(Math.Abs(expected.Width - result.Width) < 0.001);
        }

        [Test]
        public void Test_Equality_ReturnsTrue_WhenDimensionsMatch()
        {
            var dimension1 = new Dimension(10, 10, 10);

            var dimension2 = new Dimension(10, 10, 10);

            var result = dimension1.Equality(dimension2, _tolerance);

            Assert.True(result);
        }

        [Test]
        public void Test_Equality_ReturnsFalse_WhenDimensionsDoNotMatch()
        {
            var dimension1 = new Dimension(10, 20, 10);

            var dimension2 = new Dimension(10, 10, 10);

            var result = dimension1.Equality(dimension2, _tolerance);

            Assert.False(result);
        }
    }
}
