using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// A Shader used for Rendering
    /// </summary>
    public class Shader : IDisposable
    {

        public abstract class Platform
        {
            protected internal readonly Dictionary<string, ShaderAttribute> Attributes = new Dictionary<string, ShaderAttribute>();
            protected internal readonly Dictionary<string, ShaderUniform> Uniforms = new Dictionary<string, ShaderUniform>();
            protected internal abstract void Dispose();
        }

        /// <summary>
        /// A reference to the internal platform implementation of the Shader
        /// </summary>
        public readonly Platform Implementation;

        /// <summary>
        /// List of all Vertex Attributes, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderAttribute> Attributes;

        /// <summary>
        /// List of all Uniforms, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;

        public Shader(Graphics graphics, ShaderSource source)
        {
            Implementation = graphics.CreateShader(source);
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(Implementation.Uniforms);
            Attributes = new ReadOnlyDictionary<string, ShaderAttribute>(Implementation.Attributes);
        }

        public Shader(ShaderSource source) : this(App.Graphics, source)
        {
        }

        public void Dispose()
        {
            Implementation.Dispose();
        }

    }
}
