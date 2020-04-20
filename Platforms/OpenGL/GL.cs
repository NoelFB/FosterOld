using Foster.Framework;
using System;
using System.Runtime.InteropServices;

namespace Foster.OpenGL
{
    internal static class GL
    {

        public static int MajorVersion;
        public static int MinorVersion;
        public static int MaxColorAttachments;
        public static int MaxCubeMapTextureSize;
        public static int MaxDrawBuffers;
        public static int MaxElementIndices;
        public static int MaxElementVertices;
        public static int MaxRenderbufferSize;
        public static int MaxSamples;
        public static int MaxTextureImageUnits;
        public static int MaxTextureSize;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private static GL_Bindings bindings;
        private static OnError onErrorRef;
#pragma warning restore CS8618

        public static void Init(GL_Graphics graphics, ISystemOpenGL system)
        {
            bindings = new GL_Bindings(system);

            GetIntegerv((GLEnum)0x821B, out MajorVersion);
            GetIntegerv((GLEnum)0x821C, out MinorVersion);
            GetIntegerv((GLEnum)0x8CDF, out MaxColorAttachments);
            GetIntegerv((GLEnum)0x851C, out MaxCubeMapTextureSize);
            GetIntegerv((GLEnum)0x8824, out MaxDrawBuffers);
            GetIntegerv((GLEnum)0x80E9, out MaxElementIndices);
            GetIntegerv((GLEnum)0x80E8, out MaxElementVertices);
            GetIntegerv((GLEnum)0x84E8, out MaxRenderbufferSize);
            GetIntegerv((GLEnum)0x8D57, out MaxSamples);
            GetIntegerv((GLEnum)0x8872, out MaxTextureImageUnits);
            GetIntegerv((GLEnum)0x0D33, out MaxTextureSize);

#if DEBUG
            if (bindings.glDebugMessageCallback != null)
            {
                Enable(GLEnum.DEBUG_OUTPUT);
                Enable(GLEnum.DEBUG_OUTPUT_SYNCHRONOUS);

                DebugMessageCallback(Marshal.GetFunctionPointerForDelegate(onErrorRef = new OnError((source, type, id, severity, length, message, userParam) =>
                {
                    string typeName;
                    string severityName;
                    string output = Marshal.PtrToStringAnsi(message, (int)length);

                    switch (type)
                    {
                        case GLEnum.DEBUG_TYPE_ERROR: typeName = "ERROR"; break;
                        case GLEnum.DEBUG_TYPE_DEPRECATED_BEHAVIOR: typeName = "DEPRECATED BEHAVIOR"; break;
                        case GLEnum.DEBUG_TYPE_MARKER: typeName = "MARKER"; break;
                        case GLEnum.DEBUG_TYPE_OTHER: typeName = "OTHER"; break;
                        case GLEnum.DEBUG_TYPE_PERFORMANCE: typeName = "PEROFRMANCE"; break;
                        case GLEnum.DEBUG_TYPE_POP_GROUP: typeName = "POP GROUP"; break;
                        case GLEnum.DEBUG_TYPE_PORTABILITY: typeName = "PORTABILITY"; break;
                        case GLEnum.DEBUG_TYPE_PUSH_GROUP: typeName = "PUSH GROUP"; break;
                        default: case GLEnum.DEBUG_TYPE_UNDEFINED_BEHAVIOR: typeName = "UNDEFINED BEHAVIOR"; break;
                    }

                    switch (severity)
                    {
                        case GLEnum.DEBUG_SEVERITY_HIGH: severityName = "HIGH"; break;
                        case GLEnum.DEBUG_SEVERITY_MEDIUM: severityName = "MEDIUM"; break;
                        case GLEnum.DEBUG_SEVERITY_LOW: severityName = "LOW"; break;

                        // skip notifications
                        default:
                        case GLEnum.DEBUG_SEVERITY_NOTIFICATION:
                            return;
                    }

                    if (type == GLEnum.DEBUG_TYPE_ERROR)
                    {
                        throw new Exception(output);
                    }

                    Log.Warning($"OpenGL {typeName}, {severityName}: {output}");

                })), IntPtr.Zero);
            }

#endif
        }

