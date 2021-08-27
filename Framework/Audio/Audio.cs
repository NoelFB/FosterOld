using System;

namespace Foster.Framework
{
    /// <summary>
    /// The Core Audio Module, used for playing sounds
    /// </summary>
    public abstract class Audio : AppModule
    {
        public string ApiName { get; protected set; } = "Unknown";
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);

        protected Audio() : base(400)
        {

        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Audio {ApiName} {ApiVersion}");
        }

        public abstract AudioBuffer.Platform CreateBuffer();
        public abstract AudioSource.Platform CreateSource();
    }
}
