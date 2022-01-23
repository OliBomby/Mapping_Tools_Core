using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.BeatmapHelper.IO.Decoding;
using Mapping_Tools_Core.BeatmapHelper.IO.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace Mapping_Tools_Core_Tests.BeatmapHelper {
    [TestClass]
    public class BeatmapParsingTests {
        [TestMethod]
        public void UnchangingEmptyMapCodeTest() {
            var path = Path.Join("Resources", "EmptyTestMap.osu");
            var lines = File.ReadAllText(path);
            var decoder = new OsuBeatmapDecoder();
            var encoder = new OsuBeatmapEncoder();

            TestUnchanging(lines, decoder, encoder);
        }

        [TestMethod]
        public void UnchangingComplicatedMapCodeTest() {
            var path = Path.Join("Resources", "ComplicatedTestMap.osu");
            var lines = File.ReadAllText(path);
            var decoder = new OsuBeatmapDecoder();
            var encoder = new OsuBeatmapEncoder();

            TestUnchanging(lines, decoder, encoder);
        }

        private static void TestUnchanging(string lines, IDecoder<Beatmap> decoder, IEncoder<Beatmap> encoder) {
            var map = decoder.Decode(lines);
            var lines2 = encoder.Encode(map);

            //Debug.Print(lines);
            //Debug.Print(lines2);

            // Split equal asserting to lines so we know where the difference is
            var linesSplit = lines.Split(Environment.NewLine);
            var lines2Split = lines2.Split(Environment.NewLine);

            for (int i = 0; i < linesSplit.Length; i++) {
                Assert.IsTrue(i < lines2Split.Length);
                Assert.AreEqual(linesSplit[i], lines2Split[i], $"Line equality fail at line {i+1}!");
            }
        }

        [TestMethod]
        public void ManiaParseTest() {
            var path = Path.Join("Resources", "ManiaTestMap.osu");
            var lines = File.ReadAllText(path);
            var decoder = new OsuBeatmapDecoder();
            var encoder = new OsuBeatmapEncoder();

            TestUnchanging(lines, decoder, encoder);
        }
    }
}