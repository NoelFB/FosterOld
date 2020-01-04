using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public interface ISystemVulkan
    {
        /// <summary>
        /// Gets a pointer to a Vulkan function
        /// </summary>
        IntPtr GetVKProcAddress(IntPtr instance, string name);

        /// <summary>
        /// Gets the Vulkan Surface of a given Window
        /// </summary>
        IntPtr GetVKSurface(Window window);

        /// <summary>
        /// Gets a list of required Vulkan Extensions
        /// </summary>
        List<string> GetVKExtensions();
    }
}
