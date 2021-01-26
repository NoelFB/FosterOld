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
            new VertexAttribute("a_position", VertexAttrib.Position, VertexType.Float, VertexComponents.Two, false),
            new VertexAttribute("a_tex", VertexAttrib.TexCoord0, VertexType.Float, VertexComponents.Two, false),
            new VertexAttribute("a_color", VertexAttrib.Color0, VertexType.Byte, VertexComponents.Four, true),
            new VertexAttribute("a_type", VertexAttrib.TexCoord1, VertexType.Byte, VertexComponents.Three, true));

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
        public RectInt? Scissor => currentBatch.Scissor;

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
        private int vertexCount;
        private int indexCount;

        public int TriangleCount => indexCount / 3;
        public int VertexCount => vertexCount;
        public int IndexCount => indexCount;
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
            vertexCount = 0;
            indexCount = 0;
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

        public void Render(RenderTarget target, Color clearColor)
        {
            App.Graphics.Clear(target, clearColor);
            Render(target);
        }

        public void Render(RenderTarget target, Matrix4x4 matrix, RectInt? viewport = null, Color? clearColor = null)
        {
            if (clearColor != null)
                App.Graphics.Clear(target, clearColor.Value);

            pass = new RenderPass(target, Mesh, DefaultMaterial);
            pass.Viewport = viewport;

            Debug.Assert(matrixStack.Count <= 0, "Batch.MatrixStack Pushes more than it Pops");

            if (batches.Count > 0 || currentBatch.Elements > 0)
            {
                if (dirty)
                {
                    Mesh.SetVertices(new ReadOnlyMemory<Vertex>(vertices, 0, vertexCount));
                    Mesh.SetIndices(new ReadOnlyMemory<int>(indices, 0, indexCount));

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

            pass.MeshIndexStart = batch.Offset * 3;
            pass.MeshIndexCount = batch.Elements * 3;
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

        public void SetBlendMode(in BlendMode blendmode)
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

        public BlendMode GetBlendMode()
        {
            return currentBatch.BlendMode;
        }

        public void SetMatrix(in Matrix3x2 matrix)
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

        public void SetState(Material? material, in BlendMode blendmode, in Matrix3x2 matrix, RectInt? scissor)
        {
            SetMaterial(material);
            SetBlendMode(blendmode);
            SetMatrix(matrix);
            SetScissor(scissor);
        }

        public Matrix3x2 PushMatrix(in Vector2 position, in Vector2 scale, in Vector2 origin, float rotation, bool relative = true)
        {
            return PushMatrix(Transform2D.CreateMatrix(position, origin, scale, rotation), relative);
        }

        public Matrix3x2 PushMatrix(Transform2D transform, bool relative = true)
        {
            return PushMatrix(transform.WorldMatrix, relative);
        }

        public Matrix3x2 PushMatrix(in Vector2 position, bool relative = true)
        {
            return PushMatrix(Matrix3x2.CreateTranslation(position.X, position.Y), relative);
        }

        public Matrix3x2 PushMatrix(in Matrix3x2 matrix, bool relative = true)
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

        public void DashedLine(Vector2 from, Vector2 to, float thickness, Color color, float dashLength, float offsetPercent)
        {
            var diff = to - from;
            var dist = diff.Length();
            var axis = diff.Normalized();
            var perp = axis.TurnLeft() * (thickness * 0.5f);
            offsetPercent = ((offsetPercent % 1f) + 1f) % 1f;

            var startD = dashLength * offsetPercent * 2f;
            if (startD > dashLength)
                startD -= dashLength * 2f;

            for (float d = startD; d < dist; d += dashLength * 2f)
            {
                var a = from + axis * Math.Max(d, 0f);
                var b = from + axis * Math.Min(d + dashLength, dist);
                Quad(a + perp, b + perp, b - perp, a - perp, color);
            }
        }

        #endregion

        #region Quad

        public void Quad(in Quad2D quad, Color color)
        {
            Quad(quad.A, quad.B, quad.C, quad.D, color);
        }

        public void Quad(in Vector2 v0, in Vector2 v1, in Vector2 v2, in Vector2 v3, Color color)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, v3, MatrixStack);

            // COL
            vertices[vertexCount + 0].Col = color;
            vertices[vertexCount + 1].Col = color;
            vertices[vertexCount + 2].Col = color;
            vertices[vertexCount + 3].Col = color;

            // MULT
            vertices[vertexCount + 0].Mult = 0;
            vertices[vertexCount + 1].Mult = 0;
            vertices[vertexCount + 2].Mult = 0;
            vertices[vertexCount + 3].Mult = 0;

            // WASH
            vertices[vertexCount + 0].Wash = 0;
            vertices[vertexCount + 1].Wash = 0;
            vertices[vertexCount + 2].Wash = 0;
            vertices[vertexCount + 3].Wash = 0;

            // FILL
            vertices[vertexCount + 0].Fill = 255;
            vertices[vertexCount + 1].Fill = 255;
            vertices[vertexCount + 2].Fill = 255;
            vertices[vertexCount + 3].Fill = 255;

            vertexCount += 4;
        }

        public void Quad(in Vector2 v0, in Vector2 v1, in Vector2 v2, in Vector2 v3, in Vector2 t0, in Vector2 t1, in Vector2 t2, in Vector2 t3, Color color, bool washed = false)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            var mult = (byte)(washed ? 0 : 255);
            var wash = (byte)(washed ? 255 : 0);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, v3, MatrixStack);

            // TEX
            vertices[vertexCount + 0].Tex = t0;
            vertices[vertexCount + 1].Tex = t1;
            vertices[vertexCount + 2].Tex = t2;
            vertices[vertexCount + 3].Tex = t3;

            if (Graphics.OriginBottomLeft && (currentBatch.Texture?.IsFrameBuffer ?? false))
                VerticalFlip(ref vertices[vertexCount + 0].Tex, ref vertices[vertexCount + 1].Tex, ref vertices[vertexCount + 2].Tex, ref vertices[vertexCount + 3].Tex);

            // COL
            vertices[vertexCount + 0].Col = color;
            vertices[vertexCount + 1].Col = color;
            vertices[vertexCount + 2].Col = color;
            vertices[vertexCount + 3].Col = color;

            // MULT
            vertices[vertexCount + 0].Mult = mult;
            vertices[vertexCount + 1].Mult = mult;
            vertices[vertexCount + 2].Mult = mult;
            vertices[vertexCount + 3].Mult = mult;

            // WASH
            vertices[vertexCount + 0].Wash = wash;
            vertices[vertexCount + 1].Wash = wash;
            vertices[vertexCount + 2].Wash = wash;
            vertices[vertexCount + 3].Wash = wash;

            // FILL
            vertices[vertexCount + 0].Fill = 0;
            vertices[vertexCount + 1].Fill = 0;
            vertices[vertexCount + 2].Fill = 0;
            vertices[vertexCount + 3].Fill = 0;

            vertexCount += 4;
        }

        public void Quad(in Vector2 v0, in Vector2 v1, in Vector2 v2, in Vector2 v3, Color c0, Color c1, Color c2, Color c3)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, v3, MatrixStack);

            // COL
            vertices[vertexCount + 0].Col = c0;
            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Col = c2;
            vertices[vertexCount + 3].Col = c3;

            // MULT
            vertices[vertexCount + 0].Mult = 0;
            vertices[vertexCount + 1].Mult = 0;
            vertices[vertexCount + 2].Mult = 0;
            vertices[vertexCount + 3].Mult = 0;

            // WASH
            vertices[vertexCount + 0].Wash = 0;
            vertices[vertexCount + 1].Wash = 0;
            vertices[vertexCount + 2].Wash = 0;
            vertices[vertexCount + 3].Wash = 0;

            // FILL
            vertices[vertexCount + 0].Fill = 255;
            vertices[vertexCount + 1].Fill = 255;
            vertices[vertexCount + 2].Fill = 255;
            vertices[vertexCount + 3].Fill = 255;

            vertexCount += 4;
        }

        public void Quad(in Vector2 v0, in Vector2 v1, in Vector2 v2, in Vector2 v3, in Vector2 t0, in Vector2 t1, in Vector2 t2, in Vector2 t3, Color c0, Color c1, Color c2, Color c3, bool washed = false)
        {
            PushQuad();
            ExpandVertices(vertexCount + 4);

            var mult = (byte)(washed ? 0 : 255);
            var wash = (byte)(washed ? 255 : 0);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);
            Transform(ref vertices[vertexCount + 3].Pos, v3, MatrixStack);

            // TEX
            vertices[vertexCount + 0].Tex = t0;
            vertices[vertexCount + 1].Tex = t1;
            vertices[vertexCount + 2].Tex = t2;
            vertices[vertexCount + 3].Tex = t3;

            if (Graphics.OriginBottomLeft && (currentBatch.Texture?.IsFrameBuffer ?? false))
                VerticalFlip(ref vertices[vertexCount + 0].Tex, ref vertices[vertexCount + 1].Tex, ref vertices[vertexCount + 2].Tex, ref vertices[vertexCount + 3].Tex);

            // COL
            vertices[vertexCount + 0].Col = c0;
            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Col = c2;
            vertices[vertexCount + 3].Col = c3;

            // MULT
            vertices[vertexCount + 0].Mult = mult;
            vertices[vertexCount + 1].Mult = mult;
            vertices[vertexCount + 2].Mult = mult;
            vertices[vertexCount + 3].Mult = mult;

            // WASH
            vertices[vertexCount + 0].Wash = wash;
            vertices[vertexCount + 1].Wash = wash;
            vertices[vertexCount + 2].Wash = wash;
            vertices[vertexCount + 3].Wash = wash;

            // FILL
            vertices[vertexCount + 0].Fill = 0;
            vertices[vertexCount + 1].Fill = 0;
            vertices[vertexCount + 2].Fill = 0;
            vertices[vertexCount + 3].Fill = 0;

            vertexCount += 4;
        }

        #endregion

        #region Triangle

        public void Triangle(in Vector2 v0, in Vector2 v1, in Vector2 v2, Color color)
        {
            PushTriangle();
            ExpandVertices(vertexCount + 3);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);

            // COL
            vertices[vertexCount + 0].Col = color;
            vertices[vertexCount + 1].Col = color;
            vertices[vertexCount + 2].Col = color;

            // MULT
            vertices[vertexCount + 0].Mult = 0;
            vertices[vertexCount + 1].Mult = 0;
            vertices[vertexCount + 2].Mult = 0;
            vertices[vertexCount + 3].Mult = 0;

            // WASH
            vertices[vertexCount + 0].Wash = 0;
            vertices[vertexCount + 1].Wash = 0;
            vertices[vertexCount + 2].Wash = 0;
            vertices[vertexCount + 3].Wash = 0;

            // FILL
            vertices[vertexCount + 0].Fill = 255;
            vertices[vertexCount + 1].Fill = 255;
            vertices[vertexCount + 2].Fill = 255;
            vertices[vertexCount + 3].Fill = 255;

            vertexCount += 3;
        }

        public void Triangle(in Vector2 v0, in Vector2 v1, in Vector2 v2, Color c0, Color c1, Color c2)
        {
            PushTriangle();
            ExpandVertices(vertexCount + 3);

            // POS
            Transform(ref vertices[vertexCount + 0].Pos, v0, MatrixStack);
            Transform(ref vertices[vertexCount + 1].Pos, v1, MatrixStack);
            Transform(ref vertices[vertexCount + 2].Pos, v2, MatrixStack);

            // COL
            vertices[vertexCount + 0].Col = c0;
            vertices[vertexCount + 1].Col = c1;
            vertices[vertexCount + 2].Col = c2;

            // MULT
            vertices[vertexCount + 0].Mult = 0;
            vertices[vertexCount + 1].Mult = 0;
            vertices[vertexCount + 2].Mult = 0;
            vertices[vertexCount + 3].Mult = 0;

            // WASH
            vertices[vertexCount + 0].Wash = 0;
            vertices[vertexCount + 1].Wash = 0;
            vertices[vertexCount + 2].Wash = 0;
            vertices[vertexCount + 3].Wash = 0;

            // FILL
            vertices[vertexCount + 0].Fill = 255;
            vertices[vertexCount + 1].Fill = 255;
            vertices[vertexCount + 2].Fill = 255;
            vertices[vertexCount + 3].Fill = 255;

            vertexCount += 3;
        }

        #endregion

        #region Rect

        public void Rect(in Rect rect, Color color)
        {
            Quad(
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height),
                color);
        }

        public void Rect(in Vector2 position, in Vector2 size, Color color)
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

        public void Rect(in Rect rect, Color c0, Color c1, Color c2, Color c3)
        {
            Quad(
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height),
                c0, c1, c2, c3);
        }

        public void Rect(in Vector2 position, in Vector2 size, Color c0, Color c1, Color c2, Color c3)
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

        public void RoundedRect(in Rect rect, float radius, Color color)
        {
            RoundedRect(rect, radius, radius, radius, radius, color);
        }

        public void RoundedRect(in Rect rect, float r0, float r1, float r2, float r3, Color color)
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
                    while (indexCount + 30 >= indices.Length)
                        Array.Resize(ref indices, indices.Length * 2);

                    // top quad
                    {
                        indices[indexCount + 00] = vertexCount + 00; // r0b
                        indices[indexCount + 01] = vertexCount + 03; // r1a
                        indices[indexCount + 02] = vertexCount + 05; // r1d

                        indices[indexCount + 03] = vertexCount + 00; // r0b
                        indices[indexCount + 04] = vertexCount + 05; // r1d
                        indices[indexCount + 05] = vertexCount + 01; // r0c
                    }

                    // left quad
                    {
                        indices[indexCount + 06] = vertexCount + 02; // r0d
                        indices[indexCount + 07] = vertexCount + 01; // r0c
                        indices[indexCount + 08] = vertexCount + 10; // r3b

                        indices[indexCount + 09] = vertexCount + 02; // r0d
                        indices[indexCount + 10] = vertexCount + 10; // r3b
                        indices[indexCount + 11] = vertexCount + 09; // r3a
                    }

                    // right quad
                    {
                        indices[indexCount + 12] = vertexCount + 05; // r1d
                        indices[indexCount + 13] = vertexCount + 04; // r1c
                        indices[indexCount + 14] = vertexCount + 07; // r2b

                        indices[indexCount + 15] = vertexCount + 05; // r1d
                        indices[indexCount + 16] = vertexCount + 07; // r2b
                        indices[indexCount + 17] = vertexCount + 06; // r2a
                    }

                    // bottom quad
                    {
                        indices[indexCount + 18] = vertexCount + 10; // r3b
                        indices[indexCount + 19] = vertexCount + 06; // r2a
                        indices[indexCount + 20] = vertexCount + 08; // r2d

                        indices[indexCount + 21] = vertexCount + 10; // r3b
                        indices[indexCount + 22] = vertexCount + 08; // r2d
                        indices[indexCount + 23] = vertexCount + 11; // r3c
                    }

                    // center quad
                    {
                        indices[indexCount + 24] = vertexCount + 01; // r0c
                        indices[indexCount + 25] = vertexCount + 05; // r1d
                        indices[indexCount + 26] = vertexCount + 06; // r2a

                        indices[indexCount + 27] = vertexCount + 01; // r0c
                        indices[indexCount + 28] = vertexCount + 06; // r2a
                        indices[indexCount + 29] = vertexCount + 10; // r3b
                    }

                    indexCount += 30;
                    currentBatch.Elements += 10;
                    dirty = true;
                }

                // set verts
                {
                    ExpandVertices(vertexCount + 12);

                    Array.Fill(vertices, new Vertex(Vector2.Zero, Vector2.Zero, color, 0, 0, 255), vertexCount, 12);

                    Transform(ref vertices[vertexCount + 00].Pos, r0_tr, MatrixStack); // 0
                    Transform(ref vertices[vertexCount + 01].Pos, r0_br, MatrixStack); // 1
                    Transform(ref vertices[vertexCount + 02].Pos, r0_bl, MatrixStack); // 2

                    Transform(ref vertices[vertexCount + 03].Pos, r1_tl, MatrixStack); // 3
                    Transform(ref vertices[vertexCount + 04].Pos, r1_br, MatrixStack); // 4
                    Transform(ref vertices[vertexCount + 05].Pos, r1_bl, MatrixStack); // 5

                    Transform(ref vertices[vertexCount + 06].Pos, r2_tl, MatrixStack); // 6
                    Transform(ref vertices[vertexCount + 07].Pos, r2_tr, MatrixStack); // 7
                    Transform(ref vertices[vertexCount + 08].Pos, r2_bl, MatrixStack); // 8

                    Transform(ref vertices[vertexCount + 09].Pos, r3_tl, MatrixStack); // 9
                    Transform(ref vertices[vertexCount + 10].Pos, r3_tr, MatrixStack); // 10
                    Transform(ref vertices[vertexCount + 11].Pos, r3_br, MatrixStack); // 11

                    vertexCount += 12;
                }

                // TODO: replace with hard-coded values
                var left = Calc.Angle(-Vector2.UnitX);
                var right = Calc.Angle(Vector2.UnitX);
                var up = Calc.Angle(-Vector2.UnitY);
                var down = Calc.Angle(Vector2.UnitY);

                // top-left corner
                if (r0 > 0)
                    SemiCircle(r0_br, up, -left, r0, Math.Max(3, (int)(r0 / 4)), color);
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

        #endregion

        #region Circle

        public void SemiCircle(Vector2 center, float startRadians, float endRadians, float radius, int steps, Color color)
        {
            SemiCircle(center, startRadians, endRadians, radius, steps, color, color);
        }

        public void SemiCircle(Vector2 center, float startRadians, float endRadians, float radius, int steps, Color centerColor, Color edgeColor)
        {
            var last = Calc.AngleToVector(startRadians, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Calc.AngleToVector(startRadians + (endRadians - startRadians) * (i / (float)steps), radius);
                Triangle(center + last, center + next, center, edgeColor, edgeColor, centerColor);
                last = next;
            }
        }

        public void Circle(Vector2 center, float radius, int steps, Color color)
        {
            Circle(center, radius, steps, color, color);
        }

        public void Circle(Vector2 center, float radius, int steps, Color centerColor, Color edgeColor)
        {
            var last = Calc.AngleToVector(0, radius);

            for (int i = 1; i <= steps; i++)
            {
                var next = Calc.AngleToVector((i / (float)steps) * Calc.TAU, radius);
                Triangle(center + last, center + next, center, edgeColor, edgeColor, centerColor);
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

        public void HollowRect(in Rect rect, float t, Color color)
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
            in Vector2 pos0, in Vector2 pos1, in Vector2 pos2, in Vector2 pos3,
            in Vector2 uv0, in Vector2 uv1, in Vector2 uv2, in Vector2 uv3,
            Color col0, Color col1, Color col2, Color col3, bool washed = false)
        {
            SetTexture(texture);
            Quad(pos0, pos1, pos2, pos3, uv0, uv1, uv2, uv3, col0, col1, col2, col3, washed);
        }

        public void Image(Texture texture,
            in Vector2 pos0, in Vector2 pos1, in Vector2 pos2, in Vector2 pos3,
            in Vector2 uv0, in Vector2 uv1, in Vector2 uv2, in Vector2 uv3,
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

        public void Image(Texture texture, in Vector2 position, Color color, bool washed = false)
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

        public void Image(Texture texture, in Vector2 position, in Vector2 scale, in Vector2 origin, float rotation, Color color, bool washed = false)
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

        public void Image(Texture texture, in Rect clip, in Vector2 position, Color color, bool washed = false)
        {
            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.Right / texture.Width;
            var ty1 = clip.Bottom / texture.Height;

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

        public void Image(Texture texture, in Rect clip, in Vector2 position, in Vector2 scale, in Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            var tx0 = clip.X / texture.Width;
            var ty0 = clip.Y / texture.Height;
            var tx1 = clip.Right / texture.Width;
            var ty1 = clip.Bottom / texture.Height;

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

        public void Image(Subtexture subtex, in Vector2 position, Color color, bool washed = false)
        {
            SetTexture(subtex.Texture);
            Quad(position + subtex.DrawCoords[0], position + subtex.DrawCoords[1], position + subtex.DrawCoords[2], position + subtex.DrawCoords[3],
                subtex.TexCoords[0], subtex.TexCoords[1], subtex.TexCoords[2], subtex.TexCoords[3],
                color, washed);
        }

        public void Image(Subtexture subtex, in Vector2 position, in Vector2 scale, in Vector2 origin, float rotation, Color color, bool washed = false)
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

        public void Image(Subtexture subtex, in Rect clip, in Vector2 position, in Vector2 scale, in Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var (source, frame) = subtex.GetClip(clip);
            var tex = subtex.Texture;
            var was = MatrixStack;

            MatrixStack = Transform2D.CreateMatrix(position, origin, scale, rotation) * MatrixStack;

            var px0 = -frame.X;
            var py0 = -frame.Y;
            var px1 = -frame.X + source.Width;
            var py1 = -frame.Y + source.Height;

            var tx0 = 0f;
            var ty0 = 0f;
            var tx1 = 0f;
            var ty1 = 0f;

            if (tex != null)
            {
                tx0 = source.Left / tex.Width;
                ty0 = source.Top / tex.Height;
                tx1 = source.Right / tex.Width;
                ty1 = source.Bottom / tex.Height;
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

        /// <summary>
        /// Draw text on the baseline, scaled to match `size`.
        /// For example: if the font was loaded at 10pt, and you set `size = 20`, the text will be scaled x2.
        /// </summary>
        public void Text(SpriteFont font, ReadOnlySpan<char> text, Vector2 position, int size, float rotation, Color color)
        {
            float s = size / (float)font.Size;
            var scale = new Vector2(s, s);
            var origin = new Vector2(0f, font.Ascent);
            PushMatrix(position, scale, origin, rotation);
            Text(font, text, color);
            PopMatrix();
        }

        /// <summary>
        /// Draw text on the baseline, scaled to match `size`.
        /// For example: if the font was loaded at 10pt, and you set `size = 20`, the text will be scaled x2.
        /// </summary>
        public void Text(SpriteFont font, string text, Vector2 position, int size, float rotation, Color color)
        {
            float s = size / (float)font.Size;
            var scale = new Vector2(s, s);
            var origin = new Vector2(0f, font.Ascent);
            PushMatrix(position, scale, origin, rotation);
            Text(font, text.AsSpan(), color);
            PopMatrix();
        }

        /// <summary>
        /// Draws the text scaled to fit into the provided rectangle, never exceeding the max font size.
        /// </summary>
        public void TextFitted(SpriteFont font, string text, in Rect rect, float maxSize, Color color)
        {
            var textSpan = text.AsSpan();
            var size = font.SizeOf(textSpan);
            var sx = rect.Width / size.X;
            var sy = rect.Height / font.Size;
            var scale = Math.Min(maxSize / font.Size, Math.Min(sx, sy));
            var pos = rect.Size * 0.5f - size * scale * 0.5f;
            PushMatrix(Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(pos));
            Text(font, textSpan, color);
            PopMatrix();
        }

        /// <summary>
        /// Draws the text scaled to fit into the provided rectangle.
        /// </summary>
        public void TextFitted(SpriteFont font, string text, in Rect rect, Color color)
        {
            var textSpan = text.AsSpan();
            var size = font.SizeOf(textSpan);
            var sx = rect.Width / size.X;
            var sy = rect.Height / font.Size;
            var scale = Math.Min(sx, sy);
            var pos = rect.Size * 0.5f - size * scale * 0.5f;
            PushMatrix(Matrix3x2.CreateScale(scale) * Matrix3x2.CreateTranslation(pos));
            Text(font, textSpan, color);
            PopMatrix();
        }

        #endregion

        #region Copy Arrays

        /// <summary>
        /// Copies the contents of a Vertex and Index array to this Batcher
        /// </summary>
        public void CopyArray(ReadOnlySpan<Vertex> vertexBuffer, ReadOnlySpan<int> indexBuffer)
        {
            // copy vertices over
            ExpandVertices(vertexCount + vertexBuffer.Length);
            vertexBuffer.CopyTo(vertices.AsSpan().Slice(vertexCount));

            // copy indices over
            while (indexCount + indexBuffer.Length >= indices.Length)
                Array.Resize(ref indices, indices.Length * 2);
            for (int i = 0, n = indexCount; i < indexBuffer.Length; i++, n++)
                indices[n] = vertexCount + indexBuffer[i];

            // increment
            vertexCount += vertexBuffer.Length;
            indexCount += indexBuffer.Length;
            currentBatch.Elements += (uint)(vertexBuffer.Length / 3);
            dirty = true;
        }

        #endregion

        #region Misc.

        public void CheckeredPattern(in Rect bounds, float cellWidth, float cellHeight, Color a, Color b)
        {
            var odd = false;

            for (float y = bounds.Top; y < bounds.Bottom; y += cellHeight)
            {
                var cells = 0;
                for (float x = bounds.Left; x < bounds.Right; x += cellWidth)
                {
                    var color = (odd ? a : b);
                    if (color.A > 0)
                        Rect(x, y, Math.Min(bounds.Right - x, cellWidth), Math.Min(bounds.Bottom - y, cellHeight), color);

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
            while (indexCount + 3 >= indices.Length)
                Array.Resize(ref indices, indices.Length * 2);

            indices[indexCount + 0] = vertexCount + 0;
            indices[indexCount + 1] = vertexCount + 1;
            indices[indexCount + 2] = vertexCount + 2;

            indexCount += 3;
            currentBatch.Elements++;
            dirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushQuad()
        {
            int index = indexCount;
            int vert = vertexCount;

            while (index + 6 >= indices.Length)
                Array.Resize(ref indices, indices.Length * 2);

            indices[index + 0] = vert + 0;
            indices[index + 1] = vert + 1;
            indices[index + 2] = vert + 2;
            indices[index + 3] = vert + 0;
            indices[index + 4] = vert + 2;
            indices[index + 5] = vert + 3;

            indexCount += 6;
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
