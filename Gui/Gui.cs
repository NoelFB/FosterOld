using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class Gui : Module
    {

        public SpriteFont Font;
        public Vector2 ContentScale = Vector2.One;
        public Window Window => Manager.Window;

        public readonly Imgui Imgui;

        internal readonly GuiManager Manager;

        public Gui(SpriteFont font, Window window)
        {
            Font = font;
            Imgui = new Imgui(font);
            Manager = new GuiManager(this, window);
        }

        public Gui(SpriteFont font, string title, int width, int height) :
            this(font, App.System.CreateWindow(title, width, height, WindowFlags.ScaleToMonitor))
        {

        }

        public GuiPanel CreatePanel(string title, GuiPanel with)
        {
            var panel = new GuiPanel(this, title);

            if (with.Node != null)
                with.Node.InsertPanel(GuiDockNode.Placings.Center, panel);

            return panel;
        }

        public GuiPanel CreatePanel(string title, Rect bounds)
        {
            var panel = new GuiPanel(this, title);

            var node = new GuiDockNode(Manager, GuiDockNode.Modes.Floating, bounds.Int());
            node.InsertPanel(GuiDockNode.Placings.Center, panel);

            return panel;
        }

        protected override void Update()
        {
            Manager.Update();
        }
    }
}
