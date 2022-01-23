using Mapping_Tools_Core.BeatmapHelper.Events;

namespace Mapping_Tools_Core.BeatmapHelper.Types {
    /// <summary>
    /// Indicates that a type has a storyboard layer.
    /// </summary>
    public interface IHasStoryboardLayer {
        /// <summary>
        /// The storyboard layer this object belongs to.
        /// </summary>
        StoryboardLayer Layer { get; set; }
    }
}