using System;
using System.IO;
using System.Reflection;
using Foster.Framework;
using Foster.Runtime;

namespace Foster.Engine
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            // load modules ....
            // somehow ...
            // ....
            // ....

            App.Start(() =>
            {
                // create the game window
                var window = App.System.CreateWindow("Game", 1280, 720);

                // find our assets
                var assets = new PackedAssetBank();
                foreach (var file in Directory.GetFiles(Path.Combine(App.System.Directory, "Assets"), "*.pack"))
                    assets.AddPack(file);

                // let's go!
                App.Modules.Register(new Game(Game.Modes.Standalone, window, assets));
            });
        }
    }
}
