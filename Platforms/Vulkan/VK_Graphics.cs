using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    public unsafe class VK_Graphics : Graphics, IGraphicsVulkan
    {

        internal ISystemVulkan System => App.System as ISystemVulkan ?? throw new Exception("System does not implement ISystemVulkan");

        // Vulkan Objects
        internal VkInstance Instance;
        internal VkPhysicalDevice PhysicalDevice;
        internal VkDevice Device;
        internal VkQueue GraphicsQueue;
        internal VkDebugUtilsMessengerEXT DebugMessenger;

        // Vulkan Bindings
        internal VK VK;

        // Debug Validation Layers
#if DEBUG
        private readonly string[] requestedValidationLayers = new[] { "VK_LAYER_KHRONOS_validation" };
#else
        private readonly string[] requestedValidationLayers = new string[0];
#endif
        private readonly List<string> validationLayers = new List<string>();
        private bool HasValidationLayers => validationLayers.Count > 0;

        private readonly string[] deviceExtensions = new[] { VkConst.VK_KHR_SWAPCHAIN_EXTENSION_NAME };
        private readonly List<Delegate> trackedDelegates = new List<Delegate>();

        protected override void Created()
        {
            ApiName = "Vulkan";
        }

        protected override void Startup()
        {
            VK.InitStaticDelegates(System);

            if (requestedValidationLayers.Length > 0)
                FindValidationLayers(validationLayers);

            Instance = CreateVulkanInstance();

            VK = new VK(this);

            // Debug Callback
            if (HasValidationLayers)
            {
                DebugMessenger = CreateDebugMessenger((messageSeverity, messageTypes, pCallbackData, pUserData) =>
                {
                    var message = VK.STRING(pCallbackData->pMessage);

                    if (messageSeverity.HasFlag(VkDebugUtilsMessageSeverityFlagsEXT.Error))
                        Log.Error(Name, message);
                    else if (messageSeverity.HasFlag(VkDebugUtilsMessageSeverityFlagsEXT.Warning))
                        Log.Warning(Name, message);
                    else
                        Log.Message(Name, message);

                    return VkConst.FALSE;
                });
            }
            
            // Pick a Physical Device
            PhysicalDevice = PickPhysicalDevice();

            // get the API version
            {
                VkPhysicalDeviceProperties properties;
                VK.GetPhysicalDeviceProperties(PhysicalDevice, &properties);
                ApiVersion = VK.UNMAKE_VERSION(properties.apiVersion);

                int length = 0;
                while (length < VkConst.MAX_PHYSICAL_DEVICE_NAME_SIZE && properties.deviceName[length] != 0)
                    length++;
                DeviceName = Encoding.UTF8.GetString(properties.deviceName, length);
            }

            // Create the Device
            {
                TryGetQueueFamilyIndex(PhysicalDevice, VkQueueFlags.GraphicsBit, out uint graphicsFamilyIndex);

                // Graphics Family Queue
                var priority = 1.0f;
                var queueCreateInfo = new VkDeviceQueueCreateInfo
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = graphicsFamilyIndex,
                    queueCount = 1,
                    pQueuePriorities = &priority
                };

                // Device Features
                var deviceFeatures = new VkPhysicalDeviceFeatures();

                var createInfo = new VkDeviceCreateInfo
                {
                    sType = VkStructureType.DeviceCreateInfo,
                    pQueueCreateInfos = &queueCreateInfo,
                    queueCreateInfoCount = 1,
                    pEnabledFeatures = &deviceFeatures,
                };

                // Device Extensions
                using var deviceExtensionNames = new NativeStringArray(deviceExtensions);
                createInfo.enabledExtensionCount = deviceExtensionNames.Length;
                createInfo.ppEnabledExtensionNames = deviceExtensionNames;

                var result = VK.CreateDevice(PhysicalDevice, &createInfo, null, out Device);
                if (result != VkResult.Success)
                    throw new Exception($"Failed to create Vulkan Logical Device, {result}");

                // Get the Graphics Queue
                VK.GetDeviceQueue(Device, graphicsFamilyIndex, 0, out GraphicsQueue);
            }

            base.Startup();
        }

        private void FindValidationLayers(List<string> appendTo)
        {
            uint availableLayerCount;
            VK.EnumerateInstanceLayerProperties(&availableLayerCount, null);
            VkLayerProperties* availableLayers = stackalloc VkLayerProperties[(int)availableLayerCount];
            VK.EnumerateInstanceLayerProperties(&availableLayerCount, availableLayers);

            for (int i = 0; i < requestedValidationLayers.Length; i++)
            {
                var hasLayer = false;
                for (int j = 0; j < availableLayerCount; j++)
                    if (requestedValidationLayers[i] == VK.STRING(availableLayers[j].layerName))
                    {
                        hasLayer = true;
                        break;
                    }

                if (hasLayer)
                    appendTo.Add(requestedValidationLayers[i]);
                else
                    Log.Warning(Name, $"Validation Layer {requestedValidationLayers[i]} requested but does not exist. You may need to install the Vulkan SDK.");
            }
        }

        private VkDebugUtilsMessengerEXT CreateDebugMessenger(PFN_vkDebugUtilsMessengerCallbackEXT callback)
        {
            var createInfo = new VkDebugUtilsMessengerCreateInfoEXT
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
                messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
                messageType = VkDebugUtilsMessageTypeFlagsEXT.General | VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
                pfnUserCallback = Marshal.GetFunctionPointerForDelegate(callback),
                pUserData = null
            };

            trackedDelegates.Add(callback);

            var result = VK.CreateDebugUtilsMessengerEXT(Instance, &createInfo, null, out var messenger);
            if (result != VkResult.Success)
                throw new Exception(result.ToString());

            return messenger;
        }

        private VkInstance CreateVulkanInstance()
        {
            NativeString name = App.Name;
            NativeString engine = "Foster.Framework";

            // create the App Info
            var appInfo = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                pApplicationName = name,
                applicationVersion = VK.MAKE_VERSION(1, 0, 0),
                pEngineName = engine,
                engineVersion = VK.MAKE_VERSION(App.Version),
                apiVersion = VK.MAKE_VERSION(1, 0, 0),
            };

            var createInfo = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &appInfo,
            };

            // required validation layers
            using var validationLayerNames = new NativeStringArray(validationLayers);
            if (HasValidationLayers)
            {
                createInfo.enabledLayerCount = validationLayerNames.Length;
                createInfo.ppEnabledLayerNames = validationLayerNames;
            }

            // get the required Vulkan Extensions
            var exts = System.GetVKExtensions();
            if (HasValidationLayers)
                exts.Add(VkConst.EXT_DEBUG_UTILS_EXTENSION_NAME);

            using var extensions = new NativeStringArray(exts);
            createInfo.enabledExtensionCount = extensions.Length;
            createInfo.ppEnabledExtensionNames = extensions;

            // create instance
            var result = VK.CreateInstance(&createInfo, null, out var instance);
            if (result != VkResult.Success)
                throw new Exception($"Failed to create Vulkan Instance, {result}");

            return instance;
        }

        private VkPhysicalDevice PickPhysicalDevice()
        {
            uint deviceCount;
            VK.EnumeratePhysicalDevices(Instance, &deviceCount, null);

            if (deviceCount > 0)
            {
                VkPhysicalDevice* devices = stackalloc VkPhysicalDevice[(int)deviceCount];
                VkPhysicalDevice* valid = stackalloc VkPhysicalDevice[(int)deviceCount];
                VK.EnumeratePhysicalDevices(Instance, &deviceCount, devices);

                // find valid devices
                int validCount = 0;
                for (int i = 0; i < deviceCount; i++)
                {
                    if (IsValidPhysicalDevice(devices[i]))
                        valid[validCount++] = devices[i];
                }

                // find the best device
                if (validCount > 0)
                {
                    for (int i = 0; i < validCount; i++)
                    {
                        VkPhysicalDeviceProperties properties;
                        VK.GetPhysicalDeviceProperties(devices[i], &properties);

                        if (properties.deviceType == VkPhysicalDeviceType.DiscreteGpu)
                            return valid[i];
                    }

                    return valid[0];
                }
            }
            
            throw new Exception("Failed to find any GPUs that support Vulkan");

            bool IsValidPhysicalDevice(VkPhysicalDevice device)
            {
                if (!TryGetQueueFamilyIndex(device, VkQueueFlags.GraphicsBit, out _))
                    return false;

                uint extensionCount;
                VK.EnumerateDeviceExtensionProperties(device, null, &extensionCount, null);
                VkExtensionProperties* availableExtensions = stackalloc VkExtensionProperties[(int)extensionCount];
                VK.EnumerateDeviceExtensionProperties(device, null, &extensionCount, availableExtensions);

                foreach (var str in deviceExtensions)
                {
                    var hasExtension = false;
                    for (int j = 0; j < extensionCount && !hasExtension; j++)
                    {
                        if (str == VK.STRING(availableExtensions[j].extensionName))
                            hasExtension = true;
                    }

                    if (!hasExtension)
                        return false;
                }

                return true;
            }
        }

        private bool TryGetQueueFamilyIndex(VkPhysicalDevice device, VkQueueFlags flag, out uint index)
        {
            index = 0;

            uint queueFamilyCount = 0;
            VK.GetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, null);

            VkQueueFamilyProperties* queueFamilies = stackalloc VkQueueFamilyProperties[(int)queueFamilyCount];
            VK.GetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies);

            for (int i = 0; i < queueFamilyCount; i++)
            {
                if (queueFamilies[i].queueFlags.HasFlag(flag))
                {
                    index = (uint)i;
                    return true;
                }
            }

            return false;
        }

        protected override void Disposed()
        {
            if (HasValidationLayers)
                VK.DestroyDebugUtilsMessengerEXT(Instance, DebugMessenger, null);

            if (Device != IntPtr.Zero)
                VK.DestroyDevice(Device, null);

            if (Instance != IntPtr.Zero)
                VK.DestroyInstance(Instance, null);

            trackedDelegates.Clear();
        }

        public override Mesh CreateMesh()
        {
            throw new NotImplementedException();
        }

        public override RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            throw new NotImplementedException();
        }

        public override Shader CreateShader(ShaderSource source)
        {
            throw new NotImplementedException();
        }

        public override Texture CreateTexture(int width, int height, TextureFormat format)
        {
            throw new NotImplementedException();
        }

        protected override void ClearInternal(RenderTarget target, ClearFlags flags, Color color, float depth, int stencil)
        {
            throw new NotImplementedException();
        }

        protected override void RenderInternal(RenderTarget target, ref RenderPass pass)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetVulkanInstancePointer()
        {
            return Instance;
        }
    }
}
