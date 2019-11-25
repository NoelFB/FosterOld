using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public struct Sizing
    {

        public enum Modes
        {
            Preferred,
            Explicit,
            Fill
        }

        public Modes ModeX;
        public Modes ModeY;

        public float ExplicitWidth;
        public float ExplicitHeight;

        public Vector2 SizeOf(Imgui imgui, IContent content, Vector2 padding)
        {
            var size = Vector2.Zero;

            if (ModeX == Modes.Preferred || ModeY == Modes.Preferred)
                size = content.PreferredPaddedSize(imgui, padding);

            if (ModeX == Modes.Fill)
                size.X = float.MaxValue;
            else if (ModeX == Modes.Explicit)
                size.X = ExplicitWidth;

            if (ModeY == Modes.Fill)
                size.Y = float.MaxValue;
            else if (ModeY == Modes.Explicit)
                size.Y = ExplicitHeight;

            return size;
        }

        public static Sizing Preferred() => new Sizing 
        { 
            ModeX = Modes.Preferred, 
            ModeY = Modes.Preferred 
        };

        public static Sizing Size(float width, float height) => new Sizing 
        { 
            ModeX = Modes.Explicit, 
            ModeY = Modes.Explicit, 
            ExplicitWidth = width, 
            ExplicitHeight = height 
        };

        public static Sizing Width(float width, bool fillHeight = false) => new Sizing 
        { 
            ModeX = Modes.Explicit, 
            ModeY = (fillHeight ? Modes.Fill : Modes.Preferred),
            ExplicitWidth = width 
        };

        public static Sizing Height(float height, bool fillWidth = false) => new Sizing 
        { 
            ModeX = (fillWidth ? Modes.Fill : Modes.Preferred),
            ModeY = Modes.Explicit, 
            ExplicitHeight = height 
        };

        public static Sizing Fill() => new Sizing 
        { 
            ModeX = Modes.Fill, 
            ModeY = Modes.Fill 
        };

        public static Sizing FillX() => new Sizing
        {
            ModeX = Modes.Fill,
            ModeY = Modes.Preferred
        };

        public static Sizing FillX(float height) => new Sizing
        {
            ModeX = Modes.Fill,
            ModeY = Modes.Explicit,
            ExplicitHeight = height
        };

        public static Sizing FillY() => new Sizing
        {
            ModeX = Modes.Preferred,
            ModeY = Modes.Fill
        };

        public static Sizing FillY(float width) => new Sizing
        {
            ModeX = Modes.Explicit,
            ModeY = Modes.Fill,
            ExplicitWidth = width
        };

    }
}
