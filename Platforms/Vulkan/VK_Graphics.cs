using Foster.Framework;
using System;
using System.Collections.Generic;
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

        protected override void Initialized()
        {
            ApiName = "Vulkan";
        }

        protected override void Startup()
        {
            // Crate the Vulkan Instance
            {
                NativeString name = App.Name;
                NativeString engine = "Foster.Framework";

                // create the App Info
                VkApplicationInfo appInfo = new VkApplicationInfo
                {
                    sType = VkStructureType.ApplicationInfo,
                    pApplicationName = name,
                    applicationVersion = Utils.ToVulkanVersion(1, 0, 0),
                    pEngineName = engine,
                    engineVersion = Utils.ToVulkanVersion(App.Version.Major, App.Version.Minor, App.Version.Revision),
                    apiVersion = Utils.ToVulkanVersion(1, 0, 0),
                };

                // get the required Vulkan Extensions
                var exts = System.GetVKExtensions();
                var extensions = new NativeArray<NativeString>(exts.Count);
                for (int i = 0; i < exts.Count; i++)
                    extensions[i] = new NativeString(exts[i]);

                VkInstanceCreateInfo createInfo = new VkInstanceCreateInfo
                {
                    sType = VkStructureType.InstanceCreateInfo,
                    pApplicationInfo = &appInfo,
                    enabledExtensionCount = extensions.Length,
                    ppEnabledExtensionNames = extensions
                };

                // create instance
                var result = VK.CreateInstance(System, &createInfo, null, out Instance);
                if (result != VkResult.Success)
                    throw new Exception($"Failed to create Vulkan Instance, {result}");

            }

            // bind all VK calls now that we have the instance
            VK = new VK(this);

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
                ApiVersion = Utils.FromVulkanVersion(properties.apiVersion);
            }

            base.Startup();
        }

        protected override void Disposed()
        {
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
