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

            UpdateWorkspace();
            UpdateStandalone();

            if (!App.Input.Mouse.LeftDown)
                Dragging = null;
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
