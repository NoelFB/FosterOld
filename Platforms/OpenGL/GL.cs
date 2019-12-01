using Foster.Framework;
using System;
using System.Runtime.InteropServices;
using GLSizei = System.Int32;

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

        public static void Init()
        {
            AssignDelegate(ref DebugMessageCallback, "glDebugMessageCallback");
            AssignDelegate(ref Flush, "glFlush");
            AssignDelegate(ref Enable, "glEnable");
            AssignDelegate(ref Disable, "glDisable");
            AssignDelegate(ref Clear, "glClear");
            AssignDelegate(ref ClearColor, "glClearColor");
            AssignDelegate(ref ClearDepth, "glClearDepth");
            AssignDelegate(ref ClearStencil, "glClearStencil");
            AssignDelegate(ref DepthMask, "glDepthMask");
            AssignDelegate(ref Viewport, "glViewport");
            AssignDelegate(ref CullFace, "glCullFace");
            AssignDelegate(ref Scissor, "glScissor");
            AssignDelegate(ref BlendEquation, "glBlendEquation");
            AssignDelegate(ref BlendEquationSeparate, "glBlendEquationSeparate");
            AssignDelegate(ref BlendFunc, "glBlendFunc");
            AssignDelegate(ref GetIntegerv, "glGetIntegerv");
            AssignDelegate(ref GenTextures, "glGenTextures");
            AssignDelegate(ref GenRenderbuffers, "glGenRenderbuffers");
            AssignDelegate(ref GenFramebuffers, "glGenFramebuffers");
            AssignDelegate(ref ActiveTexture, "glActiveTexture");
            AssignDelegate(ref BindTexture, "glBindTexture");
            AssignDelegate(ref BindRenderbuffer, "glBindRenderbuffer");
            AssignDelegate(ref BindFramebuffer, "glBindFramebuffer");
            AssignDelegate(ref TexImage2D, "glTexImage2D");
            AssignDelegate(ref FramebufferRenderbuffer, "glFramebufferRenderbuffer");
            AssignDelegate(ref FramebufferTexture2D, "glFramebufferTexture2D");
            AssignDelegate(ref TexParameteri, "glTexParameteri");
            AssignDelegate(ref RenderbufferStorage, "glRenderbufferStorage");
            AssignDelegate(ref GetTexImage, "glGetTexImage");
            AssignDelegate(ref DrawElements, "glDrawElements");
            AssignDelegate(ref DrawElementsInstanced, "glDrawElementsInstanced");
            AssignDelegate(ref DeleteTextures, "glDeleteTextures");
            AssignDelegate(ref DeleteRenderbuffers, "glDeleteRenderbuffers");
            AssignDelegate(ref DeleteFramebuffers, "glDeleteFramebuffers");
            AssignDelegate(ref GenVertexArrays, "glGenVertexArrays");
            AssignDelegate(ref BindVertexArray, "glBindVertexArray");
            AssignDelegate(ref GenBuffers, "glGenBuffers");
            AssignDelegate(ref BindBuffer, "glBindBuffer");
            AssignDelegate(ref BufferData, "glBufferData");
            AssignDelegate(ref DeleteBuffers, "glDeleteBuffers");
            AssignDelegate(ref DeleteVertexArrays, "glDeleteVertexArrays");
            AssignDelegate(ref EnableVertexAttribArray, "glEnableVertexAttribArray");
            AssignDelegate(ref VertexAttribPointer, "glVertexAttribPointer");
            AssignDelegate(ref VertexAttribDivisor, "glVertexAttribDivisor");
            AssignDelegate(ref CreateShader, "glCreateShader");
            AssignDelegate(ref AttachShader, "glAttachShader");
            AssignDelegate(ref DetachShader, "glDetachShader");
            AssignDelegate(ref DeleteShader, "glDeleteShader");
            AssignDelegate(ref ShaderSource, "glShaderSource");
            AssignDelegate(ref CompileShader, "glCompileShader");
            AssignDelegate(ref GetShaderiv, "glGetShaderiv");
            AssignDelegate(ref getShaderInfoLog, "glGetShaderInfoLog");
            AssignDelegate(ref CreateProgram, "glCreateProgram");
            AssignDelegate(ref DeleteProgram, "glDeleteProgram");
            AssignDelegate(ref LinkProgram, "glLinkProgram");
            AssignDelegate(ref GetProgramiv, "glGetProgramiv");
            AssignDelegate(ref getProgramInfoLog, "glGetProgramInfoLog");
            AssignDelegate(ref getActiveUniform, "glGetActiveUniform");
            AssignDelegate(ref UseProgram, "glUseProgram");
            AssignDelegate(ref GetUniformLocation, "glGetUniformLocation");
            AssignDelegate(ref Uniform1f, "glUniform1f");
            AssignDelegate(ref Uniform2f, "glUniform2f");
            AssignDelegate(ref Uniform3f, "glUniform3f");
            AssignDelegate(ref Uniform4f, "glUniform4f");
            AssignDelegate(ref Uniform1fv, "glUniform1fv");
            AssignDelegate(ref Uniform2fv, "glUniform2fv");
            AssignDelegate(ref Uniform3fv, "glUniform3fv");
            AssignDelegate(ref Uniform4fv, "glUniform4fv");
            AssignDelegate(ref Uniform1i, "glUniform1i");
            AssignDelegate(ref Uniform2i, "glUniform2i");
            AssignDelegate(ref Uniform3i, "glUniform3i");
            AssignDelegate(ref Uniform4i, "glUniform4i");
            AssignDelegate(ref Uniform1iv, "glUniform1iv");
            AssignDelegate(ref Uniform2iv, "glUniform2iv");
            AssignDelegate(ref Uniform3iv, "glUniform3iv");
            AssignDelegate(ref Uniform4iv, "glUniform4iv");
            AssignDelegate(ref Uniform1ui, "glUniform1ui");
            AssignDelegate(ref Uniform2ui, "glUniform2ui");
            AssignDelegate(ref Uniform3ui, "glUniform3ui");
            AssignDelegate(ref Uniform4ui, "glUniform4ui");
            AssignDelegate(ref Uniform1uiv, "glUniform1uiv");
            AssignDelegate(ref Uniform2uiv, "glUniform2uiv");
            AssignDelegate(ref Uniform3uiv, "glUniform3uiv");
            AssignDelegate(ref Uniform4uiv, "glUniform4uiv");
            AssignDelegate(ref UniformMatrix2fv, "glUniformMatrix2fv");
            AssignDelegate(ref UniformMatrix3fv, "glUniformMatrix3fv");
            AssignDelegate(ref UniformMatrix4fv, "glUniformMatrix4fv");
            AssignDelegate(ref UniformMatrix2x3fv, "glUniformMatrix2x3fv");
            AssignDelegate(ref UniformMatrix3x2fv, "glUniformMatrix3x2fv");
            AssignDelegate(ref UniformMatrix2x4fv, "glUniformMatrix2x4fv");
            AssignDelegate(ref UniformMatrix4x2fv, "glUniformMatrix4x2fv");
            AssignDelegate(ref UniformMatrix3x4fv, "glUniformMatrix3x4fv");
            AssignDelegate(ref UniformMatrix4x3fv, "glUniformMatrix4x3fv");

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
            if (DebugMessageCallback != null)
            {
                Enable(GLEnum.DEBUG_OUTPUT);
                Enable(GLEnum.DEBUG_OUTPUT_SYNCHRONOUS);

                DebugMessageCallback(Marshal.GetFunctionPointerForDelegate(new OnError((source, type, id, severity, length, message, userParam) =>
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
                        default: case GLEnum.DEBUG_SEVERITY_NOTIFICATION: severityName = "NOTIFICATION"; break;
                    }

                    if (type == GLEnum.DEBUG_TYPE_ERROR)
                    {
                        throw new Exception(output);
                    }

                    Console.WriteLine($"GL {typeName} ({severityName})");
                    Console.WriteLine("\t" + output);
                    Console.WriteLine();

                })), IntPtr.Zero);
            }

