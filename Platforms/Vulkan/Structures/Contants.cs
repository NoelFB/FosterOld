using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal static class Contants
    {
        public const float LOD_CLAMP_NONE = 1000.0f;
        public const uint REMAINING_MIP_LEVELS = (~0U);
        public const uint REMAINING_ARRAY_LAYERS = (~0U);
        public const ulong WHOLE_SIZE = (~0UL);
        public const uint ATTACHMENT_UNUSED = (~0U);
        public const uint TRUE = 1;
        public const uint FALSE = 0;
        public const uint QUEUE_FAMILY_IGNORED = (~0U);
        public const uint SUBPASS_EXTERNAL = (~0U);
        public const uint MAX_PHYSICAL_DEVICE_NAME_SIZE = 256;
        public const uint UUID_SIZE = 16;
        public const uint MAX_MEMORY_TYPES = 32;
        public const uint MAX_MEMORY_HEAPS = 16;
        public const uint MAX_EXTENSION_NAME_SIZE = 256;
        public const uint MAX_DESCRIPTION_SIZE = 256;

    }
}
