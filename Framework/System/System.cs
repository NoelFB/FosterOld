using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public abstract class System : Module
    {

        public string? ApiName;
        public Version? ApiVersion;

        public abstract Window CreateWindow(string title, int width, int height);
        public abstract IntPtr ProcAddress(string name);

        protected internal override void Startup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

    }
}
