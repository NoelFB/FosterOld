using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Editor
{
    public class AssetsPanel : GuiPanel
    {

        public readonly ProjectEditor Editor;
        private AssetHandle<Texture> preview;

        public AssetsPanel(ProjectEditor editor) : base(App.Modules.Get<Gui>(), "Assets")
        {
            Editor = editor;
        }

        public override void Refresh(Imgui imgui)
        {
            foreach (var asset in Editor.Project.Assets.Names)
            {
                if (imgui.Button(asset))
                {
                    preview = Editor.Project.Assets.Handle<Texture>(asset);
                    Editor.Inspecting = Editor.Project.Assets.Handle<Texture>(asset);
                }
            }

            if (preview.Instance != null)
            {
                var rect = imgui.Cell(preview.Instance.Width, preview.Instance.Height);
                imgui.Batcher.Image(preview.Instance, rect.TopLeft, Color.White);
            }

            if (imgui.Button("New Component"))
            {
                var content = Calc.EmbeddedResourceText("Content/Default/Component.cs");
                content = content.Replace("{Guid}", Guid.NewGuid().ToString());
                content = content.Replace("{Name}", "NewComponent");
                File.WriteAllText(Path.Combine(Editor.Project.AssetsPath, "NewComponent.cs"), content);
            }

            if (Editor.Project.Assembly != null)
            {
                foreach (var component in Editor.Project.Assembly.Components)
                {
                    imgui.Label(component.Value.Name);
                }
            }
        }

    }
}
