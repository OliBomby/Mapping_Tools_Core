﻿using Mapping_Tools_Core.BeatmapHelper.IO.Encoding;
using Mapping_Tools_Core.BeatmapHelper.TimingStuff;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mapping_Tools_Core.ToolHelpers {
    /// <summary>
    /// Struct for helping making changes to <see cref="Timing"/>.
    /// </summary>
    public struct ControlChange {

        public TimingPoint MyTP;
        public bool MpB;
        public bool Meter;
        public bool Sampleset;
        public bool Index;
        public bool Volume;
        public bool Uninherited;
        public bool Kiai;
        public bool OmitFirstBarLine;
        public double Fuzzyness;

        public ControlChange(TimingPoint tpNew, bool mpb = false, bool meter = false, bool sampleset = false, bool index = false, bool volume = false, bool uninherited = false, bool kiai = false, bool omitFirstBarLine = false, double fuzzyness=2) {
            MyTP = tpNew;
            MpB = mpb;
            Meter = meter;
            Sampleset = sampleset;
            Index = index;
            Volume = volume;
            Uninherited = uninherited;
            Kiai = kiai;
            OmitFirstBarLine = omitFirstBarLine;
            Fuzzyness = fuzzyness;
        }

        public void AddChange(Timing timing, bool allAfter = false) {
            TimingPoint addingTimingPoint = null;
            TimingPoint prevTimingPoint = null;
            List<TimingPoint> onTimingPoints = new List<TimingPoint>();
            bool onHasRed = false;
            bool onHasGreen = false;

            foreach (TimingPoint tp in timing) {
                if (tp == null) { continue; }  // Continue nulls to avoid exceptions
                if (tp.Offset < MyTP.Offset && (prevTimingPoint == null || tp.Offset >= prevTimingPoint.Offset)) {
                    prevTimingPoint = tp;
                }
                if (Math.Abs(tp.Offset - MyTP.Offset) <= Fuzzyness) {
                    onTimingPoints.Add(tp);
                    onHasRed = tp.Uninherited || onHasRed;
                    onHasGreen = !tp.Uninherited || onHasGreen;
                }
            }

            if (onTimingPoints.Count > 0) {
                prevTimingPoint = onTimingPoints.Last();
            }

            if (Uninherited && !onHasRed) {
                // Make new redline
                if (prevTimingPoint == null) {
                    addingTimingPoint = MyTP.Copy();
                    addingTimingPoint.Uninherited = true;
                } else {
                    addingTimingPoint = prevTimingPoint.Copy();
                    addingTimingPoint.Offset = MyTP.Offset;
                    addingTimingPoint.Uninherited = true;
                }
                onTimingPoints.Add(addingTimingPoint);
            }
            if (!Uninherited && (onTimingPoints.Count == 0 || (MpB && !onHasGreen))) {
                // Make new greenline (based on prev)
                if (prevTimingPoint == null) {
                    addingTimingPoint = MyTP.Copy();
                    addingTimingPoint.Uninherited = false;
                } else {
                    addingTimingPoint = prevTimingPoint.Copy();
                    addingTimingPoint.Offset = MyTP.Offset;
                    addingTimingPoint.Uninherited = false;
                    if (prevTimingPoint.Uninherited) { addingTimingPoint.MpB = -100; }
                }
                onTimingPoints.Add(addingTimingPoint);
            }

            foreach (TimingPoint on in onTimingPoints) {
                if (MpB && (Uninherited ? on.Uninherited : !on.Uninherited)) { on.MpB = MyTP.MpB; }
                if (Meter && Uninherited && on.Uninherited) { on.Meter = MyTP.Meter; }
                if (Sampleset) { on.SampleSet = MyTP.SampleSet; }
                if (Index) { on.SampleIndex = MyTP.SampleIndex; }
                if (Volume) { on.Volume = MyTP.Volume; }
                if (Kiai) { on.Kiai = MyTP.Kiai; }
                if (OmitFirstBarLine && Uninherited && on.Uninherited) { on.OmitFirstBarLine = MyTP.OmitFirstBarLine; }
            }

            if (addingTimingPoint != null && (prevTimingPoint == null || !addingTimingPoint.SameEffect(prevTimingPoint) || Uninherited)) {
                timing.Add(addingTimingPoint);
            }

            if (allAfter) // Change every timingpoint after
            {
                foreach (TimingPoint tp in timing) {
                    if (tp.Offset > MyTP.Offset) {
                        if (Sampleset) { tp.SampleSet = MyTP.SampleSet; }
                        if (Index) { tp.SampleIndex = MyTP.SampleIndex; }
                        if (Volume) { tp.Volume = MyTP.Volume; }
                        if (Kiai) { tp.Kiai = MyTP.Kiai; }
                    }
                }
            }
        }

        public static void ApplyChanges(Timing timing, IEnumerable<ControlChange> timingPointsChanges, bool allAfter = false) {
            timingPointsChanges = timingPointsChanges.OrderBy(o => o.MyTP.Offset);
            foreach (ControlChange c in timingPointsChanges) {
                c.AddChange(timing, allAfter);
            }
        }

        public void Debug() {
            Console.WriteLine(new TimingPointEncoder(true).Encode(MyTP));
            Console.WriteLine($"{MpB}, {Meter}, {Sampleset}, {Index}, {Volume}, {Uninherited}, {Kiai}, {OmitFirstBarLine}");
        }
    }
}
