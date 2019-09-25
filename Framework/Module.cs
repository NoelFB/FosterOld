namespace Foster.Framework
{
    public abstract class Module
    {

        protected internal virtual void Created() { }
        protected internal virtual void Startup() { }
        protected internal virtual void Shutdown() { }

        protected internal virtual void BeforeUpdate() { }
        protected internal virtual void Update() { }
        protected internal virtual void AfterUpdate() { }

        protected internal virtual void BeforeRender() { }
        protected internal virtual void Render(Window window) { }
        protected internal virtual void AfterRender() { }

        protected internal virtual void Tick() { }

    }
}
