﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyService;
using log4net;
using log4net.Config;
using System.ComponentModel;

namespace tServiceREST
{

    class TheBestEllipseOfPoint
    {
        public int idxThreat { get; set; }
        public static readonly ILog log = LogManager.GetLogger(typeof(Ellipse));
        public double axisEllipseA { get; set; }
        public double axisEllipseB { get; set; }
        public double axisEllipseC { get; set; }
        public int minVariancePoints { get; set; }

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
        private Counter         counter_;
        private int             increment_;




        public TheBestEllipseOfPoint(Point[] dhPoints
                                    , Point[] bmPoints
                                    , int xAxis
                                    , int yAxis
                                    , int zAxis
                                    , int nElpsX
                                    , int nElpsY
                                    , int nElpsZ
                                    ,Counter counter)
        {

            axisEllipseA = xAxis;
            axisEllipseB = yAxis;
            axisEllipseC = zAxis;
            minVariancePoints = 4;
            counter_ = counter;
            increment_ = bmPoints.Count() / 6;

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

            rotatedPointsOfDhPoints_ = new Point [dhPoints_.Count()][][][];  //массив поворотов точек скважин
            for (int i = 0; i < rotatedPointsOfDhPoints_.Count();i++ )
            {
                rotatedPointsOfDhPoints_[i] = new Point [nElipsX_][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < nElipsX_; ix++ )
                {
                    rotatedPointsOfDhPoints_[i][ix] = new Point [nElipsY_][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < nElipsY_; iy++ )
                    {
                        rotatedPointsOfDhPoints_[i][ix][iy] = new Point [nElipsZ_]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                         for (int iz = 0; iz < nElipsZ_; iz++ )
                         {
                             rotatedPointsOfDhPoints_ [i][ix][iy][iz] = dhPoints[i].getRotatedPoint(   
                                                                                                    angleX_ * ix,
                                                                                                    angleY_ * iy,
                                                                                                    angleZ_ * iz
                                                                                                );
                         }
                    }
                }
            }
            log.Debug ("Формирование массива повернутых ячеек блочной модели");
            rotatedPointsOfBmPoints_ = new Point[bmPoints_.Count()][][][];  //массив поворотов точек скважин
            for (int i = 0; i < rotatedPointsOfBmPoints_.Count(); i++)
            {
                rotatedPointsOfBmPoints_[i] = new Point [nElipsX_][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < nElipsX_; ix++)
                {
                    rotatedPointsOfBmPoints_[i][ix] = new Point[nElipsY_][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < nElipsY_; iy++)
                    {
                        rotatedPointsOfBmPoints_[i][ix][iy] = new Point[nElipsZ_]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                        for (int iz = 0; iz < nElipsZ_; iz++)
                        {
                            rotatedPointsOfBmPoints_[i][ix][iy][iz] = bmPoints[i].getRotatedPoint(
                                                                                       angleX_ * ix,
                                                                                       angleY_ * iy,
                                                                                       angleZ_ * iz
                                                                                   );
                        }
                    }
                }
            }


        }

        public void ComputeTheBestEllipses()
        {
            int count = bmPoints_.Count();
            for (int i = 0; i < count; i++)
            {
                   theBestEllipses_[i] = computeTheBestEllipseForPoint(i);

                   // Informing user about execution progress 
                   if (((i + 1) % increment_ == 0) || (i + 1) == count) 
                   {
                       if ((i + 1) == count)
                       {
                           counter_.increaseCount(count % increment_);
                       }
                       else
                       {
                           counter_.increaseCount(increment_);
                       }
                       if (Environment.UserInteractive)
                       {
                           try
                           {
                               Console.SetCursorPosition(0, Console.CursorTop);
                           }
                           catch
                           {
                               Console.WriteLine();
                           }
                           Console.Write("Выполнение: " + counter_.getPercent().ToString() + " %");
                       }

                   }
                   

            }

        }

