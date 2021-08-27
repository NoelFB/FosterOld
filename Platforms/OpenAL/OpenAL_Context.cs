using OpenAL;
using System;

namespace Foster.OpenAL
{
    internal class OpenAL_Context : IDisposable
    {
        private readonly IntPtr ALContext;
        public OpenAL_Context(OpenAL_Device device)
        {
            ALContext = ALC10.alcCreateContext(device.ALDevice, null);
        }

        public void Dispose()
        {
            ALC10.alcDestroyContext(ALContext);
        }
    }
}
