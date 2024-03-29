﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;

namespace Mapping_Tools_Core.MathUtil {
    /// <summary>
    /// Static utility class for random number generation.
    /// </summary>
    public static class RNG {
        // Base RNG. Maybe expose methods for re-seeding in the future?
        private static readonly Random random = new Random();

        /// <summary>
        /// Returns a non-negative signed integer.
        /// </summary>
        /// <returns>A non-negative signed integer.</returns>
        public static int Next() => random.Next();

        /// <summary>
        /// Returns a signed integer in the range [0,maxValue).
        /// </summary>
        /// <param name="maxValue">The maximum value that should be returned (exclusive, the highest possible result is maxValue - 1).</param>
        /// <returns>A signed integer in the range [0,maxValue).</returns>
        public static int Next(int maxValue) => random.Next(maxValue);

        /// <summary>
        /// Returns a signed integer in the range [minValue,maxValue).
        /// </summary>
        /// <param name="minValue">The minimum value that should be returned (inclusive).</param>
        /// <param name="maxValue">The maximum value that should be returned (exclusive, the highest possible result is maxValue - 1).</param>
        /// <returns>A signed integer in the range [minValue,maxValue).</returns>
        public static int Next(int minValue, int maxValue) => random.Next(minValue, maxValue);

        /// <summary>
        /// Returns a double-precision floating point number in the range [0,1).
        /// </summary>
        /// <returns>A double-precision floating point number in the range [0,1).</returns>
        public static double NextDouble() => random.NextDouble();

        /// <summary>
        /// Returns a double-precision floating point number in the range [0,maxValue).
        /// </summary>
        /// <param name="maxValue">The maximum value that should be returned (exclusive).</param>
        /// <returns>A double-precision floating point number in the range [0,maxValue).</returns>
        public static double NextDouble(double maxValue) {
            if( maxValue < 0.0 )
                throw new ArgumentOutOfRangeException(nameof(maxValue), "The given maximum value must be greater than or equal to 0.");

            return random.NextDouble() * maxValue;
        }

        /// <summary>
        /// Returns a double-precision floating point number in the range [minValue,maxValue).
        /// </summary>
        /// <param name="minValue">The minimum value that should be returned (inclusive).</param>
        /// <param name="maxValue">The maximum value that should be returned (exclusive).</param>
        /// <returns>A double-precision floating point number in the range [minValue,maxValue).</returns>
        public static double NextDouble(double minValue, double maxValue) {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException(nameof(minValue), "The given minimum value must be less than or equal to the given maximum value.");

            return minValue + random.NextDouble() * ( maxValue - minValue );
        }

        /// <summary>
        /// Returns a double-precision floating point number in the range [0,1).
        /// </summary>
        /// <returns>A double-precision floating point number in the range [0,1).</returns>
        public static double NextSingle() => NextDouble();

        /// <summary>
        /// Returns a double-precision floating point number in the range [0,maxValue).
        /// </summary>
        /// <param name="maxValue">The maximum value that should be returned (exclusive).</param>
        /// <returns>A double-precision floating point number in the range [0,maxValue).</returns>
        public static double NextSingle(double maxValue) {
            if( maxValue < 0.0f )
                throw new ArgumentOutOfRangeException(nameof(maxValue), "The given maximum value must be greater than or equal to 0.");

            return NextSingle() * maxValue;
        }

        /// <summary>
        /// Returns a double-precision floating point number in the range [minValue,maxValue).
        /// </summary>
        /// <param name="minValue">The minimum value that should be returned (inclusive).</param>
        /// <param name="maxValue">The maximum value that should be returned (exclusive).</param>
        /// <returns>A double-precision floating point number in the range [minValue,maxValue).</returns>
        public static double NextSingle(double minValue, double maxValue) {
            if( minValue > maxValue )
                throw new ArgumentOutOfRangeException(nameof(minValue), "The given minimum value must be less than or equal to the given maximum value.");

            return minValue + NextSingle() * ( maxValue - minValue );
        }

        /// <summary>
        /// Returns true or false. The likelihood of true and false are determined by trueChance.
        /// </summary>
        /// <param name="trueChance">The chance that the result is true (a value from 0.0 to 1.0).</param>
        /// <returns>True or false with the given probability.</returns>
        public static bool NextBool(double trueChance = 0.5) => NextDouble() < trueChance;

        /// <summary>
        /// Fills the given buffer with random bytes.
        /// </summary>
        /// <param name="buffer">The buffer that should be filled.</param>
        public static void NextBytes(byte[] buffer) => random.NextBytes(buffer);

        /// <summary>
        /// Creates a new byte array with the given length and fills it with random values.
        /// </summary>
        /// <param name="length">The length the byte array should have.</param>
        /// <returns>The newly created byte array.</returns>
        public static byte[] NextBytes(int length) {
            byte[] bytes = new byte[length];
            NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Creates a string with random letters and numbers.
        /// </summary>
        /// <param name="length">The length the string should have.</param>
        /// <returns>The newly created string.</returns>
        public static string RandomString(int length) {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(1, length)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}