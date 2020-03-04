namespace Foster.Json
{
    /// <summary>
    /// A Json Token
    /// </summary>
    public enum JsonToken
    {
        Null = 0,
        ObjectStart = 1,
        ObjectEnd = 2,
        ObjectKey = 3,
        ArrayStart = 4,
        ArrayEnd = 5,
        Boolean = 6,
        String = 7,
        Number = 8,
        Binary = 9,
    }
}