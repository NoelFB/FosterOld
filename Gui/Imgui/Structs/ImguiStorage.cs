using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.GUI
{
    public class ImguiStorage
    {
        private readonly Dictionary<ImguiID, float> numbers = new Dictionary<ImguiID, float>();
        private readonly Dictionary<ImguiID, bool> bools = new Dictionary<ImguiID, bool>();
        private readonly Dictionary<ImguiID, ImguiID> ids = new Dictionary<ImguiID, ImguiID>();

        public bool Used;

        public void SetBool(ImguiID id, ImguiName name, bool value) => bools[new ImguiID(name, id)] = value;
        public bool GetBool(ImguiID id, ImguiName name, bool defaultValue) => bools.TryGetValue(new ImguiID(name, id), out var v) ? v : defaultValue;

        public void SetNumber(ImguiID id, ImguiName name, float value) => numbers[new ImguiID(name, id)] = value;
        public float GetNumber(ImguiID id, ImguiName name, float defaultValue) => numbers.TryGetValue(new ImguiID(name, id), out var v) ? v : defaultValue;

        public void SetId(ImguiID id, ImguiName name, ImguiID value) => ids[new ImguiID(name, id)] = value;
        public ImguiID GetId(ImguiID id, ImguiName name, ImguiID defaultValue) => ids.TryGetValue(new ImguiID(name, id), out var v) ? v : defaultValue;
    }
}
