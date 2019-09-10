using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.GLFW
{
    internal static class GLFW
    {
        private const string DLL = "glfw3";

        [StructLayout(LayoutKind.Sequential)]
        public struct Window
        {
            public IntPtr Ptr;

            public static implicit operator Window(IntPtr ptr) => new Window() { Ptr = ptr };
            public static implicit operator IntPtr(Window window) => window.Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Monitor
        {
            public IntPtr Ptr;

            public static implicit operator Monitor(IntPtr ptr) => new Monitor() { Ptr = ptr };
            public static implicit operator IntPtr(Monitor monitor) => monitor.Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VidMode
        {
            public int Width;
            public int Height;
            public int RedBits;
            public int GreenBits;
            public int BlueBits;
            public int RefreshRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Image
        {
            public int Width;
            public int Height;
            public byte[] Pixels;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GamepadState
        {
            public char[] Buttons;
            public float[] Axes;
        }

        public enum WindowAttributes
        {
            Resizable   = 0x00020003,
            Decorated   = 0x00020005,
            AutoIconify = 0x00020006,
            Floating    = 0x00020007,
            FocusOnShow = 0x0002000C,
        }

        public enum WindowHints
        {
            Resizable,
            Visible,
            Decorated,
            Focused,
            AutoIconify,
            Floating,
            Maximized,
            CenterCursor,
            TransparentFramebuffer,
            FocusOnshow,
            ScaleToMonitor,
            DoubleBuffer,
            ContextVersionMajor = 0x00022002,
            ContextVersionMinor = 0x00022003,
            ContextVersionRevision = 0x00022004
        }

        public delegate void ErrorFunc(int id, string message);
        public delegate void MonitorFunc(Monitor monitor, int eventType);
        public delegate void WindowPosFunc(Window window, int x, int y);
        public delegate void WindowSizeFunc(Window window, int w, int h);
        public delegate void WindowCloseFunc(Window window);
        public delegate void WindowRefreshFunc(Window window);
        public delegate void WindowFocusFunc(Window window);
        public delegate void WindowIconifyFunc(Window window, int iconified);
        public delegate void WindowMaximizeFunc(Window window, int maximized);
        public delegate void WindowFramebufferSizeFunc(Window window, int width, int height);
        public delegate void WindowContentsScaleFunc(Window window, float xscale, float yscale);
        public delegate void KeyFunc(Window window, int key, int scancode, int action, int mods);
        public delegate void CharFunc(Window window, uint codepoint);
        public delegate void CharmodsFunc(Window window, uint codepoint, int mods);
        public delegate void DropFunc(Window window, int path_count, string[] paths);
        public delegate void MouseButtonFunc(Window window, int button, int action, int mods);
        public delegate void CursorPosFunc(Window window, double xpos, double ypos);
        public delegate void CursorEnterFunc(Window window, int entered);
        public delegate void ScrollFunc(Window window, double xoffset, double yoffset);
        public delegate void JoystickFunc(int jid, int eventType);

        [DllImport(DLL, EntryPoint = "glfwInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init();

        [DllImport(DLL, EntryPoint = "glfwTerminate", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Terminate();

        [DllImport(DLL, EntryPoint = "glfwInitHint", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitHint(int hint, int value);

        [DllImport(DLL, EntryPoint = "glfwGetVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetVersion(out int major, out int minor, out int rev);

        [DllImport(DLL, EntryPoint = "glfwGetVersionString", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetVersionString();

        [DllImport(DLL, EntryPoint = "glfwGetError", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetError([MarshalAs(UnmanagedType.LPStr)] out string description);

        [DllImport(DLL, EntryPoint = "glfwSetErrorCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern ErrorFunc SetErrorCallback(ErrorFunc callback);

        [DllImport(DLL, EntryPoint = "glfwGetMonitors", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr[] GetMonitors(out int count);

        [DllImport(DLL, EntryPoint = "glfwGetPrimaryMonitor", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetPrimaryMonitor();

        [DllImport(DLL, EntryPoint = "glfwGetMonitorPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMonitorPos(Monitor monitor, out int xpos, out int ypos);

        [DllImport(DLL, EntryPoint = "glfwGetMonitorWorkarea", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMonitorWorkarea(Monitor monitor, out int xpos, out int ypos, out int width, out int height);

        [DllImport(DLL, EntryPoint = "glfwGetMonitorPhysicalSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMonitorPhysicalSize(Monitor monitor, out int widthMM, out int heightMM);

        [DllImport(DLL, EntryPoint = "glfwGetMonitorContentScale", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMonitorContentScale(Monitor monitor, out float xscale, out float yscale);

        [DllImport(DLL, EntryPoint = "glfwGetMonitorName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetMonitorName(Monitor monitor);

        [DllImport(DLL, EntryPoint = "glfwSetMonitorUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMonitorUserPointer(Monitor monitor, IntPtr pointer);

        [DllImport(DLL, EntryPoint = "glfwGetMonitorUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetMonitorUserPointer(Monitor monitor);

        [DllImport(DLL, EntryPoint = "glfwSetMonitorCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern MonitorFunc SetMonitorCallback(MonitorFunc callback);

        [DllImport(DLL, EntryPoint = "glfwGetVideoModes", CallingConvention = CallingConvention.Cdecl)]
        public static extern VidMode[] GetVideoModes(Monitor monitor, out int count);

        [DllImport(DLL, EntryPoint = "glfwGetVideoMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern VidMode GetVideoMode(Monitor monitor);

        [DllImport(DLL, EntryPoint = "glfwSetGamma", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGamma(Monitor monitor, float gamma);

        [DllImport(DLL, EntryPoint = "glfwDefaultWindowHints", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DefaultWindowHints();

        [DllImport(DLL, EntryPoint = "glfwWindowHint", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WindowHint(WindowHints hint, bool value);

        [DllImport(DLL, EntryPoint = "glfwWindowHint", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WindowHint(WindowHints hint, int value);

        [DllImport(DLL, EntryPoint = "glfwWindowHintString", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WindowHintString(WindowHints hint, [MarshalAs(UnmanagedType.LPStr)] string value);

        [DllImport(DLL, EntryPoint = "glfwCreateWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern Window CreateWindow(int width, int height, [MarshalAs(UnmanagedType.LPStr)] string title, Monitor monitor, IntPtr share);

        [DllImport(DLL, EntryPoint = "glfwDestroyWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwWindowShouldClose", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool WindowShouldClose(Window window);

        [DllImport(DLL, EntryPoint = "glfwSetWindowShouldClose", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowShouldClose(Window window, bool value);

        [DllImport(DLL, EntryPoint = "glfwSetWindowTitle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowTitle(Window window, [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport(DLL, EntryPoint = "glfwSetWindowIcon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowIcon(Window window, int count, Image images);

        [DllImport(DLL, EntryPoint = "glfwGetWindowPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetWindowPos(Window window, out int xpos, out int ypos);

        [DllImport(DLL, EntryPoint = "glfwSetWindowPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowPos(Window window, int xpos, int ypos);

        [DllImport(DLL, EntryPoint = "glfwGetWindowSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetWindowSize(Window window, out int width, out int height);

        [DllImport(DLL, EntryPoint = "glfwSetWindowSizeLimits", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowSizeLimits(Window window, int minwidth, int minheight, int maxwidth, int maxheight);

        [DllImport(DLL, EntryPoint = "glfwSetWindowAspectRatio", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowAspectRatio(Window window, int numer, int denom);

        [DllImport(DLL, EntryPoint = "glfwSetWindowSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowSize(Window window, int width, int height);

        [DllImport(DLL, EntryPoint = "glfwGetFramebufferSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetFramebufferSize(Window window, out int width, out int height);

        [DllImport(DLL, EntryPoint = "glfwGetWindowFrameSize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetWindowFrameSize(Window window, out int left, out int top, out int right, out int bottom);

        [DllImport(DLL, EntryPoint = "glfwGetWindowContentScale", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetWindowContentScale(Window window, out float xscale, out float yscale);

        [DllImport(DLL, EntryPoint = "glfwGetWindowOpacity", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetWindowOpacity(Window window);

        [DllImport(DLL, EntryPoint = "glfwSetWindowOpacity", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowOpacity(Window window, float opacity);

        [DllImport(DLL, EntryPoint = "glfwIconifyWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IconifyWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwRestoreWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RestoreWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwMaximizeWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MaximizeWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwShowWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShowWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwHideWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HideWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwFocusWindow", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FocusWindow(Window window);

        [DllImport(DLL, EntryPoint = "glfwRequestWindowAttention", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestWindowAttention(Window window);

        [DllImport(DLL, EntryPoint = "glfwGetWindowMonitor", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetWindowMonitor(Window window);

        [DllImport(DLL, EntryPoint = "glfwSetWindowMonitor", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowMonitor(Window window, Monitor monitor, int xpos, int ypos, int width, int height, int refreshRate);

        [DllImport(DLL, EntryPoint = "glfwGetWindowAttrib", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetWindowAttrib(Window window, WindowAttributes attrib);

        [DllImport(DLL, EntryPoint = "glfwSetWindowAttrib", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowAttrib(Window window, WindowAttributes attrib, bool value);

        [DllImport(DLL, EntryPoint = "glfwSetWindowUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetWindowUserPointer(Window window, IntPtr pointer);

        [DllImport(DLL, EntryPoint = "glfwGetWindowUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetWindowUserPointer(Window window);

        [DllImport(DLL, EntryPoint = "glfwSetWindowPosCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowPosFunc SetWindowPosCallback(Window window, WindowPosFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowSizeCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowSizeFunc SetWindowSizeCallback(Window window, WindowSizeFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowCloseCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowCloseFunc SetWindowCloseCallback(Window window, WindowCloseFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowRefreshCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowRefreshFunc SetWindowRefreshCallback(Window window, WindowRefreshFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowFocusCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowFocusFunc SetWindowFocusCallback(Window window, WindowFocusFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowIconifyCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowIconifyFunc SetWindowIconifyCallback(Window window, WindowIconifyFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowMaximizeCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowMaximizeFunc SetWindowMaximizeCallback(Window window, WindowMaximizeFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetFramebufferSizeCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowFramebufferSizeFunc SetFramebufferSizeCallback(Window window, WindowFramebufferSizeFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetWindowContentScaleCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern WindowContentsScaleFunc SetWindowContentScaleCallback(Window window, WindowContentsScaleFunc callback);

        [DllImport(DLL, EntryPoint = "glfwPollEvents", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PollEvents();

        [DllImport(DLL, EntryPoint = "glfwWaitEvents", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WaitEvents();

        [DllImport(DLL, EntryPoint = "glfwWaitEventsTimeout", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WaitEventsTimeout(double timeout);

        [DllImport(DLL, EntryPoint = "glfwPostEmptyEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PostEmptyEvent();

        [DllImport(DLL, EntryPoint = "glfwGetInputMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetInputMode(Window window, int mode);

        [DllImport(DLL, EntryPoint = "glfwSetInputMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInputMode(Window window, int mode, int value);

        [DllImport(DLL, EntryPoint = "glfwRawMouseMotionSupported", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RawMouseMotionSupported();

        [DllImport(DLL, EntryPoint = "glfwGetKeyName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetKeyName(int key, int scancode);

        [DllImport(DLL, EntryPoint = "glfwGetKeyScancode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetKeyScancode(int key);

        [DllImport(DLL, EntryPoint = "glfwGetKey", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetKey(Window window, int key);

        [DllImport(DLL, EntryPoint = "glfwGetMouseButton", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMouseButton(Window window, int button);

        [DllImport(DLL, EntryPoint = "glfwGetCursorPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCursorPos(Window window, out double xpos, out double ypos);

        [DllImport(DLL, EntryPoint = "glfwSetCursorPos", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCursorPos(Window window, double xpos, double ypos);

        [DllImport(DLL, EntryPoint = "glfwCreateCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateCursor(Image image, int xhot, int yhot);

        [DllImport(DLL, EntryPoint = "glfwCreateStandardCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateStandardCursor(int shape);

        [DllImport(DLL, EntryPoint = "glfwDestroyCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyCursor(IntPtr cursor);

        [DllImport(DLL, EntryPoint = "glfwSetCursor", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCursor(Window window, IntPtr cursor);

        [DllImport(DLL, EntryPoint = "glfwSetKeyCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern KeyFunc SetKeyCallback(Window window, KeyFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetCharCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern CharFunc SetCharCallback(Window window, CharFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetCharModsCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern CharmodsFunc SetCharModsCallback(Window window, CharmodsFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetMouseButtonCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern MouseButtonFunc SetMouseButtonCallback(Window window, MouseButtonFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetCursorPosCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern CursorPosFunc SetCursorPosCallback(Window window, CursorPosFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetCursorEnterCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern CursorEnterFunc SetCursorEnterCallback(Window window, CursorEnterFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetScrollCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern ScrollFunc SetScrollCallback(Window window, ScrollFunc callback);

        [DllImport(DLL, EntryPoint = "glfwSetDropCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern DropFunc SetDropCallback(Window window, DropFunc callback);

        [DllImport(DLL, EntryPoint = "glfwJoystickPresent", CallingConvention = CallingConvention.Cdecl)]
        public static extern int JoystickPresent(int jid);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickAxes", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetJoystickAxes(int jid, out int count);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickButtons", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetJoystickButtons(int jid, out int count);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickHats", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetJoystickHats(int jid, out int count);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetJoystickName(int jid);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickGUID", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetJoystickGUID(int jid);

        [DllImport(DLL, EntryPoint = "glfwSetJoystickUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetJoystickUserPointer(int jid, IntPtr pointer);

        [DllImport(DLL, EntryPoint = "glfwGetJoystickUserPointer", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetJoystickUserPointer(int jid);

        [DllImport(DLL, EntryPoint = "glfwJoystickIsGamepad", CallingConvention = CallingConvention.Cdecl)]
        public static extern int JoystickIsGamepad(int jid);

        [DllImport(DLL, EntryPoint = "glfwSetJoystickCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern JoystickFunc SetJoystickCallback(JoystickFunc callback);

        [DllImport(DLL, EntryPoint = "glfwUpdateGamepadMappings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UpdateGamepadMappings([MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(DLL, EntryPoint = "glfwGetGamepadName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetGamepadName(int jid);

        [DllImport(DLL, EntryPoint = "glfwGetGamepadState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetGamepadState(int jid, GamepadState state);

        [DllImport(DLL, EntryPoint = "glfwSetClipboardString", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetClipboardString(Window window, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(DLL, EntryPoint = "glfwGetClipboardString", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetClipboardString(Window window);

        [DllImport(DLL, EntryPoint = "glfwGetTime", CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetTime();

        [DllImport(DLL, EntryPoint = "glfwSetTime", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTime(double time);

        [DllImport(DLL, EntryPoint = "glfwGetTimerValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt64 GetTimerValue();

        [DllImport(DLL, EntryPoint = "glfwGetTimerFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt64 GetTimerFrequency();

        [DllImport(DLL, EntryPoint = "glfwMakeContextCurrent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MakeContextCurrent(Window window);

        [DllImport(DLL, EntryPoint = "glfwGetCurrentContext", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetCurrentContext();

        [DllImport(DLL, EntryPoint = "glfwSwapBuffers", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SwapBuffers(Window window);

        [DllImport(DLL, EntryPoint = "glfwSwapInterval", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SwapInterval(int interval);

        [DllImport(DLL, EntryPoint = "glfwExtensionSupported", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ExtensionSupported([MarshalAs(UnmanagedType.LPStr)] string extension);

        [DllImport(DLL, EntryPoint = "glfwGetProcAddress", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procname);

        [DllImport(DLL, EntryPoint = "glfwVulkanSupported", CallingConvention = CallingConvention.Cdecl)]
        public static extern int VulkanSupported();

        [DllImport(DLL, EntryPoint = "glfwGetRequiredInstanceExtensions", CallingConvention = CallingConvention.Cdecl)]
        public static extern string[] GetRequiredInstanceExtensions(out UInt32 count);


    }
}
