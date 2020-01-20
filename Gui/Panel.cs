using Foster.Framework;
using System;

namespace Foster.GuiSystem
{
    public class Panel
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

        internal DockNode? Node;

        public Panel(string title, Rect position) : this(App.Modules.Get<Gui>(), title, position)
        {

        }

        public Panel(Gui gui, string title, Rect position)
        {
            Gui = gui;
            Gui.Panels.Add(this);
            Title = title;

            var node = new DockNode(gui, DockNode.Modes.Floating, position.Int());
            node.InsertPanel(DockNode.Placings.Center, this);
        }

        public Panel(string title, Panel? dockWith = null) : this(App.Modules.Get<Gui>(), title, dockWith)
        {

        }

        public Panel(Gui gui, string title, Panel? dockWith = null)
        {
            Gui = gui;
            Gui.Panels.Add(this);
            Title = title;

            if (dockWith == null)
                gui.Root.InsertPanel(DockNode.Placings.Center, this);
            else if (dockWith.Node != null)
                dockWith.Node.InsertPanel(DockNode.Placings.Center, this);
        }

        public void MakeVisible()
        {
            if (Node != null)
                Node.ActivePanel = this;
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
        public void Close()
        {
            Node?.RemovePanel(this);
            Gui.Panels.Remove(this);
        }

        /// <summary>
        /// Docks this Panel with another existing panel
        /// </summary>
        public void DockWith(Panel? panel) => Dock(panel, DockNode.Placings.Center);

        /// <summary>
        /// Docks this Panel to the left of an existing panel
        /// </summary>
        public void DockLeftOf(Panel? panel) => Dock(panel, DockNode.Placings.Left);

        /// <summary>
        /// Docks this Panel to the right of an existing panel
        /// </summary>
        public void DockRightOf(Panel? panel) => Dock(panel, DockNode.Placings.Right);

        /// <summary>
        /// Docks this Panel to the bottom of an existing panel
        /// </summary>
        public void DockBottomOf(Panel? panel) => Dock(panel, DockNode.Placings.Bottom);

        /// <summary>
        /// Docks this Panel to the top of an existing panel
        /// </summary>
        public void DockTopOf(Panel? panel) => Dock(panel, DockNode.Placings.Top);

        private void Dock(Panel? panel, DockNode.Placings placing)
        {
            Node?.RemovePanel(this);

            var node = panel?.Node;
            if (node == null)
                node = Gui.Root;

            node.InsertPanel(placing, this);
        }

    }
}
