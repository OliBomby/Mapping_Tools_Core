using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mapping_Tools_Core.BeatmapHelper {
    /// <summary>
    /// Helper class for File Formats
    /// </summary>
    public static class FileFormatHelper {
        public static string[] SplitKeyValue(string line) {
            return line.Split(new[] { ':' }, 2);
        }

        public static IEnumerable<string> GetCategoryLines(IEnumerable<string> lines, string category, string[] categoryIdentifiers=null) {
            if (categoryIdentifiers == null)
                categoryIdentifiers = new[] { "[" };

            bool atCategory = false;

            foreach (string line in lines) {
                if (atCategory && line != "") {
                    if (categoryIdentifiers.Any(o => line.StartsWith(o))) // Reached another category
                    {
                        yield break;
                    }
                    yield return line;
                }
                else {
                    if (line == category) {
                        atCategory = true;
                    }
                }
            }
        }

        public static int ParseInt(string s) {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        public static float ParseFloat(string s) {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }

        public static double ParseDouble(string s) {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
