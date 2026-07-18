using System;
using System.Globalization;

namespace FarmFarmer.Core
{
    // break_infinity-style number: mantissa in [1, 10) * 10^exponent.
    // Standard double tops out around 1.8e308 -- idle-game HP/cost/DPS curves blow past that
    // well before endgame, so everything in the economy should route through this instead.
    // Not a `readonly struct`: Unity's serializer (JsonUtility included) silently skips
    // `readonly` fields on deserialize, which would zero this out on every save load.
    [Serializable]
    public struct BigDouble : IComparable<BigDouble>, IEquatable<BigDouble>
    {
        public double Mantissa;
        public long Exponent;

        public static readonly BigDouble Zero = new BigDouble(0d, 0L);
        public static readonly BigDouble One = new BigDouble(1d, 0L);

        public BigDouble(double mantissa, long exponent)
        {
            if (mantissa == 0d)
            {
                Mantissa = 0d;
                Exponent = 0L;
                return;
            }

            var absMantissa = Math.Abs(mantissa);
            var shift = (long)Math.Floor(Math.Log10(absMantissa));
            Mantissa = mantissa / Math.Pow(10d, shift);
            Exponent = exponent + shift;
        }

        public BigDouble(double value) : this(value, 0L)
        {
        }

        public static implicit operator BigDouble(double value) => new BigDouble(value);

        public double ToDouble()
        {
            if (Exponent > 308) return Mantissa > 0 ? double.PositiveInfinity : double.NegativeInfinity;
            if (Exponent < -308) return 0d;
            return Mantissa * Math.Pow(10d, Exponent);
        }

        public static BigDouble Add(BigDouble a, BigDouble b)
        {
            if (a.Mantissa == 0d) return b;
            if (b.Mantissa == 0d) return a;

            BigDouble larger = a.Exponent >= b.Exponent ? a : b;
            BigDouble smaller = a.Exponent >= b.Exponent ? b : a;

            var expDiff = larger.Exponent - smaller.Exponent;
            if (expDiff > 17) return larger; // smaller term is negligible at this scale

            var combinedMantissa = larger.Mantissa + smaller.Mantissa * Math.Pow(10d, -expDiff);
            return new BigDouble(combinedMantissa, larger.Exponent);
        }

        public static BigDouble Subtract(BigDouble a, BigDouble b) => Add(a, Negate(b));

        public static BigDouble Negate(BigDouble a) => new BigDouble(-a.Mantissa, a.Exponent);

        public static BigDouble Multiply(BigDouble a, BigDouble b) =>
            new BigDouble(a.Mantissa * b.Mantissa, a.Exponent + b.Exponent);

        public static BigDouble Divide(BigDouble a, BigDouble b) =>
            new BigDouble(a.Mantissa / b.Mantissa, a.Exponent - b.Exponent);

        public BigDouble Pow(double power)
        {
            // Approximate: fine for economy curves, not intended for exactness at extreme scale.
            var newExponent = Exponent * power;
            var mantissaPow = Math.Pow(Mantissa, power);
            var wholeExponent = (long)Math.Floor(newExponent);
            var fractionalScale = Math.Pow(10d, newExponent - wholeExponent);
            return new BigDouble(mantissaPow * fractionalScale, wholeExponent);
        }

        public static BigDouble operator +(BigDouble a, BigDouble b) => Add(a, b);
        public static BigDouble operator -(BigDouble a, BigDouble b) => Subtract(a, b);
        public static BigDouble operator -(BigDouble a) => Negate(a);
        public static BigDouble operator *(BigDouble a, BigDouble b) => Multiply(a, b);
        public static BigDouble operator /(BigDouble a, BigDouble b) => Divide(a, b);

        public static bool operator ==(BigDouble a, BigDouble b) => a.Equals(b);
        public static bool operator !=(BigDouble a, BigDouble b) => !a.Equals(b);
        public static bool operator <(BigDouble a, BigDouble b) => a.CompareTo(b) < 0;
        public static bool operator >(BigDouble a, BigDouble b) => a.CompareTo(b) > 0;
        public static bool operator <=(BigDouble a, BigDouble b) => a.CompareTo(b) <= 0;
        public static bool operator >=(BigDouble a, BigDouble b) => a.CompareTo(b) >= 0;

        public int CompareTo(BigDouble other)
        {
            if (Mantissa == 0d && other.Mantissa == 0d) return 0;
            if (Mantissa == 0d) return other.Mantissa > 0d ? -1 : 1;
            if (other.Mantissa == 0d) return Mantissa > 0d ? 1 : -1;

            var signA = Math.Sign(Mantissa);
            var signB = Math.Sign(other.Mantissa);
            if (signA != signB) return signA.CompareTo(signB);

            var expCompare = Exponent.CompareTo(other.Exponent);
            return expCompare != 0 ? expCompare * signA : Mantissa.CompareTo(other.Mantissa) * signA;
        }

        public bool Equals(BigDouble other) => Mantissa.Equals(other.Mantissa) && Exponent == other.Exponent;

        public override bool Equals(object obj) => obj is BigDouble other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Mantissa, Exponent);

        public override string ToString()
        {
            if (Mantissa == 0d) return "0";
            if (Exponent > -6 && Exponent < 21) return ToDouble().ToString("0.##", CultureInfo.InvariantCulture);
            return $"{Mantissa.ToString("0.###", CultureInfo.InvariantCulture)}e{Exponent}";
        }
    }
}
