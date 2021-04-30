﻿using System.Text.Json.Serialization;

namespace Mapping_Tools_Core.BeatmapHelper.BeatDivisors {
    public class IrrationalBeatDivisor : IBeatDivisor {
        public double Value { get; }

        [JsonConstructor]
        public IrrationalBeatDivisor(double value) {
            Value = value;
        }

        public static implicit operator IrrationalBeatDivisor(double value) {
            return new IrrationalBeatDivisor(value);
        }

        public double GetValue() {
            return Value;
        }

        protected bool Equals(IrrationalBeatDivisor other) {
            return Value.Equals(other.Value);
        }

        public bool Equals(IBeatDivisor other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is IrrationalBeatDivisor otherIrrational) return Equals(otherIrrational);
            return false;
        }

        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((IrrationalBeatDivisor) obj);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }
}