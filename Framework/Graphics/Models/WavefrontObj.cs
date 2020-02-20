using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Foster.Framework
{
    /// <summary>
    /// An Obj parser
    /// </summary>
    public class WavefrontObj
    {

        public struct Vertex
        {
            public int PositionIndex;
            public int NormalIndex;
            public int TexcoordIndex;
        }

        public struct Face
        {
            public int VertexIndex;
            public int VertexCount;
        }

        public class Obj
        {
            public readonly string Name;
            public readonly List<Vertex> Vertices = new List<Vertex>();
            public readonly List<Face> Faces = new List<Face>();

            public Obj(string name)
            {
                Name = name;
            }
        }

        public class Mtl
        {

        }

        public readonly List<Vector3> Positions = new List<Vector3>();
        public readonly List<Vector3> Normals = new List<Vector3>();
        public readonly List<Vector2> Texcoords = new List<Vector2>();

        public Dictionary<string, Obj> Objects = new Dictionary<string, Obj>();
        public Dictionary<string, Mtl> Materials = new Dictionary<string, Mtl>();

        public WavefrontObj(Stream stream)
        {
            Obj? obj = null;
            Span<char> buffer = stackalloc char[1024];

            using var reader = new StreamReader(stream);

            var remaining = buffer.Slice(0, reader.Read(buffer));
            var eof = false;

            while (remaining.Length > 0)
            {
                var index = 0;
                while ((index = remaining.IndexOf('\n')) >= 0 || (remaining.Length > 0 && eof))
                {
                    if (index <= 0)
                        index = remaining.Length;

                    var line = remaining.Slice(0, index);
                    remaining = remaining.Slice(Math.Min(remaining.Length, index + 1));

                    while (line.Length > 0 && char.IsWhiteSpace(line[0]))
                        line = line.Slice(1);
                    while (line.Length > 0 && char.IsWhiteSpace(line[line.Length - 1]))
                        line = line.Slice(0, line.Length - 1);

                    if (line.Length <= 0)
                        continue;

                    // obj
                    if (line[0] == 'o' && line.Length > 3)
                    {
                        obj = new Obj(line.Slice(2).ToString());
                        Objects.Add(obj.Name, obj);
                    }
                    else if (line[0] == 'v' && line.Length > 3)
                    {
                        // normal
                        if (line[1] == 'n')
                        {
                            if (Calc.ParseVector3(line.Slice(3), ' ', out var normal))
                                Normals.Add(normal);
                        }
                        // tex-coord
                        else if (line[1] == 't')
                        {
                            if (Calc.ParseVector2(line.Slice(3), ' ', out var texcoord))
                                Texcoords.Add(texcoord);
                        }
                        // position
                        else
                        {
                            if (Calc.ParseVector3(line.Slice(2), ' ', out var position))
                                Positions.Add(position);
                        }
                    }
                    else if (line[0] == 'f' && line.Length > 3)
                    {
                        if (obj == null)
                        {
                            obj = new Obj("Unnamed");
                            Objects.Add(obj.Name, obj);
                        }

                        var face = new Face { VertexIndex = obj.Vertices.Count };
                        var data = line.Slice(2);
                        var next = data;

                        while ((next = NextSplit(ref data, ' ')).Length > 0)
                        {
                            var vertex = new Vertex();
                            var i = 0;
                            while (next.Length > 0)
                            {
                                var sub = NextSplit(ref next, '/');

                                if (sub.Length > 0)
                                {
                                    if (i == 0)
                                        vertex.PositionIndex = int.Parse(sub) - 1;
                                    else if (i == 1)
                                        vertex.NormalIndex = int.Parse(sub) - 1;
                                    else if (i == 2)
                                        vertex.TexcoordIndex = int.Parse(sub) - 1;
                                }

                                i++;
                            }

                            obj.Vertices.Add(vertex);
                            face.VertexCount++;
                        }

                        obj.Faces.Add(face);
                    }
                }

                // shift memory back & read more
                remaining.CopyTo(buffer);
                var offset = buffer.Slice(remaining.Length);
                var read = reader.Read(offset);

                // how much is left to read
                remaining = buffer.Slice(0, remaining.Length + read);

                // reached the end
                if (read < offset.Length)
                    eof = true;
            }
        }

        private Span<char> NextSplit(ref Span<char> span, char delim)
        {
            var result = new Span<char>();

            if (span.Length > 0)
            {
                var index = span.IndexOf(delim);
                if (index >= 0)
                {
                    result = span.Slice(0, index);
                    span = span.Slice(index + 1);
                }
                else
                {
                    result = span;
                    span = new Span<char>();
                }
            }

            return result;
        }

    }
}
