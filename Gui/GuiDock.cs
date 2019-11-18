using System;
using System.Collections.Generic;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class GuiDock
    {
        #region Enums

        public enum Modes
        {
            /// <summary>
            /// This Dock has no Mode and won't be drawn or updated
            /// </summary>
            None,

            /// <summary>
            /// This Dock is the root of the Gui
            /// </summary>
            Root,

            /// <summary>
            /// This Dock resides within another Dock
            /// </summary>
            Docked,

            /// <summary>
            /// This Dock is floating within the Gui
            /// </summary>
            Floating,

            /// <summary>
            /// This Dock is a standalone window
            /// </summary>
            Standalone
        }

        public enum DockTo
        {
            Fill,
            Left,
            Right,
            Top,
            Bottom
        }

        [Flags]
        public enum Dragging
        {
            None = 0,
            Position = 1,
            Left = 2,
            Right = 4,
            Top = 8,
            Bottom = 16
        }

        #endregion

        #region Public Variables

        public readonly int ID;
        public readonly Gui Gui;
        public readonly GuiManager Manager;

        public Imgui Imgui => Gui.Imgui;
        public Modes Mode { get; private set; } = Modes.None;
        public Batch2d Batcher { get; private set; }
        public Window Window { get; private set; }

        public float SplitPoint = 0.5f;
        public bool SplitHorizontally = true;

        public readonly List<GuiPanel> Panels = new List<GuiPanel>();
        public int PanelIndex;
        public GuiPanel? Panel => PanelIndex >= 0 && PanelIndex < Panels.Count ? Panels[PanelIndex] : null;
        
        public GuiDock? Parent => parent;

        public GuiDock? Left
        {
            get => left;
            set
            {
                if (left != null)
                    left.parent = null;

                left = value;

                if (left != null)
                    left.parent = this;
            }
        }

        public GuiDock? Right
        {
            get => right;
            set
            {
                if (right != null)
                    right.parent = null;

                right = value;

                if (right != null)
                    right.parent = this;
            }
        }

        #endregion

        #region Private Variables

        private const float splitSize = 1f;
        private const float splitGrabSize = 8f;

        private GuiDock? parent;
        private GuiDock? left;
        private GuiDock? right;

        private const int standaloneWindowEdge = 4;
        private Rect floatingBounds;
        private Dragging dragging = Dragging.None;

        #endregion

        #region Constructor

        public GuiDock(GuiManager manager)
        {
            Gui = manager.Gui;
            Manager = manager;
            Window = manager.Window;
            Batcher = manager.Batcher;
            ID = Guid.NewGuid().GetHashCode();
        }

        #endregion

        #region Change Dock Mode

        public void SetAsRoot()
        {
            if (Mode != Modes.Root)
            {
                UnsetLastMode();

                if (Manager.Root != null && Manager.Root != this)
                    throw new Exception("Workspace already has a Root Dock");

                Mode = Modes.Root;
                Window = Manager.Window;
                Batcher = Manager.Batcher;
            }
        }

        public void SetAsFloating(Rect bounds)
        {
            if (Mode != Modes.Floating)
            {
                UnsetLastMode();
                Manager.Floating.Add(this);
                Window = Manager.Window;
                Batcher = Manager.Batcher;
                Mode = Modes.Floating;
                floatingBounds = bounds;
            }
        }

        public void SetAsStandalone(RectInt bounds)
        {
            if (Mode != Modes.Standalone)
            {
                UnsetLastMode();

                Mode = Modes.Standalone;
                Manager.Standalone.Add(this);

                // when we create a window we ask for it in non-High-DPI units
                var width = (int)(bounds.Width / Manager.Window.ContentScale.X);
                var height = (int)(bounds.Height / Manager.Window.ContentScale.Y);

                Window = App.System.CreateWindow("Gui Dock", width + standaloneWindowEdge, height + standaloneWindowEdge, false, true);
                Window.Position = bounds.TopLeft;
                Window.VSync = false;
                Window.Bordered = false;
                Window.OnRender = () =>
                {
                    App.Graphics.Clear(Color.Transparent);
                    Batcher.Render();
                };
                Window.OnClose = () =>
                {
                    if (Mode == Modes.Standalone)
                        UnsetLastMode();
                };

                Window.Visible = true;
                Batcher = new Batch2d();
            }
        }

        public void SetAsDock(GuiDock parent, DockTo split)
        {
            if (Mode == Modes.Docked)
                throw new Exception("The Dock is already Docked");

            if (IsChildOf(parent))
                throw new Exception("The Dock is already a child of this Dock");

            UnsetLastMode();

            if (split == DockTo.Fill)
            {
                if (parent.Left != null || parent.Right != null)
                    throw new Exception("Can't fill a Dock that is already split");

                if (parent.Panels.Count <= 0)
                {
                    parent.Left = Left;
                    parent.Right = Right;
                    parent.SplitHorizontally = SplitHorizontally;
                    parent.SplitPoint = SplitPoint;
                    parent.Panels.AddRange(Panels);
                }
                else
                {
                    GetAllChildPanels(parent.Panels);
                }
            }
            else
            {
                GuiDock? other = null;

                // we must subdivide since the parent already has a left/right dock
                if (parent.Left != null || parent.Right != null)
                {
                    other = new GuiDock(Manager);

                    other.Left = parent.Left;
                    other.Right = parent.Right;
                    other.Mode = Modes.Docked;
                    other.SplitHorizontally = parent.SplitHorizontally;
                    other.SplitPoint = parent.SplitPoint;
                }
                // move parent contents to a new child
                else
                {
                    other = new GuiDock(Manager);
                    other.Panels.AddRange(parent.Panels);
                    other.PanelIndex = parent.PanelIndex;
                    other.Mode = Modes.Docked;

                    parent.Panels.Clear();
                    parent.PanelIndex = 0;
                }

                if (split == DockTo.Left || split == DockTo.Top)
                {
                    parent.Left = this;
                    parent.Right = other;
                }
                else
                {
                    parent.Left = other;
                    parent.Right = this;
                }

                parent.SplitPoint = 0.5f;
                parent.SplitHorizontally = (split == DockTo.Left || split == DockTo.Right);

                Mode = Modes.Docked;
            }
        }

        private void UnsetLastMode()
        {
            if (Mode == Modes.Root)
            {
                throw new Exception("A workspace Root Dock cannot be undocked");
            }
            else if (Mode == Modes.Floating)
            {
                Manager.Floating.Remove(this);
            }
            else if (Mode == Modes.Standalone)
            {
                Window.OnClose = null;
                Window.OnResize = null;
                Window.OnRender = null;
                Window.Close();

                Batcher.Dispose();

                Manager.Standalone.Remove(this);
            }
            else if (Mode == Modes.Docked)
            {
                if (Parent != null)
                {
                    var lastParent = Parent;

                    GuiDock? other = null;
                    if (lastParent.Left == this)
                        other = lastParent.Right;
                    else if (lastParent.Right == this)
                        other = lastParent.Left;

                    lastParent.Left = null;
                    lastParent.Right = null;

                    if (other != null)
                    {
                        lastParent.SplitPoint = other.SplitPoint;
                        lastParent.SplitHorizontally = other.SplitHorizontally;
                        lastParent.Left = other.Left;
                        lastParent.Right = other.Right;
                        lastParent.Panels.Clear();
                        lastParent.Panels.AddRange(other.Panels);
                    }

                    // parent now also has nothing in it ...
                    if (lastParent.Left == null && lastParent.Right == null && lastParent.Panels.Count <= 0 && lastParent.Mode != Modes.Root)
                        lastParent.UnsetLastMode();
                }

                parent = null;
            }

            Mode = Modes.None;
        }

        #endregion

        #region Utilities & Short-hands

        public Rect GetContentBounds()
        {
            if (Mode == Modes.Docked)
            {
                var bounds = Parent!.GetContentBounds();
                var split = Parent.SplitPoint;
                var size = splitSize / 2f;

                if (Parent.SplitHorizontally)
                {
                    if (Parent.Left == this)
                        return new Rect(bounds.X, bounds.Y, bounds.Width * split - size, bounds.Height);
                    else if (Parent.Right == this)
                        return new Rect(bounds.X + bounds.Width * split + size, bounds.Y, bounds.Width * (1 - split) - size, bounds.Height);
                }
                else
                {
                    if (Parent.Left == this)
                        return new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * split - size);
                    else if (Parent.Right == this)
                        return new Rect(bounds.X, bounds.Y + bounds.Height * split + size, bounds.Width, bounds.Height * (1 - split) - size);
                }
            }
            else if (Mode == Modes.Floating)
            {
                return floatingBounds;
            }
            else if (Mode == Modes.Standalone || Mode == Modes.Root)
            {
                var scale = Window.ContentScale * Gui.ContentScale;
                var bounds = new Rect(0, 0, Window.DrawableWidth / scale.X, Window.DrawableHeight / scale.Y);

                if (Mode == Modes.Standalone)
                {
                    bounds.Width -= standaloneWindowEdge;
                    bounds.Height -= standaloneWindowEdge;
                }

                return bounds;
            }

            return new Rect();
        }

        public GuiDock GetBaseDock()
        {
            if (Parent == null)
                return this;
            return Parent.GetBaseDock();
        }

        public GuiPanel? GetTopLeftMostChildPanel()
        {
            if (Left != null)
                return Left.GetTopLeftMostChildPanel();
            if (Panels.Count > 0)
                return Panel;
            return null;
        }

        private bool IsChildOf(GuiDock? dock)
        {
            if (dock != null)
            {
                GuiDock? current = this;
                while (current != null)
                {
                    if (current == dock)
                        return true;
                    current = current.Parent;
                }
            }

            return false;
        }

        private void GetAllChildPanels(List<GuiPanel> populate)
        {
            populate.AddRange(Panels);
            Left?.GetAllChildPanels(populate);
            Right?.GetAllChildPanels(populate);
        }

        private RectInt BoundsToScreen(Rect bounds)
        {
            var scale = Window.ContentScale * Gui.ContentScale / Window.DrawableScale;
            return (Window.Position + bounds * scale).Int();
        }

        private Rect ScreenToBounds(Window into, RectInt bounds)
        {
            var scale = into.DrawableScale / (into.ContentScale * Gui.ContentScale);
            var x = (bounds.X - into.X) * scale.X;
            var y = (bounds.Y - into.Y) * scale.Y;
            var w = bounds.Width * scale.X;
            var h = bounds.Height * scale.Y;

            return new Rect(x, y, w, h);
        }

        #endregion

        #region Refresh

        public void DoWindowDragging()
        {
            if (dragging != Dragging.None)
            {
                if (Mode == Modes.Standalone)
                {
                    var drag = Manager.ScreenMouseDrag;
                    if (drag.Length > 0)
                    {
                        var bounds = Window.Bounds;
                        if (dragging.HasFlag(Dragging.Position))
                            bounds.Position += drag;
                        if (dragging.HasFlag(Dragging.Left))
                            bounds.Left += drag.X;
                        if (dragging.HasFlag(Dragging.Right))
                            bounds.Right += drag.X;
                        if (dragging.HasFlag(Dragging.Top))
                            bounds.Top += drag.Y;
                        if (dragging.HasFlag(Dragging.Bottom))
                            bounds.Bottom += drag.Y;

                        if (bounds != Window.Bounds)
                            Window.Bounds = bounds;

                        // turn back into a floating window
                        if (Manager.Window.Bounds.Contains(Window.Bounds))
                        {
                            var rect = ScreenToBounds(Manager.Window, Window.Bounds);
                            SetAsFloating(new Rect(rect.X, rect.Y, rect.Width - standaloneWindowEdge, rect.Height - standaloneWindowEdge));
                        }
                    }
                }
                else if (Mode == Modes.Floating)
                {
                    var drag = Manager.PixelMouseDrag / (Window.ContentScale * Gui.ContentScale);
                    if (drag.Length > 0)
                    {
                        if (dragging.HasFlag(Dragging.Position))
                            floatingBounds.Position += drag;
                        if (dragging.HasFlag(Dragging.Left))
                            floatingBounds.Left += drag.X;
                        if (dragging.HasFlag(Dragging.Right))
                            floatingBounds.Right += drag.X;
                        if (dragging.HasFlag(Dragging.Top))
                            floatingBounds.Top += drag.Y;
                        if (dragging.HasFlag(Dragging.Bottom))
                            floatingBounds.Bottom += drag.Y;

                        // pop out of the frame if it doesn't fit
                        if (App.System.SupportsMultipleWindows)
                        {
                            if (!Manager.Window.Bounds.Contains(BoundsToScreen(floatingBounds)))
                            {
                                SetAsStandalone(BoundsToScreen(floatingBounds));
                            }
                        }
                    }
                }
            }

            dragging = Dragging.None;
        }

        public void Refresh()
        {
            var bounds = GetContentBounds();
            var mode = Mode;

            Begin();
            Display();
            Resizing();
            End();

            void Begin()
            {
                if (mode == Modes.Standalone)
                {
                    Batcher.Clear();
                    Imgui.BeginViewport(Window, Batcher, Gui.ContentScale);
                    Batcher.Rect(bounds, Color.Black);
                }
            }

            void Display()
            {
                if (mode == Modes.Floating || mode == Modes.Standalone)
                    Batcher.Rect(bounds + new Vector2(4, 4), Color.Black * 0.25f);

                // Split Content
                if (Left != null || Right != null)
                {
                    Left?.Refresh();
                    Right?.Refresh();

                    if (SplitHorizontally)
                    {
                        var split = new Rect(bounds.X + bounds.Width * SplitPoint - splitGrabSize / 2, bounds.Y + 1, splitGrabSize, bounds.Height - 2);
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 10), split))
                        {
                            var world = Math.Clamp(bounds.Width * SplitPoint + Imgui.Viewport.MouseDelta.X, 64, bounds.Width - 64);
                            SplitPoint = world / bounds.Width;
                        }

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.HorizontalResize;
                    }
                    else
                    {
                        var split = new Rect(bounds.X + 1, bounds.Y + bounds.Height * SplitPoint - splitGrabSize / 2, bounds.Width - 2, splitGrabSize);
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 10), split))
                        {

                            var world = Math.Clamp(bounds.Height * SplitPoint + Imgui.Viewport.MouseDelta.Y, 64, bounds.Height - 64);
                            SplitPoint = world / bounds.Height;
                        }

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.VerticalResize;
                    }

                }
                // Main Content (Panels)
                else
                {
                    if (bounds.Contains(Imgui.Viewport.Mouse) && !IsChildOf(Manager.Dragging))
                        Manager.NextDockable = this;

                    if (Panels.Count > 0)
                    {
                        while (PanelIndex >= Panels.Count)
                            PanelIndex--;
                        if (PanelIndex < 0)
                            PanelIndex = 0;

                        if (Imgui.BeginFrame(ID + 1, bounds, false))
                        {
                            // If they grab anywhere on the Frame
                            HandleDragging(bounds, true, null);

                            Imgui.Row(Panels.Count);

                            var style = Imgui.Style;
                            style.ItemBackgroundColor = style.FrameBackgroundColor;
                            style.ItemBorderWeight = 0f;
                            Imgui.PushStyle(style);

                            for (int i = 0; i < Panels.Count; i++)
                            {
                                var id = new Imgui.ID(Panels[i].ID, 0);
                                if (Imgui.Button(id, Panels[i].Title, Imgui.PreferredSize))
                                    PanelIndex = i;

                                // if they grab a single tab
                                HandleDragging(bounds, Panels.Count <= 1 && Parent == null, Panels[i]);
                            }

                            style.ItemSpacing = 0;
                            Imgui.PushStyle(style);
                            var remainder = Imgui.Remainder();
                            Imgui.PopStyle();
                            Imgui.PopStyle();

                            if (Panel != null)
                            {
                                if (Imgui.BeginFrame(ID, remainder))
                                {
                                    Panel.OnRefresh?.Invoke(Imgui);
                                    Imgui.EndFrame();
                                }
                            }

                            Imgui.EndFrame();
                        }
                    }
                }
            }

            void Resizing()
            {
                if (mode == Modes.Standalone || mode == Modes.Floating)
                {
                    // left
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 11), new Rect(bounds.X - 3, bounds.Y, 6, bounds.Height)))
                            dragging = Dragging.Left;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.HorizontalResize;
                    }

                    // top
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 12), new Rect(bounds.X, bounds.Y - 3, bounds.Width, 6)))
                            dragging = Dragging.Top;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.VerticalResize;
                    }

                    // right
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 13), new Rect(bounds.Right - 3, bounds.Y, 6, bounds.Height)))
                            dragging = Dragging.Right;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.HorizontalResize;
                    }

                    // bottom
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 14), new Rect(bounds.X, bounds.Bottom - 3, bounds.Width, 6)))
                            dragging = Dragging.Bottom;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.VerticalResize;
                    }

                    // bottom right
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 15), new Rect(bounds.Right - 3, bounds.Bottom - 3, 6, 6)))
                            dragging = Dragging.Bottom | Dragging.Right;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Manager.NextCursor = Cursors.Crosshair;
                    }
                }
            }

            void End()
            {
                if (mode == Modes.Standalone)
                {
                    DockingGui();
                    Imgui.EndViewport();
                }
            }
        }

        private void HandleDragging(Rect bounds, bool draggingWindow, GuiPanel? draggingSinglePanel)
        {
            if (Imgui.CurrentId == Imgui.ActiveId)
            {
                // only move if we need to
                if (Manager.PixelMouseDrag.Length > 0)
                {
                    var root = GetBaseDock();

                    // drag out a single panel from a list of panels
                    if (draggingSinglePanel != null && Panels.Count > 1)
                    {
                        var dock = new GuiDock(Manager);

                        if (App.System.SupportsMultipleWindows)
                            dock.SetAsStandalone(BoundsToScreen(bounds));
                        else
                            dock.SetAsFloating(bounds);

                        dock.Panels.Add(draggingSinglePanel);
                        Panels.Remove(draggingSinglePanel);
                    }
                    // pop out of the root
                    // The Root is allowed to be totally empty
                    else if (Mode == Modes.Root)
                    {
                        var dock = new GuiDock(Manager);

                        if (App.System.SupportsMultipleWindows)
                            dock.SetAsStandalone(BoundsToScreen(bounds));
                        else
                            dock.SetAsFloating(bounds);

                        dock.Panels.AddRange(Panels);
                        dock.PanelIndex = PanelIndex;
                        dock.SplitHorizontally = SplitHorizontally;
                        dock.SplitPoint = SplitPoint;
                        dock.Left = Left;
                        dock.Right = Right;
                        Panels.Clear();
                    }
                    // drag the standalone Window around normally
                    else if (draggingWindow && root.Mode == Modes.Standalone)
                    {
                        Manager.Dragging = root;
                        root.dragging = Dragging.Position;
                    }
                    // drag the floating Window around normally
                    else if (draggingWindow && root.Mode == Modes.Floating)
                    {
                        Manager.Dragging = root;
                        root.dragging = Dragging.Position;
                    }
                    // pop out of our parent
                    else
                    {
                        if (App.System.SupportsMultipleWindows)
                            SetAsStandalone(BoundsToScreen(bounds));
                        else
                            SetAsFloating(bounds);
                    }
                }
            }
        }

        public void DockingGui()
        {
            if (Manager.LastDockable != null && Manager.Dragging != null && !Manager.LastDockable.IsChildOf(Manager.Dragging))
            {
                var other = Manager.LastDockable.Window;
                var splitable = Manager.LastDockable.Mode != Modes.Root || Manager.LastDockable.Panels.Count > 0;

                // get the offset relative to the docking window
                var offset = other.Position * other.DrawableScale / (other.ContentScale * Gui.ContentScale) - 
                             Window.Position * Window.DrawableScale / (Window.ContentScale * Gui.ContentScale);

                // get the bounds of the docking window
                var bounds = Manager.LastDockable.GetContentBounds();
                bounds.X += offset.X;
                bounds.Y += offset.Y;

                var color = new Color(0x4f98ff);
                var back = new Color(0x2b0c91);
                var center = bounds.Center.Floor();
                var size = 50f;
                var fill = new Rect(center.X - size / 2f, center.Y - size / 2f, size, size);
                var left = new Rect(center.X - size / 2f - size, center.Y - size / 2f, size, size);
                var right = new Rect(center.X - size / 2f + size, center.Y - size / 2f, size, size);
                var top = new Rect(center.X - size / 2f, center.Y - size / 2f - size, size, size);
                var bottom = new Rect(center.X - size / 2f, center.Y - size / 2f + size, size, size);

                DockHover(fill, DockTo.Fill);

                if (splitable)
                {
                    DockHover(left, DockTo.Left);
                    DockHover(right, DockTo.Right);
                    DockHover(top, DockTo.Top);
                    DockHover(bottom, DockTo.Bottom);
                }

                DockButton(fill, DockTo.Fill);

                if (splitable)
                {
                    DockButton(left, DockTo.Left);
                    DockButton(right, DockTo.Right);
                    DockButton(top, DockTo.Top);
                    DockButton(bottom, DockTo.Bottom);
                }

                void DockButton(Rect rect, DockTo split)
                {
                    if (Manager.Dragging != null)
                    {
                        var hover = false;

                        Batcher.Rect(rect.Inflate(-3), back);
                        
                        if (rect.Contains(Imgui.Viewport.Mouse))
                        {
                            hover = true;
                            if (!App.Input.Mouse.Down(MouseButtons.Left))
                            {
                                Manager.Dragging.SetAsDock(Manager.LastDockable!, split);
                                Manager.Dragging = null;
                            }
                        }

                        var line = color * 0.75f;
                        if (hover)
                            line = Color.Lerp(color, Color.White, 0.5f);

                        var box = rect.Inflate(-4f);
                        Batcher.HollowRect(box, 1f, line);
                        Batcher.Rect(box.X + 1, box.Y + 1, box.Width - 2, 4f, line);

                        if (split == DockTo.Left || split == DockTo.Right)
                            Batcher.Rect(box.Center.X - 0.5f, box.Y + 6, 1f, box.Height - 7, line);
                        if (split == DockTo.Top || split == DockTo.Bottom)
                            Batcher.Rect(box.X + 1, box.Center.Y - 0.5f, box.Width - 2f, 1f, line);
                    }
                }

                void DockHover(Rect rect, DockTo split)
                {
                    if (Manager.Dragging != null && rect.Contains(Imgui.Viewport.Mouse))
                    {
                        var fill = bounds;

                        if (split == DockTo.Left)
                            fill = new Rect(bounds.X, bounds.Y, bounds.Width * 0.5f, bounds.Height);
                        else if (split == DockTo.Right)
                            fill = new Rect(bounds.X + bounds.Width * 0.5f, bounds.Y, bounds.Width * 0.5f, bounds.Height);
                        else if (split == DockTo.Top)
                            fill = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * 0.5f);
                        else if (split == DockTo.Bottom)
                            fill = new Rect(bounds.X, bounds.Y + bounds.Height * 0.5f, bounds.Width, bounds.Height * 0.5f);

                        fill = fill.Inflate(-5);

                        var w = Math.Min(fill.Width * 0.25f, 32f);
                        var h = Math.Min(fill.Height * 0.25f, 32f);
                        var t = 5f;

                        Batcher.Rect(fill, color * 0.5f);
                        Batcher.HollowRect(fill, 1f, color);
                        Batcher.Rect(fill.X, fill.Y, w, t, color);
                        Batcher.Rect(fill.X, fill.Y, t, h, color);
                        Batcher.Rect(fill.Right - w, fill.Y, w, t, color);
                        Batcher.Rect(fill.Right - t, fill.Y, t, h, color);
                        Batcher.Rect(fill.X, fill.Bottom - t, w, t, color);
                        Batcher.Rect(fill.X, fill.Bottom - h, t, h, color);
                        Batcher.Rect(fill.Right - w, fill.Bottom - t, w, t, color);
                        Batcher.Rect(fill.Right - t, fill.Bottom - h, t, h, color);
                    }
                }
            }
        }

        #endregion
    }
}
