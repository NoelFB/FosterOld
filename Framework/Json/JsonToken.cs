using System;

namespace Foster.Framework.Json
{
    /// <summary>
    /// A Json Token
    /// </summary>
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