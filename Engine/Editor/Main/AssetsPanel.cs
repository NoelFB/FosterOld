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

        public readonly MainEditor Editor;
        private AssetHandle<Texture> preview;

        public AssetsPanel(MainEditor editor) : base(App.Modules.Get<Gui>(), "Assets")
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
                var rect = imgui.Remainder();
                imgui.Batcher.Image(preview.Instance, rect.TopLeft, Color.White);
            }

            if (imgui.Button("New Component"))
            {
                using var writer = File.CreateText(Path.Combine(Editor.Project.AssetsPath, "Code", "NewComponent.cs"));
                using var stream = Calc.EmbeddedResource("Content/EmptyComponent.cs");
                using var reader = new StreamReader(stream);

                var content = reader.ReadToEnd();
                content = content.Replace("{Guid}", Guid.NewGuid().ToString());
                content = content.Replace("{Name}", "NewComponent");

                writer.Write(content);
            }
        }

    }
}
