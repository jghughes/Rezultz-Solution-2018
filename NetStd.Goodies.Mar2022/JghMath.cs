using System;

namespace NetStd.Goodies.Mar2022
{
    /// <summary>
    /// Believe it or not, math functions in System.math do not work as advertised at the time of writing in March 2022.
    /// Hence the need for 
    /// </summary>
    public static class JghMath
    {
        #region round

        //public static double Round(double value, int digits = 0)
        //{

        //    return System.Math.Round(value, digits);
        //}

        private const double DoubleRoundLimit = 1e16d;

        private const int MaxRoundingDigits = 15;

        // This table is required for the Round function which can specify the number of digits to round to
        private static readonly double[] RoundPower10Double =
        [
            1E0, 1E1, 1E2, 1E3, 1E4, 1E5, 1E6, 1E7, 1E8,
            1E9, 1E10, 1E11, 1E12, 1E13, 1E14, 1E15
        ];

        public static double Round(double value, int digits = 0)
        {
            if (digits < 0 || digits > MaxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits),
                    "number of rounding digits must be positive and less than 16");

            return InternalRound(value, digits);
        }

        private static double InternalRound(double value, int digits)
        {
            var sign = Sign(value);

            value = Abs(value);

            if (!(value < DoubleRoundLimit))
                return value;

            var power10 = RoundPower10Double[digits];

            value *= power10;

            var fraction = value - (int)value;

            if (fraction > 0.5)
                value += 1;
            var valueAsTruncatedInt = (int)value;

            value = valueAsTruncatedInt;

            value /= power10;

            value *= sign;

            return value;
        }

        #endregion

        #region sign

        public static int Sign(long value)
        {
            return value switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        }
        public static int Sign(double value)
        {
            return value switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        }

        #endregion

        #region abs

        public static long Abs(long value)
        {
            if (value >= 0)
                return value;
            return -value;
        }

        public static double Abs(double value)
        {
            if (value >= 0)
                return value;
            return -value;
        }

        #endregion

        #region max

        public static int Max(int val1, int val2)
        {
            return val1 >= val2 ? val1 : val2;
        }

        public static long Max(long val1, long val2)
        {
            return val1 >= val2 ? val1 : val2;
        }
        public static double Max(double val1, double val2)
        {
            if (val1 > val2)
                return val1;

            return double.IsNaN(val1) ? val1 : val2;
        }

        public static decimal Max(decimal val1, decimal val2)
        {
            return val1 > val2 ? val1 : val2;
        }
        #endregion

        #region min

        public static int Min(int val1, int val2)
        {
            return val1 <= val2 ? val1 : val2;
        }

        public static long Min(long val1, long val2)
        {
            return val1 <= val2 ? val1 : val2;
        }

        public static double Min(double val1, double val2)
        {
            return val1 < val2 ? val1 : val2;
        }

        public static decimal Min(decimal val1, decimal val2)
        {
            return val1 < val2 ? val1 : val2;
        }

        #endregion

        #region GuardAgainstDivisionByZero

        private const double Epsilon = 0.09999999999;

        /// <summary>Guards against division by zero.</summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="specifiedDenominator">The specified denominator.</param>
        /// <param name="overrideIfDenominatorIsCloseToZero">
        ///     The override denominator if the delta between the specified
        ///     denominator and zero is smaller than Epsilon, where Epsilon is 0.099 recurring.
        /// </param>
        /// <returns></returns>
        public static double GuardAgainstDivisionByZero(double numerator, double specifiedDenominator, double overrideIfDenominatorIsCloseToZero)
        {
            if (Abs(specifiedDenominator - 0.0) < Epsilon)
                return overrideIfDenominatorIsCloseToZero;

            return numerator / specifiedDenominator;
        }

        #endregion
    }
}