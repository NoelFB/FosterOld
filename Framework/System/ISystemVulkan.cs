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
        /// Gets a list of required Vulkan Extensions
        /// </summary>
        List<string> GetVKExtensions();
    }
}
