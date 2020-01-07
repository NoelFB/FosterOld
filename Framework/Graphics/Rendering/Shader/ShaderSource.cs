using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
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
    public class ShaderSource
    {

        public class Program
        {
            public readonly ShaderProgram Type;
            public readonly byte[] Source;

            public Program(ShaderProgram type, byte[] source)
            {
                Type = type;
                Source = source;
            }

            public Program(ShaderProgram type, string source)
            {
                Type = type;
                Source = Encoding.UTF8.GetBytes(source);
            }

        }

        public List<Program> Programs = new List<Program>();

        public ShaderSource(params Program[] programs)
        {
            Programs.AddRange(programs);
        }

        public ShaderSource(string vertexSource, string fragmentSource, string? geomSource = null)
        {
            if (!string.IsNullOrEmpty(vertexSource))
                Programs.Add(new Program(ShaderProgram.Vertex, vertexSource));

            if (!string.IsNullOrEmpty(fragmentSource))
                Programs.Add(new Program(ShaderProgram.Fragment, fragmentSource));

            if (!string.IsNullOrEmpty(geomSource))
                Programs.Add(new Program(ShaderProgram.Geometry, geomSource));
        }

    }
}
