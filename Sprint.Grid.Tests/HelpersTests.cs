using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sprint.Grid.Tests
{
    [TestClass]
    public class HelperTests
    {       
        [TestMethod]
        public void ToNullable()
        {
            var p1 = 1;
            var p2 = 2.2;
            var p3 = 3.3m;

            Assert.AreEqual(p1, Helpers.Helpers.ToNullable<int>(p1));

            Assert.AreEqual(p2, Helpers.Helpers.ToNullable<double>(p2));

            Assert.AreEqual(p3, Helpers.Helpers.ToNullable<decimal>(p3));

            Assert.AreNotEqual(15f, Helpers.Helpers.ToNullable<decimal>(15));

            Assert.AreEqual(null, Helpers.Helpers.ToNullable<decimal>(null));
        }
    }
}
