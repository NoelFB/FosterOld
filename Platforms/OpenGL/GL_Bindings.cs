using Foster.Framework;
using System;
using System.Runtime.InteropServices;

namespace Foster.OpenGL
{
    internal class GL_Bindings
    {
        private readonly ISystemOpenGL system;

        public GL_Bindings(ISystemOpenGL system)
        {
            this.system = system ?? throw new Exception("GL Module requires a System that implements ProcAddress");

            CreateDelegate(ref glGetString!, "glGetString");
            CreateDelegate(ref glDebugMessageCallback!, "glDebugMessageCallback");
            CreateDelegate(ref glFlush!, "glFlush");
            CreateDelegate(ref glEnable!, "glEnable");
            CreateDelegate(ref glDisable!, "glDisable");
            CreateDelegate(ref glClear!, "glClear");
            CreateDelegate(ref glClearColor!, "glClearColor");
            CreateDelegate(ref glClearDepth!, "glClearDepth");
            CreateDelegate(ref glClearStencil!, "glClearStencil");
            CreateDelegate(ref glDepthMask!, "glDepthMask");
            CreateDelegate(ref glDepthFunc!, "glDepthFunc");
            CreateDelegate(ref glViewport!, "glViewport");
            CreateDelegate(ref glCullFace!, "glCullFace");
            CreateDelegate(ref glScissor!, "glScissor");
            CreateDelegate(ref glBlendEquation!, "glBlendEquation");
            CreateDelegate(ref glBlendEquationSeparate!, "glBlendEquationSeparate");
            CreateDelegate(ref glBlendFunc!, "glBlendFunc");
            CreateDelegate(ref glBlendFuncSeparate!, "glBlendFuncSeparate");
            CreateDelegate(ref glBlendColor!, "glBlendColor");
            CreateDelegate(ref glColorMask!, "glColorMask");
            CreateDelegate(ref glGetIntegerv!, "glGetIntegerv");
            CreateDelegate(ref glGenTextures!, "glGenTextures");
            CreateDelegate(ref glGenRenderbuffers!, "glGenRenderbuffers");
            CreateDelegate(ref glGenFramebuffers!, "glGenFramebuffers");
            CreateDelegate(ref glActiveTexture!, "glActiveTexture");
            CreateDelegate(ref glBindTexture!, "glBindTexture");
            CreateDelegate(ref glBindRenderbuffer!, "glBindRenderbuffer");
            CreateDelegate(ref glBindFramebuffer!, "glBindFramebuffer");
            CreateDelegate(ref glTexImage2D!, "glTexImage2D");
            CreateDelegate(ref glFramebufferRenderbuffer!, "glFramebufferRenderbuffer");
            CreateDelegate(ref glFramebufferTexture2D!, "glFramebufferTexture2D");
            CreateDelegate(ref glTexParameteri!, "glTexParameteri");
            CreateDelegate(ref glRenderbufferStorage!, "glRenderbufferStorage");
            CreateDelegate(ref glGetTexImage!, "glGetTexImage");
            CreateDelegate(ref glDrawElements!, "glDrawElements");
            CreateDelegate(ref glDrawElementsInstanced!, "glDrawElementsInstanced");
            CreateDelegate(ref glDeleteTextures!, "glDeleteTextures");
            CreateDelegate(ref glDeleteRenderbuffers!, "glDeleteRenderbuffers");
            CreateDelegate(ref glDeleteFramebuffers!, "glDeleteFramebuffers");
            CreateDelegate(ref glGenVertexArrays!, "glGenVertexArrays");
            CreateDelegate(ref glBindVertexArray!, "glBindVertexArray");
            CreateDelegate(ref glGenBuffers!, "glGenBuffers");
            CreateDelegate(ref glBindBuffer!, "glBindBuffer");
            CreateDelegate(ref glBufferData!, "glBufferData");
            CreateDelegate(ref glBufferSubData!, "glBufferSubData");
            CreateDelegate(ref glDeleteBuffers!, "glDeleteBuffers");
            CreateDelegate(ref glDeleteVertexArrays!, "glDeleteVertexArrays");
            CreateDelegate(ref glEnableVertexAttribArray!, "glEnableVertexAttribArray");
            CreateDelegate(ref glDisableVertexAttribArray!, "glDisableVertexAttribArray");
            CreateDelegate(ref glVertexAttribPointer!, "glVertexAttribPointer");
            CreateDelegate(ref glVertexAttribDivisor!, "glVertexAttribDivisor");
            CreateDelegate(ref glCreateShader!, "glCreateShader");
            CreateDelegate(ref glAttachShader!, "glAttachShader");
            CreateDelegate(ref glDetachShader!, "glDetachShader");
            CreateDelegate(ref glDeleteShader!, "glDeleteShader");
            CreateDelegate(ref glShaderSource!, "glShaderSource");
            CreateDelegate(ref glCompileShader!, "glCompileShader");
            CreateDelegate(ref glGetShaderiv!, "glGetShaderiv");
            CreateDelegate(ref glGetShaderInfoLog!, "glGetShaderInfoLog");
            CreateDelegate(ref glCreateProgram!, "glCreateProgram");
            CreateDelegate(ref glDeleteProgram!, "glDeleteProgram");
            CreateDelegate(ref glLinkProgram!, "glLinkProgram");
            CreateDelegate(ref glGetProgramiv!, "glGetProgramiv");
            CreateDelegate(ref glGetProgramInfoLog!, "glGetProgramInfoLog");
            CreateDelegate(ref glGetActiveUniform!, "glGetActiveUniform");
            CreateDelegate(ref glGetActiveAttrib!, "glGetActiveAttrib");
            CreateDelegate(ref glUseProgram!, "glUseProgram");
            CreateDelegate(ref glGetUniformLocation!, "glGetUniformLocation");
            CreateDelegate(ref glGetAttribLocation!, "glGetAttribLocation");
            CreateDelegate(ref glUniform1f!, "glUniform1f");
            CreateDelegate(ref glUniform2f!, "glUniform2f");
            CreateDelegate(ref glUniform3f!, "glUniform3f");
            CreateDelegate(ref glUniform4f!, "glUniform4f");
            CreateDelegate(ref glUniform1fv!, "glUniform1fv");
            CreateDelegate(ref glUniform2fv!, "glUniform2fv");
            CreateDelegate(ref glUniform3fv!, "glUniform3fv");
            CreateDelegate(ref glUniform4fv!, "glUniform4fv");
            CreateDelegate(ref glUniform1i!, "glUniform1i");
            CreateDelegate(ref glUniform2i!, "glUniform2i");
            CreateDelegate(ref glUniform3i!, "glUniform3i");
            CreateDelegate(ref glUniform4i!, "glUniform4i");
            CreateDelegate(ref glUniform1iv!, "glUniform1iv");
            CreateDelegate(ref glUniform2iv!, "glUniform2iv");
            CreateDelegate(ref glUniform3iv!, "glUniform3iv");
            CreateDelegate(ref glUniform4iv!, "glUniform4iv");
            CreateDelegate(ref glUniform1ui!, "glUniform1ui");
            CreateDelegate(ref glUniform2ui!, "glUniform2ui");
            CreateDelegate(ref glUniform3ui!, "glUniform3ui");
            CreateDelegate(ref glUniform4ui!, "glUniform4ui");
            CreateDelegate(ref glUniform1uiv!, "glUniform1uiv");
            CreateDelegate(ref glUniform2uiv!, "glUniform2uiv");
            CreateDelegate(ref glUniform3uiv!, "glUniform3uiv");
            CreateDelegate(ref glUniform4uiv!, "glUniform4uiv");
            CreateDelegate(ref glUniformMatrix2fv!, "glUniformMatrix2fv");
            CreateDelegate(ref glUniformMatrix3fv!, "glUniformMatrix3fv");
            CreateDelegate(ref glUniformMatrix4fv!, "glUniformMatrix4fv");
            CreateDelegate(ref glUniformMatrix2x3fv!, "glUniformMatrix2x3fv");
            CreateDelegate(ref glUniformMatrix3x2fv!, "glUniformMatrix3x2fv");
            CreateDelegate(ref glUniformMatrix2x4fv!, "glUniformMatrix2x4fv");
            CreateDelegate(ref glUniformMatrix4x2fv!, "glUniformMatrix4x2fv");
            CreateDelegate(ref glUniformMatrix3x4fv!, "glUniformMatrix3x4fv");
            CreateDelegate(ref glUniformMatrix4x3fv!, "glUniformMatrix4x3fv");
        }

