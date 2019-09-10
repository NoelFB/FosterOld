using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Graphics : Module
    {

        public GraphicsApi Api { get; protected set; }
        public string? ApiName { get; protected set; }
        public Version? ApiVersion { get; protected set; }
        public int MaxTextureSize { get; protected set; }

        public abstract Texture CreateTexture(int width, int height);
        public abstract Target CreateTarget(int width, int height);
        public abstract Shader CreateShader(int width, int height);
        public abstract Mesh<T> CreateMesh<T>() where T : struct;

        public abstract void Target(Target? target);
        public abstract void Clear(Color color);

        protected internal override void OnDisplayed()
        {
            Console.WriteLine($" - Graphics {ApiName} {ApiVersion}");
        }

    }
}
