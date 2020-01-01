using Foster.Framework;
using System;
using System.Collections.Generic;

namespace Foster.GuiSystem
{
    internal class DockNode
    {
        public enum Modes
        {
            Root,
            Standalone,
            Floating,
            Docked
        }

        public enum Placings
        {
            Center,
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

        public int ID { get; private set; } = Guid.NewGuid().GetHashCode();
        public readonly Gui Gui;
        public readonly Imgui Imgui;
        public readonly Modes Mode;
        public readonly Window Window;
        public readonly Batch2D Batcher;

        private DockNode? parent;
        private DockNode? leftChild;
        private DockNode? rightChild;
        private float splitPoint = 0.5f;
        private bool splitHorizontally = true;
        private readonly List<Panel> panels = new List<Panel>();
        private Panel? activePanel;
        private Rect floatingBounds;
        private Dragging dragging = Dragging.None;
        private bool modifyingContent = false;
        private float tabOffset = 0f;

        private const float splitSize = 1f;
        private const float splitGrabSize = 8f;

        public DockNode(DockNode parent)
        {
            this.parent = parent;

            Gui = parent.Gui;
            Gui = parent.Gui;
            Imgui = parent.Imgui;
            Mode = Modes.Docked;
            Window = parent.Window;
            Batcher = parent.Batcher;
        }

        public DockNode(Gui gui, Modes mode, Rect? bounds = null)
        {
            Gui = gui;
            Imgui = Gui.Imgui;
            Mode = mode;

            if (mode == Modes.Standalone)
            {
                if (bounds == null)
                    throw new Exception("Bounds is required for a Standalone dock node");

                var rounded = bounds.Value.Int();
                var width = rounded.Width;
                var height = rounded.Height;
                var flags = WindowFlags.Hidden;

                // create the standalone batcher
                Batcher = new Batch2D();

                // create the standalone window
                Window = App.System.CreateWindow("Gui Dock", width, height, flags);
                Window.Position = rounded.TopLeft;
                Window.VSync = false;
                Window.Bordered = false;
                Window.OnRender = () => Batcher.Render();
                Window.OnResize = () =>
                {
                    Window.Render();
                    Window.Present();
                };
                Window.OnClose = () => Discard();
                Window.Visible = true;

                Gui.Standalone.Add(this);
            }
            else
            {
                Window = Gui.Window;
                Batcher = Gui.Batcher;

                if (mode == Modes.Floating)
                {
                    if (bounds == null)
                        throw new Exception("Bounds is required for a Floating dock node");

                    floatingBounds = bounds.Value;
                    Gui.Floating.Add(this);
                }
            }
        }

        public void InsertNode(Placings placing, DockNode node)
        {
            if (placing == Placings.Center && (leftChild != null || rightChild != null))
                throw new Exception("Cannot insert into the center of a Docking Node that is split");

            if (placing == Placings.Center)
            {
                var adding = new List<Panel>();
                node.AllChildren(adding);

                foreach (var panel in adding)
                {
                    panel.Node?.RemovePanel(panel);
                    InsertPanel(Placings.Center, panel);
                }
            }
            else
            {
                modifyingContent = true;

                var nextLeft = new DockNode(this);
                var nextRight = new DockNode(this);

                (placing.IsTopLeft() ? nextLeft : nextRight).TakeContent(node);
                (placing.IsTopLeft() ? nextRight : nextLeft).TakeContent(this);

                leftChild = nextLeft;
                rightChild = nextRight;

                splitHorizontally = (placing == Placings.Left || placing == Placings.Right);
                splitPoint = 0.5f;

                modifyingContent = false;
            }
        }

        public void InsertPanel(Placings placing, Panel panel)
        {
            if (placing == Placings.Center && (leftChild != null || rightChild != null))
                throw new Exception("Cannot insert into the center of a Docking Node that is split");

            if (panel.Node != null)
                throw new Exception("Panel is already docked to a Node");

            if (placing == Placings.Center)
            {
                panels.Add(panel);
                panel.Node = this;
                activePanel = panel;
            }
            else
            {
                modifyingContent = true;

                var nextLeft = new DockNode(this);
                var nextRight = new DockNode(this);

                (placing.IsTopLeft() ? nextLeft : nextRight).InsertPanel(Placings.Center, panel);
                (placing.IsTopLeft() ? nextRight : nextLeft).TakeContent(this);

                leftChild = nextLeft;
                rightChild = nextRight;

                splitHorizontally = (placing == Placings.Left || placing == Placings.Right);
                splitPoint = 0.5f;

                modifyingContent = false;
            }
        }

