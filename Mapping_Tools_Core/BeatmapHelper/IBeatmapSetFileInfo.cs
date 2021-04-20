using System.IO;

namespace Mapping_Tools_Core.BeatmapHelper {
    public interface IBeatmapSetFileInfo {
        /// <summary>
        /// The filename of with file extension.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets a stream with the contents of the file.
        /// </summary>
        /// <returns>The stream with the contents of the file.</returns>
        public StreamReader GetData();
    }
}