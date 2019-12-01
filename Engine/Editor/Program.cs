using Foster.Framework;
using Foster.GLFW;
using Foster.GuiSystem;
using Foster.OpenGL;
using System;
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
            [VertexAttribute(0, VertexType.Float, 3)]
            public Vector3 Position;
        }

        private static void Ready()
        {
            /*
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
            };*/

            var window = App.System.CreateWindow("Window", 1280, 720);
            

            var obj = new WavefrontObj(File.OpenRead("Content/wavefront.obj"));
            var mesh = App.Graphics.CreateMesh<Vertex>();

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

            mesh.SetVertices(vertices.ToArray());
            mesh.SetTriangles(indices.ToArray());

            var shader = App.Graphics.CreateShader(@"
#version 330
uniform mat4 Matrix;
layout(location = 0) in vec3 vertPos;
out vec4 fragCol;
void main(void)
{
    gl_Position = Matrix * vec4(vertPos, 1.0);
    fragCol = (1 + normalize(gl_Position)) / 2;
}",
@"
#version 330
layout(location = 0) out vec4 outColor;
in vec4 fragCol;
void main(void)
{
    outColor = fragCol;
}");

            mesh.Material = new Material(shader);

            window.OnRender = () =>
            {
                var view = Matrix.CreateLookAt(new Vector3(MathF.Cos((float)Time.Duration.TotalSeconds), 0, MathF.Sin((float)Time.Duration.TotalSeconds)) * 60f, new Vector3(0, 0, 0), Vector3.Up);
                var projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 4f, window.DrawableWidth / (float)window.DrawableHeight, 0.25f, 100f);
                mesh.Material.SetMatrix("Matrix", view * projection);

                App.Graphics.DepthTest(false);
                App.Graphics.Clear(ClearFlags.All, Color.Yellow, 0f, 0) ;

                mesh.Draw(0, indices.Count / 3);
            };
        }
    }
}
