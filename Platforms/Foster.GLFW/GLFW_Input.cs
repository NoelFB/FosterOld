using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Foster.GLFW
{
    public class GLFW_Input : Input
    {

        private Stopwatch timer = new Stopwatch();

        protected override void Created()
        {
            timer.Start();

            // get API info
            {
                GLFW.GetVersion(out int major, out int minor, out int rev);
                ApiName = "GLFW";
                ApiVersion = new Version(major, minor, rev);
            }

            base.Created();
        }

        protected override void Startup()
        {
            base.Startup();

            var system = App.System as GLFW_System;
            if (system != null)
            {
                GLFW.SetKeyCallback(system.Contexts[0].Handle, OnKeyCallback);
                GLFW.SetCharCallback(system.Contexts[0].Handle, OnCharCallback);
                GLFW.SetMouseButtonCallback(system.Contexts[0].Handle, OnMouseCallback);

                system.OnWindowCreated += (window) =>
                {
                    var context = (window.Context as GLFW_Context);
                    if (context != null)
                    {
                        GLFW.SetKeyCallback(context.Handle, OnKeyCallback);
                        GLFW.SetCharCallback(context.Handle, OnCharCallback);
                        GLFW.SetMouseButtonCallback(context.Handle, OnMouseCallback);
                    }
                };
            }
        }

        private void OnMouseCallback(GLFW.Window window, int button, int action, int mods)
        {
            MouseButtons mb = MouseButtons.Unknown;
            if (button == 0)
                mb = MouseButtons.Left;
            else if (button == 1)
                mb = MouseButtons.Right;
            else if (button == 2)
                mb = MouseButtons.Middle;

            if (action == 1)
            {
                OnMouseDown(mb, (ulong)timer.ElapsedMilliseconds);
            }
            else if (action == 0)
            {
                OnMouseUp(mb, (ulong)timer.ElapsedMilliseconds);
            }
        }

        private void OnCharCallback(GLFW.Window window, uint codepoint)
        {
            OnText((char)codepoint);
        }

        private void OnKeyCallback(GLFW.Window window, int key, int scancode, int action, int mods)
        {
            if (action == 1)
            {
                OnKeyDown((uint)key, (ulong)timer.ElapsedMilliseconds);
            }
            else if (action == 0)
            {
                OnKeyUp((uint)key, (ulong)timer.ElapsedMilliseconds);
            }
        }

    }
}
