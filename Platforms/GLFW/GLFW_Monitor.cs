using Foster.Framework;
using System;
using System.Numerics;

namespace Foster.GLFW;

internal class GLFW_Monitor : Monitor
{
    public readonly IntPtr Pointer;

    private string name;
    private bool isPrimary;
    private RectInt bounds;
    private Vector2 contentScale;

    public override string Name => name;
    public override bool IsPrimary => isPrimary;
    public override RectInt Bounds => bounds;
    public override Vector2 ContentScale => contentScale;

    public GLFW_Monitor(IntPtr pointer)
    {
        Pointer = pointer;

        name = GLFW.GetMonitorName(Pointer);
        FetchProperties();
    }

    public void FetchProperties()
    {
        GLFW.GetMonitorContentScale(Pointer, out contentScale.X, out contentScale.Y);
        GLFW.GetMonitorWorkarea(Pointer, out bounds.X, out bounds.Y, out bounds.Width, out bounds.Height);

        isPrimary = GLFW.GetPrimaryMonitor() == Pointer;
    }
}
