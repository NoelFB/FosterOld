using System;

namespace Foster.Framework
{
    public abstract class Input : Module
    {
        public string? ApiName { get; protected set; }
        public Version? ApiVersion { get; protected set; }

        protected internal override void OnStartup()
        {
            Console.WriteLine($" - Input {ApiName} {ApiVersion}");
        }
    }
}
