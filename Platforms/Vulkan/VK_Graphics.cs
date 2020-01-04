using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    public unsafe class VK_Graphics : Graphics, IGraphicsVulkan
    {

        private ISystemVulkan System => App.System as ISystemVulkan ?? throw new Exception("System does not implement ISystemVulkan");
        private IntPtr vkInstance;

        protected override void Initialized()
        {
            ApiName = "Vulkan";
        }

        protected override void Startup()
        {
            VK.Init(System);

            NativeString name = App.Name;
            NativeString engine = "Foster.Framework";

            // create the App Info
            VkApplicationInfo appInfo = new VkApplicationInfo
            {
                sType = VkStructureType.APPLICATION_INFO,
                pApplicationName = name,
                applicationVersion = VK_Utils.Version(1, 0, 0),
                pEngineName = engine,
                engineVersion = VK_Utils.Version(App.Version.Major, App.Version.Minor, App.Version.Revision),
                apiVersion = VK_Utils.Version(1, 0, 0),
            };

            // get the required Vulkan Extensions
            var extensions = System.GetVKExtensions();
            var e = new NativeArray<NativeString>(extensions.Count);
            for (int i = 0; i < extensions.Count; i++)
                e[i] = new NativeString(extensions[i]);

            VkInstanceCreateInfo createInfo = new VkInstanceCreateInfo
            {
                sType = VkStructureType.INSTANCE_CREATE_INFO,
                pApplicationInfo = &appInfo,
                enabledExtensionCount = e.Length,
                ppEnabledExtensionNames = e
            };

            // create the Vulkan Instance
            VK.CreateInstance(&createInfo, null, out vkInstance);
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
            return vkInstance;
        }
    }
}
