using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0649

namespace Foster.Vulkan
{
    using VkFlags = UInt32;
    using VkBool32 = UInt32;
    using VkDeviceSize = UInt64;
    using VkSampleMask = UInt32;

    #region VK Core

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

    internal static partial class VkConst
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

    internal enum VkPipelineCacheHeaderVersion
    {
        One = 1,
    }

    internal enum VkResult
    {
        Success = 0,
        NotReady = 1,
        Timeout = 2,
        EventSet = 3,
        EventReset = 4,
        Incomplete = 5,
        ErrorOutOfHostMemory = -1,
        ErrorOutOfDeviceMemory = -2,
        ErrorInitializationFailed = -3,
        ErrorDeviceLost = -4,
        ErrorMemoryMapFailed = -5,
        ErrorLayerNotPresent = -6,
        ErrorExtensionNotPresent = -7,
        ErrorFeatureNotPresent = -8,
        ErrorIncompatibleDriver = -9,
        ErrorTooManyObjects = -10,
        ErrorFormatNotSupported = -11,
        ErrorFragmentedPool = -12,
        ErrorSurfaceLostKhr = -1000000000,
        ErrorNativeWindowInUseKhr = -1000000001,
        SuboptimalKhr = 1000001003,
        ErrorOutOfDateKhr = -1000001004,
        ErrorIncompatibleDisplayKhr = -1000003001,
        ErrorValidationFailedExt = -1000011001,
        ErrorInvalidShaderNv = -1000012000,
    }

    internal enum VkStructureType
    {
        ApplicationInfo = 0,
        InstanceCreateInfo = 1,
        DeviceQueueCreateInfo = 2,
        DeviceCreateInfo = 3,
        SubmitInfo = 4,
        MemoryAllocateInfo = 5,
        MappedMemoryRange = 6,
        BindSparseInfo = 7,
        FenceCreateInfo = 8,
        SemaphoreCreateInfo = 9,
        EventCreateInfo = 10,
        QueryPoolCreateInfo = 11,
        BufferCreateInfo = 12,
        BufferViewCreateInfo = 13,
        ImageCreateInfo = 14,
        ImageViewCreateInfo = 15,
        ShaderModuleCreateInfo = 16,
        PipelineCacheCreateInfo = 17,
        PipelineShaderStageCreateInfo = 18,
        PipelineVertexInputStateCreateInfo = 19,
        PipelineInputAssemblyStateCreateInfo = 20,
        PipelineTessellationStateCreateInfo = 21,
        PipelineViewportStateCreateInfo = 22,
        PipelineRasterizationStateCreateInfo = 23,
        PipelineMultisampleStateCreateInfo = 24,
        PipelineDepthStencilStateCreateInfo = 25,
        PipelineColorBlendStateCreateInfo = 26,
        PipelineDynamicStateCreateInfo = 27,
        GraphicsPipelineCreateInfo = 28,
        ComputePipelineCreateInfo = 29,
        PipelineLayoutCreateInfo = 30,
        SamplerCreateInfo = 31,
        DescriptorSetLayoutCreateInfo = 32,
        DescriptorPoolCreateInfo = 33,
        DescriptorSetAllocateInfo = 34,
        WriteDescriptorSet = 35,
        CopyDescriptorSet = 36,
        FramebufferCreateInfo = 37,
        RenderPassCreateInfo = 38,
        CommandPoolCreateInfo = 39,
        CommandBufferAllocateInfo = 40,
        CommandBufferInheritanceInfo = 41,
        CommandBufferBeginInfo = 42,
        RenderPassBeginInfo = 43,
        BufferMemoryBarrier = 44,
        ImageMemoryBarrier = 45,
        MemoryBarrier = 46,
        LoaderInstanceCreateInfo = 47,
        LoaderDeviceCreateInfo = 48,
        SwapchainCreateInfoKhr = 1000001000,
        PresentInfoKhr = 1000001001,
        DisplayModeCreateInfoKhr = 1000002000,
        DisplaySurfaceCreateInfoKhr = 1000002001,
        DisplayPresentInfoKhr = 1000003000,
        XlibSurfaceCreateInfoKhr = 1000004000,
        XcbSurfaceCreateInfoKhr = 1000005000,
        WaylandSurfaceCreateInfoKhr = 1000006000,
        MirSurfaceCreateInfoKhr = 1000007000,
        AndroidSurfaceCreateInfoKhr = 1000008000,
        Win32SurfaceCreateInfoKhr = 1000009000,
        DebugReportCallbackCreateInfoExt = 1000011000,
        PipelineRasterizationStateRasterizationOrderAmd = 1000018000,
        DebugMarkerObjectNameInfoExt = 1000022000,
        DebugMarkerObjectTagInfoExt = 1000022001,
        DebugMarkerMarkerInfoExt = 1000022002,
        DedicatedAllocationImageCreateInfoNv = 1000026000,
        DedicatedAllocationBufferCreateInfoNv = 1000026001,
        DedicatedAllocationMemoryAllocateInfoNv = 1000026002,
        ExternalMemoryImageCreateInfoNv = 1000056000,
        ExportMemoryAllocateInfoNv = 1000056001,
        ImportMemoryWin32HandleInfoNv = 1000057000,
        ExportMemoryWin32HandleInfoNv = 1000057001,
        Win32KeyedMutexAcquireReleaseInfoNv = 1000058000,
        ValidationFlagsExt = 1000061000,
        VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT = 1000128004
    }

    internal enum VkSystemAllocationScope
    {
        Command = 0,
        Object = 1,
        Cache = 2,
        Device = 3,
        Instance = 4,
    }

    internal enum VkInternalAllocationType
    {
        Executable = 0,
    }

    internal enum VkFormat
    {
        Undefined = 0,
        R4g4UnormPack8 = 1,
        R4g4b4a4UnormPack16 = 2,
        B4g4r4a4UnormPack16 = 3,
        R5g6b5UnormPack16 = 4,
        B5g6r5UnormPack16 = 5,
        R5g5b5a1UnormPack16 = 6,
        B5g5r5a1UnormPack16 = 7,
        A1r5g5b5UnormPack16 = 8,
        R8Unorm = 9,
        R8Snorm = 10,
        R8Uscaled = 11,
        R8Sscaled = 12,
        R8Uint = 13,
        R8Sint = 14,
        R8Srgb = 15,
        R8g8Unorm = 16,
        R8g8Snorm = 17,
        R8g8Uscaled = 18,
        R8g8Sscaled = 19,
        R8g8Uint = 20,
        R8g8Sint = 21,
        R8g8Srgb = 22,
        R8g8b8Unorm = 23,
        R8g8b8Snorm = 24,
        R8g8b8Uscaled = 25,
        R8g8b8Sscaled = 26,
        R8g8b8Uint = 27,
        R8g8b8Sint = 28,
        R8g8b8Srgb = 29,
        B8g8r8Unorm = 30,
        B8g8r8Snorm = 31,
        B8g8r8Uscaled = 32,
        B8g8r8Sscaled = 33,
        B8g8r8Uint = 34,
        B8g8r8Sint = 35,
        B8g8r8Srgb = 36,
        R8g8b8a8Unorm = 37,
        R8g8b8a8Snorm = 38,
        R8g8b8a8Uscaled = 39,
        R8g8b8a8Sscaled = 40,
        R8g8b8a8Uint = 41,
        R8g8b8a8Sint = 42,
        R8g8b8a8Srgb = 43,
        B8g8r8a8Unorm = 44,
        B8g8r8a8Snorm = 45,
        B8g8r8a8Uscaled = 46,
        B8g8r8a8Sscaled = 47,
        B8g8r8a8Uint = 48,
        B8g8r8a8Sint = 49,
        B8g8r8a8Srgb = 50,
        A8b8g8r8UnormPack32 = 51,
        A8b8g8r8SnormPack32 = 52,
        A8b8g8r8UscaledPack32 = 53,
        A8b8g8r8SscaledPack32 = 54,
        A8b8g8r8UintPack32 = 55,
        A8b8g8r8SintPack32 = 56,
        A8b8g8r8SrgbPack32 = 57,
        A2r10g10b10UnormPack32 = 58,
        A2r10g10b10SnormPack32 = 59,
        A2r10g10b10UscaledPack32 = 60,
        A2r10g10b10SscaledPack32 = 61,
        A2r10g10b10UintPack32 = 62,
        A2r10g10b10SintPack32 = 63,
        A2b10g10r10UnormPack32 = 64,
        A2b10g10r10SnormPack32 = 65,
        A2b10g10r10UscaledPack32 = 66,
        A2b10g10r10SscaledPack32 = 67,
        A2b10g10r10UintPack32 = 68,
        A2b10g10r10SintPack32 = 69,
        R16Unorm = 70,
        R16Snorm = 71,
        R16Uscaled = 72,
        R16Sscaled = 73,
        R16Uint = 74,
        R16Sint = 75,
        R16Sfloat = 76,
        R16g16Unorm = 77,
        R16g16Snorm = 78,
        R16g16Uscaled = 79,
        R16g16Sscaled = 80,
        R16g16Uint = 81,
        R16g16Sint = 82,
        R16g16Sfloat = 83,
        R16g16b16Unorm = 84,
        R16g16b16Snorm = 85,
        R16g16b16Uscaled = 86,
        R16g16b16Sscaled = 87,
        R16g16b16Uint = 88,
        R16g16b16Sint = 89,
        R16g16b16Sfloat = 90,
        R16g16b16a16Unorm = 91,
        R16g16b16a16Snorm = 92,
        R16g16b16a16Uscaled = 93,
        R16g16b16a16Sscaled = 94,
        R16g16b16a16Uint = 95,
        R16g16b16a16Sint = 96,
        R16g16b16a16Sfloat = 97,
        R32Uint = 98,
        R32Sint = 99,
        R32Sfloat = 100,
        R32g32Uint = 101,
        R32g32Sint = 102,
        R32g32Sfloat = 103,
        R32g32b32Uint = 104,
        R32g32b32Sint = 105,
        R32g32b32Sfloat = 106,
        R32g32b32a32Uint = 107,
        R32g32b32a32Sint = 108,
        R32g32b32a32Sfloat = 109,
        R64Uint = 110,
        R64Sint = 111,
        R64Sfloat = 112,
        R64g64Uint = 113,
        R64g64Sint = 114,
        R64g64Sfloat = 115,
        R64g64b64Uint = 116,
        R64g64b64Sint = 117,
        R64g64b64Sfloat = 118,
        R64g64b64a64Uint = 119,
        R64g64b64a64Sint = 120,
        R64g64b64a64Sfloat = 121,
        B10g11r11UfloatPack32 = 122,
        E5b9g9r9UfloatPack32 = 123,
        D16Unorm = 124,
        X8D24UnormPack32 = 125,
        D32Sfloat = 126,
        S8Uint = 127,
        D16UnormS8Uint = 128,
        D24UnormS8Uint = 129,
        D32SfloatS8Uint = 130,
        Bc1RgbUnormBlock = 131,
        Bc1RgbSrgbBlock = 132,
        Bc1RgbaUnormBlock = 133,
        Bc1RgbaSrgbBlock = 134,
        Bc2UnormBlock = 135,
        Bc2SrgbBlock = 136,
        Bc3UnormBlock = 137,
        Bc3SrgbBlock = 138,
        Bc4UnormBlock = 139,
        Bc4SnormBlock = 140,
        Bc5UnormBlock = 141,
        Bc5SnormBlock = 142,
        Bc6hUfloatBlock = 143,
        Bc6hSfloatBlock = 144,
        Bc7UnormBlock = 145,
        Bc7SrgbBlock = 146,
        Etc2R8g8b8UnormBlock = 147,
        Etc2R8g8b8SrgbBlock = 148,
        Etc2R8g8b8a1UnormBlock = 149,
        Etc2R8g8b8a1SrgbBlock = 150,
        Etc2R8g8b8a8UnormBlock = 151,
        Etc2R8g8b8a8SrgbBlock = 152,
        EacR11UnormBlock = 153,
        EacR11SnormBlock = 154,
        EacR11g11UnormBlock = 155,
        EacR11g11SnormBlock = 156,
        Astc4x4UnormBlock = 157,
        Astc4x4SrgbBlock = 158,
        Astc5x4UnormBlock = 159,
        Astc5x4SrgbBlock = 160,
        Astc5x5UnormBlock = 161,
        Astc5x5SrgbBlock = 162,
        Astc6x5UnormBlock = 163,
        Astc6x5SrgbBlock = 164,
        Astc6x6UnormBlock = 165,
        Astc6x6SrgbBlock = 166,
        Astc8x5UnormBlock = 167,
        Astc8x5SrgbBlock = 168,
        Astc8x6UnormBlock = 169,
        Astc8x6SrgbBlock = 170,
        Astc8x8UnormBlock = 171,
        Astc8x8SrgbBlock = 172,
        Astc10x5UnormBlock = 173,
        Astc10x5SrgbBlock = 174,
        Astc10x6UnormBlock = 175,
        Astc10x6SrgbBlock = 176,
        Astc10x8UnormBlock = 177,
        Astc10x8SrgbBlock = 178,
        Astc10x10UnormBlock = 179,
        Astc10x10SrgbBlock = 180,
        Astc12x10UnormBlock = 181,
        Astc12x10SrgbBlock = 182,
        Astc12x12UnormBlock = 183,
        Astc12x12SrgbBlock = 184,
        Pvrtc12bppUnormBlockImg = 1000054000,
        Pvrtc14bppUnormBlockImg = 1000054001,
        Pvrtc22bppUnormBlockImg = 1000054002,
        Pvrtc24bppUnormBlockImg = 1000054003,
        Pvrtc12bppSrgbBlockImg = 1000054004,
        Pvrtc14bppSrgbBlockImg = 1000054005,
        Pvrtc22bppSrgbBlockImg = 1000054006,
        Pvrtc24bppSrgbBlockImg = 1000054007,
    }

