using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Foster.Editor
{
    public class ProjectEditor : Module
    {
        public readonly Project Project;

        public AssetHandle Inspecting;
        private bool reloadOnNextUpdate;

        public ProjectEditor(Project project)
        {
            Project = project;
            Project.StartWatching();

            UpdateTitle((Project.Compiler.IsAssemblyValid ? "ready" : "build error"));
        }

        protected override void Startup()
        {
            var font = new SpriteFont(Calc.EmbeddedResource(Path.Combine("Content", "InputMono-Medium.ttf")), 64, Charsets.ASCII);
            App.Modules.Register(new Gui(font, App.Window));

            new ScenePanel(this);
            new AssetsPanel(this);
            new InspectorPanel(this);

            App.Window.OnFocus += OnFocus;
        }

        private void OnFocus()
        {
            if (Project.IsWaitingForReload)
            {
                reloadOnNextUpdate = true;
                UpdateTitle("rebuilding");
            }
        }

        private void UpdateTitle(string status)
        {
            App.Window.Title = $"Foster.Editor :: {Project.Config.Name} :: ({status})";
        }

        protected override void BeforeUpdate()
        {
            if (reloadOnNextUpdate)
            {
                reloadOnNextUpdate = false;
                Project.Reload();
                UpdateTitle((Project.Compiler.IsAssemblyValid ? "ready" : "build error"));
            }
        }

        protected override void Shutdown()
        {
            App.Window.OnFocus -= OnFocus;
            Project.Dispose();
        }
    }
}