        private void CreateDelegate<T>(ref T def, string name) where T : class
        {
            var addr = system.GetGLProcAddress(name);
            if (addr != IntPtr.Zero && (Marshal.GetDelegateForFunctionPointer(addr, typeof(T)) is T del))
                def = del;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GetString(GLEnum name);
        public GetString glGetString;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DebugMessageCallback(IntPtr callback, IntPtr userdata);
        public DebugMessageCallback glDebugMessageCallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Flush();
        public Flush glFlush;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Enable(GLEnum mode);
        public Enable glEnable;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Disable(GLEnum mode);
        public Disable glDisable;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Clear(GLEnum mask);
        public Clear glClear;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ClearColor(float red, float green, float blue, float alpha);
        public ClearColor glClearColor;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ClearDepth(double depth);
        public ClearDepth glClearDepth;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ClearStencil(int stencil);
        public ClearStencil glClearStencil;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DepthMask(bool enabled);
        public DepthMask glDepthMask;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DepthFunc(GLEnum func);
        public DepthFunc glDepthFunc;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Viewport(int x, int y, int width, int height);
        public Viewport glViewport;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Scissor(int x, int y, int width, int height);
        public Scissor glScissor;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CullFace(GLEnum mode);
        public CullFace glCullFace;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BlendEquation(GLEnum eq);
        public BlendEquation glBlendEquation;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BlendEquationSeparate(GLEnum modeRGB, GLEnum modeAlpha);
        public BlendEquationSeparate glBlendEquationSeparate;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BlendFunc(GLEnum sfactor, GLEnum dfactor);
        public BlendFunc glBlendFunc;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BlendFuncSeparate(GLEnum srcRGB, GLEnum dstRGB, GLEnum srcAlpha, GLEnum dstAlpha);
        public BlendFuncSeparate glBlendFuncSeparate;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BlendColor(float red, float green, float blue, float alpha);
        public BlendColor glBlendColor;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ColorMask(bool red, bool green, bool blue, bool alpha);
        public ColorMask glColorMask;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetIntegerv(GLEnum name, out int data);
        public GetIntegerv glGetIntegerv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GenTextures(int n, IntPtr textures);
        public GenTextures glGenTextures;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GenRenderbuffers(int n, IntPtr textures);
        public GenRenderbuffers glGenRenderbuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GenFramebuffers(int n, IntPtr textures);
        public GenFramebuffers glGenFramebuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ActiveTexture(uint id);
        public ActiveTexture glActiveTexture;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BindTexture(GLEnum target, uint id);
        public BindTexture glBindTexture;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BindRenderbuffer(GLEnum target, uint id);
        public BindRenderbuffer glBindRenderbuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BindFramebuffer(GLEnum target, uint id);
        public BindFramebuffer glBindFramebuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void TexImage2D(GLEnum target, int level, GLEnum internalFormat, int width, int height, int border, GLEnum format, GLEnum type, IntPtr data);
        public TexImage2D glTexImage2D;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FramebufferRenderbuffer(GLEnum target​, GLEnum attachment​, GLEnum renderbuffertarget​, uint renderbuffer​);
        public FramebufferRenderbuffer glFramebufferRenderbuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level);
        public FramebufferTexture2D glFramebufferTexture2D;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void TexParameteri(GLEnum target, GLEnum name, int param);
        public TexParameteri glTexParameteri;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RenderbufferStorage(GLEnum target​, GLEnum internalformat​, int width​, int height​);
        public RenderbufferStorage glRenderbufferStorage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetTexImage(GLEnum target, int level, GLEnum format, GLEnum type, IntPtr data);
        public GetTexImage glGetTexImage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DrawElements(GLEnum mode, int count, GLEnum type, IntPtr indices);
        public DrawElements glDrawElements;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DrawElementsInstanced(GLEnum mode, int count, GLEnum type, IntPtr indices, int amount);
        public DrawElementsInstanced glDrawElementsInstanced;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DeleteTextures(int n, uint* textures);
        public DeleteTextures glDeleteTextures;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DeleteRenderbuffers(int n, uint* renderbuffers);
        public DeleteRenderbuffers glDeleteRenderbuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DeleteFramebuffers(int n, uint* textures);
        public DeleteFramebuffers glDeleteFramebuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GenVertexArrays(int n, uint* arrays);
        public GenVertexArrays glGenVertexArrays;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BindVertexArray(uint id);
        public BindVertexArray glBindVertexArray;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GenBuffers(int n, uint* arrays);
        public GenBuffers glGenBuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BindBuffer(GLEnum target, uint buffer);
        public BindBuffer glBindBuffer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BufferData(GLEnum target, IntPtr size, IntPtr data, GLEnum usage);
        public BufferData glBufferData;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void BufferSubData(GLEnum target, IntPtr offset, IntPtr size, IntPtr data);
        public BufferSubData glBufferSubData;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DeleteBuffers(int n, uint* buffers);
        public DeleteBuffers glDeleteBuffers;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void DeleteVertexArrays(int n, uint* arrays);
        public DeleteVertexArrays glDeleteVertexArrays;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void EnableVertexAttribArray(uint location);
        public EnableVertexAttribArray glEnableVertexAttribArray;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DisableVertexAttribArray(uint location);
        public DisableVertexAttribArray glDisableVertexAttribArray;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void VertexAttribPointer(uint index, int size, GLEnum type, bool normalized, int stride, IntPtr pointer);
        public VertexAttribPointer glVertexAttribPointer;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void VertexAttribDivisor(uint index, uint divisor);
        public VertexAttribDivisor glVertexAttribDivisor;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint CreateShader(GLEnum type);
        public CreateShader glCreateShader;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void AttachShader(uint program, uint shader);
        public AttachShader glAttachShader;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DetachShader(uint program, uint shader);
        public DetachShader glDetachShader;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DeleteShader(uint shader);
        public DeleteShader glDeleteShader;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ShaderSource(uint shader, int count, string[] source, int[] length);
        public ShaderSource glShaderSource;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CompileShader(uint shader);
        public CompileShader glCompileShader;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetShaderiv(uint shader, GLEnum pname, out int result);
        public GetShaderiv glGetShaderiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetShaderInfoLog(uint shader, int maxLength, out int length, IntPtr infoLog);
        public GetShaderInfoLog glGetShaderInfoLog;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint CreateProgram();
        public CreateProgram glCreateProgram;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void DeleteProgram(uint program);
        public DeleteProgram glDeleteProgram;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void LinkProgram(uint program);
        public LinkProgram glLinkProgram;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetProgramiv(uint program, GLEnum pname, out int result);
        public GetProgramiv glGetProgramiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void GetProgramInfoLog(uint program, int maxLength, out int length, IntPtr infoLog);
        public GetProgramInfoLog glGetProgramInfoLog;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GetActiveUniform(uint program, uint index, int bufSize, out int length, out int size, out GLEnum type, IntPtr name);
        public GetActiveUniform glGetActiveUniform;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate void GetActiveAttrib(uint program, uint index, int bufSize, out int length, out int size, out GLEnum type, IntPtr name);
        public GetActiveAttrib glGetActiveAttrib;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UseProgram(uint program);
        public UseProgram glUseProgram;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetUniformLocation(uint program, string name);
        public GetUniformLocation glGetUniformLocation;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GetAttribLocation(uint program, string name);
        public GetAttribLocation glGetAttribLocation;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1f(int location, float v0);
        public Uniform1f glUniform1f;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2f(int location, float v0, float v1);
        public Uniform2f glUniform2f;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3f(int location, float v0, float v1, float v2);
        public Uniform3f glUniform3f;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4f(int location, float v0, float v1, float v2, float v3);
        public Uniform4f glUniform4f;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1fv(int location, int count, IntPtr value);
        public Uniform1fv glUniform1fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2fv(int location, int count, IntPtr value);
        public Uniform2fv glUniform2fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3fv(int location, int count, IntPtr value);
        public Uniform3fv glUniform3fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4fv(int location, int count, IntPtr value);
        public Uniform4fv glUniform4fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1i(int location, int v0);
        public Uniform1i glUniform1i;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2i(int location, int v0, int v1);
        public Uniform2i glUniform2i;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3i(int location, int v0, int v1, int v2);
        public Uniform3i glUniform3i;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4i(int location, int v0, int v1, int v2, int v3);
        public Uniform4i glUniform4i;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1iv(int location, int count, IntPtr value);
        public Uniform1iv glUniform1iv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2iv(int location, int count, IntPtr value);
        public Uniform2iv glUniform2iv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3iv(int location, int count, IntPtr value);
        public Uniform3iv glUniform3iv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4iv(int location, int count, IntPtr value);
        public Uniform4iv glUniform4iv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1ui(int location, uint v0);
        public Uniform1ui glUniform1ui;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2ui(int location, uint v0, uint v1);
        public Uniform2ui glUniform2ui;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3ui(int location, uint v0, uint v1, uint v2);
        public Uniform3ui glUniform3ui;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        public Uniform4ui glUniform4ui;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform1uiv(int location, int count, IntPtr value);
        public Uniform1uiv glUniform1uiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform2uiv(int location, int count, IntPtr value);
        public Uniform2uiv glUniform2uiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform3uiv(int location, int count, IntPtr value);
        public Uniform3uiv glUniform3uiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Uniform4uiv(int location, int count, IntPtr value);
        public Uniform4uiv glUniform4uiv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix2fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix2fv glUniformMatrix2fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix3fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix3fv glUniformMatrix3fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix4fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix4fv glUniformMatrix4fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix2x3fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix2x3fv glUniformMatrix2x3fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix3x2fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix3x2fv glUniformMatrix3x2fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix2x4fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix2x4fv glUniformMatrix2x4fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix4x2fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix4x2fv glUniformMatrix4x2fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix3x4fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix3x4fv glUniformMatrix3x4fv;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void UniformMatrix4x3fv(int location, int count, bool transpose, IntPtr value);
        public UniformMatrix4x3fv glUniformMatrix4x3fv;

    }
}
