using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class GuiPanel
    {

        public string Title;
        public RectInt Bounds;

        public readonly Gui Gui;
        public readonly ImguiContext Imgui;

        internal GuiPanel(Gui gui, string title, RectInt bounds)
        {
            Gui = gui;
            Title = title;
            Imgui = new ImguiContext(gui.Font);
            Bounds = bounds;
        }

        public void Update()
        {
            Imgui.PixelSize = Gui.Workspace.PixelSize;
            Imgui.Bounds = Bounds;
            Imgui.Update(Gui.Workspace.Mouse);
        }

        public void Render()
        {
            Imgui.Render();
        }
    }
}
