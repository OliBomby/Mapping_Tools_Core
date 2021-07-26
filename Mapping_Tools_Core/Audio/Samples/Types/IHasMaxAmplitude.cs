
namespace Mapping_Tools_Core.Audio.Samples.Types {
    /// <summary>
    /// Exposes the maximum amplitude value in the wave audio.
    /// </summary>
    public interface IHasMaxAmplitude {
        /// <summary>
        /// The maximum absolute amplitude in the wave audio.
        /// </summary>
        double MaxAmplitude { get; }
    }
}
