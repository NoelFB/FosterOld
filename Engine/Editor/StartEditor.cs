using Foster.Framework;
using Foster.GuiSystem;
using Foster.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Editor
{
    public class StartEditor : Module
    {

        private string ProjectsPath => Path.Combine(App.System.Directory, "Projects");

        private readonly SpriteFont font;
        private readonly Window window;
        private readonly Imgui imgui;
        private readonly Batch2D batcher;

        private List<string> existingProjects = new List<string>();

        public StartEditor(string[] args)
        {
            // find existing projects
            {
                if (!Directory.Exists(ProjectsPath))
                    Directory.CreateDirectory(ProjectsPath);

                foreach (var directory in Directory.EnumerateDirectories(ProjectsPath))
                {
                    var name = Path.GetFileName(directory);
                    if (!string.IsNullOrEmpty(name))
                        existingProjects.Add(name);
                }
            }

            // load default font
            font = new SpriteFont(Calc.EmbeddedResource("Content/InputMono-Medium.ttf")!, 128, Charsets.ASCII);

            // open a window
            window = App.System.CreateWindow("Foster.Editor", 1280, 720, WindowFlags.ScaleToMonitor);
            window.OnRender = Render;

            // batch2d
            batcher = new Batch2D();

            // create our imgui
            imgui = new Imgui(font);
            imgui.DefaultFontSize = 20;
            imgui.Style.TitleScale = 2f;
            imgui.Style.TitleColor = 0x00daa9;

            imgui.Style.Generic.Idle = new StyleState()
            {
                BackgroundColor = 0x6b818c,
                ContentColor = 0x2d3047,
                Padding = new Vector2(8, 4)
            };

            imgui.Style.Generic.Hot = new StyleState()
            {
                BackgroundColor = 0xa0a0a0,
                ContentColor = 0x160f29,
                Padding = new Vector2(8, 4)
            };

            imgui.Style.Generic.Active = new StyleState()
            {
                BackgroundColor = 0x00daa9,
                ContentColor = 0x000000,
                Padding = new Vector2(8, 4)
            };
        }

        protected override void Update()
        {
            batcher.Clear();

            imgui.Step();
            imgui.BeginViewport(window, batcher);

            if (imgui.BeginFrame("Main", new Rect(128, 128, 400, 400)))
            {
                imgui.PushSpacing(-10);
                imgui.Title("FOSTER");
                imgui.Label("v0.1.0");
                imgui.PopSpacing();

                if (imgui.Button("New Project"))
                {
                    var project = Project.Create("new project", Path.Combine(ProjectsPath, "new project"));
                    Launch(project);
                }

                imgui.Cell(0f, 30f);
                imgui.Label("existing projects");

                foreach (var existing in existingProjects)
                {
                    if (imgui.Button(existing))
                    {
                        var project = Project.Load(Path.Combine(ProjectsPath, existing));
                        Launch(project);
                    }
                }
                imgui.EndFrame();
            }

            imgui.EndViewport();
        }

        public void Launch(Project project)
        {
            App.Modules.Remove(this);
            App.Modules.Register(new MainEditor(project));
        }

        private void Render()
        {
            App.Graphics.ClearColor(0x2d3047);
            batcher.Render();
        }

    }
}
