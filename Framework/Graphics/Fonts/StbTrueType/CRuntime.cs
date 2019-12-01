using System;
using System.Runtime.InteropServices;

namespace StbTrueTypeSharp
{
    internal static unsafe class CRuntime
    {
        public static void* malloc(ulong size)
        {
            return malloc((long)size);
        }

        public static void* malloc(long size)
        {
            var ptr = Marshal.AllocHGlobal((int)size);

            return ptr.ToPointer();
        }

        public static void memcpy(void* a, void* b, long size)
        {
            var ap = (byte*)a;
            var bp = (byte*)b;
            for (long i = 0; i < size; ++i)
                *ap++ = *bp++;
        }

        public static void memcpy(void* a, void* b, ulong size)
        {
            memcpy(a, b, (long)size);
        }

        public static void free(void* a)
        {
            var ptr = new IntPtr(a);
            Marshal.FreeHGlobal(ptr);
        }

        public static void memset(void* ptr, int value, long size)
        {
            var bptr = (byte*)ptr;
            var bval = (byte)value;
            for (long i = 0; i < size; ++i)
                *bptr++ = bval;
        }

        public static void memset(void* ptr, int value, ulong size)
        {
            memset(ptr, value, (long)size);
        }

        public static double pow(double a, double b)
        {
            return Math.Pow(a, b);
        }

        public static float fabs(double a)
        {
            return (float)Math.Abs(a);
        }

        public static double ceil(double a)
        {
            return Math.Ceiling(a);
        }


        public static double floor(double a)
        {
            return Math.Floor(a);
        }

        public static double cos(double value)
        {
            return Math.Cos(value);
        }

        public static double acos(double value)
        {
            return Math.Acos(value);
        }

        public static double sin(double value)
        {
            return Math.Sin(value);
        }

        public static double sqrt(double val)
        {
            return Math.Sqrt(val);
        }

        public static double fmod(double x, double y)
        {
            return x % y;
        }

        public static ulong strlen(sbyte* str)
        {
            var ptr = str;

            while (*ptr != '\0')
                ptr++;

            return (ulong)ptr - (ulong)str - 1;
        }
    }
}