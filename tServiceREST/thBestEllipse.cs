using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyService;

namespace tServiceREST
{
    class Ellipse
    {
        public enum Method { Variance, Median }
        private Point       point_ { get; set; }
        private double      trDipDir_ { get; set; }
        private double      trDip_ { get; set; }
        private double?     criterion_;
        private int         axisEllipseA_ { get; set; }
        private int         axisEllipseB_ { get; set; }
        private int         axisEllipseC_ { get; set; }

        public Ellipse(Point point, int axisEllipseA, int axisEllipseB, int axisEllipseC)
        {
            point_ = point;
            axisEllipseA_ = axisEllipseA;
            axisEllipseB_ = axisEllipseB;
            axisEllipseC_ = axisEllipseC;
            criterion_ = null;
        }

        public int setCriterion ( Point[] arrayOfPoints, Method method)
        {
            double sumSquareOfIndication = 0;
            double sumOfIndication = 0;
            int nIndication = 0;
            for (int i = 0; i < arrayOfPoints.Count(); i++)
            {
                if (isPointOfEllipse(arrayOfPoints[i]))
                {
                    sumSquareOfIndication = + Math.Pow (arrayOfPoints[i].cr, 2);
                    sumOfIndication = + arrayOfPoints[i].cr;
                    nIndication++;
                }

            }
            criterion_=(sumSquareOfIndication / nIndication) - (Math.Pow(sumOfIndication, 2) / nIndication); // For a while Variance only
            return 0;
        }

        public double? getCriterion()
        {
            return criterion_;
        }

        bool isPointOfEllipse(Point point)
        {

            double radicalExpression = 1 - (Math.Pow(point.x - point_.x, 2) / Math.Pow(axisEllipseA_, 2))
                                            - (Math.Pow(point.z - point_.z, 2) / Math.Pow(axisEllipseC_, 2));
            if (radicalExpression < 0)
            {
                return false;
            }
            double topFunctionOfXY = point_.y + axisEllipseB_ * Math.Sqrt(radicalExpression);
            double bottomFunctionOfXY = point_.y - axisEllipseB_ * Math.Sqrt(radicalExpression);
            if (point.y >=  bottomFunctionOfXY && point.y <= topFunctionOfXY)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
    
    class TheBestEllipseOfPoint
    {
        private Point[]         points_;
        private Point[][][][]   rotatedPointsOfPoints_;
        private Ellipse[]       theBestEllipses_;
        private double          angleX_;
        private int             nElipsX_;
        private double          angleY_;
        private int             nElipsY_;
        private double          angleZ_;
        private int             nElipsZ_;

        public int              axisEllipseA { get; set; }
        public int              axisEllipseB { get; set; }
        public int              axisEllipseC { get; set; }


        public TheBestEllipseOfPoint(Point[] points,int nElpsX, int nElpsY, int nElpsZ)
        {

            axisEllipseA = 15;
            axisEllipseB = 10;
            axisEllipseC = 10;

            points_  = points;
            nElipsX_ = nElpsX;
            nElipsY_ = nElpsY;
            nElipsZ_ = nElpsZ;
            angleX_  = 6.2831853072 / nElipsX_;
            angleY_  = 6.2831853072 / nElipsY_;
            angleZ_  = 6.2831853072 / nElipsZ_;
            int numElipsCompination = nElipsX_ * nElipsY_ * nElipsZ_;
            rotatedPointsOfPoints_ = new Point[points_.Count()][][][];  //массив поворотов точек
            for (int i = 0; i < rotatedPointsOfPoints_.Count();i++ )
            {
                rotatedPointsOfPoints_[i] = new Point[nElipsX_][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < nElipsX_; ix++ )
                {
                    rotatedPointsOfPoints_[i][ix] = new Point[nElipsY_][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < nElipsY_; iy++ )
                    {
                        rotatedPointsOfPoints_[i][ix][iy] = new Point[nElipsZ_]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                         for (int iz = 0; iz < nElipsZ_; iz++ )
                         {
                             rotatedPointsOfPoints_[i][ix][iy][iz] = getRotatedPoint(   points[i],
                                                                                        angleX_ * ix,
                                                                                        angleY_ * iy,
                                                                                        angleZ_ * iz
                                                                                    );
                         }
                    }
                }
            }

            for (int i = 0; i < points_.Count(); i++)
            {
                theBestEllipses_[i] = computeTheBestEllipseForPoint(i);
            }


        }
        private Point getRotatedPoint(Point pPnt, double radX, double radY, double radZ)
        {

            Point p=new Point();
            p.cr = pPnt.cr;

            p.x = pPnt.x;
            p.y = pPnt.y * Math.Cos(radX) + pPnt.z * Math.Sin(radX);
            p.z = -pPnt.y * Math.Sin(radX) + pPnt.z * Math.Cos(radX);

            p.y = pPnt.y;
            p.z = pPnt.z * Math.Cos(radY) + pPnt.x * Math.Sin(radY);
            p.x = -pPnt.z * Math.Sin(radY) + pPnt.x * Math.Cos(radY);

            p.z = pPnt.z;
            p.x = pPnt.x * Math.Cos(radZ) + pPnt.y * Math.Sin(radZ);
            p.y = -pPnt.x * Math.Sin(radZ) + pPnt.y * Math.Cos(radZ);

            return p;
        }

        private Ellipse computeTheBestEllipseForPoint (int pIndex)
        {
            Point[][][] rtdPntsSet=rotatedPointsOfPoints_[pIndex];
            Ellipse eRes = new Ellipse(new Point(), 1, 1, 1);
            double? curCriterrion = 99999;
            double? theBestCriterion = 0;
            for (int ix = 0; ix < rtdPntsSet.Count(); ix++)
            {
                for (int iy = 0; iy < rtdPntsSet[ix].Count(); iy++)
                {
                    for (int iz = 0; iz < rtdPntsSet[ix][iy].Count(); iz++)
                    {
                        Ellipse e = new Ellipse(points_[pIndex], axisEllipseA, axisEllipseB, axisEllipseC);
                        Point[] oneRotationPoints=new Point[rotatedPointsOfPoints_.Count()];
                        for (int i = 0; i<rotatedPointsOfPoints_.Count(); i++)
                        {
                            oneRotationPoints[i]=rotatedPointsOfPoints_[i][ix][iy][iz];
                        }
                        e.setCriterion(oneRotationPoints, Ellipse.Method.Variance);

                        curCriterrion=e.getCriterion();
                        if (curCriterrion < theBestCriterion)
                        {
                            theBestCriterion = curCriterrion;
                            e = getEllipseByRotation (ix, iy, iz);
                        }
                    }
                }
            }
            return eRes;
        }

        private double getEllipseCriterion (Point pnt)
        {
            for (int i = 0; i < points_.Count(); i++)
            {
                for (int ix = 0; ix < rotatedPointsOfPoints_[i].Count(); ix++)
                {
                    for (int iy = 0; iy < rotatedPointsOfPoints_[i][ix].Count(); iy++)
                    {
                        for (int iz = 0; iz < rotatedPointsOfPoints_[i][ix][iy].Count(); iz++)
                        {
                            rotatedPointsOfPoints_[i][ix][iy][iz] = rotatedPointsOfPoints_[i][ix][iy][iz];
                        }
                    }
                }
            }
                return 10;
        }
    

        private Ellipse getEllipseByRotation (int ix, int iy, int iz) 
        {
            Ellipse e = new Ellipse(new Point(),1,1,1);
            return e;
        }
    }

    
}
