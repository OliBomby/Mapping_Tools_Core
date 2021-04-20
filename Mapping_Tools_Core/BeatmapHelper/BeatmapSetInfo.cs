using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Mapping_Tools_Core.BeatmapHelper {
    /// <summary>
    /// Contains all the contents of a beatmap set.
    /// </summary>
    public class BeatmapSetInfo {
        private static readonly string[] soundExtensions = { ".wav", ".mp3", ".aiff", ".ogg" };

        /// <summary>
        /// All the files in this beatmap set.
        /// </summary>
        [CanBeNull]
        public List<IBeatmapSetFileInfo> Files { get; set; }

        /// <summary>
        /// Dictionary of pairs (relative path, beatmap) that has all the beatmaps of this beatmap set.
        /// </summary>
        [CanBeNull]
        public Dictionary<string, IBeatmap> Beatmaps { get; set; }

        /// <summary>
        /// Dictionary of pairs (relative path, storyboard) that has all the storyboards of this beatmap set.
        /// </summary>
        [CanBeNull]
        public Dictionary<string, IStoryboard> Storyboards { get; set; }

        /// <summary>
        /// All the sound files in this beatmap set.
        /// </summary>
        [CanBeNull]
        public IEnumerable<IBeatmapSetFileInfo> SoundFiles => Files?.Where(f =>
            soundExtensions.Contains(Path.GetExtension(f.Filename), StringComparer.OrdinalIgnoreCase));
    }
}