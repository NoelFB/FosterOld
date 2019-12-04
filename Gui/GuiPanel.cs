using Foster.Framework;
using System;

namespace Foster.GuiSystem
{
    public class GuiPanel
    {

        public readonly Gui Gui;
        public readonly int ID = Guid.NewGuid().GetHashCode();

        public string Title = "";
        public Action<Imgui>? OnRefresh;
        public Vector2 Padding = new Vector2(4, 4);
        public bool Opened => Node != null;

        internal GuiDockNode? Node;

        internal GuiPanel(Gui gui, string title)
        {
            Gui = gui;
            Title = title;
        }

        public void Popout() => Node?.PopoutPanel(this);
        public void Close() => Node?.RemovePanel(this);

        public void DockWith(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Center);
        public void DockLeftOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Left);
        public void DockRightOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Right);
        public void DockBottomOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Bottom);
        public void DockTopOf(GuiPanel? panel) => Dock(panel, GuiDockNode.Placings.Top);

        private void Dock(GuiPanel? panel, GuiDockNode.Placings placing)
        {
            Node?.RemovePanel(this);

            var node = panel?.Node;
            if (node == null)
                node = Gui.Manager.Root;

            node.InsertPanel(placing, this);
        }

    }
}
