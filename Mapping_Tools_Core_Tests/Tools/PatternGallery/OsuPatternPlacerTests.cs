using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.BeatmapHelper.BeatDivisors;
using Mapping_Tools_Core.BeatmapHelper.Contexts;
using Mapping_Tools_Core.BeatmapHelper.HitObjects;
using Mapping_Tools_Core.BeatmapHelper.HitObjects.Objects;
using Mapping_Tools_Core.BeatmapHelper.IO.Decoding;
using Mapping_Tools_Core.BeatmapHelper.IO.Decoding.HitObjects;
using Mapping_Tools_Core.BeatmapHelper.IO.Editor;
using Mapping_Tools_Core.BeatmapHelper.TimingStuff;
using Mapping_Tools_Core.MathUtil;
using Mapping_Tools_Core.Tools.PatternGallery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mapping_Tools_Core_Tests.Tools.PatternGallery {
    [TestClass]
    public class OsuPatternPlacerTests {
        private IBeatmap patternBeatmap;

        [TestInitialize]
        public void Init() {
            var maker = new OsuPatternMaker();

            var decoder = new HitObjectDecoder();
            var hitObjects = new List<HitObject> {
                decoder.Decode("245,44,0,6,0,B|249:111|215:269|370:-5|-72:203|658:203|216:-5|371:269|337:111|341:44,1,199.999980926514"),
                decoder.Decode("105,142,600,1,0,0:0:0:0:"),
                decoder.Decode("256,192,900,12,2,1500,1:0:0:0:"),
                decoder.Decode("471,55,1800,6,6,P|462:191|385:92,1,299.99997138977,0|0,1:0|1:0,0:0:0:0:")
            };

            var tpDecoder = new TimingPointDecoder();
            var timingPoints = new List<TimingPoint> {
                tpDecoder.Decode("0,600,4,2,100,97,1,0"),
                tpDecoder.Decode("0,-25,4,2,100,97,0,0"),
                tpDecoder.Decode("1800,-100,4,2,100,97,0,0"),
                tpDecoder.Decode("1950,-25,4,1,100,97,0,0")
            };

            maker.FromObjects(hitObjects, timingPoints, out patternBeatmap, "test", globalSv: 1);
        }

        [TestMethod]
        public void ExportPatternTimingTest() {
            var path = Path.Join("Resources", "SAMString - Forget The Promise (DeviousPanda) [Elysium].osu");
            var beatmap = new BeatmapEditor(path).ReadFile();

            var placer = new OsuPatternPlacer {
                BeatDivisors = new IBeatDivisor[] {new RationalBeatDivisor(4)},
                FixBpmSv = true,
                FixColourHax = true,
                FixGlobalSv = true,
                IncludeHitsounds = true,
                IncludeKiai = true,
                PatternOverwriteMode = PatternOverwriteMode.CompleteOverwrite,
                TimingOverwriteMode = TimingOverwriteMode.PatternTimingOnly,
                ScaleToNewCircleSize = false,
                ScaleToNewTiming = false,
                SnapToNewTiming = true
            };

            placer.PlaceOsuPatternAtTime(patternBeatmap, beatmap, 101120);

            var patternHitObjects = beatmap.GetHitObjectsWithRangeInRange(101120, 101120 + 3600);

            foreach (var patternHitObject in patternHitObjects) {
                Console.WriteLine("Start time: " + patternHitObject.StartTime);
                Console.WriteLine("End time: " + patternHitObject.EndTime);
                Console.WriteLine(patternHitObject);
                Console.WriteLine(patternHitObject.GetContext<TimingContext>());
            }

            Assert.AreEqual(patternHitObjects.Count, 4);

            Assert.IsInstanceOfType(patternHitObjects[0], typeof(Slider));
            Assert.IsInstanceOfType(patternHitObjects[1], typeof(HitCircle));
            Assert.IsInstanceOfType(patternHitObjects[2], typeof(Spinner));
            Assert.IsInstanceOfType(patternHitObjects[3], typeof(Slider));

            var testTp = beatmap.BeatmapTiming.GetTimingPointAtTime(106173);

            Assert.AreEqual(0.3, testTp.GetSliderVelocity(), Precision.DOUBLE_EPSILON);
            Assert.AreEqual(6, testTp.SampleIndex);

            Assert.AreEqual(190, beatmap.BeatmapTiming.GetBpmAtTime(106173), Precision.DOUBLE_EPSILON);
            Assert.AreEqual(100, beatmap.BeatmapTiming.GetBpmAtTime(101120), Precision.DOUBLE_EPSILON);
        }

        [TestMethod]
        public void ExportOriginalTimingTest() {
            var path = Path.Join("Resources", "SAMString - Forget The Promise (DeviousPanda) [Elysium].osu");
            var beatmap = new BeatmapEditor(path).ReadFile();

            var placer = new OsuPatternPlacer {
                BeatDivisors = new IBeatDivisor[] { new RationalBeatDivisor(4) },
                FixBpmSv = false,
                FixColourHax = true,
                FixGlobalSv = true,
                IncludeHitsounds = true,
                IncludeKiai = true,
                PatternOverwriteMode = PatternOverwriteMode.CompleteOverwrite,
                TimingOverwriteMode = TimingOverwriteMode.DestinationTimingOnly,
                ScaleToNewCircleSize = false,
                ScaleToNewTiming = true,
                SnapToNewTiming = true
            };

            placer.PlaceOsuPatternAtTime(patternBeatmap, beatmap, 101120);

            var patternHitObjects = beatmap.GetHitObjectsWithRangeInRange(101120, 101120 + 3600/1.9);

            foreach (var patternHitObject in patternHitObjects) {
                Console.WriteLine("Start time: " + patternHitObject.StartTime);
                Console.WriteLine("End time: " + patternHitObject.EndTime);
                Console.WriteLine(patternHitObject);
                Console.WriteLine(patternHitObject.GetContext<TimingContext>());
            }

            Assert.AreEqual(patternHitObjects.Count, 4);

            Assert.IsInstanceOfType(patternHitObjects[0], typeof(Slider));
            Assert.IsInstanceOfType(patternHitObjects[1], typeof(HitCircle));
            Assert.IsInstanceOfType(patternHitObjects[2], typeof(Spinner));
            Assert.IsInstanceOfType(patternHitObjects[3], typeof(Slider));

            var testTp = beatmap.BeatmapTiming.GetTimingPointAtTime(106173);

            Assert.AreEqual(0.3, testTp.GetSliderVelocity(), Precision.DOUBLE_EPSILON);
            Assert.AreEqual(6, testTp.SampleIndex);

            Assert.AreEqual(190, beatmap.BeatmapTiming.GetBpmAtTime(106173), Precision.DOUBLE_EPSILON);
            Assert.AreEqual(190, beatmap.BeatmapTiming.GetBpmAtTime(101120), Precision.DOUBLE_EPSILON);

            var msBeatDelta = 1 / beatmap.BeatmapTiming.GetMpBAtTime(101120);
            Assert.AreEqual(0.5, beatmap.BeatmapTiming.GetBeatLength(patternHitObjects[0].EndTime, patternHitObjects[1].StartTime), msBeatDelta);
            Assert.AreEqual(0.5, beatmap.BeatmapTiming.GetBeatLength(patternHitObjects[1].EndTime, patternHitObjects[2].StartTime), msBeatDelta);
            Assert.AreEqual(0.5, beatmap.BeatmapTiming.GetBeatLength(patternHitObjects[2].EndTime, patternHitObjects[3].StartTime), msBeatDelta);
            Assert.AreEqual(3, beatmap.BeatmapTiming.GetBeatLength(patternHitObjects[3].StartTime, patternHitObjects[3].EndTime), msBeatDelta);
        }
    }
}