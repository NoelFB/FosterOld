using Foster.Framework;
using Foster.GLFW;
using Foster.GuiSystem;
using Foster.OpenGL;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Foster.Engine
{
    class Program
    {

        static void Main(string[] args)
        {
            App.Modules.Register<GLFW_System>();
            App.Modules.Register<GLFW_Input>();
            App.Modules.Register<GL_Graphics>();

            App.Start(Ready);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Vertex
        {
            [VertexAttribute(0, "vPosition", VertexType.Float, 3, false)]
            public Vector3 Position;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Instance
        {
            [VertexAttribute(0, "iOffset", VertexType.Float, 16, false)]
            public Matrix Offset;
        }

        private static void Ready()
        {
            var font = new SpriteFont(Path.Combine(App.System.AppDirectory, "Content", "Roboto-Medium.ttf"), 64, Charsets.ASCII);
            var gui = App.Modules.Register(new Gui(font, "Gui", 1280, 720));

            var scene = gui.CreatePanel("Scene", new Rect(32, 32, 200, 200));
            scene.DockWith(null);
            var game = gui.CreatePanel("Game", scene);

            var assets = gui.CreatePanel("Assets", new Rect(32, 32, 200, 200));
            assets.DockLeftOf(scene);
            var inspector = gui.CreatePanel("Inspector", new Rect(32, 32, 200, 200));
            inspector.DockRightOf(scene);
            var log = gui.CreatePanel("Log", new Rect(32, 32, 200, 200));
            log.DockBottomOf(scene);

            game.OnRefresh = (imgui) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    imgui.PushId(i);

                    if (imgui.Header("WHAT"))
                    {
                        imgui.Title("Something");
                        imgui.Row(3);
                        imgui.Label("Position X");
                        if (imgui.Button("Snap Left"))
                            game.DockLeftOf(scene);
                        if (imgui.Button("Snap Right"))
                            game.DockRightOf(scene);

                        imgui.Row(3);
                        imgui.Label("Position Y");
                        if (imgui.Button("Snap Top"))
                            game.DockTopOf(scene);
                        if (imgui.Button("Snap Bottom"))
                            game.DockBottomOf(scene);

                        imgui.Title("Dangerous");
                        if (imgui.Button("Popout"))
                            game.Popout();
                        if (imgui.Button("Close"))
                            game.Close();

                        imgui.EndHeader();
                    }

                    imgui.PopId();
                }
            };

            static Mesh MakeMesh(string file)
            {
                var obj = new WavefrontObj(File.OpenRead(file));

                var vertices = new List<Vertex>();
                foreach (var v in obj.Positions)
                    vertices.Add(new Vertex { Position = v });

                var indices = new List<int>();
                foreach (var o in obj.Objects.Values)
                {
                    foreach (var face in o.Faces)
                    {
                        for (int i = 0; i < face.VertexCount; i++)
                            indices.Add(o.Vertices[face.VertexIndex + i].PositionIndex);
                    }
                }

                var instances = new List<Instance>();
                for (int i = 0; i < 10; i++)
                    instances.Add(new Instance { Offset = Matrix.CreateTranslation(Rand.Instance.Range(-10, 10), Rand.Instance.Range(-10, 10), Rand.Instance.Range(-10, 10)) });

                var mesh = App.Graphics.CreateMesh();
                
                mesh.SetVertices(vertices.ToArray());
                mesh.SetIndices(indices.ToArray());
                mesh.SetInstances(instances.ToArray());

                return mesh;
            }

            var mesh = MakeMesh("Content/wavefront.obj");
            var mesh2 = MakeMesh("Content/test.obj");

            var shader = App.Graphics.CreateShader(@"
#version 330
uniform mat4 Matrix;
in vec3 vPosition;
in mat4 iOffset;
out vec4 fragCol;
void main(void)
{
    gl_Position = Matrix * iOffset * vec4(vPosition, 1.0);
    fragCol = (1 + vec4(vPosition, 1.0)) / 2;
}",
@"
#version 330
out vec4 outColor;
in vec4 fragCol;
void main(void)
{
    outColor = fragCol;
}");
            var material = new Material(shader);
            var target = App.Graphics.CreateTarget(1000, 1000, 1, true);

            mesh.Material = material;
            mesh2.Material = material;

            scene.OnRefresh = (imgui) =>
            {
                var width = (int)imgui.Frame.Bounds.Width;
                var height = (int)imgui.Frame.Bounds.Height;

                App.Graphics.Target(target);
                App.Graphics.DepthTest(true);
                App.Graphics.DepthFunction(DepthFunctions.Less);
                App.Graphics.CullMode(Cull.None);
                App.Graphics.Clear(ClearFlags.All, Color.Yellow, 1f, 0);

                var view = Matrix.CreateLookAt(new Vector3(MathF.Cos((float)Time.Duration.TotalSeconds), 0, MathF.Sin((float)Time.Duration.TotalSeconds)) * 20f, new Vector3(0, 0, 0), Vector3.Up);
                var projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 4f, target.Width / (float)target.Height, 0.25f, 100f);

                material.SetMatrix("Matrix", view * projection);
                mesh.DrawInstances();

                view = Matrix.CreateTranslation(0, -5, 0) * Matrix.CreateLookAt(new Vector3(MathF.Cos((float)-Time.Duration.TotalSeconds), 0, MathF.Sin((float)-Time.Duration.TotalSeconds)) * 20f, new Vector3(0, 0, 0), Vector3.Up);

                material.SetMatrix("Matrix", view * projection);
                mesh2.DrawInstances();

                imgui.Batcher.Image(target, new RectInt((target.Width - width) / 2, (target.Height - height) / 2, width, height), imgui.Frame.Bounds.TopLeft, Vector2.One, Vector2.Zero, 0f, Color.White);
            };
            
        }
    }
}
