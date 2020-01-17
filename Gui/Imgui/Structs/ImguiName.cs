using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public struct ImguiName
    {
        public int Value;
        public ImguiName(int value) => Value = value;

        public static implicit operator ImguiName(int id) => new ImguiName(id);
        public static implicit operator ImguiName(float id) => new ImguiName(id.GetHashCode());
        public static implicit operator ImguiName(string text) => new ImguiName(text.GetHashCode());
    }
}
