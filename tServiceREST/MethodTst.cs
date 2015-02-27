using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MyService;
using tServiceREST;

namespace Tests
{
    

    [TestFixture]
    public class TestMyService
    {
        private DhBmObj inData;

        public TestMyService()
        {
            Point [] pointsDh = new [] { new Point(){x = 1,y=2,z=3, cr=1, info="test"}
                                        ,new Point(){x = 2,y=3,z=4, cr=2, info="test"}
                                        ,new Point(){x = 3,y=4,z=5, cr=3, info="test"}
            };
            Point[] pointsBm = new[] {   new Point(){x = 2,y=3,z=4}
                                        ,new Point(){x = 3,y=4,z=5}
                                        ,new Point(){x = 5,y=5,z=4}
            };
            DhObj inDh = new DhObj() { points=pointsDh };
            BmObj inBm = new BmObj() { points=pointsBm, xAxis=20, yAxis=10, zAxis=10, xElPos=1, yElPos=4, zElPos=4};
            inData = new DhBmObj() { dh=inDh, bm=inBm };

        }

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
