using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GuiSystem
{
    public struct ImguiID
    {
        public readonly int Value;
        public ImguiID(int value) => Value = value;
        public ImguiID(ImguiName name, ImguiID parent) => Value = HashCode.Combine(name.Value, parent.Value);

        public override bool Equals(object? obj) => obj != null && (obj is ImguiID id) && (this == id);
        public override int GetHashCode() => Value;
        public override string ToString() => Value.ToString();

        public static bool operator ==(ImguiID a, ImguiID b) => a.Value == b.Value;
        public static bool operator !=(ImguiID a, ImguiID b) => a.Value != b.Value;

        public static readonly ImguiID None = new ImguiID(0);
    }
}
