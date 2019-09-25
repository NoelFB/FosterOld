using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Framework
{
    public abstract class Context
    {
        public abstract bool Disposed { get; }
        protected abstract System System { get; }

        protected Context()
        {

        }

        public void SetActive()
        {
            System.ActiveContext = this;
        }
    }
}
