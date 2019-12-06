using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// TODO: This isn't api-agnostic ... 
    /// Not sure what the best way to implement this is:
    /// 
    ///     1)  Create our own Shader "language" that each platform can convert
    ///         into their required language. This has a lot of drawbacks
    ///         because it needs to be very easy to parse somehow ...
    ///         
    ///     2)  Require either GLSL or HLSL and use a 3rd party tool to convert them.
    ///         The problem here is that I don't want to add more dependencies (especially
    ///         since most of these tools are offline / not in C#)
    /// 
    /// </summary>
    public class Shader : GraphicsResource
    {

        /// <summary>
        /// Internal Shader object
        /// </summary>
        public readonly InternalShader Internal;

        /// <summary>
        /// List of all Vertex Attributes, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderAttribute> Attributes;

        /// <summary>
        /// List of all Uniforms, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;

        public Shader(string vertex, string fragment) : this(App.Graphics, vertex, fragment)
        {

        }

        public Shader(Graphics graphics, string vertex, string fragment) : base(graphics)
        {
            Internal = graphics.CreateShader(vertex, fragment);
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(Internal.uniforms);
            Attributes = new ReadOnlyDictionary<string, ShaderAttribute>(Internal.attributes);
        }

        public override void Dispose()
        {
            if (!Disposed)
            {
                base.Dispose();
                Internal.Dispose();
            }
        }

    }
}
