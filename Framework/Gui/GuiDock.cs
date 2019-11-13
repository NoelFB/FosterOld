using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Foster.Framework
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

        #endregion

        #region Public Variables

        public readonly int ID;
        public readonly Gui Gui;
        public readonly GuiManager Manager;

        public Imgui Imgui => Gui.Imgui;
        public Modes Mode { get; private set; } = Modes.None;

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
        private const int windowMinWidth = 100;
        private const int windowMinHeight = 64;

        private GuiDock? parent;
        private GuiDock? left;
        private GuiDock? right;

        private Window? standaloneWindow;
        private Batch2d? standaloneBatcher;
        private bool windowDragging;
        private bool windowMoving;
        private Point2 windowTopLeft;
        private Point2 windowBottomRight;
        private RectInt floatingBounds;

        private Vector2 draggingOffset;

        #endregion

        #region Constructor

        public GuiDock(GuiManager manager)
        {
            Gui = manager.Gui;
            Manager = manager;
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

                parent = null;
                Mode = Modes.Root;
            }
        }

        public void SetAsFloating(Rect bounds)
        {
            if (Mode != Modes.Floating)
            {
                UnsetLastMode();

                Manager.Floating.Add(this);

                parent = null;
                Mode = Modes.Floating;
                floatingBounds = bounds.Int();
            }
        }

        public void SetAsStandalone(Rect bounds)
        {
            if (Mode != Modes.Standalone)
            {
                if (App.System.SupportsMultipleWindows)
                {
                    UnsetLastMode();

                    parent = null;
                    Mode = Modes.Standalone;
                    Manager.Standalone.Add(this);

                    standaloneWindow = App.System.CreateWindow("Gui Dock", (int)bounds.Width, (int)bounds.Height, false);
                    standaloneWindow.X = (int)bounds.X;
                    standaloneWindow.Y = (int)bounds.Y;
                    standaloneWindow.VSync = false;
                    standaloneWindow.Bordered = false;
                    standaloneWindow.OnResize = (w, h) =>
                    {
                        if (Imgui.ActiveViewport.ID == Imgui.ID.None && !windowDragging)
                        {
                            Refresh();
                            standaloneWindow.Render();
                            standaloneWindow.Present();
                        }
                    };
                    standaloneWindow.OnRender = () =>
                    {
                        standaloneBatcher?.Render();
                    };
                    standaloneWindow.OnClose = () =>
                    {
                        if (Mode == Modes.Standalone)
                            UnsetLastMode();
                    };

                    standaloneWindow.Visible = true;
                    standaloneBatcher = new Batch2d();
                    windowDragging = false;
                    windowMoving = false;
                    windowTopLeft = standaloneWindow.Bounds.TopLeft;
                    windowBottomRight = standaloneWindow.Bounds.BottomRight;
                }
                else
                {
                    SetAsFloating(new Rect(0, 0, bounds.Width, bounds.Height));
                }
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
                if (standaloneWindow != null)
                {
                    standaloneWindow.OnClose = null;
                    standaloneWindow.OnResize = null;
                    standaloneWindow.OnRender = null;
                    standaloneWindow.Close();
                    standaloneWindow = null;
                }

                if (standaloneBatcher != null)
                {
                    standaloneBatcher.Dispose();
                    standaloneBatcher = null;
                }

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

        public Batch2d GetBatcher()
        {
            if (Mode == Modes.Docked && Parent != null)
                return Parent.GetBatcher();
            if (Mode == Modes.Floating || Mode == Modes.Root)
                return Manager.Batcher;
            if (Mode == Modes.Standalone && standaloneBatcher != null)
                return standaloneBatcher;

            throw new Exception("Gui Dock doesn't have a Batcher");
        }

        public Window GetWindow()
        {
            if (Mode == Modes.Docked && Parent != null)
                return Parent.GetWindow();
            if (Mode == Modes.Floating || Mode == Modes.Root)
                return Manager.Window;
            if (Mode == Modes.Standalone && standaloneWindow != null)
                return standaloneWindow;

            throw new Exception("Gui Dock doesn't have a Window");
        }

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
                return GetWindow().ContentBounds;
            }

            return new Rect();
        }

        public GuiDock GetBaseDock()
        {
            if (Parent == null)
                return this;
            return Parent.GetBaseDock();
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

        #endregion

        #region Refresh

        public void UpdateWindow()
        {
            if (Mode == Modes.Standalone)
            {
                var window = GetWindow();

                if (windowMoving)
                {
                    window.Position = windowTopLeft;
                }
                else if (windowDragging)
                {
                    var resized = new RectInt();
                    resized.TopLeft = windowTopLeft;
                    resized.BottomRight = windowBottomRight;
                    window.Bounds = resized;
                }

                windowTopLeft = window.Bounds.TopLeft;
                windowBottomRight = window.Bounds.BottomRight;
            }
            else if (Mode == Modes.Floating)
            {
                if (windowMoving)
                {
                    floatingBounds.Position = windowTopLeft;
                }
                else if (windowDragging)
                {
                    var resized = new RectInt();
                    resized.TopLeft = windowTopLeft;
                    resized.BottomRight = windowBottomRight;
                    floatingBounds = resized;
                }

                windowTopLeft = floatingBounds.TopLeft;
                windowBottomRight = floatingBounds.BottomRight;
            }

            windowDragging = false;
            windowMoving = false;
        }

        public void Refresh()
        {
            var batcher = GetBatcher();
            var window = GetWindow();
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
                    batcher.Clear();
                    Imgui.BeginViewport(ID, batcher, window.ContentBounds, window.Mouse, window.PixelScale, !window.MouseOver);
                }
            }

            void Display()
            {
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
                            var world = Math.Clamp(bounds.Width * SplitPoint + Imgui.ActiveMouseDelta.X, 64, bounds.Width - 64);
                            SplitPoint = world / bounds.Width;
                        }
                    }
                    else
                    {
                        var split = new Rect(bounds.X + 1, bounds.Y + bounds.Height * SplitPoint - splitGrabSize / 2, bounds.Width - 2, splitGrabSize);
                        if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 10), split))
                        {
                            var world = Math.Clamp(bounds.Height * SplitPoint + Imgui.ActiveMouseDelta.Y, 64, bounds.Height - 64);
                            SplitPoint = world / bounds.Height;
                        }
                    }

                }
                // Main Content (Panels)
                else
                {
                    if (bounds.Contains(Imgui.ActiveMouse) && !IsChildOf(Manager.Dragging))
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

                            for (int i = 0; i < Panels.Count; i++)
                            {
                                if (Imgui.Button(Panels[i].Title, Imgui.PreferredSize))
                                    PanelIndex = i;

                                // if they grab a single tab
                                HandleDragging(bounds, Panels.Count <= 1 && Parent == null, Panels[i]);
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

                if (mode == Modes.Standalone || mode == Modes.Floating)
                {
                    batcher.Rect(bounds.X, bounds.Y, bounds.Width, 1, Color.Red);
                    batcher.Rect(bounds.X, bounds.Y + bounds.Height - 1, bounds.Width, 1, Color.Red);
                    batcher.Rect(bounds.X, bounds.Y, 1, bounds.Height, Color.Red);
                    batcher.Rect(bounds.X + bounds.Width - 1, bounds.Y, 1, bounds.Height, Color.Red);
                }
            }

            void Resizing()
            {
                if (mode == Modes.Standalone || mode == Modes.Floating)
                {
                    if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 11), new Rect(bounds.X, bounds.Y, 8, bounds.Height)))
                    {
                        var draggingBy = GetDraggingDelta();
                        windowDragging = true;
                        windowTopLeft.X += draggingBy.X;

                        if (mode == Modes.Standalone)
                            Imgui.ActiveMouse -= draggingBy;
                    }

                    if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 13), new Rect(bounds.Right - 8, bounds.Y, 8, bounds.Height)))
                    {
                        var draggingBy = GetDraggingDelta();
                        windowDragging = true;
                        windowBottomRight.X += draggingBy.X;

                        if (mode == Modes.Standalone)
                            floatingBounds.Right += draggingBy.X;
                    }

                    if (Imgui.GrabbingBehaviour(Imgui.Id(ID + 15), new Rect(bounds.Right - 8, bounds.Bottom - 8, 8, 8)))
                    {
                        var draggingBy = GetDraggingDelta();
                        windowDragging = true;
                        windowBottomRight += draggingBy;
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

        private Point2 GetDraggingDelta()
        {
            // reset our floating drag offset
            if (Imgui.LastActiveId != Imgui.ActiveId)
                draggingOffset = Vector2.Zero;

            // get the move by amount as an integer
            draggingOffset += Imgui.ActiveViewport.MouseDelta;
            var draggingBy = draggingOffset.Floor();
            draggingOffset -= draggingBy;

            return draggingBy;
        }

        private void HandleDragging(Rect bounds, bool draggingWindow, GuiPanel? draggingSinglePanel)
        {
            if (Imgui.CurrentId == Imgui.ActiveId)
            {
                var draggingBy = GetDraggingDelta();

                // only move if we need to
                if (draggingBy.Length > 0)
                {
                    var window = GetWindow();
                    var root = GetBaseDock();

                    // drag out a single panel from a list of panels
                    if (draggingSinglePanel != null && Panels.Count > 1)
                    {
                        var dock = new GuiDock(Manager);
                        dock.SetAsStandalone(new Rect(window.X + bounds.X, window.Y + bounds.Y, bounds.Width, bounds.Height));
                        dock.Panels.Add(draggingSinglePanel);
                        Panels.Remove(draggingSinglePanel);
                    }
                    // pop out of the root
                    // The Root is allowed to be totally empty
                    else if (Mode == Modes.Root)
                    {
                        var dock = new GuiDock(Manager);
                        dock.SetAsStandalone(new Rect(window.X + bounds.X, window.Y + bounds.Y, bounds.Width, bounds.Height));
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

                        root.windowMoving = true;
                        root.windowTopLeft += draggingBy;

                        Imgui.ActiveMouse -= draggingBy;
                    }
                    // drag the floating Window around normally
                    else if (draggingWindow && root.Mode == Modes.Floating)
                    {
                        Manager.Dragging = root;

                        root.windowMoving = true;
                        root.windowTopLeft += draggingBy;

                        // pop out of the frame if it doesn't fit
                        if (!Imgui.ActiveViewport.Bounds.Contains(root.floatingBounds))
                        {
                            root.SetAsStandalone(window.Position + root.floatingBounds);
                        }
                    }
                    // pop out of our parent
                    else
                    {
                        SetAsStandalone(new Rect(window.X + bounds.X, window.Y + bounds.Y, bounds.Width, bounds.Height));
                    }
                }
            }
        }

        public void DockingGui()
        {
            if (Manager.LastDockable != null && Manager.Dragging != null && !Manager.LastDockable.IsChildOf(Manager.Dragging))
            {
                var batcher = GetBatcher();
                var other = Manager.LastDockable.GetWindow();
                var splitable = Manager.LastDockable.Mode != Modes.Root || Manager.LastDockable.Panels.Count > 0;

                // get the offset relative to the docking window
                Vector2 offset;
                if (Mode == Modes.Standalone)
                {
                    offset = other.Position - windowTopLeft;
                }
                else
                {
                    var window = GetWindow();
                    offset = other.Position - window.Position;
                }

                // get the bounds of the docking window
                var bounds = Manager.LastDockable.GetContentBounds();
                bounds.X += offset.X;
                bounds.Y += offset.Y;

                var center = bounds.Center.Floor();
                var fill = new Rect(center.X - 16, center.Y - 16, 32, 32);
                var left = new Rect(center.X - 16 - 34, center.Y - 16, 32, 32);
                var right = new Rect(center.X - 16 + 34, center.Y - 16, 32, 32);
                var top = new Rect(center.X - 16, center.Y - 16 - 34, 32, 32);
                var bottom = new Rect(center.X - 16, center.Y - 16 + 34, 32, 32);

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
                        batcher.Rect(rect.Inflate(1), Color.White);
                        batcher.Rect(rect, Color.Blue);

                        if (rect.Contains(Imgui.ActiveMouse))
                        {
                            batcher.Rect(rect, Color.White * 0.5f);

                            if (!App.Input.Mouse.Down(MouseButtons.Left))
                            {
                                Manager.Dragging.SetAsDock(Manager.LastDockable!, split);
                                Manager.Dragging = null;
                            }
                        }
                    }
                }

                void DockHover(Rect rect, DockTo split)
                {
                    if (Manager.Dragging != null && rect.Contains(Imgui.ActiveMouse))
                    {
                        var color = Color.White * 0.5f;
                        if (split == DockTo.Fill)
                            batcher.Rect(bounds, color);
                        else if (split == DockTo.Left)
                            batcher.Rect(new Rect(bounds.X, bounds.Y, bounds.Width * 0.5f, bounds.Height), color);
                        else if (split == DockTo.Right)
                            batcher.Rect(new Rect(bounds.X + bounds.Width * 0.5f, bounds.Y, bounds.Width * 0.5f, bounds.Height), color);
                        else if (split == DockTo.Top)
                            batcher.Rect(new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * 0.5f), color);
                        else if (split == DockTo.Bottom)
                            batcher.Rect(new Rect(bounds.X, bounds.Y + bounds.Height * 0.5f, bounds.Width, bounds.Height * 0.5f), color);
                    }
                }
            }
        }

        #endregion
    }
}