        public void PopoutPanel(Panel panel)
        {
            if (panel.Node == this)
            {
                var baseNode = Base;

                if (baseNode.Mode == Modes.Standalone)
                {
                    var rect = BoundsToScreen(Bounds);
                    var node = new DockNode(Gui, Modes.Standalone, rect);

                    RemovePanel(panel);
                    node.InsertPanel(Placings.Center, panel);
                }
                else
                {
                    var node = new DockNode(Gui, Modes.Floating, Bounds);

                    RemovePanel(panel);
                    node.InsertPanel(Placings.Center, panel);
                }
            }
        }

        public void RemovePanel(Panel panel)
        {
            if (panel.Node == this)
            {
                if (activePanel == panel)
                {
                    if (panels.Count <= 1)
                        activePanel = null;
                    else
                        activePanel = panels[Math.Max(0, panels.IndexOf(activePanel) - 1)];
                }

                panels.Remove(panel);
                panel.Node = null;

                TryDiscard();
            }
        }

        public void TakeContent(DockNode absorbing)
        {
            leftChild = absorbing.leftChild;
            rightChild = absorbing.rightChild;
            tabOffset = absorbing.tabOffset;

            if (leftChild != null)
                leftChild.parent = this;

            if (rightChild != null)
                rightChild.parent = this;

            splitHorizontally = absorbing.splitHorizontally;
            splitPoint = absorbing.splitPoint;

            activePanel = absorbing.activePanel;

            while (absorbing.panels.Count > 0)
            {
                var panel = absorbing.panels[0];
                absorbing.RemovePanel(panel);
                InsertPanel(Placings.Center, panel);
            }

            absorbing.leftChild = null;
            absorbing.rightChild = null;
            absorbing.TryDiscard();
        }

        private void TryDiscard()
        {
            // we have no more panels so we shouldn't exist ...
            if (panels.Count <= 0 && leftChild == null && rightChild == null && !modifyingContent)
                Discard();
        }

        private void Discard()
        {
            if (Mode == Modes.Docked)
            {
                if (parent != null)
                {
                    DockNode? absorbing = null;
                    if (parent.leftChild == this)
                        absorbing = parent.rightChild;
                    else if (parent.rightChild == this)
                        absorbing = parent.leftChild;

                    if (absorbing != null)
                        parent.TakeContent(absorbing);
                }
            }
            else if (Mode == Modes.Standalone)
            {
                Window.Close();
                Batcher.DefaultShader.Dispose();
                Batcher.Mesh.Dispose();
                Gui.Standalone.Remove(this);
            }
            else if (Mode == Modes.Floating)
            {
                Gui.Floating.Remove(this);
            }
            else if (Mode == Modes.Root)
            {
                // .. we just chill
            }
        }

        public Rect Bounds
        {
            get
            {
                if (Mode == Modes.Docked && parent != null)
                {
                    var bounds = parent.Bounds;
                    var split = parent.splitPoint;
                    var size = splitSize / 2f;

                    if (parent.splitHorizontally)
                    {
                        if (parent.leftChild == this)
                            return new Rect(bounds.X, bounds.Y, bounds.Width * split - size, bounds.Height);

                        if (parent.rightChild == this)
                            return new Rect(bounds.X + bounds.Width * split + size, bounds.Y, bounds.Width * (1 - split) - size, bounds.Height);
                    }
                    else
                    {
                        if (parent.leftChild == this)
                            return new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * split - size);

                        if (parent.rightChild == this)
                            return new Rect(bounds.X, bounds.Y + bounds.Height * split + size, bounds.Width, bounds.Height * (1 - split) - size);
                    }
                }
                else if (Mode == Modes.Floating)
                {
                    return floatingBounds;
                }
                else if (Mode == Modes.Standalone)
                {
                    var scale = Window.ContentScale * Gui.ContentScale;
                    var bounds = new Rect(0, 0, Window.DrawableWidth / scale.X, Window.DrawableHeight / scale.Y);

                    return bounds;
                }
                else if (Mode == Modes.Root)
                {
                    var scale = Window.ContentScale * Gui.ContentScale;
                    var bounds = new Rect(0, 0, Window.DrawableWidth / scale.X, Window.DrawableHeight / scale.Y);

                    return bounds;
                }

                return new Rect();
            }
        }

