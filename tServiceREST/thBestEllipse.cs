﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyService;

namespace tServiceREST
{
    class Ellipse
    {
        public point pnt { get; set; }
        public double trDipDir{get;set;}
        public double trDip{get;set;}
        public double criterion{get;set;}
    }
    
    class theBestEllipseOfPoint
    {
        private point[] pnts;
        private point[][][][] rtdPntsOfPoints;
        private Ellipse[] theBestEllipses;
        private double angleX;
        private int numElipsX;
        private double angleY;
        private int numElipsY;
        private double angleZ;
        private int numElipsZ;

        public theBestEllipseOfPoint(point[] points,int numberElpsX, int numberElpsY, int numberElpsZ)
        {
            pnts = points;
            numElipsX = numberElpsX;
            numElipsY = numberElpsY;
            numElipsZ = numberElpsZ;
            angleX = 6.2831853072 / numElipsX;
            angleY = 6.2831853072 / numElipsY;
            angleZ = 6.2831853072 / numElipsZ;
            int numElipsCompination = numElipsX * numElipsY * numElipsZ;
            rtdPntsOfPoints = new point[pnts.Count()][][][];  //массив поворотов точек
            for (int i = 0; i < rtdPntsOfPoints.Count();i++ )
            {
                rtdPntsOfPoints[i] = new point[numElipsX][][]; // для каждой точки массив поворотов вокруг оси X 

                for (int ix = 0; ix < numElipsX;ix++ )
                {
                    rtdPntsOfPoints[i][ix] = new point[numElipsY][]; // для каждго поворота вокруг оси X создаём массив поворотов вокруг оси Y
                    for (int iy = 0; iy < numElipsY;iy++ )
                    {
                        rtdPntsOfPoints[i][ix][iy] = new point[numElipsZ]; // для каждго поворота вокруг оси Y создаём массив поворотов вокруг оси Z
                         for (int iz = 0; iz < numElipsZ;iz++ )
                         {
                             rtdPntsOfPoints[i][ix][iy][iz] = getRotatedPoint(points[i], angleX * ix, angleY * iy, angleZ * iz);
                         }
                    }
                }
            }

            for (int i = 0; i < pnts.Count(); i++)
            {
                theBestEllipses[i] = getTheBestEllipseForPoint(i);
            }


        }
        private point getRotatedPoint(point pPnt, double radX, double radY, double radZ)
        {

            point p=new point();
            p.cr = pPnt.cr;

            p.x = pPnt.x;
            p.y = pPnt.y * Math.Cos(radX) + pPnt.z * Math.Sin(radX);
            p.z = -pPnt.y * Math.Sin(radX) + pPnt.z * Math.Cos(radX);

            p.y = pPnt.y;
            p.z = pPnt.z * Math.Cos(radY) + pPnt.x * Math.Sin(radY);
            p.x = -pPnt.z * Math.Sin(radY) + pPnt.x * Math.Cos(radY);

            p.z = pPnt.z;
            p.x = pPnt.x * Math.Cos(radZ) + pPnt.y * Math.Sin(radZ);
            p.y = -pPnt.y * Math.Sin(radZ) + pPnt.y * Math.Cos(radZ);

            return p;
        }

        private Ellipse getTheBestEllipseForPoint(int pIndex)
        {
            point[][][] rtdPntsSet=rtdPntsOfPoints[pIndex];
            Ellipse e = new Ellipse();
            double curCriterrion = 0;
            double theBestCriterion = 0;
            for (int ix = 0; ix < rtdPntsSet.Count(); ix++)
            {
                for (int iy = 0; iy < rtdPntsSet[ix].Count(); iy++)
                {
                    for (int iz = 0; iz < rtdPntsSet[ix][iy].Count(); iz++)
                    {
                        curCriterrion=getEllipseCriterion(rtdPntsSet[ix][iy][iz]);
                        if (curCriterrion > theBestCriterion)
                        {
                            theBestCriterion = curCriterrion;
                            e = getEllipseByRotation(ix, iy, iz);
                        }
                    }
                }
            }
            return e;
        }

        double getEllipseCriterion(point pnt)
        {

            return 10;
        }
        Ellipse getEllipseByRotation(int ix, int iy, int iz) 
        {
            Ellipse e = new Ellipse();
            return e;
        }
    }

    
}
