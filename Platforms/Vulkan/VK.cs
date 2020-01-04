using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    internal unsafe class VK
    {

        private readonly VK_Graphics graphics;

        public VK(VK_Graphics graphics)
        {
            this.graphics = graphics;

            CreateDelegate(ref DestroyInstance, "vkDestroyInstance");
        }

        private void CreateDelegate<T>(ref T def, string name) where T : class
        {
            CreateDelegate(graphics.System, graphics.VkInsance, ref def, name);
        }

        private static void CreateDelegate<T>(ISystemVulkan system, IntPtr vkInstance, ref T def, string name) where T : class
        {
            var addr = system.GetVKProcAddress(vkInstance, name);
            if (addr != IntPtr.Zero && (Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) is T del))
                def = del;
        }

        public static int CreateInstance(ISystemVulkan system, VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance)
        {
            pInstance = IntPtr.Zero;

            VkCreateInstance call = null;
            CreateDelegate<VkCreateInstance>(system, IntPtr.Zero, ref call, "vkCreateInstance");
            return call(pCreateInfo, pAllocator, out pInstance);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate int VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void VkDestroyInstance(IntPtr instance, VkAllocationCallbacks* pAllocator);
        public VkDestroyInstance DestroyInstance;

    }
}
