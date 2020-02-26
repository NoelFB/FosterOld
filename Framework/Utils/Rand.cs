using System;
using System.Collections.Generic;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// Random Utilities
    /// </summary>
    public static class Rand
    {
        public static Random Instance = new Random();
        private static readonly Stack<Random> stack = new Stack<Random>();

        public static void Push(int newSeed)
        {
            stack.Push(Instance);
            Instance = new Random(newSeed);
        }

        public static void Push(Random random)
        {
            stack.Push(Instance);
            Instance = random;
        }

        public static void Push()
        {
            stack.Push(Instance);
            Instance = new Random();
        }

        public static void Pop()
        {
            Instance = stack.Pop();
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return min + random.NextFloat() * (max - min);
        }

        public static double NextDouble(this Random random, double max)
        {
            return random.NextDouble() * max;
        }

        public static double NextDouble(this Random random, double min, double max)
        {
            return min + random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        public static int Range(this Random random, int min, int max)
        {
            return min + random.Next(max - min);
        }

        /// <summary>
        /// Returns a random float between min (inclusive) and max (exclusive)
        /// </summary>
        public static float Range(this Random random, float min, float max)
        {
            return min + random.NextFloat(max - min);
        }

        /// <summary>
        /// Returns a random Vector2, and x- and y-values of which are between min (inclusive) and max (exclusive)
        /// </summary>
        public static Vector2 Range(this Random random, Vector2 min, Vector2 max)
        {
            return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
        }

        public static T Choose<T>(this Random random, T a, T b)
        {
            return Calc.GiveMe<T>(random.Next(2), a, b);
        }

        public static T Choose<T>(this Random random, T a, T b, T c)
        {
            return Calc.GiveMe<T>(random.Next(3), a, b, c);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d)
        {
            return Calc.GiveMe<T>(random.Next(4), a, b, c, d);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e)
        {
            return Calc.GiveMe<T>(random.Next(5), a, b, c, d, e);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f)
        {
            return Calc.GiveMe<T>(random.Next(6), a, b, c, d, e, f);
        }

        public static T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        public static T Choose<T>(this Random random, List<T> choices)
        {
            return choices[random.Next(choices.Count)];
        }

        public static T Choose<T>(this Random random, ReadOnlySpan<T> choices)
        {
            return choices[random.Next(choices.Length)];
        }
    }
}
