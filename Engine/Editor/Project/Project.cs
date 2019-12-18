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
    public class Project : IDisposable
    {
        public string ProjectPath => Config.ProjectPath;

        public readonly ProjectConfig Config;
        public readonly ProjectAssetBank Assets;
        public readonly GameAssembly GameAssembly;
        public readonly ProjectCompiler Compiler;

        public bool IsWaitingForReload => Compiler.IsWaitingForRebuild || Assets.IsWaitingForSync;

        public Project(ProjectConfig config)
        {
            Config = config;
            Assets = new ProjectAssetBank(config.AssetsPath);
            Compiler = new ProjectCompiler(config.AssetsPath);
            GameAssembly = new GameAssembly();
        }

        public void StartWatching()
        {
            Assets.StartWatching();
            Compiler.StartWatching();
        }

        public void StopWatching()
        {
            Assets.StopWatching();
            Compiler.StopWatching();
        }

        public void Reload(bool fullAssetReload = false)
        {
            if (Compiler.IsWaitingForRebuild)
            {
                Compiler.Build(Config.CsProjectPath, Config.TempBuildPath);

                if (Compiler.IsAssemblyValid)
                {
                    // Unload all Assets that rely on the Assembly
                    Assets.UnloadAll<Prefab>();

                    GameAssembly.Reload(Config.TempAssemblyPath);
                }
            }

            if (Assets.IsWaitingForSync || fullAssetReload)
            {
                if (fullAssetReload)
                    Assets.FindAllFiles();
                else
                    Assets.SyncFiles();
            }
        }

        public void Dispose()
        {
            Assets.Dispose();
            Compiler.Dispose();
            GameAssembly.Dispose();
        }
    }
}
