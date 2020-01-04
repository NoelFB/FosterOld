using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Vulkan
{
    internal struct VkInstance
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkInstance value) => value.Ptr;
        public static implicit operator VkInstance(IntPtr value) => new VkInstance { Ptr = value };
    }

    internal struct VkPhysicalDevice
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkPhysicalDevice value) => value.Ptr;
        public static implicit operator VkPhysicalDevice(IntPtr value) => new VkPhysicalDevice { Ptr = value };
    }

    internal struct VkDevice
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkDevice value) => value.Ptr;
        public static implicit operator VkDevice(IntPtr value) => new VkDevice { Ptr = value };
    }

    internal struct VkQueue
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkQueue value) => value.Ptr;
        public static implicit operator VkQueue(IntPtr value) => new VkQueue { Ptr = value };
    }

    internal struct VkSemaphore
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkSemaphore value) => value.Ptr;
        public static implicit operator VkSemaphore(IntPtr value) => new VkSemaphore { Ptr = value };
    }

    internal struct VkCommandBuffer
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkCommandBuffer value) => value.Ptr;
        public static implicit operator VkCommandBuffer(IntPtr value) => new VkCommandBuffer { Ptr = value };
    }

    internal struct VkFence
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkFence value) => value.Ptr;
        public static implicit operator VkFence(IntPtr value) => new VkFence { Ptr = value };
    }

    internal struct VkDeviceMemory
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkDeviceMemory value) => value.Ptr;
        public static implicit operator VkDeviceMemory(IntPtr value) => new VkDeviceMemory { Ptr = value };
    }

    internal struct VkBuffer
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkBuffer value) => value.Ptr;
        public static implicit operator VkBuffer(IntPtr value) => new VkBuffer { Ptr = value };
    }

    internal struct VkImage
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkImage value) => value.Ptr;
        public static implicit operator VkImage(IntPtr value) => new VkImage { Ptr = value };
    }

    internal struct VkEvent
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkEvent value) => value.Ptr;
        public static implicit operator VkEvent(IntPtr value) => new VkEvent { Ptr = value };
    }

    internal struct VkQueryPool
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkQueryPool value) => value.Ptr;
        public static implicit operator VkQueryPool(IntPtr value) => new VkQueryPool { Ptr = value };
    }

    internal struct VkBufferView
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkBufferView value) => value.Ptr;
        public static implicit operator VkBufferView(IntPtr value) => new VkBufferView { Ptr = value };
    }

    internal struct VkImageView
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkImageView value) => value.Ptr;
        public static implicit operator VkImageView(IntPtr value) => new VkImageView { Ptr = value };
    }

    internal struct VkShaderModule
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkShaderModule value) => value.Ptr;
        public static implicit operator VkShaderModule(IntPtr value) => new VkShaderModule { Ptr = value };
    }

    internal struct VkPipelineCache
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkPipelineCache value) => value.Ptr;
        public static implicit operator VkPipelineCache(IntPtr value) => new VkPipelineCache { Ptr = value };
    }

    internal struct VkPipelineLayout
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkPipelineLayout value) => value.Ptr;
        public static implicit operator VkPipelineLayout(IntPtr value) => new VkPipelineLayout { Ptr = value };
    }

    internal struct VkRenderPass
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkRenderPass value) => value.Ptr;
        public static implicit operator VkRenderPass(IntPtr value) => new VkRenderPass { Ptr = value };
    }

    internal struct VkPipeline
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkPipeline value) => value.Ptr;
        public static implicit operator VkPipeline(IntPtr value) => new VkPipeline { Ptr = value };
    }

    internal struct VkDescriptorSetLayout
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkDescriptorSetLayout value) => value.Ptr;
        public static implicit operator VkDescriptorSetLayout(IntPtr value) => new VkDescriptorSetLayout { Ptr = value };
    }

    internal struct VkSampler
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkSampler value) => value.Ptr;
        public static implicit operator VkSampler(IntPtr value) => new VkSampler { Ptr = value };
    }

    internal struct VkDescriptorPool
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkDescriptorPool value) => value.Ptr;
        public static implicit operator VkDescriptorPool(IntPtr value) => new VkDescriptorPool { Ptr = value };
    }

    internal struct VkDescriptorSet
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkDescriptorSet value) => value.Ptr;
        public static implicit operator VkDescriptorSet(IntPtr value) => new VkDescriptorSet { Ptr = value };
    }

    internal struct VkFramebuffer
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkFramebuffer value) => value.Ptr;
        public static implicit operator VkFramebuffer(IntPtr value) => new VkFramebuffer { Ptr = value };
    }

    internal struct VkCommandPool
    {
        public IntPtr Ptr;

        public static implicit operator IntPtr(VkCommandPool value) => value.Ptr;
        public static implicit operator VkCommandPool(IntPtr value) => new VkCommandPool { Ptr = value };
    }

}
