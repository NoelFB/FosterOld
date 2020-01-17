using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foster.GuiSystem
{
    public struct Size
    {
        public enum Modes
        {
            Preferred,
            Fill,
            Explicit
        }

        public float UpTo;
        public float Min;
        public float Max;
        public Modes Mode;

        public static Size FillUpTo(float upTo, float min = 0, float max = float.MaxValue)
        {
            return new Size
            {
                UpTo = upTo,
                Min = min,
                Max = max,
                Mode = Modes.Fill
            };
        }

        public static Size Fill(float min = 0, float max = float.MaxValue)
        {
            return new Size
            {
                UpTo = 0,
                Min = min,
                Max = max,
                Mode = Modes.Fill
            };
        }

        public static Size Preferred(float min = 0, float max = float.MaxValue)
        {
            return new Size
            {
                UpTo = 0,
                Min = min,
                Max = max,
                Mode = Modes.Preferred
            };
        }

        public static Size Explicit(float value)
        {
            return new Size
            {
                UpTo = 0,
                Min = value,
                Max = value,
                Mode = Modes.Explicit
            };
        }

    }
}
