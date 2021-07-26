using Mapping_Tools_Core.Audio.Samples;
using System;
using System.Collections.Generic;

namespace Mapping_Tools_Core.Audio.DuplicateDetection {
    /// <summary>
    /// Equality comparer for comparing audio sample generators on actual audio content.
    /// </summary>
    public interface IAudioSampleContentComparer<T> : IEqualityComparer<T> where T : IAudioSampleGenerator, IHashableSampleGenerator {
        /// <summary>
        /// Checks if the given sample is registered in the sample map.
        /// </summary>
        /// <param name="sample">The sample to check.</param>
        /// <returns>Whether given sample is registered in the sample map.</returns>
        /// <exception cref="ArgumentNullException">If the sample is null.</exception>
        bool SampleRegistered(T sample);

        /// <summary>
        /// Registers a new sample to the comparer, so it may be compared. This may be a slow operation.
        /// </summary>
        /// <param name="sample">The sample to register.</param>
        /// <exception cref="ArgumentNullException">If the sample is null.</exception>
        void RegisterSample(T sample);
    }
}