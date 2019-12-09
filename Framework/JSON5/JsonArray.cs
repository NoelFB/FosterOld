using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public class JsonArray : IJsonValue, IList<IJsonValue>
    {
        public JsonType Type => JsonType.Array;

        public IJsonValue this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(IJsonValue item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IJsonValue item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IJsonValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IJsonValue> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(IJsonValue item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IJsonValue item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IJsonValue item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
