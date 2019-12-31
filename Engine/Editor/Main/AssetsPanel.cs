using Foster.Framework;
using Foster.GuiSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Editor
{
    public class AssetsPanel : Panel
    {

        public readonly ProjectEditor Editor;

        private struct FileInfo
        {
            public string Path;
            public string Name;
            public bool IsDirectory;
            public int Depth;
        }

        private List<FileInfo> assetDirectory = new List<FileInfo>();

        public AssetsPanel(ProjectEditor editor) : base(App.Modules.Get<Gui>(), "Assets")
        {
            Editor = editor;
            Reload();
        }

        public void Reload()
        {
            // TODO:
            // The Assets Panel is currently updated independently of the Asset Bank, since it needs to include .cs files
            // It also reloads the ENTIRE directory listing every time.
            // This could maybe be refactored to:
            //      1) Merge Assets / Directory / .CS listing into one thing
            //      2) Only Add / Insert files as they're created / deleted / moved
            // That said ... I tested this with like 10,000 files and it took 50ms so, maybe it's OK?
            // - noel

            assetDirectory.Clear();
            Add(Editor.Project.Config.AssetsPath, 0);

            void Add(string path, int depth)
            {
                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    assetDirectory.Add(new FileInfo 
                    { 
                        Name = Path.GetFileName(directory), 
                        Path = directory, 
                        IsDirectory = true,
                        Depth = depth
                    });

                    Add(directory, depth + 1);
                }

                foreach (var file in Directory.EnumerateFiles(path))
                {
                    var ext = (ReadOnlySpan<char>)file;

                    // ignore files with no extension
                    var index = ext.LastIndexOf('.');
                    if (index < 0)
                        continue;

                    // ignore meta files
                    ext = ext.Slice(index + 1);
                    if (ext.Equals("meta", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // ignore files that aren't code and aren't an asset
                    if (!ext.Equals("cs", StringComparison.OrdinalIgnoreCase) && !Editor.Project.Assets.HasFile(file))
                        continue;

                    assetDirectory.Add(new FileInfo
                    {
                        Name = Path.GetFileName(file),
                        Path = file,
                        IsDirectory = false,
                        Depth = depth
                    });
                }
            }
        }

        public override void Refresh(Imgui imgui)
        {
            var depth = 0;
            for (int i = 0; i < assetDirectory.Count; i ++)
            {
                var info = assetDirectory[i];

                if (info.Depth > depth)
                {
                    imgui.PushIndent(15);
                    depth = info.Depth;
                }
                else if (info.Depth < depth)
                {
                    imgui.EndHeader();
                    imgui.PopIndent();
                    depth = info.Depth;
                }

                if (info.IsDirectory)
                {
                    if (!imgui.BeginHeader(info.Name))
                    {
                        i++;
                        while (i < assetDirectory.Count && assetDirectory[i].Depth > depth)
                            i++;
                        i--;
                    }
                }
                else
                {
                    imgui.Label(info.Name, Sizing.FillX(), new StyleState { Padding = Vector2.Zero, ContentColor = Color.White });
                }
            }
        }

    }
}
