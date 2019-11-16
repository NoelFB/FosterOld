using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiHeader
    {
        /*
        public static bool Header(this Imgui context, string label, bool startOpen = false)
        {
            var toggle = context.Button(label);
            var id = context.LastId;
            var enabled = (context.Stored(id, out var info) && info.Toggled) || (!context.Stored(id) && startOpen);

            if (toggle)
                enabled = !enabled;

            context.Store(id, new Imgui.Storage() { Toggled = enabled });

            if (enabled)
            {
                context.PushId(id);
                context.PushIndent(30f);
            }

            return enabled;
        }

        public static void EndHeader(this Imgui context)
        {
            context.PopIndent();
            context.PopId();
        }
        */
    }
}
