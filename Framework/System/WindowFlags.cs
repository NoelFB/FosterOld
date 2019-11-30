using System;

namespace Foster.Framework
{
    [Flags]
    public enum WindowFlags
    {
        None = 0,
        Hidden = 1,
        Transparent = 2,
        ScaleToMonitor = 4,
        MultiSampling = 8
    }
}
