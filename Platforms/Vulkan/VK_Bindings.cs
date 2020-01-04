using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    internal class VK_Bindings
    {
        private readonly ISystemVulkan system;

        public VK_Bindings(ISystemVulkan system)
        {
            this.system = system;

            CreateDelegate(ref vkCreateInstance, "vkCreateInstance");
        }

        private void CreateDelegate<T>(ref T def, string name) where T : class
        {
            var addr = system.GetVKProcAddress(IntPtr.Zero, name);
            if (addr != IntPtr.Zero && (Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) is T del))
                def = del;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate int CreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance);
        public CreateInstance vkCreateInstance;
    }
}
