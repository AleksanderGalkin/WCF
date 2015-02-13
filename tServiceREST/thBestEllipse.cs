using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyService;
using log4net;
using log4net.Config;

namespace tServiceREST
{

    class TheBestEllipseOfPoint
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Ellipse));
        private Point[]         dhPoints_;
        private Point[]         bmPoints_;
        private Point[][][][]   rotatedPointsOfDhPoints_;
        private Point[][][][]   rotatedPointsOfBmPoints_;
        private Ellipse[]       theBestEllipses_;
        private double          angleX_;
        private int             nElipsX_;
        private double          angleY_;
        private int             nElipsY_;
        private double          angleZ_;
        private int             nElipsZ_;

        public double              axisEllipseA { get; set; }
        public double              axisEllipseB { get; set; }
        public double              axisEllipseC { get; set; }
        public int                 minVariancePoints { get; set; }


        public TheBestEllipseOfPoint(Point[] dhPoints, Point[] bmPoints, int xAxis, int yAxis, int zAxis, int nElpsX, int nElpsY, int nElpsZ)
        {

            axisEllipseA = xAxis;
            axisEllipseB = yAxis;
            axisEllipseC = zAxis;
            minVariancePoints = 4;

            theBestEllipses_ = new Ellipse[bmPoints.Count()];

            dhPoints_  = dhPoints;
            bmPoints_  = bmPoints;
            nElipsX_ = nElpsX;
            nElipsY_ = nElpsY;
            nElipsZ_ = nElpsZ;
            angleX_  = 6.2831853072 / (nElipsX_*2);
            angleY_  = 6.2831853072 / (nElipsY_*2);
            angleZ_  = 6.2831853072 / (nElipsZ_*2);
            int numElipsCombination = nElipsX_ * nElipsY_ * nElipsZ_;

            log.Debug("Формирование массива повернутых скважин");

            rotatedPointsOfDhPoints_ = new Point[dhPoints_.Count()][][][];  //массив поворотов точек скважин
            for (int i = 0; i < rotatedPointsOfDhPoints_.Count();i++ )
            {
                rotatedPointsOfDhPoints_[i] = new Point[nElipsX_][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < nElipsX_; ix++ )
                {
                    rotatedPointsOfDhPoints_[i][ix] = new Point[nElipsY_][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < nElipsY_; iy++ )
                    {
                        rotatedPointsOfDhPoints_[i][ix][iy] = new Point[nElipsZ_]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                         for (int iz = 0; iz < nElipsZ_; iz++ )
                         {
                             rotatedPointsOfDhPoints_[i][ix][iy][iz] = getRotatedPoint(   dhPoints[i],
                                                                                        angleX_ * ix,
                                                                                        angleY_ * iy,
                                                                                        angleZ_ * iz
                                                                                    );
                         }
                    }
                }
            }
            log.Debug("Формирование массива повернутых ячеек блочной модели");
            rotatedPointsOfBmPoints_ = new Point[bmPoints_.Count()][][][];  //массив поворотов точек скважин
            for (int i = 0; i < rotatedPointsOfBmPoints_.Count(); i++)
            {
                rotatedPointsOfBmPoints_[i] = new Point[nElipsX_][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < nElipsX_; ix++)
                {
                    rotatedPointsOfBmPoints_[i][ix] = new Point[nElipsY_][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < nElipsY_; iy++)
                    {
                        rotatedPointsOfBmPoints_[i][ix][iy] = new Point[nElipsZ_]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                        for (int iz = 0; iz < nElipsZ_; iz++)
                        {
                            rotatedPointsOfBmPoints_[i][ix][iy][iz] = getRotatedPoint(bmPoints[i],
                                                                                       angleX_ * ix,
                                                                                       angleY_ * iy,
                                                                                       angleZ_ * iz
                                                                                   );
                        }
                    }
                }
            }


        }
        private Point getRotatedPoint(Point pPnt, double radX, double radY, double radZ)
        {
            Point bufferPoint = new Point(pPnt);
            Point resultPoint = new Point();
            resultPoint.cr = bufferPoint.cr;
            resultPoint.info = bufferPoint.info;

            resultPoint.x = bufferPoint.x;
            resultPoint.y = bufferPoint.y * Math.Cos(radX) + bufferPoint.z * Math.Sin(radX);
            resultPoint.z = -bufferPoint.y * Math.Sin(radX) + bufferPoint.z * Math.Cos(radX);
            bufferPoint.x = resultPoint.x; bufferPoint.y = resultPoint.y; bufferPoint.z = resultPoint.z;

            resultPoint.y = bufferPoint.y;
            resultPoint.z = bufferPoint.z * Math.Cos(radY) + bufferPoint.x * Math.Sin(radY);
            resultPoint.x = -bufferPoint.z * Math.Sin(radY) + bufferPoint.x * Math.Cos(radY);
            bufferPoint.x = resultPoint.x; bufferPoint.y = resultPoint.y; bufferPoint.z = resultPoint.z;

            resultPoint.z = bufferPoint.z;
            resultPoint.x = bufferPoint.x * Math.Cos(radZ) + bufferPoint.y * Math.Sin(radZ);
            resultPoint.y = -bufferPoint.x * Math.Sin(radZ) + bufferPoint.y * Math.Cos(radZ);

            return resultPoint;
        }


        private Point getReverseRotatedPoint(Point pPnt, double radX, double radY, double radZ)
        {
            Point bufferPoint = new Point(pPnt);
            Point resultPoint = new Point();
            resultPoint.cr = bufferPoint.cr;
            resultPoint.info = bufferPoint.info;

            resultPoint.z = bufferPoint.z;
            resultPoint.x = bufferPoint.x * Math.Cos(radZ) - bufferPoint.y * Math.Sin(radZ);
            resultPoint.y = bufferPoint.x * Math.Sin(radZ) + bufferPoint.y * Math.Cos(radZ);
            bufferPoint.x = resultPoint.x; bufferPoint.y = resultPoint.y; bufferPoint.z = resultPoint.z;

            resultPoint.y = bufferPoint.y;
            resultPoint.z = bufferPoint.z * Math.Cos(radY) - bufferPoint.x * Math.Sin(radY);
            resultPoint.x = bufferPoint.z * Math.Sin(radY) + bufferPoint.x * Math.Cos(radY);
            bufferPoint.x = resultPoint.x; bufferPoint.y = resultPoint.y; bufferPoint.z = resultPoint.z;

            resultPoint.x = bufferPoint.x;
            resultPoint.y = bufferPoint.y * Math.Cos(radX) - bufferPoint.z * Math.Sin(radX);
            resultPoint.z = bufferPoint.y * Math.Sin(radX) + bufferPoint.z * Math.Cos(radX);
            

            return resultPoint;
        }

        public int ComputeTheBestEllipses()
        {
            for (int i=0; i < bmPoints_.Count(); i++)
            {
                   theBestEllipses_[i] = computeTheBestEllipseForPoint(i);
                if (i%100==0)
                    Console.WriteLine("Точка: "+ i.ToString()+" Найдено.");
            }

            return 0;
        }

        private Ellipse computeTheBestEllipseForPoint (int pIndex)
        {
            log.DebugFormat("Начали оценку эллипсов вокруг ячейки {0}",pIndex);
            log.Debug("------------------------------------");
            Point[][][] rtdPntsSet=rotatedPointsOfBmPoints_[pIndex];
            Ellipse eRes = new Ellipse(bmPoints_[pIndex], axisEllipseA, axisEllipseB, axisEllipseC);
            double? curCriterrion = 99999;
            double? theBestCriterion = 99999;
            int noRotationX = 0;
            int noRotationY = 0;
            int noRotationZ = 0;
            for (double multAxises = 1; multAxises < 1.6 && theBestCriterion==99999; multAxises = multAxises + 0.2)
            {
                for (int ix = 0; ix < rtdPntsSet.Count(); ix++)
                {
                    for (int iy = 0; iy < rtdPntsSet[ix].Count(); iy++)
                    {
                        for (int iz = 0; iz < rtdPntsSet[ix][iy].Count(); iz++)
                        {
                            Ellipse e = new Ellipse(rtdPntsSet[ix][iy][iz],
                                                    axisEllipseA * multAxises,
                                                    axisEllipseB * multAxises,
                                                    axisEllipseC * multAxises);
                            Point[] oneRotationPoints = new Point[rotatedPointsOfDhPoints_.Count()];
                            for (int i = 0; i < rotatedPointsOfDhPoints_.Count(); i++)
                            {
                                oneRotationPoints[i] = rotatedPointsOfDhPoints_[i][ix][iy][iz];
                            }
                            e.setCriterion(oneRotationPoints, Ellipse.Method.Variance);
                            curCriterrion = e.getCriterion();

                            log.DebugFormat("Посчитан поворот {0}-{1}-{2} , критерий оценки {3}, кол-во точек Variance {4}", ix, iy, iz, curCriterrion,e.getNumberIndication());

                            bool isCurCriterionIsTheBest = curCriterrion < theBestCriterion;
                            bool isCurCriterionNumPointsMoreMinVPoints = e.getNumberIndication() >= minVariancePoints;
                            if (isCurCriterionIsTheBest && isCurCriterionNumPointsMoreMinVPoints)
                            {
                                theBestCriterion = curCriterrion;
                                noRotationX = ix;
                                noRotationY = iy;
                                noRotationZ = iz;
                            }

                        }
                    }
                }
            }
           log.DebugFormat(" Лучший поворот эллипса {0}-{1}-{2},  критерий оценки {3}", noRotationX, noRotationY, noRotationZ, theBestCriterion);
           eRes.trDipDir = getTrueDipDirection(angleX_ * noRotationX,
                                                angleY_ * noRotationY,
                                                angleZ_ * noRotationZ
                                                );

           eRes.trDip = getTrueDip(
                                    angleX_ * noRotationX,
                                    angleY_ * noRotationY,
                                    angleZ_ * noRotationZ
                                    );
           log.DebugFormat("Истинный поворот эллипса: угол направления простирания {0}, угол падения {1}", eRes.trDipDir, eRes.trDip);
           log.Debug("--------------------------------------");
           return eRes;
        }


        double getTrueDipDirection(double radX, double radY,double radZ)
        {


            Point reversedXVektor = getReverseRotatedPoint(getXVector(), radX, radY, radZ);
            Point azimuth = getAzimuth();
            Point XYProjectionOfReversedXVektor = getXYProjection(reversedXVektor, azimuth);

            if (isNullVector(XYProjectionOfReversedXVektor))
            {
                XYProjectionOfReversedXVektor.z = 10;
            }

            double pseudoScalarProduct = XYProjectionOfReversedXVektor.x * azimuth.y - azimuth.x * XYProjectionOfReversedXVektor.y;
            double scalarVektor1 = Math.Sqrt(Math.Pow(azimuth.x, 2) + Math.Pow(azimuth.y, 2) );
            double scalarVektor2 = Math.Sqrt(Math.Pow(XYProjectionOfReversedXVektor.x, 2) + Math.Pow(XYProjectionOfReversedXVektor.y, 2) );
            double radAngleAz = Math.Asin(pseudoScalarProduct / (scalarVektor1 * scalarVektor2));
            double degreeAngleAz = radAngleAz * 180 / Math.PI;
            if (degreeAngleAz < 0)
            {
                degreeAngleAz = 360 + degreeAngleAz;
            }


            return degreeAngleAz;
        }

        double getTrueDip( double radX, double radY, double radZ)
        {
            Point reversedXVektor = getReverseRotatedPoint(getXVector(), radX, radY, radZ);
            Point azimuth = getAzimuth();
            Point ZYProjectionOfReversedXVektor = getZYProjection(reversedXVektor, azimuth);

            if (isNullVector(ZYProjectionOfReversedXVektor))
            {
                ZYProjectionOfReversedXVektor.y = 10;
            }

            double pseudoScalarProduct = ZYProjectionOfReversedXVektor.y * azimuth.z - azimuth.y * ZYProjectionOfReversedXVektor.z;
            double scalarVektor1 = Math.Sqrt(Math.Pow(azimuth.y, 2) + Math.Pow(azimuth.z, 2) );
            double scalarVektor2 = Math.Sqrt(Math.Pow(ZYProjectionOfReversedXVektor.y, 2) + Math.Pow(ZYProjectionOfReversedXVektor.z, 2));
            double radAngleAz = Math.Asin(pseudoScalarProduct / (scalarVektor1 * scalarVektor2));
            double degreeAngleAz = radAngleAz * 180 / Math.PI;
            if (degreeAngleAz < 0)
            {
                degreeAngleAz = 360 + degreeAngleAz;
            }

            return degreeAngleAz;
        }

        //Point getPointOfAxisXOrientedVektor (Point point)
        //{
        //    Point resPoint = new Point (point);
        //    resPoint.x = resPoint.x + 10;
        //    return resPoint;
        //}

        Point getXYProjection (Point point, Point planePoint)
        {
            Point resPoint = new Point(point);
            resPoint.z = planePoint.z;
            return resPoint;
        }

        Point getZYProjection(Point point, Point planePoint)
        {
            Point resPoint = new Point(point);
            resPoint.x = planePoint.x;
            return resPoint;
        }

        Point getAzimuth ()
        {
            Point resPoint = new Point();
            resPoint.x = 0;
            resPoint.z = 0;
            resPoint.y = 10;
            return resPoint;
        }

        Point getXVector()
        {
            Point resPoint = new Point();
            resPoint.x = 10;
            resPoint.z = 0;
            resPoint.y = 0;
            return resPoint;
        }

        bool isNullVector(Point point)
        {
            if (point.x == 0 && point.y == 0 && point.z == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //Point getVektor(Point point1, Point point2)
        //{
        //    Point resPoint = new Point();
        //    resPoint.x = point2.x - point1.x;
        //    resPoint.y = point2.y - point1.y;
        //    resPoint.z = point2.z - point1.z;
        //    return resPoint;
        //}

        public Ellipse[] getTheBestEllipses()
        {
            return theBestEllipses_;
        }

    }

    
}
