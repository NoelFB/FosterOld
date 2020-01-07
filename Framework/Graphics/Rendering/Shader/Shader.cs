using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    public abstract class Shader : IDisposable
    {
        /// <summary>
        /// List of all Vertex Attributes, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderAttribute> Attributes;

        /// <summary>
        /// List of all Uniforms, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;

        /// <summary>
        /// Internal list of Attributes, which should be managed by the Shader implementation
        /// </summary>
        protected readonly Dictionary<string, ShaderAttribute> attributes = new Dictionary<string, ShaderAttribute>();

        /// <summary>
        /// Internal list of Uniforms, which should be managed by the Shader implementation
        /// </summary>
        protected readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        public static Shader Create(ShaderSource source)
        {
            return App.Graphics.CreateShader(source);
        }

        public static Shader Create(Graphics graphics, ShaderSource source)
        {
            return graphics.CreateShader(source);
        }

        protected Shader()
        {
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(uniforms);
            Attributes = new ReadOnlyDictionary<string, ShaderAttribute>(attributes);
        }

        public abstract void Dispose();

    }
}
