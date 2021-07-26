using System.IO;

namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Sample generator which derives from a file.
    /// </summary>
    public interface IFromFileSampleGenerator : ISampleGenerator {
        /// <summary>
        /// The filename of the source file with extension.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Gets the data stream from the source file.
        /// </summary>
        /// <returns>The data stream from the source file.</returns>
        Stream GetFileStream();

        /// <summary>
        /// Whether the source file directly contains all the sample information of this sample generator.
        /// </summary>
        bool DirectSource { get; }
    }
}