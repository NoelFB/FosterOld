using OpenAL;
using System;

namespace Foster.OpenAL
{
    internal class OpenAL_Device : IDisposable
    {
        public readonly IntPtr ALDevice;
        public OpenAL_Device()
        {
            ALDevice = ALC10.alcOpenDevice(null);
        }
        public void Dispose()
        {
            ALC10.alcCloseDevice(ALDevice);
        }
    }
}
