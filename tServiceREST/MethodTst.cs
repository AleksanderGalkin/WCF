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
        public void returnedEllipsesCountEqualBmCellsCount()
        {
            MyService.MyService target = new MyService.MyService();
            Ellipse[] ellipses;
            ellipses = target.startCalculation(inData);
            Assert.AreEqual(inData.bm.points.Count(), ellipses.Count());
        }

 

    }

    [TestFixture]
    public class TestBmObj
    {
        private BmObj inData;

        public TestBmObj ()
        {
            Point[] pointsBm = new[] {   new Point(){x = 2,y=3,z=4}
                                        ,new Point(){x = 3,y=4,z=5}
                                        ,new Point(){x = 5,y=8,z=4}
                                        ,new Point(){x = 3,y=2,z=5}
                                        ,new Point(){x = 8,y=7,z=4}
                                        ,new Point(){x = 7,y=5,z=3}
                                        ,new Point(){x = 0,y=0,z=1}
                                        ,new Point(){x = 5,y=6,z=1}
                                        ,new Point(){x = 0,y=3,z=1}
            };
            inData = new BmObj() { points=pointsBm, xAxis=20, yAxis=10, zAxis=10, xElPos=1, yElPos=4, zElPos=4};
        }
        [Test]
        public void truePartionOfPoints()
        {
            int countOfParts = 4;

            BmObj[] bmObjParts = new BmObj[countOfParts];
            for (int i = 0; i < countOfParts; i++)
            {
                bmObjParts[i] = new BmObj(inData, countOfParts, i + 1);
            }
            Assert.AreEqual(3, bmObjParts[0].points.Count());
            Assert.AreEqual(2, bmObjParts[1].points.Count());
            Assert.AreEqual(2, bmObjParts[2].points.Count());
            Assert.AreEqual(2, bmObjParts[3].points.Count());


        }
    }
    [TestFixture]
    public class TestEllipse
    {
        Point centreOfEllipse;
        Point[] pointsBm;
        int axisEllipseA_ ;
        int axisEllipseB_ ;
        int axisEllipseC_ ;
        public TestEllipse()
        {
            centreOfEllipse = new Point() { x = 2, y = 2, z = 3 };
            pointsBm = new[] {   
                                         new Point(){x = 2,y = 2,z = 5      ,cr = 1,info = "test"} // in
                                        ,new Point(){x = 30,y = 40,z = 50   ,cr = 2,info = "test"} //out
                                        ,new Point(){x = 12,y = 5,z = 8     ,cr = 3,info = "test"} //in
                                        ,new Point(){x = 22,y = 5,z = 3     ,cr = 4,info = "test"} // out
            }; 
            axisEllipseA_ = 20;
            axisEllipseB_ = 15;
            axisEllipseC_ = 10;
  
        }
        [Test]
        public void trueVarianceCalculationOfEllipse()
        {
            Ellipse target = new Ellipse(centreOfEllipse, axisEllipseA_, axisEllipseB_, axisEllipseC_);
            target.setCriterion(pointsBm, Ellipse.Method.Variance);
            Assert.AreEqual(1,target.getCriterion());
        }
    }
}
