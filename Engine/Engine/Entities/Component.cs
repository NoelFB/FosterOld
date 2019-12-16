using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Engine
{
    public abstract class Component
    {

        public Entity? Entity { get; internal set; }

        public Transform? Transform => Entity?.Transform;

        public virtual void OnAwake()
        {

        }

        public virtual void OnStart()
        {

        }

        public virtual void OnDestroy()
        {

        }

    }
}