#endif
        }

        private static void AssignDelegate<T>(ref T def, string name) where T : class
        {
            if (App.System == null)
            {
                throw new Exception("GL Module requires a System that implements ProcAddress");
            }

            IntPtr addr = App.System.GetProcAddress(name);
            if (addr != IntPtr.Zero && (Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) is T del))
                def = del;
        }

        private delegate void OnError(GLEnum source, GLEnum type, uint id, GLEnum severity, uint length, IntPtr message, IntPtr userParam);


#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public static GL_Delegates.DebugMessageCallback DebugMessageCallback;

        public static GL_Delegates.Flush Flush;
        public static GL_Delegates.Enable Enable;
        public static GL_Delegates.Disable Disable;
        public static GL_Delegates.Clear Clear;
        public static GL_Delegates.ClearColor ClearColor;
        public static GL_Delegates.ClearDepth ClearDepth;
        public static GL_Delegates.ClearStencil ClearStencil;
        public static GL_Delegates.DepthMask DepthMask;
        public static GL_Delegates.Viewport Viewport;
        public static GL_Delegates.Scissor Scissor;
        public static GL_Delegates.CullFace CullFace;
        public static GL_Delegates.BlendEquation BlendEquation;
        public static GL_Delegates.BlendEquationSeparate BlendEquationSeparate;
        public static GL_Delegates.BlendFunc BlendFunc;
        public static GL_Delegates.GetIntegerv GetIntegerv;

        public static GL_Delegates.GenTextures GenTextures;
        public static unsafe uint GenTexture()
        {
            uint id;
            GenTextures(1, new IntPtr(&id));
            return id;
        }

        public static GL_Delegates.GenTextures GenRenderbuffers;
        public static unsafe uint GenRenderbuffer()
        {
            uint id;
            GenRenderbuffers(1, new IntPtr(&id));
            return id;
        }

        public static GL_Delegates.GenFramebuffers GenFramebuffers;
        public static unsafe uint GenFramebuffer()
        {
            uint id;
            GenFramebuffers(1, new IntPtr(&id));
            return id;
        }

        public static GL_Delegates.ActiveTexture ActiveTexture;
        public static GL_Delegates.BindTexture BindTexture;
        public static GL_Delegates.BindRenderbuffer BindRenderbuffer;
        public static GL_Delegates.BindFramebuffer BindFramebuffer;
        public static GL_Delegates.TexImage2D TexImage2D;
        public static GL_Delegates.FramebufferRenderbuffer FramebufferRenderbuffer;
        public static GL_Delegates.FramebufferTexture2D FramebufferTexture2D;
        public static GL_Delegates.TexParameteri TexParameteri;
        public static GL_Delegates.RenderbufferStorage RenderbufferStorage;
        public static GL_Delegates.GetTexImage GetTexImage;
        public static GL_Delegates.DrawElements DrawElements;
        public static GL_Delegates.DrawElementsInstanced DrawElementsInstanced;

        public static GL_Delegates.DeleteTextures DeleteTextures;
        public static unsafe void DeleteTexture(uint id)
        {
            DeleteTextures(1, &id);
        }

        public static GL_Delegates.DeleteRenderbuffers DeleteRenderbuffers;
        public static unsafe void DeleteRenderbuffer(uint id)
        {
            DeleteRenderbuffers(1, &id);
        }

        public static GL_Delegates.DeleteFramebuffers DeleteFramebuffers;
        public static unsafe void DeleteFramebuffer(uint id)
        {
            DeleteFramebuffers(1, &id);
        }

        public static GL_Delegates.GenVertexArrays GenVertexArrays;
        public static unsafe uint GenVertexArray()
        {
            uint id;
            GenVertexArrays(1, &id);
            return id;
        }

        public static GL_Delegates.BindVertexArray BindVertexArray;
        public static GL_Delegates.GenBuffers GenBuffers;
        public static unsafe uint GenBuffer()
        {
            uint id;
            GenBuffers(1, &id);
            return id;
        }

        public static GL_Delegates.BindBuffer BindBuffer;
        public static GL_Delegates.BufferData BufferData;

        public static GL_Delegates.DeleteBuffers DeleteBuffers;
        public static unsafe void DeleteBuffer(uint id)
        {
            DeleteBuffers(1, &id);
        }

        public static GL_Delegates.DeleteVertexArrays DeleteVertexArrays;
        public static unsafe void DeleteVertexArray(uint id)
        {
            DeleteVertexArrays(1, &id);
        }

        public static GL_Delegates.EnableVertexAttribArray EnableVertexAttribArray;
        public static GL_Delegates.VertexAttribPointer VertexAttribPointer;
        public static GL_Delegates.VertexAttribDivisor VertexAttribDivisor;
        public static GL_Delegates.CreateShader CreateShader;
        public static GL_Delegates.AttachShader AttachShader;
        public static GL_Delegates.DetachShader DetachShader;
        public static GL_Delegates.DeleteShader DeleteShader;
        public static GL_Delegates.ShaderSource ShaderSource;
        public static GL_Delegates.CompileShader CompileShader;
        public static GL_Delegates.GetShaderiv GetShaderiv;

        private static GL_Delegates.GetShaderInfoLog getShaderInfoLog;
        public static unsafe string? GetShaderInfoLog(uint shader)
        {
            GetShaderiv(shader, (GLEnum)0x8B84, out int len);

            char* bytes = stackalloc char[len];
            IntPtr ptr = new IntPtr(bytes);

            getShaderInfoLog(shader, len, out len, ptr);

            if (len <= 0)
            {
                return null;
            }

            return Marshal.PtrToStringAnsi(ptr, len);
        }

        public static GL_Delegates.CreateProgram CreateProgram;
        public static GL_Delegates.DeleteProgram DeleteProgram;
        public static GL_Delegates.LinkProgram LinkProgram;
        public static GL_Delegates.GetProgramiv GetProgramiv;

        private static GL_Delegates.GetProgramInfoLog getProgramInfoLog;
        public static unsafe string? GetProgramInfoLog(uint program)
        {
            GetProgramiv(program, (GLEnum)0x8B84, out int len);

            char* bytes = stackalloc char[len];
            IntPtr ptr = new IntPtr(bytes);

            getProgramInfoLog(program, len, out len, ptr);

            if (len <= 0)
            {
                return null;
            }

            return Marshal.PtrToStringAnsi(ptr, len);
        }

        private static GL_Delegates.GetActiveUniform getActiveUniform;
        public static unsafe void GetActiveUniform(uint program, uint index, out int size, out GLEnum type, out string name)
        {
            char* uniformName = stackalloc char[64];
            IntPtr ptr = new IntPtr(uniformName);

            getActiveUniform(program, index, 64, out int length, out size, out type, ptr);

            name = Marshal.PtrToStringAnsi(ptr, length) ?? "";
        }

        public static GL_Delegates.UseProgram UseProgram;
        public static GL_Delegates.GetUniformLocation GetUniformLocation;
        public static GL_Delegates.Uniform1f Uniform1f;
        public static GL_Delegates.Uniform2f Uniform2f;
        public static GL_Delegates.Uniform3f Uniform3f;
        public static GL_Delegates.Uniform4f Uniform4f;
        public static GL_Delegates.Uniform1fv Uniform1fv;
        public static GL_Delegates.Uniform2fv Uniform2fv;
        public static GL_Delegates.Uniform3fv Uniform3fv;
        public static GL_Delegates.Uniform4fv Uniform4fv;
        public static GL_Delegates.Uniform1i Uniform1i;
        public static GL_Delegates.Uniform2i Uniform2i;
        public static GL_Delegates.Uniform3i Uniform3i;
        public static GL_Delegates.Uniform4i Uniform4i;
        public static GL_Delegates.Uniform1iv Uniform1iv;
        public static GL_Delegates.Uniform2iv Uniform2iv;
        public static GL_Delegates.Uniform3iv Uniform3iv;
        public static GL_Delegates.Uniform4iv Uniform4iv;
        public static GL_Delegates.Uniform1ui Uniform1ui;
        public static GL_Delegates.Uniform2ui Uniform2ui;
        public static GL_Delegates.Uniform3ui Uniform3ui;
        public static GL_Delegates.Uniform4ui Uniform4ui;
        public static GL_Delegates.Uniform1uiv Uniform1uiv;
        public static GL_Delegates.Uniform2uiv Uniform2uiv;
        public static GL_Delegates.Uniform3uiv Uniform3uiv;
        public static GL_Delegates.Uniform4uiv Uniform4uiv;
        public static GL_Delegates.UniformMatrix2fv UniformMatrix2fv;
        public static GL_Delegates.UniformMatrix3fv UniformMatrix3fv;
        public static GL_Delegates.UniformMatrix4fv UniformMatrix4fv;
        public static GL_Delegates.UniformMatrix2x3fv UniformMatrix2x3fv;
        public static GL_Delegates.UniformMatrix3x2fv UniformMatrix3x2fv;
        public static GL_Delegates.UniformMatrix2x4fv UniformMatrix2x4fv;
        public static GL_Delegates.UniformMatrix4x2fv UniformMatrix4x2fv;
        public static GL_Delegates.UniformMatrix3x4fv UniformMatrix3x4fv;
        public static GL_Delegates.UniformMatrix4x3fv UniformMatrix4x3fv;

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    }

    internal static class GL_Delegates
    {
        public delegate void DebugMessageCallback(IntPtr callback, IntPtr userdata);
        public delegate void Flush();
        public delegate void Enable(GLEnum mode);
        public delegate void Disable(GLEnum mode);
        public delegate void Clear(GLEnum mask);
        public delegate void ClearColor(float red, float green, float blue, float alpha);
        public delegate void ClearDepth(double depth);
        public delegate void ClearStencil(int stencil);
        public delegate void DepthMask(bool enabled);
        public delegate void Viewport(int x, int y, GLSizei width, GLSizei height);
        public delegate void Scissor(int x, int y, GLSizei width, GLSizei height);
        public delegate void CullFace(GLEnum mode);
        public delegate void BlendEquation(GLEnum eq);
        public delegate void BlendEquationSeparate(GLEnum modeRGB, GLEnum modeAlpha);
        public delegate void BlendFunc(GLEnum sfactor, GLEnum dfactor);
        public delegate void GetIntegerv(GLEnum name, out int data);
        public unsafe delegate void GenTextures(GLSizei n, IntPtr textures);
        public unsafe delegate void GenRenderbuffers(GLSizei n, IntPtr textures);
        public unsafe delegate void GenFramebuffers(GLSizei n, IntPtr textures);
        public delegate void ActiveTexture(uint id);
        public delegate void BindTexture(GLEnum target, uint id);
        public delegate void BindRenderbuffer(GLEnum target, uint id);
        public delegate void BindFramebuffer(GLEnum target, uint id);
        public delegate void TexImage2D(GLEnum target, int level, GLEnum internalFormat, GLSizei width, GLSizei height, int border, GLEnum format, GLEnum type, IntPtr data);
        public delegate void FramebufferRenderbuffer(GLEnum target​, GLEnum attachment​, GLEnum renderbuffertarget​, uint renderbuffer​);
        public delegate void FramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level);
        public delegate void TexParameteri(GLEnum target, GLEnum name, int param);
        public delegate void RenderbufferStorage(GLEnum target​, GLEnum internalformat​, GLSizei width​, GLSizei height​);
        public delegate void GetTexImage(GLEnum target, int level, GLEnum format, GLEnum type, IntPtr data);
        public unsafe delegate void DrawElements(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices);
        public unsafe delegate void DrawElementsInstanced(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices, int amount);
        public unsafe delegate void DeleteTextures(GLSizei n, uint* textures);
        public unsafe delegate void DeleteRenderbuffers(GLSizei n, uint* renderbuffers);
        public unsafe delegate void DeleteFramebuffers(GLSizei n, uint* textures);
        public unsafe delegate void GenVertexArrays(GLSizei n, uint* arrays);
        public delegate void BindVertexArray(uint id);
        public unsafe delegate void GenBuffers(GLSizei n, uint* arrays);
        public delegate void BindBuffer(GLEnum target, uint buffer);
        public delegate void BufferData(GLEnum target, IntPtr size, IntPtr data, GLEnum usage);
        public unsafe delegate void DeleteBuffers(GLSizei n, uint* buffers);
        public unsafe delegate void DeleteVertexArrays(GLSizei n, uint* arrays);
        public delegate void EnableVertexAttribArray(uint location);
        public delegate void VertexAttribPointer(uint index, int size, GLEnum type, bool normalized, GLSizei stride, IntPtr pointer);
        public delegate void VertexAttribDivisor(uint index, uint divisor);
        public delegate uint CreateShader(GLEnum type);
        public delegate void AttachShader(uint program, uint shader);
        public delegate void DetachShader(uint program, uint shader);
        public delegate void DeleteShader(uint shader);
        public delegate void ShaderSource(uint shader, GLSizei count, string[] source, int[] length);
        public delegate void CompileShader(uint id);
        public delegate void GetShaderiv(uint shader, GLEnum pname, out int result);
        public delegate void GetShaderInfoLog(uint shader, GLSizei maxLength, out GLSizei length, IntPtr infoLog);
        public delegate uint CreateProgram();
        public delegate void DeleteProgram(uint program);
        public delegate void LinkProgram(uint program);
        public delegate void GetProgramiv(uint program, GLEnum pname, out int result);
        public delegate void GetProgramInfoLog(uint program, GLSizei maxLength, out GLSizei length, IntPtr infoLog);
        public unsafe delegate void GetActiveUniform(uint program, uint index, GLSizei bufSize, out GLSizei length, out int size, out GLEnum type, IntPtr name);
        public delegate void UseProgram(uint program);
        public delegate int GetUniformLocation(uint program, string name);
        public delegate void Uniform1f(int location, float v0);
        public delegate void Uniform2f(int location, float v0, float v1);
        public delegate void Uniform3f(int location, float v0, float v1, float v2);
        public delegate void Uniform4f(int location, float v0, float v1, float v2, float v3);
        public delegate void Uniform1fv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform2fv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform3fv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform4fv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform1i(int location, int v0);
        public delegate void Uniform2i(int location, int v0, int v1);
        public delegate void Uniform3i(int location, int v0, int v1, int v2);
        public delegate void Uniform4i(int location, int v0, int v1, int v2, int v3);
        public delegate void Uniform1iv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform2iv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform3iv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform4iv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform1ui(int location, uint v0);
        public delegate void Uniform2ui(int location, uint v0, uint v1);
        public delegate void Uniform3ui(int location, uint v0, uint v1, uint v2);
        public delegate void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        public delegate void Uniform1uiv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform2uiv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform3uiv(int location, GLSizei count, IntPtr value);
        public delegate void Uniform4uiv(int location, GLSizei count, IntPtr value);
        public delegate void UniformMatrix2fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix3fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix4fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix2x3fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix3x2fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix2x4fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix4x2fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix3x4fv(int location, GLSizei count, bool transpose, IntPtr value);
        public delegate void UniformMatrix4x3fv(int location, GLSizei count, bool transpose, IntPtr value);
    }

    public enum GLEnum : int
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