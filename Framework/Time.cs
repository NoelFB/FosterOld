using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public static class Time
    {

        /// <summary>
        /// Whether the Update function runs at a Fixed Timestep
        /// </summary>
        public static bool FixedStepEnabled = false;

        /// <summary>
        /// The Target Framerate of a Fixed Timestep update
        /// </summary>
        public static int FixedStepTarget = 60;

        /// <summary>
        /// The total running time of the application
        /// </summary>
        public static TimeSpan Duration { get; internal set; }

        /// <summary>
        /// The Delta Time from the last frame
        /// </summary>
        public static float Delta { get; internal set; }

        /// <summary>
        /// A rough estimate of the current Frames Per Second
        /// </summary>
        public static int FPS { get; internal set; }

        public static bool OnInterval(double time, double delta, double interval, double offset = 0f)
        {
            return Math.Floor((time - offset - delta) / interval) < Math.Floor((time - offset) / interval);
        }

        public static bool OnInterval(double interval, double delta, double offset = 0f)
        {
            return Math.Floor((Duration.TotalSeconds - offset - delta) / interval) < Math.Floor((Duration.TotalSeconds - offset) / interval);
        }

        public static bool OnInterval(double interval, double offset = 0f)
        {
            return Math.Floor((Duration.TotalSeconds - offset - Delta) / interval) < Math.Floor((Duration.TotalSeconds - offset) / interval);
        }

    }
}
