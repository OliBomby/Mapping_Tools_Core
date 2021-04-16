namespace Mapping_Tools_Core.BeatmapHelper.Sections {
    /// <summary>
    /// Contains all the values in the [Difficulty] section of a .osu file.
    /// </summary>
    public class SectionDifficulty {
        public float ApproachRate { get; set; } = 5;
        public float CircleSize { get; set; } = 5;
        public float HpDrainRate { get; set; } = 5;
        public float OverallDifficulty { get; set; } = 5;

        public double SliderMultiplier { get; set; } = 1.4;
        public double SliderTickRate { get; set; } = 1;

        /// <summary>
        /// Maps a difficulty value [0, 10] to a two-piece linear range of values.
        /// </summary>
        /// <param name="difficulty">The difficulty value to be mapped.</param>
        /// <param name="min">Minimum of the resulting range which will be achieved by a difficulty value of 0.</param>
        /// <param name="mid">Midpoint of the resulting range which will be achieved by a difficulty value of 5.</param>
        /// <param name="max">Maximum of the resulting range which will be achieved by a difficulty value of 10.</param>
        /// <returns>Value to which the difficulty value maps in the specified range.</returns>
        public static double DifficultyRange(double difficulty, double min, double mid, double max) {
            if (difficulty > 5)
                return mid + (max - mid) * (difficulty - 5) / 5;
            if (difficulty < 5)
                return mid - (mid - min) * (5 - difficulty) / 5;

            return mid;
        }
    }
}