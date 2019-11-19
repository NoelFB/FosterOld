using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Monitor
    {

        /// <summary>
        /// Whether this Monitor is the Primary Monitor
        /// </summary>
        public abstract bool IsPrimary { get; }

        /// <summary>
        /// The name of the Monitor
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The bounds of the Monitor, in Screen Coordinates
        /// </summary>
        public abstract RectInt Bounds { get; }

        /// <summary>
        /// The Content Scale of the Monitor
        /// </summary>
        public abstract Vector2 ContentScale { get; }

    }
}
