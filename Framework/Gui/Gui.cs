using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Gui : Module
    {


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

        protected internal override void Update()
        {
            manager.Update();
        }
    }
}
