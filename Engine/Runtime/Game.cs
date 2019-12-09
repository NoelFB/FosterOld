using Foster.Framework;

namespace Foster.Runtime
{
    public class Game : Module
    {

        public enum Modes
        {
            Standalone,
            Inline
        }

        public readonly Modes Mode;
        public readonly Window Window;
        public readonly AssetBank Assets;

        public Game(Modes mode, Window window, AssetBank assets)
        {
            Mode = mode;
            Window = window;
            Assets = assets;
        }
    }
}
