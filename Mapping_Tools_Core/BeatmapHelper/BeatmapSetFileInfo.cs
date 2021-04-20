using System.IO;

namespace Mapping_Tools_Core.BeatmapHelper {
    public class BeatmapSetFileInfo : IBeatmapSetFileInfo {
        private readonly string rootPath;

        public string Filename { get; }
        public long Size => new FileInfo(GetFullPath()).Length;

        public BeatmapSetFileInfo(string rootPath, string filename) {
            this.rootPath = rootPath;
            Filename = filename;
        }

        private string GetFullPath() {
            return Path.Join(rootPath, Filename);
        }

        public StreamReader GetData() {
            return new StreamReader(File.OpenRead(GetFullPath()));
        }
    }
}