﻿namespace Mapping_Tools_Core.BeatmapHelper.IO.Decoding {
    public interface IDecoder<out T> {
        /// <summary>
        /// Parses the code to create a new object.
        /// </summary>
        /// <param name="code">The string to parse</param>
        /// <returns>The new parsed object</returns>
        T Decode(string code);
    }
}