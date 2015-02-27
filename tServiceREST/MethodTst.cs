using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MyService;

namespace Tests
{

    [TestFixture]
    public class TestMyService
    {
        [Test]
        public void Test()
        {
            Assert.AreEqual(2, 1 + 1);
        }
        [Test]
        public void Test2()
        {
            Assert.AreNotEqual(2, 1 + 1);
        }
    }
}
