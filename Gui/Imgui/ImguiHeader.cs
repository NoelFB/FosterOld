using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public static class ImguiHeader
    {
        public static bool Header(this Imgui imgui, string label, bool startOpen = false)
        {
            const string StorageKey = "TOGGLED";

            var toggle = imgui.Button(label);
            var id = imgui.CurrentId;
            var has = imgui.Retreive(id, StorageKey, out bool toggled);
            var enabled = (has && toggled) || (!has && startOpen);

            if (toggle)
                enabled = !enabled;

            imgui.Store(id, StorageKey, enabled);

            if (enabled)
            {
                imgui.PushId(id);
                imgui.PushIndent(20f);
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
