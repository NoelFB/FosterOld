using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    /// <summary>
    /// A 2D Sprite Batcher used for drawing images, text, and shapes
    /// </summary>
    public class Batch2D
    {

        public static readonly VertexFormat VertexFormat = new VertexFormat(
            new VertexAttribute("a_position",    VertexAttrib.Position,  VertexType.Float,   VertexComponents.Two,     false),
            new VertexAttribute("a_tex",         VertexAttrib.TexCoord0, VertexType.Float,   VertexComponents.Two,     false),
            new VertexAttribute("a_color",       VertexAttrib.Color0,    VertexType.Byte,    VertexComponents.Four,    true),
            new VertexAttribute("a_type",        VertexAttrib.TexCoord1, VertexType.Byte,    VertexComponents.Three,   true));

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

        private static Shader? defaultBatchShader;

        public readonly Graphics Graphics;
        public readonly Shader DefaultShader;
        public readonly Material DefaultMaterial;
        public readonly Mesh Mesh;
        public Matrix3x2 MatrixStack = Matrix3x2.Identity;

        public string TextureUniformName = "u_texture";
        public string MatrixUniformName = "u_matrix";

        private readonly Stack<Matrix3x2> matrixStack = new Stack<Matrix3x2>();
        private Vertex[] vertices;
        private int[] indices;
        private RenderPass pass;
        private readonly List<Batch> batches;
        private Batch currentBatch;
        private int currentBatchInsert;
        private bool dirty;

        public int TriangleCount => IndicesCount / 3;
        public int VertexCount { get; private set; }
        public int IndicesCount { get; private set; }
        public int BatchCount => batches.Count + (currentBatch.Elements > 0 ? 1 : 0);

        private struct Batch
        {
            public int Layer;
            public Material? Material;
            public BlendMode BlendMode;
            public Matrix3x2 Matrix;
            public Texture? Texture;
            public RectInt? Scissor;
            public uint Offset;
            public uint Elements;

            public Batch(Material? material, BlendMode blend, Texture? texture, Matrix3x2 matrix, uint offset, uint elements)
            {
                Layer = 0;
                Material = material;
                BlendMode = blend;
                Texture = texture;
                Matrix = matrix;
                Scissor = null;
                Offset = offset;
                Elements = elements;
            }
        }

        public Batch2D() : this(App.Graphics)
        {

        }

        public Batch2D(Graphics graphics)
        {
            Graphics = graphics;

            if (defaultBatchShader == null)
                defaultBatchShader = new Shader(graphics, graphics.CreateShaderSourceBatch2D());

            DefaultShader = defaultBatchShader;
            DefaultMaterial = new Material(DefaultShader);

            Mesh = new Mesh(graphics);

            vertices = new Vertex[64];
            indices = new int[64];
            batches = new List<Batch>();

            Clear();
        }

        public void Clear()
        {
            VertexCount = 0;
            IndicesCount = 0;
            currentBatchInsert = 0;
            currentBatch = new Batch(null, BlendMode.Normal, null, Matrix3x2.Identity, 0, 0);
            batches.Clear();
            matrixStack.Clear();
            MatrixStack = Matrix3x2.Identity;
        }

        #region Rendering

        public void Render(RenderTarget target)
        {
            var matrix = Matrix4x4.CreateOrthographicOffCenter(0, target.RenderWidth, target.RenderHeight, 0, 0, float.MaxValue);
            Render(target, matrix);
        }

        public void Render(RenderTarget target, Matrix4x4 matrix, RectInt? viewport = null)
        {
            pass = new RenderPass(target, Mesh, DefaultMaterial);
            pass.Viewport = viewport;

            Debug.Assert(matrixStack.Count <= 0, "Batch.MatrixStack Pushes more than it Pops");

            if (batches.Count > 0 || currentBatch.Elements > 0)
            {
                if (dirty)
                {
                    Mesh.SetVertices(new ReadOnlyMemory<Vertex>(vertices, 0, VertexCount));
                    Mesh.SetIndices(new ReadOnlyMemory<int>(indices, 0, IndicesCount));

                    dirty = false;
                }

                // render batches
                for (int i = 0; i < batches.Count; i++)
                {
                    // remaining elements in the current batch
                    if (currentBatchInsert == i && currentBatch.Elements > 0)
                        RenderBatch(currentBatch, matrix);

                    // render the batch
                    RenderBatch(batches[i], matrix);
                }

                // remaining elements in the current batch
                if (currentBatchInsert == batches.Count && currentBatch.Elements > 0)
                    RenderBatch(currentBatch, matrix);
            }
        }

        private void RenderBatch(in Batch batch, in Matrix4x4 matrix)
        {
            pass.Scissor = batch.Scissor;
            pass.BlendMode = batch.BlendMode;

            // Render the Mesh
            // Note we apply the texture and matrix based on the current batch
            // If the user set these on the Material themselves, they will be overwritten here

            pass.Material = batch.Material ?? DefaultMaterial;
            pass.Material[TextureUniformName]?.SetTexture(batch.Texture);
            pass.Material[MatrixUniformName]?.SetMatrix4x4(new Matrix4x4(batch.Matrix) * matrix);

            pass.MeshIndexStart = batch.Offset;
            pass.MeshIndexCount = batch.Elements;
            pass.MeshInstanceCount = 0;

            Graphics.Render(ref pass);
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

        public void SetMatrix(Matrix3x2 matrix)
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

            currentBatch.Layer = layer;
            currentBatchInsert = insert;
        }

        public void SetState(Material? material, BlendMode blendmode, Matrix3x2 matrix, RectInt? scissor)
        {
            SetMaterial(material);
            SetBlendMode(blendmode);
            SetMatrix(matrix);
            SetScissor(scissor);
        }

        public Matrix3x2 PushMatrix(Vector2 position, Vector2 scale, Vector2 origin, float rotation, bool relative = true)
        {
            return PushMatrix(Transform2D.CreateMatrix(position, origin, scale, rotation), relative);
        }

        public Matrix3x2 PushMatrix(Transform2D transform, bool relative = true)
        {
            return PushMatrix(transform.WorldMatrix, relative);
        }

        public Matrix3x2 PushMatrix(Vector2 position, bool relative = true)
        {
            return PushMatrix(Matrix3x2.CreateTranslation(position.X, position.Y), relative);
        }

        public Matrix3x2 PushMatrix(Matrix3x2 matrix, bool relative = true)
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

        public Matrix3x2 PopMatrix()
        {
            Debug.Assert(matrixStack.Count > 0, "Batch.MatrixStack Pops more than it Pushes");

            if (matrixStack.Count > 0)
            {
                MatrixStack = matrixStack.Pop();
            }
            else
            {
                MatrixStack = Matrix3x2.Identity;
            }

            return MatrixStack;
        }

        #endregion

        #region Line

        public void Line(Vector2 from, Vector2 to, float thickness, Color color)
        {
            var normal = (to - from).Normalized();
            var perp = new Vector2(-normal.Y, normal.X) * thickness * .5f;
            Quad(from + perp, from - perp, to - perp, to + perp, color);
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
            ExpandVertices(VertexCount + 4);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), VertexCount, 4);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[VertexCount + 3].Pos, v3, MatrixStack);

            VertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 t0, Vector2 t1, Vector2 t2, Vector2 t3, Color color, bool washed = false)
        {
            PushQuad();
            ExpandVertices(VertexCount + 4);

            if (Graphics.OriginBottomLeft && (currentBatch.Texture?.IsFrameBuffer ?? false))
                VerticalFlip(ref t0, ref t1, ref t2, ref t3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, t0, color, washed ? 0 : 255, washed ? 255 : 0, 0), VertexCount, 4);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[VertexCount + 3].Pos, v3, MatrixStack);

            vertices[VertexCount + 1].Tex = t1;
            vertices[VertexCount + 2].Tex = t2;
            vertices[VertexCount + 3].Tex = t3;

            VertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Color c0, Color c1, Color c2, Color c3)
        {
            PushQuad();
            ExpandVertices(VertexCount + 4);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, c0, 0, 0, 255), VertexCount, 4);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[VertexCount + 3].Pos, v3, MatrixStack);

            vertices[VertexCount + 1].Col = c1;
            vertices[VertexCount + 2].Col = c2;
            vertices[VertexCount + 3].Col = c3;

            VertexCount += 4;
        }

        public void Quad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 t0, Vector2 t1, Vector2 t2, Vector2 t3, Color c0, Color c1, Color c2, Color c3, bool washed = false)
        {
            PushQuad();
            ExpandVertices(VertexCount + 4);

            if (Graphics.OriginBottomLeft && (currentBatch.Texture?.IsFrameBuffer ?? false))
                VerticalFlip(ref t0, ref t1, ref t2, ref t3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, t0, c0, washed ? 0 : 255, washed ? 255 : 0, 0), VertexCount, 4);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[VertexCount + 3].Pos, v3, MatrixStack);

            vertices[VertexCount + 1].Col = c1;
            vertices[VertexCount + 1].Tex = t1;
            vertices[VertexCount + 2].Col = c2;
            vertices[VertexCount + 2].Tex = t2;
            vertices[VertexCount + 3].Col = c3;
            vertices[VertexCount + 3].Tex = t3;

            VertexCount += 4;
        }

        #endregion

        #region Triangle

        public void Triangle(Vector2 v0, Vector2 v1, Vector2 v2, Color color)
        {
            PushTriangle();
            ExpandVertices(VertexCount + 3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), VertexCount, 3);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);

            VertexCount += 3;
        }

        public void Triangle(Vector2 v0, Vector2 v1, Vector2 v2, Color c0, Color c1, Color c2)
        {
            PushTriangle();
            ExpandVertices(VertexCount + 3);

            Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, c0, 0, 0, 255), VertexCount, 3);

            Transform(ref vertices[VertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[VertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[VertexCount + 2].Pos, v2, MatrixStack);

            vertices[VertexCount + 1].Col = c1;
            vertices[VertexCount + 2].Col = c2;

            VertexCount += 3;
        }

        #endregion

        #region Rect

        public void Rect(Rect rect, Color color)
        {
            Quad(
                new Vector2(rect.X, rect.Y), 
                new Vector2(rect.X + rect.Width, rect.Y), 
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height), 
                new Vector2(rect.X, rect.Y + rect.Height), 
                color);
        }

        public void Rect(Vector2 position, Vector2 size, Color color)
        {
            Quad(
                position, 
                position + new Vector2(size.X, 0), 
                position + new Vector2(size.X, size.Y), 
                position + new Vector2(0, size.Y), 
                color);
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
                new Vector2(rect.X, rect.Y + rect.Height), 
                c0, c1, c2, c3);
        }

        public void Rect(Vector2 position, Vector2 size, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                position, 
                position + new Vector2(size.X, 0),
                position + new Vector2(size.X, size.Y), 
                position + new Vector2(0, size.Y), 
                c0, c1, c2, c3);
        }

        public void Rect(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                new Vector2(x, y), 
                new Vector2(x + width, y), 
                new Vector2(x + width, y + height), 
                new Vector2(x, y + height), 
                c0, c1, c2, c3);
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
                var r0_tl = rect.A;
                var r0_tr = r0_tl + new Vector2(r0, 0);
                var r0_br = r0_tl + new Vector2(r0, r0);
                var r0_bl = r0_tl + new Vector2(0, r0);

                var r1_tl = rect.B + new Vector2(-r1, 0);
                var r1_tr = r1_tl + new Vector2(r1, 0);
                var r1_br = r1_tl + new Vector2(r1, r1);
                var r1_bl = r1_tl + new Vector2(0, r1);

                var r2_tl = rect.C + new Vector2(-r2, -r2);
                var r2_tr = r2_tl + new Vector2(r2, 0);
                var r2_bl = r2_tl + new Vector2(0, r2);
                var r2_br = r2_tl + new Vector2(r2, r2);

                var r3_tl = rect.D + new Vector2(0, -r3);
                var r3_tr = r3_tl + new Vector2(r3, 0);
                var r3_bl = r3_tl + new Vector2(0, r3);
                var r3_br = r3_tl + new Vector2(r3, r3);

                // set tris
                {
                    while (IndicesCount + 30 >= indices.Length)
                        Array.Resize(ref indices, indices.Length * 2);

                    // top quad
                    {
                        indices[IndicesCount + 00] = VertexCount + 00; // r0b
                        indices[IndicesCount + 01] = VertexCount + 03; // r1a
                        indices[IndicesCount + 02] = VertexCount + 05; // r1d

                        indices[IndicesCount + 03] = VertexCount + 00; // r0b
                        indices[IndicesCount + 04] = VertexCount + 05; // r1d
                        indices[IndicesCount + 05] = VertexCount + 01; // r0c
                    }

                    // left quad
                    {
                        indices[IndicesCount + 06] = VertexCount + 02; // r0d
                        indices[IndicesCount + 07] = VertexCount + 01; // r0c
                        indices[IndicesCount + 08] = VertexCount + 10; // r3b

                        indices[IndicesCount + 09] = VertexCount + 02; // r0d
                        indices[IndicesCount + 10] = VertexCount + 10; // r3b
                        indices[IndicesCount + 11] = VertexCount + 09; // r3a
                    }

                    // right quad
                    {
                        indices[IndicesCount + 12] = VertexCount + 05; // r1d
                        indices[IndicesCount + 13] = VertexCount + 04; // r1c
                        indices[IndicesCount + 14] = VertexCount + 07; // r2b

                        indices[IndicesCount + 15] = VertexCount + 05; // r1d
                        indices[IndicesCount + 16] = VertexCount + 07; // r2b
                        indices[IndicesCount + 17] = VertexCount + 06; // r2a
                    }

                    // bottom quad
                    {
                        indices[IndicesCount + 18] = VertexCount + 10; // r3b
                        indices[IndicesCount + 19] = VertexCount + 06; // r2a
                        indices[IndicesCount + 20] = VertexCount + 08; // r2d

                        indices[IndicesCount + 21] = VertexCount + 10; // r3b
                        indices[IndicesCount + 22] = VertexCount + 08; // r2d
                        indices[IndicesCount + 23] = VertexCount + 11; // r3c
                    }

                    // center quad
                    {
                        indices[IndicesCount + 24] = VertexCount + 01; // r0c
                        indices[IndicesCount + 25] = VertexCount + 05; // r1d
                        indices[IndicesCount + 26] = VertexCount + 06; // r2a

                        indices[IndicesCount + 27] = VertexCount + 01; // r0c
                        indices[IndicesCount + 28] = VertexCount + 06; // r2a
                        indices[IndicesCount + 29] = VertexCount + 10; // r3b
                    }

                    IndicesCount += 30;
                    currentBatch.Elements += 10;
                    dirty = true;
                }

                // set verts
                {
                    ExpandVertices(VertexCount + 12);

                    Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), VertexCount, 12);

                    Transform(ref vertices[VertexCount + 00].Pos, r0_tr, MatrixStack); // 0
                    Transform(ref vertices[VertexCount + 01].Pos, r0_br, MatrixStack); // 1
                    Transform(ref vertices[VertexCount + 02].Pos, r0_bl, MatrixStack); // 2

                    Transform(ref vertices[VertexCount + 03].Pos, r1_tl, MatrixStack); // 3
                    Transform(ref vertices[VertexCount + 04].Pos, r1_br, MatrixStack); // 4
                    Transform(ref vertices[VertexCount + 05].Pos, r1_bl, MatrixStack); // 5

                    Transform(ref vertices[VertexCount + 06].Pos, r2_tl, MatrixStack); // 6
                    Transform(ref vertices[VertexCount + 07].Pos, r2_tr, MatrixStack); // 7
                    Transform(ref vertices[VertexCount + 08].Pos, r2_bl, MatrixStack); // 8

                    Transform(ref vertices[VertexCount + 09].Pos, r3_tl, MatrixStack); // 9
                    Transform(ref vertices[VertexCount + 10].Pos, r3_tr, MatrixStack); // 10
                    Transform(ref vertices[VertexCount + 11].Pos, r3_br, MatrixStack); // 11

                    VertexCount += 12;
                }

                // TODO: replace with hard-coded values
                var left = Calc.Angle(-Vector2.UnitX);
                var right = Calc.Angle(Vector2.UnitX);
                var up = Calc.Angle(-Vector2.UnitY);
                var down = Calc.Angle(Vector2.UnitY);

                // top-left corner
                if (r0 > 0)
                    SemiCircle(r0_br, left, up, r0, Math.Max(3, (int)(r0 / 4)), color);
                else
                    Quad(r0_tl, r0_tr, r0_br, r0_bl, color);

                // top-right corner
                if (r1 > 0)
                    SemiCircle(r1_bl, up, right, r1, Math.Max(3, (int)(r1 / 4)), color);
                else
                    Quad(r1_tl, r1_tr, r1_br, r1_bl, color);

                // bottom-right corner
                if (r2 > 0)
                    SemiCircle(r2_tl, right, down, r2, Math.Max(3, (int)(r2 / 4)), color);
                else
                    Quad(r2_tl, r2_tr, r2_br, r2_bl, color);

                // bottom-left corner
                if (r3 > 0)
                    SemiCircle(r3_tr, down, left, r3, Math.Max(3, (int)(r3 / 4)), color);
                else
                    Quad(r3_tl, r3_tr, r3_br, r3_bl, color);
            }

        }

        public void SemiCircle(Vector2 center, float startRadians, float endRadians, float radius, int steps, Color color)
        {
            var last = Calc.AngleToVector(startRadians, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Calc.AngleToVector(Calc.AngleLerp(startRadians, endRadians, (i / (float)steps)), radius);
                Triangle(center + last, center + next, center, color);
                last = next;
            }
        }

        public void Circle(Vector2 center, float radius, int steps, Color color)
        {
            var last = Calc.AngleToVector(0, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Calc.AngleToVector((i / (float)steps) * Calc.TAU, radius);
                Triangle(center + last, center + next, center, color);
                last = next;
            }
        }

        public void HollowCircle(Vector2 center, float radius, float thickness, int steps, Color color)
        {
            var last = Calc.AngleToVector(0, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Calc.AngleToVector((i / (float)steps) * Calc.TAU, radius);
                Line(center + last, center + next, thickness, color);
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
                Rect(rect.X, rect.MaxY - ty, rect.Width, ty, color);
                Rect(rect.X, rect.Y + ty, tx, rect.Height - ty * 2, color);
                Rect(rect.MaxX - tx, rect.Y + ty, tx, rect.Height - ty * 2, color);
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
                new Vector2(0, 0), 
                new Vector2(texture.Width, 0), 
                new Vector2(texture.Width, texture.Height), 
                new Vector2(0, texture.Height),
                new Vector2(0, 0), 
                Vector2.UnitX, 
                new Vector2(1, 1), 
                Vector2.UnitY, 
                color, washed);
        }

        public void Image(Texture texture, Vector2 position, Color color, bool washed = false)
        {
            SetTexture(texture);
            Quad(
                position, 
                position + new Vector2(texture.Width, 0), 
                position + new Vector2(texture.Width, texture.Height),
                position + new Vector2(0, texture.Height),
                new Vector2(0, 0), 
                Vector2.UnitX, 
                new Vector2(1, 1), 
                Vector2.UnitY, 
                color, washed);
        }

        public void Image(Texture texture, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            SetTexture(texture);
            Quad(
                new Vector2(0, 0), 
                new Vector2(texture.Width, 0), 
                new Vector2(texture.Width, texture.Height), 
                new Vector2(0, texture.Height),
                new Vector2(0, 0), 
                Vector2.UnitX, 
                new Vector2(1, 1), 
                Vector2.UnitY, 
                color, washed);

            MatrixStack = was;
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Color color, bool washed = false)
        {
            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.MaxX / texture.Width;
            var ty1 = clip.MaxY / texture.Height;

            SetTexture(texture);
            Quad(
                position, 
                position + new Vector2(clip.Width, 0), 
                position + new Vector2(clip.Width, clip.Height), 
                position + new Vector2(0, clip.Height),
                new Vector2(tx0, ty0), 
                new Vector2(tx1, ty0), 
                new Vector2(tx1, ty1), 
                new Vector2(tx0, ty1), color, washed);
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.MaxX / texture.Width;
            var ty1 = clip.MaxY / texture.Height;

            SetTexture(texture);
            Quad(
                new Vector2(0, 0), 
                new Vector2(clip.Width, 0), 
                new Vector2(clip.Width, clip.Height), 
                new Vector2(0, clip.Height),
                new Vector2(tx0, ty0), 
                new Vector2(tx1, ty0), 
                new Vector2(tx1, ty1), 
                new Vector2(tx0, ty1), 
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

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            SetTexture(subtex.Texture);
            Quad(
                subtex.DrawCoords[0], subtex.DrawCoords[1], subtex.DrawCoords[2], subtex.DrawCoords[3],
                subtex.TexCoords[0], subtex.TexCoords[1], subtex.TexCoords[2], subtex.TexCoords[3],
                color, washed);

            MatrixStack = was;
        }

        public void Image(Subtexture subtex, Rect clip, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var (source, frame) = subtex.GetClip(clip);
            var tex = subtex.Texture;
            var was = MatrixStack;

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            // pos
            float px0 = -frame.X, px1 = -frame.X + source.Width,
                  py0 = -frame.Y, py1 = -frame.Y + source.Height;

            // tex-coords
            float tx0 = 0, tx1 = 0, ty0 = 0, ty1 = 0;
            if (tex != null)
            {
                tx0 = source.MinX / tex.Width;
                tx1 = source.MaxX / tex.Width;
                ty0 = source.MinY / tex.Width;
                ty1 = source.MaxY / tex.Width;
            }

            SetTexture(subtex.Texture);
            Quad(
                new Vector2(px0, py0), new Vector2(px1, py0), new Vector2(px1, py1), new Vector2(px0, py1),
                new Vector2(tx0, ty0), new Vector2(tx1, ty0), new Vector2(tx1, ty1), new Vector2(tx0, ty1),
                color, washed);

            MatrixStack = was;
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

        public void Text(SpriteFont font, string text, Vector2 position, Color color)
        {
            PushMatrix(position);
            Text(font, text.AsSpan(), color);
            PopMatrix();
        }

        public void Text(SpriteFont font, ReadOnlySpan<char> text, Vector2 position, Color color)
        {
            PushMatrix(position);
            Text(font, text, color);
            PopMatrix();
        }

        public void Text(SpriteFont font, string text, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color)
        {
            PushMatrix(position, scale, origin, rotation);
            Text(font, text.AsSpan(), color);
            PopMatrix();
        }

        public void Text(SpriteFont font, ReadOnlySpan<char> text, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color)
        {
            PushMatrix(position, scale, origin, rotation);
            Text(font, text, color);
            PopMatrix();
        }

        #endregion

        #region Misc.

        public void CheckeredPattern(Rect bounds, float cellWidth, float cellHeight, Color a, Color b)
        {
            var odd = false;

            for (float y = bounds.MinY; y < bounds.MaxY; y += cellHeight)
            {
                var cells = 0;
                for (float x = bounds.MinX; x < bounds.MaxX; x += cellWidth)
                {
                    var color = (odd ? a : b);
                    if (color.A > 0)
                        Rect(x, y, Math.Min(bounds.MaxX - x, cellWidth), Math.Min(bounds.MaxY - y, cellHeight), color);

                    odd = !odd;
                    cells++;
                }

                if (cells % 2 == 0)
                    odd = !odd;
            }
        }

        #endregion

        #region Internal Utils

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushTriangle()
        {
            while (IndicesCount + 3 >= indices.Length)
            {
                Array.Resize(ref indices, indices.Length * 2);
            }

            indices[IndicesCount + 0] = VertexCount + 0;
            indices[IndicesCount + 1] = VertexCount + 1;
            indices[IndicesCount + 2] = VertexCount + 2;

            IndicesCount += 3;
            currentBatch.Elements++;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushQuad()
        {
            while (IndicesCount + 6 >= indices.Length)
            {
                Array.Resize(ref indices, indices.Length * 2);
            }

            indices[IndicesCount + 0] = VertexCount + 0;
            indices[IndicesCount + 1] = VertexCount + 1;
            indices[IndicesCount + 2] = VertexCount + 2;
            indices[IndicesCount + 3] = VertexCount + 0;
            indices[IndicesCount + 4] = VertexCount + 2;
            indices[IndicesCount + 5] = VertexCount + 3;

            IndicesCount += 6;
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
        private void Transform(ref Vector2 to, in Vector2 position, in Matrix3x2 matrix)
        {
            to.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M31;
            to.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M32;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VerticalFlip(ref Vector2 uv0, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3)
        {
            uv0.Y = 1 - uv0.Y;
            uv1.Y = 1 - uv1.Y;
            uv2.Y = 1 - uv2.Y;
            uv3.Y = 1 - uv3.Y;
        }

        #endregion
    }
}
