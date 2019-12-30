using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foster.Framework.Internal
{
    public abstract class InternalTarget : InternalResource
    {

        protected internal readonly List<InternalTexture> attachments = new List<InternalTexture>();
        protected internal InternalTexture? depth;

    }
}
