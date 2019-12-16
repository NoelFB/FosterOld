using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Editor
{
    public class AssetsPanel : GuiPanel
    {

        public AssetsPanel() : base(App.Modules.Get<Gui>(), "Assets")
        {

        }

        public override void Refresh(Imgui imgui)
        {

        }

    }
}
