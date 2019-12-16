using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    public class Batch2D
    {

        public static readonly VertexFormat VertexFormat = new VertexFormat(
            new VertexElement("vPosition", VertexType.Float, 2),
            new VertexElement("vTex", VertexType.Float, 2),
            new VertexElement("vColor", VertexType.UnsignedByte, 4, true),
            new VertexElement("vMult", VertexType.UnsignedByte, 1, true),
            new VertexElement("vWash", VertexType.UnsignedByte, 1, true),
            new VertexElement("vFill", VertexType.UnsignedByte, 1, true));

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertex : IVertex
        {
            public Vector2 Pos;
            public Vector2 Tex;
            public Color Col;
            public byte Mult;
            public byte Wash;
            public byte Fill;

            public Vertex(Vector2 position, Vector2 texcoord, Color color, int mult, int wash, int fill)
            {
                Pos = position;
                Tex = texcoord;
                Col = color;
                Mult = (byte)mult;
                Wash = (byte)wash;
                Fill = (byte)fill;
            }

            public VertexFormat Format => VertexFormat;

            public override string ToString()
            {
                return $"{{Pos:{Pos}, Tex:{Tex}, Col:{Col}, Mult:{Mult}, Wash:{Wash}, Fill:{Fill}}}";
            }
        }

        #region Shader Source

        private const string VertexSource = @"
#version 330
uniform mat4 matrix;

in vec2 vPosition;
in vec2 vTex;
in vec4 vColor;
in float vMult;
in float vWash;
in float vFill;

out vec2 fTex;
out vec4 fColor;
out float fMult;
out float fWash;
out float fFill;

void main(void)
{
    gl_Position = matrix * vec4(vPosition, 0.0, 1.0);
    fTex = vTex;
    fColor = vColor;
    fMult = vMult;
    fWash = vWash;
    fFill = vFill;
}";

        private const string FragmentSource = @"
#version 330
uniform sampler2D mainTexture;

in vec2 fTex;
in vec4 fColor;
in float fMult;
in float fWash;
in float fFill;

out vec4 outColor;

void main(void)
{
    vec4 color = texture(mainTexture, fTex);
    outColor = 
        fMult * color * fColor + 
        fWash * color.a * fColor + 
        fFill * fColor;
}";

        #endregion

        public readonly Shader DefaultShader;
        public readonly Material DefaultMaterial;
        public readonly Mesh Mesh;

        public string TextureUniformName = "mainTexture";
        public string MatrixUniformName = "matrix";

        public Matrix OrthographicMatrix =>
            Matrix.CreateScale((1.0f / graphics.Viewport.Width) * 2, -(1.0f / graphics.Viewport.Height) * 2, 1f) *
            Matrix.CreateTranslation(-1.0f, 1.0f, 0f);

        public Matrix2D MatrixStack = Matrix2D.Identity;

        private readonly Graphics graphics;
        private readonly Stack<Matrix2D> matrixStack = new Stack<Matrix2D>();

        private Vertex[] vertices;
        private int[] triangles;
        private readonly List<Batch> batches;
        private Batch currentBatch;
        private int currentBatchInsert;

        private int vertexCount;
        private int triangleCount;
        private bool dirty = false;

        public int Triangles => triangleCount / 3;
        public int Vertices => vertexCount;
        public int Batches => batches.Count + (currentBatch.Elements > 0 ? 1 : 0);

        private struct Batch
        {
            public int Layer;
            public bool NextHasSameState;
            public Material? Material;
            public BlendMode BlendMode;
            public Matrix2D Matrix;
            public Texture? Texture;
            public RectInt? Scissor;
            public int Offset;
            public int Elements;

            public Batch(Material? material, BlendMode blend, Texture? texture, Matrix2D matrix, int offset, int elements)
            {
                Layer = 0;
                NextHasSameState = false;
                Material = material;
                BlendMode = blend;
                Texture = texture;
                Matrix = matrix;
                Scissor = null;
                Offset = offset;
                Elements = elements;
            }

            public bool CanMerge(ref Batch batch)
            {
                return batch.Layer == Layer && batch.Material == Material && batch.BlendMode == BlendMode && batch.Matrix == Matrix && batch.Texture == Texture && batch.Scissor == Scissor;
            }
        }

        public Batch2D() : this(App.Graphics)
        {

        }

        public Batch2D(Graphics graphics)
        {
            DefaultShader = new Shader(graphics, VertexSource, FragmentSource);
            DefaultMaterial = new Material(DefaultShader);
            Mesh = new Mesh(graphics);

            this.graphics = graphics;
            vertices = new Vertex[64];
            triangles = new int[64];
            batches = new List<Batch>();

            Clear();
        }

        public void Clear()
        {
            vertexCount = 0;
            triangleCount = 0;
            currentBatchInsert = 0;
            currentBatch = new Batch(null, BlendMode.Normal, null, Matrix2D.Identity, 0, 0);
            batches.Clear();
            matrixStack.Clear();
            MatrixStack = Matrix2D.Identity;
        }

        #region Rendering

        public void Render()
        {
            Render(OrthographicMatrix);
        }

        public void Render(Matrix matrix)
        {
            Debug.Assert(matrixStack.Count <= 0, "Batch.MatrixStack Pushes more than it Pops");

            if (batches.Count > 0 || currentBatch.Elements > 0)
            {
                if (dirty)
                {
                    Mesh.SetVertices(new ReadOnlyMemory<Vertex>(vertices, 0, vertexCount));
                    Mesh.SetIndices(new ReadOnlyMemory<int>(triangles, 0, triangleCount));

                    dirty = false;
                }

                graphics.SetDepthTest(false);
                graphics.SetCullMode(Cull.None);

                // render batches
                var shareState = false;
                for (int i = 0; i < batches.Count; i++)
                {
                    // remaining elements in the current batch
                    if (currentBatchInsert == i && currentBatch.Elements > 0)
                        RenderBatch(currentBatch, ref shareState, ref matrix);

                    // render the batch
                    RenderBatch(batches[i], ref shareState, ref matrix);
                }

                // remaining elements in the current batch
                if (currentBatchInsert == batches.Count && currentBatch.Elements > 0)
                    RenderBatch(currentBatch, ref shareState, ref matrix);
            }
        }

        private void RenderBatch(Batch batch, ref bool shareState, ref Matrix matrix)
        {
            if (!shareState)
            {
                if (batch.Scissor != null)
                    graphics.SetScissor(batch.Scissor.Value);
                else
                    graphics.DisableScissor();

                // set BlendMode
                graphics.SetBlendMode(batch.BlendMode);

                // Render the Mesh
                // Note we apply the texture and matrix based on the current batch
                // If the user set these on the Material themselves, they will be overwritten here

                Mesh.Material = batch.Material ?? DefaultMaterial;
                Mesh.Material[TextureUniformName]?.SetTexture(batch.Texture);
                Mesh.Material[MatrixUniformName]?.SetMatrix(new Matrix(batch.Matrix) * matrix);
            }

            Mesh.Draw(batch.Offset, batch.Elements);
            shareState = batch.NextHasSameState;
        }

        #endregion

        #region Modify State

        public void SetMaterial(Material? material)
        {
            if (currentBatch.Elements == 0)
            {
                currentBatch.Material = material;
            }
            else if (currentBatch.Material != material)
            {
                batches.Insert(currentBatchInsert, currentBatch);

                currentBatch.Material = material;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
                currentBatchInsert++;
            }
        }

        public void SetBlendMode(BlendMode blendmode)
        {
            if (currentBatch.Elements == 0)
            {
                currentBatch.BlendMode = blendmode;
            }
            else if (currentBatch.BlendMode != blendmode)
            {
                batches.Insert(currentBatchInsert, currentBatch);

                currentBatch.BlendMode = blendmode;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
                currentBatchInsert++;
            }
        }

        public void SetMatrix(Matrix2D matrix)
        {
            if (currentBatch.Elements == 0)
            {
                currentBatch.Matrix = matrix;
            }
            else if (currentBatch.Matrix != matrix)
            {
                batches.Insert(currentBatchInsert, currentBatch);

                currentBatch.Matrix = matrix;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
                currentBatchInsert++;
            }
        }

        public void SetScissor(RectInt? scissor)
        {
            if (currentBatch.Elements == 0)
            {
                currentBatch.Scissor = scissor;
            }
            else if (currentBatch.Scissor != scissor)
            {
                batches.Insert(currentBatchInsert, currentBatch);

                currentBatch.Scissor = scissor;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
                currentBatchInsert++;
            }
        }

        public void SetTexture(Texture? texture)
        {
            if (currentBatch.Texture == null || currentBatch.Elements == 0)
            {
                currentBatch.Texture = texture;
            }
            else if (currentBatch.Texture != texture)
            {
                batches.Insert(currentBatchInsert, currentBatch);

                currentBatch.Texture = texture;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
                currentBatchInsert++;
            }
        }

        public void SetLayer(int layer)
        {
            if (currentBatch.Layer == layer)
                return;

            // insert last batch
            if (currentBatch.Elements > 0)
            {
                batches.Insert(currentBatchInsert, currentBatch);
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
            }

            // find the point to insert us
            var insert = 0;
            while (insert < batches.Count && batches[insert].Layer >= layer)
                insert++;

            // can the previous one merge with us?
            if (insert > 0 && batches[insert - 1].CanMerge(ref currentBatch))
            {
                var prev = batches[insert - 1];
                prev.NextHasSameState = true;
                batches[insert - 1] = prev;
            }

            currentBatch.Layer = layer;
            currentBatchInsert = insert;
        }

        public void SetState(Material? material, BlendMode blendmode, Matrix2D matrix, RectInt? scissor)
        {
            SetMaterial(material);
            SetBlendMode(blendmode);
            SetMatrix(matrix);
            SetScissor(scissor);
        }

        public Matrix2D PushMatrix(Vector2 position, Vector2 scale, Vector2 origin, float rotation, bool relative = true)
        {
            return PushMatrix(Matrix2D.CreateTransform(position, origin, scale, rotation), relative);
        }

        public Matrix2D PushMatrix(Transform2D transform, bool relative = true)
        {
            return PushMatrix(transform.Matrix, relative);
        }

        public Matrix2D PushMatrix(Vector2 position, bool relative = true)
        {
            return PushMatrix(Matrix2D.CreateTranslation(position.X, position.Y), relative);
        }

        public Matrix2D PushMatrix(Matrix2D matrix, bool relative = true)
        {
            matrixStack.Push(MatrixStack);

            if (relative)
            {
                MatrixStack = matrix * MatrixStack;
            }
            else
            {
                MatrixStack = matrix;
            }

            return MatrixStack;
        }

        public Matrix2D PopMatrix()
        {
            Debug.Assert(matrixStack.Count > 0, "Batch.MatrixStack Pops more than it Pushes");

            if (matrixStack.Count > 0)
            {
                MatrixStack = matrixStack.Pop();
            }
            else
            {
                MatrixStack = Matrix2D.Identity;
            }

            return MatrixStack;
        }

        #endregion

        #region Quad

        public void Quad(Quad2D quad, Color color)
        {
            Quad(quad.A, quad.B, quad.C, quad.D, color);
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), vertexCount, 4);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, ref v3, ref MatrixStack);

            vertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 t0, Vector2 t1, Vector2 t2, Vector2 t3, Color color, bool washed = false)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            if (currentBatch.Texture?.Internal.FlipVertically ?? false)
                FlipYUVs(ref t0, ref t1, ref t2, ref t3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, t0, color, washed ? 0 : 255, washed ? 255 : 0, 0), vertexCount, 4);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, ref v3, ref MatrixStack);

            vertices[vertexCount + 1].Tex = t1;
            vertices[vertexCount + 2].Tex = t2;
            vertices[vertexCount + 3].Tex = t3;

            vertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Color c0, Color c1, Color c2, Color c3)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, c0, 0, 0, 255), vertexCount, 4);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, ref v3, ref MatrixStack);

            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Col = c2;
            vertices[vertexCount + 3].Col = c3;

            vertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 t0, Vector2 t1, Vector2 t2, Vector2 t3, Color c0, Color c1, Color c2, Color c3, bool washed = false)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            if (currentBatch.Texture?.Internal.FlipVertically ?? false)
                FlipYUVs(ref t0, ref t1, ref t2, ref t3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, t0, c0, washed ? 0 : 255, washed ? 255 : 0, 0), vertexCount, 4);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, ref v3, ref MatrixStack);

            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 1].Tex = t1;
            vertices[vertexCount + 2].Col = c2;
            vertices[vertexCount + 2].Tex = t2;
            vertices[vertexCount + 3].Col = c3;
            vertices[vertexCount + 3].Tex = t3;

            vertexCount += 4;
        }

        #endregion

        #region Triangle

        public void Triangle(Vector2 v0, Vector2 v1, Vector2 v2, Color color)
        {
            PushTriangle();
            ExpandVertices(vertexCount + 3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), vertexCount, 3);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);

            vertexCount += 3;
        }

        public void Triangle(Vector2 v0, Vector2 v1, Vector2 v2, Color c0, Color c1, Color c2)
        {
            PushTriangle();
            ExpandVertices(vertexCount + 3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, c0, 0, 0, 255), vertexCount, 3);

            Transform(ref vertices[vertexCount + 0].Pos, ref v0, ref MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, ref v1, ref MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, ref v2, ref MatrixStack);

            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Col = c2;

            vertexCount += 3;
        }

        #endregion

        #region Rect

        public void Rect(Rect rect, Color color)
        {
            Quad(
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height), color);
        }

        public void Rect(Vector2 position, Vector2 size, Color color)
        {
            Quad(
                position,
                position + new Vector2(size.X, 0),
                position + new Vector2(size.X, size.Y),
                position + new Vector2(0, size.Y), color);
        }

        public void Rect(float x, float y, float width, float height, Color color)
        {
            Quad(
                new Vector2(x, y),
                new Vector2(x + width, y),
                new Vector2(x + width, y + height),
                new Vector2(x, y + height), color);
        }

        public void Rect(Rect rect, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height), c0, c1, c2, c3);
        }

        public void Rect(Vector2 position, Vector2 size, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                position,
                position + new Vector2(size.X, 0),
                position + new Vector2(size.X, size.Y),
                position + new Vector2(0, size.Y), c0, c1, c2, c3);
        }

        public void Rect(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                new Vector2(x, y),
                new Vector2(x + width, y),
                new Vector2(x + width, y + height),
                new Vector2(x, y + height), c0, c1, c2, c3);
        }

        #endregion

        #region Rounded Rect

        public void RoundedRect(float x, float y, float width, float height, float r0, float r1, float r2, float r3, Color color)
        {
            RoundedRect(new Rect(x, y, width, height), r0, r1, r2, r3, color);
        }

        public void RoundedRect(float x, float y, float width, float height, float radius, Color color)
        {
            RoundedRect(new Rect(x, y, width, height), radius, radius, radius, radius, color);
        }

        public void RoundedRect(Rect rect, float radius, Color color)
        {
            RoundedRect(rect, radius, radius, radius, radius, color);
        }

        public void RoundedRect(Rect rect, float r0, float r1, float r2, float r3, Color color)
        {
            // clamp
            r0 = Math.Min(Math.Min(Math.Max(0, r0), rect.Width / 2f), rect.Height / 2f);
            r1 = Math.Min(Math.Min(Math.Max(0, r1), rect.Width / 2f), rect.Height / 2f);
            r2 = Math.Min(Math.Min(Math.Max(0, r2), rect.Width / 2f), rect.Height / 2f);
            r3 = Math.Min(Math.Min(Math.Max(0, r3), rect.Width / 2f), rect.Height / 2f);

            if (r0 <= 0 && r1 <= 0 && r2 <= 0 && r3 <= 0)
            {
                Rect(rect, color);
            }
            else
            {
                // get corners
                var r0_tl = rect.TopLeft;
                var r0_tr = r0_tl + new Vector2(r0, 0);
                var r0_br = r0_tl + new Vector2(r0, r0);
                var r0_bl = r0_tl + new Vector2(0, r0);

                var r1_tl = rect.TopRight + new Vector2(-r1, 0);
                var r1_tr = r1_tl + new Vector2(r1, 0);
                var r1_br = r1_tl + new Vector2(r1, r1);
                var r1_bl = r1_tl + new Vector2(0, r1);

                var r2_tl = rect.BottomRight + new Vector2(-r2, -r2);
                var r2_tr = r2_tl + new Vector2(r2, 0);
                var r2_bl = r2_tl + new Vector2(0, r2);
                var r2_br = r2_tl + new Vector2(r2, r2);

                var r3_tl = rect.BottomLeft + new Vector2(0, -r3);
                var r3_tr = r3_tl + new Vector2(r3, 0);
                var r3_bl = r3_tl + new Vector2(0, r3);
                var r3_br = r3_tl + new Vector2(r3, r3);

                // set tris
                {
                    while (triangleCount + 30 >= triangles.Length)
                        Array.Resize(ref triangles, triangles.Length * 2);

                    // top quad
                    {
                        triangles[triangleCount + 00] = vertexCount + 00; // r0b
                        triangles[triangleCount + 01] = vertexCount + 03; // r1a
                        triangles[triangleCount + 02] = vertexCount + 05; // r1d

                        triangles[triangleCount + 03] = vertexCount + 00; // r0b
                        triangles[triangleCount + 04] = vertexCount + 05; // r1d
                        triangles[triangleCount + 05] = vertexCount + 01; // r0c
                    }

                    // left quad
                    {
                        triangles[triangleCount + 06] = vertexCount + 02; // r0d
                        triangles[triangleCount + 07] = vertexCount + 01; // r0c
                        triangles[triangleCount + 08] = vertexCount + 10; // r3b

                        triangles[triangleCount + 09] = vertexCount + 02; // r0d
                        triangles[triangleCount + 10] = vertexCount + 10; // r3b
                        triangles[triangleCount + 11] = vertexCount + 09; // r3a
                    }

                    // right quad
                    {
                        triangles[triangleCount + 12] = vertexCount + 05; // r1d
                        triangles[triangleCount + 13] = vertexCount + 04; // r1c
                        triangles[triangleCount + 14] = vertexCount + 07; // r2b

                        triangles[triangleCount + 15] = vertexCount + 05; // r1d
                        triangles[triangleCount + 16] = vertexCount + 07; // r2b
                        triangles[triangleCount + 17] = vertexCount + 06; // r2a
                    }

                    // bottom quad
                    {
                        triangles[triangleCount + 18] = vertexCount + 10; // r3b
                        triangles[triangleCount + 19] = vertexCount + 06; // r2a
                        triangles[triangleCount + 20] = vertexCount + 08; // r2d

                        triangles[triangleCount + 21] = vertexCount + 10; // r3b
                        triangles[triangleCount + 22] = vertexCount + 08; // r2d
                        triangles[triangleCount + 23] = vertexCount + 11; // r3c
                    }

                    // center quad
                    {
                        triangles[triangleCount + 24] = vertexCount + 01; // r0c
                        triangles[triangleCount + 25] = vertexCount + 05; // r1d
                        triangles[triangleCount + 26] = vertexCount + 06; // r2a

                        triangles[triangleCount + 27] = vertexCount + 01; // r0c
                        triangles[triangleCount + 28] = vertexCount + 06; // r2a
                        triangles[triangleCount + 29] = vertexCount + 10; // r3b
                    }

                    triangleCount += 30;
                    currentBatch.Elements += 10;
                    dirty = true;
                }

                // set verts
                {
                    ExpandVertices(vertexCount + 12);

                    Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), vertexCount, 12);

                    Transform(ref vertices[vertexCount + 00].Pos, ref r0_tr, ref MatrixStack); // 0
                    Transform(ref vertices[vertexCount + 01].Pos, ref r0_br, ref MatrixStack); // 1
                    Transform(ref vertices[vertexCount + 02].Pos, ref r0_bl, ref MatrixStack); // 2

                    Transform(ref vertices[vertexCount + 03].Pos, ref r1_tl, ref MatrixStack); // 3
                    Transform(ref vertices[vertexCount + 04].Pos, ref r1_br, ref MatrixStack); // 4
                    Transform(ref vertices[vertexCount + 05].Pos, ref r1_bl, ref MatrixStack); // 5

                    Transform(ref vertices[vertexCount + 06].Pos, ref r2_tl, ref MatrixStack); // 6
                    Transform(ref vertices[vertexCount + 07].Pos, ref r2_tr, ref MatrixStack); // 7
                    Transform(ref vertices[vertexCount + 08].Pos, ref r2_bl, ref MatrixStack); // 8

                    Transform(ref vertices[vertexCount + 09].Pos, ref r3_tl, ref MatrixStack); // 9
                    Transform(ref vertices[vertexCount + 10].Pos, ref r3_tr, ref MatrixStack); // 10
                    Transform(ref vertices[vertexCount + 11].Pos, ref r3_br, ref MatrixStack); // 11

                    vertexCount += 12;
                }

                // top-left corner
                if (r0 > 0)
                    SemiCircle(r0_br, Vector2.Left.Angle(), Vector2.Up.Angle(), r0, Math.Max(3, (int)(r0 / 4)), color);
                else
                    Quad(r0_tl, r0_tr, r0_br, r0_bl, color);

                // top-right corner
                if (r1 > 0)
                    SemiCircle(r1_bl, Vector2.Up.Angle(), Vector2.Right.Angle(), r1, Math.Max(3, (int)(r1 / 4)), color);
                else
                    Quad(r1_tl, r1_tr, r1_br, r1_bl, color);

                // bottom-right corner
                if (r2 > 0)
                    SemiCircle(r2_tl, Vector2.Right.Angle(), Vector2.Down.Angle(), r2, Math.Max(3, (int)(r2 / 4)), color);
                else
                    Quad(r2_tl, r2_tr, r2_br, r2_bl, color);

                // bottom-left corner
                if (r3 > 0)
                    SemiCircle(r3_tr, Vector2.Down.Angle(), Vector2.Left.Angle(), r3, Math.Max(3, (int)(r3 / 4)), color);
                else
                    Quad(r3_tl, r3_tr, r3_br, r3_bl, color);
            }

        }

        public void SemiCircle(Vector2 center, float start, float end, float radius, int steps, Color color)
        {
            var last = Vector2.Angle(start, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Vector2.Angle(Calc.AngleLerp(start, end, (i / (float)steps)), radius);
                Triangle(center + last, center + next, center, color);
                last = next;
            }
        }

        #endregion

        #region Hollow Rect

        public void HollowRect(Rect rect, float t, Color color)
        {
            if (t > 0)
            {
                var tx = Math.Min(t, rect.Width / 2f);
                var ty = Math.Min(t, rect.Height / 2f);

                Rect(rect.X, rect.Y, rect.Width, ty, color);
                Rect(rect.X, rect.Bottom - ty, rect.Width, ty, color);
                Rect(rect.X, rect.Y + ty, tx, rect.Height - ty * 2, color);
                Rect(rect.Right - tx, rect.Y + ty, tx, rect.Height - ty * 2, color);
            }
        }

        #endregion

        #region Image

        public void Image(Texture texture,
            Vector2 pos0, Vector2 pos1, Vector2 pos2, Vector2 pos3,
            Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3,
            Color col0, Color col1, Color col2, Color col3, bool washed = false)
        {
            SetTexture(texture);
            Quad(pos0, pos1, pos2, pos3, uv0, uv1, uv2, uv3, col0, col1, col2, col3, washed);
        }

        public void Image(Texture texture,
            Vector2 pos0, Vector2 pos1, Vector2 pos2, Vector2 pos3,
            Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3,
            Color color, bool washed)
        {
            SetTexture(texture);
            Quad(pos0, pos1, pos2, pos3, uv0, uv1, uv2, uv3, color, washed);
        }

        public void Image(Texture texture, Color color, bool washed = false)
        {
            SetTexture(texture);
            Quad(
                new Vector2(0, 0), new Vector2(texture.Width, 0), new Vector2(texture.Width, texture.Height), new Vector2(0, texture.Height),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                color, washed);
        }

        public void Image(Texture texture, Vector2 position, Color color, bool washed = false)
        {
            SetTexture(texture);
            Quad(
                position, position + new Vector2(texture.Width, 0), position + new Vector2(texture.Width, texture.Height), position + new Vector2(0, texture.Height),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                color, washed);
        }

        public void Image(Texture texture, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Matrix2D.CreateTransform(position, origin, scale, rotation) * MatrixStack;

            SetTexture(texture);
            Quad(
                Vector2.Zero, new Vector2(texture.Width, 0), new Vector2(texture.Width, texture.Height), new Vector2(0, texture.Height),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                color, washed);

            MatrixStack = was;
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Color color, bool washed = false)
        {
            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.Right / texture.Width;
            var ty1 = clip.Bottom / texture.Height;

            SetTexture(texture);
            Quad(
                position, position + new Vector2(clip.Width, 0), position + new Vector2(clip.Width, clip.Height), position + new Vector2(0, clip.Height),
                new Vector2(tx0, ty0), new Vector2(tx1, ty0), new Vector2(tx1, ty1), new Vector2(tx0, ty1),
                color, washed);
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Matrix2D.CreateTransform(position, origin, scale, rotation) * MatrixStack;

            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.Right / texture.Width;
            var ty1 = clip.Bottom / texture.Height;

            SetTexture(texture);
            Quad(
                Vector2.Zero, new Vector2(clip.Width, 0), new Vector2(clip.Width, clip.Height), new Vector2(0, clip.Height),
                new Vector2(tx0, ty0), new Vector2(tx1, ty0), new Vector2(tx1, ty1), new Vector2(tx0, ty1),
                color, washed);

            MatrixStack = was;
        }

        public void Image(Subtexture subtex, Color color, bool washed = false)
        {
            SetTexture(subtex.Texture);
            Quad(
                subtex.DrawCoords[0], subtex.DrawCoords[1], subtex.DrawCoords[2], subtex.DrawCoords[3],
                subtex.TexCoords[0], subtex.TexCoords[1], subtex.TexCoords[2], subtex.TexCoords[3],
                color, washed);
        }

        public void Image(Subtexture subtex, Vector2 position, Color color, bool washed = false)
        {
            SetTexture(subtex.Texture);
            Quad(position + subtex.DrawCoords[0], position + subtex.DrawCoords[1], position + subtex.DrawCoords[2], position + subtex.DrawCoords[3],
                subtex.TexCoords[0], subtex.TexCoords[1], subtex.TexCoords[2], subtex.TexCoords[3],
                color, washed);
        }

        public void Image(Subtexture subtex, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Matrix2D.CreateTransform(position, origin, scale, rotation) * MatrixStack;

            SetTexture(subtex.Texture);
            Quad(
                subtex.DrawCoords[0], subtex.DrawCoords[1], subtex.DrawCoords[2], subtex.DrawCoords[3],
                subtex.TexCoords[0], subtex.TexCoords[1], subtex.TexCoords[2], subtex.TexCoords[3],
                color, washed);

            MatrixStack = was;
        }

        public void Image(Subtexture subtex, RectInt clip, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Text

        public void Text(SpriteFont font, string text, Color color)
        {
            Text(font, text.AsSpan(), color);
        }

        public void Text(SpriteFont font, ReadOnlySpan<char> text, Color color)
        {
            var position = new Vector2(0, font.Ascent);

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    position.X = 0;
                    position.Y += font.LineHeight;
                    continue;
                }

                if (!font.Charset.TryGetValue(text[i], out var ch))
                    continue;

                if (ch.Image != null)
                {
                    var at = position + ch.Offset;

                    if (i < text.Length - 1 && text[i + 1] != '\n')
                    {
                        if (ch.Kerning.TryGetValue(text[i + 1], out float kerning))
                            at.X += kerning;
                    }

                    Image(ch.Image, at, color, true);
                }

                position.X += ch.Advance;
            }
        }

        #endregion

        #region Internal Utils

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushTriangle()
        {
            while (triangleCount + 3 >= triangles.Length)
            {
                Array.Resize(ref triangles, triangles.Length * 2);
            }

            triangles[triangleCount + 0] = vertexCount + 0;
            triangles[triangleCount + 1] = vertexCount + 1;
            triangles[triangleCount + 2] = vertexCount + 2;

            triangleCount += 3;
            currentBatch.Elements++;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushQuad()
        {
            while (triangleCount + 6 >= triangles.Length)
            {
                Array.Resize(ref triangles, triangles.Length * 2);
            }

            triangles[triangleCount + 0] = vertexCount + 0;
            triangles[triangleCount + 1] = vertexCount + 1;
            triangles[triangleCount + 2] = vertexCount + 2;
            triangles[triangleCount + 3] = vertexCount + 0;
            triangles[triangleCount + 4] = vertexCount + 2;
            triangles[triangleCount + 5] = vertexCount + 3;

            triangleCount += 6;
            currentBatch.Elements += 2;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandVertices(int index)
        {
            while (index >= vertices.Length)
            {
                Array.Resize(ref vertices, vertices.Length * 2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Transform(ref Vector2 to, ref Vector2 position, ref Matrix2D matrix)
        {
            to.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M31;
            to.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M32;
        }

        private void FlipYUVs(ref Vector2 uv0, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3)
        {
            var temp = uv0.Y;
            uv0.Y = uv3.Y;
            uv3.Y = temp;

            temp = uv1.Y;
            uv1.Y = uv2.Y;
            uv2.Y = temp;
        }

        #endregion
    }
}
