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

        public JsonObject(string path) 
            : this()
        {
            using var reader = new JsonTextReader(File.OpenRead(path));
            reader.ReadObject(this);
        }

        public JsonObject(Stream stream)
            : this()
        {
            using var reader = new JsonTextReader(stream);
            reader.ReadObject(this);
        }

        public static JsonObject FromFile(string path)
        {
            return new JsonObject(path);
        }

        public static JsonObject FromString(string jsonString)
        {
            using var reader = new JsonTextReader(new StringReader(jsonString));
            return reader.ReadObject();
        }

        public void ToFile(string path, bool strict = true)
        {
            using var writer = new JsonTextWriter(File.Create(path), strict);
            writer.Json(this);
        }

        public override JsonValue this[string key]
        {
            get
            {
                if (Value.TryGetValue(key, out var value))
                    return value;
                return new JsonNull();
            }
            set => Value[key] = value;
        }

        public override IEnumerable<string> Keys => Value.Keys;
        public override IEnumerable<JsonValue> Values => Value.Values;
        public override IEnumerable<KeyValuePair<string, JsonValue>> Object => Value;

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
