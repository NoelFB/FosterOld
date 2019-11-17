using System;
using System.Collections.Generic;
using System.Text;
using Foster.Framework;

namespace Foster.GuiSystem
{
    public class GuiPanel
    {

        public int ID = Guid.NewGuid().GetHashCode();
        public string Title = "";
        public Action<Imgui>? OnRefresh;
        
        public GuiPanel(string title)
        {
            Title = title;
        }

    }
}
