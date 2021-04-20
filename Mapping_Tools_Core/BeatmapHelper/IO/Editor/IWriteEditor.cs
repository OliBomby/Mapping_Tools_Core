﻿namespace Mapping_Tools_Core.BeatmapHelper.IO.Editor {
    /// <summary>
    /// Write interface./>
    /// </summary>
    public interface IWriteEditor<in T> : IEditor {
        /// <summary>
        /// Writes the given object.
        /// </summary>
        /// <param name="instance">The object to write.</param>
        void WriteFile(T instance);
    }
}