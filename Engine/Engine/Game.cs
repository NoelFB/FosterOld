using Foster.Framework;
using System.Collections.Generic;

namespace Foster.Engine
{

    public class Game : Module
    {

        public enum Modes
        {
            Standalone,
            Inline
        }

        public static Modes Mode;
        public static Window Window;
        public static AssetBank Assets;

        public Game(Modes mode, Window window, AssetBank assets)
        {
            Mode = mode;
            Window = window;
            Assets = assets;
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
