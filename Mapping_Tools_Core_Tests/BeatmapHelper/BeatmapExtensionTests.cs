using Mapping_Tools_Core.BeatmapHelper;
using Mapping_Tools_Core.BeatmapHelper.IO.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Mapping_Tools_Core_Tests.BeatmapHelper {
    [TestClass]
    public class BeatmapExtensionTests {
        [TestMethod]
        public void QueryTimeCodeTest() {
            var path = Path.Join("Resources", "ComplicatedTestMap.osu");
            var beatmap = new BeatmapEditor(path).ReadFile();

            var hos = beatmap.QueryTimeCode("00:56:823 (1,2,1,2) - ").ToArray();
            Assert.AreEqual(0, hos.Length);


            hos = beatmap.QueryTimeCode("00:00:015 (1,2,3,4,5,1) - ").ToArray();
            Assert.AreEqual(6, hos.Length);
        }
    }
}