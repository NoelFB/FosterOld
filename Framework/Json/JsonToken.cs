using System;

namespace Foster.Framework.Json
{
    public enum JsonToken
    {
        ObjectStart,
        ObjectKey,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        String,
        Number,
        Boolean,
        Null
    }
}