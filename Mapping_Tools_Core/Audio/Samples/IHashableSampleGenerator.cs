using System;

namespace Mapping_Tools_Core.Audio.Samples {
    /// <summary>
    /// Immutable hashable sample generator.
    /// </summary>
    public interface IHashableSampleGenerator : ISampleGenerator, IEquatable<IHashableSampleGenerator> {
    }
}
