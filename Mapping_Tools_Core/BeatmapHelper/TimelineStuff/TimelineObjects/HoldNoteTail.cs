﻿using JetBrains.Annotations;
using Mapping_Tools_Core.BeatmapHelper.HitObjects;

namespace Mapping_Tools_Core.BeatmapHelper.TimelineStuff.TimelineObjects {
    public class HoldNoteTail : TimelineObject {
        public override bool HasHitsound => false;
        public override bool CanCustoms => false;

        public HoldNoteTail(double time, [NotNull] HitSampleInfo hitsounds) : base(time, hitsounds) { }

        public override void HitsoundsToOrigin() { }
    }
}