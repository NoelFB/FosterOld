using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal static class VK
    {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private static VK_Bindings bindings;
#pragma warning restore CS8618

        public static void Init(ISystemVulkan system)
        {
            bindings = new VK_Bindings(system);
        }

        public static unsafe int CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance) => bindings.vkCreateInstance(pCreateInfo, pAllocator, out pInstance);

    }
}
