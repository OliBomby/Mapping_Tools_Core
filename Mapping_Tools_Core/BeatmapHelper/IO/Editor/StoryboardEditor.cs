﻿using Mapping_Tools_Core.BeatmapHelper.IO.Decoding;
using Mapping_Tools_Core.BeatmapHelper.IO.Encoding;

namespace Mapping_Tools_Core.BeatmapHelper.IO.Editor {
    /// <summary>
    /// Editor specifically for storyboards
    /// </summary>
    public class StoryboardEditor : PathEditor<IStoryboard> {
        public StoryboardEditor() : base(new OsuStoryboardEncoder(), new OsuStoryboardDecoder()) {}

        public StoryboardEditor(string path) : base(new OsuStoryboardEncoder(), new OsuStoryboardDecoder(), path) {}
    }
}
