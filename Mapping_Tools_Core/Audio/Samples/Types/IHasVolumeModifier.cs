namespace Mapping_Tools_Core.Audio.Samples.Types {
    /// <summary>
    /// Exposes a generic volume modifier.
    /// </summary>
    public interface IHasVolumeModifier {
        /// <summary>
        /// The volume multiplier. 0 for no sound. 1 for all sound.
        /// </summary>
        double Volume { get; set; }
    }
}
