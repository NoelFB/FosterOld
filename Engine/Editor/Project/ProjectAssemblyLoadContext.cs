using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Foster.Editor
{
#pragma warning disable CS8603 // Possible null reference return.

    /// <summary>
    /// Custom Assembly Load Context for the Project code, required for unloadability
    /// See details here: https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
    /// </summary>
    public class ProjectAssemblyLoadContext : AssemblyLoadContext
    {
        public ProjectAssemblyLoadContext() : base(isCollectible: true)
        {

        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }

#pragma warning restore CS8603 // Possible null reference return.
}
