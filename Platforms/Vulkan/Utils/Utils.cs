using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal static class Utils
    {

        public static uint ToVulkanVersion(int major, int minor, int patch)
        {
            return (uint)(((major) << 22) | ((minor) << 12) | (patch));
        }

        public static Version FromVulkanVersion(uint version)
        {
            return new Version((int)(version >> 22), (int)((version >> 22) & 0x3ff), (int)(version & 0xfff));
        }

    }
}
