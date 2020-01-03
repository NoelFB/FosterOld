using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    ///
    /// TODO: This isn't api-agnostic and doesn't represent the final implementation
    /// 
    /// Not sure what the best way to implement this is:
    /// 
    ///     1)  Create our own Shader language that each platform can convert
    ///         into their required language. This has a lot of drawbacks
    ///         because it needs to be very easy to parse somehow ...
    ///         
    ///     2)  Require either GLSL or HLSL and use a 3rd party tool to convert them.
    ///         The problem here is that I don't want to add more dependencies (especially
    ///         since most of these tools are offline / not in C#). I would like everything to
    ///         be cross-platform and runtime available.
    /// 
    /// </summary>
    public abstract class Shader
    {
        /// <summary>
        /// List of all Vertex Attributes, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderAttribute> Attributes;

        /// <summary>
        /// List of all Uniforms, by Name
        /// </summary>
        public readonly ReadOnlyDictionary<string, ShaderUniform> Uniforms;


        protected readonly Dictionary<string, ShaderAttribute> attributes = new Dictionary<string, ShaderAttribute>();
        protected readonly Dictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();

        public static Shader Create(string vertex, string fragment)
        {
            return App.Graphics.CreateShader(vertex, fragment);
        }

        public static Shader Create(Graphics graphics, string vertex, string fragment)
        {
            return graphics.CreateShader(vertex, fragment);
        }

        protected Shader()
        {
            Uniforms = new ReadOnlyDictionary<string, ShaderUniform>(uniforms);
            Attributes = new ReadOnlyDictionary<string, ShaderAttribute>(attributes);
        }

    }
}
