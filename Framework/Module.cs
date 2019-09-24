namespace Foster.Framework
{
    public abstract class Module
    {

        protected internal virtual void OnCreated() { }
        protected internal virtual void OnStartup() { }
        protected internal virtual void OnContext() { }
        protected internal virtual void OnDisplayed() { }
        protected internal virtual void OnShutdown() { }
        protected internal virtual void OnDestroyed() { }
        protected internal virtual void OnPreUpdate() { }
        protected internal virtual void OnPostUpdate() { }
        protected internal virtual void OnPreRender() { }
        protected internal virtual void OnPostRender() { }

    }
}
