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

        public void Refresh(Imgui imgui, Rect bounds)
        {
            if (imgui.BeginFrame(Title, bounds))
            {
                OnRefresh?.Invoke(imgui);
                imgui.EndFrame();
            }
        }

    }
}
