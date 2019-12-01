using System;
using System.Collections.Generic;

namespace Foster.Framework
{
    public abstract class Mesh<TVertex> : GraphicsResource where TVertex : struct
    {
        public Material? Material;

        protected Mesh(Graphics graphics) : base(graphics)
        {
            if (!VertexAttributeAttribute.TypeHasAttributes<TVertex>())
            {
                throw new Exception("Vertex Type must have at least 1 field with a VertexAttribute");
            }
        }

        public abstract void SetVertices(Memory<TVertex> vertices);
        public abstract void SetTriangles(Memory<int> triangles);
        public abstract void SetInstances<T>(Memory<T> instances) where T : struct;

        public abstract void Draw();
        public abstract void Draw(int start, int elements);
        public abstract void DrawInstances(int start, int elements, int instances);

        public static Mesh<TVertex> Create()
        {
            return App.Graphics.CreateMesh<TVertex>();
        }
    }
}
