using Foster.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foster.Engine
{
    public class Entity
    {

        public string Name;
        public bool Active;
        public Entity? Parent { get; private set; }

        public readonly Transform Transform;
        
        private readonly List<Entity> children;
        private readonly List<Component> components;

        public Entity(string name)
        {
            Name = name;
            Active = true;
            Transform = new Transform();
            children = new List<Entity>();
            components = new List<Component>();
        }

        public Entity? FindFirst(string name, bool recursive = false)
        {
            foreach (var child in children)
            {
                if (child.Name.Equals(name))
                    return child;
            }

            if (recursive)
            {
                foreach (var child in children)
                {
                    var found = child.FindFirst(name, true);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public void AddChild(Entity entity)
        {
            if (entity.Parent != null)
                throw new Exception("Entity is already a child of another Entity");

            entity.Transform.Parent = Transform;
            entity.Parent = this;

            children.Add(entity);
        }

        public void RemoveChild(Entity entity)
        {
            if (entity.Parent == this)
            {
                entity.Transform.Parent = null;
                entity.Parent = null;

                children.Remove(entity);
            }
        }

        /// <summary>
        /// Creates a Component of the given Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Create<T>() where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Entity = this;
            component.OnAwake();
            components.Add(component);
        }

        /// <summary>
        /// Creates a Component of the given Type
        /// </summary>
        public void Create(Type type)
        {
            var instance = Activator.CreateInstance(type);
            if (!(instance is Component component))
                throw new Exception("Type does not inherit from Component");

            component.Entity = this;
            component.OnAwake();
            components.Add(component);
        }

        public T? FindFirst<T>(bool recursive = false) where T : Component
        {
            foreach (var component in components)
                if (component is T typed)
                    return typed;

            if (recursive)
            {
                foreach (var child in children)
                {
                    var found = child.FindFirst<T>(true);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public List<T> FindAll<T>(bool recursive = false) where T : Component
        {
            var list = new List<T>();

            FindAll(list, recursive);

            return list;
        }

        public bool FindAll<T>(List<T> populate, bool recursive = false) where T : Component
        {
            var initial = populate.Count;

            foreach (var component in components)
                if (component is T typed)
                    populate.Add(typed);

            if (recursive)
            {
                foreach (var child in children)
                    child.FindAll<T>(populate, true);
            }

            return populate.Count > initial;
        }

        /// <summary>
        /// Destroys a Component
        /// </summary>
        /// <param name="component"></param>
        public void Destroy(Component component)
        {
            if (component.Entity == this)
            {
                component.OnDestroy();
                component.Entity = null;
                components.Remove(component);
            }
        }

        /// <summary>
        /// Destroys the first Component of the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Destroy<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i ++)
                if (components[i] is T)
                {
                    Destroy(components[i]);
                    break;
                }
        }

        /// <summary>
        /// Destroys the Entity and all its Children and Components
        /// </summary>
        public void Destroy()
        {
            Parent?.RemoveChild(this);
            
            while (children.Count > 0)
                children[children.Count - 1].Destroy();

            while (components.Count > 0)
                Destroy(components[components.Count - 1]);
        }

        public virtual Entity Clone()
        {
            throw new NotImplementedException();
        }
    }
}
