﻿using JetBrains.Annotations;
using Mapping_Tools_Core.BeatmapHelper.HitObjects;

namespace Mapping_Tools_Core.BeatmapHelper.TimelineStuff.TimelineObjects {
    public class SliderTail : SliderNode {
        public SliderTail(double time, [NotNull] HitSampleInfo hitsounds, int nodeIndex) : base(time, hitsounds, nodeIndex) { }
    }
}