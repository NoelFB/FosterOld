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

        public MainEditor(Project project)
        {
            Project = project;
        }

        protected override void Startup()
        {
            var font = new SpriteFont(Path.Combine(App.System.Directory, "Content", "InputMono-Medium.ttf"), 64, Charsets.ASCII);
            var gui = App.Modules.Register(new Gui(font, App.Window));

            new GuiPanel(gui, "Scene");
        }
    }
}
