using Foster.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Foster.Json
{
    /// <summary>
    /// A data structure encapsulating a Json Object
    /// </summary>
    public class JsonObject : JsonValue<Dictionary<string, JsonValue>>
    {
        public JsonObject()
            : base(JsonType.Object, new Dictionary<string, JsonValue>())
        {

        }

        public override JsonValue this[string key]
        {
            get
            {
                if (Value.TryGetValue(key, out var value))
                    return value;
                return JsonNull.nul;
            }
            set
            {
                Value[key] = value;
            }
        }

        public override IEnumerable<string> Keys => Value.Keys;
        public override IEnumerable<JsonValue> Values => Value.Values;
        public override IEnumerable<KeyValuePair<string, JsonValue>> Pairs => Value;
        public override int Count => Value.Count;

        public override int GetHashedValue()
        {
            unchecked
            {
                int hash = 17;
                foreach (var (key, value) in Value)
                {
                    hash = hash * 23 + Calc.StaticStringHash(key);
                    hash = hash * 23 + value.GetHashedValue();
                }
                return hash;
            }
        }

        public override JsonValue Clone()
        {
            var clone = new JsonObject();
            foreach (var (key, value) in Value)
                clone[key] = value.Clone();
            return clone;
        }
    }
}
