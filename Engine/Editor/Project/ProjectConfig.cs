using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Editor
{
    public class ProjectConfig
    {

        public readonly string ProjectPath;

        public string ConfigPath => Path.Combine(ProjectPath, "Project.foster");
        public string CsProjectPath => Path.Combine(ProjectPath, "Project.csproj");
        public string GeneratedPath => Path.Combine(ProjectPath, "Generated");
        public string AssetsPath => Path.Combine(ProjectPath, "Assets");
        public string TempBuildPath => Path.Combine(ProjectPath, "bin");
        public string TempAssemblyPath => Path.Combine(TempBuildPath, "Project.dll");

        public string Name = "Unnamed";
        public List<string> Tags = new List<string>();

        public ProjectConfig(string path)
        {
            ProjectPath = path;

            if (!Directory.Exists(ProjectPath))
                Directory.CreateDirectory(ProjectPath);
        }

        public void Reload()
        {

        }

        public void Save()
        {
            var json = new JsonObject
            {
                ["Name"] = Name,
                ["Tags"] = new JsonArray(Tags)
            };

            using var writer = new JsonWriter(File.OpenWrite(ConfigPath), true);
            writer.JsonValue(json);
        }
    }
}
