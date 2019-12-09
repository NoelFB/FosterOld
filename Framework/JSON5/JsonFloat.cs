using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public struct JsonFloat : IJsonValue
    {
        public JsonType Type => JsonType.Number;
        public float Value;

        public JsonFloat(float value)
        {
            Value = value;
        }

        public static implicit operator float(JsonFloat value) => value.Value;
    }
}
