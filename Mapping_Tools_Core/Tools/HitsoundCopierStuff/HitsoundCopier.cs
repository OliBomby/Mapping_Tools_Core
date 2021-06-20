using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapping_Tools_Core.Audio;
using Mapping_Tools_Core.Audio.DuplicateDetection;
using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.BeatmapHelper.BeatDivisors;
using Mapping_Tools_Core.BeatmapHelper.Contexts;
using Mapping_Tools_Core.BeatmapHelper.Enums;
using Mapping_Tools_Core.BeatmapHelper.Events;
using Mapping_Tools_Core.BeatmapHelper.HitObjects;
using Mapping_Tools_Core.BeatmapHelper.IO.Editor;
using Mapping_Tools_Core.BeatmapHelper.TimelineStuff;
using Mapping_Tools_Core.MathUtil;
using Mapping_Tools_Core.ToolHelpers;

namespace Mapping_Tools_Core.Tools.HitsoundCopierStuff {
    /// <summary>
    /// Hitsound Copier tool.
    /// Allows copying hitsounds between maps with various options.
    /// </summary>
    public class HitsoundCopier {
        /// <summary>
        /// Leniency for dealing with misalignments in time between the beatmaps.
        /// This is the maximum time in milliseconds between two notes that copy hitsounds.
        /// </summary>
        public double TemporalLeniency = 5;

        /// <summary>
        /// Whether to copy hitsounds of hitsound events (circles, sliderhead/tail, spinner end)
        /// </summary>
        public bool DoCopyHitsounds = true;

        /// <summary>
        /// Whether to copy hitsounds of timing points that change the sound of held notes.
        /// </summary>
        public bool DoCopyBodyHitsounds = true;

        /// <summary>
        /// Whether to copy sample set changes.
        /// </summary>
        public bool DoCopySampleSets = true;

        /// <summary>
        /// Whether to copy sample volume changes.
        /// </summary>
        public bool DoCopyVolumes = true;

        /// <summary>
        /// Whether to preserve all 5% volume hitsounds in the destination beatmap regardless of the volume in the source beatmap.
        /// </summary>
        public bool AlwaysPreserve5Volume = true;

        /// <summary>
        /// Whether to copy storyboarded samples.
        /// </summary>
        public bool CopyStoryboardedSamples = false;

        /// <summary>
        /// Whether to prevent copying storyboarded samples that are already played in the hitsounds.
        /// Requires <see cref="IBeatmap.BeatmapSet"/> with all sound samples to be set in the destination beatmap.
        /// </summary>
        public bool IgnoreHitsoundSatisfiedSamples = true;

        /// <summary>
        /// Whether to copy to slider ticks and create new samples for this.
        /// </summary>
        public bool DoCopyToSliderTicks = false;

        /// <summary>
        /// Whether to copy to slider slides and create new samples for this. 
        /// </summary>
        public bool DoCopyToSliderSlides = false;
        
        /// <summary>
        /// The sample index to start counting from when making new samples for slider ticks or slider slides.
        /// </summary>
        /// <remarks>
        /// The sample index will count upwards from this starting value.
        /// </remarks>
        public int StartIndex = 100;


