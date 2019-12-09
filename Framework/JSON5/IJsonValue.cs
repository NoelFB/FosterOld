using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public interface IJsonValue
    {
        public JsonType Type { get; }
    }

    public static class IJsonValueExt
    {
        public static bool IsNull(this IJsonValue value) => value.Type == JsonType.Null;
        public static bool IsBool(this IJsonValue value) => value.Type == JsonType.Bool;
        public static bool IsNumber(this IJsonValue value) => value.Type == JsonType.Number;
        public static bool IsString(this IJsonValue value) => value.Type == JsonType.String;
        public static bool IsObject(this IJsonValue value) => value.Type == JsonType.Object;
        public static bool IsArray(this IJsonValue value) => value.Type == JsonType.Array;

        public static float Float(this IJsonValue value)
        {
            if (value.Type == JsonType.Number && value is JsonFloat fl)
                return fl.Value;
            return 0;
        }
    }
}
