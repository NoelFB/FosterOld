using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Foster.Editor
{
    public class ProjectCompiler
    {

        public readonly Project Project;
        public bool IsBuilding { get; private set; }
        public List<string> Log = new List<string>();

        private Action<bool>? onComplete;

        public ProjectCompiler(Project project)
        {
            Project = project;
        }

        public void Build(Action<bool>? onComplete = null)
        {
            if (!IsBuilding)
            {
                Log.Clear();
                IsBuilding = true;
                this.onComplete = onComplete;

                var thread = new Thread(new ThreadStart(BuildThread));
                thread.Start();
            }
        }

        private void BuildThread()
        {
            using var pr = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{Project.CsProjectPath}\" -o \"{Project.TempPath}\" --verbosity q --nologo",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            pr.OutputDataReceived += (s, ev) =>
            {
                Console.WriteLine(ev.Data);
                if (!string.IsNullOrWhiteSpace(ev.Data))
                    Log.Add(ev.Data);
            };

            pr.ErrorDataReceived += (s, err) =>
            {
                Console.WriteLine(err.Data);
                if (!string.IsNullOrWhiteSpace(err.Data))
                    Log.Add(err.Data);
            };

            pr.Start();
            pr.BeginOutputReadLine();
            pr.BeginErrorReadLine();
            pr.WaitForExit();

            IsBuilding = false;
            onComplete?.Invoke(pr.ExitCode == 0);
        }

    }
}
