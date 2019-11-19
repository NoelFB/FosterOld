using System;
using System.Runtime.InteropServices;

namespace Foster.GLFW
{
    internal static class GLFW
    {
        private const string DLL = "glfw3";

        [StructLayout(LayoutKind.Sequential)]
        public struct Window
        {
            public IntPtr Ptr;

            public static implicit operator Window(IntPtr ptr)
            {
                return new Window() { Ptr = ptr };
            }

            public static implicit operator IntPtr(Window window)
            {
                return window.Ptr;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Monitor
        {
            public IntPtr Ptr;

            public static implicit operator Monitor(IntPtr ptr)
            {
                return new Monitor() { Ptr = ptr };
            }

            public static implicit operator IntPtr(Monitor monitor)
            {
                return monitor.Ptr;
            }
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)GLFW_Enum.GAMEPAD_BUTTON_LAST + 1)]
            public char[] Buttons;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)GLFW_Enum.GAMEPAD_AXIS_LAST + 1)]
            public float[] Axes;
        }

        public enum WindowAttributes
        {
            Resizable = 0x00020003,
            Decorated = 0x00020005,
            AutoIconify = 0x00020006,
            Floating = 0x00020007,
            FocusOnShow = 0x0002000C,
        }

        public enum WindowHints
        {
            Focused = 0x00020001,
            Resizable = 0x00020003,
            Visible = 0x00020004,
            Decorated = 0x00020005,
            AutoIconify = 0x00020006,
            Floating = 0x00020007,
            Maximized = 0x00020008,
            CenterCursor = 0x00020009,
            TransparentFramebuffer = 0x0002000A,
            FocusOnshow = 0x0002000C,
            ScaleToMonitor = 0x0002200C,
            DoubleBuffer = 0x00021010,
            ContextVersionMajor = 0x00022002,
            ContextVersionMinor = 0x00022003,
            ContextVersionRevision = 0x00022004,

            OpenGLVersionMajor = 0x00022002,
            OpenGLVersionMinor = 0x00022003,
            OpenGLProfile = 0x00022008,
            OpenGLForwardCompat = 0x00022006
        }

        public delegate void ErrorFunc(int id, string message);
        public delegate void MonitorFunc(Monitor monitor, GLFW_Enum eventType);
        public delegate void WindowPosFunc(Window window, int x, int y);
        public delegate void WindowSizeFunc(Window window, int w, int h);
        public delegate void WindowCloseFunc(Window window);
        public delegate void WindowRefreshFunc(Window window);
        public delegate void WindowFocusFunc(Window window, int focused);
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
        public delegate void JoystickFunc(int jid, GLFW_Enum eventType);

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
        public static unsafe extern IntPtr* GetMonitors(out int count);

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
        public static extern WindowCloseFunc? SetWindowCloseCallback(Window window, WindowCloseFunc? callback);

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
        public static extern int GetGamepadState(int jid, ref GamepadState state);

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
        public static extern ulong GetTimerValue();

        [DllImport(DLL, EntryPoint = "glfwGetTimerFrequency", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetTimerFrequency();

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
        public static extern string[] GetRequiredInstanceExtensions(out uint count);

    }

    internal enum GLFW_Enum
    {
        TRUE = 1,
        FALSE = 0,
        RELEASE = 0,

        PRESS = 1,
        REPEAT = 2,

        HAT_CENTERED = 0,
        HAT_UP = 1,
        HAT_RIGHT = 2,
        HAT_DOWN = 4,
        HAT_LEFT = 8,
        HAT_RIGHT_UP = (HAT_RIGHT | HAT_UP),
        HAT_RIGHT_DOWN = (HAT_RIGHT | HAT_DOWN),
        HAT_LEFT_UP = (HAT_LEFT | HAT_UP),
        HAT_LEFT_DOWN = (HAT_LEFT | HAT_DOWN),

        /* The unknown key */
        KEY_UNKNOWN = -1,

        /* Printable keys */
        KEY_SPACE = 32,
        KEY_APOSTROPHE = 39,  /* ' */
        KEY_COMMA = 44,  /* , */
        KEY_MINUS = 45,  /* - */
        KEY_PERIOD = 46,  /* . */
        KEY_SLASH = 47,  /* / */
        KEY_0 = 48,
        KEY_1 = 49,
        KEY_2 = 50,
        KEY_3 = 51,
        KEY_4 = 52,
        KEY_5 = 53,
        KEY_6 = 54,
        KEY_7 = 55,
        KEY_8 = 56,
        KEY_9 = 57,
        KEY_SEMICOLON = 59,  /* ; */
        KEY_EQUAL = 61,  /* = */
        KEY_A = 65,
        KEY_B = 66,
        KEY_C = 67,
        KEY_D = 68,
        KEY_E = 69,
        KEY_F = 70,
        KEY_G = 71,
        KEY_H = 72,
        KEY_I = 73,
        KEY_J = 74,
        KEY_K = 75,
        KEY_L = 76,
        KEY_M = 77,
        KEY_N = 78,
        KEY_O = 79,
        KEY_P = 80,
        KEY_Q = 81,
        KEY_R = 82,
        KEY_S = 83,
        KEY_T = 84,
        KEY_U = 85,
        KEY_V = 86,
        KEY_W = 87,
        KEY_X = 88,
        KEY_Y = 89,
        KEY_Z = 90,
        KEY_LEFT_BRACKET = 91,  /* [ */
        KEY_BACKSLASH = 92,  /* \ */
        KEY_RIGHT_BRACKET = 93,  /* ] */
        KEY_GRAVE_ACCENT = 96,  /* ` */
        KEY_WORLD_1 = 161, /* non-US #1 */
        KEY_WORLD_2 = 162, /* non-US #2 */

        /* Function keys */
        KEY_ESCAPE = 256,
        KEY_ENTER = 257,
        KEY_TAB = 258,
        KEY_BACKSPACE = 259,
        KEY_INSERT = 260,
        KEY_DELETE = 261,
        KEY_RIGHT = 262,
        KEY_LEFT = 263,
        KEY_DOWN = 264,
        KEY_UP = 265,
        KEY_PAGE_UP = 266,
        KEY_PAGE_DOWN = 267,
        KEY_HOME = 268,
        KEY_END = 269,
        KEY_CAPS_LOCK = 280,
        KEY_SCROLL_LOCK = 281,
        KEY_NUM_LOCK = 282,
        KEY_PRINT_SCREEN = 283,
        KEY_PAUSE = 284,
        KEY_F1 = 290,
        KEY_F2 = 291,
        KEY_F3 = 292,
        KEY_F4 = 293,
        KEY_F5 = 294,
        KEY_F6 = 295,
        KEY_F7 = 296,
        KEY_F8 = 297,
        KEY_F9 = 298,
        KEY_F10 = 299,
        KEY_F11 = 300,
        KEY_F12 = 301,
        KEY_F13 = 302,
        KEY_F14 = 303,
        KEY_F15 = 304,
        KEY_F16 = 305,
        KEY_F17 = 306,
        KEY_F18 = 307,
        KEY_F19 = 308,
        KEY_F20 = 309,
        KEY_F21 = 310,
        KEY_F22 = 311,
        KEY_F23 = 312,
        KEY_F24 = 313,
        KEY_F25 = 314,
        KEY_KP_0 = 320,
        KEY_KP_1 = 321,
        KEY_KP_2 = 322,
        KEY_KP_3 = 323,
        KEY_KP_4 = 324,
        KEY_KP_5 = 325,
        KEY_KP_6 = 326,
        KEY_KP_7 = 327,
        KEY_KP_8 = 328,
        KEY_KP_9 = 329,
        KEY_KP_DECIMAL = 330,
        KEY_KP_DIVIDE = 331,
        KEY_KP_MULTIPLY = 332,
        KEY_KP_SUBTRACT = 333,
        KEY_KP_ADD = 334,
        KEY_KP_ENTER = 335,
        KEY_KP_EQUAL = 336,
        KEY_LEFT_SHIFT = 340,
        KEY_LEFT_CONTROL = 341,
        KEY_LEFT_ALT = 342,
        KEY_LEFT_SUPER = 343,
        KEY_RIGHT_SHIFT = 344,
        KEY_RIGHT_CONTROL = 345,
        KEY_RIGHT_ALT = 346,
        KEY_RIGHT_SUPER = 347,
        KEY_MENU = 348,

        KEY_LAST = KEY_MENU,

        MOD_SHIFT = 0x0001,
        MOD_CONTROL = 0x0002,
        MOD_ALT = 0x0004,
        MOD_SUPER = 0x0008,
        MOD_CAPS_LOCK = 0x0010,
        MOD_NUM_LOCK = 0x0020,

        MOUSE_BUTTON_1 = 0,
        MOUSE_BUTTON_2 = 1,
        MOUSE_BUTTON_3 = 2,
        MOUSE_BUTTON_4 = 3,
        MOUSE_BUTTON_5 = 4,
        MOUSE_BUTTON_6 = 5,
        MOUSE_BUTTON_7 = 6,
        MOUSE_BUTTON_8 = 7,
        MOUSE_BUTTON_LAST = MOUSE_BUTTON_8,
        MOUSE_BUTTON_LEFT = MOUSE_BUTTON_1,
        MOUSE_BUTTON_RIGHT = MOUSE_BUTTON_2,
        MOUSE_BUTTON_MIDDLE = MOUSE_BUTTON_3,

        JOYSTICK_1 = 0,
        JOYSTICK_2 = 1,
        JOYSTICK_3 = 2,
        JOYSTICK_4 = 3,
        JOYSTICK_5 = 4,
        JOYSTICK_6 = 5,
        JOYSTICK_7 = 6,
        JOYSTICK_8 = 7,
        JOYSTICK_9 = 8,
        JOYSTICK_10 = 9,
        JOYSTICK_11 = 10,
        JOYSTICK_12 = 11,
        JOYSTICK_13 = 12,
        JOYSTICK_14 = 13,
        JOYSTICK_15 = 14,
        JOYSTICK_16 = 15,
        JOYSTICK_LAST = JOYSTICK_16,

        GAMEPAD_BUTTON_A = 0,
        GAMEPAD_BUTTON_B = 1,
        GAMEPAD_BUTTON_X = 2,
        GAMEPAD_BUTTON_Y = 3,
        GAMEPAD_BUTTON_LEFT_BUMPER = 4,
        GAMEPAD_BUTTON_RIGHT_BUMPER = 5,
        GAMEPAD_BUTTON_BACK = 6,
        GAMEPAD_BUTTON_START = 7,
        GAMEPAD_BUTTON_GUIDE = 8,
        GAMEPAD_BUTTON_LEFT_THUMB = 9,
        GAMEPAD_BUTTON_RIGHT_THUMB = 10,
        GAMEPAD_BUTTON_DPAD_UP = 11,
        GAMEPAD_BUTTON_DPAD_RIGHT = 12,
        GAMEPAD_BUTTON_DPAD_DOWN = 13,
        GAMEPAD_BUTTON_DPAD_LEFT = 14,
        GAMEPAD_BUTTON_LAST = GAMEPAD_BUTTON_DPAD_LEFT,

        GAMEPAD_BUTTON_CROSS = GAMEPAD_BUTTON_A,
        GAMEPAD_BUTTON_CIRCLE = GAMEPAD_BUTTON_B,
        GAMEPAD_BUTTON_SQUARE = GAMEPAD_BUTTON_X,
        GAMEPAD_BUTTON_TRIANGLE = GAMEPAD_BUTTON_Y,

        GAMEPAD_AXIS_LEFT_X = 0,
        GAMEPAD_AXIS_LEFT_Y = 1,
        GAMEPAD_AXIS_RIGHT_X = 2,
        GAMEPAD_AXIS_RIGHT_Y = 3,
        GAMEPAD_AXIS_LEFT_TRIGGER = 4,
        GAMEPAD_AXIS_RIGHT_TRIGGER = 5,
        GAMEPAD_AXIS_LAST = GAMEPAD_AXIS_RIGHT_TRIGGER,

        NO_ERROR = 0,
        NOT_INITIALIZED = 0x00010001,
        NO_CURRENT_CONTEXT = 0x00010002,
        INVALID_ENUM = 0x00010003,
        INVALID_VALUE = 0x00010004,
        OUT_OF_MEMORY = 0x00010005,
        API_UNAVAILABLE = 0x00010006,
        VERSION_UNAVAILABLE = 0x00010007,
        PLATFORM_ERROR = 0x00010008,
        FORMAT_UNAVAILABLE = 0x00010009,
        NO_WINDOW_CONTEXT = 0x0001000A,

        FOCUSED = 0x00020001,
        ICONIFIED = 0x00020002,
        RESIZABLE = 0x00020003,
        VISIBLE = 0x00020004,
        DECORATED = 0x00020005,
        AUTO_ICONIFY = 0x00020006,
        FLOATING = 0x00020007,
        MAXIMIZED = 0x00020008,
        CENTER_CURSOR = 0x00020009,
        TRANSPARENT_FRAMEBUFFER = 0x0002000A,
        HOVERED = 0x0002000B,
        FOCUS_ON_SHOW = 0x0002000C,

        RED_BITS = 0x00021001,
        GREEN_BITS = 0x00021002,
        BLUE_BITS = 0x00021003,
        ALPHA_BITS = 0x00021004,
        DEPTH_BITS = 0x00021005,
        STENCIL_BITS = 0x00021006,
        ACCUM_RED_BITS = 0x00021007,
        ACCUM_GREEN_BITS = 0x00021008,
        ACCUM_BLUE_BITS = 0x00021009,
        ACCUM_ALPHA_BITS = 0x0002100A,

        AUX_BUFFERS = 0x0002100B,
        STEREO = 0x0002100C,
        SAMPLES = 0x0002100D,
        SRGB_CAPABLE = 0x0002100E,
        REFRESH_RATE = 0x0002100F,
        DOUBLEBUFFER = 0x00021010,

        CLIENT_API = 0x00022001,

        CONTEXT_VERSION_MAJOR = 0x00022002,
        CONTEXT_VERSION_MINOR = 0x00022003,
        CONTEXT_REVISION = 0x00022004,
        CONTEXT_ROBUSTNESS = 0x00022005,

        OPENGL_FORWARD_COMPAT = 0x00022006,
        OPENGL_DEBUG_CONTEXT = 0x00022007,
        OPENGL_PROFILE = 0x00022008,

        CONTEXT_RELEASE_BEHAVIOR = 0x00022009,
        CONTEXT_NO_ERROR = 0x0002200A,
        CONTEXT_CREATION_API = 0x0002200B,

        SCALE_TO_MONITOR = 0x0002200C,

        COCOA_RETINA_FRAMEBUFFER = 0x00023001,
        COCOA_FRAME_NAME = 0x00023002,
        COCOA_GRAPHICS_SWITCHING = 0x00023003,

        X11_CLASS_NAME = 0x00024001,
        X11_INSTANCE_NAME = 0x00024002,

        NO_API = 0,
        OPENGL_API = 0x00030001,
        OPENGL_ES_API = 0x00030002,

        NO_ROBUSTNESS = 0,
        NO_RESET_NOTIFICATION = 0x00031001,
        LOSE_CONTEXT_ON_RESET = 0x00031002,

        OPENGL_ANY_PROFILE = 0,
        OPENGL_CORE_PROFILE = 0x00032001,
        OPENGL_COMPAT_PROFILE = 0x00032002,

        CURSOR = 0x00033001,
        STICKY_KEYS = 0x00033002,
        STICKY_MOUSE_BUTTONS = 0x00033003,
        LOCK_KEY_MODS = 0x00033004,
        RAW_MOUSE_MOTION = 0x00033005,

        CURSOR_NORMAL = 0x00034001,
        CURSOR_HIDDEN = 0x00034002,
        CURSOR_DISABLED = 0x00034003,

        ANY_RELEASE_BEHAVIOR = 0,
        RELEASE_BEHAVIOR_FLUSH = 0x00035001,
        RELEASE_BEHAVIOR_NONE = 0x00035002,

        NATIVE_CONTEXT_API = 0x00036001,
        EGL_CONTEXT_API = 0x00036002,
        OSMESA_CONTEXT_API = 0x00036003,

        ARROW_CURSOR = 0x00036001,
        IBEAM_CURSOR = 0x00036002,
        CROSSHAIR_CURSOR = 0x00036003,
        HAND_CURSOR = 0x00036004,
        HRESIZE_CURSOR = 0x00036005,
        VRESIZE_CURSOR = 0x00036006,

        CONNECTED = 0x00040001,
        DISCONNECTED = 0x00040002,

        JOYSTICK_HAT_BUTTONS = 0x00050001,
        COCOA_CHDIR_RESOURCES = 0x00051001,
        COCOA_MENUBAR = 0x00051002,
        DONT_CARE = -1
    }
}
