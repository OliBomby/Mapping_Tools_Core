using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.BeatmapHelper.Decoding;
using Mapping_Tools_Core.BeatmapHelper.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Mapping_Tools_Core_Tests.BeatmapHelper {
    [TestClass]
    public class BeatmapHelperTests {
        [TestMethod]
        public void UnchangingEmptyMapCodeTest() {
            var path = "Resources\\EmptyTestMap.osu";
            var lines = File.ReadAllText(path);
            var decoder = new OsuBeatmapDecoder();
            var encoder = new OsuBeatmapEncoder();

            TestUnchanging(lines, decoder, encoder);
        }

        [TestMethod]
        public void UnchangingComplicatedMapCodeTest() {
            var path = "Resources\\ComplicatedTestMap.osu";
            var lines = File.ReadAllText(path);
            var decoder = new OsuBeatmapDecoder();
            var encoder = new OsuBeatmapEncoder();

            TestUnchanging(lines, decoder, encoder);
        }

        private static void TestUnchanging(string lines, IDecoder<Beatmap> decoder, IEncoder<Beatmap> encoder) {
            var lines2 = encoder.Encode(decoder.DecodeNew(lines));

            Assert.AreEqual(lines, lines2);
        }
    }
}