        /// <summary>
        /// Copies hitsounds from one map to another.
        /// </summary>
        /// <remarks>
        /// Copying to slider ticks or slider slides is not supported in this method.
        /// </remarks>
        /// <param name="sourceBeatmap">The map to copy hitsounds from.</param>
        /// <param name="destBeatmap">The map to copy hitsounds to.</param>
        /// <param name="processedTimeline">The timeline of the destination beatmap with <see cref="HasCopiedContext"/> on each timeline object that has been copied to.</param>
        /// <returns></returns>
        public void CopyHitsoundsBasic(IBeatmap sourceBeatmap, IBeatmap destBeatmap, out Timeline processedTimeline) {
            // Every defined hitsound and sampleset on hitsound gets copied to their copyTo destination
            // Timelines
            var tlTo = destBeatmap.GetTimeline();
            var tlFrom = sourceBeatmap.GetTimeline();

            var volumeMuteTimes = DoCopyVolumes && AlwaysPreserve5Volume ? new List<double>() : null;

            if (DoCopyHitsounds) {
                ResetHitObjectHitsounds(destBeatmap);
                CopyHitsounds(tlFrom, tlTo);
            }

            // Save tlo times where timingpoint volume is 5%
            // Timingpointchange all the undefined tlo from copyFrom
            volumeMuteTimes?.AddRange(from tloTo in tlTo.TimelineObjects
                                      where !tloTo.HasContext<HasCopiedContext>() && Math.Abs(tloTo.Hitsounds.Volume) < Precision.DOUBLE_EPSILON
                                                          && Math.Abs(tloTo.FenoSampleVolume - 5) < Precision.DOUBLE_EPSILON
                                      select tloTo.Time);

            // Volumes and samplesets and customindices greenlines get copied with timingpointchanges and allafter enabled
            var controlChanges = sourceBeatmap.BeatmapTiming.TimingPoints.Select(tp =>
                new ControlChange(tp, sampleset: DoCopySampleSets, index: DoCopySampleSets,
                    volume: DoCopyVolumes)).ToList();

            // Apply the timingpoint changes
            ControlChange.ApplyChanges(destBeatmap.BeatmapTiming, controlChanges, true);

            processedTimeline = tlTo;

            // Return 5% volume to tlo that had it before
            if (volumeMuteTimes != null) {
                var timingPointsChangesMute = new List<ControlChange>();
                processedTimeline.GiveTimingContext(destBeatmap.BeatmapTiming);

                // Exclude objects which use their own sample volume property instead
                foreach (var tloTo in processedTimeline.TimelineObjects
                    .Where(o => Math.Abs(o.Hitsounds.Volume) < Precision.DOUBLE_EPSILON)) {
                    if (volumeMuteTimes.Contains(tloTo.Time)) {
                        // Add timingpointschange to copy timingpoint hitsounds
                        var tp = tloTo.GetContext<TimingContext>().HitsoundTimingPoint.Copy();
                        tp.Offset = tloTo.Time;
                        tp.Volume = 5;
                        timingPointsChangesMute.Add(new ControlChange(tp, volume: true));
                    } else {
                        // Add timingpointschange to preserve index and volume
                        var tp = tloTo.GetContext<TimingContext>().HitsoundTimingPoint.Copy();
                        tp.Offset = tloTo.Time;
                        tp.Volume = tloTo.FenoSampleVolume;
                        timingPointsChangesMute.Add(new ControlChange(tp, volume: true));
                    }
                }

                // Apply the timingpoint changes
                ControlChange.ApplyChanges(destBeatmap.BeatmapTiming, timingPointsChangesMute);
            }
            
            if (CopyStoryboardedSamples) {
                CopyStoryboardedSamples(sourceBeatmap, destBeatmap, processedTimeline, true);
            }
        }

        /// <summary>
        /// Copies storyboarded samples between beatmaps.
        /// </summary>
        /// <param name="sourceBeatmap">The beatmap to copy samples from.</param>
        /// <param name="destBeatmap">The beatmap to copy samples to.</param>
        /// <param name="removeOldSamples">Whether to remove the existing storyboarded samples in the destination beatmap.</param>
        public void CopyStoryboardedSamples(IBeatmap sourceBeatmap, IBeatmap destBeatmap, bool removeOldSamples) {
            CopyStoryboardedSamples(sourceBeatmap, destBeatmap, destBeatmap.GetTimeline(), removeOldSamples);
        }

