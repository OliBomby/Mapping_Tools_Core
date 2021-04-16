using JetBrains.Annotations;
using Mapping_Tools_Core.BeatmapHelper.ComboColours;
using Mapping_Tools_Core.BeatmapHelper.TimingStuff;
using System.Collections.Generic;
using Mapping_Tools_Core.BeatmapHelper.Sections;

namespace Mapping_Tools_Core.BeatmapHelper {
    public interface IBeatmap : IComboColourCollection {
        /// <summary>
        /// The version number of the beatmap.
        /// <para/>
        /// Version 4 introduces custom samplesets per timing section.<br/>
        /// Version 5 changes the map's offset by 24ms due to an internal calculation change.<br/>
        /// Version 6 changes stacking algorithm and fixes animation speeds for storyboarded sprites.<br/>
        /// Version 7 fixes multipart bezier slider math error (http://osu.sifterapp.com/projects/4151/issues/145)<br/>
        /// Version 8 mm additions: constant sliderticks-per-beat; HP drain changes near breaks; Taiko triple drumrolls.<br/>
        /// Version 9 makes bezier the default slider type, which now handles linear corners better;
        /// Spinner new combos are no longer forced. (Some restrictions re-imposed by the editor.)<br/>
        /// Version 10 fixes sliders being 1/50 shorter than they should be for every bezier part.<br/>
        /// Version 11 Support hold notes.<br/>
        /// Version 14 Support per-node samplesets on sliders (ctb)<br/>
        /// </summary>
        int BeatmapVersion { get; }

        /// <summary>
        /// Contains all the values in the [General] section of a .osu file.
        /// </summary>
        [NotNull]
        SectionGeneral General { get; }

        /// <summary>
        /// Contains all the values in the [Editor] section of a .osu file.
        /// </summary>
        [NotNull]
        SectionEditor Editor { get; }

        /// <summary>
        /// Contains all the values in the [Metadata] section of a .osu file.
        /// </summary>
        [NotNull]
        SectionMetadata Metadata { get; }

        /// <summary>
        /// Contains all the values in the [Difficulty] section of a .osu file.
        /// </summary>
        [NotNull]
        SectionDifficulty Difficulty { get; }

        /// <summary>
        /// Contains all the basic combo colours. The order of this list is the same as how they are numbered in the .osu.
        /// There can not be more than 8 combo colours.
        /// <c>Combo1 : 245,222,139</c>
        /// </summary>
        [NotNull]
        List<IComboColour> ComboColoursList { get; }

        /// <summary>
        /// Contains all the special colours. These include the colours of slider bodies or slider outlines.
        /// The key is the name of the special colour and the value is the actual colour.
        /// </summary>
        [NotNull]
        Dictionary<string, IComboColour> SpecialColours { get; }

        /// <summary>
        /// The timing of this beatmap. This objects contains all the timing points (data from the [TimingPoints] section) plus the global slider multiplier.
        /// It also has a number of helper methods to fetch data from the timing points.
        /// With this object you can always calculate the slider velocity at any time.
        /// Any changes to the slider multiplier property in this object will not be serialized. Change the value in <see cref="Difficulty"/> instead.
        /// </summary>
        [NotNull]
        Timing BeatmapTiming { get; }

        /// <summary>
        /// The storyboard of the Beatmap. Stores everything under the [Events] section.
        /// </summary>
        [NotNull]
        IStoryboard StoryBoard { get; }

        /// <summary>
        /// List of all the hit objects in this beatmap.
        /// </summary>
        [NotNull]
        IReadOnlyList<HitObject> HitObjects { get; }

        /// <summary>
        /// Gets the bookmarks of this beatmap. This returns a clone of the real bookmarks which are stored in the <see cref="Editor"/> property.
        /// The bookmarks are represented with just a double which is the time of the bookmark.
        /// </summary>
        [NotNull]
        List<double> GetBookmarks();

        /// <summary>
        /// Sets the bookmarks of this beatmap. This replaces the bookmarks which are stored in the <see cref="Editor"/> property.
        /// The bookmarks are represented with just a double which is the time of the bookmark.
        /// </summary>
        void SetBookmarks([NotNull] List<double> value);

        /// <summary>
        /// Grabs the specified file name of beatmap file.
        /// with format of:
        /// <c>Artist - Title (Host) [Difficulty].osu</c>
        /// </summary>
        /// <returns>String of file name.</returns>
        string GetFileName();

        /// <summary>
        /// Creates a deep-clone of this beatmap and returns it.
        /// </summary>
        /// <returns>The deep-cloned beatmap</returns>
        IBeatmap DeepClone();

        /// <summary>
        /// Creates a shallow-clone of this beatmap and returns it.
        /// </summary>
        /// <returns>The shallow-cloned beatmap</returns>
        IBeatmap Clone();
    }
}