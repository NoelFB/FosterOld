using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Editor
{
    public class ScenePanel : GuiPanel
    {

        public readonly MainEditor Editor;

        public ScenePanel(MainEditor editor) : base(App.Modules.Get<Gui>(), "Scene")
        {
            Editor = editor;
        }

        public override void Refresh(Imgui imgui)
        {

        }

    }
}