        public DockNode Base
        {
            get
            {
                if (parent != null)
                    return parent.Base;
                return this;
            }
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

        private bool IsChildOf(DockNode? dock)
        {
            if (dock != null)
            {
                DockNode? current = this;
                while (current != null)
                {
                    if (current == dock)
                        return true;
                    current = current.parent;
                }
            }

            return false;
        }

        private void AllChildren(List<Panel> append)
        {
            foreach (var panel in panels)
                append.Add(panel);

            leftChild?.AllChildren(append);
            rightChild?.AllChildren(append);
        }

        public void Positioning()
        {
            if (dragging != Dragging.None)
            {
                if (Mode == Modes.Standalone)
                {
                    var drag = Gui.ScreenMouseDrag;
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
                        if (Gui.Window.Bounds.Contains(Window.Bounds))
                        {
                            var rect = ScreenToBounds(Gui.Window, Window.Bounds);
                            var dock = new DockNode(Gui, Modes.Floating, new Rect(rect.X, rect.Y, rect.Width, rect.Height));

                            dock.ID = ID;
                            dock.TakeContent(this);
                        }
                    }
                }
                else if (Mode == Modes.Floating)
                {
                    var drag = Gui.PixelMouseDrag / (Window.ContentScale * Gui.ContentScale);
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
                        if (App.System.SupportsMultipleWindows && !Gui.Window.Bounds.Contains(BoundsToScreen(floatingBounds)))
                        {
                            try
                            {
                                var rect = BoundsToScreen(floatingBounds);
                                var dock = new DockNode(Gui, Modes.Standalone, rect);

                                dock.ID = ID;
                                dock.TakeContent(this);
                            }
                            catch
                            {
                                // something stopped us from being able to create a window ...
                                // I've noticed this happen before due to some kind of OS issue
                                // (ex. if you create/destroy windows over and over again very fast)
                                Console.WriteLine("Failed to create Standalone Window");
                            }
                        }
                    }
                }
            }

            dragging = Dragging.None;
        }

