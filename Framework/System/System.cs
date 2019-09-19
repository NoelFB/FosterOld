using System;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class System : Module
    {

        /// <summary>
        /// Underlying System implementation API Name
        /// </summary>
        public string? ApiName;

        /// <summary>
        /// Underlying System implementation API Version
        /// </summary>
        public Version? ApiVersion;

        /// <summary>
        /// The First-created and Primary App Window
        /// </summary>
        public Window? MainWindow { get; internal set; }

        /// <summary>
        /// The Currently Active Window
        /// Call Window.MakeCurrent() to change the CurrentWindow
        /// </summary>
        public Window? CurrentWindow { get; internal set; }

        /// <summary>
        /// Creates a new Window
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public abstract Window CreateWindow(string title, int width, int height, bool visible = true);

        /// <summary>
        /// A list of all opened Windows
        /// </summary>
        public abstract ReadOnlyCollection<Window> Windows { get; }

        /// <summary>
        /// Gets a Pointer to a Platform rendering method of the given name
        /// This is used internally by the Graphics
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IntPtr GetProcAddress(string name);

        protected internal override void OnStartup()
        {
            Console.WriteLine($" - System {ApiName} {ApiVersion}");
        }

    }
}
