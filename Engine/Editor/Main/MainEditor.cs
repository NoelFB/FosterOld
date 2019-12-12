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
        public readonly ProjectCompiler Compiler;

        public MainEditor(Project project)
        {
            Project = project;
            Compiler = new ProjectCompiler(project);
            App.Window.Title = "Foster.Editor :: " + Project.Name;
        }

        protected override void Startup()
        {
            var font = new SpriteFont(Calc.EmbeddedResource(Path.Combine("Content", "InputMono-Medium.ttf")), 64, Charsets.ASCII);
            
            App.Modules.Register(new Gui(font, App.Window));

            new ScenePanel(this);
        }
    }
}
