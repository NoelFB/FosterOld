using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    using VkFlags = UInt32;
    using VkBool32 = UInt32;
    using VkDeviceSize = UInt64;
    using VkSampleMask = UInt32;

    internal unsafe class VK
    {

        private readonly VK_Graphics graphics;

        public VK(VK_Graphics graphics)
        {
            this.graphics = graphics;

            CreateDelegate(ref DestroyInstance, "vkDestroyInstance");
            CreateDelegate(ref EnumeratePhysicalDevices, "vkEnumeratePhysicalDevices");
            CreateDelegate(ref GetPhysicalDeviceFeatures, "vkGetPhysicalDeviceFeatures");
            CreateDelegate(ref GetPhysicalDeviceFormatProperties, "vkGetPhysicalDeviceFormatProperties");
            CreateDelegate(ref GetPhysicalDeviceImageFormatProperties, "vkGetPhysicalDeviceImageFormatProperties");
            CreateDelegate(ref GetPhysicalDeviceProperties, "vkGetPhysicalDeviceProperties");
            CreateDelegate(ref GetPhysicalDeviceQueueFamilyProperties, "vkGetPhysicalDeviceQueueFamilyProperties");
            CreateDelegate(ref GetPhysicalDeviceMemoryProperties, "vkGetPhysicalDeviceMemoryProperties");
            CreateDelegate(ref GetInstanceProcAddr, "vkGetInstanceProcAddr");
            CreateDelegate(ref GetDeviceProcAddr, "vkGetDeviceProcAddr");
            CreateDelegate(ref CreateDevice, "vkCreateDevice");
            CreateDelegate(ref DestroyDevice, "vkDestroyDevice");
            CreateDelegate(ref EnumerateInstanceExtensionProperties, "vkEnumerateInstanceExtensionProperties");
            CreateDelegate(ref EnumerateDeviceExtensionProperties, "vkEnumerateDeviceExtensionProperties");
            CreateDelegate(ref EnumerateInstanceLayerProperties, "vkEnumerateInstanceLayerProperties");
            CreateDelegate(ref EnumerateDeviceLayerProperties, "vkEnumerateDeviceLayerProperties");
            CreateDelegate(ref GetDeviceQueue, "vkGetDeviceQueue");
            CreateDelegate(ref QueueSubmit, "vkQueueSubmit");
            CreateDelegate(ref QueueWaitIdle, "vkQueueWaitIdle");
            CreateDelegate(ref DeviceWaitIdle, "vkDeviceWaitIdle");
            CreateDelegate(ref AllocateMemory, "vkAllocateMemory");
            CreateDelegate(ref FreeMemory, "vkFreeMemory");
            CreateDelegate(ref MapMemory, "vkMapMemory");
            CreateDelegate(ref UnmapMemory, "vkUnmapMemory");
            CreateDelegate(ref FlushMappedMemoryRanges, "vkFlushMappedMemoryRanges");
            CreateDelegate(ref InvalidateMappedMemoryRanges, "vkInvalidateMappedMemoryRanges");
            CreateDelegate(ref GetDeviceMemoryCommitment, "vkGetDeviceMemoryCommitment");
            CreateDelegate(ref BindBufferMemory, "vkBindBufferMemory");
            CreateDelegate(ref BindImageMemory, "vkBindImageMemory");
            CreateDelegate(ref GetBufferMemoryRequirements, "vkGetBufferMemoryRequirements");
            CreateDelegate(ref GetImageMemoryRequirements, "vkGetImageMemoryRequirements");
            CreateDelegate(ref GetImageSparseMemoryRequirements, "vkGetImageSparseMemoryRequirements");
            CreateDelegate(ref GetPhysicalDeviceSparseImageFormatProperties, "vkGetPhysicalDeviceSparseImageFormatProperties");
            CreateDelegate(ref QueueBindSparse, "vkQueueBindSparse");
            CreateDelegate(ref CreateFence, "vkCreateFence");
            CreateDelegate(ref DestroyFence, "vkDestroyFence");
            CreateDelegate(ref ResetFences, "vkResetFences");
            CreateDelegate(ref GetFenceStatus, "vkGetFenceStatus");
            CreateDelegate(ref WaitForFences, "vkWaitForFences");
            CreateDelegate(ref CreateSemaphore, "vkCreateSemaphore");
            CreateDelegate(ref DestroySemaphore, "vkDestroySemaphore");
            CreateDelegate(ref CreateEvent, "vkCreateEvent");
            CreateDelegate(ref DestroyEvent, "vkDestroyEvent");
            CreateDelegate(ref GetEventStatus, "vkGetEventStatus");
            CreateDelegate(ref SetEvent, "vkSetEvent");
            CreateDelegate(ref ResetEvent, "vkResetEvent");
            CreateDelegate(ref CreateQueryPool, "vkCreateQueryPool");
            CreateDelegate(ref DestroyQueryPool, "vkDestroyQueryPool");
            CreateDelegate(ref GetQueryPoolResults, "vkGetQueryPoolResults");
            CreateDelegate(ref CreateBuffer, "vkCreateBuffer");
            CreateDelegate(ref DestroyBuffer, "vkDestroyBuffer");
            CreateDelegate(ref CreateBufferView, "vkCreateBufferView");
            CreateDelegate(ref DestroyBufferView, "vkDestroyBufferView");
            CreateDelegate(ref CreateImage, "vkCreateImage");
            CreateDelegate(ref DestroyImage, "vkDestroyImage");
            CreateDelegate(ref GetImageSubresourceLayout, "vkGetImageSubresourceLayout");
            CreateDelegate(ref CreateImageView, "vkCreateImageView");
            CreateDelegate(ref DestroyImageView, "vkDestroyImageView");
            CreateDelegate(ref CreateShaderModule, "vkCreateShaderModule");
            CreateDelegate(ref DestroyShaderModule, "vkDestroyShaderModule");
            CreateDelegate(ref CreatePipelineCache, "vkCreatePipelineCache");
            CreateDelegate(ref DestroyPipelineCache, "vkDestroyPipelineCache");
            CreateDelegate(ref GetPipelineCacheData, "vkGetPipelineCacheData");
            CreateDelegate(ref MergePipelineCaches, "vkMergePipelineCaches");
            CreateDelegate(ref CreateGraphicsPipelines, "vkCreateGraphicsPipelines");
            CreateDelegate(ref CreateComputePipelines, "vkCreateComputePipelines");
            CreateDelegate(ref DestroyPipeline, "vkDestroyPipeline");
            CreateDelegate(ref CreatePipelineLayout, "vkCreatePipelineLayout");
            CreateDelegate(ref DestroyPipelineLayout, "vkDestroyPipelineLayout");
            CreateDelegate(ref CreateSampler, "vkCreateSampler");
            CreateDelegate(ref DestroySampler, "vkDestroySampler");
            CreateDelegate(ref CreateDescriptorSetLayout, "vkCreateDescriptorSetLayout");
            CreateDelegate(ref DestroyDescriptorSetLayout, "vkDestroyDescriptorSetLayout");
            CreateDelegate(ref CreateDescriptorPool, "vkCreateDescriptorPool");
            CreateDelegate(ref DestroyDescriptorPool, "vkDestroyDescriptorPool");
            CreateDelegate(ref ResetDescriptorPool, "vkResetDescriptorPool");
            CreateDelegate(ref AllocateDescriptorSets, "vkAllocateDescriptorSets");
            CreateDelegate(ref FreeDescriptorSets, "vkFreeDescriptorSets");
            CreateDelegate(ref UpdateDescriptorSets, "vkUpdateDescriptorSets");
            CreateDelegate(ref CreateFramebuffer, "vkCreateFramebuffer");
            CreateDelegate(ref DestroyFramebuffer, "vkDestroyFramebuffer");
            CreateDelegate(ref CreateRenderPass, "vkCreateRenderPass");
            CreateDelegate(ref DestroyRenderPass, "vkDestroyRenderPass");
            CreateDelegate(ref GetRenderAreaGranularity, "vkGetRenderAreaGranularity");
            CreateDelegate(ref CreateCommandPool, "vkCreateCommandPool");
            CreateDelegate(ref DestroyCommandPool, "vkDestroyCommandPool");
            CreateDelegate(ref ResetCommandPool, "vkResetCommandPool");
            CreateDelegate(ref AllocateCommandBuffers, "vkAllocateCommandBuffers");
            CreateDelegate(ref FreeCommandBuffers, "vkFreeCommandBuffers");
            CreateDelegate(ref BeginCommandBuffer, "vkBeginCommandBuffer");
            CreateDelegate(ref EndCommandBuffer, "vkEndCommandBuffer");
            CreateDelegate(ref ResetCommandBuffer, "vkResetCommandBuffer");
            CreateDelegate(ref CmdBindPipeline, "vkCmdBindPipeline");
            CreateDelegate(ref CmdSetViewport, "vkCmdSetViewport");
            CreateDelegate(ref CmdSetScissor, "vkCmdSetScissor");
            CreateDelegate(ref CmdSetLineWidth, "vkCmdSetLineWidth");
            CreateDelegate(ref CmdSetDepthBias, "vkCmdSetDepthBias");
            CreateDelegate(ref CmdSetBlendConstants, "vkCmdSetBlendConstants");
            CreateDelegate(ref CmdSetDepthBounds, "vkCmdSetDepthBounds");
            CreateDelegate(ref CmdSetStencilCompareMask, "vkCmdSetStencilCompareMask");
            CreateDelegate(ref CmdSetStencilWriteMask, "vkCmdSetStencilWriteMask");
            CreateDelegate(ref CmdSetStencilReference, "vkCmdSetStencilReference");
            CreateDelegate(ref CmdBindDescriptorSets, "vkCmdBindDescriptorSets");
            CreateDelegate(ref CmdBindIndexBuffer, "vkCmdBindIndexBuffer");
            CreateDelegate(ref CmdBindVertexBuffers, "vkCmdBindVertexBuffers");
            CreateDelegate(ref CmdDraw, "vkCmdDraw");
            CreateDelegate(ref CmdDrawIndexed, "vkCmdDrawIndexed");
            CreateDelegate(ref CmdDrawIndirect, "vkCmdDrawIndirect");
            CreateDelegate(ref CmdDrawIndexedIndirect, "vkCmdDrawIndexedIndirect");
            CreateDelegate(ref CmdDispatch, "vkCmdDispatch");
            CreateDelegate(ref CmdDispatchIndirect, "vkCmdDispatchIndirect");
            CreateDelegate(ref CmdCopyBuffer, "vkCmdCopyBuffer");
            CreateDelegate(ref CmdCopyImage, "vkCmdCopyImage");
            CreateDelegate(ref CmdBlitImage, "vkCmdBlitImage");
            CreateDelegate(ref CmdCopyBufferToImage, "vkCmdCopyBufferToImage");
            CreateDelegate(ref CmdCopyImageToBuffer, "vkCmdCopyImageToBuffer");
            CreateDelegate(ref CmdUpdateBuffer, "vkCmdUpdateBuffer");
            CreateDelegate(ref CmdFillBuffer, "vkCmdFillBuffer");
            CreateDelegate(ref CmdClearColorImage, "vkCmdClearColorImage");
            CreateDelegate(ref CmdClearDepthStencilImage, "vkCmdClearDepthStencilImage");
            CreateDelegate(ref CmdClearAttachments, "vkCmdClearAttachments");
            CreateDelegate(ref CmdResolveImage, "vkCmdResolveImage");
            CreateDelegate(ref CmdSetEvent, "vkCmdSetEvent");
            CreateDelegate(ref CmdResetEvent, "vkCmdResetEvent");
            CreateDelegate(ref CmdWaitEvents, "vkCmdWaitEvents");
            CreateDelegate(ref CmdPipelineBarrier, "vkCmdPipelineBarrier");
            CreateDelegate(ref CmdBeginQuery, "vkCmdBeginQuery");
            CreateDelegate(ref CmdEndQuery, "vkCmdEndQuery");
            CreateDelegate(ref CmdResetQueryPool, "vkCmdResetQueryPool");
            CreateDelegate(ref CmdWriteTimestamp, "vkCmdWriteTimestamp");
            CreateDelegate(ref CmdCopyQueryPoolResults, "vkCmdCopyQueryPoolResults");
            CreateDelegate(ref CmdPushConstants, "vkCmdPushConstants");
            CreateDelegate(ref CmdBeginRenderPass, "vkCmdBeginRenderPass");
            CreateDelegate(ref CmdNextSubpass, "vkCmdNextSubpass");
            CreateDelegate(ref CmdEndRenderPass, "vkCmdEndRenderPass");
            CreateDelegate(ref CmdExecuteCommands, "vkCmdExecuteCommands");
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

        public static VkResult CreateInstance(ISystemVulkan system, VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance)
        {
            pInstance = IntPtr.Zero;

            VkCreateInstance call = null;
            CreateDelegate(system, IntPtr.Zero, ref call, "vkCreateInstance");
            return call(pCreateInfo, pAllocator, out pInstance);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult VkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, out IntPtr pInstance);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void VkDestroyInstance(IntPtr instance, VkAllocationCallbacks* pAllocator);
        public VkDestroyInstance DestroyInstance;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEnumeratePhysicalDevices(VkInstance instance, UInt32* pPhysicalDeviceCount, VkPhysicalDevice* pPhysicalDevices);
        public vkEnumeratePhysicalDevices EnumeratePhysicalDevices;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceFeatures(VkPhysicalDevice physicalDevice, VkPhysicalDeviceFeatures* pFeatures);
        public vkGetPhysicalDeviceFeatures GetPhysicalDeviceFeatures;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkFormatProperties* pFormatProperties);
        public vkGetPhysicalDeviceFormatProperties GetPhysicalDeviceFormatProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkGetPhysicalDeviceImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkImageTiling tiling, VkImageUsageFlags usage, VkImageCreateFlags flags, VkImageFormatProperties* pImageFormatProperties);
        public vkGetPhysicalDeviceImageFormatProperties GetPhysicalDeviceImageFormatProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceProperties* pProperties);
        public vkGetPhysicalDeviceProperties GetPhysicalDeviceProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceQueueFamilyProperties(VkPhysicalDevice physicalDevice, UInt32* pQueueFamilyPropertyCount, VkQueueFamilyProperties* pQueueFamilyProperties);
        public vkGetPhysicalDeviceQueueFamilyProperties GetPhysicalDeviceQueueFamilyProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceMemoryProperties(VkPhysicalDevice physicalDevice, VkPhysicalDeviceMemoryProperties* pMemoryProperties);
        public vkGetPhysicalDeviceMemoryProperties GetPhysicalDeviceMemoryProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate IntPtr vkGetInstanceProcAddr(VkInstance instance, byte** pName);
        public vkGetInstanceProcAddr GetInstanceProcAddr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate IntPtr vkGetDeviceProcAddr(VkDevice device, byte** pName);
        public vkGetDeviceProcAddr GetDeviceProcAddr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateDevice(VkPhysicalDevice physicalDevice, VkDeviceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDevice* pDevice);
        public vkCreateDevice CreateDevice;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyDevice(VkDevice device, VkAllocationCallbacks* pAllocator);
        public vkDestroyDevice DestroyDevice;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEnumerateInstanceExtensionProperties(byte** pLayerName, UInt32* pPropertyCount, VkExtensionProperties* pProperties);
        public vkEnumerateInstanceExtensionProperties EnumerateInstanceExtensionProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEnumerateDeviceExtensionProperties(VkPhysicalDevice physicalDevice, byte** pLayerName, UInt32* pPropertyCount, VkExtensionProperties* pProperties);
        public vkEnumerateDeviceExtensionProperties EnumerateDeviceExtensionProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEnumerateInstanceLayerProperties(UInt32* pPropertyCount, VkLayerProperties* pProperties);
        public vkEnumerateInstanceLayerProperties EnumerateInstanceLayerProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEnumerateDeviceLayerProperties(VkPhysicalDevice physicalDevice, UInt32* pPropertyCount, VkLayerProperties* pProperties);
        public vkEnumerateDeviceLayerProperties EnumerateDeviceLayerProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetDeviceQueue(VkDevice device, UInt32 queueFamilyIndex, UInt32 queueIndex, VkQueue* pQueue);
        public vkGetDeviceQueue GetDeviceQueue;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkQueueSubmit(VkQueue queue, UInt32 submitCount, VkSubmitInfo* pSubmits, VkFence fence);
        public vkQueueSubmit QueueSubmit;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkQueueWaitIdle(VkQueue queue);
        public vkQueueWaitIdle QueueWaitIdle;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkDeviceWaitIdle(VkDevice device);
        public vkDeviceWaitIdle DeviceWaitIdle;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkAllocateMemory(VkDevice device, VkMemoryAllocateInfo* pAllocateInfo, VkAllocationCallbacks* pAllocator, VkDeviceMemory* pMemory);
        public vkAllocateMemory AllocateMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkFreeMemory(VkDevice device, VkDeviceMemory memory, VkAllocationCallbacks* pAllocator);
        public vkFreeMemory FreeMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkMapMemory(VkDevice device, VkDeviceMemory memory, VkDeviceSize offset, VkDeviceSize size, VkMemoryMapFlags flags, void** ppData);
        public vkMapMemory MapMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkUnmapMemory(VkDevice device, VkDeviceMemory memory);
        public vkUnmapMemory UnmapMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkFlushMappedMemoryRanges(VkDevice device, UInt32 memoryRangeCount, VkMappedMemoryRange* pMemoryRanges);
        public vkFlushMappedMemoryRanges FlushMappedMemoryRanges;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkInvalidateMappedMemoryRanges(VkDevice device, UInt32 memoryRangeCount, VkMappedMemoryRange* pMemoryRanges);
        public vkInvalidateMappedMemoryRanges InvalidateMappedMemoryRanges;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetDeviceMemoryCommitment(VkDevice device, VkDeviceMemory memory, VkDeviceSize* pCommittedMemoryInBytes);
        public vkGetDeviceMemoryCommitment GetDeviceMemoryCommitment;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkBindBufferMemory(VkDevice device, VkBuffer buffer, VkDeviceMemory memory, VkDeviceSize memoryOffset);
        public vkBindBufferMemory BindBufferMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkBindImageMemory(VkDevice device, VkImage image, VkDeviceMemory memory, VkDeviceSize memoryOffset);
        public vkBindImageMemory BindImageMemory;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetBufferMemoryRequirements(VkDevice device, VkBuffer buffer, VkMemoryRequirements* pMemoryRequirements);
        public vkGetBufferMemoryRequirements GetBufferMemoryRequirements;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetImageMemoryRequirements(VkDevice device, VkImage image, VkMemoryRequirements* pMemoryRequirements);
        public vkGetImageMemoryRequirements GetImageMemoryRequirements;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetImageSparseMemoryRequirements(VkDevice device, VkImage image, UInt32* pSparseMemoryRequirementCount, VkSparseImageMemoryRequirements* pSparseMemoryRequirements);
        public vkGetImageSparseMemoryRequirements GetImageSparseMemoryRequirements;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetPhysicalDeviceSparseImageFormatProperties(VkPhysicalDevice physicalDevice, VkFormat format, VkImageType type, VkSampleCountFlagBits samples, VkImageUsageFlags usage, VkImageTiling tiling, UInt32* pPropertyCount, VkSparseImageFormatProperties* pProperties);
        public vkGetPhysicalDeviceSparseImageFormatProperties GetPhysicalDeviceSparseImageFormatProperties;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkQueueBindSparse(VkQueue queue, UInt32 bindInfoCount, VkBindSparseInfo* pBindInfo, VkFence fence);
        public vkQueueBindSparse QueueBindSparse;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateFence(VkDevice device, VkFenceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFence* pFence);
        public vkCreateFence CreateFence;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyFence(VkDevice device, VkFence fence, VkAllocationCallbacks* pAllocator);
        public vkDestroyFence DestroyFence;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkResetFences(VkDevice device, UInt32 fenceCount, VkFence* pFences);
        public vkResetFences ResetFences;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkGetFenceStatus(VkDevice device, VkFence fence);
        public vkGetFenceStatus GetFenceStatus;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkWaitForFences(VkDevice device, UInt32 fenceCount, VkFence* pFences, VkBool32 waitAll, UInt64 timeout);
        public vkWaitForFences WaitForFences;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateSemaphore(VkDevice device, VkSemaphoreCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSemaphore* pSemaphore);
        public vkCreateSemaphore CreateSemaphore;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroySemaphore(VkDevice device, VkSemaphore semaphore, VkAllocationCallbacks* pAllocator);
        public vkDestroySemaphore DestroySemaphore;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateEvent(VkDevice device, VkEventCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkEvent* pEvent);
        public vkCreateEvent CreateEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyEvent(VkDevice device, VkEvent evnt, VkAllocationCallbacks* pAllocator);
        public vkDestroyEvent DestroyEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkGetEventStatus(VkDevice device, VkEvent evnt);
        public vkGetEventStatus GetEventStatus;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkSetEvent(VkDevice device, VkEvent evnt);
        public vkSetEvent SetEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkResetEvent(VkDevice device, VkEvent evnt);
        public vkResetEvent ResetEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateQueryPool(VkDevice device, VkQueryPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkQueryPool* pQueryPool);
        public vkCreateQueryPool CreateQueryPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyQueryPool(VkDevice device, VkQueryPool queryPool, VkAllocationCallbacks* pAllocator);
        public vkDestroyQueryPool DestroyQueryPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkGetQueryPoolResults(VkDevice device, VkQueryPool queryPool, UInt32 firstQuery, UInt32 queryCount, int dataSize, void* pData, VkDeviceSize stride, VkQueryResultFlags flags);
        public vkGetQueryPoolResults GetQueryPoolResults;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateBuffer(VkDevice device, VkBufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBuffer* pBuffer);
        public vkCreateBuffer CreateBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyBuffer(VkDevice device, VkBuffer buffer, VkAllocationCallbacks* pAllocator);
        public vkDestroyBuffer DestroyBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateBufferView(VkDevice device, VkBufferViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkBufferView* pView);
        public vkCreateBufferView CreateBufferView;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyBufferView(VkDevice device, VkBufferView bufferView, VkAllocationCallbacks* pAllocator);
        public vkDestroyBufferView DestroyBufferView;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateImage(VkDevice device, VkImageCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImage* pImage);
        public vkCreateImage CreateImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyImage(VkDevice device, VkImage image, VkAllocationCallbacks* pAllocator);
        public vkDestroyImage DestroyImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetImageSubresourceLayout(VkDevice device, VkImage image, VkImageSubresource* pSubresource, VkSubresourceLayout* pLayout);
        public vkGetImageSubresourceLayout GetImageSubresourceLayout;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateImageView(VkDevice device, VkImageViewCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkImageView* pView);
        public vkCreateImageView CreateImageView;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyImageView(VkDevice device, VkImageView imageView, VkAllocationCallbacks* pAllocator);
        public vkDestroyImageView DestroyImageView;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateShaderModule(VkDevice device, VkShaderModuleCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkShaderModule* pShaderModule);
        public vkCreateShaderModule CreateShaderModule;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyShaderModule(VkDevice device, VkShaderModule shaderModule, VkAllocationCallbacks* pAllocator);
        public vkDestroyShaderModule DestroyShaderModule;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreatePipelineCache(VkDevice device, VkPipelineCacheCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineCache* pPipelineCache);
        public vkCreatePipelineCache CreatePipelineCache;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyPipelineCache(VkDevice device, VkPipelineCache pipelineCache, VkAllocationCallbacks* pAllocator);
        public vkDestroyPipelineCache DestroyPipelineCache;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkGetPipelineCacheData(VkDevice device, VkPipelineCache pipelineCache, int* pDataSize, void* pData);
        public vkGetPipelineCacheData GetPipelineCacheData;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkMergePipelineCaches(VkDevice device, VkPipelineCache dstCache, UInt32 srcCacheCount, VkPipelineCache* pSrcCaches);
        public vkMergePipelineCaches MergePipelineCaches;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateGraphicsPipelines(VkDevice device, VkPipelineCache pipelineCache, UInt32 createInfoCount, VkGraphicsPipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines);
        public vkCreateGraphicsPipelines CreateGraphicsPipelines;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateComputePipelines(VkDevice device, VkPipelineCache pipelineCache, UInt32 createInfoCount, VkComputePipelineCreateInfo* pCreateInfos, VkAllocationCallbacks* pAllocator, VkPipeline* pPipelines);
        public vkCreateComputePipelines CreateComputePipelines;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyPipeline(VkDevice device, VkPipeline pipeline, VkAllocationCallbacks* pAllocator);
        public vkDestroyPipeline DestroyPipeline;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreatePipelineLayout(VkDevice device, VkPipelineLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkPipelineLayout* pPipelineLayout);
        public vkCreatePipelineLayout CreatePipelineLayout;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyPipelineLayout(VkDevice device, VkPipelineLayout pipelineLayout, VkAllocationCallbacks* pAllocator);
        public vkDestroyPipelineLayout DestroyPipelineLayout;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateSampler(VkDevice device, VkSamplerCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSampler* pSampler);
        public vkCreateSampler CreateSampler;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroySampler(VkDevice device, VkSampler sampler, VkAllocationCallbacks* pAllocator);
        public vkDestroySampler DestroySampler;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateDescriptorSetLayout(VkDevice device, VkDescriptorSetLayoutCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorSetLayout* pSetLayout);
        public vkCreateDescriptorSetLayout CreateDescriptorSetLayout;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyDescriptorSetLayout(VkDevice device, VkDescriptorSetLayout descriptorSetLayout, VkAllocationCallbacks* pAllocator);
        public vkDestroyDescriptorSetLayout DestroyDescriptorSetLayout;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateDescriptorPool(VkDevice device, VkDescriptorPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkDescriptorPool* pDescriptorPool);
        public vkCreateDescriptorPool CreateDescriptorPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkAllocationCallbacks* pAllocator);
        public vkDestroyDescriptorPool DestroyDescriptorPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkResetDescriptorPool(VkDevice device, VkDescriptorPool descriptorPool, VkDescriptorPoolResetFlags flags);
        public vkResetDescriptorPool ResetDescriptorPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkAllocateDescriptorSets(VkDevice device, VkDescriptorSetAllocateInfo* pAllocateInfo, VkDescriptorSet* pDescriptorSets);
        public vkAllocateDescriptorSets AllocateDescriptorSets;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkFreeDescriptorSets(VkDevice device, VkDescriptorPool descriptorPool, UInt32 descriptorSetCount, VkDescriptorSet* pDescriptorSets);
        public vkFreeDescriptorSets FreeDescriptorSets;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkUpdateDescriptorSets(VkDevice device, UInt32 descriptorWriteCount, VkWriteDescriptorSet* pDescriptorWrites, UInt32 descriptorCopyCount, VkCopyDescriptorSet* pDescriptorCopies);
        public vkUpdateDescriptorSets UpdateDescriptorSets;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateFramebuffer(VkDevice device, VkFramebufferCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkFramebuffer* pFramebuffer);
        public vkCreateFramebuffer CreateFramebuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyFramebuffer(VkDevice device, VkFramebuffer framebuffer, VkAllocationCallbacks* pAllocator);
        public vkDestroyFramebuffer DestroyFramebuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateRenderPass(VkDevice device, VkRenderPassCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkRenderPass* pRenderPass);
        public vkCreateRenderPass CreateRenderPass;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyRenderPass(VkDevice device, VkRenderPass renderPass, VkAllocationCallbacks* pAllocator);
        public vkDestroyRenderPass DestroyRenderPass;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkGetRenderAreaGranularity(VkDevice device, VkRenderPass renderPass, VkExtent2D* pGranularity);
        public vkGetRenderAreaGranularity GetRenderAreaGranularity;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkCreateCommandPool(VkDevice device, VkCommandPoolCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkCommandPool* pCommandPool);
        public vkCreateCommandPool CreateCommandPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkDestroyCommandPool(VkDevice device, VkCommandPool commandPool, VkAllocationCallbacks* pAllocator);
        public vkDestroyCommandPool DestroyCommandPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkResetCommandPool(VkDevice device, VkCommandPool commandPool, VkCommandPoolResetFlags flags);
        public vkResetCommandPool ResetCommandPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkAllocateCommandBuffers(VkDevice device, VkCommandBufferAllocateInfo* pAllocateInfo, VkCommandBuffer* pCommandBuffers);
        public vkAllocateCommandBuffers AllocateCommandBuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkFreeCommandBuffers(VkDevice device, VkCommandPool commandPool, UInt32 commandBufferCount, VkCommandBuffer* pCommandBuffers);
        public vkFreeCommandBuffers FreeCommandBuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkBeginCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferBeginInfo* pBeginInfo);
        public vkBeginCommandBuffer BeginCommandBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkEndCommandBuffer(VkCommandBuffer commandBuffer);
        public vkEndCommandBuffer EndCommandBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate VkResult vkResetCommandBuffer(VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags);
        public vkResetCommandBuffer ResetCommandBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBindPipeline(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline);
        public vkCmdBindPipeline CmdBindPipeline;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetViewport(VkCommandBuffer commandBuffer, UInt32 firstViewport, UInt32 viewportCount, VkViewport* pViewports);
        public vkCmdSetViewport CmdSetViewport;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetScissor(VkCommandBuffer commandBuffer, UInt32 firstScissor, UInt32 scissorCount, VkRect2D* pScissors);
        public vkCmdSetScissor CmdSetScissor;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetLineWidth(VkCommandBuffer commandBuffer, float lineWidth);
        public vkCmdSetLineWidth CmdSetLineWidth;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetDepthBias(VkCommandBuffer commandBuffer, float depthBiasConstantFactor, float depthBiasClamp, float depthBiasSlopeFactor);
        public vkCmdSetDepthBias CmdSetDepthBias;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetBlendConstants(VkCommandBuffer commandBuffer, float[] blendConstants);
        public vkCmdSetBlendConstants CmdSetBlendConstants;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetDepthBounds(VkCommandBuffer commandBuffer, float minDepthBounds, float maxDepthBounds);
        public vkCmdSetDepthBounds CmdSetDepthBounds;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetStencilCompareMask(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, UInt32 compareMask);
        public vkCmdSetStencilCompareMask CmdSetStencilCompareMask;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetStencilWriteMask(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, UInt32 writeMask);
        public vkCmdSetStencilWriteMask CmdSetStencilWriteMask;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetStencilReference(VkCommandBuffer commandBuffer, VkStencilFaceFlags faceMask, UInt32 reference);
        public vkCmdSetStencilReference CmdSetStencilReference;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBindDescriptorSets(VkCommandBuffer commandBuffer, VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, UInt32 firstSet, UInt32 descriptorSetCount, VkDescriptorSet* pDescriptorSets, UInt32 dynamicOffsetCount, UInt32* pDynamicOffsets);
        public vkCmdBindDescriptorSets CmdBindDescriptorSets;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBindIndexBuffer(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, VkIndexType indexType);
        public vkCmdBindIndexBuffer CmdBindIndexBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBindVertexBuffers(VkCommandBuffer commandBuffer, UInt32 firstBinding, UInt32 bindingCount, VkBuffer* pBuffers, VkDeviceSize* pOffsets);
        public vkCmdBindVertexBuffers CmdBindVertexBuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDraw(VkCommandBuffer commandBuffer, UInt32 vertexCount, UInt32 instanceCount, UInt32 firstVertex, UInt32 firstInstance);
        public vkCmdDraw CmdDraw;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDrawIndexed(VkCommandBuffer commandBuffer, UInt32 indexCount, UInt32 instanceCount, UInt32 firstIndex, Int32 vertexOffset, UInt32 firstInstance);
        public vkCmdDrawIndexed CmdDrawIndexed;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDrawIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, UInt32 drawCount, UInt32 stride);
        public vkCmdDrawIndirect CmdDrawIndirect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDrawIndexedIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset, UInt32 drawCount, UInt32 stride);
        public vkCmdDrawIndexedIndirect CmdDrawIndexedIndirect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDispatch(VkCommandBuffer commandBuffer, UInt32 x, UInt32 y, UInt32 z);
        public vkCmdDispatch CmdDispatch;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdDispatchIndirect(VkCommandBuffer commandBuffer, VkBuffer buffer, VkDeviceSize offset);
        public vkCmdDispatchIndirect CmdDispatchIndirect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdCopyBuffer(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkBuffer dstBuffer, UInt32 regionCount, VkBufferCopy* pRegions);
        public vkCmdCopyBuffer CmdCopyBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdCopyImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, UInt32 regionCount, VkImageCopy* pRegions);
        public vkCmdCopyImage CmdCopyImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBlitImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, UInt32 regionCount, VkImageBlit* pRegions, VkFilter filter);
        public vkCmdBlitImage CmdBlitImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdCopyBufferToImage(VkCommandBuffer commandBuffer, VkBuffer srcBuffer, VkImage dstImage, VkImageLayout dstImageLayout, UInt32 regionCount, VkBufferImageCopy* pRegions);
        public vkCmdCopyBufferToImage CmdCopyBufferToImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdCopyImageToBuffer(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkBuffer dstBuffer, UInt32 regionCount, VkBufferImageCopy* pRegions);
        public vkCmdCopyImageToBuffer CmdCopyImageToBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdUpdateBuffer(VkCommandBuffer commandBuffer, VkBuffer dstBuffer, VkDeviceSize dstOffset, VkDeviceSize dataSize, void* pData);
        public vkCmdUpdateBuffer CmdUpdateBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdFillBuffer(VkCommandBuffer commandBuffer, VkBuffer dstBuffer, VkDeviceSize dstOffset, VkDeviceSize size, UInt32 data);
        public vkCmdFillBuffer CmdFillBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdClearColorImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearColorValue* pColor, UInt32 rangeCount, VkImageSubresourceRange* pRanges);
        public vkCmdClearColorImage CmdClearColorImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdClearDepthStencilImage(VkCommandBuffer commandBuffer, VkImage image, VkImageLayout imageLayout, VkClearDepthStencilValue* pDepthStencil, UInt32 rangeCount, VkImageSubresourceRange* pRanges);
        public vkCmdClearDepthStencilImage CmdClearDepthStencilImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdClearAttachments(VkCommandBuffer commandBuffer, UInt32 attachmentCount, VkClearAttachment* pAttachments, UInt32 rectCount, VkClearRect* pRects);
        public vkCmdClearAttachments CmdClearAttachments;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdResolveImage(VkCommandBuffer commandBuffer, VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, UInt32 regionCount, VkImageResolve* pRegions);
        public vkCmdResolveImage CmdResolveImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdSetEvent(VkCommandBuffer commandBuffer, VkEvent evnt, VkPipelineStageFlags stageMask);
        public vkCmdSetEvent CmdSetEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdResetEvent(VkCommandBuffer commandBuffer, VkEvent evnt, VkPipelineStageFlags stageMask);
        public vkCmdResetEvent CmdResetEvent;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdWaitEvents(VkCommandBuffer commandBuffer, UInt32 eventCount, VkEvent* pEvents, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, UInt32 memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, UInt32 bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, UInt32 imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers);
        public vkCmdWaitEvents CmdWaitEvents;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdPipelineBarrier(VkCommandBuffer commandBuffer, VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, UInt32 memoryBarrierCount, VkMemoryBarrier* pMemoryBarriers, UInt32 bufferMemoryBarrierCount, VkBufferMemoryBarrier* pBufferMemoryBarriers, UInt32 imageMemoryBarrierCount, VkImageMemoryBarrier* pImageMemoryBarriers);
        public vkCmdPipelineBarrier CmdPipelineBarrier;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBeginQuery(VkCommandBuffer commandBuffer, VkQueryPool queryPool, UInt32 query, VkQueryControlFlags flags);
        public vkCmdBeginQuery CmdBeginQuery;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdEndQuery(VkCommandBuffer commandBuffer, VkQueryPool queryPool, UInt32 query);
        public vkCmdEndQuery CmdEndQuery;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdResetQueryPool(VkCommandBuffer commandBuffer, VkQueryPool queryPool, UInt32 firstQuery, UInt32 queryCount);
        public vkCmdResetQueryPool CmdResetQueryPool;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdWriteTimestamp(VkCommandBuffer commandBuffer, VkPipelineStageFlagBits pipelineStage, VkQueryPool queryPool, UInt32 query);
        public vkCmdWriteTimestamp CmdWriteTimestamp;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdCopyQueryPoolResults(VkCommandBuffer commandBuffer, VkQueryPool queryPool, UInt32 firstQuery, UInt32 queryCount, VkBuffer dstBuffer, VkDeviceSize dstOffset, VkDeviceSize stride, VkQueryResultFlags flags);
        public vkCmdCopyQueryPoolResults CmdCopyQueryPoolResults;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdPushConstants(VkCommandBuffer commandBuffer, VkPipelineLayout layout, VkShaderStageFlags stageFlags, UInt32 offset, UInt32 size, void* pValues);
        public vkCmdPushConstants CmdPushConstants;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdBeginRenderPass(VkCommandBuffer commandBuffer, VkRenderPassBeginInfo* pRenderPassBegin, VkSubpassContents contents);
        public vkCmdBeginRenderPass CmdBeginRenderPass;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdNextSubpass(VkCommandBuffer commandBuffer, VkSubpassContents contents);
        public vkCmdNextSubpass CmdNextSubpass;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdEndRenderPass(VkCommandBuffer commandBuffer);
        public vkCmdEndRenderPass CmdEndRenderPass;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void vkCmdExecuteCommands(VkCommandBuffer commandBuffer, UInt32 commandBufferCount, VkCommandBuffer* pCommandBuffers);
        public vkCmdExecuteCommands CmdExecuteCommands;

    }
}
