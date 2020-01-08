using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework.Json
{
    /// <summary>
    /// A data structure encapsulating a Json Array
    /// </summary>
    public class JsonArray : JsonValue<List<JsonValue>>, IList<JsonValue>
    {
        public JsonArray() : base(JsonType.Array, new List<JsonValue>())
        {
        }

        public JsonArray(IList<string> list) : base(JsonType.Array, new List<JsonValue>())
        {
            foreach (var value in list)
                Value.Add(value);
        }

        public JsonValue this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }

        public int Count => Value.Count;

        public bool IsReadOnly => false;

        public void Add(JsonValue item) => Value.Add(item);

        public void Clear() => Value.Clear();

        public bool Contains(JsonValue item) => Value.Contains(item);

        public void CopyTo(JsonValue[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);

        public IEnumerator<JsonValue> GetEnumerator() => Value.GetEnumerator();

        public int IndexOf(JsonValue item) => Value.IndexOf(item);

        public void Insert(int index, JsonValue item) => Value.Insert(index, item);

        public bool Remove(JsonValue item) => Value.Remove(item);

        public void RemoveAt(int index) => Value.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();
    }
}
