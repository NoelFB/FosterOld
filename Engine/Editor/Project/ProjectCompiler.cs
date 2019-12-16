using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Foster.Editor
{
    public class ProjectCompiler
    {

        public bool IsBuilding { get; private set; }
        public bool IsSuccess { get; private set; }

        public List<string> Log = new List<string>();
        public List<string> Errors = new List<string>();

        public bool Build(string csProjectPath, string binPath)
        {
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
                IsSuccess = pr.ExitCode == 0;

                return IsSuccess;
            }

            return false;
        }


    }
}
