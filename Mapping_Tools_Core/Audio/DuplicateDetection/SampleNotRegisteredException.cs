using Mapping_Tools_Core.Audio.Samples;
using System;

namespace Mapping_Tools_Core.Audio.DuplicateDetection {
    /// <summary>
    /// An exception that occurs when doing operations with an <see cref="IAudioSampleContentComparer{T}"/> and unregistered samples.
    /// </summary>
    public class SampleNotRegisteredException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleNotRegisteredException"/> with a specified sample.
        /// </summary>
        /// <param name="sample">The sample which was not registered.</param>
        public SampleNotRegisteredException(ISampleGenerator sample) : base($"This sample is not registered in the content comparer: {sample.GetName()}") {
        }
    }
}
