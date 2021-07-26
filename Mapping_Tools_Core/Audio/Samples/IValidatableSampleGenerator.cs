namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Sample generator which is able to be validated, 
    /// so you know whether it will successfully generate the sample.
    /// </summary>
    public interface IValidatableSampleGenerator {
        /// <summary>
        /// Returns whether this sample generator is valid,
        /// so it could successfully generate a sample.
        /// </summary>
        /// <returns></returns>
        bool IsValid();
    }
}
