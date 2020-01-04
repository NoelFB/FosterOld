using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal static class VK_Utils
    {

        public static uint Version(int major, int minor, int patch)
        {
            return (uint)(((major) << 22) | ((minor) << 12) | (patch));
        }

    }
}
