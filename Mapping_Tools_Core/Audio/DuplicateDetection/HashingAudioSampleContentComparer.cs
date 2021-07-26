using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mapping_Tools_Core.Audio.Samples;

namespace Mapping_Tools_Core.Audio.DuplicateDetection {
    /// <summary>
    /// Implementation of <see cref="IAudioSampleContentComparer{T}"/> using hashes.
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    public class HashingAudioSampleContentComparer<T> : IAudioSampleContentComparer<T> where T : IAudioSampleGenerator, IHashableSampleGenerator {
        /// <summary>
        /// Dictionary mapping the audio samples to their audio hash.
        /// </summary>
        protected readonly Dictionary<T, int> AudioHashes = new();

        /// <summary>
        /// Initializes a new instance of <see cref="HashingAudioSampleContentComparer{T}"/> with no samples registered.
        /// </summary>
        public HashingAudioSampleContentComparer() { }

        /// <summary>
        /// Initializes a new instance of <see cref="HashingAudioSampleContentComparer{T}"/> with specified samples registered.
        /// </summary>
        public HashingAudioSampleContentComparer(IEnumerable<T> samples) {
            foreach (var sample in samples) {
                AudioHashes.Add(sample, Helpers.ComputeAudioHash(sample));
            }
        }

        /// <inheritdoc/>
        public bool SampleRegistered(T sample) {
            return AudioHashes.ContainsKey(sample);
        }

        /// <inheritdoc/>
        public void RegisterSample(T sample) {
            if (SampleRegistered(sample)) {
                return;
            }

            AudioHashes.Add(sample, Helpers.ComputeAudioHash(sample));
        }

        /// <summary>
        /// Checks if the two samples make the same sound.
        /// </summary>
        /// <exception cref="SampleNotRegisteredException">If one of the samples is not registered.</exception>
        public bool Equals(T x, T y) {
            if (x is null && y is null) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            if (!SampleRegistered(x)) {
                throw new SampleNotRegisteredException(x);
            }

            if (!SampleRegistered(y)) {
                throw new SampleNotRegisteredException(y);
            }

            return AudioHashes[x] == AudioHashes[y];
        }

        /// <summary>
        /// Gets the audio hash of the sample.
        /// </summary>
        /// <param name="obj">The sample to get the hash of.</param>
        /// <returns>The audio hash.</returns>
        /// <exception cref="SampleNotRegisteredException">If the sample is not registered.</exception>
        public int GetHashCode([NotNull] T obj) {
            if (!SampleRegistered(obj)) {
                throw new SampleNotRegisteredException(obj);
            }

            return AudioHashes[obj];
        }
    }
}