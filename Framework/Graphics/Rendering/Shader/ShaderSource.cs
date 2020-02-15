using System;
using System.Collections.Generic;
using System.IO;
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
    ///     3)  Don't worry about it and just make the end-user write shaders for each platform
    ///         they intend to use, and load them manually. (** what currently happens **)
    /// 
    /// </summary>
    public class ShaderSource
    {

        public byte[]? Vertex;
        public byte[]? Fragment;
        public byte[]? Geometry;

        public ShaderSource(string vertexSource, string fragmentSource, string? geomSource = null)
        {
            if (!string.IsNullOrEmpty(vertexSource))
                Vertex = Encoding.UTF8.GetBytes(vertexSource);

            if (!string.IsNullOrEmpty(fragmentSource))
                Fragment = Encoding.UTF8.GetBytes(fragmentSource);

            if (!string.IsNullOrEmpty(geomSource))
                Geometry = Encoding.UTF8.GetBytes(geomSource);
        }

        public ShaderSource(Stream vertexSource, Stream fragmentSource, Stream? geomSource = null)
        {
            if (vertexSource != null)
            {
                Vertex = new byte[vertexSource.Length];
                vertexSource.Read(Vertex, 0, Vertex.Length);
            }

            if (fragmentSource != null)
            {
                Fragment = new byte[fragmentSource.Length];
                fragmentSource.Read(Fragment, 0, Fragment.Length);
            }

            if (geomSource != null)
            {
                Geometry = new byte[geomSource.Length];
                geomSource.Read(Geometry, 0, Geometry.Length);
            }
        }

    }
}
