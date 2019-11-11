using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public class GuiPanel
    {

        public string Title = "";
        public Action<Imgui>? OnRefresh;
        
        public GuiPanel(string title)
        {
            Title = title;
        }

    }
}
