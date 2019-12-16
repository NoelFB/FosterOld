using Foster.Framework;
using Foster.Framework.Json;
using Foster.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Foster.Editor
{
    public class Project
    {
        public string ProjectName;
        public string ProjectPath;

        public string ConfigPath => Path.Combine(ProjectPath, "Project.foster");
        public string CsProjectPath => Path.Combine(ProjectPath, "Project.csproj");
        public string GeneratedPath => Path.Combine(ProjectPath, "Generated");
        public string AssetsPath => Path.Combine(ProjectPath, "Assets");
        public string TempBuildPath => Path.Combine(ProjectPath, "bin");
        public string TempAssemblyPath => Path.Combine(TempBuildPath, "Project.dll");

        public readonly ProjectAssetBank Assets;
        public ProjectAssembly Assembly { get; private set; }

        public bool IsAssemblyValid { get; private set; }
        public bool IsWaitingForReload => codeDirty || assetsDirty;

        private FileSystemWatcher? watcher;
        private bool codeDirty = true;
        private bool assetsDirty = true;

        public List<string> Tags = new List<string>();

        private Project(string name, string projectPath)
        {
            ProjectName = name;
            ProjectPath = projectPath;

            Assets = new ProjectAssetBank(AssetsPath);
            Assembly = new ProjectAssembly();
        }

        public void StartWatching()
        {
            watcher = new FileSystemWatcher(AssetsPath, "*.*")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite,
            };

            watcher.Changed += FileChanged;
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            var ext = Path.GetExtension(e.FullPath);

            if (ext != null)
            {
                if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase) || ext.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    codeDirty = true;
                }
                else
                {
                    // TODO:
                    // mark specific assets as dirty
                    assetsDirty = true;
                }
            }
        }

        public void Reload()
        {
            if (!IsWaitingForReload)
                return;

            // rebuild & reload assembly
            if (codeDirty)
            {
                codeDirty = false;
                IsAssemblyValid = false;

                // recompile assembly
                var compiler = new ProjectCompiler();
                compiler.Build(CsProjectPath, TempBuildPath);

                // reload the assembly if we successfully re-compiled
                if (compiler.IsSuccess)
                {
                    IsAssemblyValid = true;

                    // TODO:
                    // Mark all Assets that rely on the Assembly as dirty

                    Assembly.Dispose();
                    Assembly = new ProjectAssembly();
                    Assembly.Load(TempAssemblyPath);
                }
            }

            if (assetsDirty)
            {
                assetsDirty = false;

                Assets.Refresh();
            }
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
            File.WriteAllText(
                Path.Combine(project.ProjectPath, ".gitignore"), 
                Calc.EmbeddedResourceText("Content/Default/.gitignore"));

            return project;
        }

        public static Project Load(string projectPath)
        {
            var project = new Project(Path.GetFileName(projectPath) ?? "Unnamed", projectPath);

            if (!File.Exists(project.ConfigPath))
                project.CreateConfigFile();

            if (!File.Exists(project.CsProjectPath))
                project.CreateCsProjectFile();

            return project;
        }

        private void CreateCsProjectFile()
        {
            File.WriteAllText(CsProjectPath,
                Calc.EmbeddedResourceText("Content/Default/Project.csproj"));
        }

        private void CreateConfigFile()
        {
            var config = new JsonObject
            {
                ["name"] = ProjectName,
                ["tags"] = new JsonArray()
            };

            using var writer = new JsonWriter(ConfigPath);
            writer.Strict = false;
            writer.JsonValue(config);
        }
    }
}
