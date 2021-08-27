using System;
using Foster.Framework;
using OpenAL;

namespace Foster.OpenAL
{
    internal class OpenAL_AudioBuffer : AudioBuffer.Platform
    {
        uint AL_Buffer;
        internal OpenAL_AudioBuffer()
        {
            AL10.alGenBuffers(1, out AL_Buffer);
        }
        protected override void SetData<T>(ReadOnlyMemory<T> buffer)
        {
            throw new NotImplementedException();
        }
        protected override void Dispose()
        {
            AL10.alDeleteBuffers(1, ref AL_Buffer);
        }
    }
}
