using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Audio Buffer contains audio data to be played.
    /// </summary>
    public class AudioBuffer
    {
        public abstract class Platform
        {
            protected internal abstract void SetData<T>(ReadOnlyMemory<T> buffer);
            protected internal abstract void Dispose();
        }

        public readonly Platform Implementation;

        public AudioBuffer(Audio audio)
        {
            Implementation = audio.CreateBuffer();
        }
    }
}
