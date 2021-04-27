using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mapping_Tools_Core.BeatmapHelper.TimelineStuff;
using Mapping_Tools_Core.BeatmapHelper.Types;

namespace Mapping_Tools_Core.BeatmapHelper.Contexts {
    public class TimelineContext : IContext {
        /// <summary>
        /// The timeline objects associated with this hit object.
        /// </summary>
        [NotNull]
        public List<TimelineObject> TimelineObjects { get; set; }

        public TimelineContext() {
            TimelineObjects = new List<TimelineObject>();
        }

        public TimelineContext(IEnumerable<TimelineObject> timelineObjects) {
            TimelineObjects = timelineObjects.ToList();
        }

        /// <summary>
        /// Update the associated timeline object with new time information.
        /// </summary>
        /// <param name="hitObject">The hit object to align the timeline objects with.</param>
        public void UpdateTimelineObjectTimes<T>(T hitObject) where T : IHasStartTime {
            switch (hitObject) {
                case IHasRepeats hasRepeats: {
                    for (int i = 0; i < TimelineObjects.Count; i++) {
                        double time = Math.Floor(hitObject.StartTime + hasRepeats.SpanDuration * i);
                        TimelineObjects[i].Time = time;
                    }
                    break;
                }
                case IHasDuration hasDuration: {
                    for (int i = 0; i < TimelineObjects.Count; i++) {
                        double time = Math.Floor(hitObject.StartTime + hasDuration.Duration / (TimelineObjects.Count - 1) * i);
                        TimelineObjects[i].Time = time;
                    }
                    break;
                }
                default: {
                    // Offset everything to match the start time
                    var offset = hitObject.StartTime - TimelineObjects[0].Time;
                    foreach (var tlo in TimelineObjects) {
                        tlo.Time += offset;
                    }
                    break;
                }
            }
        }

        public IContext Copy() {
            return new TimelineContext(TimelineObjects.Select(o => o.Copy()));
        }
    }
}