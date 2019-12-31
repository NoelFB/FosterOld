using System;

namespace Foster.Framework
{
    public struct Tag
    {

        public const uint Any = 0xFFFFFFFF;

        public uint Mask;

        public Tag(uint mask)
        {
            Mask = mask;
        }

        public void Add(Tag tag)
        {
            Mask |= tag.Mask;
        }

        public void Remove(Tag tag)
        {
            Mask &= ~tag.Mask;
        }

        public bool Has(Tag tag)
        {
            return (Mask & tag.Mask) > 0;
        }

        public static Tag Make(int index)
        {
            if (index < 0 || index > 31)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 31");

            return new Tag((uint)(1 << index));
        }

        public static implicit operator uint(Tag tag) => tag.Mask;
        public static implicit operator Tag(uint val) => new Tag(val);

    }
}
