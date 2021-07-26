namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Object that generates a sample in any way, shape, or form.
    /// </summary>
    public interface ISampleGenerator {
        /// <summary>
        /// Gets a string that describes this sample.
        /// Should be usable as a filename.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}