using System;
using System.Collections.Generic;
using Mapping_Tools_Core.BeatmapHelper.Events;
using static Mapping_Tools_Core.BeatmapHelper.FileFormatHelper;

namespace Mapping_Tools_Core.BeatmapHelper.IO.Decoding {
    public class OsuStoryboardDecoder : IDecoder<Storyboard> {
        public void Decode(Storyboard obj, string code) {
            var lines = code.Split(Environment.NewLine);

            // Load up all the stuff
            IEnumerable<string> backgroundAndVideoEventsLines = GetCategoryLines(lines, "//Background and Video events", new[] { "[", "//" });
            IEnumerable<string> breakPeriodsLines = GetCategoryLines(lines, "//Break Periods", new[] { "[", "//" });
            IEnumerable<string> storyboardLayerBackgroundLines = GetCategoryLines(lines, "//Storyboard Layer 0 (Background)", new[] { "[", "//" });
            IEnumerable<string> storyboardLayerFailLines = GetCategoryLines(lines, "//Storyboard Layer 1 (Fail)", new[] { "[", "//" });
            IEnumerable<string> storyboardLayerPassLines = GetCategoryLines(lines, "//Storyboard Layer 2 (Pass)", new[] { "[", "//" });
            IEnumerable<string> storyboardLayerForegroundLines = GetCategoryLines(lines, "//Storyboard Layer 3 (Foreground)", new[] { "[", "//" });
            IEnumerable<string> storyboardLayerOverlayLines = GetCategoryLines(lines, "//Storyboard Layer 4 (Overlay)", new[] { "[", "//" });
            IEnumerable<string> storyboardSoundSamplesLines = GetCategoryLines(lines, "//Storyboard Sound Samples", new[] { "[", "//" });

            foreach (string line in backgroundAndVideoEventsLines) {
                obj.BackgroundAndVideoEvents.Add(Event.MakeEvent(line));
            }

            foreach (string line in breakPeriodsLines) {
                obj.BreakPeriods.Add(new Break(line));
            }

            obj.StoryboardLayerBackground.AddRange(Event.ParseEventTree(storyboardLayerBackgroundLines));
            obj.StoryboardLayerFail.AddRange(Event.ParseEventTree(storyboardLayerFailLines));
            obj.StoryboardLayerPass.AddRange(Event.ParseEventTree(storyboardLayerPassLines));
            obj.StoryboardLayerForeground.AddRange(Event.ParseEventTree(storyboardLayerForegroundLines));
            obj.StoryboardLayerOverlay.AddRange(Event.ParseEventTree(storyboardLayerOverlayLines));

            foreach (string line in storyboardSoundSamplesLines) {
                obj.StoryboardSoundSamples.Add(new StoryboardSoundSample(line));
            }
        }

        public Storyboard Decode(string code) {
            var storyboard = new Storyboard();
            Decode(storyboard, code);

            return storyboard;
        }
    }
}