using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class GuiPanel
    {

        public readonly Gui Gui;
        public readonly int ID = Guid.NewGuid().GetHashCode();

        public string Title = "";
        public Action<Imgui>? OnRefresh;

        internal GuiDockNode Node;

        public GuiPanel(Gui gui, string title)
        {
            Gui = gui;
            Title = title;
        }

        public void Popout()
        {
            throw new NotImplementedException();
        }

        public void DockWith(GuiPanel panel)
        {
            throw new NotImplementedException();
        }

        public void DockLeftOf(GuiPanel panel)
        {
            throw new NotImplementedException();
        }

        public void DockRightOf(GuiPanel panel)
        {
            throw new NotImplementedException();
        }

        public void DockDownOf(GuiPanel panel)
        {
            throw new NotImplementedException();
        }

        public void DockUpOf(GuiPanel panel)
        {
            throw new NotImplementedException();
        }

    }
}