    internal enum VkImageType
    {
        Image1d = 0,
        Image2d = 1,
        Image3d = 2,
    }

    internal enum VkImageTiling
    {
        Optimal = 0,
        Linear = 1,
    }

    internal enum VkPhysicalDeviceType
    {
        Other = 0,
        IntegratedGpu = 1,
        DiscreteGpu = 2,
        VirtualGpu = 3,
        Cpu = 4,
    }

    internal enum VkQueryType
    {
        Occlusion = 0,
        PipelineStatistics = 1,
        Timestamp = 2,
    }

    internal enum VkSharingMode
    {
        Exclusive = 0,
        Concurrent = 1,
    }

    internal enum VkImageLayout
    {
        Undefined = 0,
        General = 1,
        ColorAttachmentOptimal = 2,
        DepthStencilAttachmentOptimal = 3,
        DepthStencilReadOnlyOptimal = 4,
        ShaderReadOnlyOptimal = 5,
        TransferSrcOptimal = 6,
        TransferDstOptimal = 7,
        Preinitialized = 8,
        PresentSrcKhr = 1000001002,
    }

    internal enum VkImageViewType
    {
        Image1d = 0,
        Image2d = 1,
        Image3d = 2,
        ImageCube = 3,
        Image1dArray = 4,
        Image2dArray = 5,
        ImageCubeArray = 6,
    }

    internal enum VkComponentSwizzle
    {
        Identity = 0,
        Zero = 1,
        One = 2,
        R = 3,
        G = 4,
        B = 5,
        A = 6,
    }

    internal enum VkVertexInputRate
    {
        Vertex = 0,
        Instance = 1,
    }

    internal enum VkPrimitiveTopology
    {
        PointList = 0,
        LineList = 1,
        LineStrip = 2,
        TriangleList = 3,
        TriangleStrip = 4,
        TriangleFan = 5,
        LineListWithAdjacency = 6,
        LineStripWithAdjacency = 7,
        TriangleListWithAdjacency = 8,
        TriangleStripWithAdjacency = 9,
        PatchList = 10,
    }

    internal enum VkPolygonMode
    {
        Fill = 0,
        Line = 1,
        Point = 2,
    }

    internal enum VkFrontFace
    {
        CounterClockwise = 0,
        Clockwise = 1,
    }

    internal enum VkCompareOp
    {
        Never = 0,
        Less = 1,
        Equal = 2,
        LessOrEqual = 3,
        Greater = 4,
        NotEqual = 5,
        GreaterOrEqual = 6,
        Always = 7,
    }

    internal enum VkStencilOp
    {
        Keep = 0,
        Zero = 1,
        Replace = 2,
        IncrementAndClamp = 3,
        DecrementAndClamp = 4,
        Invert = 5,
        IncrementAndWrap = 6,
        DecrementAndWrap = 7,
    }

    internal enum VkLogicOp
    {
        Clear = 0,
        And = 1,
        AndReverse = 2,
        Copy = 3,
        AndInverted = 4,
        NoOp = 5,
        Xor = 6,
        Or = 7,
        Nor = 8,
        Equivalent = 9,
        Invert = 10,
        OrReverse = 11,
        CopyInverted = 12,
        OrInverted = 13,
        Nand = 14,
        Set = 15,
    }

