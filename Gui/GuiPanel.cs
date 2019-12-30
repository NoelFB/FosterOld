using Foster.Framework;
using System;

namespace Foster.GuiSystem
{
    public class GuiPanel
    {

        /// <summary>
        /// Gui this Panel belongs to
        /// </summary>
        public readonly Gui Gui;

        /// <summary>
        /// Unique ID generated for this Panel
        /// </summary>
        public readonly int ID = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// The title of the Panel
        /// </summary>
        public string Title = "";

        /// <summary>
        /// The Padding of the Contents of the Panel
        /// </summary>
        public Vector2 Padding = new Vector2(4, 4);

        /// <summary>
        /// Whether this panel is opened
        /// </summary>
        public bool Opened => Node != null;

        internal GuiDockNode? Node;

        public GuiPanel(Gui gui, string title, Rect position)
        {
            Gui = gui;
            Title = title;

            var node = new GuiDockNode(gui, GuiDockNode.Modes.Floating, position.Int());
            node.InsertPanel(GuiDockNode.Placings.Center, this);
        }

        public GuiPanel(Gui gui, string title, GuiPanel? dockWidth = null)
        {
            Gui = gui;
            Title = title;

            if (dockWidth == null)
                gui.Root.InsertPanel(GuiDockNode.Placings.Center, this);
            else if (dockWidth.Node != null)
                dockWidth.Node.InsertPanel(GuiDockNode.Placings.Center, this);
        }

        /// <summary>
        /// Called when the Panel contents are refreshed
        /// </summary>
        public virtual void Refresh(Imgui imgui)
        {

        }

        /// <summary>
        /// Pops the Panel out into a standalone Window
        /// </summary>
        public void Popout() => Node?.PopoutPanel(this);

        /// <summary>
        /// Closes the Panel
        /// </summary>
        public void Close() => Node?.RemovePanel(this);

        /// <summary>
        /// Docks this Panel with another existing panel
        /// </summary>
        public void DockWith(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Center);

        /// <summary>
        /// Docks this Panel to the left of an existing panel
        /// </summary>
        public void DockLeftOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Left);

        /// <summary>
        /// Docks this Panel to the right of an existing panel
        /// </summary>
        public void DockRightOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Right);

        /// <summary>
        /// Docks this Panel to the bottom of an existing panel
        /// </summary>
        public void DockBottomOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Bottom);

        /// <summary>
        /// Docks this Panel to the top of an existing panel
        /// </summary>
        public void DockTopOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Top);

        private void Dock(GuiPanel? panel, GuiDockNode.Placings placing)
        {
            Node?.RemovePanel(this);

            var node = panel?.Node;
            if (node == null)
                node = Gui.Root;

            node.InsertPanel(placing, this);
        }

    }
}
