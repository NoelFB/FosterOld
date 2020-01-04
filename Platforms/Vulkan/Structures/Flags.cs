using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{

    internal struct VkInstanceCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkInstanceCreateFlags flag) => flag.Flag;
        public static implicit operator VkInstanceCreateFlags(UInt32 flag) => new VkInstanceCreateFlags { Flag = flag };
    }

    internal struct VkFormatFeatureFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkFormatFeatureFlags flag) => flag.Flag;
        public static implicit operator VkFormatFeatureFlags(UInt32 flag) => new VkFormatFeatureFlags { Flag = flag };
    }

    internal struct VkImageUsageFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkImageUsageFlags flag) => flag.Flag;
        public static implicit operator VkImageUsageFlags(UInt32 flag) => new VkImageUsageFlags { Flag = flag };
    }

    internal struct VkImageCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkImageCreateFlags flag) => flag.Flag;
        public static implicit operator VkImageCreateFlags(UInt32 flag) => new VkImageCreateFlags { Flag = flag };
    }

    internal struct VkSampleCountFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSampleCountFlags flag) => flag.Flag;
        public static implicit operator VkSampleCountFlags(UInt32 flag) => new VkSampleCountFlags { Flag = flag };
    }

    internal struct VkQueueFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkQueueFlags flag) => flag.Flag;
        public static implicit operator VkQueueFlags(UInt32 flag) => new VkQueueFlags { Flag = flag };
    }

    internal struct VkMemoryPropertyFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkMemoryPropertyFlags flag) => flag.Flag;
        public static implicit operator VkMemoryPropertyFlags(UInt32 flag) => new VkMemoryPropertyFlags { Flag = flag };
    }

    internal struct VkMemoryHeapFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkMemoryHeapFlags flag) => flag.Flag;
        public static implicit operator VkMemoryHeapFlags(UInt32 flag) => new VkMemoryHeapFlags { Flag = flag };
    }

    internal struct VkDeviceCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDeviceCreateFlags flag) => flag.Flag;
        public static implicit operator VkDeviceCreateFlags(UInt32 flag) => new VkDeviceCreateFlags { Flag = flag };
    }

    internal struct VkDeviceQueueCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDeviceQueueCreateFlags flag) => flag.Flag;
        public static implicit operator VkDeviceQueueCreateFlags(UInt32 flag) => new VkDeviceQueueCreateFlags { Flag = flag };
    }

    internal struct VkPipelineStageFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineStageFlags flag) => flag.Flag;
        public static implicit operator VkPipelineStageFlags(UInt32 flag) => new VkPipelineStageFlags { Flag = flag };
    }

    internal struct VkMemoryMapFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkMemoryMapFlags flag) => flag.Flag;
        public static implicit operator VkMemoryMapFlags(UInt32 flag) => new VkMemoryMapFlags { Flag = flag };
    }

    internal struct VkImageAspectFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkImageAspectFlags flag) => flag.Flag;
        public static implicit operator VkImageAspectFlags(UInt32 flag) => new VkImageAspectFlags { Flag = flag };
    }

    internal struct VkSparseImageFormatFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSparseImageFormatFlags flag) => flag.Flag;
        public static implicit operator VkSparseImageFormatFlags(UInt32 flag) => new VkSparseImageFormatFlags { Flag = flag };
    }

    internal struct VkSparseMemoryBindFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSparseMemoryBindFlags flag) => flag.Flag;
        public static implicit operator VkSparseMemoryBindFlags(UInt32 flag) => new VkSparseMemoryBindFlags { Flag = flag };
    }

    internal struct VkFenceCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkFenceCreateFlags flag) => flag.Flag;
        public static implicit operator VkFenceCreateFlags(UInt32 flag) => new VkFenceCreateFlags { Flag = flag };
    }

    internal struct VkSemaphoreCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSemaphoreCreateFlags flag) => flag.Flag;
        public static implicit operator VkSemaphoreCreateFlags(UInt32 flag) => new VkSemaphoreCreateFlags { Flag = flag };
    }

    internal struct VkEventCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkEventCreateFlags flag) => flag.Flag;
        public static implicit operator VkEventCreateFlags(UInt32 flag) => new VkEventCreateFlags { Flag = flag };
    }

    internal struct VkQueryPoolCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkQueryPoolCreateFlags flag) => flag.Flag;
        public static implicit operator VkQueryPoolCreateFlags(UInt32 flag) => new VkQueryPoolCreateFlags { Flag = flag };
    }

    internal struct VkQueryPipelineStatisticFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkQueryPipelineStatisticFlags flag) => flag.Flag;
        public static implicit operator VkQueryPipelineStatisticFlags(UInt32 flag) => new VkQueryPipelineStatisticFlags { Flag = flag };
    }

    internal struct VkQueryResultFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkQueryResultFlags flag) => flag.Flag;
        public static implicit operator VkQueryResultFlags(UInt32 flag) => new VkQueryResultFlags { Flag = flag };
    }

    internal struct VkBufferCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkBufferCreateFlags flag) => flag.Flag;
        public static implicit operator VkBufferCreateFlags(UInt32 flag) => new VkBufferCreateFlags { Flag = flag };
    }

    internal struct VkBufferUsageFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkBufferUsageFlags flag) => flag.Flag;
        public static implicit operator VkBufferUsageFlags(UInt32 flag) => new VkBufferUsageFlags { Flag = flag };
    }

    internal struct VkBufferViewCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkBufferViewCreateFlags flag) => flag.Flag;
        public static implicit operator VkBufferViewCreateFlags(UInt32 flag) => new VkBufferViewCreateFlags { Flag = flag };
    }

    internal struct VkImageViewCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkImageViewCreateFlags flag) => flag.Flag;
        public static implicit operator VkImageViewCreateFlags(UInt32 flag) => new VkImageViewCreateFlags { Flag = flag };
    }

    internal struct VkShaderModuleCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkShaderModuleCreateFlags flag) => flag.Flag;
        public static implicit operator VkShaderModuleCreateFlags(UInt32 flag) => new VkShaderModuleCreateFlags { Flag = flag };
    }

    internal struct VkPipelineCacheCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineCacheCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineCacheCreateFlags(UInt32 flag) => new VkPipelineCacheCreateFlags { Flag = flag };
    }

    internal struct VkPipelineCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineCreateFlags(UInt32 flag) => new VkPipelineCreateFlags { Flag = flag };
    }

    internal struct VkPipelineShaderStageCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineShaderStageCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineShaderStageCreateFlags(UInt32 flag) => new VkPipelineShaderStageCreateFlags { Flag = flag };
    }

    internal struct VkPipelineVertexInputStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineVertexInputStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineVertexInputStateCreateFlags(UInt32 flag) => new VkPipelineVertexInputStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineInputAssemblyStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineInputAssemblyStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineInputAssemblyStateCreateFlags(UInt32 flag) => new VkPipelineInputAssemblyStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineTessellationStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineTessellationStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineTessellationStateCreateFlags(UInt32 flag) => new VkPipelineTessellationStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineViewportStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineViewportStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineViewportStateCreateFlags(UInt32 flag) => new VkPipelineViewportStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineRasterizationStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineRasterizationStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineRasterizationStateCreateFlags(UInt32 flag) => new VkPipelineRasterizationStateCreateFlags { Flag = flag };
    }

    internal struct VkCullModeFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkCullModeFlags flag) => flag.Flag;
        public static implicit operator VkCullModeFlags(UInt32 flag) => new VkCullModeFlags { Flag = flag };
    }

    internal struct VkPipelineMultisampleStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineMultisampleStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineMultisampleStateCreateFlags(UInt32 flag) => new VkPipelineMultisampleStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineDepthStencilStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineDepthStencilStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineDepthStencilStateCreateFlags(UInt32 flag) => new VkPipelineDepthStencilStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineColorBlendStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineColorBlendStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineColorBlendStateCreateFlags(UInt32 flag) => new VkPipelineColorBlendStateCreateFlags { Flag = flag };
    }

    internal struct VkColorComponentFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkColorComponentFlags flag) => flag.Flag;
        public static implicit operator VkColorComponentFlags(UInt32 flag) => new VkColorComponentFlags { Flag = flag };
    }

    internal struct VkPipelineDynamicStateCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineDynamicStateCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineDynamicStateCreateFlags(UInt32 flag) => new VkPipelineDynamicStateCreateFlags { Flag = flag };
    }

    internal struct VkPipelineLayoutCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkPipelineLayoutCreateFlags flag) => flag.Flag;
        public static implicit operator VkPipelineLayoutCreateFlags(UInt32 flag) => new VkPipelineLayoutCreateFlags { Flag = flag };
    }

    internal struct VkShaderStageFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkShaderStageFlags flag) => flag.Flag;
        public static implicit operator VkShaderStageFlags(UInt32 flag) => new VkShaderStageFlags { Flag = flag };
    }

    internal struct VkSamplerCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSamplerCreateFlags flag) => flag.Flag;
        public static implicit operator VkSamplerCreateFlags(UInt32 flag) => new VkSamplerCreateFlags { Flag = flag };
    }

    internal struct VkDescriptorSetLayoutCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDescriptorSetLayoutCreateFlags flag) => flag.Flag;
        public static implicit operator VkDescriptorSetLayoutCreateFlags(UInt32 flag) => new VkDescriptorSetLayoutCreateFlags { Flag = flag };
    }

    internal struct VkDescriptorPoolCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDescriptorPoolCreateFlags flag) => flag.Flag;
        public static implicit operator VkDescriptorPoolCreateFlags(UInt32 flag) => new VkDescriptorPoolCreateFlags { Flag = flag };
    }

    internal struct VkDescriptorPoolResetFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDescriptorPoolResetFlags flag) => flag.Flag;
        public static implicit operator VkDescriptorPoolResetFlags(UInt32 flag) => new VkDescriptorPoolResetFlags { Flag = flag };
    }

    internal struct VkFramebufferCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkFramebufferCreateFlags flag) => flag.Flag;
        public static implicit operator VkFramebufferCreateFlags(UInt32 flag) => new VkFramebufferCreateFlags { Flag = flag };
    }

    internal struct VkRenderPassCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkRenderPassCreateFlags flag) => flag.Flag;
        public static implicit operator VkRenderPassCreateFlags(UInt32 flag) => new VkRenderPassCreateFlags { Flag = flag };
    }

    internal struct VkAttachmentDescriptionFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkAttachmentDescriptionFlags flag) => flag.Flag;
        public static implicit operator VkAttachmentDescriptionFlags(UInt32 flag) => new VkAttachmentDescriptionFlags { Flag = flag };
    }

    internal struct VkSubpassDescriptionFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkSubpassDescriptionFlags flag) => flag.Flag;
        public static implicit operator VkSubpassDescriptionFlags(UInt32 flag) => new VkSubpassDescriptionFlags { Flag = flag };
    }

    internal struct VkAccessFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkAccessFlags flag) => flag.Flag;
        public static implicit operator VkAccessFlags(UInt32 flag) => new VkAccessFlags { Flag = flag };
    }

    internal struct VkDependencyFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkDependencyFlags flag) => flag.Flag;
        public static implicit operator VkDependencyFlags(UInt32 flag) => new VkDependencyFlags { Flag = flag };
    }

    internal struct VkCommandPoolCreateFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkCommandPoolCreateFlags flag) => flag.Flag;
        public static implicit operator VkCommandPoolCreateFlags(UInt32 flag) => new VkCommandPoolCreateFlags { Flag = flag };
    }

    internal struct VkCommandPoolResetFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkCommandPoolResetFlags flag) => flag.Flag;
        public static implicit operator VkCommandPoolResetFlags(UInt32 flag) => new VkCommandPoolResetFlags { Flag = flag };
    }

    internal struct VkCommandBufferUsageFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkCommandBufferUsageFlags flag) => flag.Flag;
        public static implicit operator VkCommandBufferUsageFlags(UInt32 flag) => new VkCommandBufferUsageFlags { Flag = flag };
    }

    internal struct VkQueryControlFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkQueryControlFlags flag) => flag.Flag;
        public static implicit operator VkQueryControlFlags(UInt32 flag) => new VkQueryControlFlags { Flag = flag };
    }

    internal struct VkCommandBufferResetFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkCommandBufferResetFlags flag) => flag.Flag;
        public static implicit operator VkCommandBufferResetFlags(UInt32 flag) => new VkCommandBufferResetFlags { Flag = flag };
    }

    internal struct VkStencilFaceFlags
    {
        public UInt32 Flag;
        public static implicit operator UInt32(VkStencilFaceFlags flag) => flag.Flag;
        public static implicit operator VkStencilFaceFlags(UInt32 flag) => new VkStencilFaceFlags { Flag = flag };
    }
}
