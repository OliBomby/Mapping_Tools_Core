﻿using Mapping_Tools_Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Mapping_Tools_Core_Tests {
    [TestClass]
    public class InputParsersTests {
        [TestMethod]
        public void ParseDoubleTest() {
            double actual = InputParsers.ParseDouble("1 + 1");
            const double expected = 2;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TimestampParserTest() {
            var test1 = InputParsers.ParseOsuTimestamp("00:00:891 (1) - ");
            Assert.AreEqual(891, test1.TotalMilliseconds);

            var test2 = InputParsers.ParseOsuTimestamp("60:00:074 (2,4) - ");
            Assert.AreEqual(3600074, test2.TotalMilliseconds);

            var test3 = InputParsers.ParseOsuTimestamp("60:00:074 - ");
            Assert.AreEqual(3600074, test3.TotalMilliseconds);

            var test4 = InputParsers.ParseOsuTimestamp("00:-01:-230 (1) - ");
            Assert.AreEqual(-1230, test4.TotalMilliseconds);
        }
    }
}