        private delegate void OnError(GLEnum source, GLEnum type, uint id, GLEnum severity, uint length, IntPtr message, IntPtr userParam);

        public static unsafe string GetString(GLEnum name)
        {
            return Marshal.PtrToStringAnsi(bindings.glGetString(name)) ?? "";
        }

        public static void DebugMessageCallback(IntPtr callback, IntPtr userdata) => bindings.glDebugMessageCallback(callback, userdata);

        public static void Flush() => bindings.glFlush();

        public static void Enable(GLEnum mode) => bindings.glEnable(mode);

        public static void Disable(GLEnum mode) => bindings.glDisable(mode);

        public static void Clear(GLEnum mask) => bindings.glClear(mask);

        public static void ClearColor(float red, float green, float blue, float alpha) => bindings.glClearColor(red, green, blue, alpha);

        public static void ClearDepth(double depth) => bindings.glClearDepth(depth);

        public static void ClearStencil(int stencil) => bindings.glClearStencil(stencil);

        public static void DepthMask(bool enabled) => bindings.glDepthMask(enabled);

        public static void DepthFunc(GLEnum func) => bindings.glDepthFunc(func);

        public static void Viewport(int x, int y, int width, int height) => bindings.glViewport(x, y, width, height);

        public static void Scissor(int x, int y, int width, int height) => bindings.glScissor(x, y, width, height);

        public static void CullFace(GLEnum mode) => bindings.glCullFace(mode);

        public static void BlendEquation(GLEnum eq) => bindings.glBlendEquation(eq);

        public static void BlendEquationSeparate(GLEnum modeRGB, GLEnum modeAlpha) => bindings.glBlendEquationSeparate(modeRGB, modeAlpha);

        public static void BlendFunc(GLEnum sfactor, GLEnum dfactor) => bindings.glBlendFunc(sfactor, dfactor);

        public static void BlendFuncSeparate(GLEnum srcRGB, GLEnum dstRGB, GLEnum srcAlpha, GLEnum dstAlpha) => bindings.glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);

        public static void BlendColor(float red, float green, float blue, float alpha) => bindings.glBlendColor(red, green, blue, alpha);

        public static void ColorMask(bool red, bool green, bool blue, bool alpha) => bindings.glColorMask(red, green, blue, alpha);

        public static void GetIntegerv(GLEnum name, out int data) => bindings.glGetIntegerv(name, out data);

        public static void GenTextures(int n, IntPtr textures) => bindings.glGenTextures(n, textures);

        public static unsafe uint GenTexture()
        {
            uint id;
            bindings.glGenTextures(1, new IntPtr(&id));
            return id;
        }

        public static void GenRenderbuffers(int n, IntPtr textures) => bindings.glGenRenderbuffers(n, textures);

        public static unsafe uint GenRenderbuffer()
        {
            uint id;
            bindings.glGenRenderbuffers(1, new IntPtr(&id));
            return id;
        }

        public static void GenFramebuffers(int n, IntPtr textures) => bindings.glGenFramebuffers(n, textures);

        public static unsafe uint GenFramebuffer()
        {
            uint id;
            bindings.glGenFramebuffers(1, new IntPtr(&id));
            return id;
        }

        public static void ActiveTexture(uint id) => bindings.glActiveTexture(id);

        public static void BindTexture(GLEnum target, uint id) => bindings.glBindTexture(target, id);

        public static void BindRenderbuffer(GLEnum target, uint id) => bindings.glBindRenderbuffer(target, id);

        public static void BindFramebuffer(GLEnum target, uint id) => bindings.glBindFramebuffer(target, id);

        public static void TexImage2D(GLEnum target, int level, GLEnum internalFormat, int width, int height, int border, GLEnum format, GLEnum type, IntPtr data) => bindings.glTexImage2D(target, level, internalFormat, width, height, border, format, type, data);

