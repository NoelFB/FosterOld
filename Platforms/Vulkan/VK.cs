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

        private void CreateDelegate<T>(ref T def, string name) where T : class
        {
            CreateDelegate(graphics.System, graphics.Instance, ref def, name);
        }

        private static void CreateDelegate<T>(ISystemVulkan system, IntPtr vkInstance, ref T def, string name) where T : class
        {
            var addr = system.GetVKProcAddress(vkInstance, name);
            if (addr != IntPtr.Zero && (Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) is T del))
                def = del;
        }

        public static uint MAKE_VERSION(int major, int minor, int patch)
        {
            return (uint)(((major) << 22) | ((minor) << 12) | (patch));
        }

        public static uint MAKE_VERSION(Version version)
        {
            return MAKE_VERSION(version.Major, version.Minor, version.Revision);
        }

        public static Version UNMAKE_VERSION(uint version)
        {
            return new Version((int)(version >> 22), (int)((version >> 22) & 0x3ff), (int)(version & 0xfff));
        }

        public static string STRING(byte* ptr)
        {
            int length = 0;
            while (length < 4096 && ptr[length] != 0)
                length++;

            return Encoding.UTF8.GetString(ptr, length);
        }

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
            CreateDelegate(ref EnumerateDeviceExtensionProperties, "vkEnumerateDeviceExtensionProperties");
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
            CreateDelegate(ref DestroySurfaceKHR, "vkDestroySurfaceKHR");
            CreateDelegate(ref GetPhysicalDeviceSurfaceSupportKHR, "vkGetPhysicalDeviceSurfaceSupportKHR");
            CreateDelegate(ref GetPhysicalDeviceSurfaceCapabilitiesKHR, "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
            CreateDelegate(ref GetPhysicalDeviceSurfaceFormatsKHR, "vkGetPhysicalDeviceSurfaceFormatsKHR");
            CreateDelegate(ref GetPhysicalDeviceSurfacePresentModesKHR, "vkGetPhysicalDeviceSurfacePresentModesKHR");
            CreateDelegate(ref CreateSwapchainKHR, "vkCreateSwapchainKHR");
            CreateDelegate(ref DestroySwapchainKHR, "vkDestroySwapchainKHR");
            CreateDelegate(ref GetSwapchainImagesKHR, "vkGetSwapchainImagesKHR");
            CreateDelegate(ref AcquireNextImageKHR, "vkAcquireNextImageKHR");
            CreateDelegate(ref QueuePresentKHR, "vkQueuePresentKHR");
            CreateDelegate(ref GetPhysicalDeviceDisplayPropertiesKHR, "vkGetPhysicalDeviceDisplayPropertiesKHR");
            CreateDelegate(ref GetPhysicalDeviceDisplayPlanePropertiesKHR, "vkGetPhysicalDeviceDisplayPlanePropertiesKHR");
            CreateDelegate(ref GetDisplayPlaneSupportedDisplaysKHR, "vkGetDisplayPlaneSupportedDisplaysKHR");
            CreateDelegate(ref GetDisplayModePropertiesKHR, "vkGetDisplayModePropertiesKHR");
            CreateDelegate(ref CreateDisplayModeKHR, "vkCreateDisplayModeKHR");
            CreateDelegate(ref GetDisplayPlaneCapabilitiesKHR, "vkGetDisplayPlaneCapabilitiesKHR");
            CreateDelegate(ref CreateDisplayPlaneSurfaceKHR, "vkCreateDisplayPlaneSurfaceKHR");
            CreateDelegate(ref CreateSharedSwapchainsKHR, "vkCreateSharedSwapchainsKHR");
            CreateDelegate(ref CreateDebugReportCallbackEXT, "vkCreateDebugReportCallbackEXT");
            CreateDelegate(ref DestroyDebugReportCallbackEXT, "vkDestroyDebugReportCallbackEXT");
            CreateDelegate(ref DebugReportMessageEXT, "vkDebugReportMessageEXT");
            CreateDelegate(ref DebugMarkerSetObjectTagEXT, "vkDebugMarkerSetObjectTagEXT");
            CreateDelegate(ref DebugMarkerSetObjectNameEXT, "vkDebugMarkerSetObjectNameEXT");
            CreateDelegate(ref CmdDebugMarkerBeginEXT, "vkCmdDebugMarkerBeginEXT");
            CreateDelegate(ref CmdDebugMarkerEndEXT, "vkCmdDebugMarkerEndEXT");
            CreateDelegate(ref CmdDebugMarkerInsertEXT, "vkCmdDebugMarkerInsertEXT");
            CreateDelegate(ref CmdDrawIndirectCountAMD, "vkCmdDrawIndirectCountAMD");
            CreateDelegate(ref CmdDrawIndexedIndirectCountAMD, "vkCmdDrawIndexedIndirectCountAMD");
            CreateDelegate(ref GetPhysicalDeviceExternalImageFormatPropertiesNV, "vkGetPhysicalDeviceExternalImageFormatPropertiesNV");

            CreateDelegate(ref SetDebugUtilsObjectNameEXT, "vkSetDebugUtilsObjectNameEXT");
            CreateDelegate(ref SetDebugUtilsObjectTagEXT, "vkSetDebugUtilsObjectTagEXT");
            CreateDelegate(ref QueueBeginDebugUtilsLabelEXT, "vkQueueBeginDebugUtilsLabelEXT");
            CreateDelegate(ref QueueEndDebugUtilsLabelEXT, "vkQueueEndDebugUtilsLabelEXT");
            CreateDelegate(ref QueueInsertDebugUtilsLabelEXT, "vkQueueInsertDebugUtilsLabelEXT");
            CreateDelegate(ref CmdBeginDebugUtilsLabelEXT, "vkCmdBeginDebugUtilsLabelEXT");
            CreateDelegate(ref CmdEndDebugUtilsLabelEXT, "vkCmdEndDebugUtilsLabelEXT");
            CreateDelegate(ref CmdInsertDebugUtilsLabelEXT, "vkCmdInsertDebugUtilsLabelEXT");
            CreateDelegate(ref CreateDebugUtilsMessengerEXT, "vkCreateDebugUtilsMessengerEXT");
            CreateDelegate(ref DestroyDebugUtilsMessengerEXT, "vkDestroyDebugUtilsMessengerEXT");
            CreateDelegate(ref SubmitDebugUtilsMessageEXT, "vkSubmitDebugUtilsMessageEXT");
        }

        public static void InitStaticDelegates(ISystemVulkan system)
        {
            CreateDelegate(system, IntPtr.Zero, ref CreateInstance, "vkCreateInstance");
            CreateDelegate(system, IntPtr.Zero, ref EnumerateInstanceExtensionProperties, "vkEnumerateInstanceExtensionProperties");
            CreateDelegate(system, IntPtr.Zero, ref EnumerateInstanceLayerProperties, "vkEnumerateInstanceLayerProperties");
        }

        public static vkCreateInstance CreateInstance;
        public vkDestroyInstance DestroyInstance;
        public vkEnumeratePhysicalDevices EnumeratePhysicalDevices;
        public vkGetPhysicalDeviceFeatures GetPhysicalDeviceFeatures;
        public vkGetPhysicalDeviceFormatProperties GetPhysicalDeviceFormatProperties;
        public vkGetPhysicalDeviceImageFormatProperties GetPhysicalDeviceImageFormatProperties;
        public vkGetPhysicalDeviceProperties GetPhysicalDeviceProperties;
        public vkGetPhysicalDeviceQueueFamilyProperties GetPhysicalDeviceQueueFamilyProperties;
        public vkGetPhysicalDeviceMemoryProperties GetPhysicalDeviceMemoryProperties;
        public vkGetInstanceProcAddr GetInstanceProcAddr;
        public vkGetDeviceProcAddr GetDeviceProcAddr;
        public vkCreateDevice CreateDevice;
        public vkDestroyDevice DestroyDevice;
        public static vkEnumerateInstanceExtensionProperties EnumerateInstanceExtensionProperties;
        public vkEnumerateDeviceExtensionProperties EnumerateDeviceExtensionProperties;
        public static vkEnumerateInstanceLayerProperties EnumerateInstanceLayerProperties;
        public vkEnumerateDeviceLayerProperties EnumerateDeviceLayerProperties;
        public vkGetDeviceQueue GetDeviceQueue;
        public vkQueueSubmit QueueSubmit;
        public vkQueueWaitIdle QueueWaitIdle;
        public vkDeviceWaitIdle DeviceWaitIdle;
        public vkAllocateMemory AllocateMemory;
        public vkFreeMemory FreeMemory;
        public vkMapMemory MapMemory;
        public vkUnmapMemory UnmapMemory;
        public vkFlushMappedMemoryRanges FlushMappedMemoryRanges;
        public vkInvalidateMappedMemoryRanges InvalidateMappedMemoryRanges;
        public vkGetDeviceMemoryCommitment GetDeviceMemoryCommitment;
        public vkBindBufferMemory BindBufferMemory;
        public vkBindImageMemory BindImageMemory;
        public vkGetBufferMemoryRequirements GetBufferMemoryRequirements;
        public vkGetImageMemoryRequirements GetImageMemoryRequirements;
        public vkGetImageSparseMemoryRequirements GetImageSparseMemoryRequirements;
        public vkGetPhysicalDeviceSparseImageFormatProperties GetPhysicalDeviceSparseImageFormatProperties;
        public vkQueueBindSparse QueueBindSparse;
        public vkCreateFence CreateFence;
        public vkDestroyFence DestroyFence;
        public vkResetFences ResetFences;
        public vkGetFenceStatus GetFenceStatus;
        public vkWaitForFences WaitForFences;
        public vkCreateSemaphore CreateSemaphore;
        public vkDestroySemaphore DestroySemaphore;
        public vkCreateEvent CreateEvent;
        public vkDestroyEvent DestroyEvent;
        public vkGetEventStatus GetEventStatus;
        public vkSetEvent SetEvent;
        public vkResetEvent ResetEvent;
        public vkCreateQueryPool CreateQueryPool;
        public vkDestroyQueryPool DestroyQueryPool;
        public vkGetQueryPoolResults GetQueryPoolResults;
        public vkCreateBuffer CreateBuffer;
        public vkDestroyBuffer DestroyBuffer;
        public vkCreateBufferView CreateBufferView;
        public vkDestroyBufferView DestroyBufferView;
        public vkCreateImage CreateImage;
        public vkDestroyImage DestroyImage;
        public vkGetImageSubresourceLayout GetImageSubresourceLayout;
        public vkCreateImageView CreateImageView;
        public vkDestroyImageView DestroyImageView;
        public vkCreateShaderModule CreateShaderModule;
        public vkDestroyShaderModule DestroyShaderModule;
        public vkCreatePipelineCache CreatePipelineCache;
        public vkDestroyPipelineCache DestroyPipelineCache;
        public vkGetPipelineCacheData GetPipelineCacheData;
        public vkMergePipelineCaches MergePipelineCaches;
        public vkCreateGraphicsPipelines CreateGraphicsPipelines;
        public vkCreateComputePipelines CreateComputePipelines;
        public vkDestroyPipeline DestroyPipeline;
        public vkCreatePipelineLayout CreatePipelineLayout;
        public vkDestroyPipelineLayout DestroyPipelineLayout;
        public vkCreateSampler CreateSampler;
        public vkDestroySampler DestroySampler;
        public vkCreateDescriptorSetLayout CreateDescriptorSetLayout;
        public vkDestroyDescriptorSetLayout DestroyDescriptorSetLayout;
        public vkCreateDescriptorPool CreateDescriptorPool;
        public vkDestroyDescriptorPool DestroyDescriptorPool;
        public vkResetDescriptorPool ResetDescriptorPool;
        public vkAllocateDescriptorSets AllocateDescriptorSets;
        public vkFreeDescriptorSets FreeDescriptorSets;
        public vkUpdateDescriptorSets UpdateDescriptorSets;
        public vkCreateFramebuffer CreateFramebuffer;
        public vkDestroyFramebuffer DestroyFramebuffer;
        public vkCreateRenderPass CreateRenderPass;
        public vkDestroyRenderPass DestroyRenderPass;
        public vkGetRenderAreaGranularity GetRenderAreaGranularity;
        public vkCreateCommandPool CreateCommandPool;
        public vkDestroyCommandPool DestroyCommandPool;
        public vkResetCommandPool ResetCommandPool;
        public vkAllocateCommandBuffers AllocateCommandBuffers;
        public vkFreeCommandBuffers FreeCommandBuffers;
        public vkBeginCommandBuffer BeginCommandBuffer;
        public vkEndCommandBuffer EndCommandBuffer;
        public vkResetCommandBuffer ResetCommandBuffer;
        public vkCmdBindPipeline CmdBindPipeline;
        public vkCmdSetViewport CmdSetViewport;
        public vkCmdSetScissor CmdSetScissor;
        public vkCmdSetLineWidth CmdSetLineWidth;
        public vkCmdSetDepthBias CmdSetDepthBias;
        public vkCmdSetBlendConstants CmdSetBlendConstants;
        public vkCmdSetDepthBounds CmdSetDepthBounds;
        public vkCmdSetStencilCompareMask CmdSetStencilCompareMask;
        public vkCmdSetStencilWriteMask CmdSetStencilWriteMask;
        public vkCmdSetStencilReference CmdSetStencilReference;
        public vkCmdBindDescriptorSets CmdBindDescriptorSets;
        public vkCmdBindIndexBuffer CmdBindIndexBuffer;
        public vkCmdBindVertexBuffers CmdBindVertexBuffers;
        public vkCmdDraw CmdDraw;
        public vkCmdDrawIndexed CmdDrawIndexed;
        public vkCmdDrawIndirect CmdDrawIndirect;
        public vkCmdDrawIndexedIndirect CmdDrawIndexedIndirect;
        public vkCmdDispatch CmdDispatch;
        public vkCmdDispatchIndirect CmdDispatchIndirect;
        public vkCmdCopyBuffer CmdCopyBuffer;
        public vkCmdCopyImage CmdCopyImage;
        public vkCmdBlitImage CmdBlitImage;
        public vkCmdCopyBufferToImage CmdCopyBufferToImage;
        public vkCmdCopyImageToBuffer CmdCopyImageToBuffer;
        public vkCmdUpdateBuffer CmdUpdateBuffer;
        public vkCmdFillBuffer CmdFillBuffer;
        public vkCmdClearColorImage CmdClearColorImage;
        public vkCmdClearDepthStencilImage CmdClearDepthStencilImage;
        public vkCmdClearAttachments CmdClearAttachments;
        public vkCmdResolveImage CmdResolveImage;
        public vkCmdSetEvent CmdSetEvent;
        public vkCmdResetEvent CmdResetEvent;
        public vkCmdWaitEvents CmdWaitEvents;
        public vkCmdPipelineBarrier CmdPipelineBarrier;
        public vkCmdBeginQuery CmdBeginQuery;
        public vkCmdEndQuery CmdEndQuery;
        public vkCmdResetQueryPool CmdResetQueryPool;
        public vkCmdWriteTimestamp CmdWriteTimestamp;
        public vkCmdCopyQueryPoolResults CmdCopyQueryPoolResults;
        public vkCmdPushConstants CmdPushConstants;
        public vkCmdBeginRenderPass CmdBeginRenderPass;
        public vkCmdNextSubpass CmdNextSubpass;
        public vkCmdEndRenderPass CmdEndRenderPass;
        public vkCmdExecuteCommands CmdExecuteCommands;
        public vkDestroySurfaceKHR DestroySurfaceKHR;
        public vkGetPhysicalDeviceSurfaceSupportKHR GetPhysicalDeviceSurfaceSupportKHR;
        public vkGetPhysicalDeviceSurfaceCapabilitiesKHR GetPhysicalDeviceSurfaceCapabilitiesKHR;
        public vkGetPhysicalDeviceSurfaceFormatsKHR GetPhysicalDeviceSurfaceFormatsKHR;
        public vkGetPhysicalDeviceSurfacePresentModesKHR GetPhysicalDeviceSurfacePresentModesKHR;
        public vkCreateSwapchainKHR CreateSwapchainKHR;
        public vkDestroySwapchainKHR DestroySwapchainKHR;
        public vkGetSwapchainImagesKHR GetSwapchainImagesKHR;
        public vkAcquireNextImageKHR AcquireNextImageKHR;
        public vkQueuePresentKHR QueuePresentKHR;
        public vkGetPhysicalDeviceDisplayPropertiesKHR GetPhysicalDeviceDisplayPropertiesKHR;
        public vkGetPhysicalDeviceDisplayPlanePropertiesKHR GetPhysicalDeviceDisplayPlanePropertiesKHR;
        public vkGetDisplayPlaneSupportedDisplaysKHR GetDisplayPlaneSupportedDisplaysKHR;
        public vkGetDisplayModePropertiesKHR GetDisplayModePropertiesKHR;
        public vkCreateDisplayModeKHR CreateDisplayModeKHR;
        public vkGetDisplayPlaneCapabilitiesKHR GetDisplayPlaneCapabilitiesKHR;
        public vkCreateDisplayPlaneSurfaceKHR CreateDisplayPlaneSurfaceKHR;
        public vkCreateSharedSwapchainsKHR CreateSharedSwapchainsKHR;
        public vkCreateDebugReportCallbackEXT CreateDebugReportCallbackEXT;
        public vkDestroyDebugReportCallbackEXT DestroyDebugReportCallbackEXT;
        public vkDebugReportMessageEXT DebugReportMessageEXT;
        public vkDebugMarkerSetObjectTagEXT DebugMarkerSetObjectTagEXT;
        public vkDebugMarkerSetObjectNameEXT DebugMarkerSetObjectNameEXT;
        public vkCmdDebugMarkerBeginEXT CmdDebugMarkerBeginEXT;
        public vkCmdDebugMarkerEndEXT CmdDebugMarkerEndEXT;
        public vkCmdDebugMarkerInsertEXT CmdDebugMarkerInsertEXT;
        public vkCmdDrawIndirectCountAMD CmdDrawIndirectCountAMD;
        public vkCmdDrawIndexedIndirectCountAMD CmdDrawIndexedIndirectCountAMD;
        public vkGetPhysicalDeviceExternalImageFormatPropertiesNV GetPhysicalDeviceExternalImageFormatPropertiesNV;

        public vkSetDebugUtilsObjectNameEXT SetDebugUtilsObjectNameEXT;
        public vkSetDebugUtilsObjectTagEXT SetDebugUtilsObjectTagEXT;
        public vkQueueBeginDebugUtilsLabelEXT QueueBeginDebugUtilsLabelEXT;
        public vkQueueEndDebugUtilsLabelEXT QueueEndDebugUtilsLabelEXT;
        public vkQueueInsertDebugUtilsLabelEXT QueueInsertDebugUtilsLabelEXT;
        public vkCmdBeginDebugUtilsLabelEXT CmdBeginDebugUtilsLabelEXT;
        public vkCmdEndDebugUtilsLabelEXT CmdEndDebugUtilsLabelEXT;
        public vkCmdInsertDebugUtilsLabelEXT CmdInsertDebugUtilsLabelEXT;
        public vkCreateDebugUtilsMessengerEXT CreateDebugUtilsMessengerEXT;
        public vkDestroyDebugUtilsMessengerEXT DestroyDebugUtilsMessengerEXT;
        public vkSubmitDebugUtilsMessageEXT SubmitDebugUtilsMessageEXT;

    }
}