        private void CopyStoryboardedSamples(IBeatmap sourceBeatmap, IBeatmap destBeatmap, Timeline destTimeline, bool removeOldSamples) {
            if (removeOldSamples) {
                destBeatmap.Storyboard.StoryboardSoundSamples.Clear();
            }

            destBeatmap.GiveObjectsTimingContext();
            destTimeline.GiveTimingContext(destBeatmap.BeatmapTiming);

            IDuplicateSampleMap sampleComparer = null;
            string containingFolderPath = string.Empty;
            if (destBeatmap.BeatmapSet != null && IgnoreHitsoundSatisfiedSamples) {
                sampleComparer = new MonolithicDuplicateSampleDetector().AnalyzeSamples(destBeatmap.BeatmapSet.SoundFiles, out _);
                containingFolderPath = Path.GetDirectoryName(destBeatmap.GetBeatmapSetRelativePath()) ?? string.Empty;
            }

            var samplesTo = new HashSet<StoryboardSoundSample>(destBeatmap.Storyboard.StoryboardSoundSamples);
            var mode = destBeatmap.General.Mode;

            foreach (var sampleFrom in sourceBeatmap.Storyboard.StoryboardSoundSamples) {
                // Add the StoryboardSoundSamples from beatmapFrom to beatmapTo only if it doesn't already have the sample
                if (samplesTo.Contains(sampleFrom)) {
                    continue;
                }

                // If IgnoreHitoundSatisfiedSamples and the beatmap set is not null
                if (sampleComparer != null) {
                    var tloHere = destTimeline.TimelineObjects.FindAll(o =>
                        Math.Abs(o.Time - sampleFrom.StartTime) <= TemporalLeniency);
                    var samplesHere = new HashSet<string>();
                    foreach (var tlo in tloHere) {
                        foreach (var filename in tlo.GetFirstPlayingFilenames(mode, containingFolderPath, sampleComparer, false)) {
                            samplesHere.Add(filename);
                        }
                    }

                    var sbSamplePath = Path.Combine(containingFolderPath, sampleFrom.FilePath);

                    sbSamplePath = sampleComparer.GetOriginalSample(sbSamplePath) ?? sbSamplePath;

                    if (samplesHere.Contains(sbSamplePath))
                        continue;
                }

                destBeatmap.Storyboard.StoryboardSoundSamples.Add(sampleFrom);
            }

            // Sort the storyboarded samples
            destBeatmap.Storyboard.StoryboardSoundSamples.Sort();
        }

