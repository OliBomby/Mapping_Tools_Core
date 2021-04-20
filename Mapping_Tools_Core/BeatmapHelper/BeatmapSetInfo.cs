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

        [CanBeNull]
        public List<IBeatmapSetFileInfo> Files { get; set; }

        [CanBeNull]
        public List<IBeatmap> Beatmaps { get; set; }

        [CanBeNull]
        public List<IStoryboard> StoryboardFile { get; set; }

        [CanBeNull]
        public IEnumerable<IBeatmapSetFileInfo> SoundFiles => Files?.Where(f => soundExtensions.Contains(Path.GetExtension(f.Filename), StringComparer.OrdinalIgnoreCase));
    }
}