﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.MathUtil;

namespace Mapping_Tools_Core.Tools.SnappingTools.DataStructure.RelevantObject.RelevantObjects {
    public class RelevantHitObject : RelevantObject {
        public HitObject HitObject { get; set; }

        public override double Time {
            get => HitObject.StartTime;
            set {
                HitObject.StartTime = value;
                if (ChildObjects == null) return;
                foreach (var relevantObject in ChildObjects) {
                    relevantObject.UpdateTime();
                }
            }
        }

        public override bool IsSelected {
            get => HitObject.IsSelected;
            set => HitObject.IsSelected = value;
        }

        [UsedImplicitly]
        public RelevantHitObject() { }

        public RelevantHitObject(HitObject hitObject) {
            HitObject = hitObject;
        }

        public double Difference(RelevantHitObject other) {
            if (HitObject.ObjectType != other.HitObject.ObjectType) {
                return double.PositiveInfinity;
            }

            if (HitObject.SliderType != other.HitObject.SliderType) {
                return double.PositiveInfinity;
            }

            if (HitObject.CurvePoints.Count != other.HitObject.CurvePoints.Count) {
                return double.PositiveInfinity;
            }

            var differences = new List<double> {Vector2.DistanceSquared(HitObject.Pos, other.HitObject.Pos)};
            differences.AddRange(HitObject.CurvePoints.Select((t, i) => Vector2.DistanceSquared(t, other.HitObject.CurvePoints[i])));

            return differences.Sum() / differences.Count;
        }

        public override double DistanceTo(IRelevantObject relevantObject) {
            return double.PositiveInfinity;
        }
    }
}
