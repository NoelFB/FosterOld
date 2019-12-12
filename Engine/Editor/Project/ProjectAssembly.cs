using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Editor
{
    public class ProjectAssembly : IDisposable
    {

        private readonly Project project;

        private ProjectAssemblyLoadContext? context;
        private Assembly? assembly;

        public ProjectAssembly(Project project)
        {
            this.project = project;
            Load();
        }

        ~ProjectAssembly()
        {
            Dispose();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Load()
        {
            using var stream = File.OpenRead(project.TempAssemblyPath);
            
            context = new ProjectAssemblyLoadContext();
            assembly = context.LoadFromStream(stream);

            // TEMP: test
            var test = assembly.GetType("Test");
            var instance = Activator.CreateInstance(test);
        }

        public void Dispose()
        {
            assembly = null;
            context?.Unload();
            context = null;
        }

    }
}
