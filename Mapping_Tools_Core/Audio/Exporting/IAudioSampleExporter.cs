using NAudio.Wave;

namespace Mapping_Tools_Core.Audio.Exporting {
    public interface IAudioSampleExporter : ISampleExporter {
        /// <summary>
        /// Adds a sample audio stream to the exporter to be exported.
        /// </summary>
        /// <param name="sample">The audio to export.</param>
        void AddAudio(ISampleProvider sample);

        /// <summary>
        /// Returns the wave encoding that fits with the export of the exporter.
        /// Returns null if not applicable.
        /// </summary>
        /// <returns></returns>
        WaveFormatEncoding? GetDesiredWaveEncoding();
    }
}