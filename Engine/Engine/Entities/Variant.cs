using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Engine
{
    public class Variant : Entity
    {

        public Prefab Prefab;

        public Variant(Prefab prefab) : base(prefab.Name)
        {
            Prefab = prefab;
        }

    }
}
