using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class Gui : Module
    {
        // TODO:
        // You should be able to set the GUI pixel scale different
        // from the Window.PixelScale. Shouldn't be a hard refactor but
        // window bounds/mouse input stuff will need to be tweaked

        public SpriteFont Font;
        public readonly Imgui Imgui;

        private readonly GuiManager manager;

        public Gui(SpriteFont font, Window window)
        {
            Font = font;
            Imgui = new Imgui(font);
            manager = new GuiManager(this, window);
        }

        public Gui(SpriteFont font, string title, int width, int height) :
            this(font, App.System.CreateWindow(title, width, height))
        {

        }

        public GuiPanel CreatePanel(string title, Rect bounds)
        {
            var panel = new GuiPanel(title);

            var dock = new GuiDock(manager);
            dock.SetAsFloating(bounds);
            dock.Panels.Add(panel);

            return panel;
        }

        protected override void Update()
        {
            manager.Update();
        }
    }
}
