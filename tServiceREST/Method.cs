﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using tServiceREST;
using log4net;
using log4net.Config;

namespace MyService    
{
    public class Point
    {
        public double x;
        public double y;
        public double z;
        public double cr;
        public string info;
        public string priznak;

        public Point (Point point)
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
    }
    [DataContract]
    public class DhObj
    {
        [DataMember] public Point[] points { get; set; }
        [DataMember] public int xElPos { get; set; }
        [DataMember] public int yElPos { get; set; }
        [DataMember] public int zElPos { get; set; }
 
    };
    [DataContract]
    public class BmObj
    {
        [DataMember] public Point[] points { get; set; }
        [DataMember] public int xAxis { get; set; }
        [DataMember] public int yAxis { get; set; }
        [DataMember] public int zAxis { get; set; }
        [DataMember] public int xElPos { get; set; }
        [DataMember] public int yElPos { get; set; }
        [DataMember] public int zElPos { get; set; }

    };

    [DataContract]
    public class DhBmObj
    {
        [DataMember]
        public DhObj dh { get; set;}
        [DataMember]
        public BmObj bm { get; set; }
    }

    [DataContract]
    public class Ellipse
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Ellipse));

        [DataMember] public Point point { get; set; }
        [DataMember] public double trDipDir { get; set; }
        [DataMember] public double trDip { get; set; }

        public enum Method { Variance, Median }

        private double? criterion_ { get; set; }
        private int? nIndication_ { get; set; }

        private double axisEllipseA_ { get; set; }
        private double axisEllipseB_ { get; set; }
        private double axisEllipseC_ { get; set; }


        public Ellipse(Point pPoint, double axisEllipseA, double axisEllipseB, double axisEllipseC)
        {
            point = pPoint;
            axisEllipseA_ = axisEllipseA;
            axisEllipseB_ = axisEllipseB;
            axisEllipseC_ = axisEllipseC;
            criterion_ = null;
            nIndication_ = null;
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

                    log.DebugFormat("Скважина - {0}, значение критерия - {1}", arrayOfPoints[i].info, arrayOfPoints[i].cr);
                    sumSquareOfIndication += Math.Pow(arrayOfPoints[i].cr, 2);
                    sumOfIndication += arrayOfPoints[i].cr;
                    nIndication++;
                }

            }

            criterion_ = (sumSquareOfIndication / nIndication) - Math.Pow(sumOfIndication / nIndication, 2); // For a while Variance only
            nIndication_ = nIndication;

            log.DebugFormat("Значение оценки для эллипса: {0}", criterion_);
            return 0;
        }

        public double? getCriterion()
        {
            return criterion_;
        }

        public double? getNumberIndication()
        {
            return nIndication_;
        }

        bool isPointOfEllipse(Point parPoint)
        {

            double radicalExpression = 1 - (Math.Pow(parPoint.x - point.x, 2) / Math.Pow(axisEllipseA_, 2))
                                            - (Math.Pow(parPoint.z - point.z, 2) / Math.Pow(axisEllipseC_, 2));
            
            if (radicalExpression < 0)
            {
                return false;
            }
            double topFunctionOfXY = point.y + axisEllipseB_ * Math.Sqrt(radicalExpression);
            double bottomFunctionOfXY = point.y - axisEllipseB_ * Math.Sqrt(radicalExpression);

            if (parPoint.y >=  bottomFunctionOfXY && parPoint.y <= topFunctionOfXY)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    };
    
        [ServiceContract]
        public interface IMyService
        {
            [OperationContract]
            [WebInvoke(Method = "POST",   
            RequestFormat = WebMessageFormat.Json,   
            ResponseFormat = WebMessageFormat.Json,
            //BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "GetData/")]

            Ellipse[] getData(DhBmObj data);
        }

        public class MyService : IMyService
        {
            public static readonly ILog log = LogManager.GetLogger(typeof(MyService));
            public static readonly ILog logCon = LogManager.GetLogger("ConsoleAppender");

            public Ellipse[] getData(DhBmObj data)
            {

               // Ellipse[] ellipses1 = new Ellipse[data.points.Count()];
                Ellipse[] ellipses = new Ellipse[data.bm.points.Count()];

                log.DebugFormat("Объявлем класс TheBestEllipseOfPoint: кол-во скважин - {0},кол-во ячеек БМ - {1},размер полуосей:{2}-{3}-{4} кол-во эллипсов вокруг: X - {5}," +
                                "Y - {6}, Z - {7}", data.dh.points.Count(), data.bm.points.Count(), data.bm.xAxis, data.bm.yAxis, data.bm.zAxis, data.dh.xElPos, data.dh.yElPos, data.dh.zElPos);

                TheBestEllipseOfPoint elSeek = new TheBestEllipseOfPoint(data.dh.points, data.bm.points, data.bm.xAxis, data.bm.yAxis, data.bm.zAxis, data.bm.xElPos, data.bm.yElPos, data.bm.zElPos);
                log.Debug("Запускаем вычисление оптимальных эллипсов для каждой ячейки");
                elSeek.ComputeTheBestEllipses();
                ellipses=elSeek.getTheBestEllipses();

                for (int i = 0; i < data.bm.points.Count(); i++ )
                {
                    if (
                          !(   ellipses[i].point.x == data.bm.points[i].x
                            || ellipses[i].point.y == data.bm.points[i].y
                            || ellipses[i].point.z == data.bm.points[i].z
                            )
                        )
                    {
                        log.Error ("Координаты входной БМ не соответствуют координатам результата");
                        logCon.Error ("Координаты входной БМ не соответствуют координатам результата");
                        return null;
                    }
                }

                log.InfoFormat("Возвращено в вызывающий модуль {0} ячеек БМ с параметрами эллипсов", ellipses.Count());
                Console.WriteLine(String.Format("Возвращено в вызывающий модуль {0} ячеек БМ с параметрами эллипсов", ellipses.Count()));
                return ellipses;
            }
        }
}
