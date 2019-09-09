using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Audio : Module
    {
        public string? ApiName { get; protected set; }
        public Version? ApiVersion { get; protected set; }

        protected internal override void Startup()
        {
            Console.WriteLine($" - Audio {ApiName} {ApiVersion}");
        }
    }
}
