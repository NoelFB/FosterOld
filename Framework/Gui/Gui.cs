using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Gui : Module
    {

        public SpriteFont Font { get; private set; }

        public readonly Window Workspace;
        public readonly List<GuiPanel> Panels = new List<GuiPanel>();

        public Gui(SpriteFont font, Window workspace)
        {
            Font = font;
            Workspace = workspace;
            Workspace.OnRender = Render;
            Workspace.OnResize = (w, h) => Redraw();
        }

        public Gui(SpriteFont font, string title, int width, int height)
        {
            Font = font;
            Workspace = App.System.CreateWindow(title, width, height);
            Workspace.OnRender = Render;
            Workspace.OnResize = (w, h) => Redraw();
        }

        public GuiPanel CreatePanel(string title, RectInt bounds)
        {
            var panel = new GuiPanel(this, title, bounds);
            Panels.Add(panel);
            return panel;
        }

        private void Redraw()
        {
            Update();
            App.Redraw(Workspace);
        }

        protected internal override void Update()
        {
            if (Workspace != null)
            {
                foreach (var window in Panels)
                    window.Update();
            }
        }

        private void Render()
        {
            if (Workspace != null)
            {
                foreach (var window in Panels)
                    window.Render();
            }
        }

    }
}
