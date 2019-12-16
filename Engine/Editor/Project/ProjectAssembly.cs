using Foster.Engine;
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
        private ProjectAssemblyLoadContext? context;
        private Assembly? assembly;

        public readonly Dictionary<Guid, Type> Components = new Dictionary<Guid, Type>();

        public void Load(string assemblyPath)
        {
            using var stream = File.OpenRead(assemblyPath);
            
            context = new ProjectAssemblyLoadContext();
            assembly = context.LoadFromStream(stream);

            // find all the component types
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(Component).IsAssignableFrom(type))
                {
                    Components[type.GUID] = type;
                    Console.WriteLine(type.Name + ": " + type.GUID);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Dispose()
        {
            if (assembly != null)
            {
                var weakRef = new WeakReference(assembly, trackResurrection: true);

                Components.Clear();
                assembly = null;
                context?.Unload();
                context = null;

                for (int i = 0; weakRef.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (weakRef.IsAlive)
                    throw new Exception("Something is keeping the reference alive");
            }
        }

    }
}
