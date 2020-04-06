using System;

namespace Foster.Framework
{
    /// <summary>
    /// Time values
    /// </summary>
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
        /// The Maximum elapsed time a fixed update can take before skipping update calls
        /// </summary>
        public static TimeSpan FixedMaxElapsedTime = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// The total running time of the application
        /// </summary>
        public static TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Multiplies the Delta Time per frame by the scale value
        /// </summary>
        public static float DeltaScale = 1.0f;

        /// <summary>
        /// The Delta Time from the last frame
        /// </summary>
        public static float Delta { get; internal set; }

        /// <summary>
        /// The Delta Time from the last frame, not scaled by DeltaScale
        /// </summary>
        public static float UnscaledDelta { get; internal set; }

        /// <summary>
        /// A rough estimate of the current Frames Per Second
        /// </summary>
        public static int FPS { get; internal set; }

        /// <summary>
        /// Returns true when the elapsed time passes a given interval based on the delta time
        /// </summary>
        public static bool OnInterval(double time, double delta, double interval, double offset)
        {
            return Math.Floor((time - offset - delta) / interval) < Math.Floor((time - offset) / interval);
        }

        /// <summary>
        /// Returns true when the elapsed time passes a given interval based on the delta time
        /// </summary>
        public static bool OnInterval(double delta, double interval, double offset)
        {
            return OnInterval(Duration.TotalSeconds, delta, interval, offset);
        }

        /// <summary>
        /// Returns true when the elapsed time passes a given interval based on the delta time
        /// </summary>
        public static bool OnInterval(double interval, double offset = 0.0)
        {
            return OnInterval(Duration.TotalSeconds, Delta, interval, offset);
        }

    }
}
