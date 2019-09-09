using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public struct Tag
    {
        
        public const uint Any = 0xFFFFFFFF;

        public uint Flag;

        public Tag(uint flag)
        {
            Flag = flag;
        }

        public void Add(uint flag) => Flag |= flag;
        public void Remove(uint flag) => Flag &= ~flag;
        public bool Has(uint flag) => (Flag & flag) > 0;

        public static Tag Make(int index)
        {
            if (index < 0 || index > 31)
                throw new Exception("Index must be between 0 and 31");

            return new Tag((uint)(1 << index));
        }

        public static implicit operator uint(Tag tag) => tag.Flag;
        public static implicit operator Tag(uint val) => new Tag(val);

    }
}
