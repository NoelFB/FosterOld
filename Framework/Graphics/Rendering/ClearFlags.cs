using System;
namespace Foster.Framework
{
    [Flags]
    public enum ClearFlags
    {
        None = 0,
        Color = 1,
        Depth = 2,
        Stencil = 4,
        All = 7
    }
}
