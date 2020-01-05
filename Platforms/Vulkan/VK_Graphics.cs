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
        internal VkInstance Instance;
        internal VkPhysicalDevice PhysicalDevice;
        internal VK VK;

#if DEBUG
        private readonly string[] requestedValidationLayers = new[] { "VK_LAYER_KHRONOS_validation" };
#else
        private readonly string[] requestedValidationLayers = new string[0];
#endif
        private readonly List<string> validationLayers = new List<string>();
        private bool HasValidationLayers => validationLayers.Count > 0;
        private VkDebugUtilsMessengerEXT debugMessenger;
        private PFN_vkDebugUtilsMessengerCallbackEXT debugMessengerCallback;


        protected override void Initialized()
        {
            ApiName = "Vulkan";
        }

        protected override void Startup()
        {
            // Init Static delegates
            VK.InitStaticDelegates(System);

            // Find available validation layers
            if (requestedValidationLayers.Length > 0)
            {
                uint availableLayerCount;
                VK.EnumerateInstanceLayerProperties(&availableLayerCount, null);
                VkLayerProperties* availableLayers = stackalloc VkLayerProperties[(int)availableLayerCount];
                VK.EnumerateInstanceLayerProperties(&availableLayerCount, availableLayers);

                for (int i = 0; i < requestedValidationLayers.Length; i ++)
                {
                    var hasLayer = false;
                    for (int j = 0; j < availableLayerCount; j ++)
                        if (requestedValidationLayers[i] == VK.STRING(availableLayers[j].layerName))
                        {
                            hasLayer = true;
                            break;
                        }

                    if (hasLayer)
                        validationLayers.Add(requestedValidationLayers[i]);
                    else
                        Log.Warning(Name, $"Validation Layer {requestedValidationLayers[i]} requested but does not exist. You may need to install the Vulkan SDK.");
                }
            }

            // Create the Vulkan Instance
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
                if (HasValidationLayers)
                {
                    // get the required Vulkan Extensions
                    var layers = new NativeArray<NativeString>(validationLayers.Count);
                    for (int i = 0; i < validationLayers.Count; i++)
                        layers[i] = new NativeString(validationLayers[i]);

                    createInfo.enabledLayerCount = layers.Length;
                    createInfo.ppEnabledLayerNames = layers;
                }

                // required extenstions
                {
                    // get the required Vulkan Extensions
                    var exts = System.GetVKExtensions();
                    if (HasValidationLayers)
                        exts.Add(VkConst.EXT_DEBUG_UTILS_EXTENSION_NAME);

                    var extensions = new NativeArray<NativeString>(exts.Count);
                    for (int i = 0; i < exts.Count; i++)
                        extensions[i] = new NativeString(exts[i]);

                    createInfo.enabledExtensionCount = extensions.Length;
                    createInfo.ppEnabledExtensionNames = extensions;
                }

                // create instance
                var result = VK.CreateInstance(&createInfo, null, out Instance);
                if (result != VkResult.Success)
                    throw new Exception($"Failed to create Vulkan Instance, {result}");
            }

            // bind all VK calls now that we have the instance
            VK = new VK(this);

            // Debug Messenger
            if (HasValidationLayers)
            {
                debugMessengerCallback = (messageSeverity, messageTypes, pCallbackData, pUserData) =>
                {
                    var message = VK.STRING(pCallbackData->pMessage);

                    if (messageSeverity.HasFlag(VkDebugUtilsMessageSeverityFlagsEXT.Error))
                        Log.Error(Name, message);
                    else if (messageSeverity.HasFlag(VkDebugUtilsMessageSeverityFlagsEXT.Warning))
                        Log.Warning(Name, message);
                    else
                        Log.Message(Name, message);

                    return VkConst.FALSE;
                };

                var createInfo = new VkDebugUtilsMessengerCreateInfoEXT
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
                    messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Info | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error,
                    messageType = VkDebugUtilsMessageTypeFlagsEXT.General | VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
                    pfnUserCallback = Marshal.GetFunctionPointerForDelegate(debugMessengerCallback),
                    pUserData = null
                };

                var result = VK.CreateDebugUtilsMessengerEXT(Instance, &createInfo, null, out debugMessenger);
                if (result != VkResult.Success)
                    throw new Exception(result.ToString());
            }

            // Pick a Physical Device
            {
                uint deviceCount;
                VK.EnumeratePhysicalDevices(Instance, &deviceCount, null);

                if (deviceCount <= 0)
                    throw new Exception("Failed to find any GPUs that support Vulkan");

                VkPhysicalDevice* devices = stackalloc VkPhysicalDevice[(int)deviceCount];
                VK.EnumeratePhysicalDevices(Instance, &deviceCount, devices);
                PhysicalDevice = devices[0];
            }

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

            base.Startup();
        }

        protected override void Disposed()
        {
            if (HasValidationLayers)
                VK.DestroyDebugUtilsMessengerEXT(Instance, debugMessenger, null);
            VK.DestroyInstance(Instance, null);
        }

        public override Mesh CreateMesh()
        {
            throw new NotImplementedException();
        }

        public override RenderTexture CreateRenderTexture(int width, int height, TextureFormat[] colorAttachmentFormats, TextureFormat depthFormat)
        {
            throw new NotImplementedException();
        }

        public override Shader CreateShader(string vertexSource, string fragmentSource)
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
