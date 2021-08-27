using Foster.Framework;
using System;
using OpenAL;
namespace Foster.OpenAL
{
    public class OpenAL_Audio : Audio
    {
        OpenAL_Device device;
        OpenAL_Context context;
        protected override void ApplicationStarted()
        {
            ApiName = "OpenAL-soft";
            device = new OpenAL_Device();
            context = new OpenAL_Context(device);
        }
        protected override void Shutdown()
        {
            context.Dispose();
            device.Dispose();
        }
        public override AudioBuffer.Platform CreateBuffer()
        {
            throw new NotImplementedException();
        }

        public override AudioSource.Platform CreateSource()
        {
            throw new NotImplementedException();
        }
    }
}
