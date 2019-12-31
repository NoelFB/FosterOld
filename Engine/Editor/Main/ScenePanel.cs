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
    public class ScenePanel : Panel
    {

        public readonly ProjectEditor Editor;

        public ScenePanel(ProjectEditor editor) : base(App.Modules.Get<Gui>(), "Scene")
        {
            Editor = editor;
        }

        public override void Refresh(Imgui imgui)
        {

        }

    }
}
