using Foster.Framework;
using Foster.GuiSystem;
using System.Collections;
using System.IO;

namespace Foster.Editor
{
    public class ProjectEditor : Module
    {
        public readonly Project Project;
        public readonly Window Window;

        public AssetsPanel AssetsPanel;
        public ScenePanel ScenePanel;
        public InspectorPanel InspectorPanel;
        public AssetHandle Inspecting;

        private bool reloading;

        public ProjectEditor(Project project)
        {
            Window = App.Window ?? throw new System.Exception("Project Editor requires an open Window");

            // tell project to start watching the File System
            Project = project;
            Project.StartWatching();

            // create GUI module
            var font = new SpriteFont(Calc.EmbeddedResource(Path.Combine("Content", "InputMono-Medium.ttf")), 64, Charsets.ASCII);
            App.Modules.Register(new Gui(font, App.Window));

            // create main editor panels
            ScenePanel = new ScenePanel(this);
            AssetsPanel = new AssetsPanel(this);
            InspectorPanel = new InspectorPanel(this);

            // update window title
            UpdateTitle((Project.Compiler.IsAssemblyValid ? "ready" : "build error"));
        }

        protected override void Startup()
        {
            Window.OnFocus += OnWindowFocus;
            Window.OnClose += OnWindowClose;
        }

        protected override void Shutdown()
        {
            if (Window.Opened)
                Window.OnFocus -= OnWindowFocus;
            Project.Dispose();
        }

        private void OnWindowFocus()
        {
            if (!reloading && Project.IsWaitingForReload)
            {
                reloading = true;
                RunRoutine(Reload());
            }
        }

        private void OnWindowClose()
        {
            App.Exit();
        }

        private IEnumerator Reload()
        {
            UpdateTitle("rebuilding");
            yield return null;

            Project.Reload();
            AssetsPanel.Reload();

            UpdateTitle((Project.Compiler.IsAssemblyValid ? "ready" : "build error"));
            reloading = false;
        }

        private void UpdateTitle(string status)
        {
            Window.Title = $"Foster.Editor :: {Project.Config.Name} :: ({status})";
        }
    }
}
