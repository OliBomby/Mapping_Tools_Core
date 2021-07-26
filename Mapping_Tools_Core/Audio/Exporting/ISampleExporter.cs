using Mapping_Tools_Core.Audio.Samples;
using System;

namespace Mapping_Tools_Core.Audio.Exporting {
    /// <summary>
    /// Exports one or more samples to a destination.
    /// </summary>
    public interface ISampleExporter : IDisposable {
        /// <summary>
        /// The number of samples fed to the exporter.
        /// </summary>
        int NumSamples { get; }

        /// <summary>
        /// Adds a sample to the exporter to be exported.
        /// </summary>
        /// <param name="sample">The sample to add to the exporter.</param>
        /// <returns>Whether the sample got accepted into the exporter.</returns>
        bool AddSample(ISampleGenerator sample);

        /// <summary>
        /// Returns the last previously accepted sample and removes it from the exporter.
        /// Returns null if empty.
        /// </summary>
        /// <returns>The last previously accepted sample.</returns>
        ISampleGenerator PopSample();

        /// <summary>
        /// Exports the sample(s) and resets the exporter.
        /// </summary>
        /// <returns>Whether the export was successfull.</returns>
        bool Flush();

        /// <summary>
        /// Resets the exporter, returning it to a state where no samples are added.
        /// </summary>
        void Reset();
    }
}