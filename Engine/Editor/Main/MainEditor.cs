using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Editor
{
    public class MainEditor : Module
    {
        public readonly Project Project;
        public AssetHandle Inspecting;

        public MainEditor(Project project)
        {
            Project = project;
            Reload();
            App.Window.OnFocus += Reload;
        }

        private void Reload()
        {
            Project.Assets.Refresh();
            Project.Compiler.Build((b) => UpdateTitle());
            UpdateTitle();
        }

        protected override void Startup()
        {
            var font = new SpriteFont(Calc.EmbeddedResource(Path.Combine("Content", "InputMono-Medium.ttf")), 64, Charsets.ASCII);
            
            App.Modules.Register(new Gui(font, App.Window));

            new ScenePanel(this);
            new AssetsPanel(this);
            new InspectorPanel(this);
        }

        private void UpdateTitle()
        {
            string status;

            if (Project.Compiler.IsBuilding)
                status = "rebuilding";
            else if (Project.Compiler.IsSuccess)
                status = "ready";
            else
                status = "build error";

            App.Window.Title = $"Foster.Editor :: {Project.Name} :: ({status})";
        }
    }
}
