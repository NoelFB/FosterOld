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
    public class ScenePanel : GuiPanel
    {

        public readonly MainEditor Editor;

        private bool building = false;
        private bool failed = false;

        public ScenePanel(MainEditor editor) : base(App.Modules.Get<Gui>(), "Scene")
        {
            Editor = editor;
        }

        public override void Refresh(Imgui imgui)
        {
            if (building)
            {
                imgui.Label("... building");
            }
            else if (imgui.Button("Rebuild"))
            {
                building = true;
                Editor.Project.Compiler.Log.Clear();
                Editor.Project.Compiler.Build((s) =>
                {
                    building = false;
                    failed = !s;
                });
            }

            if (failed)
            {
                imgui.Label("Fix compile errors before running");
            }
            else if (imgui.Button("Run Code"))
            {
                using (var proj = new ProjectAssembly(Editor.Project))
                {

                }
            }

            if (imgui.BeginFrame("LOG", imgui.Cell(Sizing.Fill().SizeOfEmpty())))
            {
                foreach (var line in Editor.Project.Compiler.Log)
                {
                    imgui.Label(line);
                    break;
                }
                imgui.EndFrame();
            }
        }



    }
}
