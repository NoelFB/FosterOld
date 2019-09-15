using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Foster.Framework
{
    public class Batch2D : GraphicsResource
    {

        #region Shader Source

        private const string VertexSource = @"
#version 330
uniform mat4 Matrix;
layout(location = 0) in vec2 vertPos;
layout(location = 1) in vec2 vertUV;
layout(location = 2) in vec4 vertCol;
layout(location = 3) in float vertMult;
layout(location = 4) in float vertWash;
layout(location = 5) in float vertFill;
out vec2 fragUV;
out vec4 fragCol;
out float fragMult;
out float fragWash;
out float fragFill;
void main(void)
{
    gl_Position = Matrix * vec4(vertPos, 0.0, 1.0);
    fragUV = vertUV;
    fragCol = vertCol;
    fragMult = vertMult;
    fragWash = vertWash;
    fragFill = vertFill;
}";

        private const string FragmentSource = @"
#version 330
uniform sampler2D Texture;
in vec2 fragUV;
in vec4 fragCol;
in float fragMult;
in float fragWash;
in float fragFill;
layout(location = 0) out vec4 outColor;
void main(void)
{
    vec4 color = texture(Texture, fragUV);
    outColor = 
        fragMult * color * fragCol + 
        fragWash * color.a * fragCol + 
        fragFill * fragCol;
}";

        #endregion

        public readonly Shader DefaultShader;
        public readonly Mesh<Vertex2D> Mesh;

        private Vertex2D[] vertices;
        private int[] triangles;
        private List<Batch> batches;

        private int vertexCount;
        private int triangleCount;
        private Batch activeBatch;
        private bool dirty = false;

        private struct Batch
        {
            public BlendMode BlendMode;
            public Matrix3x2 Matrix;
            public Shader? Shader;
            public Texture? Texture;
            public int Elements;

            public Batch(BlendMode blendMode, Matrix3x2 matrix, Shader? shader, Texture? texture, int elements)
            {
                BlendMode = blendMode;
                Matrix = matrix;
                Shader = shader;
                Texture = texture;
                Elements = elements;
            }
        }

        public Batch2D(Graphics graphics) : base(graphics)
        {
            DefaultShader = graphics.CreateShader(VertexSource, FragmentSource);
            Mesh = graphics.CreateMesh<Vertex2D>();

            vertices = new Vertex2D[64];
            triangles = new int[64];
            batches = new List<Batch>();

            Clear();
        }

        public void Clear()
        {
            vertexCount = 0;
            triangleCount = 0;
            activeBatch = new Batch(BlendMode.Normal, Matrix3x2.Identity, null, null, 0);
            batches.Clear();
        }

        public void Render()
        {
            if (batches.Count > 0 || activeBatch.Elements > 0)
            {
                if (dirty)
                {
                    Mesh.SetTriangles(new Memory<int>(triangles, 0, triangleCount));
                    Mesh.SetVertices(new Memory<Vertex2D>(vertices, 0, vertexCount));

                    dirty = false;
                }

                Graphics.DepthTest(false);
                Graphics.CullMode(Cull.None);

                var ortho =
                    Matrix3x2.CreateScale((1.0f / Graphics.Viewport.Width) * 2, -(1.0f / Graphics.Viewport.Height) * 2) *
                    Matrix3x2.CreateTranslation(-1.0f, 1.0f);

                // render batches
                int start = 0;
                for (int i = 0; i < batches.Count; i++)
                {
                    RenderBatch(batches[i], ortho, start);
                    start += batches[i].Elements;
                }

                // remaining elements
                if (activeBatch.Elements > 0)
                    RenderBatch(activeBatch, ortho, start);

            }
        }

        private void RenderBatch(Batch batch, Matrix3x2 orthographic, int start = 0)
        {
            var shader = batch.Shader ?? DefaultShader;
            var world = batch.Matrix * orthographic;

            shader.SetUniform("Texture", batch.Texture);
            shader.SetUniform("Matrix", world);
            shader.Use();

            Graphics.BlendMode(batch.BlendMode);
            Mesh.Draw(start, batch.Elements);
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            Array.Fill(vertices, new Vertex2D(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), vertexCount, 4);

            vertices[vertexCount + 0].Pos = v0;
            vertices[vertexCount + 1].Pos = v1;
            vertices[vertexCount + 2].Pos = v2;
            vertices[vertexCount + 3].Pos = v3;

            vertexCount += 4;
        }

        public void Triangle(Vector2 v0, Vector2 v1, Vector2 v2, Color c0, Color c1, Color c2)
        {
            PushTriangle();
            ExpandVertices(vertexCount + 3);

            Array.Fill(vertices, new Vertex2D(Vector2.Zero, Vector2.Zero, c0, 0, 0, 255), vertexCount, 3);

            vertices[vertexCount + 0].Pos = v0;
            vertices[vertexCount + 1].Pos = v1;
            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Pos = v2;
            vertices[vertexCount + 2].Col = c2;

            vertexCount += 3;
        }

        public void Rect(Rect rect, Color color)
        {
            Quad(
                new Vector2(rect.X, rect.Y), 
                new Vector2(rect.X + rect.Width, rect.Y), 
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height), 
                new Vector2(rect.X, rect.Y + rect.Height), color);
        }

        private void SetActiveTexture(Texture? texture)
        {
            if (activeBatch.Texture == null || activeBatch.Elements == 0)
            {
                activeBatch.Texture = texture;
            }
            else if (activeBatch.Texture != texture)
            {
                batches.Add(activeBatch);

                activeBatch.Texture = texture;
                activeBatch.Elements = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushTriangle()
        {
            while (triangleCount + 3 >= triangles.Length)
                Array.Resize(ref triangles, triangles.Length * 2);

            triangles[triangleCount + 0] = vertexCount + 0;
            triangles[triangleCount + 1] = vertexCount + 1;
            triangles[triangleCount + 2] = vertexCount + 2;

            triangleCount += 3;
            activeBatch.Elements++;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushQuad()
        {
            while (triangleCount + 6 >= triangles.Length)
                Array.Resize(ref triangles, triangles.Length * 2);

            triangles[triangleCount + 0] = vertexCount + 0;
            triangles[triangleCount + 1] = vertexCount + 1;
            triangles[triangleCount + 2] = vertexCount + 2;
            triangles[triangleCount + 3] = vertexCount + 0;
            triangles[triangleCount + 4] = vertexCount + 2;
            triangles[triangleCount + 5] = vertexCount + 3;

            triangleCount += 6;
            activeBatch.Elements += 2;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushVertexFill(Vector2 pos, Vector2 tex, Color color)
        {
            vertices[vertexCount] = new Vertex2D(pos, tex, color, 0, 0, 255);
            vertexCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushVertexWash(Vector2 pos, Vector2 tex, Color color)
        {
            vertices[vertexCount] = new Vertex2D(pos, tex, color, 0, 255, 0);
            vertexCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushVertexMult(Vector2 pos, Vector2 tex, Color color)
        {
            vertices[vertexCount] = new Vertex2D(pos, tex, color, 255, 0, 0);
            vertexCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandVertices(int index)
        {
            while (index >= vertices.Length)
                Array.Resize(ref vertices, vertices.Length * 2);
        }
    }
}
