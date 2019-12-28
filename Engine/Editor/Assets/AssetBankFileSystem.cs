using Foster.Engine;
using Foster.Framework;
using Foster.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Foster.Editor
{
    /// <summary>
    /// An Asset Bank that works from the OS File System
    /// </summary>
    public class ProjectAssetBank : AssetBank, IDisposable
    {

        public readonly string AssetsPath;

        private readonly Dictionary<string, Guid> pathToGuid = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, string> guidToPath = new Dictionary<Guid, string>();
        private readonly List<(string, WatcherChangeTypes)> markedFiles = new List<(string, WatcherChangeTypes)>();
        private readonly FileSystemWatcher watcher;

        public bool IsWaitingForSync => markedFiles.Count > 0;

        public readonly Dictionary<Type, AssetLoader> Loaders = new Dictionary<Type, AssetLoader>();
        public readonly Dictionary<Type, AssetProcessor> Processors = new Dictionary<Type, AssetProcessor>();
        public readonly Dictionary<string, Type> AssociatedFileTypes = new Dictionary<string, Type>();

        public ProjectAssetBank(string assetsPath)
        {
            AssetsPath = assetsPath;

            if (!Directory.Exists(AssetsPath))
                Directory.CreateDirectory(AssetsPath);

            watcher = new FileSystemWatcher(AssetsPath, "*.*")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
            };

            watcher.Created += FileChanged;
            watcher.Changed += FileChanged;
            watcher.Deleted += FileChanged;
            watcher.Renamed += FileRenamed;
        }

        public bool HasFile(string fullpath)
        {
            return pathToGuid.ContainsKey(NormalizePath(fullpath));
        }

        public bool HasRelativeFile(string path)
        {
            return pathToGuid.ContainsKey(NormalizePath(path, false));
        }

        public string? PathOf(Guid guid)
        {
            if (guidToPath.TryGetValue(guid, out var path))
                return path;
            return null;
        }

        public void StartWatching()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            watcher.EnableRaisingEvents = false;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            var ext = Path.GetExtension(e.FullPath);

            if (ext != null && !ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                MarkFile(e.FullPath, e.ChangeType);
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            var ext = Path.GetExtension(e.FullPath);

            if (ext != null && !ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                // TODO:
                // actually mark the file as renamed, so it can hande this more gracefully?
                // ex. it could automatically hook up the guid again, maybe?

                MarkFile(e.OldFullPath, WatcherChangeTypes.Deleted);
                MarkFile(e.FullPath, WatcherChangeTypes.Created);
            }
        }

        public void MarkFile(string fullpath, WatcherChangeTypes mark)
        {
            if (mark == WatcherChangeTypes.Created || pathToGuid.ContainsKey(NormalizePath(fullpath)))
                markedFiles.Add((fullpath, mark));
        }

        public void SyncAllFiles()
        {
            // we don't care about any marked files because we're doing a full sweep
            markedFiles.Clear();

            foreach (var file in Directory.EnumerateFiles(AssetsPath, "*.*", SearchOption.AllDirectories))
                AddFile(file);
        }
        
        public void SyncMarkedFiles()
        {
            int count = markedFiles.Count;
            for (int i = 0; i < count; i ++)
            {
                var path = markedFiles[i].Item1;
                var mark = markedFiles[i].Item2;

                if (mark == WatcherChangeTypes.Created)
                    AddFile(path);
                else if (mark == WatcherChangeTypes.Deleted)
                    RemoveFile(path);
                else if (mark == WatcherChangeTypes.Changed)
                    UpdateFile(path);
            }

            markedFiles.RemoveRange(0, count);
        }

        private bool AddFile(string fullpath)
        {
            var ext = ((ReadOnlySpan<char>)Path.GetExtension(fullpath));
            if (ext.Length > 0 && ext[0] == '.')
                ext = ext.Slice(1);

            // skip meta files
            if (ext.Equals("meta", StringComparison.OrdinalIgnoreCase))
                return false;

            // find the type based on the extension
            Type? type = null;
            foreach (var loader in Loaders.Values)
            {
                foreach (var extension in loader.FileExtensions)
                    if (ext.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        type = loader.AssetType;
                        break;
                    }
                
                if (type != null)
                    break;
            }

            // we can't figure out what type this should be ... so ignore it
            if (type == null)
                return false;

            // get the name, and unload if it already exists
            var name = GetName(fullpath);
            if (GetEntry(type, name) != null)
            {
                Unload(type, name);
                return false;
            }

            var relative = NormalizePath(fullpath);
            var metaPath = fullpath + ".meta";
            var guid = new Guid();
            var hasMeta = false;
            var hasChanged = false;

            // TODO:
            // store Hash or file-dates to check if files need to be processed again
            // ...

            // check for meta file
            if (File.Exists(metaPath))
            {
                using var stream = File.OpenRead(metaPath);
                using var reader = new JsonReader(stream);

                while (reader.Read())
                {
                    if (reader.Token == JsonToken.ObjectKey && reader.Value is string guidKey && guidKey == "guid")
                    {
                        reader.Read();
                        if (reader.Value is string guidValue)
                        {
                            guid = new Guid(guidValue);
                            hasMeta = true;
                        }
                    }
                }
            }
            else
                hasChanged = true;

            // create a default meta file if none exists
            if (!hasMeta)
            {
                guid = Guid.NewGuid();

                using var writer = new JsonWriter(File.OpenWrite(metaPath), false);
                writer.JsonValue(new JsonObject
                {
                    ["guid"] = guid.ToString()
                });
            }

            // add entry
            Add(type, guid, name);
            pathToGuid[relative] = guid;
            guidToPath[guid] = relative;

            // run processors on it ... if we need to
            if (hasChanged)
            {
                if (Processors.TryGetValue(type, out var processor))
                    processor.Changed(this, guid, name, fullpath);
            }

            return true;
        }

        private void UpdateFile(string fullpath)
        {
            // TODO:
            // Check if Hash or File-Dates have changed before unloading/reloading

            var relative = NormalizePath(fullpath);
            if (pathToGuid.TryGetValue(relative, out var guid))
            {
                var entry = GetEntry(guid);
                if (entry != null)
                {
                    // unload the asset
                    Unload(guid);

                    // check if there's a processor for this asset type and run it
                    if (Processors.TryGetValue(entry.Type, out var processor))
                        processor.Changed(this, guid, entry.Name, fullpath);
                }
            }
        }

        private void RemoveFile(string fullpath)
        {
            var relative = NormalizePath(fullpath);
            if (pathToGuid.TryGetValue(relative, out var guid))
            {
                guidToPath.Remove(guid);
                pathToGuid.Remove(relative);
                Remove(guid);
            }
        }

        protected override IAsset? LoadAsset(Guid guid, Type type)
        {
            if (Loaders.TryGetValue(type, out var loader) && guidToPath.TryGetValue(guid, out var path))
            {
                var fullpath = Path.Combine(AssetsPath, path);
                if (File.Exists(fullpath))
                {
                    // asset stream
                    using var stream = File.OpenRead(fullpath);

                    // meta data
                    JsonObject? meta = null;
                    var metapath = fullpath + ".meta";
                    if (File.Exists(metapath))
                    {
                        using var reader = new JsonReader(File.OpenRead(metapath));
                        reader.TryReadObject(out meta);
                    }

                    return loader.Load(this, stream, meta);
                }
                
            }

            return null;
        }

        private string GetName(string path)
        {
            var p = NormalizePath(path);
            var ext = p.LastIndexOf('.');

            if (ext >= 0)
                return p.Substring(0, ext);

            return p;
        }

        private string NormalizePath(string path, bool isFullPath = true)
        {
            if (isFullPath)
                path = Path.GetRelativePath(AssetsPath, path);

            ReadOnlySpan<char> ptr = path;
            Span<char> norm = stackalloc char[ptr.Length];

            int length = ptr.Length;
            for (int i = 0, t = 0, l = length; t < l; i++, t++)
            {
                if (t > 0 && norm[t - 1] == '/' && (ptr[t] == '/' || ptr[t] == '\\'))
                {
                    i--;
                    length--;
                }
                else if (ptr[t] == '\\')
                    norm[i] = '/';
                else
                    norm[i] = ptr[t];
            }

            var normalized = norm.Slice(0, length).Trim('/').ToString();
            return normalized;
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
