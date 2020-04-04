using System;

namespace Foster.Framework
{
    /// <summary>
    /// Blend Operations
    /// </summary>
    public enum BlendOperations : byte
    {
        Add,
        Subtract,
        ReverseSubtract,
        Min,
        Max
    }

    /// <summary>
    /// Blend Factors
    /// </summary>
    public enum BlendFactors : byte
    {
        Zero,
        One,
        SrcColor,
        OneMinusSrcColor,
        DstColor,
        OneMinusDstColor,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        ConstantColor,
        OneMinusConstantColor,
        ConstantAlpha,
        OneMinusConstantAlpha,
        SrcAlphaSaturate,
        Src1Color,
        OneMinusSrc1Color,
        Src1Alpha,
        OneMinusSrc1Alpha
    }

    public enum BlendMask : byte
    {
        None    = 0,
        Red     = 1,
        Green   = 2,
        Blue    = 4,
        Alpha   = 8,
        RGB     = Red | Green | Blue,
        RGBA    = Red | Green | Blue | Alpha,
    }

    /// <summary>
    /// Blend Mode
    /// </summary>
    public struct BlendMode
    {
        public BlendOperations ColorOperation;
        public BlendFactors ColorSource;
        public BlendFactors ColorDestination;
        public BlendOperations AlphaOperation;
        public BlendFactors AlphaSource;
        public BlendFactors AlphaDestination;
        public BlendMask Mask;
        public Color Color;

        public BlendMode(BlendOperations operation, BlendFactors source, BlendFactors destination)
        {
            ColorOperation = AlphaOperation = operation;
            ColorSource = AlphaSource = source;
            ColorDestination = AlphaDestination = destination;
            Mask = BlendMask.RGBA;
            Color = Color.White;
        }

        public BlendMode(
            BlendOperations colorOperation, BlendFactors colorSource, BlendFactors colorDestination, 
            BlendOperations alphaOperation, BlendFactors alphaSource, BlendFactors alphaDestination, 
            BlendMask mask, Color color)
        {
            ColorOperation = colorOperation;
            ColorSource = colorSource;
            ColorDestination = colorDestination;
            AlphaOperation = alphaOperation;
            AlphaSource = alphaSource;
            AlphaDestination = alphaDestination;
            Mask = mask;
            Color = color;
        }

        public static readonly BlendMode Normal = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.OneMinusSrcAlpha);
        public static readonly BlendMode Add = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.DstAlpha);
        public static readonly BlendMode Subtract = new BlendMode(BlendOperations.ReverseSubtract, BlendFactors.One, BlendFactors.One);
        public static readonly BlendMode Multiply = new BlendMode(BlendOperations.Add, BlendFactors.DstColor, BlendFactors.OneMinusSrcAlpha);
        public static readonly BlendMode Screen = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.OneMinusSrcColor);

        public static bool operator ==(BlendMode a, BlendMode b)
        {
            return
                a.ColorOperation == b.ColorOperation &&
                a.ColorSource == b.ColorSource &&
                a.ColorDestination == b.ColorDestination &&
                a.AlphaOperation == b.AlphaOperation &&
                a.AlphaSource == b.AlphaSource &&
                a.AlphaDestination == b.AlphaDestination &&
                a.Mask == b.Mask &&
                a.Color == b.Color;
        }

        public static bool operator !=(BlendMode a, BlendMode b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            return (obj is BlendMode mode) && (this == mode);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ColorOperation,
                ColorSource,
                ColorDestination,
                AlphaOperation,
                AlphaSource,
                AlphaDestination,
                Mask,
                Color);
        }
    }
}
