using System.Collections.Generic;

namespace Foster.Json
{
    /// <summary>
    /// A data structure encapsulating a Json Array
    /// </summary>
    public class JsonArray : JsonValue<List<JsonValue>>
    {
        public JsonArray() : base(JsonType.Array, new List<JsonValue>())
        {
        }

        public JsonArray(IList<string> list) : base(JsonType.Array, new List<JsonValue>())
        {
            foreach (var value in list)
                Value.Add(value);
        }

        public override JsonValue this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }

        public void Add(JsonValue value)
        {
            Value.Add(value);
        }

        public void Remove(JsonValue value)
        {
            Value.Remove(value);
        }

        public bool Contains(JsonValue value)
        {
            return Value.Contains(value);
        }

        public override int Count => Value.Count;
        public override IEnumerable<JsonValue> Array => Value;

        public override int GetHashedValue()
        {
            unchecked
            {
                int hash = 17;
                foreach (var value in Value)
                    hash = hash * 23 + value.GetHashedValue();
                return hash;
            }
        }

        public override JsonValue Clone()
        {
            var clone = new JsonArray();
            foreach (var value in Value)
                clone.Add(value.Clone());
            return clone;
        }

    }
}
