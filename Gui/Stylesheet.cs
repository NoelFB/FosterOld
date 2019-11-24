using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public struct StyleState
    {
        public Vector2 Padding;
        public BorderRadius BorderRadius;
        public BorderWeight BorderWeight;
        public Color BorderColor;
        public Color BackgroundColor;
        public Color ContentColor;
    }

    public struct StyleElement
    {
        public StyleState Idle;
        public StyleState Hot;
        public StyleState Active;

        public StyleState Current(Imgui.ID active, Imgui.ID hot, Imgui.ID id)
        {
            if (id == active)
                return Active;
            else if (id == hot)
                return Hot;
            else
                return Idle;
        }
    }

    public struct StyleWindow
    {
        public StyleState Window;
        public Vector2 WindowPadding;
        public StyleState Frame;
        public Vector2 FramePadding;
        public float TabSpacing;
        public StyleElement Tab;
        public StyleElement CurrentTab;
    }

    public struct Stylesheet
    {
        public float TitleScale;

        public StyleWindow Window;
        public StyleWindow Docked;

        public StyleState Frame;
        public Vector2 FramePadding;

        public StyleElement Scrollbar;
        public float ScrollbarWeight;

        public StyleElement Generic;
        public StyleState Label;
    }

    public static class Stylesheets
    {
        public static Stylesheet Default = new Stylesheet
        {
            TitleScale = 1.25f,

            Window = new StyleWindow
            {
                Window = new StyleState
                {
                    BorderRadius = 2,
                    BorderWeight = 1,
                    BorderColor = 0x6b6b6b,
                    BackgroundColor = 0x424242,
                    Padding = new Vector2(0, 0)
                },

                Frame = new StyleState
                {
                    BorderRadius = 0,
                    BackgroundColor = 0x535353,
                    BorderColor = 0x535353,
                    BorderWeight = new BorderWeight(0, 0, 0, 0),
                    Padding = new Vector2(4, 4),
                },

                Tab = new StyleElement
                {
                    Idle = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 1),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x424242,
                        ContentColor = 0xa0a0a0,
                        Padding = new Vector2(10, 4),
                    },
                    Hot = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 1),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x535353,
                        ContentColor = 0xf0f0f0,
                        Padding = new Vector2(10, 4),
                    },
                    Active = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 0),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x383838,
                        ContentColor = 0xffffff,
                        Padding = new Vector2(10, 4),
                    },
                },
                CurrentTab = new StyleElement
                {
                    Idle = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 0),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x535353,
                        ContentColor = 0xf0f0f0,
                        Padding = new Vector2(10, 4),
                    },
                    Hot = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 0),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x535353,
                        ContentColor = 0xf0f0f0,
                        Padding = new Vector2(10, 4),
                    },
                    Active = new StyleState
                    {
                        BorderRadius = 0,
                        BorderWeight = new BorderWeight(0, 0, 1, 0),
                        BorderColor = 0x383838,
                        BackgroundColor = 0x383838,
                        ContentColor = 0xffffff,
                        Padding = new Vector2(10, 4),
                    }
                },
            },

            ScrollbarWeight = 8f,
            Scrollbar = new StyleElement
            {
                Idle = new StyleState
                {
                    BorderRadius = 8,
                    BorderWeight = new BorderWeight(0, 0, 0, 1),
                    BorderColor = 0x252e36,
                    BackgroundColor = 0x639ec0
                },
                Hot = new StyleState
                {
                    BorderRadius = 8,
                    BorderWeight = new BorderWeight(0, 0, 0, 1),
                    BorderColor = 0x252e36,
                    BackgroundColor = 0xa8e5f6
                },
                Active = new StyleState
                {
                    BorderRadius = 8,
                    BorderWeight = 0,
                    BackgroundColor = 0xee9ec0
                },
            },

            Generic = new StyleElement
            {
                Idle = new StyleState
                {
                    BorderRadius = new BorderRadius(3),
                    BorderWeight = new BorderWeight(0, 0, 0, 1),
                    BorderColor = 0x252e36,
                    BackgroundColor = 0x626d78,
                    ContentColor = 0xc3c5cf,
                    Padding = new Vector2(6, 2)
                },
                Hot = new StyleState
                {
                    BorderRadius = new BorderRadius(3),
                    BorderWeight = new BorderWeight(0, 0, 0, 1),
                    BorderColor = 0x252e36,
                    BackgroundColor = 0x83949e,
                    ContentColor = 0xc3c5cf,
                    Padding = new Vector2(6, 2)
                },
                Active = new StyleState
                {
                    BorderRadius = new BorderRadius(3),
                    BorderWeight = new BorderWeight(0),
                    BorderColor = 0xd95bad,
                    BackgroundColor = 0xd95bad,
                    ContentColor = 0x000000,
                    Padding = new Vector2(6, 2)
                },
            },

        };
    }
}
