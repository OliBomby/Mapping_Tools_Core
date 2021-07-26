using Mapping_Tools_Core.Audio.Exporting;
using Mapping_Tools_Core.Audio.Midi;

namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Represents the arguments for a single MIDI note.
    /// Expected to work with <see cref="IMidiSampleExporter"/>.
    /// </summary>
    public interface IMidiSampleGenerator : ISampleGenerator {
        /// <summary>
        /// The MIDI note.
        /// </summary>
        IMidiNote Note { get; }
    }
}