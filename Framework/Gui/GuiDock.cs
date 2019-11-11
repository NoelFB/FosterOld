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

        public enum SplitDirection
        {
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
                    UnsetLastMode();
                };

                standaloneBatcher = new Batch2d();
            }
        }

        public void SetAsDock(GuiDock parent, SplitDirection split)
        {
            if (Mode != Modes.Docked || Parent != parent)
            {
                UnsetLastMode();

                var currentLeft = parent.Left;
                var currentRight = parent.Right;

                GuiDock? other = null;

                // there's currently an open side ... and it's the same side we're splitting!
                // so we just hold our existing one and drop in the other
                if (parent.SplitHorizontally && currentLeft == null && currentRight != null && split == SplitDirection.Left)
                {
                    other = currentRight;
                }
                else if (!parent.SplitHorizontally && currentLeft == null && currentRight != null && split == SplitDirection.Top)
                {
                    other = currentRight;
                }
                else if (parent.SplitHorizontally && currentLeft != null && currentRight == null && split == SplitDirection.Right)
                {
                    other = currentLeft;
                }
                else if (!parent.SplitHorizontally && currentLeft != null && currentRight == null && split == SplitDirection.Bottom)
                {
                    other = currentLeft;
                }
                // we must subdivide since the parent already has a left/right dock
                else if (currentLeft != null || currentRight != null)
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
                    parent.PanelIndex = -1;
                }

                if (split == SplitDirection.Left || split == SplitDirection.Top)
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
                parent.SplitHorizontally = (split == SplitDirection.Left || split == SplitDirection.Right);

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
                var container = Parent!.GetContentBounds().Inflate(-10);

                if (Parent.SplitHorizontally)
                {
                    var w = container.Width - 20;
                    if (Parent.Left == this)
                        return new Rect(container.X, container.Y, w * Parent.SplitPoint, container.Height);
                    else if (Parent.Right == this)
                        return new Rect(container.Right - w * (1f - Parent.SplitPoint), container.Y, w * (1f - Parent.SplitPoint), container.Height);
                }
                else
                {
                    var h = container.Height - 20;
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

        public void Refresh()
        {
            var batcher = GetBatcher();
            if (batcher == null)
                throw new Exception("Gui Dock has no Batcher");

            if (Mode == Modes.Standalone)
            {
                batcher.Clear();

                var window = GetCurrentWindow();
                if (window == null)
                    throw new Exception("Gui Dock has no Window");

                Imgui.BeginViewport(window, batcher);
            }

            InternalRefresh(batcher, GetContentBounds());

            if (Mode == Modes.Standalone)
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
                while (PanelIndex >= Panels.Count)
                    PanelIndex--;
                if (PanelIndex < 0)
                    PanelIndex = 0;

                Imgui.BeginFrame(ID, bounds, false);
                {
                    Imgui.Row(Panels.Count);

                    for (int i = 0; i < Panels.Count; i++)
                    {
                        if (Imgui.Button(Panels[i].Title))
                            PanelIndex = i;
                    }

                    if (Panel != null)
                    {
                        Imgui.BeginFrame(ID, Imgui.Remainder());
                        Panel.OnRefresh?.Invoke(Imgui);
                        Imgui.EndFrame();
                    }
                }
                Imgui.EndFrame();
            }
        }
    }
}
