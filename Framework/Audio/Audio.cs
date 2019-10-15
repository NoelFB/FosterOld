using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Audio : Module
    {
        public string ApiName { get; protected set; } = "Unknown";
        public Version ApiVersion { get; protected set; } = new Version(0, 0, 0);

        protected Audio()
        {
            Priority = 400;
        }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Audio {ApiName} {ApiVersion}");
        }
    }
}
