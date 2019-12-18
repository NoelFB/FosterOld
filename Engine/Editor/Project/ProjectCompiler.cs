using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Foster.Editor
{
    public class ProjectCompiler : IDisposable
    {

        public bool IsWaitingForRebuild => codeDirty;
        public bool IsBuilding { get; private set; }
        public bool IsAssemblyValid { get; private set; }

        public List<string> Log = new List<string>();
        public List<string> Errors = new List<string>();

        private FileSystemWatcher watcher;
        private bool codeDirty = true;

        public ProjectCompiler(string assetsPath)
        {
            watcher = new FileSystemWatcher(assetsPath, "*.cs")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
            };

            watcher.Created += FileChanged;
            watcher.Changed += FileChanged;
            watcher.Deleted += FileChanged;
            watcher.Renamed += FileChanged;
        }

        public void StartWatching()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            codeDirty = true;
        }

        public bool Build(string csProjectPath, string binPath)
        {
            codeDirty = false;

            if (!IsBuilding)
            {
                Log.Clear();
                Errors.Clear();

                IsBuilding = true;

                using var pr = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"build \"{csProjectPath}\" -o \"{binPath}\" --verbosity q --nologo /clp:NoSummary",
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };

                pr.OutputDataReceived += (s, ev) =>
                {
                    var msg = ev.Data;
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        Log.Add(msg);

                        if (msg.Contains("error CS"))
                        {
                            var path = msg.LastIndexOf('[');
                            if (path >= 0)
                            {
                                var error = msg.Substring(0, path);
                                Console.WriteLine(error);
                                Errors.Add(error);
                            }
                        }
                    }

                };

                pr.Start();
                pr.BeginOutputReadLine();
                pr.BeginErrorReadLine();
                pr.WaitForExit();

                IsBuilding = false;
                IsAssemblyValid = pr.ExitCode == 0;

                return IsAssemblyValid;
            }

            return false;
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
