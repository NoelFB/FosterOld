using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Foster.Framework
{
    public class GuiDock
    {

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

        public readonly int ID;
        public readonly Gui Gui;
        public Imgui Imgui => Gui.Imgui;

        public Modes Mode { get; private set; } = Modes.None;
        public GuiDock? Parent { get; private set; }
        public GuiDock? Left { get; private set; }
        public GuiDock? Right { get; private set; }

        public float SplitPoint = 0.5f;
        public bool SplitHorizontally = true;

        public readonly List<GuiPanel> Panels = new List<GuiPanel>();
        public int PanelIndex;
        public GuiPanel? Panel => PanelIndex >= 0 && PanelIndex < Panels.Count ? Panels[PanelIndex] : null;

        private Window? standaloneWindow;
        private Batch2d? standaloneBatcher;
        private Rect floatingBounds;
        private Vector2 draggingOffset;

        public GuiDock(Gui gui)
        {
            Gui = gui;
            ID = Guid.NewGuid().GetHashCode();
        }

        public void SetAsRoot()
        {
            if (Mode != Modes.Root)
            {
                UnsetLastMode();

                if (Gui.Root != null && Gui.Root != this)
                    throw new Exception("Workspace already has a Root Dock");

                Parent = null;
                Mode = Modes.Root;
            }
        }

        public void SetAsFloating(Rect bounds)
        {
            if (Mode != Modes.Floating)
            {
                UnsetLastMode();

                Gui.Floating.Add(this);

                Parent = null;
                Mode = Modes.Floating;
                floatingBounds = bounds;
            }
        }

        public void SetAsStandalone(Rect bounds)
        {
            if (Mode != Modes.Standalone)
            {
                UnsetLastMode();

                Parent = null;
                Mode = Modes.Standalone;
                Gui.Standalone.Add(this);

                standaloneWindow = App.System.CreateWindow("Gui Dock", (int)bounds.Width, (int)bounds.Height);
                standaloneWindow.X = (int)bounds.X;
                standaloneWindow.Y = (int)bounds.Y;
                standaloneWindow.VSync = false;
                standaloneWindow.Bordered = false;
                standaloneWindow.OnResize = (w, h) =>
                {
                    Refresh();
                    standaloneWindow.Render();
                    standaloneWindow.Present();
                };
                standaloneWindow.OnRender = () =>
                {
                    GetBatcher()?.Render();
                };
                standaloneWindow.OnClose = () =>
                {
                    if (Mode == Modes.Standalone)
                        UnsetLastMode();
                };

                standaloneBatcher = new Batch2d();
            }
        }

        public void SetAsDock(GuiDock parent, DockTo split)
        {
            if (Mode == Modes.Docked)
                throw new Exception("This Dock is already Docked");

            if (IsChildOf(parent))
                throw new Exception("Parented to this element already");

            UnsetLastMode();

            if (split == DockTo.Fill)
            {
                if (parent.Left != null || parent.Right != null)
                    throw new Exception("Can't fill a Dock that is already split");

                if (parent.Panels.Count <= 0)
                {
                    parent.Left = Left;
                    if (parent.Left != null)
                        parent.Left.Parent = parent;
                    parent.Right = Right;
                    if (parent.Right != null)
                        parent.Right.Parent = parent;
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

                var currentLeft = parent.Left;
                var currentRight = parent.Right;

                GuiDock? other = null;

                // we must subdivide since the parent already has a left/right dock
                if (currentLeft != null || currentRight != null)
                {
                    other = new GuiDock(Gui);

                    other.Left = currentLeft;
                    if (other.Left != null)
                        other.Left.Parent = other;

                    other.Right = currentRight;
                    if (other.Right != null)
                        other.Right.Parent = other;

                    other.Parent = parent;
                    other.Mode = Modes.Docked;
                    other.SplitHorizontally = parent.SplitHorizontally;
                    other.SplitPoint = parent.SplitPoint;
                }
                // move parent contents to a new child
                else
                {
                    other = new GuiDock(Gui);
                    other.Panels.AddRange(parent.Panels);
                    other.PanelIndex = parent.PanelIndex;
                    other.Parent = parent;
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

                Parent = parent;
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
                Gui.Floating.Remove(this);
            }
            else if (Mode == Modes.Standalone)
            {
                standaloneWindow?.Close();
                Gui.Standalone.Remove(this);
                standaloneWindow = null;

                standaloneBatcher?.Dispose();
                standaloneBatcher = null;
            }
            else if (Mode == Modes.Docked)
            {
                if (Parent != null)
                {
                    GuiDock? other = null;
                    if (Parent.Left == this)
                        other = Parent.Right;
                    else if (Parent.Right == this)
                        other = Parent.Left;

                    Parent.Left = null;
                    Parent.Right = null;

                    if (other != null)
                    {
                        Parent.SplitPoint = other.SplitPoint;
                        Parent.SplitHorizontally = other.SplitHorizontally;
                        Parent.Left = other.Left;
                        Parent.Right = other.Right;

                        if (Parent.Left != null)
                            Parent.Left.Parent = Parent;
                        if (Parent.Right != null)
                            Parent.Right.Parent = Parent;

                        Parent.Panels.Clear();
                        Parent.Panels.AddRange(other.Panels);
                    }

                    // parent now also has nothing in it ...
                    if (Parent.Left == null && Parent.Right == null && Parent.Panels.Count <= 0 && Parent.Mode != Modes.Root)
                        Parent.UnsetLastMode();
                }

                Parent = null;
            }

            Mode = Modes.None;
        }

        public Batch2d? GetBatcher()
        {
            if (Mode == Modes.Docked && Parent != null)
                return Parent.GetBatcher();
            if (Mode == Modes.Floating || Mode == Modes.Root)
                return Gui.Batcher;
            if (Mode == Modes.Standalone)
                return standaloneBatcher;
            return null;
        }

        public Window? GetCurrentWindow()
        {
            if (Mode == Modes.Docked && Parent != null)
                return Parent.GetCurrentWindow();
            if (Mode == Modes.Floating || Mode == Modes.Root)
                return Gui.Window;
            if (Mode == Modes.Standalone)
                return standaloneWindow;
            return null;
        }

        public Rect GetContentBounds()
        {
            if (Mode == Modes.Docked)
            {
                var container = Parent!.GetContentBounds();

                if (Parent.SplitHorizontally)
                {
                    var w = container.Width - 12;
                    if (Parent.Left == this)
                        return new Rect(container.X, container.Y, w * Parent.SplitPoint, container.Height);
                    else if (Parent.Right == this)
                        return new Rect(container.Right - w * (1f - Parent.SplitPoint), container.Y, w * (1f - Parent.SplitPoint), container.Height);
                }
                else
                {
                    var h = container.Height - 12;
                    if (Parent.Left == this)
                        return new Rect(container.X, container.Y, container.Width, h * Parent.SplitPoint);
                    else if (Parent.Right == this)
                        return new Rect(container.X, container.Bottom - h * (1f - Parent.SplitPoint), container.Width, h * (1f - Parent.SplitPoint));
                }
            }
            else if (Mode == Modes.Floating)
            {
                return floatingBounds;
            }
            else if (Mode == Modes.Standalone || Mode == Modes.Root)
            {
                var window = GetCurrentWindow();
                if (window != null)
                    return window.ContentBounds;
            }

            return new Rect();
        }

        public Modes GetRootMode()
        {
            if (Parent == null)
                return Mode;
            return Parent.GetRootMode();
        }

        public GuiDock GetRootDock()
        {
            if (Parent == null)
                return this;
            return Parent.GetRootDock();
        }

        public bool IsTopLeftDock()
        {
            if (Parent == null)
                return true;
            else if (Parent.Left == this)
                return Parent.IsTopLeftDock();
            return false;
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

        public void Refresh()
        {
            var batcher = GetBatcher();
            if (batcher == null)
                throw new Exception("Gui Dock has no Batcher");

            var standalone = Mode == Modes.Standalone;
            if (standalone)
            {
                batcher.Clear();

                var window = GetCurrentWindow();
                if (window == null)
                    throw new Exception("Gui Dock has no Window");

                Imgui.BeginViewport(ID, batcher, window.ContentBounds, window.Mouse, window.PixelScale, !window.MouseOver);
            }

            var bounds = GetContentBounds();
            InternalRefresh(batcher, bounds);

            if (standalone)
            {
                Imgui.EndViewport();
            }
        }

        private void InternalRefresh(Batch2d batcher, Rect bounds)
        {
            batcher.Rect(bounds.X, bounds.Y, bounds.Width, 1, Color.Red);
            batcher.Rect(bounds.X, bounds.Y + bounds.Height - 1, bounds.Width, 1, Color.Red);
            batcher.Rect(bounds.X, bounds.Y, 1, bounds.Height, Color.Red);
            batcher.Rect(bounds.X + bounds.Width - 1, bounds.Y, 1, bounds.Height, Color.Red);
            batcher.Rect(bounds.Inflate(-1), Color.Blue);

            if (Left != null || Right != null)
            {
                if (SplitHorizontally)
                    batcher.Rect(bounds.X + bounds.Width * SplitPoint, bounds.Y + 1, 1, bounds.Height - 2, Color.Yellow);
                else
                    batcher.Rect(bounds.X + 1, bounds.Y + bounds.Height * SplitPoint, bounds.Width - 2, 1, Color.Yellow);
            }

            if (Left != null || Right != null)
            {
                Left?.Refresh();
                Right?.Refresh();
            }
            else
            {
                if (bounds.Contains(Imgui.ActiveMouse) && !IsChildOf(Gui.Dragging))
                    Gui.Hot = this;

                if (Panels.Count > 0)
                {
                    while (PanelIndex >= Panels.Count)
                        PanelIndex--;
                    if (PanelIndex < 0)
                        PanelIndex = 0;

                    if (Imgui.BeginFrame(ID + 1, bounds, false))
                    {
                        Imgui.Row(Panels.Count + 1);
                        Imgui.Button("o", 16);

                        HandleDragging(bounds, null);

                        for (int i = 0; i < Panels.Count; i++)
                        {
                            if (Imgui.Button(Panels[i].Title))
                                PanelIndex = i;

                            HandleDragging(bounds, Panels[i]);
                        }

                        if (Panel != null)
                        {
                            if (Imgui.BeginFrame(ID, Imgui.Remainder()))
                            {
                                Panel.OnRefresh?.Invoke(Imgui);
                                Imgui.EndFrame();
                            }
                        }

                        Imgui.EndFrame();
                    }
                }
            }

            if (Gui.LastHot == this && Gui.Dragging != null && !IsChildOf(Gui.Dragging))
            {
                batcher.Rect(bounds, Color.White * 0.25f);

                var center = bounds.Center;

                HoveringDockButton(batcher, new Rect(center.X - 16, center.Y - 16, 32, 32), DockTo.Fill);
                HoveringDockButton(batcher, new Rect(center.X - 16 - 34, center.Y - 16, 32, 32), DockTo.Left);
                HoveringDockButton(batcher, new Rect(center.X - 16 + 34, center.Y - 16, 32, 32), DockTo.Right);
                HoveringDockButton(batcher, new Rect(center.X - 16, center.Y - 16 - 34, 32, 32), DockTo.Top);
                HoveringDockButton(batcher, new Rect(center.X - 16, center.Y - 16 + 34, 32, 32), DockTo.Bottom);
            }
        }

        private void HoveringDockButton(Batch2d batcher, Rect rect, DockTo split)
        {
            if (Gui.LastHot == this && Gui.Dragging != null)
            {
                batcher.Rect(rect.Inflate(1), Color.White);
                batcher.Rect(rect, Color.Blue);

                if (rect.Contains(Imgui.ActiveMouse))
                {
                    batcher.Rect(rect, Color.White * 0.5f);

                    if (!App.Input.Mouse.Down(MouseButtons.Left))
                        Gui.Dragging.SetAsDock(this, split);
                }
            }
        }

        private void HandleDragging(Rect bounds, GuiPanel? singlePanel)
        {
            if (Imgui.CurrentId == Imgui.ActiveId)
            {
                if (Imgui.LastActiveId != Imgui.ActiveId)
                    draggingOffset = Vector2.Zero;

                draggingOffset += Imgui.ActiveViewport.MouseDelta;
                var moveBy = draggingOffset.Floor();
                draggingOffset -= moveBy;

                var window = GetCurrentWindow();
                if (window == null)
                    throw new Exception("Gui Dock Window is null");

                if (moveBy.Length > 0)
                {
                    // drag out a single panel
                    if (singlePanel != null && Panels.Count > 1)
                    {
                        var dock = new GuiDock(Gui);
                        dock.SetAsStandalone(new Rect(window.X + bounds.X, window.Y + bounds.Y, bounds.Width, bounds.Height));
                        dock.Panels.Add(singlePanel);
                        Panels.Remove(singlePanel);
                    }
                    // drag the full window
                    else if (IsTopLeftDock())
                    {
                        var root = GetRootDock();
                        Gui.Dragging = root;

                        var rootMode = GetRootMode();
                        if (rootMode == Modes.Standalone)
                        {
                            Imgui.ActiveMouse -= moveBy;
                            window.Position += moveBy;
                        }
                        else if (rootMode == Modes.Floating)
                        {
                            root.floatingBounds.X += moveBy.X;
                            root.floatingBounds.Y += moveBy.Y;

                            if (!Imgui.ActiveViewport.Bounds.Contains(root.floatingBounds))
                            {
                                if (App.System.SupportsMultipleWindows)
                                {
                                    root.SetAsStandalone(new Rect(window.X + root.floatingBounds.X, window.Y + root.floatingBounds.Y, root.floatingBounds.Width, root.floatingBounds.Height));
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    // pop out of parent
                    else
                    {
                        SetAsStandalone(new Rect(window.X + bounds.X, window.Y + bounds.Y, bounds.Width, bounds.Height));
                    }
                }
            }
        }
    }
}
