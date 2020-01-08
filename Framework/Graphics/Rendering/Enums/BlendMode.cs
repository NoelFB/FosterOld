using System;

namespace Foster.Framework
{
    /// <summary>
    /// Blend Operations
    /// </summary>
    public enum BlendOperations
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
    public enum BlendFactors
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

    /// <summary>
    /// Blend Mode
    /// </summary>
    public struct BlendMode
    {
        public BlendOperations Operation;
        public BlendFactors Source;
        public BlendFactors Destination;

        public BlendMode(BlendOperations operation, BlendFactors source, BlendFactors destination)
        {
            Operation = operation;
            Source = source;
            Destination = destination;
        }

        public static readonly BlendMode Normal = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.OneMinusSrcAlpha);
        public static readonly BlendMode Add = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.DstAlpha);
        public static readonly BlendMode Subtract = new BlendMode(BlendOperations.ReverseSubtract, BlendFactors.One, BlendFactors.One);
        public static readonly BlendMode Multiply = new BlendMode(BlendOperations.Add, BlendFactors.DstColor, BlendFactors.OneMinusSrcAlpha);
        public static readonly BlendMode Screen = new BlendMode(BlendOperations.Add, BlendFactors.One, BlendFactors.OneMinusSrcColor);

        public static bool operator ==(BlendMode a, BlendMode b)
        {
            return a.Operation == b.Operation && a.Source == b.Source && a.Destination == b.Destination;
        }

        public static bool operator !=(BlendMode a, BlendMode b)
        {
            return a.Operation != b.Operation || a.Source != b.Source || a.Destination != b.Destination;
        }

        public override bool Equals(object? obj)
        {
            return obj is BlendMode mode &&
                   Operation == mode.Operation &&
                   Source == mode.Source &&
                   Destination == mode.Destination;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Operation, Source, Destination);
        }
    }
}
