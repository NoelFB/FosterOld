using System;

namespace Foster.Framework
{
    /// <summary>
    /// A Struct for managing Masks
    /// </summary>
    public struct Mask
    {

        public const ulong All = 0xFFFFFFFFFFFFFFFF;
        public const ulong None = 0;

        public ulong Value;

        public Mask(ulong value)
        {
            Value = value;
        }

        public void Add(Mask mask)
        {
            Value |= mask.Value;
        }

        public void Remove(Mask mask)
        {
            Value &= ~mask.Value;
        }

        public bool Has(Mask mask)
        {
            return (Value & mask.Value) > 0;
        }

        public static Mask Make(int index)
        {
            if (index < 0 || index > 63)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 63");

            return new Mask((ulong)(1 << index));
        }

        public static implicit operator ulong(Mask mask) => mask.Value;
        public static implicit operator Mask(ulong val) => new Mask(val);

    }
}
