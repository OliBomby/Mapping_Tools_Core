using Mapping_Tools_Core.BeatmapHelper.Contexts;

namespace Mapping_Tools_Core.Tools.HitsoundCopierStuff {
    /// <summary>
    /// Indicates that an object has participated in hitsound copying.
    /// </summary>
    public class HasCopiedContext : IContext {
        /// <inheritdoc />
        public IContext Copy() {
            return (IContext) MemberwiseClone();
        }
    }
}