        private Ellipse computeTheBestEllipseForPoint (int pIndex)
        {
            log.DebugFormat("Начали оценку эллипсов вокруг ячейки {0}",pIndex);
            log.Debug("------------------------------------");
            Point[][][] rtdPntsSet=rotatedPointsOfBmPoints_[pIndex];
            Ellipse eRes = new Ellipse(bmPoints_[pIndex], axisEllipseA, axisEllipseB, axisEllipseC);
            double? curCriterrion = 99999;
            double? theBestCriterion = 99999;
            double? theBestAverage = 99999;
            int noRotationX = 0;
            int noRotationY = 0;
            int noRotationZ = 0;
            double LIMIT_OF_ELLIPSE_EXPAND = 1.6;
            double STEP_OF_ELLIPSE_EXPAND = 0.2;
            for (double multAxises = 1; multAxises < LIMIT_OF_ELLIPSE_EXPAND && theBestCriterion == 99999; multAxises = multAxises + STEP_OF_ELLIPSE_EXPAND)
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
                                theBestAverage = e.average;
                                noRotationX = ix;
                                noRotationY = iy;
                                noRotationZ = iz;
                            }

                        }
                    }
                }
            }
           log.DebugFormat(" Лучший поворот эллипса {0}-{1}-{2},  критерий оценки {3}", noRotationX, noRotationY, noRotationZ, theBestCriterion);
           eRes.average = theBestAverage;
           eRes.criterion = theBestCriterion;
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
            Point reversedXVektor = getXVector().getReverseRotatedPoint(radX, radY, radZ);
            Point azimuth = getAzimuth();
            Point XYProjectionOfReversedXVektor = getXYProjection(reversedXVektor, azimuth);

            if (isNullVector(XYProjectionOfReversedXVektor))
            {
                XYProjectionOfReversedXVektor.y = 10;
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
            Point reversedXVektor = getXVector().getReverseRotatedPoint( radX, radY, radZ);
            Point azimuth = getAzimuth();
            Point XYProjectionOfReversedXVektor = getXYProjection(reversedXVektor, azimuth);

            if (isNullVector(XYProjectionOfReversedXVektor))
            {
                XYProjectionOfReversedXVektor.y = 10;
            }

            double pseudoScalarX = reversedXVektor.y * XYProjectionOfReversedXVektor.z -  reversedXVektor.z * XYProjectionOfReversedXVektor.y;
            double pseudoScalarY = reversedXVektor.z * XYProjectionOfReversedXVektor.x - reversedXVektor.x * XYProjectionOfReversedXVektor.z;
            double pseudoScalarZ = reversedXVektor.x * XYProjectionOfReversedXVektor.y - reversedXVektor.y * XYProjectionOfReversedXVektor.x;

            double pseudoScalar0 = Math.Sqrt(Math.Pow(pseudoScalarX, 2) + Math.Pow(pseudoScalarY, 2) + Math.Pow(pseudoScalarZ, 2)); ;
            double scalarVektor1 = Math.Sqrt(Math.Pow(XYProjectionOfReversedXVektor.x, 2) + Math.Pow(XYProjectionOfReversedXVektor.y, 2) + Math.Pow(XYProjectionOfReversedXVektor.z, 2));
            double scalarVektor2 = Math.Sqrt(Math.Pow(reversedXVektor.x, 2) + Math.Pow(reversedXVektor.y, 2) + Math.Pow(reversedXVektor.z, 2));

            double radAngleAz = Math.Asin(pseudoScalar0 / (scalarVektor1 * scalarVektor2));
            double degreeAngleAz = radAngleAz * 180 / Math.PI;
            if (degreeAngleAz < 0)
            {
                degreeAngleAz = 360 + degreeAngleAz;
            }

            return degreeAngleAz;
        }


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

        public Ellipse[] getTheBestEllipses()
        {
            return theBestEllipses_;
        }

    }

    public class Counter
    {
        private double count_;
        private int totalAllThreats_;
        private object locker = new object();


        public Counter(int totalAllThreats)
        {
            totalAllThreats_ = totalAllThreats;
            count_ = 0;
        }
        public int getCount()
        {
            return (int) Math.Round(count_,0);
        }
        public int getPercent()
        {
            if (getCount() > totalAllThreats_)
            {
                return 100;
            }
            else
            {
                return (int)(100.0 / totalAllThreats_ * getCount());
            }
        }
        public void setCount(double count)
        {
            lock (locker)
            {
                count_ = count;
            }
        }
        public void increaseCount(double count)
        {
            lock (locker)
            {
                count_ += count;
            }
        }
    }

    public class Point
    {
        public double x;
        public double y;
        public double z;
        public double cr;
        public string info;


        public Point(Point point)
        {
            this.cr = point.cr;
            this.info = point.info;
            this.x = point.x;
            this.y = point.y;
            this.z = point.z;
        }
        public Point()
        {
            cr = 0;
            x = 0;
            y = 0;
            z = 0;
        }

        public Point getRotatedPoint(double radX, double radY, double radZ)
        {
            Point bufferPoint = new Point(this);
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

            resultPoint.x = Math.Round(resultPoint.x, 6);
            resultPoint.y = Math.Round(resultPoint.y, 6);
            resultPoint.z = Math.Round(resultPoint.z, 6);

            return resultPoint;
        }

        public Point getReverseRotatedPoint(double radX, double radY, double radZ)
        {
            Point bufferPoint = new Point(this);
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

            resultPoint.x = Math.Round(resultPoint.x, 6);
            resultPoint.y = Math.Round(resultPoint.y, 6);
            resultPoint.z = Math.Round(resultPoint.z, 6);

            return resultPoint;
        }
    }
}
