using System.Collections.Generic;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class Gui : Module
    {

        public Window Window { get; private set; }

        public readonly Batch2D Batcher;
        public readonly Imgui Imgui;

        public SpriteFont Font;
        public Vector2 ContentScale = Vector2.One;

        internal readonly DockNode Root;
        internal readonly List<DockNode> Floating = new List<DockNode>();
        internal readonly List<DockNode> Standalone = new List<DockNode>();
        internal readonly List<Panel> Panels = new List<Panel>();

        internal DockNode? Dragging;
        internal DockNode? LastDockable;
        internal DockNode? NextDockable;

        internal Cursors? NextCursor;
        internal Cursors? LastCursor;

        private Vector2 pixelMouse;
        private Vector2 pixelMouseRemainder;
        internal Point2 PixelMouseDrag;

        private Vector2 screenMouse;
        private Vector2 screenMouseRemainder;
        internal Point2 ScreenMouseDrag;

        public Gui()
        {
            using var fontStream = Calc.EmbeddedResource(System.Reflection.Assembly.GetExecutingAssembly(), "Resources/InputMono-Medium.ttf");

            // Create default font & Sprite Batcher
            Font = new SpriteFont(new Font(fontStream), 64, Charsets.ASCII);
            Batcher = new Batch2D();

            // ImGui
            Imgui = new Imgui(Font);

            // Primary Window
            SetPrimaryWindow(App.Window);

            // Root Node
            Root = new DockNode(this, DockNode.Modes.Root);
        }

        public void SetPrimaryWindow(Window window)
        {
            if (Window != window)
            {
                if (Window != null)
                {
                    Window.OnRender -= Render;
                    Window.OnClose -= Close;
                }

                Window = window;
                Window.OnRender += Render;
                Window.OnClose += Close;
            }
        }

        protected override void Shutdown()
        {
            while (Panels.Count > 0)
                Panels[Panels.Count - 1].Close();

            Window.OnRender -= Render;
            Window.OnClose -= Close;
        }

        protected override void Update()
        {
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

            for (int i = Standalone.Count - 1; i >= 0; i--)
                Standalone[i].Positioning();
            for (int i = Floating.Count - 1; i >= 0; i--)
                Floating[i].Positioning();

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
            Batcher.Rect(Window.DrawableBounds, Color.Black);

            Imgui.Step();
            Imgui.BeginViewport(Window, Batcher, ContentScale);
            {
                Root.Refresh();

                for (int i = 0; i < Floating.Count; i++)
                    Floating[i].Refresh();

                Root.DockingGui();
            }

            Imgui.EndViewport();
        }

        private void Close(Window window)
        {
            App.Modules.Remove(this);
        }

        private void Render(Window window)
        {
            Batcher.Render(window);
        }
    }
}
