using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiHeader
    {
        public static bool Header(this Imgui context, string label, bool startOpen = false)
        {
            const string StorageKey = "TOGGLED";

            var toggle = context.Button(label);
            var id = context.CurrentId;
            var has = context.Retreive(id, StorageKey, out bool toggled);
            var enabled = (has && toggled) || (!has && startOpen);

            if (toggle)
                enabled = !enabled;

            context.Store(id, StorageKey, enabled);

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
    }
}
