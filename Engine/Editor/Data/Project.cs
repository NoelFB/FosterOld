using Foster.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Foster.Editor
{
    public class Project : Asset
    {

        public string Name;
        public string ProjectPath;
        public string CsProjectPath => Path.Combine(ProjectPath, "Project.csproj");
        public string CodePath => Path.Combine(ProjectPath, "Code");
        public string AssetsPath => Path.Combine(ProjectPath, "Assets");
        public string TempPath => Path.Combine(ProjectPath, "Temp");
        public string TempBinaryPath => Path.Combine(ProjectPath, "Temp", "Project.dll");

        private Project(string projectPath)
        {
            Name = Path.GetFileName(projectPath);
            ProjectPath = projectPath;
        }

        public static Project Create(string projectPath)
        {
            var project = new Project(projectPath);

            // create project directory
            Directory.CreateDirectory(projectPath);

            // create csproj
            {
                using var writer = File.CreateText(project.CsProjectPath);
                using var stream = Calc.EmbeddedResource("Content/EmptyProject.csproj");
                using var reader = new StreamReader(stream);

                writer.Write(reader.ReadToEnd());
            }

            // create default directories
            Directory.CreateDirectory(project.CodePath);
            Directory.CreateDirectory(project.AssetsPath);

            // create default .git ignore
            File.WriteAllText(Path.Combine(projectPath, ".gitignore"), "bin\nobj\n.vs");

            return project;
        }

        public static Project Load(string projectPath)
        {
            return new Project(projectPath);
        }

    }
}