    internal enum VkBlendFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 2,
        OneMinusSrcColor = 3,
        DstColor = 4,
        OneMinusDstColor = 5,
        SrcAlpha = 6,
        OneMinusSrcAlpha = 7,
        DstAlpha = 8,
        OneMinusDstAlpha = 9,
        ConstantColor = 10,
        OneMinusConstantColor = 11,
        ConstantAlpha = 12,
        OneMinusConstantAlpha = 13,
        SrcAlphaSaturate = 14,
        Src1Color = 15,
        OneMinusSrc1Color = 16,
        Src1Alpha = 17,
        OneMinusSrc1Alpha = 18,
    }

    internal enum VkBlendOp
    {
        Add = 0,
        Subtract = 1,
        ReverseSubtract = 2,
        Min = 3,
        Max = 4,
    }

    internal enum VkDynamicState
    {
        Viewport = 0,
        Scissor = 1,
        LineWidth = 2,
        DepthBias = 3,
        BlendConstants = 4,
        DepthBounds = 5,
        StencilCompareMask = 6,
        StencilWriteMask = 7,
        StencilReference = 8,
    }

    internal enum VkFilter
    {
        Nearest = 0,
        Linear = 1,
        CubicImg = 1000015000,
    }

    internal enum VkSamplerMipmapMode
    {
        Nearest = 0,
        Linear = 1,
    }

    internal enum VkSamplerAddressMode
    {
        Repeat = 0,
        MirroredRepeat = 1,
        ClampToEdge = 2,
        ClampToBorder = 3,
        MirrorClampToEdge = 4,
    }

    internal enum VkBorderColor
    {
        FloatTransparentBlack = 0,
        IntTransparentBlack = 1,
        FloatOpaqueBlack = 2,
        IntOpaqueBlack = 3,
        FloatOpaqueWhite = 4,
        IntOpaqueWhite = 5,
    }

    internal enum VkDescriptorType
    {
        Sampler = 0,
        CombinedImageSampler = 1,
        SampledImage = 2,
        StorageImage = 3,
        UniformTexelBuffer = 4,
        StorageTexelBuffer = 5,
        UniformBuffer = 6,
        StorageBuffer = 7,
        UniformBufferDynamic = 8,
        StorageBufferDynamic = 9,
        InputAttachment = 10,
    }

    internal enum VkAttachmentLoadOp
    {
        Load = 0,
        Clear = 1,
        DontCare = 2,
    }

    internal enum VkAttachmentStoreOp
    {
        Store = 0,
        DontCare = 1,
    }

    internal enum VkPipelineBindPoint
    {
        Graphics = 0,
        Compute = 1,
    }

    internal enum VkCommandBufferLevel
    {
        Primary = 0,
        Secondary = 1,
    }

    internal enum VkIndexType
    {
        Uint16 = 0,
        Uint32 = 1,
    }

    internal enum VkSubpassContents
    {
        Inline = 0,
        SecondaryCommandBuffers = 1,
    }

    internal enum VkObjectType
    {
        Unknown = 0,
        Instance = 1,
        PhysicalDevice = 2,
        Device = 3,
        Queue = 4,
        Semaphore = 5,
        CommandBuffer = 6,
        Fence = 7,
        DeviceMemory = 8,
        Buffer = 9,
        Image = 10,
        Event = 11,
        QueryPool = 12,
        BufferView = 13,
        ImageView = 14,
        ShaderModule = 15,
        PipelineCache = 16,
        PipelineLayout = 17,
        RenderPass = 18,
        Pipeline = 19,
        DescriptorSetLayout = 20,
        Sampler = 21,
        DescriptorPool = 22,
        DescriptorSet = 23,
        Framebuffer = 24,
        CommandPool = 25,
        SamplerYCBCRConversion = 1000156000,
        DescriptorUpdateTemplate = 1000085000,
        SurfaceKHR = 1000000000,
        SwapchainKHR = 1000001000,
        DisplayKHR = 1000002000,
        DisplayModeKHR = 1000002001,
        DebugReportCallbackEXT = 1000011000,
        ObjectTableNVX = 1000086000,
        IndirectCommandsLayoutNVX = 1000086001,
        DebugUtilsMessengerEXT = 1000128000,
        ValidationCacheEXT = 1000160000,
        AccelerationStructureNV = 1000165000,
        PerformanceConfigurationNV = 1000210000,
        DescriptorUpdateTEmplateKHR = DescriptorUpdateTemplate,
        SamplerYCBCRConversionKHR = SamplerYCBCRConversion,
    }

    internal unsafe struct VkInstanceCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkInstanceCreateFlags value) => value.Flag;
        public static implicit operator VkInstanceCreateFlags(VkFlags flag) => new VkInstanceCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkFormatFeatureFlags
    {
        SampledImageBit = 0x00000001,
        StorageImageBit = 0x00000002,
        StorageImageAtomicBit = 0x00000004,
        UniformTexelBufferBit = 0x00000008,
        StorageTexelBufferBit = 0x00000010,
        StorageTexelBufferAtomicBit = 0x00000020,
        VertexBufferBit = 0x00000040,
        ColorAttachmentBit = 0x00000080,
        ColorAttachmentBlendBit = 0x00000100,
        DepthStencilAttachmentBit = 0x00000200,
        BlitSrcBit = 0x00000400,
        BlitDstBit = 0x00000800,
        SampledImageFilterLinearBit = 0x00001000,
        SampledImageFilterCubicBitImg = 0x00002000,
    }

    [Flags]
    internal enum VkImageUsageFlags
    {
        TransferSrcBit = 0x00000001,
        TransferDstBit = 0x00000002,
        SampledBit = 0x00000004,
        StorageBit = 0x00000008,
        ColorAttachmentBit = 0x00000010,
        DepthStencilAttachmentBit = 0x00000020,
        TransientAttachmentBit = 0x00000040,
        InputAttachmentBit = 0x00000080,
    }

    [Flags]
    internal enum VkImageCreateFlags
    {
        SparseBindingBit = 0x00000001,
        SparseResidencyBit = 0x00000002,
        SparseAliasedBit = 0x00000004,
        MutableFormatBit = 0x00000008,
        CubeCompatibleBit = 0x00000010,
    }

    [Flags]
    internal enum VkSampleCountFlags
    {
        Count1Bit = 0x00000001,
        Count2Bit = 0x00000002,
        Count4Bit = 0x00000004,
        Count8Bit = 0x00000008,
        Count16Bit = 0x00000010,
        Count32Bit = 0x00000020,
        Count64Bit = 0x00000040,
    }

    [Flags]
    internal enum VkQueueFlags
    {
        GraphicsBit = 0x00000001,
        ComputeBit = 0x00000002,
        TransferBit = 0x00000004,
        SparseBindingBit = 0x00000008,
    }

    [Flags]
    internal enum VkMemoryPropertyFlags
    {
        DeviceLocalBit = 0x00000001,
        HostVisibleBit = 0x00000002,
        HostCoherentBit = 0x00000004,
        HostCachedBit = 0x00000008,
        LazilyAllocatedBit = 0x00000010,
    }

    [Flags]
    internal enum VkMemoryHeapFlags
    {
        DeviceLocalBit = 0x00000001,
    }

    internal unsafe struct VkDeviceCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDeviceCreateFlags value) => value.Flag;
        public static implicit operator VkDeviceCreateFlags(VkFlags flag) => new VkDeviceCreateFlags { Flag = flag };
    }

    internal unsafe struct VkDeviceQueueCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDeviceQueueCreateFlags value) => value.Flag;
        public static implicit operator VkDeviceQueueCreateFlags(VkFlags flag) => new VkDeviceQueueCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkPipelineStageFlags
    {
        TopOfPipeBit = 0x00000001,
        DrawIndirectBit = 0x00000002,
        VertexInputBit = 0x00000004,
        VertexShaderBit = 0x00000008,
        TessellationControlShaderBit = 0x00000010,
        TessellationEvaluationShaderBit = 0x00000020,
        GeometryShaderBit = 0x00000040,
        FragmentShaderBit = 0x00000080,
        EarlyFragmentTestsBit = 0x00000100,
        LateFragmentTestsBit = 0x00000200,
        ColorAttachmentOutputBit = 0x00000400,
        ComputeShaderBit = 0x00000800,
        TransferBit = 0x00001000,
        BottomOfPipeBit = 0x00002000,
        HostBit = 0x00004000,
        AllGraphicsBit = 0x00008000,
        AllCommandsBit = 0x00010000,
    }

    internal unsafe struct VkMemoryMapFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkMemoryMapFlags value) => value.Flag;
        public static implicit operator VkMemoryMapFlags(VkFlags flag) => new VkMemoryMapFlags { Flag = flag };
    }

    [Flags]
    internal enum VkImageAspectFlags
    {
        ColorBit = 0x00000001,
        DepthBit = 0x00000002,
        StencilBit = 0x00000004,
        MetadataBit = 0x00000008,
    }

    [Flags]
    internal enum VkSparseImageFormatFlags
    {
        SingleMiptailBit = 0x00000001,
        AlignedMipSizeBit = 0x00000002,
        NonstandardBlockSizeBit = 0x00000004,
    }

    [Flags]
    internal enum VkSparseMemoryBindFlags
    {
        MetadataBit = 0x00000001,
    }

    [Flags]
    internal enum VkFenceCreateFlags
    {
        SignaledBit = 0x00000001,
    }

    internal unsafe struct VkSemaphoreCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkSemaphoreCreateFlags value) => value.Flag;
        public static implicit operator VkSemaphoreCreateFlags(VkFlags flag) => new VkSemaphoreCreateFlags { Flag = flag };
    }

    internal unsafe struct VkEventCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkEventCreateFlags value) => value.Flag;
        public static implicit operator VkEventCreateFlags(VkFlags flag) => new VkEventCreateFlags { Flag = flag };
    }

    internal unsafe struct VkQueryPoolCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkQueryPoolCreateFlags value) => value.Flag;
        public static implicit operator VkQueryPoolCreateFlags(VkFlags flag) => new VkQueryPoolCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkQueryPipelineStatisticFlags
    {
        InputAssemblyVerticesBit = 0x00000001,
        InputAssemblyPrimitivesBit = 0x00000002,
        VertexShaderInvocationsBit = 0x00000004,
        GeometryShaderInvocationsBit = 0x00000008,
        GeometryShaderPrimitivesBit = 0x00000010,
        ClippingInvocationsBit = 0x00000020,
        ClippingPrimitivesBit = 0x00000040,
        FragmentShaderInvocationsBit = 0x00000080,
        TessellationControlShaderPatchesBit = 0x00000100,
        TessellationEvaluationShaderInvocationsBit = 0x00000200,
        ComputeShaderInvocationsBit = 0x00000400,
    }

    [Flags]
    internal enum VkQueryResultFlags
    {
        Result64Bit = 0x00000001,
        ResultWaitBit = 0x00000002,
        ResultWithAvailabilityBit = 0x00000004,
        ResultPartialBit = 0x00000008,
    }

    [Flags]
    internal enum VkBufferCreateFlags
    {
        SparseBindingBit = 0x00000001,
        SparseResidencyBit = 0x00000002,
        SparseAliasedBit = 0x00000004,
    }

    [Flags]
    internal enum VkBufferUsageFlags
    {
        TransferSrcBit = 0x00000001,
        TransferDstBit = 0x00000002,
        UniformTexelBufferBit = 0x00000004,
        StorageTexelBufferBit = 0x00000008,
        UniformBufferBit = 0x00000010,
        StorageBufferBit = 0x00000020,
        IndexBufferBit = 0x00000040,
        VertexBufferBit = 0x00000080,
        IndirectBufferBit = 0x00000100,
    }

    internal unsafe struct VkBufferViewCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkBufferViewCreateFlags value) => value.Flag;
        public static implicit operator VkBufferViewCreateFlags(VkFlags flag) => new VkBufferViewCreateFlags { Flag = flag };
    }

    internal unsafe struct VkImageViewCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkImageViewCreateFlags value) => value.Flag;
        public static implicit operator VkImageViewCreateFlags(VkFlags flag) => new VkImageViewCreateFlags { Flag = flag };
    }

    internal unsafe struct VkShaderModuleCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkShaderModuleCreateFlags value) => value.Flag;
        public static implicit operator VkShaderModuleCreateFlags(VkFlags flag) => new VkShaderModuleCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineCacheCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineCacheCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineCacheCreateFlags(VkFlags flag) => new VkPipelineCacheCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkPipelineCreateFlags
    {
        DisableOptimizationBit = 0x00000001,
        AllowDerivativesBit = 0x00000002,
        DerivativeBit = 0x00000004,
    }

    internal unsafe struct VkPipelineShaderStageCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineShaderStageCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineShaderStageCreateFlags(VkFlags flag) => new VkPipelineShaderStageCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkShaderStageFlags
    {
        VertexBit = 0x00000001,
        TessellationControlBit = 0x00000002,
        TessellationEvaluationBit = 0x00000004,
        GeometryBit = 0x00000008,
        FragmentBit = 0x00000010,
        ComputeBit = 0x00000020,
        AllGraphics = 0x0000001f,
        All = 0x7fffffff,
    }

    internal unsafe struct VkPipelineVertexInputStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineVertexInputStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineVertexInputStateCreateFlags(VkFlags flag) => new VkPipelineVertexInputStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineInputAssemblyStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineInputAssemblyStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineInputAssemblyStateCreateFlags(VkFlags flag) => new VkPipelineInputAssemblyStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineTessellationStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineTessellationStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineTessellationStateCreateFlags(VkFlags flag) => new VkPipelineTessellationStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineViewportStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineViewportStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineViewportStateCreateFlags(VkFlags flag) => new VkPipelineViewportStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineRasterizationStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineRasterizationStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineRasterizationStateCreateFlags(VkFlags flag) => new VkPipelineRasterizationStateCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkCullModeFlags
    {
        None = 0,
        FrontBit = 0x00000001,
        BackBit = 0x00000002,
        FrontAndBack = 0x00000003,
    }

    internal unsafe struct VkPipelineMultisampleStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineMultisampleStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineMultisampleStateCreateFlags(VkFlags flag) => new VkPipelineMultisampleStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineDepthStencilStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineDepthStencilStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineDepthStencilStateCreateFlags(VkFlags flag) => new VkPipelineDepthStencilStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineColorBlendStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineColorBlendStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineColorBlendStateCreateFlags(VkFlags flag) => new VkPipelineColorBlendStateCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkColorComponentFlags
    {
        RBit = 0x00000001,
        GBit = 0x00000002,
        BBit = 0x00000004,
        ABit = 0x00000008,
    }

    internal unsafe struct VkPipelineDynamicStateCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineDynamicStateCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineDynamicStateCreateFlags(VkFlags flag) => new VkPipelineDynamicStateCreateFlags { Flag = flag };
    }

    internal unsafe struct VkPipelineLayoutCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkPipelineLayoutCreateFlags value) => value.Flag;
        public static implicit operator VkPipelineLayoutCreateFlags(VkFlags flag) => new VkPipelineLayoutCreateFlags { Flag = flag };
    }

    internal unsafe struct VkSamplerCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkSamplerCreateFlags value) => value.Flag;
        public static implicit operator VkSamplerCreateFlags(VkFlags flag) => new VkSamplerCreateFlags { Flag = flag };
    }

    internal unsafe struct VkDescriptorSetLayoutCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDescriptorSetLayoutCreateFlags value) => value.Flag;
        public static implicit operator VkDescriptorSetLayoutCreateFlags(VkFlags flag) => new VkDescriptorSetLayoutCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkDescriptorPoolCreateFlags
    {
        FreeDescriptorSetBit = 0x00000001,
    }

    internal unsafe struct VkDescriptorPoolResetFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDescriptorPoolResetFlags value) => value.Flag;
        public static implicit operator VkDescriptorPoolResetFlags(VkFlags flag) => new VkDescriptorPoolResetFlags { Flag = flag };
    }

    internal unsafe struct VkFramebufferCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkFramebufferCreateFlags value) => value.Flag;
        public static implicit operator VkFramebufferCreateFlags(VkFlags flag) => new VkFramebufferCreateFlags { Flag = flag };
    }

    internal unsafe struct VkRenderPassCreateFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkRenderPassCreateFlags value) => value.Flag;
        public static implicit operator VkRenderPassCreateFlags(VkFlags flag) => new VkRenderPassCreateFlags { Flag = flag };
    }

    [Flags]
    internal enum VkAttachmentDescriptionFlags
    {
        MayAliasBit = 0x00000001,
    }

    internal unsafe struct VkSubpassDescriptionFlags
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkSubpassDescriptionFlags value) => value.Flag;
        public static implicit operator VkSubpassDescriptionFlags(VkFlags flag) => new VkSubpassDescriptionFlags { Flag = flag };
    }

    [Flags]
    internal enum VkAccessFlags
    {
        IndirectCommandReadBit = 0x00000001,
        IndexReadBit = 0x00000002,
        VertexAttributeReadBit = 0x00000004,
        UniformReadBit = 0x00000008,
        InputAttachmentReadBit = 0x00000010,
        ShaderReadBit = 0x00000020,
        ShaderWriteBit = 0x00000040,
        ColorAttachmentReadBit = 0x00000080,
        ColorAttachmentWriteBit = 0x00000100,
        DepthStencilAttachmentReadBit = 0x00000200,
        DepthStencilAttachmentWriteBit = 0x00000400,
        TransferReadBit = 0x00000800,
        TransferWriteBit = 0x00001000,
        HostReadBit = 0x00002000,
        HostWriteBit = 0x00004000,
        MemoryReadBit = 0x00008000,
        MemoryWriteBit = 0x00010000,
    }

    [Flags]
    internal enum VkDependencyFlags
    {
        ByRegionBit = 0x00000001,
    }

    [Flags]
    internal enum VkCommandPoolCreateFlags
    {
        CreateTransientBit = 0x00000001,
        CreateResetCommandBufferBit = 0x00000002,
    }

    [Flags]
    internal enum VkCommandPoolResetFlags
    {
        ReleaseResourcesBit = 0x00000001,
    }

    [Flags]
    internal enum VkCommandBufferUsageFlags
    {
        OneTimeSubmitBit = 0x00000001,
        RenderPassContinueBit = 0x00000002,
        SimultaneousUseBit = 0x00000004,
    }

    [Flags]
    internal enum VkQueryControlFlags
    {
        PreciseBit = 0x00000001,
    }

    [Flags]
    internal enum VkCommandBufferResetFlags
    {
        ReleaseResourcesBit = 0x00000001,
    }

    [Flags]
    internal enum VkStencilFaceFlags
    {
        FaceFrontBit = 0x00000001,
        FaceBackBit = 0x00000002,
        FrontAndBack = 0x00000003,
    }

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
        fixed UInt32 maxComputeWorkGroupCount[3];
        public UInt32 maxComputeWorkGroupInvocations;
        fixed UInt32 maxComputeWorkGroupSize[3];
        public UInt32 subPixelPrecisionBits;
        public UInt32 subTexelPrecisionBits;
        public UInt32 mipmapPrecisionBits;
        public UInt32 maxDrawIndexedIndexValue;
        public UInt32 maxDrawIndirectCount;
        public float maxSamplerLodBias;
        public float maxSamplerAnisotropy;
        public UInt32 maxViewports;
        fixed UInt32 maxViewportDimensions[2];
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
        public fixed byte deviceName[(int)VkConst.MAX_PHYSICAL_DEVICE_NAME_SIZE];
        public fixed byte pipelineCacheUUID[(int)VkConst.UUID_SIZE];
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
        public fixed byte extensionName[(int)VkConst.MAX_EXTENSION_NAME_SIZE];
        public UInt32 specVersion;
    }

    internal unsafe struct VkLayerProperties
    {
        public fixed byte layerName[(int)VkConst.MAX_EXTENSION_NAME_SIZE];
        public UInt32 specVersion;
        public UInt32 implementationVersion;
        public fixed byte description[(int)VkConst.MAX_DESCRIPTION_SIZE];
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
        public VkSampleCountFlags samples;
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
        public VkShaderStageFlags stage;
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
        public VkSampleCountFlags rasterizationSamples;
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
        public VkSampleCountFlags samples;
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

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateInstance(
        VkInstanceCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        out VkInstance pInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyInstance(
        VkInstance instance,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEnumeratePhysicalDevices(
        VkInstance instance,
        UInt32* pPhysicalDeviceCount,
        VkPhysicalDevice* pPhysicalDevices);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceFeatures(
        VkPhysicalDevice physicalDevice,
        VkPhysicalDeviceFeatures* pFeatures);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceFormatProperties(
        VkPhysicalDevice physicalDevice,
        VkFormat format,
        VkFormatProperties* pFormatProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceImageFormatProperties(
        VkPhysicalDevice physicalDevice,
        VkFormat format,
        VkImageType type,
        VkImageTiling tiling,
        VkImageUsageFlags usage,
        VkImageCreateFlags flags,
        VkImageFormatProperties* pImageFormatProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceProperties(
        VkPhysicalDevice physicalDevice,
        VkPhysicalDeviceProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceQueueFamilyProperties(
        VkPhysicalDevice physicalDevice,
        UInt32* pQueueFamilyPropertyCount,
        VkQueueFamilyProperties* pQueueFamilyProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceMemoryProperties(
        VkPhysicalDevice physicalDevice,
        VkPhysicalDeviceMemoryProperties* pMemoryProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate IntPtr vkGetInstanceProcAddr(
        VkInstance instance,
        byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate IntPtr vkGetDeviceProcAddr(
        VkDevice device,
        byte* pName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDevice(
        VkPhysicalDevice physicalDevice,
        VkDeviceCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        out VkDevice pDevice);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyDevice(
        VkDevice device,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEnumerateInstanceExtensionProperties(
        byte* pLayerName,
        UInt32* pPropertyCount,
        VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEnumerateDeviceExtensionProperties(
        VkPhysicalDevice physicalDevice,
        byte* pLayerName,
        UInt32* pPropertyCount,
        VkExtensionProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEnumerateInstanceLayerProperties(
        UInt32* pPropertyCount,
        VkLayerProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEnumerateDeviceLayerProperties(
        VkPhysicalDevice physicalDevice,
        UInt32* pPropertyCount,
        VkLayerProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetDeviceQueue(
        VkDevice device,
        UInt32 queueFamilyIndex,
        UInt32 queueIndex,
        out VkQueue pQueue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkQueueSubmit(
        VkQueue queue,
        UInt32 submitCount,
        VkSubmitInfo* pSubmits,
        VkFence fence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkQueueWaitIdle(
        VkQueue queue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkDeviceWaitIdle(
        VkDevice device);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkAllocateMemory(
        VkDevice device,
        VkMemoryAllocateInfo* pAllocateInfo,
        VkAllocationCallbacks* pAllocator,
        VkDeviceMemory* pMemory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkFreeMemory(
        VkDevice device,
        VkDeviceMemory memory,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkMapMemory(
        VkDevice device,
        VkDeviceMemory memory,
        VkDeviceSize offset,
        VkDeviceSize size,
        VkMemoryMapFlags flags,
        void** ppData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkUnmapMemory(
        VkDevice device,
        VkDeviceMemory memory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkFlushMappedMemoryRanges(
        VkDevice device,
        UInt32 memoryRangeCount,
        VkMappedMemoryRange* pMemoryRanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkInvalidateMappedMemoryRanges(
        VkDevice device,
        UInt32 memoryRangeCount,
        VkMappedMemoryRange* pMemoryRanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetDeviceMemoryCommitment(
        VkDevice device,
        VkDeviceMemory memory,
        VkDeviceSize* pCommittedMemoryInBytes);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkBindBufferMemory(
        VkDevice device,
        VkBuffer buffer,
        VkDeviceMemory memory,
        VkDeviceSize memoryOffset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkBindImageMemory(
        VkDevice device,
        VkImage image,
        VkDeviceMemory memory,
        VkDeviceSize memoryOffset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetBufferMemoryRequirements(
        VkDevice device,
        VkBuffer buffer,
        VkMemoryRequirements* pMemoryRequirements);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetImageMemoryRequirements(
        VkDevice device,
        VkImage image,
        VkMemoryRequirements* pMemoryRequirements);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetImageSparseMemoryRequirements(
        VkDevice device,
        VkImage image,
        UInt32* pSparseMemoryRequirementCount,
        VkSparseImageMemoryRequirements* pSparseMemoryRequirements);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetPhysicalDeviceSparseImageFormatProperties(
        VkPhysicalDevice physicalDevice,
        VkFormat format,
        VkImageType type,
        VkSampleCountFlags samples,
        VkImageUsageFlags usage,
        VkImageTiling tiling,
        UInt32* pPropertyCount,
        VkSparseImageFormatProperties* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkQueueBindSparse(
        VkQueue queue,
        UInt32 bindInfoCount,
        VkBindSparseInfo* pBindInfo,
        VkFence fence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateFence(
        VkDevice device,
        VkFenceCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkFence* pFence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyFence(
        VkDevice device,
        VkFence fence,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkResetFences(
        VkDevice device,
        UInt32 fenceCount,
        VkFence* pFences);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetFenceStatus(
        VkDevice device,
        VkFence fence);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkWaitForFences(
        VkDevice device,
        UInt32 fenceCount,
        VkFence* pFences,
        VkBool32 waitAll,
        UInt64 timeout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateSemaphore(
        VkDevice device,
        VkSemaphoreCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkSemaphore* pSemaphore);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroySemaphore(
        VkDevice device,
        VkSemaphore semaphore,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateEvent(
        VkDevice device,
        VkEventCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkEvent* pEvent);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyEvent(
        VkDevice device,
        VkEvent evnt,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetEventStatus(
        VkDevice device,
        VkEvent evnt);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkSetEvent(
        VkDevice device,
        VkEvent evnt);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkResetEvent(
        VkDevice device,
        VkEvent evnt);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateQueryPool(
        VkDevice device,
        VkQueryPoolCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkQueryPool* pQueryPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyQueryPool(
        VkDevice device,
        VkQueryPool queryPool,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetQueryPoolResults(
        VkDevice device,
        VkQueryPool queryPool,
        UInt32 firstQuery,
        UInt32 queryCount,
        UIntPtr dataSize,
        void* pData,
        VkDeviceSize stride,
        VkQueryResultFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateBuffer(
        VkDevice device,
        VkBufferCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkBuffer* pBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyBuffer(
        VkDevice device,
        VkBuffer buffer,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateBufferView(
        VkDevice device,
        VkBufferViewCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkBufferView* pView);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyBufferView(
        VkDevice device,
        VkBufferView bufferView,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateImage(
        VkDevice device,
        VkImageCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkImage* pImage);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyImage(
        VkDevice device,
        VkImage image,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetImageSubresourceLayout(
        VkDevice device,
        VkImage image,
        VkImageSubresource* pSubresource,
        VkSubresourceLayout* pLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateImageView(
        VkDevice device,
        VkImageViewCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkImageView* pView);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyImageView(
        VkDevice device,
        VkImageView imageView,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateShaderModule(
        VkDevice device,
        VkShaderModuleCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkShaderModule* pShaderModule);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyShaderModule(
        VkDevice device,
        VkShaderModule shaderModule,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreatePipelineCache(
        VkDevice device,
        VkPipelineCacheCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkPipelineCache* pPipelineCache);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyPipelineCache(
        VkDevice device,
        VkPipelineCache pipelineCache,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPipelineCacheData(
        VkDevice device,
        VkPipelineCache pipelineCache,
        UIntPtr* pDataSize,
        void* pData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkMergePipelineCaches(
        VkDevice device,
        VkPipelineCache dstCache,
        UInt32 srcCacheCount,
        VkPipelineCache* pSrcCaches);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateGraphicsPipelines(
        VkDevice device,
        VkPipelineCache pipelineCache,
        UInt32 createInfoCount,
        VkGraphicsPipelineCreateInfo* pCreateInfos,
        VkAllocationCallbacks* pAllocator,
        VkPipeline* pPipelines);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateComputePipelines(
        VkDevice device,
        VkPipelineCache pipelineCache,
        UInt32 createInfoCount,
        VkComputePipelineCreateInfo* pCreateInfos,
        VkAllocationCallbacks* pAllocator,
        VkPipeline* pPipelines);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyPipeline(
        VkDevice device,
        VkPipeline pipeline,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreatePipelineLayout(
        VkDevice device,
        VkPipelineLayoutCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkPipelineLayout* pPipelineLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyPipelineLayout(
        VkDevice device,
        VkPipelineLayout pipelineLayout,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateSampler(
        VkDevice device,
        VkSamplerCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkSampler* pSampler);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroySampler(
        VkDevice device,
        VkSampler sampler,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDescriptorSetLayout(
        VkDevice device,
        VkDescriptorSetLayoutCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkDescriptorSetLayout* pSetLayout);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyDescriptorSetLayout(
        VkDevice device,
        VkDescriptorSetLayout descriptorSetLayout,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDescriptorPool(
        VkDevice device,
        VkDescriptorPoolCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkDescriptorPool* pDescriptorPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyDescriptorPool(
        VkDevice device,
        VkDescriptorPool descriptorPool,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkResetDescriptorPool(
        VkDevice device,
        VkDescriptorPool descriptorPool,
        VkDescriptorPoolResetFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkAllocateDescriptorSets(
        VkDevice device,
        VkDescriptorSetAllocateInfo* pAllocateInfo,
        VkDescriptorSet* pDescriptorSets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkFreeDescriptorSets(
        VkDevice device,
        VkDescriptorPool descriptorPool,
        UInt32 descriptorSetCount,
        VkDescriptorSet* pDescriptorSets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkUpdateDescriptorSets(
        VkDevice device,
        UInt32 descriptorWriteCount,
        VkWriteDescriptorSet* pDescriptorWrites,
        UInt32 descriptorCopyCount,
        VkCopyDescriptorSet* pDescriptorCopies);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateFramebuffer(
        VkDevice device,
        VkFramebufferCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkFramebuffer* pFramebuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyFramebuffer(
        VkDevice device,
        VkFramebuffer framebuffer,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateRenderPass(
        VkDevice device,
        VkRenderPassCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkRenderPass* pRenderPass);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyRenderPass(
        VkDevice device,
        VkRenderPass renderPass,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkGetRenderAreaGranularity(
        VkDevice device,
        VkRenderPass renderPass,
        VkExtent2D* pGranularity);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateCommandPool(
        VkDevice device,
        VkCommandPoolCreateInfo* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkCommandPool* pCommandPool);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyCommandPool(
        VkDevice device,
        VkCommandPool commandPool,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkResetCommandPool(
        VkDevice device,
        VkCommandPool commandPool,
        VkCommandPoolResetFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkAllocateCommandBuffers(
        VkDevice device,
        VkCommandBufferAllocateInfo* pAllocateInfo,
        VkCommandBuffer* pCommandBuffers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkFreeCommandBuffers(
        VkDevice device,
        VkCommandPool commandPool,
        UInt32 commandBufferCount,
        VkCommandBuffer* pCommandBuffers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkBeginCommandBuffer(
        VkCommandBuffer commandBuffer,
        VkCommandBufferBeginInfo* pBeginInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkEndCommandBuffer(
        VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkResetCommandBuffer(
        VkCommandBuffer commandBuffer,
        VkCommandBufferResetFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBindPipeline(
        VkCommandBuffer commandBuffer,
        VkPipelineBindPoint pipelineBindPoint,
        VkPipeline pipeline);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetViewport(
        VkCommandBuffer commandBuffer,
        UInt32 firstViewport,
        UInt32 viewportCount,
        VkViewport* pViewports);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetScissor(
        VkCommandBuffer commandBuffer,
        UInt32 firstScissor,
        UInt32 scissorCount,
        VkRect2D* pScissors);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetLineWidth(
        VkCommandBuffer commandBuffer,
        float lineWidth);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetDepthBias(
        VkCommandBuffer commandBuffer,
        float depthBiasConstantFactor,
        float depthBiasClamp,
        float depthBiasSlopeFactor);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetBlendConstants(
        VkCommandBuffer commandBuffer,
        float* blendConstants);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetDepthBounds(
        VkCommandBuffer commandBuffer,
        float minDepthBounds,
        float maxDepthBounds);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetStencilCompareMask(
        VkCommandBuffer commandBuffer,
        VkStencilFaceFlags faceMask,
        UInt32 compareMask);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetStencilWriteMask(
        VkCommandBuffer commandBuffer,
        VkStencilFaceFlags faceMask,
        UInt32 writeMask);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetStencilReference(
        VkCommandBuffer commandBuffer,
        VkStencilFaceFlags faceMask,
        UInt32 reference);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBindDescriptorSets(
        VkCommandBuffer commandBuffer,
        VkPipelineBindPoint pipelineBindPoint,
        VkPipelineLayout layout,
        UInt32 firstSet,
        UInt32 descriptorSetCount,
        VkDescriptorSet* pDescriptorSets,
        UInt32 dynamicOffsetCount,
        UInt32* pDynamicOffsets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBindIndexBuffer(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset,
        VkIndexType indexType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBindVertexBuffers(
        VkCommandBuffer commandBuffer,
        UInt32 firstBinding,
        UInt32 bindingCount,
        VkBuffer* pBuffers,
        VkDeviceSize* pOffsets);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDraw(
        VkCommandBuffer commandBuffer,
        UInt32 vertexCount,
        UInt32 instanceCount,
        UInt32 firstVertex,
        UInt32 firstInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDrawIndexed(
        VkCommandBuffer commandBuffer,
        UInt32 indexCount,
        UInt32 instanceCount,
        UInt32 firstIndex,
        Int32 vertexOffset,
        UInt32 firstInstance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDrawIndirect(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset,
        UInt32 drawCount,
        UInt32 stride);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDrawIndexedIndirect(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset,
        UInt32 drawCount,
        UInt32 stride);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDispatch(
        VkCommandBuffer commandBuffer,
        UInt32 x,
        UInt32 y,
        UInt32 z);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDispatchIndirect(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdCopyBuffer(
        VkCommandBuffer commandBuffer,
        VkBuffer srcBuffer,
        VkBuffer dstBuffer,
        UInt32 regionCount,
        VkBufferCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdCopyImage(
        VkCommandBuffer commandBuffer,
        VkImage srcImage,
        VkImageLayout srcImageLayout,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        UInt32 regionCount,
        VkImageCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBlitImage(
        VkCommandBuffer commandBuffer,
        VkImage srcImage,
        VkImageLayout srcImageLayout,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        UInt32 regionCount,
        VkImageBlit* pRegions,
        VkFilter filter);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdCopyBufferToImage(
        VkCommandBuffer commandBuffer,
        VkBuffer srcBuffer,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        UInt32 regionCount,
        VkBufferImageCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdCopyImageToBuffer(
        VkCommandBuffer commandBuffer,
        VkImage srcImage,
        VkImageLayout srcImageLayout,
        VkBuffer dstBuffer,
        UInt32 regionCount,
        VkBufferImageCopy* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdUpdateBuffer(
        VkCommandBuffer commandBuffer,
        VkBuffer dstBuffer,
        VkDeviceSize dstOffset,
        VkDeviceSize dataSize,
        void* pData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdFillBuffer(
        VkCommandBuffer commandBuffer,
        VkBuffer dstBuffer,
        VkDeviceSize dstOffset,
        VkDeviceSize size,
        UInt32 data);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdClearColorImage(
        VkCommandBuffer commandBuffer,
        VkImage image,
        VkImageLayout imageLayout,
        VkClearColorValue* pColor,
        UInt32 rangeCount,
        VkImageSubresourceRange* pRanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdClearDepthStencilImage(
        VkCommandBuffer commandBuffer,
        VkImage image,
        VkImageLayout imageLayout,
        VkClearDepthStencilValue* pDepthStencil,
        UInt32 rangeCount,
        VkImageSubresourceRange* pRanges);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdClearAttachments(
        VkCommandBuffer commandBuffer,
        UInt32 attachmentCount,
        VkClearAttachment* pAttachments,
        UInt32 rectCount,
        VkClearRect* pRects);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdResolveImage(
        VkCommandBuffer commandBuffer,
        VkImage srcImage,
        VkImageLayout srcImageLayout,
        VkImage dstImage,
        VkImageLayout dstImageLayout,
        UInt32 regionCount,
        VkImageResolve* pRegions);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdSetEvent(
        VkCommandBuffer commandBuffer,
        VkEvent envt,
        VkPipelineStageFlags stageMask);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdResetEvent(
        VkCommandBuffer commandBuffer,
        VkEvent envt,
        VkPipelineStageFlags stageMask);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdWaitEvents(
        VkCommandBuffer commandBuffer,
        UInt32 eventCount,
        VkEvent* pEvents,
        VkPipelineStageFlags srcStageMask,
        VkPipelineStageFlags dstStageMask,
        UInt32 memoryBarrierCount,
        VkMemoryBarrier* pMemoryBarriers,
        UInt32 bufferMemoryBarrierCount,
        VkBufferMemoryBarrier* pBufferMemoryBarriers,
        UInt32 imageMemoryBarrierCount,
        VkImageMemoryBarrier* pImageMemoryBarriers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdPipelineBarrier(
        VkCommandBuffer commandBuffer,
        VkPipelineStageFlags srcStageMask,
        VkPipelineStageFlags dstStageMask,
        VkDependencyFlags dependencyFlags,
        UInt32 memoryBarrierCount,
        VkMemoryBarrier* pMemoryBarriers,
        UInt32 bufferMemoryBarrierCount,
        VkBufferMemoryBarrier* pBufferMemoryBarriers,
        UInt32 imageMemoryBarrierCount,
        VkImageMemoryBarrier* pImageMemoryBarriers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBeginQuery(
        VkCommandBuffer commandBuffer,
        VkQueryPool queryPool,
        UInt32 query,
        VkQueryControlFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdEndQuery(
        VkCommandBuffer commandBuffer,
        VkQueryPool queryPool,
        UInt32 query);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdResetQueryPool(
        VkCommandBuffer commandBuffer,
        VkQueryPool queryPool,
        UInt32 firstQuery,
        UInt32 queryCount);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdWriteTimestamp(
        VkCommandBuffer commandBuffer,
        VkPipelineStageFlags pipelineStage,
        VkQueryPool queryPool,
        UInt32 query);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdCopyQueryPoolResults(
        VkCommandBuffer commandBuffer,
        VkQueryPool queryPool,
        UInt32 firstQuery,
        UInt32 queryCount,
        VkBuffer dstBuffer,
        VkDeviceSize dstOffset,
        VkDeviceSize stride,
        VkQueryResultFlags flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdPushConstants(
        VkCommandBuffer commandBuffer,
        VkPipelineLayout layout,
        VkShaderStageFlags stageFlags,
        UInt32 offset,
        UInt32 size,
        void* pValues);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdBeginRenderPass(
        VkCommandBuffer commandBuffer,
        VkRenderPassBeginInfo* pRenderPassBegin,
        VkSubpassContents contents);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdNextSubpass(
        VkCommandBuffer commandBuffer,
        VkSubpassContents contents);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdEndRenderPass(
        VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdExecuteCommands(
        VkCommandBuffer commandBuffer,
        UInt32 commandBufferCount,
        VkCommandBuffer* pCommandBuffers);

    #endregion

    #region VK_KHR_Surface

    internal static partial class VkConst
    {
        public const uint KHR_surface = 1;
        public const uint KHR_SURFACE_SPEC_VERSION = 25;
        public const string KHR_SURFACE_EXTENSION_NAME = "VK_KHR_surface";
        public const uint COLORSPACE_SRGB_NONLINEAR_KHR = (uint)VkColorSpaceKHR.SrgbNonlinearKhr;
    }

    internal unsafe struct VkSurfaceKHR
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkSurfaceKHR value) => value.Ptr;
        public static implicit operator VkSurfaceKHR(IntPtr value) => new VkSurfaceKHR { Ptr = value };
    }

    internal enum VkColorSpaceKHR
    {
        SrgbNonlinearKhr = 0
    }

    internal enum VkPresentModeKHR
    {
        ImmediateKhr = 0,
        MailboxKhr = 1,
        FifoKhr = 2,
        FifoRelaxedKhr = 3
    }

    [Flags]
    internal enum VkSurfaceTransformFlagsKHR
    {
        IdentityBitKhr = 0x00000001,
        Rotate90BitKhr = 0x00000002,
        Rotate180BitKhr = 0x00000004,
        Rotate270BitKhr = 0x00000008,
        HorizontalMirrorBitKhr = 0x00000010,
        HorizontalMirrorRotate90BitKhr = 0x00000020,
        HorizontalMirrorRotate180BitKhr = 0x00000040,
        HorizontalMirrorRotate270BitKhr = 0x00000080,
        InheritBitKhr = 0x00000100,
        FlagBitsMaxEnumKhr = 0x7fffffff
    }

    [Flags]
    internal enum VkCompositeAlphaFlagsKHR
    {
        OpaqueBitKhr = 0x00000001,
        PreMultipliedBitKhr = 0x00000002,
        PostMultipliedBitKhr = 0x00000004,
        InheritBitKhr = 0x00000008,
        FlagBitsMaxEnumKhr = 0x7fffffff
    }

    internal unsafe struct VkSurfaceCapabilitiesKHR
    {
        public UInt32 minImageCount;
        public UInt32 maxImageCount;
        public VkExtent2D currentExtent;
        public VkExtent2D minImageExtent;
        public VkExtent2D maxImageExtent;
        public UInt32 maxImageArrayLayers;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkSurfaceTransformFlagsKHR currentTransform;
        public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
        public VkImageUsageFlags supportedUsageFlags;
    }

    internal unsafe struct VkSurfaceFormatKHR
    {
        public VkFormat format;
        public VkColorSpaceKHR colorSpace;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroySurfaceKHR(
        VkInstance instance,
        VkSurfaceKHR surface,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceSurfaceSupportKHR(
        VkPhysicalDevice physicalDevice,
        UInt32 queueFamilyIndex,
        VkSurfaceKHR surface,
        VkBool32* pSupported);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceSurfaceCapabilitiesKHR(
        VkPhysicalDevice physicalDevice,
        VkSurfaceKHR surface,
        VkSurfaceCapabilitiesKHR* pSurfaceCapabilities);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(
        VkPhysicalDevice physicalDevice,
        VkSurfaceKHR surface,
        UInt32* pSurfaceFormatCount,
        VkSurfaceFormatKHR* pSurfaceFormats);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(
        VkPhysicalDevice physicalDevice,
        VkSurfaceKHR surface,
        UInt32* pPresentModeCount,
        VkPresentModeKHR* pPresentModes);

    #endregion

    #region VK_KHR_swapchain

    internal static partial class VkConst
    {
        public const uint VK_KHR_swapchain = 1;
        public const uint VK_KHR_SWAPCHAIN_SPEC_VERSION = 68;
        public const string VK_KHR_SWAPCHAIN_EXTENSION_NAME = "VK_KHR_swapchain";
    }

    internal unsafe struct VkSwapchainKHR
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkSwapchainKHR value) => value.Ptr;
        public static implicit operator VkSwapchainKHR(IntPtr value) => new VkSwapchainKHR { Ptr = value };
    }

    internal unsafe struct VkSwapchainCreateFlagsKHR
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkSwapchainCreateFlagsKHR value) => value.Flag;
        public static implicit operator VkSwapchainCreateFlagsKHR(VkFlags flag) => new VkSwapchainCreateFlagsKHR { Flag = flag };
    }

    internal unsafe struct VkSwapchainCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkSwapchainCreateFlagsKHR flags;
        public VkSurfaceKHR surface;
        public UInt32 minImageCount;
        public VkFormat imageFormat;
        public VkColorSpaceKHR imageColorSpace;
        public VkExtent2D imageExtent;
        public UInt32 imageArrayLayers;
        public VkImageUsageFlags imageUsage;
        public VkSharingMode imageSharingMode;
        public UInt32 queueFamilyIndexCount;
        public UInt32* pQueueFamilyIndices;
        public VkSurfaceTransformFlagsKHR preTransform;
        public VkCompositeAlphaFlagsKHR compositeAlpha;
        public VkPresentModeKHR presentMode;
        public VkBool32 clipped;
        public VkSwapchainKHR oldSwapchain;
    }

    internal unsafe struct VkPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public UInt32 waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public UInt32 swapchainCount;
        public VkSwapchainKHR* pSwapchains;
        public UInt32* pImageIndices;
        public VkResult* pResults;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateSwapchainKHR(
        VkDevice device,
        VkSwapchainCreateInfoKHR* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkSwapchainKHR* pSwapchain);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroySwapchainKHR(
        VkDevice device,
        VkSwapchainKHR swapchain,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetSwapchainImagesKHR(
        VkDevice device,
        VkSwapchainKHR swapchain,
        UInt32* pSwapchainImageCount,
        VkImage* pSwapchainImages);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkAcquireNextImageKHR(
        VkDevice device,
        VkSwapchainKHR swapchain,
        UInt64 timeout,
        VkSemaphore semaphore,
        VkFence fence,
        UInt32* pImageIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkQueuePresentKHR(
        VkQueue queue,
        VkPresentInfoKHR* pPresentInfo);

    #endregion

    #region VK_KHR_display

    internal static partial class VkConst
    {
        public const uint KHR_display = 1;
        public const uint KHR_DISPLAY_SPEC_VERSION = 21;
        public const string KHR_DISPLAY_EXTENSION_NAME = "VK_KHR_display";
    }

    internal unsafe struct VkDisplayKHR
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkDisplayKHR value) => value.Ptr;
        public static implicit operator VkDisplayKHR(IntPtr value) => new VkDisplayKHR { Ptr = value };
    }

    internal unsafe struct VkDisplayModeKHR
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkDisplayModeKHR value) => value.Ptr;
        public static implicit operator VkDisplayModeKHR(IntPtr value) => new VkDisplayModeKHR { Ptr = value };
    }

    [Flags]
    internal enum VkDisplayPlaneAlphaFlagsKHR
    {
        OpaqueBitKhr = 0x00000001,
        GlobalBitKhr = 0x00000002,
        PerPixelBitKhr = 0x00000004,
        PerPixelPremultipliedBitKhr = 0x00000008,
        FlagBitsMaxEnumKhr = 0x7fffffff
    }

    internal unsafe struct VkDisplayModeCreateFlagsKHR
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDisplayModeCreateFlagsKHR value) => value.Flag;
        public static implicit operator VkDisplayModeCreateFlagsKHR(VkFlags flag) => new VkDisplayModeCreateFlagsKHR { Flag = flag };
    }

    internal unsafe struct VkDisplaySurfaceCreateFlagsKHR
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDisplaySurfaceCreateFlagsKHR value) => value.Flag;
        public static implicit operator VkDisplaySurfaceCreateFlagsKHR(VkFlags flag) => new VkDisplaySurfaceCreateFlagsKHR { Flag = flag };
    }

    internal unsafe struct VkDisplayPropertiesKHR
    {
        public VkDisplayKHR display;
        public byte* displayName;
        public VkExtent2D physicalDimensions;
        public VkExtent2D physicalResolution;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkBool32 planeReorderPossible;
        public VkBool32 persistentContent;
    }

    internal unsafe struct VkDisplayModeParametersKHR
    {
        public VkExtent2D visibleRegion;
        public UInt32 refreshRate;
    }

    internal unsafe struct VkDisplayModePropertiesKHR
    {
        public VkDisplayModeKHR displayMode;
        public VkDisplayModeParametersKHR parameters;
    }

    internal unsafe struct VkDisplayModeCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDisplayModeCreateFlagsKHR flags;
        public VkDisplayModeParametersKHR parameters;
    }

    internal unsafe struct VkDisplayPlaneCapabilitiesKHR
    {
        public VkDisplayPlaneAlphaFlagsKHR supportedAlpha;
        public VkOffset2D minSrcPosition;
        public VkOffset2D maxSrcPosition;
        public VkExtent2D minSrcExtent;
        public VkExtent2D maxSrcExtent;
        public VkOffset2D minDstPosition;
        public VkOffset2D maxDstPosition;
        public VkExtent2D minDstExtent;
        public VkExtent2D maxDstExtent;
    }

    internal unsafe struct VkDisplayPlanePropertiesKHR
    {
        public VkDisplayKHR currentDisplay;
        public UInt32 currentStackIndex;
    }

    internal unsafe struct VkDisplaySurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDisplaySurfaceCreateFlagsKHR flags;
        public VkDisplayModeKHR displayMode;
        public UInt32 planeIndex;
        public UInt32 planeStackIndex;
        public VkSurfaceTransformFlagsKHR transform;
        public float globalAlpha;
        public VkDisplayPlaneAlphaFlagsKHR alphaMode;
        public VkExtent2D imageExtent;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceDisplayPropertiesKHR(
        VkPhysicalDevice physicalDevice,
        UInt32* pPropertyCount,
        VkDisplayPropertiesKHR* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceDisplayPlanePropertiesKHR(
        VkPhysicalDevice physicalDevice,
        UInt32* pPropertyCount,
        VkDisplayPlanePropertiesKHR* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetDisplayPlaneSupportedDisplaysKHR(
        VkPhysicalDevice physicalDevice,
        UInt32 planeIndex,
        UInt32* pDisplayCount,
        VkDisplayKHR* pDisplays);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetDisplayModePropertiesKHR(
        VkPhysicalDevice physicalDevice,
        VkDisplayKHR display,
        UInt32* pPropertyCount,
        VkDisplayModePropertiesKHR* pProperties);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDisplayModeKHR(
        VkPhysicalDevice physicalDevice,
        VkDisplayKHR display,
        VkDisplayModeCreateInfoKHR* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkDisplayModeKHR* pMode);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetDisplayPlaneCapabilitiesKHR(
        VkPhysicalDevice physicalDevice,
        VkDisplayModeKHR mode,
        UInt32 planeIndex,
        VkDisplayPlaneCapabilitiesKHR* pCapabilities);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDisplayPlaneSurfaceKHR(
        VkInstance instance,
        VkDisplaySurfaceCreateInfoKHR* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkSurfaceKHR* pSurface);

    #endregion

    #region VK_KHR_display_swapchain

    internal static partial class VkConst
    {
        public const uint KHR_display_swapchain = 1;
        public const uint KHR_DISPLAY_SWAPCHAIN_SPEC_VERSION = 9;
        public const string KHR_DISPLAY_SWAPCHAIN_EXTENSION_NAME = "VK_KHR_display_swapchain";
    }

    internal unsafe struct VkDisplayPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRect2D srcRect;
        public VkRect2D dstRect;
        public VkBool32 persistent;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateSharedSwapchainsKHR(
        VkDevice device,
        UInt32 swapchainCount,
        VkSwapchainCreateInfoKHR* pCreateInfos,
        VkAllocationCallbacks* pAllocator,
        VkSwapchainKHR* pSwapchains);

    #endregion

    #region VK_KHR_sampler_mirror_clamp_to_edge

    internal static partial class VkConst
    {
        public const uint KHR_sampler_mirror_clamp_to_edge = 1;
        public const uint KHR_SAMPLER_MIRROR_CLAMP_TO_EDGE_SPEC_VERSION = 1;
        public const string KHR_SAMPLER_MIRROR_CLAMP_TO_EDGE_EXTENSION_NAME = "VK_KHR_sampler_mirror_clamp_to_edge";
        public const uint EXT_debug_report = 1;
    }

    internal unsafe struct VkDebugReportCallbackEXT
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkDebugReportCallbackEXT value) => value.Ptr;
        public static implicit operator VkDebugReportCallbackEXT(IntPtr value) => new VkDebugReportCallbackEXT { Ptr = value };
    }

    #endregion

    #region VK_EXT_debug_report

    internal static partial class VkConst
    {
        public const uint EXT_DEBUG_REPORT_SPEC_VERSION = 3;
        public const string EXT_DEBUG_REPORT_EXTENSION_NAME = "VK_EXT_debug_report";
        public const uint STRUCTURE_TYPE_DEBUG_REPORT_CREATE_INFO_EXT = (uint)VkStructureType.DebugReportCallbackCreateInfoExt;
    }

    internal enum VkDebugReportObjectTypeEXT
    {
        UnknownExt = 0,
        InstanceExt = 1,
        PhysicalDeviceExt = 2,
        DeviceExt = 3,
        QueueExt = 4,
        SemaphoreExt = 5,
        CommandBufferExt = 6,
        FenceExt = 7,
        DeviceMemoryExt = 8,
        BufferExt = 9,
        ImageExt = 10,
        EventExt = 11,
        QueryPoolExt = 12,
        BufferViewExt = 13,
        ImageViewExt = 14,
        ShaderModuleExt = 15,
        PipelineCacheExt = 16,
        PipelineLayoutExt = 17,
        RenderPassExt = 18,
        PipelineExt = 19,
        DescriptorSetLayoutExt = 20,
        SamplerExt = 21,
        DescriptorPoolExt = 22,
        DescriptorSetExt = 23,
        FramebufferExt = 24,
        CommandPoolExt = 25,
        SurfaceKhrExt = 26,
        SwapchainKhrExt = 27,
        DebugReportExt = 28
    }

    internal enum VkDebugReportErrorEXT
    {
        NoneExt = 0,
        CallbackRefExt = 1
    }

    [Flags]
    internal enum VkDebugReportFlagsEXT
    {
        InformationBitExt = 0x00000001,
        WarningBitExt = 0x00000002,
        PerformanceWarningBitExt = 0x00000004,
        ErrorBitExt = 0x00000008,
        DebugBitExt = 0x00000010
    }

    internal unsafe struct VkDebugReportCallbackCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportFlagsEXT flags;
        public IntPtr pfnCallback;
        public void* pUserData;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkCreateDebugReportCallbackEXT(
        VkInstance instance,
        VkDebugReportCallbackCreateInfoEXT* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        VkDebugReportCallbackEXT* pCallback);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDestroyDebugReportCallbackEXT(
        VkInstance instance,
        VkDebugReportCallbackEXT callback,
        VkAllocationCallbacks* pAllocator);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkDebugReportMessageEXT(
        VkInstance instance,
        VkDebugReportFlagsEXT flags,
        VkDebugReportObjectTypeEXT objectType,
        UInt64 obj,
        UIntPtr location,
        Int32 messageCode,
        byte* pLayerPrefix,
        byte* pMessage);

    #endregion

    #region VK_NV_glsl_shader

    internal static partial class VkConst
    {
        public const uint NV_glsl_shader = 1;
        public const uint NV_GLSL_SHADER_SPEC_VERSION = 1;
        public const string NV_GLSL_SHADER_EXTENSION_NAME = "VK_NV_glsl_shader";
    }

    #endregion

    #region VK_IMG_filter_cubic

    internal static partial class VkConst
    {
        public const uint IMG_filter_cubic = 1;
        public const uint IMG_FILTER_CUBIC_SPEC_VERSION = 1;
        public const string IMG_FILTER_CUBIC_EXTENSION_NAME = "VK_IMG_filter_cubic";
    }

    #endregion

    #region VK_AMD_rasterization_order

    internal static partial class VkConst
    {
        public const uint AMD_rasterization_order = 1;
        public const uint AMD_RASTERIZATION_ORDER_SPEC_VERSION = 1;
        public const string AMD_RASTERIZATION_ORDER_EXTENSION_NAME = "VK_AMD_rasterization_order";
    }

    internal enum VkRasterizationOrderAMD
    {
        StrictAmd = 0,
        RelaxedAmd = 1
    }

    internal unsafe struct VkPipelineRasterizationStateRasterizationOrderAMD
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRasterizationOrderAMD rasterizationOrder;
    }

    internal static partial class VkConst
    {
        public const uint AMD_shader_trinary_minmax = 1;
        public const uint AMD_SHADER_TRINARY_MINMAX_SPEC_VERSION = 1;
        public const string AMD_SHADER_TRINARY_MINMAX_EXTENSION_NAME = "VK_AMD_shader_trinary_minmax";
        public const uint AMD_shader_explicit_vertex_parameter = 1;
        public const uint AMD_SHADER_EXPLICIT_VERTEX_PARAMETER_SPEC_VERSION = 1;
        public const string AMD_SHADER_EXPLICIT_VERTEX_PARAMETER_EXTENSION_NAME = "VK_AMD_shader_explicit_vertex_parameter";
        public const uint EXT_debug_marker = 1;
        public const uint EXT_DEBUG_MARKER_SPEC_VERSION = 3;
        public const string EXT_DEBUG_MARKER_EXTENSION_NAME = "VK_EXT_debug_marker";
    }

    internal unsafe struct VkDebugMarkerObjectNameInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public UInt64 obj;
        public byte* pObjectName;
    }

    internal unsafe struct VkDebugMarkerObjectTagInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public UInt64 obj;
        public UInt64 tagName;
        public UIntPtr tagSize;
        public void* pTag;
    }

    internal unsafe struct VkDebugMarkerMarkerInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pMarkerName;
        public fixed float color[4];
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkDebugMarkerSetObjectTagEXT(
        VkDevice device,
        VkDebugMarkerObjectTagInfoEXT* pTagInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkDebugMarkerSetObjectNameEXT(
        VkDevice device,
        VkDebugMarkerObjectNameInfoEXT* pNameInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDebugMarkerBeginEXT(
        VkCommandBuffer commandBuffer,
        VkDebugMarkerMarkerInfoEXT* pMarkerInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDebugMarkerEndEXT(
        VkCommandBuffer commandBuffer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDebugMarkerInsertEXT(
        VkCommandBuffer commandBuffer,
        VkDebugMarkerMarkerInfoEXT* pMarkerInfo);

    #endregion

    #region VK_AMD_gcn_shader

    internal static partial class VkConst
    {
        public const uint AMD_gcn_shader = 1;
        public const uint AMD_GCN_SHADER_SPEC_VERSION = 1;
        public const string AMD_GCN_SHADER_EXTENSION_NAME = "VK_AMD_gcn_shader";
    }

    #endregion

    #region VK_NV_dedicated_allocation

    internal static partial class VkConst
    {
        public const uint NV_dedicated_allocation = 1;
        public const uint NV_DEDICATED_ALLOCATION_SPEC_VERSION = 1;
        public const string NV_DEDICATED_ALLOCATION_EXTENSION_NAME = "VK_NV_dedicated_allocation";
    }

    internal unsafe struct VkDedicatedAllocationImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
    }

    internal unsafe struct VkDedicatedAllocationBufferCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
    }

    internal unsafe struct VkDedicatedAllocationMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public VkBuffer buffer;
    }

    #endregion

    #region VK_AMD_draw_indirect_count

    internal static partial class VkConst
    {
        public const uint AMD_draw_indirect_count = 1;
        public const uint AMD_DRAW_INDIRECT_COUNT_SPEC_VERSION = 1;
        public const string AMD_DRAW_INDIRECT_COUNT_EXTENSION_NAME = "VK_AMD_draw_indirect_count";
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDrawIndirectCountAMD(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset,
        VkBuffer countBuffer,
        VkDeviceSize countBufferOffset,
        UInt32 maxDrawCount,
        UInt32 stride);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate void vkCmdDrawIndexedIndirectCountAMD(
        VkCommandBuffer commandBuffer,
        VkBuffer buffer,
        VkDeviceSize offset,
        VkBuffer countBuffer,
        VkDeviceSize countBufferOffset,
        UInt32 maxDrawCount,
        UInt32 stride);

    #endregion

    #region VK_AMD_negative_viewport_height

    internal static partial class VkConst
    {
        public const uint AMD_negative_viewport_height = 1;
        public const uint AMD_NEGATIVE_VIEWPORT_HEIGHT_SPEC_VERSION = 0;
        public const string AMD_NEGATIVE_VIEWPORT_HEIGHT_EXTENSION_NAME = "VK_AMD_negative_viewport_height";
    }

    #endregion

    #region VK_AMD_gpu_shader_half_float

    internal static partial class VkConst
    {
        public const uint AMD_gpu_shader_half_float = 1;
        public const uint AMD_GPU_SHADER_HALF_FLOAT_SPEC_VERSION = 1;
        public const string AMD_GPU_SHADER_HALF_FLOAT_EXTENSION_NAME = "VK_AMD_gpu_shader_half_float";
    }

    #endregion

    #region VK_AMD_shader_ballot

    internal static partial class VkConst
    {
        public const uint AMD_shader_ballot = 1;
        public const uint AMD_SHADER_BALLOT_SPEC_VERSION = 0;
        public const string AMD_SHADER_BALLOT_EXTENSION_NAME = "VK_AMD_shader_ballot";
    }

    #endregion

    #region VK_IMG_format_pvrtc

    internal static partial class VkConst
    {
        public const uint IMG_format_pvrtc = 1;
        public const uint IMG_FORMAT_PVRTC_SPEC_VERSION = 1;
        public const string IMG_FORMAT_PVRTC_EXTENSION_NAME = "VK_IMG_format_pvrtc";
    }

    #endregion

    #region VK_NV_external_memory_capabilities

    internal static partial class VkConst
    {
        public const uint NV_external_memory_capabilities = 1;
        public const uint NV_EXTERNAL_MEMORY_CAPABILITIES_SPEC_VERSION = 1;
        public const string NV_EXTERNAL_MEMORY_CAPABILITIES_EXTENSION_NAME = "VK_NV_external_memory_capabilities";
    }

    [Flags]
    internal enum VkExternalMemoryHandleTypeFlagsNV
    {
        OpaqueWin32BitNv = 0x00000001,
        OpaqueWin32KmtBitNv = 0x00000002,
        D3d11ImageBitNv = 0x00000004,
        D3d11ImageKmtBitNv = 0x00000008,
        FlagBitsMaxEnumNv = 0x7fffffff
    }

    [Flags]
    internal enum VkExternalMemoryFeatureFlagsNV
    {
        DedicatedOnlyBitNv = 0x00000001,
        ExportableBitNv = 0x00000002,
        ImportableBitNv = 0x00000004,
        FlagBitsMaxEnumNv = 0x7fffffff
    }

    internal unsafe struct VkExternalImageFormatPropertiesNV
    {
        public VkImageFormatProperties imageFormatProperties;
        public VkExternalMemoryFeatureFlagsNV externalMemoryFeatures;
        public VkExternalMemoryHandleTypeFlagsNV exportFromImportedHandleTypes;
        public VkExternalMemoryHandleTypeFlagsNV compatibleHandleTypes;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal unsafe delegate VkResult vkGetPhysicalDeviceExternalImageFormatPropertiesNV(
        VkPhysicalDevice physicalDevice,
        VkFormat format,
        VkImageType type,
        VkImageTiling tiling,
        VkImageUsageFlags usage,
        VkImageCreateFlags flags,
        VkExternalMemoryHandleTypeFlagsNV externalHandleType,
        VkExternalImageFormatPropertiesNV* pExternalImageFormatProperties);

    #endregion

    #region VK_NV_external_memory

    internal static partial class VkConst
    {
        public const uint NV_external_memory = 1;
        public const uint NV_EXTERNAL_MEMORY_SPEC_VERSION = 1;
        public const string NV_EXTERNAL_MEMORY_EXTENSION_NAME = "VK_NV_external_memory";
    }

    internal unsafe struct VkExternalMemoryImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
    }

    internal unsafe struct VkExportMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
    }

    #endregion

    #region VK_EXT_validation_flags

    internal static partial class VkConst
    {
        public const uint EXT_validation_flags = 1;
        public const uint EXT_VALIDATION_FLAGS_SPEC_VERSION = 1;
        public const string EXT_VALIDATION_FLAGS_EXTENSION_NAME = "VK_EXT_validation_flags";
    }

    internal enum VkValidationCheckEXT
    {
        AllExt = 0
    }

    internal unsafe struct VkValidationFlagsEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public UInt32 disabledValidationCheckCount;
        public VkValidationCheckEXT* pDisabledValidationChecks;
    }

    #endregion

    #region VK_EXT_debug_utils

    internal partial class VkConst
    {
        public const uint EXT_debug_utils = 1;
        public const uint EXT_DEBUG_UTILS_SPEC_VERSION = 1;
        public const string EXT_DEBUG_UTILS_EXTENSION_NAME = "VK_EXT_debug_utils";
    }

    internal struct VkDebugUtilsMessengerEXT
    {
        public IntPtr Ptr;
        public static implicit operator IntPtr(VkDebugUtilsMessengerEXT value) => value.Ptr;
        public static implicit operator VkDebugUtilsMessengerEXT(IntPtr value) => new VkDebugUtilsMessengerEXT { Ptr = value };
    }

    internal unsafe struct VkDebugUtilsMessengerCallbackDataFlagsEXT
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDebugUtilsMessengerCallbackDataFlagsEXT value) => value.Flag;
        public static implicit operator VkDebugUtilsMessengerCallbackDataFlagsEXT(VkFlags flag) => new VkDebugUtilsMessengerCallbackDataFlagsEXT { Flag = flag };
    }

    internal unsafe struct VkDebugUtilsMessengerCreateFlagsEXT
    {
        public VkFlags Flag;
        public static implicit operator VkFlags(VkDebugUtilsMessengerCreateFlagsEXT value) => value.Flag;
        public static implicit operator VkDebugUtilsMessengerCreateFlagsEXT(VkFlags flag) => new VkDebugUtilsMessengerCreateFlagsEXT { Flag = flag };
    }

    [Flags]
    internal enum VkDebugUtilsMessageSeverityFlagsEXT
    {
        Verbose = 0x00000001,
        Info = 0x00000010,
        Warning = 0x00000100,
        Error = 0x00001000,
    }
    
    [Flags]
    internal enum VkDebugUtilsMessageTypeFlagsEXT
    {
        General = 0x00000001,
        Validation = 0x00000002,
        Performance = 0x00000004,
    }

    internal unsafe struct VkDebugUtilsObjectNameInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectType objectType;
        public UInt64 objectHandle;
        public byte* pObjectName;
    }

    internal unsafe struct VkDebugUtilsObjectTagInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectType objectType;
        public UInt64 objectHandle;
        public UInt64 tagName;
        public UIntPtr tagSize;
        public void* pTag;
    }

    internal unsafe struct VkDebugUtilsLabelEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pLabelName;
        public fixed float color[4];
    }

    internal unsafe struct VkDebugUtilsMessengerCallbackDataEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugUtilsMessengerCallbackDataFlagsEXT flags;
        public byte* pMessageIdName;
        public Int32 messageIdNumber;
        public byte* pMessage;
        public UInt32 queueLabelCount;
        public VkDebugUtilsLabelEXT* pQueueLabels;
        public UInt32 cmdBufLabelCount;
        public VkDebugUtilsLabelEXT* pCmdBufLabels;
        public UInt32 objectCount;
        public VkDebugUtilsObjectNameInfoEXT* pObjects;
    }

    internal unsafe delegate VkBool32 PFN_vkDebugUtilsMessengerCallbackEXT(
        VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* pUserData);

    internal unsafe struct VkDebugUtilsMessengerCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugUtilsMessengerCreateFlagsEXT flags;
        public VkDebugUtilsMessageSeverityFlagsEXT messageSeverity;
        public VkDebugUtilsMessageTypeFlagsEXT messageType;
        public IntPtr pfnUserCallback;
        public void* pUserData;
    }

    internal unsafe delegate VkResult vkSetDebugUtilsObjectNameEXT(VkDevice device, VkDebugUtilsObjectNameInfoEXT* pNameInfo);

    internal unsafe delegate VkResult vkSetDebugUtilsObjectTagEXT(
        VkDevice device,
        VkDebugUtilsObjectTagInfoEXT* pTagInfo);

    internal unsafe delegate void vkQueueBeginDebugUtilsLabelEXT(
        VkQueue queue,
        VkDebugUtilsLabelEXT* pLabelInfo);

    internal unsafe delegate void vkQueueEndDebugUtilsLabelEXT(
        VkQueue queue);

    internal unsafe delegate void vkQueueInsertDebugUtilsLabelEXT(
        VkQueue queue,
        VkDebugUtilsLabelEXT* pLabelInfo);

    internal unsafe delegate void vkCmdBeginDebugUtilsLabelEXT(
        VkCommandBuffer commandBuffer,
        VkDebugUtilsLabelEXT* pLabelInfo);

    internal unsafe delegate void vkCmdEndDebugUtilsLabelEXT(
        VkCommandBuffer commandBuffer);

    internal unsafe delegate void vkCmdInsertDebugUtilsLabelEXT(
        VkCommandBuffer commandBuffer,
        VkDebugUtilsLabelEXT* pLabelInfo);

    internal unsafe delegate VkResult vkCreateDebugUtilsMessengerEXT(
        VkInstance instance,
        VkDebugUtilsMessengerCreateInfoEXT* pCreateInfo,
        VkAllocationCallbacks* pAllocator,
        out VkDebugUtilsMessengerEXT pMessenger);

    internal unsafe delegate void vkDestroyDebugUtilsMessengerEXT(
        VkInstance instance,
        VkDebugUtilsMessengerEXT messenger,
        VkAllocationCallbacks* pAllocator);

    internal unsafe delegate void vkSubmitDebugUtilsMessageEXT(
        VkInstance instance,
        VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData);

    #endregion
}

#pragma warning restore CS0649