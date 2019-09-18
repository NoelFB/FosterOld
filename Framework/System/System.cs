using System;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class System : Module
    {

        public string? ApiName;
        public Version? ApiVersion;

        public abstract Window CreateWindow(string title, int width, int height, bool visible = true);
        public abstract ReadOnlyCollection<Window> Windows { get; }
        public abstract IntPtr GetProcAddress(string name);

        protected internal override void OnStartup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }


    }
}
