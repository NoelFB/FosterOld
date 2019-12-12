using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Foster.Editor
{
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
}
