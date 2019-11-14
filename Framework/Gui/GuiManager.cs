using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class GuiManager
    {

        public readonly Gui Gui;
        public readonly Window Window;
        public readonly Batch2d Batcher;

        public readonly GuiDock Root;
        public readonly List<GuiDock> Floating = new List<GuiDock>();
        public readonly List<GuiDock> Standalone = new List<GuiDock>();

        public GuiDock? Dragging;
        public GuiDock? LastDockable;
        public GuiDock? NextDockable;
        public Cursors? NextCursor;
        public Cursors? LastCursor;

        public GuiManager(Gui gui, Window window)
        {
            Gui = gui;
            Batcher = new Batch2d();

            Window = window;
            Window.OnRender = Render;
            Window.OnResize = Resize;

            Root = new GuiDock(this);
            Root.SetAsRoot();
        }

        public GuiManager(Gui gui, string title, int width, int height) :
            this (gui, App.System.CreateWindow(title, width, height))
        {

        }

        public void Update()
        {
            LastDockable = NextDockable;
            NextDockable = null;
            LastCursor = NextCursor;
            NextCursor = null;

            foreach (var standalone in Standalone)
                standalone.UpdateWindow();
            foreach (var floating in Floating)
                floating.UpdateWindow();

            UpdateWorkspace();

            for (int i = 0; i < Standalone.Count; i++)
                Standalone[i].Refresh();

            if (!App.Input.Mouse.LeftDown)
                Dragging = null;

            if (NextCursor != null)
                App.Input.SetMouseCursor(NextCursor.Value);
            else if (LastCursor != null)
                App.Input.SetMouseCursor(Cursors.Default);
        }

        private void UpdateWorkspace()
        {
            Batcher.Clear();

            Gui.Imgui.Step();
            Gui.Imgui.BeginViewport(Window, Batcher);
            {
                Root.Refresh();

                for (int i = 0; i < Floating.Count; i++)
                    Floating[i].Refresh();

                Root.DockingGui();
            }
            Gui.Imgui.EndViewport();
        }

        private void Resize(int width, int height)
        {
            App.Graphics.Clear(Color.Black);
            UpdateWorkspace();
            Window.Render();
            Window.Present();
        }

        private void Render()
        {
            App.Graphics.Clear(Color.Black);
            Batcher.Render();
        }
    }
}
