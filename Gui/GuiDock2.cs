using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    internal class GuiDockNode
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

        public readonly Gui Gui;
        public readonly GuiManager Manager;
        public readonly Modes Mode;
        public readonly Window Window;
        public readonly Batch2d Batcher;

        public GuiDockNode? Parent;
        public GuiDockNode? Left;
        public GuiDockNode? Right;

        public float SplitPoint = 0.5f;
        public bool SplitHorizontally = true;

        public List<GuiPanel> Panels = new List<GuiPanel>();
        public GuiPanel? ActivePanel;

        public GuiDockNode(GuiManager mananger, Modes mode)
        {
            Gui = mananger.Gui;
            Manager = mananger;
            Mode = mode;

            if (mode == Modes.Root)
            {
                Window = Manager.Window;
                Batcher = Manager.Batcher;
            }
            else if (mode == Modes.Standalone)
            {

            }
        }

        public void AddPanels(Placings placing, params GuiPanel[] panels)
        {
            // we're already split ...
            if (Left != null && Right != null)
                throw new Exception("Cannot add Panels to a Docking Node that is split");

            // remove panels from their current node
            foreach (var panel in panels)
                panel.Node.RemovePanel(panel);

            // add directly
            if (placing == Placings.Center)
            {
                foreach (var panel in panels)
                {
                    panel.Node = this;
                    Panels.Add(panel);
                }
            }
            else
            {
                if (Left == null)
                {
                    Left = new GuiDockNode(Manager, Modes.Docked);
                    Left.Parent = this;
                }

                if (Right == null)
                {
                    Right = new GuiDockNode(Manager, Modes.Docked);
                    Right.Parent = this;
                }

                if (placing == Placings.Left || placing == Placings.Top)
                {
                    Left.AddPanels(Placings.Center, panels);
                    Right.AddPanels(Placings.Center, Panels.ToArray());
                }
                else
                {
                    Right.AddPanels(Placings.Center, panels);
                    Left.AddPanels(Placings.Center, Panels.ToArray());
                }

                SplitHorizontally = (placing == Placings.Left || placing == Placings.Right);
                SplitPoint = 0.5f;
            }
        }

        public void RemovePanel(GuiPanel panel)
        {
            if (panel.Node == this)
            {
                Panels.Remove(panel);
                panel.Node = null;

                // we have no more panels so we shouldn't exist ...
                if (Panels.Count <= 0)
                {
                    if (Mode == Modes.Docked && Parent != null)
                    {
                        GuiDockNode? absorbing = null;

                        if (Parent.Left == this)
                            absorbing = Parent.Right;
                        else
                            absorbing = Parent.Left;

                        if (absorbing != null)
                        {
                            Parent.Left = absorbing.Left;
                            Parent.Right = absorbing.Right;
                            Parent.SplitHorizontally = absorbing.SplitHorizontally;
                            Parent.SplitPoint = absorbing.SplitPoint;
                            Parent.Panels.AddRange(absorbing.Panels);
                            foreach (var child in absorbing.Panels)
                                child.Node = Parent;
                        }
                    }
                    else if (Mode == Modes.Standalone)
                    {

                    }
                    else if (Mode == Modes.Floating)
                    {

                    }
                    else if (Mode == Modes.Root)
                    {

                    }
                }
            }
        }

        public void Refresh()
        {

        }
    }
}
