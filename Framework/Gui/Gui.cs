using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class Gui : Module
    {

        public SpriteFont Font;

        public readonly Imgui Imgui;
        public readonly Window Window;
        public readonly Batch2d Batcher;

        public readonly GuiDock Root;
        public readonly List<GuiDock> Floating = new List<GuiDock>();
        public readonly List<GuiDock> Standalone = new List<GuiDock>();

        public Gui(SpriteFont font, Window window)
        {
            Font = font;

            Batcher = new Batch2d();
            Imgui = new Imgui(Font);

            Window = window;
            Window.OnRender = Render;
            Window.OnResize = Resize;

            Root = new GuiDock(this);
            Root.SetAsRoot();

            test();
        }

        public Gui(SpriteFont font, string title, int width, int height) :
            this (font,App.System.CreateWindow(title, width, height))
        {

        }

        private void test()
        {
            var dock0 = new GuiDock(this);
            var dock1 = new GuiDock(this);
            var dock2 = new GuiDock(this);

            dock0.SetAsDock(Root, GuiDock.SplitDirection.Left);
            dock1.SetAsDock(Root, GuiDock.SplitDirection.Right);
            dock2.SetAsDock(dock1, GuiDock.SplitDirection.Top);

            var dock5 = new GuiDock(this);

            var dock6 = new GuiDock(this);
            dock6.SetAsDock(dock5, GuiDock.SplitDirection.Right);

            dock5.SetAsDock(Root, GuiDock.SplitDirection.Bottom);

            GuiPanel p;
            dock5.Left.Panels.Add(p = new GuiPanel("Hello 1"));
            dock5.Left.Panels.Add(p = new GuiPanel("Hello 2"));
            p.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 20; i++)
                    imgui.Button($"What {i}");
            };

            var dock8 = new GuiDock(this);
            dock8.SetAsStandalone(new Rect(32, 32, 400, 400));
            dock8.Panels.Add(p = new GuiPanel("what"));
            dock8.Panels.Add(p = new GuiPanel("what 2"));
            p.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 20; i++)
                    imgui.Button($"What {i}");
            };

            var dock7 = new GuiDock(this);
            dock7.SetAsFloating(new Rect(200, 32, 300, 400));
            dock7.Panels.Add(p = new GuiPanel("what"));
            dock7.Panels.Add(p = new GuiPanel("what 2"));
            dock7.Panels.Add(p = new GuiPanel("what 3"));
            dock7.Panels.Add(p = new GuiPanel("what 4"));
            p.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 20; i++)
                    imgui.Button($"What {i}");
            };

            dock7 = new GuiDock(this);
            dock7.SetAsDock(dock8, GuiDock.SplitDirection.Bottom);
            dock7.Panels.Add(p = new GuiPanel("what"));
            dock7.Panels.Add(p = new GuiPanel("what 2"));
            dock7.Panels.Add(p = new GuiPanel("what 3"));
            dock7.Panels.Add(p = new GuiPanel("what 4"));
            p.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 20; i++)
                    imgui.Button($"What {i}");
            };
        }

        protected internal override void Update()
        {
            UpdateWorkspace();
            UpdateStandalone();
        }

        private void UpdateWorkspace()
        {
            Batcher.Clear();

            Imgui.Step();
            Imgui.BeginViewport(Window, Batcher);
            {
                Root.Refresh();

                for (int i = 0; i < Floating.Count; i++)
                    Floating[i].Refresh();
            }
            Imgui.EndViewport();
        }

        private void UpdateStandalone()
        {
            for (int i = 0; i < Standalone.Count; i++)
                Standalone[i].Refresh();
        }

        private void Resize(int width, int height)
        {
            UpdateWorkspace();
            Window.Render();
            Window.Present();
        }

        private void Render()
        {
            Batcher.Render();
        }
    }
}
