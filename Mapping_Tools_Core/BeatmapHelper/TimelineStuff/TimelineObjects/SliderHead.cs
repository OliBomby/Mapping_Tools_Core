using Mapping_Tools_Core.BeatmapHelper.HitObjects;

namespace Mapping_Tools_Core.BeatmapHelper.TimelineStuff.TimelineObjects {
    public class SliderHead : SliderNode {
        public SliderHead(double time, [NotNull] HitSampleInfo hitsounds, int nodeIndex) : base(time, hitsounds, nodeIndex) { }
    }
}