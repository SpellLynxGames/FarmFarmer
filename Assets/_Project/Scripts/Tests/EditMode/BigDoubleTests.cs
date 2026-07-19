using NUnit.Framework;
using UnityEngine;
using FarmFarmer.Core;

namespace FarmFarmer.Tests
{
    // BigDouble is the number type everything routes through -- if it's wrong, every balance
    // number in the game is wrong. These pin down the behaviors the rest of the code relies on.
    public class BigDoubleTests
    {
        private const double Tolerance = 1e-9;

        [Test]
        public void Constructor_NormalizesMantissaIntoDecade()
        {
            var value = new BigDouble(1234d);
            Assert.AreEqual(1.234d, value.Mantissa, Tolerance);
            Assert.AreEqual(3L, value.Exponent);
        }

        [Test]
        public void Constructor_ZeroStaysCanonical()
        {
            var value = new BigDouble(0d, 50L);
            Assert.AreEqual(BigDouble.Zero, value);
        }

        [Test]
        public void Add_SameScale()
        {
            var sum = new BigDouble(2d, 10L) + new BigDouble(3d, 10L);
            Assert.AreEqual(5d, sum.Mantissa, Tolerance);
            Assert.AreEqual(10L, sum.Exponent);
        }

        [Test]
        public void Add_NegligibleSmallerTermIsDropped()
        {
            // expDiff > 17 is beyond double precision -- the small term must vanish, not corrupt.
            var sum = new BigDouble(1d, 20L) + BigDouble.One;
            Assert.AreEqual(new BigDouble(1d, 20L), sum);
        }

        [Test]
        public void Subtract_CrossingZeroGoesNegative()
        {
            var result = new BigDouble(5d) - new BigDouble(8d);
            Assert.AreEqual(-3d, result.ToDouble(), Tolerance);
        }

        [Test]
        public void Multiply_AddsExponents()
        {
            var product = new BigDouble(2d, 100L) * new BigDouble(4d, 200L);
            Assert.AreEqual(8d, product.Mantissa, Tolerance);
            Assert.AreEqual(300L, product.Exponent);
        }

        [Test]
        public void Divide_RenormalizesBelowOne()
        {
            // 1e400 / 2 = 0.5e400 -> must renormalize to 5e399, not keep a sub-1 mantissa.
            var quotient = new BigDouble(1d, 400L) / new BigDouble(2d);
            Assert.AreEqual(5d, quotient.Mantissa, Tolerance);
            Assert.AreEqual(399L, quotient.Exponent);
        }

        [Test]
        public void Pow_HandlesFractionalExponentCarry()
        {
            // (2e10)^2 = 4e20 -- exercises the fractional-exponent path in Pow.
            var result = new BigDouble(2d, 10L).Pow(2d);
            Assert.AreEqual(4d, result.Mantissa, 1e-6);
            Assert.AreEqual(20L, result.Exponent);
        }

        [Test]
        public void Comparisons_RespectExponentAndSign()
        {
            Assert.IsTrue(new BigDouble(9d, 5L) < new BigDouble(1d, 6L));
            Assert.IsTrue(new BigDouble(1d, 300L) > new BigDouble(9.99d, 299L));
            Assert.IsTrue(new BigDouble(-1d, 10L) < BigDouble.Zero);
            Assert.IsTrue(BigDouble.Zero < BigDouble.One);
        }

        [Test]
        public void ToString_UsesPlainNotationInHumanRange_ScientificBeyond()
        {
            Assert.AreEqual("1500", new BigDouble(1500d).ToString());
            Assert.AreEqual("1.5e400", new BigDouble(1.5d, 400L).ToString());
        }

        [Test]
        public void JsonUtility_RoundTripsThroughSerializableWrapper()
        {
            // BigDouble fields ride inside [Serializable] classes (SaveData) -- this is the exact
            // mechanism saves use, so it has to survive a round trip beyond double range.
            var wrapper = new Wrapper { value = new BigDouble(6.25d, 420L) };
            var json = JsonUtility.ToJson(wrapper);
            var back = JsonUtility.FromJson<Wrapper>(json);
            Assert.AreEqual(wrapper.value, back.value);
        }

        [System.Serializable]
        private class Wrapper
        {
            public BigDouble value;
        }
    }
}
