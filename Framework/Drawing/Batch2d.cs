using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Foster.Framework
{
    public class Batch2d : GraphicsResource
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertex
        {
            [VertexAttribute(0, VertexType.Float, 2)]
            public Vector2 Pos;

            [VertexAttribute(1, VertexType.Float, 2)]
            public Vector2 Tex;

            [VertexAttribute(2, VertexType.UnsignedByte, 4, true)]
            public Color Col;

            [VertexAttribute(3, VertexType.UnsignedByte, 1, true)]
            public byte Mult;

            [VertexAttribute(4, VertexType.UnsignedByte, 1, true)]
            public byte Wash;

            [VertexAttribute(5, VertexType.UnsignedByte, 1, true)]
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

            public override string ToString()
            {
                return $"{{Pos:{Pos}, Tex:{Tex}, Col:{Col}, Mult:{Mult}, Wash:{Wash}, Fill:{Fill}}}";
            }

        }

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
        public readonly Material DefaultMaterial;
        public readonly Mesh<Vertex> Mesh;

        public Matrix3x2 MatrixStack = Matrix3x2.Identity;
        private readonly Stack<Matrix3x2> matrixStack = new Stack<Matrix3x2>();

        private Vertex[] vertices;
        private int[] triangles;
        private readonly List<Batch> batches;
        private Batch currentBatch;

        private int vertexCount;
        private int triangleCount;
        private bool dirty = false;

        public int Triangles => triangleCount / 3;
        public int Vertices => vertexCount;
        public int Batches => batches.Count + (currentBatch.Elements > 0 ? 1 : 0);

        private struct Batch
        {
            public Material? Material;
            public BlendMode BlendMode;
            public Matrix3x2 Matrix;
            public Texture? Texture;
            public RectInt? Scissor;
            public int Offset;
            public int Elements;

            public Batch(Material? material, BlendMode blend, Texture? texture, Matrix3x2 matrix, int offset, int elements)
            {
                Material = material;
                BlendMode = blend;
                Texture = texture;
                Matrix = matrix;
                Scissor = null;
                Offset = offset;
                Elements = elements;
            }
        }

        public Batch2d() : this(App.Graphics)
        {

        }

        public Batch2d(Graphics graphics) : base(graphics)
        {
            DefaultShader = graphics.CreateShader(VertexSource, FragmentSource);
            DefaultMaterial = new Material(DefaultShader);
            Mesh = graphics.CreateMesh<Vertex>();

            vertices = new Vertex[64];
            triangles = new int[64];
            batches = new List<Batch>();

            Clear();
        }

        public override void Dispose()
        {
            DefaultShader.Dispose();
            Mesh.Dispose();
            base.Dispose();
        }

        public void Clear()
        {
            vertexCount = 0;
            triangleCount = 0;
            currentBatch = new Batch(null, BlendMode.Normal, null, Matrix3x2.Identity, 0, 0);
            batches.Clear();
            matrixStack.Clear();
            MatrixStack = Matrix3x2.Identity;
        }

        #region Rendering

        public void Render()
        {
            Render(Matrix3x2.Identity);
        }

        public void Render(Matrix3x2 matrix)
        {
            Debug.Assert(matrixStack.Count <= 0, "Batch.MatrixStack Pushes more than it Pops");
            Debug.Assert(!Disposed, "Batch was Disposed and cannot Render");
            Debug.Assert(!Mesh.Disposed, "Batch Mesh was Disposed and cannont Render");

            if (batches.Count > 0 || currentBatch.Elements > 0)
            {
                if (dirty)
                {
                    Mesh.SetTriangles(new Memory<int>(triangles, 0, triangleCount));
                    Mesh.SetVertices(new Memory<Vertex>(vertices, 0, vertexCount));

                    dirty = false;
                }

                Graphics.DepthTest(false);
                Graphics.CullMode(Cull.None);

                var ortho =
                    Matrix3x2.CreateScale((1.0f / Graphics.Viewport.Width) * 2, -(1.0f / Graphics.Viewport.Height) * 2) *
                    Matrix3x2.CreateTranslation(-1.0f, 1.0f);

                var view = matrix * ortho;

                // render batches
                for (int i = 0; i < batches.Count; i++)
                    RenderBatch(batches[i], ref view);

                // remaining elements
                if (currentBatch.Elements > 0)
                    RenderBatch(currentBatch, ref view);
            }
        }

        private void RenderBatch(Batch batch, ref Matrix3x2 matrix)
        {
            if (batch.Scissor != null)
                Graphics.Scissor(batch.Scissor.Value);
            else
                Graphics.DisableScissor();

            // set BlendMode
            Graphics.BlendMode(batch.BlendMode);

            // Render the Mesh
            // Note we apply the texture and matrix based on the current batch
            // If the user set these on the Material themselves, they will be overwritten here

            Mesh.Material = batch.Material ?? DefaultMaterial;
            Mesh.Material.SetTexture("Texture", batch.Texture);
            Mesh.Material.SetMatrix("Matrix", batch.Matrix * matrix);
            Mesh.Draw(batch.Offset, batch.Elements);
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
                batches.Add(currentBatch);

                currentBatch.Material = material;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
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
                batches.Add(currentBatch);

                currentBatch.BlendMode = blendmode;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
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
                batches.Add(currentBatch);

                currentBatch.Matrix = matrix;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
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
                batches.Add(currentBatch);

                currentBatch.Scissor = scissor;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
            }
        }

        public void SetState(Material? material, BlendMode blendmode, Matrix3x2 matrix, RectInt? scissor)
        {
            SetMaterial(material);
            SetBlendMode(blendmode);
            SetMatrix(matrix);
            SetScissor(scissor);
        }

        public void SetTexture(Texture? texture)
        {
            if (currentBatch.Texture == null || currentBatch.Elements == 0)
            {
                currentBatch.Texture = texture;
            }
            else if (currentBatch.Texture != texture)
            {
                batches.Add(currentBatch);

                currentBatch.Texture = texture;
                currentBatch.Offset += currentBatch.Elements;
                currentBatch.Elements = 0;
            }
        }

        public Matrix3x2 PushMatrix(Vector2 position, Vector2 scale, Vector2 origin, float rotation, bool relative = true)
        {
            return PushMatrix(Matrix3x2.CreateTransform(position, origin, scale, rotation), relative);
        }

        public Matrix3x2 PushMatrix(Transform transform, bool relative = true)
        {
            return PushMatrix(transform.Matrix, relative);
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

        #region Quad

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

            if (currentBatch.Texture?.FlipVertically ?? false)
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

            if (currentBatch.Texture?.FlipVertically ?? false)
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
                new Vector2(0, 0), new Vector2(texture.Width, 0), new Vector2(texture.Width, texture.Height),  new Vector2(0, texture.Height),
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

            MatrixStack = Matrix3x2.CreateTransform(position, origin, scale, rotation) * MatrixStack;

            SetTexture(texture);
            Quad(
                Vector2.Zero, new Vector2(texture.Width, 0), new Vector2(texture.Width, texture.Height), new Vector2(0, texture.Height),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                color, washed);

            MatrixStack = was;
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Color color, bool washed = false)
        {
            var tx0 = clip.X / (float)texture.Width;
            var ty0 = clip.Y / (float)texture.Height;
            var tx1 = clip.Right / (float)texture.Width;
            var ty1 = clip.Bottom / (float)texture.Height;

            SetTexture(texture);
            Quad(
                position, position + new Vector2(clip.Width, 0), position + new Vector2(clip.Width, clip.Height), position + new Vector2(0, clip.Height),
                new Vector2(tx0, ty0), new Vector2(tx1, ty0), new Vector2(tx1, ty1), new Vector2(tx0, ty1),
                color, washed);
        }

        public void Image(Texture texture, Rect clip, Vector2 position, Vector2 scale, Vector2 origin, float rotation, Color color, bool washed = false)
        {
            var was = MatrixStack;

            MatrixStack = Matrix3x2.CreateTransform(position, origin, scale, rotation) * MatrixStack;

            var tx0 = clip.X / (float)texture.Width;
            var ty0 = clip.Y / (float)texture.Height;
            var tx1 = clip.Right / (float)texture.Width;
            var ty1 = clip.Bottom / (float)texture.Height;

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

            MatrixStack = Matrix3x2.CreateTransform(position, origin, scale, rotation) * MatrixStack;

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
        private void Transform(ref Vector2 to, ref Vector2 position, ref Matrix3x2 matrix)
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
