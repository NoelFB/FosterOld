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

        private ProjectCompiler compiler;

        private bool building = false;
        private bool failed = false;

        public ScenePanel(MainEditor editor) : base(App.Modules.Get<Gui>(), "Scene")
        {
            Editor = editor;
            compiler = new ProjectCompiler(editor.Project);
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
                compiler.Log.Clear();
                compiler.Build((s) =>
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
                ExecuteAndUnload(Editor.Project.TempBinaryPath);
            }

            if (imgui.BeginFrame("LOG", imgui.Cell(Sizing.Fill().SizeOfEmpty())))
            {
                foreach (var line in compiler.Log)
                    imgui.Label(line);
                imgui.EndFrame();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void ExecuteAndUnload(string assemblyPath)
        {
            var alc = new ProjectAssemblyLoadContext();

            using var stream = File.OpenRead(assemblyPath);
            var assembly = alc.LoadFromStream(stream);
            stream.Dispose();

            try
            {
                var test = assembly.GetType("Test");
                var instance = Activator.CreateInstance(test);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            alc.Unload();
        }



    }
}
