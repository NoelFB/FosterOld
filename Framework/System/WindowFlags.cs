using System;

namespace Foster.Framework
{
    [Flags]
    public enum WindowFlags
    {
        None = 0,

        /// <summary>
        /// Hides the Window when it is created
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// Gives the Window a Transparent background
        /// </summary>
        Transparent = 2,

        /// <summary>
        /// Whether the Window should automatically scale to the Monitor
        /// Ex. if a 1280x720 window is created, but the Monitor DPI is 2, this will
        /// create a window at 2560x1440
        /// </summary>
        ScaleToMonitor = 4,

        /// <summary>
        /// Whether the Window BackBuffer should use Multi Sampling. The exact value
        /// of multisampling depends on the platform
        /// </summary>
        MultiSampling = 8
    }
}
