using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Engine
{
    public class Prefab : IAsset
    {

        public Guid Guid { get; set; }

        public Entity Entity;

        public Prefab(string name)
        {
            Entity = new Entity(name);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
