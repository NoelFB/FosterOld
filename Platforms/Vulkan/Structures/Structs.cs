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

    internal unsafe struct VkAllocationCallbacks
    {
        public void* pUserData;
        public IntPtr pfnAllocation;
        public IntPtr pfnReallocation;
        public IntPtr pfnFree;
        public IntPtr pfnInternalAllocation;
        public IntPtr pfnInternalFree;
    }

    internal unsafe struct VkPhysicalDeviceFeatures
    {
        public VkBool32 robustBufferAccess;
        public VkBool32 fullDrawIndexUint32;
        public VkBool32 imageCubeArray;
        public VkBool32 independentBlend;
        public VkBool32 geometryShader;
        public VkBool32 tessellationShader;
        public VkBool32 sampleRateShading;
        public VkBool32 dualSrcBlend;
        public VkBool32 logicOp;
        public VkBool32 multiDrawIndirect;
        public VkBool32 drawIndirectFirstInstance;
        public VkBool32 depthClamp;
        public VkBool32 depthBiasClamp;
        public VkBool32 fillModeNonSolid;
        public VkBool32 depthBounds;
        public VkBool32 wideLines;
        public VkBool32 largePoints;
        public VkBool32 alphaToOne;
        public VkBool32 multiViewport;
        public VkBool32 samplerAnisotropy;
        public VkBool32 textureCompressionETC2;
        public VkBool32 textureCompressionASTC_LDR;
        public VkBool32 textureCompressionBC;
        public VkBool32 occlusionQueryPrecise;
        public VkBool32 pipelineStatisticsQuery;
        public VkBool32 vertexPipelineStoresAndAtomics;
        public VkBool32 fragmentStoresAndAtomics;
        public VkBool32 shaderTessellationAndGeometryPointSize;
        public VkBool32 shaderImageGatherExtended;
        public VkBool32 shaderStorageImageExtendedFormats;
        public VkBool32 shaderStorageImageMultisample;
        public VkBool32 shaderStorageImageReadWithoutFormat;
        public VkBool32 shaderStorageImageWriteWithoutFormat;
        public VkBool32 shaderUniformBufferArrayDynamicIndexing;
        public VkBool32 shaderSampledImageArrayDynamicIndexing;
        public VkBool32 shaderStorageBufferArrayDynamicIndexing;
        public VkBool32 shaderStorageImageArrayDynamicIndexing;
        public VkBool32 shaderClipDistance;
        public VkBool32 shaderCullDistance;
        public VkBool32 shaderFloat64;
        public VkBool32 shaderInt64;
        public VkBool32 shaderInt16;
        public VkBool32 shaderResourceResidency;
        public VkBool32 shaderResourceMinLod;
        public VkBool32 sparseBinding;
        public VkBool32 sparseResidencyBuffer;
        public VkBool32 sparseResidencyImage2D;
        public VkBool32 sparseResidencyImage3D;
        public VkBool32 sparseResidency2Samples;
        public VkBool32 sparseResidency4Samples;
        public VkBool32 sparseResidency8Samples;
        public VkBool32 sparseResidency16Samples;
        public VkBool32 sparseResidencyAliased;
        public VkBool32 variableMultisampleRate;
        public VkBool32 inheritedQueries;
    }

    internal unsafe struct VkFormatProperties
    {
        public VkFormatFeatureFlags linearTilingFeatures;
        public VkFormatFeatureFlags optimalTilingFeatures;
        public VkFormatFeatureFlags bufferFeatures;
    }

    internal unsafe struct VkExtent3D
    {
        public UInt32 width;
        public UInt32 height;
        public UInt32 depth;
    }

    internal unsafe struct VkImageFormatProperties
    {
        public VkExtent3D maxExtent;
        public UInt32 maxMipLevels;
        public UInt32 maxArrayLayers;
        public VkSampleCountFlags sampleCounts;
        public VkDeviceSize maxResourceSize;
    }

    internal unsafe struct VkPhysicalDeviceLimits
    {
        public UInt32 maxImageDimension1D;
        public UInt32 maxImageDimension2D;
        public UInt32 maxImageDimension3D;
        public UInt32 maxImageDimensionCube;
        public UInt32 maxImageArrayLayers;
        public UInt32 maxTexelBufferElements;
        public UInt32 maxUniformBufferRange;
        public UInt32 maxStorageBufferRange;
        public UInt32 maxPushConstantsSize;
        public UInt32 maxMemoryAllocationCount;
        public UInt32 maxSamplerAllocationCount;
        public VkDeviceSize bufferImageGranularity;
        public VkDeviceSize sparseAddressSpaceSize;
        public UInt32 maxBoundDescriptorSets;
        public UInt32 maxPerStageDescriptorSamplers;
        public UInt32 maxPerStageDescriptorUniformBuffers;
        public UInt32 maxPerStageDescriptorStorageBuffers;
        public UInt32 maxPerStageDescriptorSampledImages;
        public UInt32 maxPerStageDescriptorStorageImages;
        public UInt32 maxPerStageDescriptorInputAttachments;
        public UInt32 maxPerStageResources;
        public UInt32 maxDescriptorSetSamplers;
        public UInt32 maxDescriptorSetUniformBuffers;
        public UInt32 maxDescriptorSetUniformBuffersDynamic;
        public UInt32 maxDescriptorSetStorageBuffers;
        public UInt32 maxDescriptorSetStorageBuffersDynamic;
        public UInt32 maxDescriptorSetSampledImages;
        public UInt32 maxDescriptorSetStorageImages;
        public UInt32 maxDescriptorSetInputAttachments;
        public UInt32 maxVertexInputAttributes;
        public UInt32 maxVertexInputBindings;
        public UInt32 maxVertexInputAttributeOffset;
        public UInt32 maxVertexInputBindingStride;
        public UInt32 maxVertexOutputComponents;
        public UInt32 maxTessellationGenerationLevel;
        public UInt32 maxTessellationPatchSize;
        public UInt32 maxTessellationControlPerVertexInputComponents;
        public UInt32 maxTessellationControlPerVertexOutputComponents;
        public UInt32 maxTessellationControlPerPatchOutputComponents;
        public UInt32 maxTessellationControlTotalOutputComponents;
        public UInt32 maxTessellationEvaluationInputComponents;
        public UInt32 maxTessellationEvaluationOutputComponents;
        public UInt32 maxGeometryShaderInvocations;
        public UInt32 maxGeometryInputComponents;
        public UInt32 maxGeometryOutputComponents;
        public UInt32 maxGeometryOutputVertices;
        public UInt32 maxGeometryTotalOutputComponents;
        public UInt32 maxFragmentInputComponents;
        public UInt32 maxFragmentOutputAttachments;
        public UInt32 maxFragmentDualSrcAttachments;
        public UInt32 maxFragmentCombinedOutputResources;
        public UInt32 maxComputeSharedMemorySize;
        public fixed UInt32 maxComputeWorkGroupCount[3];
        public UInt32 maxComputeWorkGroupInvocations;
        public fixed UInt32 maxComputeWorkGroupSize[3];
        public UInt32 subPixelPrecisionBits;
        public UInt32 subTexelPrecisionBits;
        public UInt32 mipmapPrecisionBits;
        public UInt32 maxDrawIndexedIndexValue;
        public UInt32 maxDrawIndirectCount;
        public float maxSamplerLodBias;
        public float maxSamplerAnisotropy;
        public UInt32 maxViewports;
        public fixed UInt32 maxViewportDimensions[2];
        public fixed float viewportBoundsRange[2];
        public UInt32 viewportSubPixelBits;
        public UIntPtr minMemoryMapAlignment;
        public VkDeviceSize minTexelBufferOffsetAlignment;
        public VkDeviceSize minUniformBufferOffsetAlignment;
        public VkDeviceSize minStorageBufferOffsetAlignment;
        public Int32 minTexelOffset;
        public UInt32 maxTexelOffset;
        public Int32 minTexelGatherOffset;
        public UInt32 maxTexelGatherOffset;
        public float minInterpolationOffset;
        public float maxInterpolationOffset;
        public UInt32 subPixelInterpolationOffsetBits;
        public UInt32 maxFramebufferWidth;
        public UInt32 maxFramebufferHeight;
        public UInt32 maxFramebufferLayers;
        public VkSampleCountFlags framebufferColorSampleCounts;
        public VkSampleCountFlags framebufferDepthSampleCounts;
        public VkSampleCountFlags framebufferStencilSampleCounts;
        public VkSampleCountFlags framebufferNoAttachmentsSampleCounts;
        public UInt32 maxColorAttachments;
        public VkSampleCountFlags sampledImageColorSampleCounts;
        public VkSampleCountFlags sampledImageIntegerSampleCounts;
        public VkSampleCountFlags sampledImageDepthSampleCounts;
        public VkSampleCountFlags sampledImageStencilSampleCounts;
        public VkSampleCountFlags storageImageSampleCounts;
        public UInt32 maxSampleMaskWords;
        public VkBool32 timestampComputeAndGraphics;
        public float timestampPeriod;
        public UInt32 maxClipDistances;
        public UInt32 maxCullDistances;
        public UInt32 maxCombinedClipAndCullDistances;
        public UInt32 discreteQueuePriorities;
        public fixed float pointSizeRange[2];
        public fixed float lineWidthRange[2];
        public float pointSizeGranularity;
        public float lineWidthGranularity;
        public VkBool32 strictLines;
        public VkBool32 standardSampleLocations;
        public VkDeviceSize optimalBufferCopyOffsetAlignment;
        public VkDeviceSize optimalBufferCopyRowPitchAlignment;
        public VkDeviceSize nonCoherentAtomSize;
    }

    internal unsafe struct VkPhysicalDeviceSparseProperties
    {
        public VkBool32 residencyStandard2DBlockShape;
        public VkBool32 residencyStandard2DMultisampleBlockShape;
        public VkBool32 residencyStandard3DBlockShape;
        public VkBool32 residencyAlignedMipSize;
        public VkBool32 residencyNonResidentStrict;
    }

    internal unsafe struct VkPhysicalDeviceProperties
    {
        public UInt32 apiVersion;
        public UInt32 driverVersion;
        public UInt32 vendorID;
        public UInt32 deviceID;
        public VkPhysicalDeviceType deviceType;
        public fixed byte deviceName[(int)Contants.MAX_PHYSICAL_DEVICE_NAME_SIZE];
        public fixed byte pipelineCacheUUID[(int)Contants.UUID_SIZE];
        public VkPhysicalDeviceLimits limits;
        public VkPhysicalDeviceSparseProperties sparseProperties;
    }

    internal unsafe struct VkQueueFamilyProperties
    {
        public VkQueueFlags queueFlags;
        public UInt32 queueCount;
        public UInt32 timestampValidBits;
        public VkExtent3D minImageTransferGranularity;
    }

    internal unsafe struct VkMemoryType
    {
        public VkMemoryPropertyFlags propertyFlags;
        public UInt32 heapIndex;
    }

    internal unsafe struct VkMemoryHeap
    {
        public VkDeviceSize size;
        public VkMemoryHeapFlags flags;
    }

    internal unsafe struct VkPhysicalDeviceMemoryProperties
    {
        public UInt32 memoryTypeCount;
        public VkMemoryType memoryTypes0;
        public VkMemoryType memoryTypes1;
        public VkMemoryType memoryTypes2;
        public VkMemoryType memoryTypes3;
        public VkMemoryType memoryTypes4;
        public VkMemoryType memoryTypes5;
        public VkMemoryType memoryTypes6;
        public VkMemoryType memoryTypes7;
        public VkMemoryType memoryTypes8;
        public VkMemoryType memoryTypes9;
        public VkMemoryType memoryTypes10;
        public VkMemoryType memoryTypes11;
        public VkMemoryType memoryTypes12;
        public VkMemoryType memoryTypes13;
        public VkMemoryType memoryTypes14;
        public VkMemoryType memoryTypes15;
        public VkMemoryType memoryTypes16;
        public VkMemoryType memoryTypes17;
        public VkMemoryType memoryTypes18;
        public VkMemoryType memoryTypes19;
        public VkMemoryType memoryTypes20;
        public VkMemoryType memoryTypes21;
        public VkMemoryType memoryTypes22;
        public VkMemoryType memoryTypes23;
        public VkMemoryType memoryTypes24;
        public VkMemoryType memoryTypes25;
        public VkMemoryType memoryTypes26;
        public VkMemoryType memoryTypes27;
        public VkMemoryType memoryTypes28;
        public VkMemoryType memoryTypes29;
        public VkMemoryType memoryTypes30;
        public VkMemoryType memoryTypes31;

        public UInt32 memoryHeapCount;
        public VkMemoryHeap memoryHeaps0;
        public VkMemoryHeap memoryHeaps1;
        public VkMemoryHeap memoryHeaps2;
        public VkMemoryHeap memoryHeaps3;
        public VkMemoryHeap memoryHeaps4;
        public VkMemoryHeap memoryHeaps5;
        public VkMemoryHeap memoryHeaps6;
        public VkMemoryHeap memoryHeaps7;
        public VkMemoryHeap memoryHeaps8;
        public VkMemoryHeap memoryHeaps9;
        public VkMemoryHeap memoryHeaps10;
        public VkMemoryHeap memoryHeaps11;
        public VkMemoryHeap memoryHeaps12;
        public VkMemoryHeap memoryHeaps13;
        public VkMemoryHeap memoryHeaps14;
        public VkMemoryHeap memoryHeaps15;
    }

    internal unsafe struct VkDeviceQueueCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceQueueCreateFlags flags;
        public UInt32 queueFamilyIndex;
        public UInt32 queueCount;
        public float* pQueuePriorities;
    }

    internal unsafe struct VkDeviceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceCreateFlags flags;
        public UInt32 queueCreateInfoCount;
        public VkDeviceQueueCreateInfo* pQueueCreateInfos;
        public UInt32 enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public UInt32 enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
        public VkPhysicalDeviceFeatures* pEnabledFeatures;
    }

    internal unsafe struct VkExtensionProperties
    {
        public fixed byte extensionName[(int)Contants.MAX_EXTENSION_NAME_SIZE];
        public UInt32 specVersion;
    }

    internal unsafe struct VkLayerProperties
    {
        public fixed byte layerName[(int)Contants.MAX_EXTENSION_NAME_SIZE];
        public UInt32 specVersion;
        public UInt32 implementationVersion;
        public fixed byte description[(int)Contants.MAX_DESCRIPTION_SIZE];
    }

    internal unsafe struct VkSubmitInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public UInt32 waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public VkPipelineStageFlags* pWaitDstStageMask;
        public UInt32 commandBufferCount;
        public VkCommandBuffer* pCommandBuffers;
        public UInt32 signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
    }

    internal unsafe struct VkMemoryAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceSize allocationSize;
        public UInt32 memoryTypeIndex;
    }

    internal unsafe struct VkMappedMemoryRange
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public VkDeviceSize offset;
        public VkDeviceSize size;
    }

    internal unsafe struct VkMemoryRequirements
    {
        public VkDeviceSize size;
        public VkDeviceSize alignment;
        public UInt32 memoryTypeBits;
    }

    internal unsafe struct VkSparseImageFormatProperties
    {
        public VkImageAspectFlags aspectMask;
        public VkExtent3D imageGranularity;
        public VkSparseImageFormatFlags flags;
    }

    internal unsafe struct VkSparseImageMemoryRequirements
    {
        public VkSparseImageFormatProperties formatProperties;
        public UInt32 imageMipTailFirstLod;
        public VkDeviceSize imageMipTailSize;
        public VkDeviceSize imageMipTailOffset;
        public VkDeviceSize imageMipTailStride;
    }

    internal unsafe struct VkSparseMemoryBind
    {
        public VkDeviceSize resourceOffset;
        public VkDeviceSize size;
        public VkDeviceMemory memory;
        public VkDeviceSize memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    internal unsafe struct VkSparseBufferMemoryBindInfo
    {
        public VkBuffer buffer;
        public UInt32 bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    internal unsafe struct VkSparseImageOpaqueMemoryBindInfo
    {
        public VkImage image;
        public UInt32 bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    internal unsafe struct VkImageSubresource
    {
        public VkImageAspectFlags aspectMask;
        public UInt32 mipLevel;
        public UInt32 arrayLayer;
    }

    internal unsafe struct VkOffset3D
    {
        public Int32 x;
        public Int32 y;
        public Int32 z;
    }

    internal unsafe struct VkSparseImageMemoryBind
    {
        public VkImageSubresource subresource;
        public VkOffset3D offset;
        public VkExtent3D extent;
        public VkDeviceMemory memory;
        public VkDeviceSize memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    internal unsafe struct VkSparseImageMemoryBindInfo
    {
        public VkImage image;
        public UInt32 bindCount;
        public VkSparseImageMemoryBind* pBinds;
    }

    internal unsafe struct VkBindSparseInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public UInt32 waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public UInt32 bufferBindCount;
        public VkSparseBufferMemoryBindInfo* pBufferBinds;
        public UInt32 imageOpaqueBindCount;
        public VkSparseImageOpaqueMemoryBindInfo* pImageOpaqueBinds;
        public UInt32 imageBindCount;
        public VkSparseImageMemoryBindInfo* pImageBinds;
        public UInt32 signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
    }

    internal unsafe struct VkFenceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFenceCreateFlags flags;
    }

    internal unsafe struct VkSemaphoreCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSemaphoreCreateFlags flags;
    }

    internal unsafe struct VkEventCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkEventCreateFlags flags;
    }

    internal unsafe struct VkQueryPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkQueryPoolCreateFlags flags;
        public VkQueryType queryType;
        public UInt32 queryCount;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
    }

    internal unsafe struct VkBufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBufferCreateFlags flags;
        public VkDeviceSize size;
        public VkBufferUsageFlags usage;
        public VkSharingMode sharingMode;
        public UInt32 queueFamilyIndexCount;
        public UInt32* pQueueFamilyIndices;
    }

    internal unsafe struct VkBufferViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBufferViewCreateFlags flags;
        public VkBuffer buffer;
        public VkFormat format;
        public VkDeviceSize offset;
        public VkDeviceSize range;
    }

    internal unsafe struct VkImageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageCreateFlags flags;
        public VkImageType imageType;
        public VkFormat format;
        public VkExtent3D extent;
        public UInt32 mipLevels;
        public UInt32 arrayLayers;
        public VkSampleCountFlagBits samples;
        public VkImageTiling tiling;
        public VkImageUsageFlags usage;
        public VkSharingMode sharingMode;
        public UInt32 queueFamilyIndexCount;
        public UInt32* pQueueFamilyIndices;
        public VkImageLayout initialLayout;
    }

    internal unsafe struct VkSubresourceLayout
    {
        public VkDeviceSize offset;
        public VkDeviceSize size;
        public VkDeviceSize rowPitch;
        public VkDeviceSize arrayPitch;
        public VkDeviceSize depthPitch;
    }

    internal unsafe struct VkComponentMapping
    {
        public VkComponentSwizzle r;
        public VkComponentSwizzle g;
        public VkComponentSwizzle b;
        public VkComponentSwizzle a;
    }

    internal unsafe struct VkImageSubresourceRange
    {
        public VkImageAspectFlags aspectMask;
        public UInt32 baseMipLevel;
        public UInt32 levelCount;
        public UInt32 baseArrayLayer;
        public UInt32 layerCount;
    }

    internal unsafe struct VkImageViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageViewCreateFlags flags;
        public VkImage image;
        public VkImageViewType viewType;
        public VkFormat format;
        public VkComponentMapping components;
        public VkImageSubresourceRange subresourceRange;
    }

    internal unsafe struct VkShaderModuleCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkShaderModuleCreateFlags flags;
        public UIntPtr codeSize;
        public UInt32* pCode;
    }

    internal unsafe struct VkPipelineCacheCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCacheCreateFlags flags;
        public UIntPtr initialDataSize;
        public void* pInitialData;
    }

    internal unsafe struct VkSpecializationMapEntry
    {
        public UInt32 constantID;
        public UInt32 offset;
        public UIntPtr size;
    }

    internal unsafe struct VkSpecializationInfo
    {
        public UInt32 mapEntryCount;
        public VkSpecializationMapEntry* pMapEntries;
        public UIntPtr dataSize;
        public void* pData;
    }

    internal unsafe struct VkPipelineShaderStageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineShaderStageCreateFlags flags;
        public VkShaderStageFlagBits stage;
        public VkShaderModule module;
        public byte* pName;
        public VkSpecializationInfo* pSpecializationInfo;
    }

    internal unsafe struct VkVertexInputBindingDescription
    {
        public UInt32 binding;
        public UInt32 stride;
        public VkVertexInputRate inputRate;
    }

    internal unsafe struct VkVertexInputAttributeDescription
    {
        public UInt32 location;
        public UInt32 binding;
        public VkFormat format;
        public UInt32 offset;
    }

    internal unsafe struct VkPipelineVertexInputStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineVertexInputStateCreateFlags flags;
        public UInt32 vertexBindingDescriptionCount;
        public VkVertexInputBindingDescription* pVertexBindingDescriptions;
        public UInt32 vertexAttributeDescriptionCount;
        public VkVertexInputAttributeDescription* pVertexAttributeDescriptions;
    }

    internal unsafe struct VkPipelineInputAssemblyStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineInputAssemblyStateCreateFlags flags;
        public VkPrimitiveTopology topology;
        public VkBool32 primitiveRestartEnable;
    }

    internal unsafe struct VkPipelineTessellationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineTessellationStateCreateFlags flags;
        public UInt32 patchControlPoints;
    }

    internal unsafe struct VkViewport
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float minDepth;
        public float maxDepth;
    }

    internal unsafe struct VkOffset2D
    {
        public Int32 x;
        public Int32 y;
    }

    internal unsafe struct VkExtent2D
    {
        public UInt32 width;
        public UInt32 height;
    }

    internal unsafe struct VkRect2D
    {
        public VkOffset2D offset;
        public VkExtent2D extent;
    }

    internal unsafe struct VkPipelineViewportStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineViewportStateCreateFlags flags;
        public UInt32 viewportCount;
        public VkViewport* pViewports;
        public UInt32 scissorCount;
        public VkRect2D* pScissors;
    }

    internal unsafe struct VkPipelineRasterizationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineRasterizationStateCreateFlags flags;
        public VkBool32 depthClampEnable;
        public VkBool32 rasterizerDiscardEnable;
        public VkPolygonMode polygonMode;
        public VkCullModeFlags cullMode;
        public VkFrontFace frontFace;
        public VkBool32 depthBiasEnable;
        public float depthBiasConstantFactor;
        public float depthBiasClamp;
        public float depthBiasSlopeFactor;
        public float lineWidth;
    }

    internal unsafe struct VkPipelineMultisampleStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineMultisampleStateCreateFlags flags;
        public VkSampleCountFlagBits rasterizationSamples;
        public VkBool32 sampleShadingEnable;
        public float minSampleShading;
        public VkSampleMask* pSampleMask;
        public VkBool32 alphaToCoverageEnable;
        public VkBool32 alphaToOneEnable;
    }

    internal unsafe struct VkStencilOpState
    {
        public VkStencilOp failOp;
        public VkStencilOp passOp;
        public VkStencilOp depthFailOp;
        public VkCompareOp compareOp;
        public UInt32 compareMask;
        public UInt32 writeMask;
        public UInt32 reference;
    }

    internal unsafe struct VkPipelineDepthStencilStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineDepthStencilStateCreateFlags flags;
        public VkBool32 depthTestEnable;
        public VkBool32 depthWriteEnable;
        public VkCompareOp depthCompareOp;
        public VkBool32 depthBoundsTestEnable;
        public VkBool32 stencilTestEnable;
        public VkStencilOpState front;
        public VkStencilOpState back;
        public float minDepthBounds;
        public float maxDepthBounds;
    }

    internal unsafe struct VkPipelineColorBlendAttachmentState
    {
        public VkBool32 blendEnable;
        public VkBlendFactor srcColorBlendFactor;
        public VkBlendFactor dstColorBlendFactor;
        public VkBlendOp colorBlendOp;
        public VkBlendFactor srcAlphaBlendFactor;
        public VkBlendFactor dstAlphaBlendFactor;
        public VkBlendOp alphaBlendOp;
        public VkColorComponentFlags colorWriteMask;
    }

    internal unsafe struct VkPipelineColorBlendStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineColorBlendStateCreateFlags flags;
        public VkBool32 logicOpEnable;
        public VkLogicOp logicOp;
        public UInt32 attachmentCount;
        public VkPipelineColorBlendAttachmentState* pAttachments;
        public fixed float blendConstants[4];
    }

    internal unsafe struct VkPipelineDynamicStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineDynamicStateCreateFlags flags;
        public UInt32 dynamicStateCount;
        public VkDynamicState* pDynamicStates;
    }

    internal unsafe struct VkGraphicsPipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public UInt32 stageCount;
        public VkPipelineShaderStageCreateInfo* pStages;
        public VkPipelineVertexInputStateCreateInfo* pVertexInputState;
        public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
        public VkPipelineTessellationStateCreateInfo* pTessellationState;
        public VkPipelineViewportStateCreateInfo* pViewportState;
        public VkPipelineRasterizationStateCreateInfo* pRasterizationState;
        public VkPipelineMultisampleStateCreateInfo* pMultisampleState;
        public VkPipelineDepthStencilStateCreateInfo* pDepthStencilState;
        public VkPipelineColorBlendStateCreateInfo* pColorBlendState;
        public VkPipelineDynamicStateCreateInfo* pDynamicState;
        public VkPipelineLayout layout;
        public VkRenderPass renderPass;
        public UInt32 subpass;
        public VkPipeline basePipelineHandle;
        public Int32 basePipelineIndex;
    }

    internal unsafe struct VkComputePipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public VkPipelineShaderStageCreateInfo stage;
        public VkPipelineLayout layout;
        public VkPipeline basePipelineHandle;
        public Int32 basePipelineIndex;
    }

    internal unsafe struct VkPushConstantRange
    {
        public VkShaderStageFlags stageFlags;
        public UInt32 offset;
        public UInt32 size;
    }

    internal unsafe struct VkPipelineLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineLayoutCreateFlags flags;
        public UInt32 setLayoutCount;
        public VkDescriptorSetLayout* pSetLayouts;
        public UInt32 pushConstantRangeCount;
        public VkPushConstantRange* pPushConstantRanges;
    }

    internal unsafe struct VkSamplerCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSamplerCreateFlags flags;
        public VkFilter magFilter;
        public VkFilter minFilter;
        public VkSamplerMipmapMode mipmapMode;
        public VkSamplerAddressMode addressModeU;
        public VkSamplerAddressMode addressModeV;
        public VkSamplerAddressMode addressModeW;
        public float mipLodBias;
        public VkBool32 anisotropyEnable;
        public float maxAnisotropy;
        public VkBool32 compareEnable;
        public VkCompareOp compareOp;
        public float minLod;
        public float maxLod;
        public VkBorderColor borderColor;
        public VkBool32 unnormalizedCoordinates;
    }

    internal unsafe struct VkDescriptorSetLayoutBinding
    {
        public UInt32 binding;
        public VkDescriptorType descriptorType;
        public UInt32 descriptorCount;
        public VkShaderStageFlags stageFlags;
        public VkSampler* pImmutableSamplers;
    }

    internal unsafe struct VkDescriptorSetLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSetLayoutCreateFlags flags;
        public UInt32 bindingCount;
        public VkDescriptorSetLayoutBinding* pBindings;
    }

    internal unsafe struct VkDescriptorPoolSize
    {
        public VkDescriptorType type;
        public UInt32 descriptorCount;
    }

    internal unsafe struct VkDescriptorPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPoolCreateFlags flags;
        public UInt32 maxSets;
        public UInt32 poolSizeCount;
        public VkDescriptorPoolSize* pPoolSizes;
    }

    internal unsafe struct VkDescriptorSetAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPool descriptorPool;
        public UInt32 descriptorSetCount;
        public VkDescriptorSetLayout* pSetLayouts;
    }

    internal unsafe struct VkDescriptorImageInfo
    {
        public VkSampler sampler;
        public VkImageView imageView;
        public VkImageLayout imageLayout;
    }

    internal unsafe struct VkDescriptorBufferInfo
    {
        public VkBuffer buffer;
        public VkDeviceSize offset;
        public VkDeviceSize range;
    }

    internal unsafe struct VkWriteDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet dstSet;
        public UInt32 dstBinding;
        public UInt32 dstArrayElement;
        public UInt32 descriptorCount;
        public VkDescriptorType descriptorType;
        public VkDescriptorImageInfo* pImageInfo;
        public VkDescriptorBufferInfo* pBufferInfo;
        public VkBufferView* pTexelBufferView;
    }

    internal unsafe struct VkCopyDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet srcSet;
        public UInt32 srcBinding;
        public UInt32 srcArrayElement;
        public VkDescriptorSet dstSet;
        public UInt32 dstBinding;
        public UInt32 dstArrayElement;
        public UInt32 descriptorCount;
    }

    internal unsafe struct VkFramebufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFramebufferCreateFlags flags;
        public VkRenderPass renderPass;
        public UInt32 attachmentCount;
        public VkImageView* pAttachments;
        public UInt32 width;
        public UInt32 height;
        public UInt32 layers;
    }

    internal unsafe struct VkAttachmentDescription
    {
        public VkAttachmentDescriptionFlags flags;
        public VkFormat format;
        public VkSampleCountFlagBits samples;
        public VkAttachmentLoadOp loadOp;
        public VkAttachmentStoreOp storeOp;
        public VkAttachmentLoadOp stencilLoadOp;
        public VkAttachmentStoreOp stencilStoreOp;
        public VkImageLayout initialLayout;
        public VkImageLayout finalLayout;
    }

    internal unsafe struct VkAttachmentReference
    {
        public UInt32 attachment;
        public VkImageLayout layout;
    }

    internal unsafe struct VkSubpassDescription
    {
        public VkSubpassDescriptionFlags flags;
        public VkPipelineBindPoint pipelineBindPoint;
        public UInt32 inputAttachmentCount;
        public VkAttachmentReference* pInputAttachments;
        public UInt32 colorAttachmentCount;
        public VkAttachmentReference* pColorAttachments;
        public VkAttachmentReference* pResolveAttachments;
        public VkAttachmentReference* pDepthStencilAttachment;
        public UInt32 preserveAttachmentCount;
        public UInt32* pPreserveAttachments;
    }

    internal unsafe struct VkSubpassDependency
    {
        public UInt32 srcSubpass;
        public UInt32 dstSubpass;
        public VkPipelineStageFlags srcStageMask;
        public VkPipelineStageFlags dstStageMask;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkDependencyFlags dependencyFlags;
    }

    internal unsafe struct VkRenderPassCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPassCreateFlags flags;
        public UInt32 attachmentCount;
        public VkAttachmentDescription* pAttachments;
        public UInt32 subpassCount;
        public VkSubpassDescription* pSubpasses;
        public UInt32 dependencyCount;
        public VkSubpassDependency* pDependencies;
    }

    internal unsafe struct VkCommandPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPoolCreateFlags flags;
        public UInt32 queueFamilyIndex;
    }

    internal unsafe struct VkCommandBufferAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPool commandPool;
        public VkCommandBufferLevel level;
        public UInt32 commandBufferCount;
    }

    internal unsafe struct VkCommandBufferInheritanceInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public UInt32 subpass;
        public VkFramebuffer framebuffer;
        public VkBool32 occlusionQueryEnable;
        public VkQueryControlFlags queryFlags;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
    }

    internal unsafe struct VkCommandBufferBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandBufferUsageFlags flags;
        public VkCommandBufferInheritanceInfo* pInheritanceInfo;
    }

    internal unsafe struct VkBufferCopy
    {
        public VkDeviceSize srcOffset;
        public VkDeviceSize dstOffset;
        public VkDeviceSize size;
    }

    internal unsafe struct VkImageSubresourceLayers
    {
        public VkImageAspectFlags aspectMask;
        public UInt32 mipLevel;
        public UInt32 baseArrayLayer;
        public UInt32 layerCount;
    }

    internal unsafe struct VkImageCopy
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    internal unsafe struct VkImageBlit
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffsets0;
        public VkOffset3D srcOffsets1;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffsets0;
        public VkOffset3D dstOffsets1;
    }

    internal unsafe struct VkBufferImageCopy
    {
        public VkDeviceSize bufferOffset;
        public UInt32 bufferRowLength;
        public UInt32 bufferImageHeight;
        public VkImageSubresourceLayers imageSubresource;
        public VkOffset3D imageOffset;
        public VkExtent3D imageExtent;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal partial struct VkClearColorValue
    {
        [FieldOffset(0)]
        public float float32_0;
        [FieldOffset(4)]
        public float float32_1;
        [FieldOffset(8)]
        public float float32_2;
        [FieldOffset(12)]
        public float float32_3;
        [FieldOffset(0)]
        public int int32_0;
        [FieldOffset(4)]
        public int int32_1;
        [FieldOffset(8)]
        public int int32_2;
        [FieldOffset(12)]
        public int int32_3;
        [FieldOffset(0)]
        public uint uint32_0;
        [FieldOffset(4)]
        public uint uint32_1;
        [FieldOffset(8)]
        public uint uint32_2;
        [FieldOffset(12)]
        public uint uint32_3;
    }

    internal unsafe struct VkClearDepthStencilValue
    {
        public float depth;
        public UInt32 stencil;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal partial struct VkClearValue
    {
        [FieldOffset(0)]
        public VkClearColorValue color;
        [FieldOffset(0)]
        public VkClearDepthStencilValue depthStencil;
    }

    internal unsafe struct VkClearAttachment
    {
        public VkImageAspectFlags aspectMask;
        public UInt32 colorAttachment;
        public VkClearValue clearValue;
    }

    internal unsafe struct VkClearRect
    {
        public VkRect2D rect;
        public UInt32 baseArrayLayer;
        public UInt32 layerCount;
    }

    internal unsafe struct VkImageResolve
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    internal unsafe struct VkMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
    }

    internal unsafe struct VkBufferMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public UInt32 srcQueueFamilyIndex;
        public UInt32 dstQueueFamilyIndex;
        public VkBuffer buffer;
        public VkDeviceSize offset;
        public VkDeviceSize size;
    }

    internal unsafe struct VkImageMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkImageLayout oldLayout;
        public VkImageLayout newLayout;
        public UInt32 srcQueueFamilyIndex;
        public UInt32 dstQueueFamilyIndex;
        public VkImage image;
        public VkImageSubresourceRange subresourceRange;
    }

    internal unsafe struct VkRenderPassBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public VkFramebuffer framebuffer;
        public VkRect2D renderArea;
        public UInt32 clearValueCount;
        public VkClearValue* pClearValues;
    }

    internal unsafe struct VkDispatchIndirectCommand
    {
        public UInt32 x;
        public UInt32 y;
        public UInt32 z;
    }

    internal unsafe struct VkDrawIndexedIndirectCommand
    {
        public UInt32 indexCount;
        public UInt32 instanceCount;
        public UInt32 firstIndex;
        public Int32 vertexOffset;
        public UInt32 firstInstance;
    }

    internal unsafe struct VkDrawIndirectCommand
    {
        public UInt32 vertexCount;
        public UInt32 instanceCount;
        public UInt32 firstVertex;
        public UInt32 firstInstance;
    }
}
