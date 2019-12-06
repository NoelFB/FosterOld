using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foster.Framework
{
    public abstract class InternalTarget : InternalResource
    {

        protected internal readonly List<InternalTexture> attachments = new List<InternalTexture>();

    }
}
