namespace Mapping_Tools_Core.Audio.Samples.Types {
    /// <summary>
    /// Represents a sample generator which is just a reference to an audio file.
    /// </summary>
    public interface IAudioFile : IAudioSampleGenerator, IFromFileSampleGenerator, IHashableSampleGenerator {
    }
}
