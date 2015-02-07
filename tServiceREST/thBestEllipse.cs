using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyService;

namespace tServiceREST
{

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

            theBestEllipses_ = new Ellipse[points.Count()];

            points_  = points;
            nElipsX_ = nElpsX;
            nElipsY_ = nElpsY;
            nElipsZ_ = nElpsZ;
            angleX_  = 6.2831853072 / (nElipsX_*2);
            angleY_  = 6.2831853072 / (nElipsY_*2);
            angleZ_  = 6.2831853072 / (nElipsZ_*2);
            int numElipsCombination = nElipsX_ * nElipsY_ * nElipsZ_;

            Console.WriteLine("Формирование массива повернутых точек");

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



        }
        private Point getRotatedPoint(Point pPnt, double radX, double radY, double radZ)
        {
            Point bufferPoint = new Point(pPnt);
            Point resultPoint = new Point();
            resultPoint.cr = bufferPoint.cr;

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

        public int setTheBestEllipses()
        {
            Console.WriteLine("Ищем лучший эллипс для точек:");
            for (int i=0; i < points_.Count(); i++)
            {
                   theBestEllipses_[i] = computeTheBestEllipseForPoint(i);
                if (i%100==0)
                    Console.WriteLine("Точка: "+ i.ToString()+" Найдено.");
            }

            return 0;
        }

        private Ellipse computeTheBestEllipseForPoint (int pIndex)
        {
            Point[][][] rtdPntsSet=rotatedPointsOfPoints_[pIndex];
            Ellipse eRes = new Ellipse(points_[pIndex], axisEllipseA, axisEllipseB, axisEllipseC);
            double? curCriterrion = 99999;
            double? theBestCriterion = 99999;
            int noRotationX = 0;
            int noRotationY = 0;
            int noRotationZ = 0;
            for (int ix = 0; ix < rtdPntsSet.Count(); ix++)
            {
                for (int iy = 0; iy < rtdPntsSet[ix].Count(); iy++)
                {
                    for (int iz = 0; iz < rtdPntsSet[ix][iy].Count(); iz++)
                    {
                        Ellipse e = new Ellipse(rtdPntsSet[ix][iy][iz], axisEllipseA, axisEllipseB, axisEllipseC);
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
                            noRotationX = ix;
                            noRotationY = iy;
                            noRotationZ = iz;
                        }
                    }
                }
            }
            eRes.trDipDir = getTrueDipDirection(points_[pIndex],
                                                rotatedPointsOfPoints_[pIndex][noRotationX][noRotationY][noRotationZ],
                                                angleX_ * noRotationX,
                                                angleY_ * noRotationY
                                                );

            
            eRes.trDip = getTrueDip(points_[pIndex],
                                    rotatedPointsOfPoints_[pIndex][noRotationX][noRotationY][noRotationZ],
                                    angleX_ * noRotationX,
                                    angleZ_ * noRotationZ
                                    );
            if (double.IsNaN(eRes.trDipDir))  // решить потом эту проблемму
                eRes.trDipDir = 360;
            if (double.IsNaN(eRes.trDip))
                eRes.trDip = 360;

            return eRes;
        }


        double getTrueDipDirection(Point truePoint, Point fullRotatedPoint, double radX, double radY)
        {
            Point oneRotatedPoint = getReverseRotatedPoint(fullRotatedPoint, radX, radY, 0);
            double scalarProduct = truePoint.x * oneRotatedPoint.x + truePoint.y * oneRotatedPoint.y + truePoint.z * oneRotatedPoint.z;
            double scalarVektor1 = Math.Sqrt(Math.Pow(      truePoint.x, 2)+Math.Pow(        truePoint.y, 2)+Math.Pow(        truePoint.z, 2));
            double scalarVektor2 = Math.Sqrt(Math.Pow(oneRotatedPoint.x, 2) + Math.Pow(oneRotatedPoint.y, 2) + Math.Pow(oneRotatedPoint.z, 2));
            double radAngle = Math.Acos(scalarProduct / (scalarVektor1 * scalarVektor2));
            double degreeAngle = radAngle * 180 / Math.PI;

            return degreeAngle+90;
        }

        double getTrueDip(Point truePoint, Point fullRotatedPoint, double radX, double radZ)
        {
            Point oneRotatedPoint = getReverseRotatedPoint(fullRotatedPoint, radX, 0, radZ);
            double scalarProduct = truePoint.x * oneRotatedPoint.x + truePoint.y * oneRotatedPoint.y + truePoint.z * oneRotatedPoint.z;
            double scalarVektor1 = Math.Sqrt(Math.Pow(truePoint.x, 2) + Math.Pow(truePoint.y, 2) + Math.Pow(truePoint.z, 2));
            double scalarVektor2 = Math.Sqrt(Math.Pow(oneRotatedPoint.x, 2) + Math.Pow(oneRotatedPoint.y, 2) + Math.Pow(oneRotatedPoint.z, 2));
            double radAngle = Math.Acos(scalarProduct / (scalarVektor1 * scalarVektor2));
            double degreeAngle = radAngle * 180 / Math.PI;

            return degreeAngle;
        }

        public Ellipse[] getTheBestEllipses()
        {
            return theBestEllipses_;
        }

    }

    
}
