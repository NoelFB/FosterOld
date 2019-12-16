using Foster.Framework;
using Foster.Framework.Json;
using Foster.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Foster.Editor
{
    public class Project
    {
        public string Name;
        public string ProjectPath;

        public string ConfigPath => Path.Combine(ProjectPath, "Project.foster");
        public string CsProjectPath => Path.Combine(ProjectPath, "Project.csproj");
        public string GeneratedPath => Path.Combine(ProjectPath, "Generated");
        public string AssetsPath => Path.Combine(ProjectPath, "Assets");

        public string TempPath => Path.Combine(ProjectPath, "bin");
        public string TempAssemblyPath => Path.Combine(ProjectPath, "bin", "Project.dll");

        public List<string> Tags = new List<string>();

        public readonly FileAssetBank Assets;
        public readonly ProjectCompiler Compiler;

        private Project(string name, string projectPath)
        {
            Name = name;
            ProjectPath = projectPath;
            Assets = new FileAssetBank(AssetsPath);
            Compiler = new ProjectCompiler(this);
        }

        public void Reload()
        {

        }

        public void Save()
        {

        }

        public static Project Create(string name, string projectPath)
        {
            var project = new Project(name, projectPath);

            // create directories
            Directory.CreateDirectory(projectPath);
            Directory.CreateDirectory(project.GeneratedPath);
            Directory.CreateDirectory(project.AssetsPath);

            // create config files
            project.CreateCsProjectFile();
            project.CreateConfigFile();

            // create default .git ignore
            File.WriteAllText(Path.Combine(projectPath, ".gitignore"), "bin\nobj\n.vs");

            project.Reload();

            return project;
        }

        public static Project Load(string projectPath)
        {
            var project = new Project(Path.GetFileName(projectPath) ?? "Unnamed", projectPath);

            if (!File.Exists(project.ConfigPath))
                project.CreateConfigFile();

            if (!File.Exists(project.CsProjectPath))
                project.CreateCsProjectFile();

            project.Reload();
            return project;
        }

        private void CreateCsProjectFile()
        {
            using var writer = File.CreateText(CsProjectPath);
            using var stream = Calc.EmbeddedResource("Content/EmptyProject.csproj");
            using var reader = new StreamReader(stream);

            writer.Write(reader.ReadToEnd());
        }

        private void CreateConfigFile()
        {
            var config = new JsonObject
            {
                ["name"] = Name,
                ["tags"] = new JsonArray()
            };

            using var writer = new JsonWriter(ConfigPath);
            writer.Strict = false;
            writer.JsonValue(config);
        }
    }
}