        public void Refresh()
        {
            var bounds = Bounds;

            // Begin
            if (Mode == Modes.Standalone)
            {
                Batcher.Clear();
                Batcher.Rect(Window.DrawableBounds, Color.Black);
                Imgui.BeginViewport(Window, Batcher, Gui.ContentScale);
            }

            // Display
            {
                // Split Content
                if (leftChild != null || rightChild != null)
                {
                    leftChild?.Refresh();
                    rightChild?.Refresh();

                    if (splitHorizontally)
                    {
                        var split = new Rect(bounds.X + bounds.Width * splitPoint - splitGrabSize / 2, bounds.Y + 1, splitGrabSize, bounds.Height - 2);
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 5), split))
                        {
                            var world = Math.Clamp(bounds.Width * splitPoint + Imgui.Viewport.MouseDelta.X, 64, bounds.Width - 64);
                            splitPoint = world / bounds.Width;
                        }

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.HorizontalResize;
                    }
                    else
                    {
                        var split = new Rect(bounds.X + 1, bounds.Y + bounds.Height * splitPoint - splitGrabSize / 2, bounds.Width - 2, splitGrabSize);
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 6), split))
                        {

                            var world = Math.Clamp(bounds.Height * splitPoint + Imgui.Viewport.MouseDelta.Y, 64, bounds.Height - 64);
                            splitPoint = world / bounds.Height;
                        }

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.VerticalResize;
                    }

                }
                // Main Content (Panels)
                else
                {
                    if (bounds.Contains(Imgui.Viewport.Mouse) && !IsChildOf(Gui.Dragging))
                        Gui.NextDockable = this;

                    if (panels.Count > 0)
                    {
                        if (activePanel == null || !panels.Contains(activePanel))
                            activePanel = panels[0];

                        // The Window
                        if (Imgui.BeginFrame(ID, bounds, Imgui.Style.Window.Window, false))
                        {
                            var windowId = Imgui.CurrentId;
                            var frameId = windowId;
                            var grabbingOffset = 0f;
                            Panel? grabbingPanel = null;

                            // The Tabs
                            {
                                Imgui.Row(panels.Count + (tabOffset > 0 ? 1 : 0));
                                Imgui.PushSpacing(Imgui.Style.Window.TabSpacing);

                                if (tabOffset > 0)
                                    Imgui.Cell(tabOffset, 1);

                                foreach (var panel in panels)
                                {
                                    var tabStyle = (panel == activePanel ? Imgui.Style.Window.CurrentTab : Imgui.Style.Window.Tab);

                                    if (Imgui.Button(panel.Title, new Text(panel.Title), Sizing.Preferred(), tabStyle))
                                        activePanel = panel;

                                    if (Imgui.ActiveId == Imgui.CurrentId)
                                    {
                                        grabbingPanel = panel;
                                        grabbingOffset = Imgui.LastCell.X - Imgui.Frame.Bounds.X;
                                    }
                                }

                                Imgui.PopSpacing();
                            }

                            // The Content
                            if (activePanel != null)
                            {
                                Imgui.PushSpacing(0);
                                var remainder = Imgui.Remainder();
                                Imgui.PopSpacing();

                                var style = Imgui.Style.Window.Frame;
                                style.Padding = activePanel.Padding;

                                // we push a fresh ID here so that it doesn't car what its parent ID is
                                // we also push a storage so that if this panel is destroyed, its storage will also be disposed
                                Imgui.PushId(new Imgui.ID(activePanel.ID));
                                Imgui.BeginStorage(0);
                                if (Imgui.BeginFrame(0, remainder, style, true))
                                {
                                    frameId = Imgui.CurrentId;
                                    activePanel.Refresh(Imgui);
                                    Imgui.EndFrame();
                                }
                                Imgui.EndStorage();
                                Imgui.PopId();
                            }

                            Imgui.EndFrame();

                            // handle dragging
                            if (Imgui.ActiveId == windowId || Imgui.ActiveId == frameId || grabbingPanel != null)
                            {
                                var baseNode = Base;

                                if (grabbingPanel != null && (panels.Count > 1 || baseNode != this || baseNode.Mode == Modes.Root) && Gui.PixelMouseDrag.Length > 0)
                                {
                                    grabbingPanel.Popout();
                                    grabbingPanel.Node!.tabOffset = grabbingOffset;
                                    Imgui.ActiveId = Imgui.Id(grabbingPanel.Node!.ID);
                                }
                                else
                                {
                                    baseNode.dragging = Dragging.Position;
                                    Gui.Dragging = baseNode;
                                }
                            }
                            else
                            {
                                tabOffset = Calc.Approach(tabOffset, 0, Time.Delta * 1200f);
                            }
                        }
                    }
                }
            }

            // Window Resizing
            {
                if (Mode == Modes.Standalone || Mode == Modes.Floating)
                {
                    // left
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 11), new Rect(bounds.X - 3, bounds.Y, 6, bounds.Height)))
                            dragging = Dragging.Left;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.HorizontalResize;
                    }

                    // top
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 12), new Rect(bounds.X, bounds.Y - 3, bounds.Width, 6)))
                            dragging = Dragging.Top;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.VerticalResize;
                    }

                    // right
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 13), new Rect(bounds.Right - 3, bounds.Y, 6, bounds.Height)))
                            dragging = Dragging.Right;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.HorizontalResize;
                    }

                    // bottom
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 14), new Rect(bounds.X, bounds.Bottom - 3, bounds.Width, 6)))
                            dragging = Dragging.Bottom;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.VerticalResize;
                    }

                    // top left
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 15), new Rect(bounds.Left - 3, bounds.Top - 3, 6, 6)))
                            dragging = Dragging.Top | Dragging.Left;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.Crosshair;
                    }

                    // top right
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 16), new Rect(bounds.Right - 3, bounds.Top - 3, 6, 6)))
                            dragging = Dragging.Top | Dragging.Right;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.Crosshair;
                    }

                    // bottom right
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 17), new Rect(bounds.Right - 3, bounds.Bottom - 3, 6, 6)))
                            dragging = Dragging.Bottom | Dragging.Right;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.Crosshair;
                    }

                    // bottom left
                    {
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 18), new Rect(bounds.Left - 3, bounds.Bottom - 3, 6, 6)))
                            dragging = Dragging.Bottom | Dragging.Left;

                        if (Imgui.HoveringOrDragging(Imgui.CurrentId))
                            Gui.NextCursor = Cursors.Crosshair;
                    }
                }
            }

            // End
            {
                if (Mode == Modes.Standalone)
                {
                    DockingGui();
                    Imgui.EndViewport();
                }
            }
        }

        public void DockingGui()
        {
            if (Gui.LastDockable != null && Gui.Dragging != null && !Gui.LastDockable.IsChildOf(Gui.Dragging))
            {
                var other = Gui.LastDockable.Window;
                var splitable = Gui.LastDockable.Mode != Modes.Root || Gui.LastDockable.panels.Count > 0;

                // get the offset relative to the docking window
                var offset = other.Position * other.DrawableScale / (other.ContentScale * Gui.ContentScale) -
                             Window.Position * Window.DrawableScale / (Window.ContentScale * Gui.ContentScale);

                // get the bounds of the docking window
                var bounds = Gui.LastDockable.Bounds;
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

                DockHover(fill, Placings.Center);

                if (splitable)
                {
                    DockHover(left, Placings.Left);
                    DockHover(right, Placings.Right);
                    DockHover(top, Placings.Top);
                    DockHover(bottom, Placings.Bottom);
                }

                DockButton(fill, Placings.Center);

                if (splitable)
                {
                    DockButton(left, Placings.Left);
                    DockButton(right, Placings.Right);
                    DockButton(top, Placings.Top);
                    DockButton(bottom, Placings.Bottom);
                }

                void DockButton(Rect rect, Placings split)
                {
                    if (Gui.Dragging != null)
                    {
                        var hover = false;

                        Batcher.Rect(rect.Inflate(-3), back);

                        if (rect.Contains(Imgui.Viewport.Mouse))
                        {
                            hover = true;

                            if (!App.Input.Mouse.Down(MouseButtons.Left) && Gui.LastDockable != null)
                            {
                                Gui.LastDockable.InsertNode(split, Gui.Dragging);
                                Gui.LastDockable = null;
                            }
                        }

                        var line = color * 0.75f;
                        if (hover)
                            line = Color.Lerp(color, Color.White, 0.5f);

                        var box = rect.Inflate(-4f);
                        Batcher.HollowRect(box, 1f, line);
                        Batcher.Rect(box.X + 1, box.Y + 1, box.Width - 2, 4f, line);

                        if (split == Placings.Left || split == Placings.Right)
                            Batcher.Rect(box.Center.X - 0.5f, box.Y + 6, 1f, box.Height - 7, line);
                        if (split == Placings.Top || split == Placings.Bottom)
                            Batcher.Rect(box.X + 1, box.Center.Y - 0.5f, box.Width - 2f, 1f, line);
                    }
                }

                void DockHover(Rect rect, Placings split)
                {
                    if (Gui.Dragging != null && rect.Contains(Imgui.Viewport.Mouse))
                    {
                        var fill = bounds;

                        if (split == Placings.Left)
                            fill = new Rect(bounds.X, bounds.Y, bounds.Width * 0.5f, bounds.Height);
                        else if (split == Placings.Right)
                            fill = new Rect(bounds.X + bounds.Width * 0.5f, bounds.Y, bounds.Width * 0.5f, bounds.Height);
                        else if (split == Placings.Top)
                            fill = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * 0.5f);
                        else if (split == Placings.Bottom)
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
    }

    internal static class GuiDockNodePlacingExt
    {
        public static bool IsTopLeft(this DockNode.Placings placing) => placing == DockNode.Placings.Left || placing == DockNode.Placings.Top;
    }
}