        public static void FramebufferRenderbuffer(GLEnum target​, GLEnum attachment​, GLEnum renderbuffertarget​, uint renderbuffer) => bindings.glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);

        public static void FramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level) => bindings.glFramebufferTexture2D(target, attachment, textarget, texture, level);

        public static void TexParameteri(GLEnum target, GLEnum name, int param) => bindings.glTexParameteri(target, name, param);

        public static void RenderbufferStorage(GLEnum target​, GLEnum internalformat​, int width​, int height​) => bindings.glRenderbufferStorage(target, internalformat, width, height);

        public static void GetTexImage(GLEnum target, int level, GLEnum format, GLEnum type, IntPtr data) => bindings.glGetTexImage(target, level, format, type, data);

        public static void DrawElements(GLEnum mode, int count, GLEnum type, IntPtr indices) => bindings.glDrawElements(mode, count, type, indices);

        public static void DrawElementsInstanced(GLEnum mode, int count, GLEnum type, IntPtr indices, int amount) => bindings.glDrawElementsInstanced(mode, count, type, indices, amount);

        public static unsafe void DeleteTextures(ReadOnlyMemory<uint> textures)
        {
            using var pin = textures.Pin();
            bindings.glDeleteTextures(textures.Length, (uint*)pin.Pointer);
        }

        public static unsafe void DeleteTexture(uint id)
        {
            bindings.glDeleteTextures(1, &id);
        }

        public static unsafe void DeleteRenderbuffers(ReadOnlyMemory<uint> renderbuffers)
        {
            using var pin = renderbuffers.Pin();
            bindings.glDeleteRenderbuffers(renderbuffers.Length, (uint*)pin.Pointer);
        }

        public static unsafe void DeleteRenderbuffer(uint id)
        {
            bindings.glDeleteRenderbuffers(1, &id);
        }

        public static unsafe void DeleteFramebuffers(ReadOnlyMemory<uint> framebuffers)
        {
            using var pin = framebuffers.Pin();
            bindings.glDeleteFramebuffers(framebuffers.Length, (uint*)pin.Pointer);
        }

        public static unsafe void DeleteFramebuffer(uint id)
        {
            bindings.glDeleteFramebuffers(1, &id);
        }

        public static unsafe void GenVertexArrays(Memory<uint> arrays)
        {
            using var pin = arrays.Pin();
            bindings.glGenVertexArrays(arrays.Length, (uint*)pin.Pointer);
        }

        public static unsafe uint GenVertexArray()
        {
            uint id;
            bindings.glGenVertexArrays(1, &id);
            return id;
        }

        public static void BindVertexArray(uint id) => bindings.glBindVertexArray(id);

        public static unsafe void GenBuffers(Memory<uint> arrays)
        {
            using var pin = arrays.Pin();
            bindings.glGenBuffers(arrays.Length, (uint*)pin.Pointer);
        }

        public static unsafe uint GenBuffer()
        {
            uint id;
            bindings.glGenBuffers(1, &id);
            return id;
        }

        public static void BindBuffer(GLEnum target, uint buffer) => bindings.glBindBuffer(target, buffer);

        public static void BufferData(GLEnum target, IntPtr size, IntPtr data, GLEnum usage) => bindings.glBufferData(target, size, data, usage);

        public static void BufferSubData(GLEnum target, IntPtr offset, IntPtr size, IntPtr data) => bindings.glBufferSubData(target, offset, size, data);

        public static unsafe void DeleteBuffers(ReadOnlyMemory<uint> buffers)
        {
            using var pin = buffers.Pin();
            bindings.glDeleteBuffers(buffers.Length, (uint*)pin.Pointer);
        }

        public static unsafe void DeleteBuffer(uint id)
        {
            bindings.glDeleteBuffers(1, &id);
        }

        public static unsafe void DeleteVertexArrays(ReadOnlyMemory<uint> arrays)
        {
            using var pin = arrays.Pin();
            bindings.glDeleteVertexArrays(arrays.Length, (uint*)pin.Pointer);
        }

        public static unsafe void DeleteVertexArray(uint id)
        {
            bindings.glDeleteVertexArrays(1, &id);
        }

        public static void EnableVertexAttribArray(uint location) => bindings.glEnableVertexAttribArray(location);

        public static void DisableVertexAttribArray(uint location) => bindings.glDisableVertexAttribArray(location);

        public static void VertexAttribPointer(uint index, int size, GLEnum type, bool normalized, int stride, IntPtr pointer) => bindings.glVertexAttribPointer(index, size, type, normalized, stride, pointer);

        public static void VertexAttribDivisor(uint index, uint divisor) => bindings.glVertexAttribDivisor(index, divisor);

        public static uint CreateShader(GLEnum type) => bindings.glCreateShader(type);

        public static void AttachShader(uint program, uint shader) => bindings.glAttachShader(program, shader);

        public static void DetachShader(uint program, uint shader) => bindings.glDetachShader(program, shader);

        public static void DeleteShader(uint shader) => bindings.glDeleteShader(shader);

        public static void ShaderSource(uint shader, int count, string[] source, int[] length) => bindings.glShaderSource(shader, count, source, length);

        public static void CompileShader(uint shader) => bindings.glCompileShader(shader);

        public static void GetShaderiv(uint shader, GLEnum pname, out int result) => bindings.glGetShaderiv(shader, pname, out result);

        public static unsafe string? GetShaderInfoLog(uint shader)
        {
            bindings.glGetShaderiv(shader, (GLEnum)0x8B84, out int len);

            char* bytes = stackalloc char[len];
            IntPtr ptr = new IntPtr(bytes);

            bindings.glGetShaderInfoLog(shader, len, out len, ptr);

            if (len <= 0)
            {
                return null;
            }

            return Marshal.PtrToStringAnsi(ptr, len);
        }

        public static uint CreateProgram() => bindings.glCreateProgram();

        public static void DeleteProgram(uint program) => bindings.glDeleteProgram(program);

        public static void LinkProgram(uint program) => bindings.glLinkProgram(program);

        public static void GetProgramiv(uint program, GLEnum pname, out int result) => bindings.glGetProgramiv(program, pname, out result);

        public static unsafe string? GetProgramInfoLog(uint program)
        {
            bindings.glGetProgramiv(program, (GLEnum)0x8B84, out int len);

            char* bytes = stackalloc char[len];
            IntPtr ptr = new IntPtr(bytes);

            bindings.glGetProgramInfoLog(program, len, out len, ptr);

            if (len <= 0)
            {
                return null;
            }

            return Marshal.PtrToStringAnsi(ptr, len);
        }

        public static unsafe void GetActiveUniform(uint program, uint index, out int size, out GLEnum type, out string name)
        {
            var uniformName = stackalloc char[256];
            var ptr = new IntPtr(uniformName);

            bindings.glGetActiveUniform(program, index, 256, out int length, out size, out type, ptr);

            name = Marshal.PtrToStringAnsi(ptr, length) ?? "";
        }

        public static unsafe void GetActiveAttrib(uint program, uint index, out int size, out GLEnum type, out string name)
        {
            var uniformName = stackalloc char[256];
            var ptr = new IntPtr(uniformName);

            bindings.glGetActiveAttrib(program, index, 256, out int length, out size, out type, ptr);

            name = Marshal.PtrToStringAnsi(ptr, length) ?? "";
        }

        public static void UseProgram(uint program) => bindings.glUseProgram(program);

        public static int GetUniformLocation(uint program, string name) => bindings.glGetUniformLocation(program, name);

        public static int GetAttribLocation(uint program, string name) => bindings.glGetAttribLocation(program, name);

        public static void Uniform1f(int location, float v0) => bindings.glUniform1f(location, v0);

        public static void Uniform2f(int location, float v0, float v1) => bindings.glUniform2f(location, v0, v1);

        public static void Uniform3f(int location, float v0, float v1, float v2) => bindings.glUniform3f(location, v0, v1, v2);

        public static void Uniform4f(int location, float v0, float v1, float v2, float v3) => bindings.glUniform4f(location, v0, v1, v2, v3);

        public static void Uniform1fv(int location, int count, IntPtr value) => bindings.glUniform1fv(location, count, value);

        public static void Uniform2fv(int location, int count, IntPtr value) => bindings.glUniform2fv(location, count, value);

        public static void Uniform3fv(int location, int count, IntPtr value) => bindings.glUniform3fv(location, count, value);

        public static void Uniform4fv(int location, int count, IntPtr value) => bindings.glUniform4fv(location, count, value);

        public static void Uniform1i(int location, int v0) => bindings.glUniform1i(location, v0);

        public static void Uniform2i(int location, int v0, int v1) => bindings.glUniform2i(location, v0, v1);

        public static void Uniform3i(int location, int v0, int v1, int v2) => bindings.glUniform3i(location, v0, v1, v2);

        public static void Uniform4i(int location, int v0, int v1, int v2, int v3) => bindings.glUniform4i(location, v0, v1, v2, v3);

        public static void Uniform1iv(int location, int count, IntPtr value) => bindings.glUniform1iv(location, count, value);

        public static void Uniform2iv(int location, int count, IntPtr value) => bindings.glUniform2iv(location, count, value);

        public static void Uniform3iv(int location, int count, IntPtr value) => bindings.glUniform3iv(location, count, value);

        public static void Uniform4iv(int location, int count, IntPtr value) => bindings.glUniform4iv(location, count, value);

        public static void Uniform1ui(int location, uint v0) => bindings.glUniform1ui(location, v0);

        public static void Uniform2ui(int location, uint v0, uint v1) => bindings.glUniform2ui(location, v0, v1);

        public static void Uniform3ui(int location, uint v0, uint v1, uint v2) => bindings.glUniform3ui(location, v0, v1, v2);

        public static void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3) => bindings.glUniform4ui(location, v0, v1, v2, v3);

        public static void Uniform1uiv(int location, int count, IntPtr value) => bindings.glUniform1uiv(location, count, value);

        public static void Uniform2uiv(int location, int count, IntPtr value) => bindings.glUniform2uiv(location, count, value);

        public static void Uniform3uiv(int location, int count, IntPtr value) => bindings.glUniform3uiv(location, count, value);

        public static void Uniform4uiv(int location, int count, IntPtr value) => bindings.glUniform4uiv(location, count, value);

        public static void UniformMatrix2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2fv(location, count, transpose, value);

        public static void UniformMatrix3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3fv(location, count, transpose, value);

        public static void UniformMatrix4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4fv(location, count, transpose, value);

        public static void UniformMatrix2x3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2x3fv(location, count, transpose, value);

        public static void UniformMatrix3x2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3x2fv(location, count, transpose, value);

        public static void UniformMatrix2x4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2x4fv(location, count, transpose, value);

        public static void UniformMatrix4x2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4x2fv(location, count, transpose, value);

        public static void UniformMatrix3x4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3x4fv(location, count, transpose, value);

        public static void UniformMatrix4x3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4x3fv(location, count, transpose, value);

    }

    internal enum GLEnum
    {
        // Hint Enum Value
        DONT_CARE = 0x1100,
        // 0/1
        ZERO = 0x0000,
        ONE = 0x0001,
        // Types
        BYTE = 0x1400,
        UNSIGNED_BYTE = 0x1401,
        SHORT = 0x1402,
        UNSIGNED_SHORT = 0x1403,
        INT = 0x1404,
        UNSIGNED_INT = 0x1405,
        FLOAT = 0x1406,
        HALF_FLOAT = 0x140B,
        UNSIGNED_SHORT_4_4_4_4_REV = 0x8365,
        UNSIGNED_SHORT_5_5_5_1_REV = 0x8366,
        UNSIGNED_INT_2_10_10_10_REV = 0x8368,
        UNSIGNED_SHORT_5_6_5 = 0x8363,
        UNSIGNED_INT_24_8 = 0x84FA,
        // Strings
        VENDOR = 0x1F00,
        RENDERER = 0x1F01,
        VERSION = 0x1F02,
        EXTENSIONS = 0x1F03,
        // Clear Mask
        COLOR_BUFFER_BIT = 0x4000,
        DEPTH_BUFFER_BIT = 0x0100,
        STENCIL_BUFFER_BIT = 0x0400,
        // Enable Caps
        SCISSOR_TEST = 0x0C11,
        DEPTH_TEST = 0x0B71,
        STENCIL_TEST = 0x0B90,
        // Polygons
        LINE = 0x1B01,
        FILL = 0x1B02,
        CW = 0x0900,
        CCW = 0x0901,
        FRONT = 0x0404,
        BACK = 0x0405,
        FRONT_AND_BACK = 0x0408,
        CULL_FACE = 0x0B44,
        POLYGON_OFFSET_FILL = 0x8037,
        // Texture Type
        TEXTURE_2D = 0x0DE1,
        TEXTURE_3D = 0x806F,
        TEXTURE_CUBE_MAP = 0x8513,
        TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515,
        // Blend Mode
        BLEND = 0x0BE2,
        SRC_COLOR = 0x0300,
        ONE_MINUS_SRC_COLOR = 0x0301,
        SRC_ALPHA = 0x0302,
        ONE_MINUS_SRC_ALPHA = 0x0303,
        DST_ALPHA = 0x0304,
        ONE_MINUS_DST_ALPHA = 0x0305,
        DST_COLOR = 0x0306,
        ONE_MINUS_DST_COLOR = 0x0307,
        SRC_ALPHA_SATURATE = 0x0308,
        CONSTANT_COLOR = 0x8001,
        ONE_MINUS_CONSTANT_COLOR = 0x8002,
        CONSTANT_ALPHA = 0x8003,
        ONE_MINUS_CONSTANT_ALPHA = 0x8004,
        SRC1_ALPHA = 0x8589,
        SRC1_COLOR = 0x88F9,
        ONE_MINUS_SRC1_COLOR = 0x88FA,
        ONE_MINUS_SRC1_ALPHA = 0x88FB,
        // Equations
        MIN = 0x8007,
        MAX = 0x8008,
        FUNC_ADD = 0x8006,
        FUNC_SUBTRACT = 0x800A,
        FUNC_REVERSE_SUBTRACT = 0x800B,
        // Comparisons
        NEVER = 0x0200,
        LESS = 0x0201,
        EQUAL = 0x0202,
        LEQUAL = 0x0203,
        GREATER = 0x0204,
        NOTEQUAL = 0x0205,
        GEQUAL = 0x0206,
        ALWAYS = 0x0207,
        // Stencil Operations
        INVERT = 0x150A,
        KEEP = 0x1E00,
        REPLACE = 0x1E01,
        INCR = 0x1E02,
        DECR = 0x1E03,
        INCR_WRAP = 0x8507,
        DECR_WRAP = 0x8508,
        // Wrap Modes
        REPEAT = 0x2901,
        CLAMP_TO_EDGE = 0x812F,
        MIRRORED_REPEAT = 0x8370,
        // Filters
        NEAREST = 0x2600,
        LINEAR = 0x2601,
        NEAREST_MIPMAP_NEAREST = 0x2700,
        NEAREST_MIPMAP_LINEAR = 0x2702,
        LINEAR_MIPMAP_NEAREST = 0x2701,
        LINEAR_MIPMAP_LINEAR = 0x2703,
        // Attachments
        COLOR_ATTACHMENT0 = 0x8CE0,
        DEPTH_ATTACHMENT = 0x8D00,
        STENCIL_ATTACHMENT = 0x8D20,
        DEPTH_STENCIL_ATTACHMENT = 0x821A,
        // Texture Formats
        RED = 0x1903,
        RGB = 0x1907,
        RGBA = 0x1908,
        LUMINANCE = 0x1909,
        RGB8 = 0x8051,
        RGBA8 = 0x8058,
        RGBA4 = 0x8056,
        RGB5_A1 = 0x8057,
        RGB10_A2_EXT = 0x8059,
        RGBA16 = 0x805B,
        BGRA = 0x80E1,
        DEPTH_COMPONENT16 = 0x81A5,
        DEPTH_COMPONENT24 = 0x81A6,
        RG = 0x8227,
        RG8 = 0x822B,
        RG16 = 0x822C,
        R16F = 0x822D,
        R32F = 0x822E,
        RG16F = 0x822F,
        RG32F = 0x8230,
        RGBA32F = 0x8814,
        RGBA16F = 0x881A,
        DEPTH24_STENCIL8 = 0x88F0,
        COMPRESSED_TEXTURE_FORMATS = 0x86A3,
        COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1,
        COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2,
        COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3,
        // Texture Internal Formats
        DEPTH_COMPONENT = 0x1902,
        DEPTH_STENCIL = 0x84F9,
        // Textures
        TEXTURE_WRAP_S = 0x2802,
        TEXTURE_WRAP_T = 0x2803,
        TEXTURE_WRAP_R = 0x8072,
        TEXTURE_MAG_FILTER = 0x2800,
        TEXTURE_MIN_FILTER = 0x2801,
        TEXTURE_MAX_ANISOTROPY_EXT = 0x84FE,
        TEXTURE_BASE_LEVEL = 0x813C,
        TEXTURE_MAX_LEVEL = 0x813D,
        TEXTURE_LOD_BIAS = 0x8501,
        UNPACK_ALIGNMENT = 0x0CF5,
        // Multitexture
        TEXTURE0 = 0x84C0,
        MAX_TEXTURE_IMAGE_UNITS = 0x8872,
        MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C,
        // Buffer objects
        ARRAY_BUFFER = 0x8892,
        ELEMENT_ARRAY_BUFFER = 0x8893,
        STREAM_DRAW = 0x88E0,
        STATIC_DRAW = 0x88E4,
        DYNAMIC_DRAW = 0x88E8,
        MAX_VERTEX_ATTRIBS = 0x8869,
        // Render targets
        FRAMEBUFFER = 0x8D40,
        READ_FRAMEBUFFER = 0x8CA8,
        DRAW_FRAMEBUFFER = 0x8CA9,
        RENDERBUFFER = 0x8D41,
        MAX_DRAW_BUFFERS = 0x8824,
        // Draw Primitives
        POINTS = 0x0000,
        LINES = 0x0001,
        LINE_STRIP = 0x0003,
        TRIANGLES = 0x0004,
        TRIANGLE_STRIP = 0x0005,
        // Query Objects
        QUERY_RESULT = 0x8866,
        QUERY_RESULT_AVAILABLE = 0x8867,
        SAMPLES_PASSED = 0x8914,
        // Multisampling
        MULTISAMPLE = 0x809D,
        MAX_SAMPLES = 0x8D57,
        SAMPLE_MASK = 0x8E51,
        // Shaders
        FRAGMENT_SHADER = 0x8B30,
        VERTEX_SHADER = 0x8B31,
        ACTIVE_UNIFORMS = 0x8B86,
        ACTIVE_ATTRIBUTES = 0x8B89,
        FLOAT_VEC2 = 0x8B50,
        FLOAT_VEC3 = 0x8B51,
        FLOAT_VEC4 = 0x8B52,
        SAMPLER_2D = 0x8B5E,
        FLOAT_MAT3x2 = 0x8B67,
        FLOAT_MAT4 = 0x8B5C,
        // 3.2 Core Profile
        NUM_EXTENSIONS = 0x821D,
        // Source Enum Values
        DEBUG_SOURCE_API = 0x8246,
        DEBUG_SOURCE_WINDOW_SYSTEM = 0x8247,
        DEBUG_SOURCE_SHADER_COMPILER = 0x8248,
        DEBUG_SOURCE_THIRD_PARTY = 0x8249,
        DEBUG_SOURCE_APPLICATION = 0x824A,
        DEBUG_SOURCE_OTHER = 0x824B,
        // Type Enum Values
        DEBUG_TYPE_ERROR = 0x824C,
        DEBUG_TYPE_PUSH_GROUP = 0x8269,
        DEBUG_TYPE_POP_GROUP = 0x826A,
        DEBUG_TYPE_MARKER = 0x8268,
        DEBUG_TYPE_DEPRECATED_BEHAVIOR = 0x824D,
        DEBUG_TYPE_UNDEFINED_BEHAVIOR = 0x824E,
        DEBUG_TYPE_PORTABILITY = 0x824F,
        DEBUG_TYPE_PERFORMANCE = 0x8250,
        DEBUG_TYPE_OTHER = 0x8251,
        // Severity Enum Values
        DEBUG_SEVERITY_HIGH = 0x9146,
        DEBUG_SEVERITY_MEDIUM = 0x9147,
        DEBUG_SEVERITY_LOW = 0x9148,
        DEBUG_SEVERITY_NOTIFICATION = 0x826B,
        // Debug
        DEBUG_OUTPUT = 0x92E0,
        DEBUG_OUTPUT_SYNCHRONOUS = 0x8242
    }
}