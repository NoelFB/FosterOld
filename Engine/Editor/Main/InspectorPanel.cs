using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Editor
{
    public class InspectorPanel : GuiPanel
    {

        public readonly MainEditor Editor;

        public InspectorPanel(MainEditor editor) : base(App.Modules.Get<Gui>(), "Inspector")
        {
            Editor = editor;
        }

        public override void Refresh(Imgui imgui)
        {
            if (Editor.Inspecting.Instance != null)
            {
                var asset = Editor.Inspecting.Instance;
                var name = Editor.Inspecting.Name!;

                imgui.Label($"Editing {name}");
                imgui.Button("Save");
            }
        }

    }
}
