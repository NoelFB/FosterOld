using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Foster.Framework;

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

        private static void AssignDelegate<T>(ref T def, string name) where T : class
        {
            if (App.System == null)
                throw new Exception("GL Module requires a System that implements ProcAddress");

            var addr = App.System.GetProcAddress(name);
            if (addr == IntPtr.Zero)
                throw new Exception($"OpenGL method '{name}' not available");

            def = Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) as T;
        }

        public static void Init()
        {
            AssignDelegate(ref glFlush, "glFlush");
            AssignDelegate(ref glEnable, "glEnable");
            AssignDelegate(ref glDisable, "glDisable");
            AssignDelegate(ref glClear, "glClear");
            AssignDelegate(ref glClearColor, "glClearColor");
            AssignDelegate(ref glViewport, "glViewport");
            AssignDelegate(ref glCullFace, "glCullFace");
            AssignDelegate(ref glScissor, "glScissor");
            AssignDelegate(ref glBlendEquation, "glBlendEquation");
            AssignDelegate(ref glBlendEquationSeparate, "glBlendEquationSeparate");
            AssignDelegate(ref glBlendFunc, "glBlendFunc");
            AssignDelegate(ref glGetIntegerv, "glGetIntegerv");
            AssignDelegate(ref glGenTextures, "glGenTextures");
            AssignDelegate(ref glGenRenderbuffers, "glGenRenderbuffers");
            AssignDelegate(ref glGenFramebuffers, "glGenFramebuffers");
            AssignDelegate(ref glActiveTexture, "glActiveTexture");
            AssignDelegate(ref glBindTexture, "glBindTexture");
            AssignDelegate(ref glBindRenderbuffer, "glBindRenderbuffer");
            AssignDelegate(ref glBindFramebuffer, "glBindFramebuffer");
            AssignDelegate(ref glTexImage2D, "glTexImage2D");
            AssignDelegate(ref glFramebufferRenderbuffer, "glFramebufferRenderbuffer");
            AssignDelegate(ref glFramebufferTexture2D, "glFramebufferTexture2D");
            AssignDelegate(ref glTexParameteri, "glTexParameteri");
            AssignDelegate(ref glRenderbufferStorage, "glRenderbufferStorage");
            AssignDelegate(ref glGetTexImage, "glGetTexImage");
            AssignDelegate(ref glDrawElements, "glDrawElements");
            AssignDelegate(ref glDrawElementsInstanced, "glDrawElementsInstanced");
            AssignDelegate(ref glDeleteTextures, "glDeleteTextures");
            AssignDelegate(ref glDeleteRenderbuffers, "glDeleteRenderbuffers");
            AssignDelegate(ref glDeleteFramebuffers, "glDeleteFramebuffers");
            AssignDelegate(ref glGenVertexArrays, "glGenVertexArrays");
            AssignDelegate(ref glBindVertexArray, "glBindVertexArray");
            AssignDelegate(ref glGenBuffers, "glGenBuffers");
            AssignDelegate(ref glBindBuffer, "glBindBuffer");
            AssignDelegate(ref glBufferData, "glBufferData");
            AssignDelegate(ref glDeleteBuffers, "glDeleteBuffers");
            AssignDelegate(ref glDeleteVertexArrays, "glDeleteVertexArrays");
            AssignDelegate(ref glEnableVertexAttribArray, "glEnableVertexAttribArray");
            AssignDelegate(ref glVertexAttribPointer, "glVertexAttribPointer");
            AssignDelegate(ref glVertexAttribDivisor, "glVertexAttribDivisor");
            AssignDelegate(ref glCreateShader, "glCreateShader");
            AssignDelegate(ref glAttachShader, "glAttachShader");
            AssignDelegate(ref glDetachShader, "glDetachShader");
            AssignDelegate(ref glDeleteShader, "glDeleteShader");
            AssignDelegate(ref glShaderSource, "glShaderSource");
            AssignDelegate(ref glCompileShader, "glCompileShader");
            AssignDelegate(ref glGetShaderiv, "glGetShaderiv");
            AssignDelegate(ref glGetShaderInfoLog, "glGetShaderInfoLog");
            AssignDelegate(ref glCreateProgram, "glCreateProgram");
            AssignDelegate(ref glDeleteProgram, "glDeleteProgram");
            AssignDelegate(ref glLinkProgram, "glLinkProgram");
            AssignDelegate(ref glGetProgramiv, "glGetProgramiv");
            AssignDelegate(ref glGetProgramInfoLog, "glGetProgramInfoLog");
            AssignDelegate(ref glGetActiveUniform, "glGetActiveUniform");
            AssignDelegate(ref glUseProgram, "glUseProgram");
            AssignDelegate(ref glGetUniformLocation, "glGetUniformLocation");
            AssignDelegate(ref glUniform1f, "glUniform1f");
            AssignDelegate(ref glUniform2f, "glUniform2f");
            AssignDelegate(ref glUniform3f, "glUniform3f");
            AssignDelegate(ref glUniform4f, "glUniform4f");
            AssignDelegate(ref glUniform1fv, "glUniform1fv");
            AssignDelegate(ref glUniform2fv, "glUniform2fv");
            AssignDelegate(ref glUniform3fv, "glUniform3fv");
            AssignDelegate(ref glUniform4fv, "glUniform4fv");
            AssignDelegate(ref glUniform1i, "glUniform1i");
            AssignDelegate(ref glUniform2i, "glUniform2i");
            AssignDelegate(ref glUniform3i, "glUniform3i");
            AssignDelegate(ref glUniform4i, "glUniform4i");
            AssignDelegate(ref glUniform1iv, "glUniform1iv");
            AssignDelegate(ref glUniform2iv, "glUniform2iv");
            AssignDelegate(ref glUniform3iv, "glUniform3iv");
            AssignDelegate(ref glUniform4iv, "glUniform4iv");
            AssignDelegate(ref glUniform1ui, "glUniform1ui");
            AssignDelegate(ref glUniform2ui, "glUniform2ui");
            AssignDelegate(ref glUniform3ui, "glUniform3ui");
            AssignDelegate(ref glUniform4ui, "glUniform4ui");
            AssignDelegate(ref glUniform1uiv, "glUniform1uiv");
            AssignDelegate(ref glUniform2uiv, "glUniform2uiv");
            AssignDelegate(ref glUniform3uiv, "glUniform3uiv");
            AssignDelegate(ref glUniform4uiv, "glUniform4uiv");
            AssignDelegate(ref glUniformMatrix2fv, "glUniformMatrix2fv");
            AssignDelegate(ref glUniformMatrix3fv, "glUniformMatrix3fv");
            AssignDelegate(ref glUniformMatrix4fv, "glUniformMatrix4fv");
            AssignDelegate(ref glUniformMatrix2x3fv, "glUniformMatrix2x3fv");
            AssignDelegate(ref glUniformMatrix3x2fv, "glUniformMatrix3x2fv");
            AssignDelegate(ref glUniformMatrix2x4fv, "glUniformMatrix2x4fv");
            AssignDelegate(ref glUniformMatrix4x2fv, "glUniformMatrix4x2fv");
            AssignDelegate(ref glUniformMatrix3x4fv, "glUniformMatrix3x4fv");
            AssignDelegate(ref glUniformMatrix4x3fv, "glUniformMatrix4x3fv");

            GetIntegerV((GLEnum)0x821B, out MajorVersion);
            GetIntegerV((GLEnum)0x821C, out MinorVersion);
            GetIntegerV((GLEnum)0x8CDF, out MaxColorAttachments);
            GetIntegerV((GLEnum)0x851C, out MaxCubeMapTextureSize);
            GetIntegerV((GLEnum)0x8824, out MaxDrawBuffers);
            GetIntegerV((GLEnum)0x80E9, out MaxElementIndices);
            GetIntegerV((GLEnum)0x80E8, out MaxElementVertices);
            GetIntegerV((GLEnum)0x84E8, out MaxRenderbufferSize);
            GetIntegerV((GLEnum)0x8D57, out MaxSamples);
            GetIntegerV((GLEnum)0x8872, out MaxTextureImageUnits);
            GetIntegerV((GLEnum)0x0D33, out MaxTextureSize);
        }


        delegate void _glFlush();
        static _glFlush glFlush;
        public static void Flush() { glFlush(); }

        delegate void _glEnable(GLEnum mode);
        static _glEnable glEnable;
        public static void Enable(GLEnum mode) { glEnable(mode); }

        delegate void _glDisable(GLEnum mode);
        static _glDisable glDisable;
        public static void Disable(GLEnum mode) { glDisable(mode); }

        delegate void _glClear(GLEnum mask);
        static _glClear glClear;
        public static void Clear(GLEnum mask) { glClear(mask); }

        delegate void _glClearColor(float red, float green, float blue, float alpha);
        static _glClearColor glClearColor;
        public static void ClearColor(float red, float green, float blue, float alpha) { glClearColor(red, green, blue, alpha); }

        delegate void _glViewport(int x, int y, GLSizei width, GLSizei height);
        static _glViewport glViewport;
        public static void Viewport(int x, int y, GLSizei width, GLSizei height) { glViewport(x, y, width, height); }

        delegate void _glScissor(int x, int y, GLSizei width, GLSizei height);
        static _glScissor glScissor;
        public static void Scissor(int x, int y, GLSizei width, GLSizei height) { glScissor(x, y, width, height); }

        delegate void _glCullFace(GLEnum mode);
        static _glCullFace glCullFace;
        public static void CullFace(GLEnum mode) { glCullFace(mode); }

        delegate void _glBlendEquation(GLEnum eq);
        static _glBlendEquation glBlendEquation;
        public static void BlendEquation(GLEnum eq) { glBlendEquation(eq); }

        delegate void _glBlendEquationSeparate(GLEnum modeRGB, GLEnum modeAlpha);
        static _glBlendEquationSeparate glBlendEquationSeparate;
        public static void BlendEquationSeparate(GLEnum modeRGB, GLEnum modeAlpha) { glBlendEquationSeparate(modeRGB, modeAlpha); }

        delegate void _glBlendFunc(GLEnum sfactor, GLEnum dfactor);
        static _glBlendFunc glBlendFunc;
        public static void BlendFunc(GLEnum sfactor, GLEnum dfactor) { glBlendFunc(sfactor, dfactor); }

        unsafe delegate void _glGetIntegerv(GLEnum name, int* data);
        static _glGetIntegerv glGetIntegerv;
        static unsafe void GetIntegerV(GLEnum name, out int val)
        {
            fixed (int* p = &val)
            {
                glGetIntegerv(name, p);
                val = *p;
            }
        }

        unsafe delegate void _glGenTextures(GLSizei n, uint* textures);
        static _glGenTextures glGenTextures;

        unsafe public static uint GenTexture()
        {
            uint texture = 0;
            glGenTextures(1, &texture);
            return texture;
        }

        unsafe delegate void _glGenRenderbuffers(GLSizei n, uint* textures);
        static _glGenTextures glGenRenderbuffers;

        unsafe public static uint GenRenderbuffer()
        {
            uint buffer = 0;
            glGenRenderbuffers(1, &buffer);
            return buffer;
        }

        unsafe delegate void _glGenFramebuffers(GLSizei n, uint* textures);
        static _glGenFramebuffers glGenFramebuffers;

        unsafe public static uint GenFramebuffer()
        {
            uint buffer = 0;
            glGenFramebuffers(1, &buffer);
            return buffer;
        }

        delegate void _glActiveTexture(uint id);
        static _glActiveTexture glActiveTexture;
        public static void ActiveTexture(uint id) { glActiveTexture(id); }

        delegate void _glBindTexture(GLEnum target, uint id);
        static _glBindTexture glBindTexture;
        public static void BindTexture(GLEnum target, uint id) { glBindTexture(target, id); }

        delegate void _glBindRenderbuffer(GLEnum target, uint id);
        static _glBindRenderbuffer glBindRenderbuffer;
        public static void BindRenderbuffer(GLEnum target, uint id) { glBindRenderbuffer(target, id); }

        delegate void _glBindFramebuffer(GLEnum target, uint id);
        static _glBindFramebuffer glBindFramebuffer;
        public static void BindFramebuffer(GLEnum target, uint id) { glBindFramebuffer(target, id); }

        delegate void _glTexImage2D(GLEnum target, int level, GLEnum internalFormat, GLSizei width, GLSizei height, int border, GLEnum format, GLEnum type, IntPtr data);
        static _glTexImage2D glTexImage2D;
        public static void TexImage2D(GLEnum target, int level, GLEnum internalFormat, GLSizei width, GLSizei height, int border, GLEnum format, GLEnum type, IntPtr data) { glTexImage2D(target, level, internalFormat, width, height, border, format, type, data); }

        delegate void _glFramebufferRenderbuffer(GLEnum target​, GLEnum attachment​, GLEnum renderbuffertarget​, uint renderbuffer​);
        static _glFramebufferRenderbuffer glFramebufferRenderbuffer;
        public static void FramebufferRenderbuffer(GLEnum target​, GLEnum attachment​, GLEnum renderbuffertarget​, uint renderbuffer​) { glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer); }

        delegate void _glFramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level);
        static _glFramebufferTexture2D glFramebufferTexture2D;
        public static void FramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level) { glFramebufferTexture2D(target, attachment, textarget, texture, level); }

        delegate void _glTexParameteri(GLEnum target, GLEnum name, int param);
        static _glTexParameteri glTexParameteri;
        public static void TexParameteri(GLEnum target, GLEnum name, int param) { glTexParameteri(target, name, param); }

        delegate void _glRenderbufferStorage(GLEnum target​, GLEnum internalformat​, GLSizei width​, GLSizei height​);
        static _glRenderbufferStorage glRenderbufferStorage;
        public static void RenderbufferStorage(GLEnum target​, GLEnum internalformat​, GLSizei width​, GLSizei height​) { glRenderbufferStorage(target, internalformat, width, height); }

        delegate void _glGetTexImage(GLEnum target, int level, GLEnum format, GLEnum type, IntPtr data);
        static _glGetTexImage glGetTexImage;
        public static void GetTexImage(GLEnum target, int level, GLEnum format, GLEnum type, IntPtr data) { glGetTexImage(target, level, format, type, data); }

        unsafe delegate void _glDrawElements(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices);
        static _glDrawElements glDrawElements;
        public static void DrawElements(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices) { glDrawElements(mode, count, type, indices); }

        unsafe delegate void _glDrawElementsInstanced(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices, int amount);
        static _glDrawElementsInstanced glDrawElementsInstanced;
        public static void DrawElementsInstanced(GLEnum mode, GLSizei count, GLEnum type, IntPtr indices, int amount) { glDrawElementsInstanced(mode, count, type, indices, amount); }

        unsafe delegate void _glDeleteTextures(GLSizei n, uint* textures);
        static _glDeleteTextures glDeleteTextures;
        unsafe public static void DeleteTexture(uint id) { glDeleteTextures(1, &id); }

        unsafe delegate void _glDeleteRenderbuffers(GLSizei n, uint* renderbuffers);
        static _glDeleteRenderbuffers glDeleteRenderbuffers;
        unsafe public static void DeleteRenderbuffer(uint id) { glDeleteRenderbuffers(1, &id); }

        unsafe delegate void _glDeleteFramebuffers(GLSizei n, uint* textures);
        static _glDeleteFramebuffers glDeleteFramebuffers;
        unsafe public static void DeleteFramebuffer(uint id) { glDeleteFramebuffers(1, &id); }

        unsafe delegate void _glGenVertexArrays(GLSizei n, uint* arrays);
        static _glGenVertexArrays glGenVertexArrays;
        unsafe public static uint GenVertexArray()
        {
            uint arr = 0;
            glGenVertexArrays(1, &arr);
            return arr;
        }

        delegate void _glBindVertexArray(uint id);
        static _glBindVertexArray glBindVertexArray;
        public static void BindVertexArray(uint id) { glBindVertexArray(id); }

        unsafe delegate void _glGenBuffers(GLSizei n, uint* arrays);
        static _glGenBuffers glGenBuffers;
        unsafe public static uint GenBuffer()
        {
            uint arr = 0;
            glGenBuffers(1, &arr);
            return arr;
        }

        delegate void _glBindBuffer(GLEnum target, uint buffer);
        static _glBindBuffer glBindBuffer;
        public static void BindBuffer(GLEnum target, uint buffer) { glBindBuffer(target, buffer); }

        delegate void _glBufferData(GLEnum target, IntPtr size, IntPtr data, GLEnum usage);
        static _glBufferData glBufferData;
        public static void BufferData<T>(GLEnum target, int size, T[] data, GLEnum usage) where T : struct
        {
            var pinned = GCHandle.Alloc(data, GCHandleType.Pinned);
            glBufferData(target, new IntPtr(size), pinned.AddrOfPinnedObject(), usage);
            pinned.Free();
        }

        unsafe delegate void _glDeleteBuffers(GLSizei n, uint* buffers);
        static _glDeleteBuffers glDeleteBuffers;
        unsafe public static void DeleteBuffer(uint buffer) { glDeleteBuffers(1, &buffer); }

        unsafe delegate void _glDeleteVertexArrays(GLSizei n, uint* arrays);
        static _glDeleteVertexArrays glDeleteVertexArrays;
        unsafe public static void DeleteVertexArray(uint id) { glDeleteVertexArrays(1, &id); }

        delegate void _glEnableVertexAttribArray(uint location);
        static _glEnableVertexAttribArray glEnableVertexAttribArray;
        public static void EnableVertexAttribArray(uint location) { glEnableVertexAttribArray(location); }

        delegate void _glVertexAttribPointer(uint index, int size, GLEnum type, bool normalized, GLSizei stride, IntPtr pointer);
        static _glVertexAttribPointer glVertexAttribPointer;
        public static void VertexAttribPointer(uint index, int size, GLEnum type, bool normalized, GLSizei stride, IntPtr pointer) { glVertexAttribPointer(index, size, type, normalized, stride, pointer); }

        delegate void _glVertexAttribDivisor(uint index, uint divisor);
        static _glVertexAttribDivisor glVertexAttribDivisor;
        public static void VertexAttribDivisor(uint index, uint divisor) { glVertexAttribDivisor(index, divisor); }


        delegate uint _glCreateShader(GLEnum type);
        static _glCreateShader glCreateShader;
        public static uint CreateShader(GLEnum type) { return glCreateShader(type); }

        delegate void _glAttachShader(uint program, uint shader);
        static _glAttachShader glAttachShader;
        public static void AttachShader(uint program, uint shader) { glAttachShader(program, shader); }

        delegate void _glDetachShader(uint program, uint shader);
        static _glDetachShader glDetachShader;
        public static void DetachShader(uint program, uint shader) { glDetachShader(program, shader); }

        delegate void _glDeleteShader(uint shader);
        static _glDeleteShader glDeleteShader;
        public static void DeleteShader(uint shader) { glDeleteShader(shader); }

        delegate void _glShaderSource(uint shader, GLSizei count, string[] source, int[] length);
        static _glShaderSource glShaderSource;
        public static void ShaderSource(uint shader, string source)
        {
            var sourceArr = new string[] { source };
            var lengthArr = new int[] { source.Length };
            glShaderSource(shader, 1, sourceArr, lengthArr);
        }

        delegate void _glCompileShader(uint id);
        static _glCompileShader glCompileShader;
        public static void CompileShader(uint id) { glCompileShader(id); }

        delegate void _glGetShaderiv(uint shader, GLEnum pname, out int result);
        static _glGetShaderiv glGetShaderiv;
        public static void GetShader(uint shader, GLEnum pname, out int result) { glGetShaderiv(shader, pname, out result); }

        delegate void _glGetShaderInfoLog(uint shader, GLSizei maxLength, out GLSizei length, IntPtr infoLog);
        static _glGetShaderInfoLog glGetShaderInfoLog;
        public static unsafe string GetShaderInfoLog(uint shader)
        {
            GetShader(shader, (GLEnum)0x8B84, out int len);

            var bytes = stackalloc char[len];
            var ptr = new IntPtr(bytes);

            glGetShaderInfoLog(shader, len, out len, ptr);

            return new string(bytes, 0, len);
        }

        delegate uint _glCreateProgram();
        static _glCreateProgram glCreateProgram;
        public static uint CreateProgram() { return glCreateProgram(); }

        delegate void _glDeleteProgram(uint program);
        static _glDeleteProgram glDeleteProgram;
        public static void DeleteProgram(uint program) { glDeleteProgram(program); }

        delegate void _glLinkProgram(uint program);
        static _glLinkProgram glLinkProgram;
        public static void LinkProgram(uint program) { glLinkProgram(program); }

        delegate void _glGetProgramiv(uint program, GLEnum pname, out int result);
        static _glGetProgramiv glGetProgramiv;
        public static void GetProgram(uint program, GLEnum pname, out int result) { glGetProgramiv(program, pname, out result); }

        delegate void _glGetProgramInfoLog(uint program, GLSizei maxLength, out GLSizei length, IntPtr infoLog);
        static _glGetProgramInfoLog glGetProgramInfoLog;
        public static unsafe string GetProgramInfoLog(uint program)
        {
            GetProgram(program, (GLEnum)0x8B84, out int len);

            var bytes = stackalloc char[len];
            var ptr = new IntPtr(bytes);

            glGetProgramInfoLog(program, len, out len, ptr);

            return new string(bytes, 0, len);
        }

        unsafe delegate void _glGetActiveUniform(uint program, uint index, GLSizei bufSize, out GLSizei length, out int size, out GLEnum type, IntPtr name);
        static _glGetActiveUniform glGetActiveUniform;
        public static unsafe void GetActiveUniform(uint program, uint index, out int size, out GLEnum type, out string name)
        {
            var uniformName = stackalloc char[64];
            var ptr = new IntPtr(uniformName);

            glGetActiveUniform(program, index, 64, out int length, out size, out type, ptr);

            name = null;
            if (length > 0)
                name = new string(uniformName, 0, length);
        }

        delegate void _glUseProgram(uint program);
        static _glUseProgram glUseProgram;
        public static void UseProgram(uint program) { glUseProgram(program); }

        delegate int _glGetUniformLocation(uint program, string name);
        static _glGetUniformLocation glGetUniformLocation;
        public static int GetUniformLocation(uint program, string name) { return glGetUniformLocation(program, name); }

        delegate void _glUniform1f(int location, float v0);
        static _glUniform1f glUniform1f;
        public static void Uniform1f(int location, float v0) { glUniform1f(location, v0); }

        delegate void _glUniform2f(int location, float v0, float v1);
        static _glUniform2f glUniform2f;
        public static void Uniform2f(int location, float v0, float v1) { glUniform2f(location, v0, v1); }

        delegate void _glUniform3f(int location, float v0, float v1, float v2);
        static _glUniform3f glUniform3f;
        public static void Uniform3f(int location, float v0, float v1, float v2) { glUniform3f(location, v0, v1, v2); }

        delegate void _glUniform4f(int location, float v0, float v1, float v2, float v3);
        static _glUniform4f glUniform4f;
        public static void Uniform4f(int location, float v0, float v1, float v2, float v3) { glUniform4f(location, v0, v1, v2, v3); }

        delegate void _glUniform1fv(int location, GLSizei count, float[] value);
        static _glUniform1fv glUniform1fv;
        public static void Uniform1fv(int location, GLSizei count, float[] value) { glUniform1fv(location, count, value); }

        delegate void _glUniform2fv(int location, GLSizei count, float[] value);
        static _glUniform2fv glUniform2fv;
        public static void Uniform2fv(int location, GLSizei count, float[] value) { glUniform2fv(location, count, value); }

        delegate void _glUniform3fv(int location, GLSizei count, float[] value);
        static _glUniform3fv glUniform3fv;
        public static void Uniform3fv(int location, GLSizei count, float[] value) { glUniform3fv(location, count, value); }

        delegate void _glUniform4fv(int location, GLSizei count, float[] value);
        static _glUniform4fv glUniform4fv;
        public static void Uniform4fv(int location, GLSizei count, float[] value) { glUniform4fv(location, count, value); }

        delegate void _glUniform1i(int location, int v0);
        static _glUniform1i glUniform1i;
        public static void Uniform1i(int location, int v0) { glUniform1i(location, v0); }

        delegate void _glUniform2i(int location, int v0, int v1);
        static _glUniform2i glUniform2i;
        public static void Uniform2i(int location, int v0, int v1) { glUniform2i(location, v0, v1); }

        delegate void _glUniform3i(int location, int v0, int v1, int v2);
        static _glUniform3i glUniform3i;
        public static void Uniform3i(int location, int v0, int v1, int v2) { glUniform3i(location, v0, v1, v2); }

        delegate void _glUniform4i(int location, int v0, int v1, int v2, int v3);
        static _glUniform4i glUniform4i;
        public static void Uniform4i(int location, int v0, int v1, int v2, int v3) { glUniform4i(location, v0, v1, v2, v3); }

        delegate void _glUniform1iv(int location, GLSizei count, int[] value);
        static _glUniform1iv glUniform1iv;
        public static void Uniform1iv(int location, GLSizei count, int[] value) { glUniform1iv(location, count, value); }

        delegate void _glUniform2iv(int location, GLSizei count, int[] value);
        static _glUniform2iv glUniform2iv;
        public static void Uniform2iv(int location, GLSizei count, int[] value) { glUniform2iv(location, count, value); }

        delegate void _glUniform3iv(int location, GLSizei count, int[] value);
        static _glUniform3iv glUniform3iv;
        public static void Uniform3iv(int location, GLSizei count, int[] value) { glUniform3iv(location, count, value); }

        delegate void _glUniform4iv(int location, GLSizei count, int[] value);
        static _glUniform4iv glUniform4iv;
        public static void Uniform4iv(int location, GLSizei count, int[] value) { glUniform4iv(location, count, value); }

        delegate void _glUniform1ui(int location, uint v0);
        static _glUniform1ui glUniform1ui;
        public static void Uniform1ui(int location, uint v0) { glUniform1ui(location, v0); }

        delegate void _glUniform2ui(int location, uint v0, uint v1);
        static _glUniform2ui glUniform2ui;
        public static void Uniform2ui(int location, uint v0, uint v1) { glUniform2ui(location, v0, v1); }

        delegate void _glUniform3ui(int location, uint v0, uint v1, uint v2);
        static _glUniform3ui glUniform3ui;
        public static void Uniform3ui(int location, uint v0, uint v1, uint v2) { glUniform3ui(location, v0, v1, v2); }

        delegate void _glUniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        static _glUniform4ui glUniform4ui;
        public static void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3) { glUniform4ui(location, v0, v1, v2, v3); }

        delegate void _glUniform1uiv(int location, GLSizei count, uint[] value);
        static _glUniform1uiv glUniform1uiv;
        public static void Uniform1uiv(int location, GLSizei count, uint[] value) { glUniform1uiv(location, count, value); }

        delegate void _glUniform2uiv(int location, GLSizei count, uint[] value);
        static _glUniform2uiv glUniform2uiv;
        public static void Uniform2uiv(int location, GLSizei count, uint[] value) { glUniform2uiv(location, count, value); }

        delegate void _glUniform3uiv(int location, GLSizei count, uint[] value);
        static _glUniform3uiv glUniform3uiv;
        public static void Uniform3uiv(int location, GLSizei count, uint[] value) { glUniform3uiv(location, count, value); }

        delegate void _glUniform4uiv(int location, GLSizei count, uint[] value);
        static _glUniform4uiv glUniform4uiv;
        public static void Uniform4uiv(int location, GLSizei count, uint[] value) { glUniform4uiv(location, count, value); }

        delegate void _glUniformMatrix2fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2fv glUniformMatrix2fv;
        public static void UniformMatrix2fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix2fv(location, count, transpose, value); }

        delegate void _glUniformMatrix3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix3fv glUniformMatrix3fv;
        public static void UniformMatrix3fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix3fv(location, count, transpose, value); }

        unsafe delegate void _glUniformMatrix4fv(int location, GLSizei count, bool transpose, float* value);
        static _glUniformMatrix4fv glUniformMatrix4fv;
        public static unsafe void UniformMatrix4fv(int location, GLSizei count, bool transpose, float[] value)
        {
            fixed (float* ptr = value) { glUniformMatrix4fv(location, count, transpose, ptr); }
        }

        delegate void _glUniformMatrix2x3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2x3fv glUniformMatrix2x3fv;
        public static void UniformMatrix2x3fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix2x3fv(location, count, transpose, value); }

        unsafe delegate void _glUniformMatrix3x2fv(int location, GLSizei count, bool transpose, float* value);
        static _glUniformMatrix3x2fv glUniformMatrix3x2fv;
        public static unsafe void UniformMatrix3x2fv(int location, GLSizei count, bool transpose, float[] value)
        {
            fixed (float* ptr = value) { glUniformMatrix3x2fv(location, count, transpose, ptr); }
        }

        delegate void _glUniformMatrix2x4fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2x4fv glUniformMatrix2x4fv;
        public static void UniformMatrix2x4fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix2x4fv(location, count, transpose, value); }

        delegate void _glUniformMatrix4x2fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix4x2fv glUniformMatrix4x2fv;
        public static void UniformMatrix4x2fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix4x2fv(location, count, transpose, value); }

        delegate void _glUniformMatrix3x4fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix3x4fv glUniformMatrix3x4fv;
        public static void UniformMatrix3x4fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix3x4fv(location, count, transpose, value); }

        delegate void _glUniformMatrix4x3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix4x3fv glUniformMatrix4x3fv;
        public static void UniformMatrix4x3fv(int location, GLSizei count, bool transpose, float[] value) { glUniformMatrix4x3fv(location, count, transpose, value); }

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
        DEBUG_TYPE_DEPRECATED_BEHAVIOR = 0x824D,
        DEBUG_TYPE_UNDEFINED_BEHAVIOR = 0x824E,
        DEBUG_TYPE_PORTABILITY = 0x824F,
        DEBUG_TYPE_PERFORMANCE = 0x8250,
        DEBUG_TYPE_OTHER = 0x8251,
        // Severity Enum Values
        DEBUG_SEVERITY_HIGH = 0x9146,
        DEBUG_SEVERITY_MEDIUM = 0x9147,
        DEBUG_SEVERITY_LOW = 0x9148,
        DEBUG_SEVERITY_NOTIFICATION = 0x826B
    }
}