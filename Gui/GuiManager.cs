using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    internal class GuiManager
    {

        public readonly Gui Gui;
        public readonly Window Window;
        public readonly Batch2d Batcher;

        public readonly GuiDockNode RootNode;
        public readonly GuiDock Root;
        public readonly List<GuiDock> Floating = new List<GuiDock>();
        public readonly List<GuiDock> Standalone = new List<GuiDock>();

        public GuiDock? Dragging;
        public GuiDock? LastDockable;
        public GuiDock? NextDockable;
        public Cursors? NextCursor;
        public Cursors? LastCursor;

        private Vector2 pixelMouse;
        private Vector2 pixelMouseRemainder;
        public Point2 PixelMouseDrag;

        private Vector2 screenMouse;
        private Vector2 screenMouseRemainder;
        public Point2 ScreenMouseDrag;

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
            // active dock we're hovering over
            LastDockable = NextDockable;
            NextDockable = null;

            // unset the next cursor
            LastCursor = NextCursor;
            NextCursor = null;

            // get the mouse deltas
            {
                var nextScreenMouse = Window.ScreenMouse;
                screenMouseRemainder += nextScreenMouse - screenMouse;
                ScreenMouseDrag = screenMouseRemainder.Floor();
                screenMouseRemainder -= ScreenMouseDrag;
                screenMouse = nextScreenMouse;

                var nextFloatingMouse = Window.DrawableMouse;
                pixelMouseRemainder += nextFloatingMouse - pixelMouse;
                PixelMouseDrag = pixelMouseRemainder.Floor();
                pixelMouseRemainder -= PixelMouseDrag;
                pixelMouse = nextFloatingMouse;
            }

            // Update Window bounds of Standalone and Floating windows
            for (int i = Standalone.Count - 1; i >= 0; i--)
                Standalone[i].DoWindowDragging();
            for (int i = Floating.Count - 1; i >= 0; i --)
                Floating[i].DoWindowDragging();

            // Update the primary worksapce
            UpdateWorkspace();

            // Refresh Standalone Windows
            for (int i = 0; i < Standalone.Count; i++)
                Standalone[i].Refresh();

            // Unset the Dock we're dragging
            if (!App.Input.Mouse.LeftDown)
                Dragging = null;

            // Set the Cursor
            if (NextCursor != null)
                App.Input.SetMouseCursor(NextCursor.Value);
            else if (LastCursor != null)
                App.Input.SetMouseCursor(Cursors.Default);
        }

        private void UpdateWorkspace()
        {
            Batcher.Clear();

            Gui.Imgui.Step();
            Gui.Imgui.BeginViewport(Window, Batcher, Gui.ContentScale);
            {
                Root.Refresh();

                for (int i = 0; i < Floating.Count; i++)
                    Floating[i].Refresh();

                Root.DockingGui();
            }
            Gui.Imgui.EndViewport();
        }

        private void Resize()
        {
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