        /// <summary>
        /// Copies hitsounds from one map to another.
        /// This smart version will preserve all hitsounds in objects of the destination beatmap that didn't get anything copied to them.
        /// </summary>
        /// <param name="sourceBeatmap">The map to copy hitsounds from.</param>
        /// <param name="destBeatmap">The map to copy hitsounds to.</param>
        /// <param name="processedTimeline">The timeline of the destination beatmap with <see cref="HasCopiedContext"/> on each timeline object that has been copied to.</param>
        /// <param name="sampleSchema">The sample schema to add new samples to when copy to slider ticks or copy to slider slides is enabled.</param>
        /// <returns></returns>
        public void CopyHitsoundsSmart(IBeatmap sourceBeatmap, IBeatmap destBeatmap, out Timeline processedTimeline, SampleSchema sampleSchema = null) {
            destBeatmap = editorTo.Beatmap;
            sourceBeatmap;

            if (!string.IsNullOrEmpty(arg.PathFrom)) {
                var editorFrom = EditorReaderStuff.GetNewestVersionOrNot(arg.PathFrom, reader);
                sourceBeatmap = editorFrom.Beatmap;
            } else {
                // Copy from an empty beatmap similar to the map to copy to
                sourceBeatmap = destBeatmap.DeepCopy();
                sourceBeatmap.HitObjects.Clear();
                sourceBeatmap.BeatmapTiming.Clear();
            }

            if (arg.CopyMode == 0) {
                // Every defined hitsound and sampleset on hitsound gets copied to their copyTo destination
                // Timelines
                var tlTo = destBeatmap.GetTimeline();
                var tlFrom = sourceBeatmap.GetTimeline();

                var volumeMuteTimes = arg.CopyVolumes && arg.AlwaysPreserve5Volume ? new List<double>() : null;

                if (arg.CopyHitsounds) {
                    ResetHitObjectHitsounds(destBeatmap);
                    CopyHitsounds(arg, tlFrom, tlTo);
                }

                // Save tlo times where timingpoint volume is 5%
                // Timingpointchange all the undefined tlo from copyFrom
                volumeMuteTimes?.AddRange(from tloTo in tlTo.TimelineObjects
                                          where tloTo.CanCopy && Math.Abs(tloTo.SampleVolume) < Precision.DOUBLE_EPSILON
                                                              && Math.Abs(tloTo.FenoSampleVolume - 5) < Precision.DOUBLE_EPSILON
                                          select tloTo.Time);

                // Volumes and samplesets and customindices greenlines get copied with timingpointchanges and allafter enabled
                var timingPointsChanges = sourceBeatmap.BeatmapTiming.TimingPoints.Select(tp =>
                    new TimingPointsChange(tp, sampleset: arg.CopySampleSets, index: arg.CopySampleSets,
                        volume: arg.CopyVolumes)).ToList();

                // Apply the timingpoint changes
                TimingPointsChange.ApplyChanges(destBeatmap.BeatmapTiming, timingPointsChanges, true);

                processedTimeline = tlTo;

                // Return 5% volume to tlo that had it before
                if (volumeMuteTimes != null) {
                    var timingPointsChangesMute = new List<TimingPointsChange>();
                    processedTimeline.GiveTimingPoints(destBeatmap.BeatmapTiming);

                    // Exclude objects which use their own sample volume property instead
                    foreach (var tloTo in processedTimeline.TimelineObjects.Where(o => Math.Abs(o.SampleVolume) < Precision.DOUBLE_EPSILON)) {
                        if (volumeMuteTimes.Contains(tloTo.Time)) {
                            // Add timingpointschange to copy timingpoint hitsounds
                            var tp = tloTo.HitsoundTimingPoint.Copy();
                            tp.Offset = tloTo.Time;
                            tp.Volume = 5;
                            timingPointsChangesMute.Add(new TimingPointsChange(tp, volume: true));
                        } else {
                            // Add timingpointschange to preserve index and volume
                            var tp = tloTo.HitsoundTimingPoint.Copy();
                            tp.Offset = tloTo.Time;
                            tp.Volume = tloTo.FenoSampleVolume;
                            timingPointsChangesMute.Add(new TimingPointsChange(tp, volume: true));
                        }
                    }

                    // Apply the timingpoint changes
                    TimingPointsChange.ApplyChanges(destBeatmap.BeatmapTiming, timingPointsChangesMute);
                }
            } else {
                // Smarty mode
                // Copy the defined hitsounds literally (not feno, that will be reserved for cleaner). Only the tlo that have been defined by copyFrom get overwritten.
                var tlTo = destBeatmap.GetTimeline();
                var tlFrom = sourceBeatmap.GetTimeline();

                var timingPointsChanges = new List<TimingPointsChange>();
                var mode = (GameMode)destBeatmap.General["Mode"].IntValue;
                var mapDir = editorTo.GetParentFolder();
                var firstSamples = HitsoundImporter.AnalyzeSamples(mapDir);

                if (arg.CopyHitsounds) {
                    CopyHitsounds(arg, destBeatmap, tlFrom, tlTo, timingPointsChanges, mode, mapDir, firstSamples, ref sampleSchema);
                }

                if (arg.CopyBodyHitsounds) {
                    // Remove timingpoints in beatmapTo that are in a sliderbody/spinnerbody for both beatmapTo and BeatmapFrom
                    foreach (var tp in from ho in destBeatmap.HitObjects
                                       from tp in ho.BodyHitsounds
                                       where sourceBeatmap.HitObjects.Any(o => o.Time < tp.Offset && o.EndTime > tp.Offset)
                                       where !tp.Uninherited
                                       select tp) {
                        destBeatmap.BeatmapTiming.Remove(tp);
                    }

                    // Get timingpointschanges for every timingpoint from beatmapFrom that is in a sliderbody/spinnerbody for both beatmapTo and BeatmapFrom
                    timingPointsChanges.AddRange(from ho in sourceBeatmap.HitObjects
                                                 from tp in ho.BodyHitsounds
                                                 where destBeatmap.HitObjects.Any(o => o.Time < tp.Offset && o.EndTime > tp.Offset)
                                                 select new TimingPointsChange(tp.Copy(), sampleset: arg.CopySampleSets, index: arg.CopySampleSets,
                                                     volume: arg.CopyVolumes));
                }

                // Apply the timingpoint changes
                TimingPointsChange.ApplyChanges(destBeatmap.BeatmapTiming, timingPointsChanges);

                processedTimeline = tlTo;
            }

            if (arg.CopyStoryboardedSamples) {
                if (arg.CopyMode == 0) {
                    destBeatmap.StoryboardSoundSamples.Clear();
                }

                destBeatmap.GiveObjectsGreenlines();
                processedTimeline.GiveTimingPoints(destBeatmap.BeatmapTiming);

                var mapDir = editorTo.GetParentFolder();
                var firstSamples = HitsoundImporter.AnalyzeSamples(mapDir, true);

                var samplesTo = new HashSet<StoryboardSoundSample>(destBeatmap.StoryboardSoundSamples);
                var mode = (GameMode)destBeatmap.General["Mode"].IntValue;

                foreach (var sampleFrom in sourceBeatmap.StoryboardSoundSamples) {
                    if (arg.IgnoreHitsoundSatisfiedSamples) {
                        var tloHere = processedTimeline.TimelineObjects.FindAll(o =>
                            Math.Abs(o.Time - sampleFrom.StartTime) <= arg.TemporalLeniency);
                        var samplesHere = new HashSet<string>();
                        foreach (var tlo in tloHere) {
                            foreach (var filename in tlo.GetPlayingFilenames(mode)) {
                                var samplePath = Path.Combine(mapDir, filename);
                                var fullPathExtLess = Path.Combine(Path.GetDirectoryName(samplePath),
                                    Path.GetFileNameWithoutExtension(samplePath));

                                if (firstSamples.Keys.Contains(fullPathExtLess)) {
                                    samplePath = firstSamples[fullPathExtLess];
                                }

                                samplesHere.Add(samplePath);
                            }
                        }

                        var sbSamplePath = Path.Combine(mapDir, sampleFrom.FilePath);
                        var sbFullPathExtLess = Path.Combine(Path.GetDirectoryName(sbSamplePath),
                            Path.GetFileNameWithoutExtension(sbSamplePath));

                        if (firstSamples.Keys.Contains(sbFullPathExtLess)) {
                            sbSamplePath = firstSamples[sbFullPathExtLess];
                        }

                        if (samplesHere.Contains(sbSamplePath))
                            continue;
                    }

                    // Add the StoryboardSoundSamples from beatmapFrom to beatmapTo if it doesn't already have the sample
                    if (!samplesTo.Contains(sampleFrom)) {
                        destBeatmap.StoryboardSoundSamples.Add(sampleFrom);
                    }
                }

                // Sort the storyboarded samples
                destBeatmap.StoryboardSoundSamples.Sort();
            }

            if (arg.MuteSliderends) {
                var timingPointsChanges = new List<TimingPointsChange>();
                destBeatmap.GiveObjectsGreenlines();
                processedTimeline.GiveTimingPoints(destBeatmap.BeatmapTiming);

                foreach (var tloTo in processedTimeline.TimelineObjects) {
                    if (FilterMuteTlo(tloTo, destBeatmap, arg)) {
                        // Set volume to 5%, remove all hitsounds, apply customindex and sampleset
                        tloTo.SampleSet = arg.MutedSampleSet;
                        tloTo.AdditionSet = 0;
                        tloTo.Normal = false;
                        tloTo.Whistle = false;
                        tloTo.Finish = false;
                        tloTo.Clap = false;

                        tloTo.HitsoundsToOrigin();

                        // Add timingpointschange to copy timingpoint hitsounds
                        var tp = tloTo.HitsoundTimingPoint.Copy();
                        tp.Offset = tloTo.Time;
                        tp.SampleSet = arg.MutedSampleSet;
                        tp.SampleIndex = arg.MutedIndex;
                        tp.Volume = 5;
                        timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: true, index: doMutedIndex,
                            volume: true));
                    } else {
                        // Add timingpointschange to preserve index and volume and sampleset
                        var tp = tloTo.HitsoundTimingPoint.Copy();
                        tp.Offset = tloTo.Time;
                        timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: true, index: doMutedIndex,
                            volume: true));
                    }
                }

                // Apply the timingpoint changes
                TimingPointsChange.ApplyChanges(destBeatmap.BeatmapTiming, timingPointsChanges);
            }
        }

        private void CopyHitsounds(Timeline tlFrom, Timeline tlTo) {
            foreach (var tloFrom in tlFrom.TimelineObjects) {
                var tloTo = tlTo.GetNearestTlo(tloFrom.Time, true);

                if (tloTo != null &&
                    Math.Abs(Math.Round(tloFrom.Time) - Math.Round(tloTo.Time)) <= arg.TemporalLeniency) {
                    // Copy to this tlo
                    CopyHitsounds(arg, tloFrom, tloTo);
                }

                tloFrom.CanCopy = false;
            }
        }

        private void CopyHitsounds(HitsoundCopierVm arg, Beatmap beatmapTo,
            Timeline tlFrom, Timeline tlTo,
            List<TimingPointsChange> timingPointsChanges, GameMode mode, string mapDir,
            Dictionary<string, string> firstSamples, ref SampleSchema sampleSchema) {

            var CustomSampledTimes = new HashSet<int>();
            var tloToSliderSlide = new List<TimelineObject>();

            foreach (var tloFrom in tlFrom.TimelineObjects) {
                var tloTo = tlTo.GetNearestTlo(tloFrom.Time, true);

                if (tloTo != null &&
                    Math.Abs(Math.Round(tloFrom.Time) - Math.Round(tloTo.Time)) <= arg.TemporalLeniency) {
                    // Copy to this tlo
                    CopyHitsounds(arg, tloFrom, tloTo);

                    // Add timingpointschange to copy timingpoint hitsounds
                    var tp = tloFrom.HitsoundTimingPoint.Copy();
                    tp.Offset = tloTo.Time;
                    timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: arg.CopySampleSets,
                        index: arg.CopySampleSets, volume: arg.CopyVolumes));
                }
                // Try to find a slider tick in range to copy the sample to instead.
                // This slider tick gets a custom sample and timingpoints change to imitate the copied hitsound.
                else
                if (arg.CopyToSliderTicks &&
                           FindSliderTickInRange(beatmapTo, tloFrom.Time - arg.TemporalLeniency, tloFrom.Time + arg.TemporalLeniency, out var sliderTickTime, out var tickSlider) &&
                           !CustomSampledTimes.Contains((int)sliderTickTime)) {
                    // Add a new custom sample to this slider tick to represent the hitsounds
                    List<string> sampleFilenames = tloFrom.GetFirstPlayingFilenames(mode, mapDir, firstSamples, false);
                    List<SampleGeneratingArgs> samples = sampleFilenames
                        .Select(o => new SampleGeneratingArgs(Path.Combine(mapDir, o)))
                        .Where(o => SampleImporter.ValidateSampleArgs(o, true))
                        .ToList();

                    if (samples.Count > 0) {
                        if (sampleSchema.AddHitsound(samples, "slidertick", tloFrom.FenoSampleSet,
                            out int index, out var sampleSet, arg.StartIndex)) {
                            // Add a copy of the slider slide sound to this index if necessary
                            var oldIndex = tloFrom.HitsoundTimingPoint.SampleIndex;
                            var oldSampleSet = tloFrom.HitsoundTimingPoint.SampleSet;
                            var oldSlideFilename =
                                $"{oldSampleSet.ToString().ToLower()}-sliderslide{(oldIndex == 1 ? string.Empty : oldIndex.ToInvariant())}";
                            var oldSlidePath = Path.Combine(mapDir, oldSlideFilename);

                            if (firstSamples.ContainsKey(oldSlidePath)) {
                                oldSlidePath = firstSamples[oldSlidePath];
                                var slideGeneratingArgs = new SampleGeneratingArgs(oldSlidePath);
                                var newSlideFilename =
                                    $"{sampleSet.ToString().ToLower()}-sliderslide{index.ToInvariant()}";

                                sampleSchema.Add(newSlideFilename,
                                    new List<SampleGeneratingArgs> { slideGeneratingArgs });
                            }
                        }

                        // Make sure the slider with the slider ticks uses auto sampleset so the customized greenlines control the hitsounds
                        tickSlider.SampleSet = SampleSet.None;

                        // Add timingpointschange
                        var tp = tloFrom.HitsoundTimingPoint.Copy();
                        tp.Offset = sliderTickTime;
                        tp.SampleIndex = index;
                        tp.SampleSet = sampleSet;
                        tp.Volume = tloFrom.FenoSampleVolume;
                        timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: arg.CopySampleSets,
                            index: arg.CopySampleSets, volume: arg.CopyVolumes));

                        // Add timingpointschange 5ms later to revert the stuff back to whatever it should be
                        var tp2 = tloFrom.HitsoundTimingPoint.Copy();
                        tp2.Offset = sliderTickTime + 5;
                        timingPointsChanges.Add(new TimingPointsChange(tp2, sampleset: arg.CopySampleSets,
                            index: arg.CopySampleSets, volume: arg.CopyVolumes));

                        CustomSampledTimes.Add((int)sliderTickTime);
                    }
                }
                // If the there is no slidertick to be found, then try copying it to the slider slide
                else
                if (arg.CopyToSliderSlides) {
                    tloToSliderSlide.Add(tloFrom);
                }

                tloFrom.CanCopy = false;
            }

            // Do the sliderslide hitsounds after because the ticks need to add sliderslides with strict indices.
            foreach (var tlo in tloToSliderSlide) {
                if (!FindSliderAtTime(beatmapTo, tlo.Time, out var slideSlider) ||
                      CustomSampledTimes.Contains((int)tlo.Time))
                    continue;

                // Add a new custom sample to this slider slide to represent the hitsounds
                List<string> sampleFilenames = tlo.GetFirstPlayingFilenames(mode, mapDir, firstSamples, false);
                List<SampleGeneratingArgs> samples = sampleFilenames
                    .Select(o => new SampleGeneratingArgs(Path.Combine(mapDir, o)))
                    .Where(o => SampleImporter.ValidateSampleArgs(o))
                    .ToList();

                if (samples.Count > 0) {
                    sampleSchema.AddHitsound(samples, "sliderslide", tlo.FenoSampleSet,
                        out int index, out var sampleSet, arg.StartIndex);

                    // Add timingpointschange
                    var tp = tlo.HitsoundTimingPoint.Copy();
                    tp.Offset = tlo.Time;
                    tp.SampleIndex = index;
                    tp.SampleSet = sampleSet;
                    tp.Volume = tlo.FenoSampleVolume;
                    timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: arg.CopySampleSets,
                        index: arg.CopySampleSets, volume: arg.CopyVolumes));

                    // Make sure the slider with the slider ticks uses auto sampleset so the customized greenlines control the hitsounds
                    slideSlider.SampleSet = SampleSet.None;
                }
            }

            // Timingpointchange all the undefined tlo from copyFrom
            foreach (var tloTo in tlTo.TimelineObjects) {
                if (!tloTo.CanCopy) continue;
                var tp = tloTo.HitsoundTimingPoint.Copy();
                var holdSampleset = arg.CopySampleSets && tloTo.SampleSet == SampleSet.None;
                var holdIndex = arg.CopySampleSets && !(tloTo.CanCustoms && tloTo.CustomIndex != 0);

                // Dont hold indexes or sampleset if the sample it plays currently is the same as the sample it would play without conserving
                if (holdSampleset || holdIndex) {
                    var nativeSamples = tloTo.GetFirstPlayingFilenames(mode, mapDir, firstSamples);

                    if (holdSampleset) {
                        var oldSampleSet = tloTo.FenoSampleSet;
                        var newSampleSet = tloTo.FenoSampleSet;
                        var latest = double.NegativeInfinity;
                        foreach (TimingPointsChange tpc in timingPointsChanges) {
                            if (!tpc.Sampleset || !(tpc.MyTP.Offset <= tloTo.Time) || !(tpc.MyTP.Offset >= latest))
                                continue;
                            newSampleSet = tpc.MyTP.SampleSet;
                            latest = tpc.MyTP.Offset;
                        }

                        tp.SampleSet = newSampleSet;
                        tloTo.GiveHitsoundTimingPoint(tp);
                        var newSamples = tloTo.GetFirstPlayingFilenames(mode, mapDir, firstSamples);
                        tp.SampleSet = nativeSamples.SequenceEqual(newSamples) ? newSampleSet : oldSampleSet;
                    }

                    if (holdIndex) {
                        var oldIndex = tloTo.FenoCustomIndex;
                        var newIndex = tloTo.FenoCustomIndex;
                        var latest = double.NegativeInfinity;
                        foreach (var tpc in timingPointsChanges) {
                            if (!tpc.Index || !(tpc.MyTP.Offset <= tloTo.Time) || !(tpc.MyTP.Offset >= latest))
                                continue;
                            newIndex = tpc.MyTP.SampleIndex;
                            latest = tpc.MyTP.Offset;
                        }

                        tp.SampleIndex = newIndex;
                        tloTo.GiveHitsoundTimingPoint(tp);
                        var newSamples = tloTo.GetFirstPlayingFilenames(mode, mapDir, firstSamples);
                        tp.SampleIndex = nativeSamples.SequenceEqual(newSamples) ? newIndex : oldIndex;
                    }

                    tloTo.GiveHitsoundTimingPoint(tp);
                }

                tp.Offset = tloTo.Time;
                timingPointsChanges.Add(new TimingPointsChange(tp, sampleset: holdSampleset, index: holdIndex,
                    volume: arg.CopyVolumes));
            }
        }

        private static bool FindSliderTickInRange(Beatmap beatmap, double startTime, double endTime, out double sliderTickTime, out HitObject tickSlider) {
            var tickrate = beatmap.Difficulty.ContainsKey("SliderTickRate")
                ? beatmap.Difficulty["SliderTickRate"].DoubleValue : 1.0;

            // Check all sliders in range and exclude those which have NaN SV, because those dont have slider ticks
            foreach (var slider in beatmap.HitObjects.Where(o => o.IsSlider &&
                                                                 !double.IsNaN(o.SliderVelocity) &&
                                                                 (o.Time < endTime || o.EndTime > startTime))) {
                var timeBetweenTicks = slider.UnInheritedTimingPoint.MpB / tickrate;

                sliderTickTime = slider.Time + timeBetweenTicks;
                while (sliderTickTime < slider.EndTime - 5) {  // This -5 is to make sure the +5 ms timingpoint that reverts the change is still inside the slider
                    if (sliderTickTime >= startTime && sliderTickTime <= endTime) {
                        tickSlider = slider;
                        return true;
                    }
                    sliderTickTime += timeBetweenTicks;
                }
            }

            sliderTickTime = -1;
            tickSlider = null;
            return false;
        }

        private static bool FindSliderAtTime(Beatmap beatmap, double time, out HitObject slider) {
            slider = beatmap.HitObjects.FirstOrDefault(ho => ho.IsSlider && ho.Time < time && ho.EndTime > time);
            return slider != null;
        }

        private void CopyHitsounds(TimelineObject tloFrom, TimelineObject tloTo) {
            // Copy to this tlo
            tloTo.SampleSet = tloFrom.SampleSet;
            tloTo.AdditionSet = tloFrom.AdditionSet;
            tloTo.Normal = tloFrom.Normal;
            tloTo.Whistle = tloFrom.Whistle;
            tloTo.Finish = tloFrom.Finish;
            tloTo.Clap = tloFrom.Clap;

            if (tloTo.CanCustoms) {
                tloTo.CustomIndex = tloFrom.CustomIndex;
                tloTo.SampleVolume = tloFrom.SampleVolume;
                tloTo.Filename = tloFrom.Filename;
            }

            // Copy sliderbody hitsounds
            if (tloTo.IsSliderHead && tloFrom.IsSliderHead && arg.CopyBodyHitsounds) {
                tloTo.Origin.Hitsounds = tloFrom.Origin.Hitsounds;
                tloTo.Origin.SampleSet = tloFrom.Origin.SampleSet;
                tloTo.Origin.AdditionSet = tloFrom.Origin.AdditionSet;
            }

            tloTo.HitsoundsToOrigin();
            tloTo.CanCopy = false;
        }

        private static void ResetHitObjectHitsounds(IBeatmap beatmap) {
            foreach (var ho in beatmap.HitObjects) {
                // Remove all hitsounds
                ho.Clap = false;
                ho.Whistle = false;
                ho.Finish = false;
                ho.Clap = false;
                ho.SampleSet = 0;
                ho.AdditionSet = 0;
                ho.CustomIndex = 0;
                ho.SampleVolume = 0;
                ho.Filename = "";

                if (!ho.IsSlider) continue;
                // Remove edge hitsounds
                ho.EdgeHitsounds = ho.EdgeHitsounds.Select(o => 0).ToList();
                ho.EdgeSampleSets = ho.EdgeSampleSets.Select(o => SampleSet.None).ToList();
                ho.EdgeAdditionSets = ho.EdgeAdditionSets.Select(o => SampleSet.None).ToList();
            }
        }
    }
}