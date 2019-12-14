using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Engine
{
    public class Prefab : IAsset
    {

        public Guid Guid { get; set; }

        public bool IsVariant => Variant != null;
        public string Name => (Entity?.Name ?? Variant?.Name ?? "");

        public Variant? Variant;
        public Entity? Entity;

        public Prefab(string name)
        {
            Entity = new Entity(name);
        }

        public Prefab(Prefab variantOf)
        {
            Variant = new Variant(variantOf);
        }

        public Entity Instantiate()
        {
            if (Entity != null)
                return Entity.Clone();
            else if (Variant != null)
                return Variant.Clone();

            throw new Exception();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
