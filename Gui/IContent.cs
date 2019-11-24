using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public interface IContent
    {

        Vector2 PreferredSize(Imgui imgui);
        void Draw(Imgui imgui, Batch2d batcher, StyleState style, Rect position);
        Imgui.UniqueInfo UniqueInfo();

    }
}
