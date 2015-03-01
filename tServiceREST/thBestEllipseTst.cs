using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MyService;
using tServiceREST;
using System.IO;

namespace Tests
{
    

    [TestFixture]
    public class TestTheBestEllipseOfPoint
    {
        private DhBmObj inData;
        private Counter counter;

        public TestTheBestEllipseOfPoint()
        {
            Point [] pointsDh = new [] { new Point(){x = 1,y=2,z=3, cr=1, info="test"}
                                        ,new Point(){x = 2,y=3,z=4, cr=2, info="test"}
                                        ,new Point(){x = 3,y=4,z=5, cr=3, info="test"}
            };
            Point[] pointsBm = new[] {   new Point(){x = 2,y=3,z=4}
                                        ,new Point(){x = 3,y=4,z=5}
                                        ,new Point(){x = 5,y=8,z=4}
                                        ,new Point(){x = 3,y=2,z=5}
                                        ,new Point(){x = 8,y=7,z=4}
                                        ,new Point(){x = 7,y=5,z=3}
                                        ,new Point(){x = 0,y=0,z=1}
            };
            DhObj inDh = new DhObj() { points=pointsDh };
            BmObj inBm = new BmObj() { points=pointsBm, xAxis=20, yAxis=10, zAxis=10, xElPos=1, yElPos=4, zElPos=4};
            inData = new DhBmObj() { dh=inDh, bm=inBm };

            counter = new Counter(2);
        }

        [Test]
       //  [ExpectedException(typeof(IOException))]
        public void returnedEllipsesCountEqualBmCellsCount()
        {
            TheBestEllipseOfPoint target = new TheBestEllipseOfPoint(inData.dh.points
                                                                    , inData.bm.points
                                                                    , inData.bm.xAxis
                                                                    , inData.bm.yAxis
                                                                    , inData.bm.zAxis
                                                                    , inData.bm.xElPos
                                                                    , inData.bm.yElPos
                                                                    , inData.bm.zElPos
                                                                    , counter);
            target.ComputeTheBestEllipses();
            Ellipse[] ellipses;
            ellipses = target.getTheBestEllipses();
            for (int i = 0; i < ellipses.Count(); i++)
            {

                Assert.IsInstanceOf<Ellipse>(ellipses[i]);
            }
            Assert.AreEqual(inData.bm.points.Count(), ellipses.Count());
            
        }

    }

    [TestFixture]
    public class TestPoint
    {
        private Point[] points;

        public TestPoint()
        {
            points = new[] {  new Point(){x = 1,y=2,z=3, cr=1, info="test"}
                             ,new Point(){x = 2,y=3,z=4, cr=2, info="test"}
                             ,new Point(){x = 3,y=4,z=5, cr=3, info="test"}
            };
        }
       
        [Test]
        public void trueRotateAndReverseRotateOfPoint()
        {
            double degX = 10;
            double degY = 20 ;
            double degZ = 30;
            double radX = degX * Math.PI / 180;
            double radY = degY * Math.PI / 180;
            double radZ = degZ * Math.PI / 180;

            Point[] pointsBuffer = new Point[points.Count()];
            for (int i = 0; i < points.Count(); i++)
            {
                pointsBuffer[i] = points[i].getRotatedPoint(radX, radY, radZ);
            }

            for (int i = 0; i < pointsBuffer.Count(); i++)
            {
                Assert.AreEqual (points[i].x, pointsBuffer[i].getReverseRotatedPoint(radX,radY,radZ).x);
                Assert.AreEqual (points[i].y, pointsBuffer[i].getReverseRotatedPoint(radX, radY, radZ).y);
                Assert.AreEqual (points[i].z, pointsBuffer[i].getReverseRotatedPoint(radX, radY, radZ).z);
            }

        }

    }

    [TestFixture]
    public class TestCounter
    {
        private Counter counter_;
        private int NUM_ITEMS = 3071;
        int[] threat;
        public TestCounter()
        {
            counter_ = new Counter(NUM_ITEMS);
            threat = new int[4];
            threat[0] = 911;
            threat[1] = 911;
            threat[2] = 911;
            threat[3] = 338;

        }

        [Test]
        public void trueCounterWork ()
        {
            double t=0;
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    t = t + threat[i] / 10.0;
                    counter_.increaseCount(threat[i] / 10.0 );
                }
            }
            Assert.AreEqual(NUM_ITEMS, counter_.getCount());
            Assert.AreEqual(100, counter_.getPercent());
        }

    }
}
