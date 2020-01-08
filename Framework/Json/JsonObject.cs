using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Foster.Framework.Json
{
    /// <summary>
    /// A data structure encapsulating a Json Object
    /// </summary>
    public class JsonObject : JsonValue<Dictionary<string, JsonValue>>, IDictionary<string, JsonValue>
    {
        public JsonObject()
            : base(JsonType.Object, new Dictionary<string, JsonValue>())
        {
        }

        public static JsonObject FromFile(string path)
        {
            using var reader = new JsonReader(File.OpenRead(path));
            return reader.ReadObject();
        }

        public static JsonObject FromString(string jsonString)
        {
            using var reader = new JsonReader(new StringReader(jsonString));
            return reader.ReadObject();
        }

        public JsonValue this[string key]
        {
            get => Value[key];
            set => Value[key] = value;
        }

        public ICollection<string> Keys => Value.Keys;

        public ICollection<JsonValue> Values => Value.Values;

        public int Count => Value.Count;

        public bool IsReadOnly => false;

        public void Add(string key, JsonValue value) => Value.Add(key, value);

        public void Add(KeyValuePair<string, JsonValue> item) => Value.Add(item.Key, item.Value);

        public void Clear() => Value.Clear();

        public bool Contains(KeyValuePair<string, JsonValue> item)
        {
            return (Value.TryGetValue(item.Key, out var value) && value == item.Value);
        }

        public bool ContainsKey(string key) => Value.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
        {
            foreach (var pair in Value)
                array[arrayIndex++] = pair;
        }

        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator() => Value.GetEnumerator();

        public bool Remove(string key) => Value.Remove(key);

        public bool Remove(KeyValuePair<string, JsonValue> item) => Value.Remove(item.Key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out JsonValue value)
        {
            // FIXME: This seems like a C# 8 compiler bug?
            // we should never need to disable the nullable check

#nullable disable
            return Value.TryGetValue(key, out value);
#nullable enable
        }

        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();

    }
}
