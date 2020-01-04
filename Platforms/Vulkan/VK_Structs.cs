using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    using VkInstanceCreateFlags = UInt32;

    internal unsafe struct VkApplicationInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pApplicationName;
        public UInt32 applicationVersion;
        public byte* pEngineName;
        public UInt32 engineVersion;
        public UInt32 apiVersion;
    }

    internal unsafe struct VkInstanceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkInstanceCreateFlags flags;
        public VkApplicationInfo* pApplicationInfo;
        public UInt32 enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public UInt32 enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
    }

    internal struct VkAllocationCallbacks
    {

    }
}
