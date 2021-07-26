namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Sample generator which can be frozen to an immutable copy.
    /// </summary>
    public interface IFreezableSampleGenerator : ISampleGenerator {
        /// <summary>
        /// Generates an immutable copy of the sample generator.
        /// </summary>
        /// <returns>An immutable copy of the sample generator.</returns>
        IHashableSampleGenerator Freeze();
    }
}
