using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Foster.Engine
{
#pragma warning disable CS8603 // Possible null reference return.

    /// <summary>
    /// Custom Assembly Load Context for the Game code, required for unloadability
    /// See details here: https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
    /// </summary>
    public class GameAssemblyLoadContext : AssemblyLoadContext
    {
        public GameAssemblyLoadContext() : base(isCollectible: true)
        {

        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }

#pragma warning restore CS8603 // Possible null reference return.
}
