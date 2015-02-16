using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using tServiceREST;
using log4net;
using log4net.Config;
using System.Threading;
using System.ComponentModel;



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
        public BmObj() { }
        public BmObj(BmObj bmObj, int nParts, int numOfPart)
        {
            this.xAxis = bmObj.xAxis;
            this.yAxis = bmObj.yAxis;
            this.zAxis = bmObj.zAxis;
            this.xElPos = bmObj.xElPos;
            this.yElPos = bmObj.yElPos;
            this.zElPos = bmObj.zElPos;
            int itemFrom = (numOfPart - 1) * bmObj.points.Count() / nParts;
            int itemTo = numOfPart != nParts ?
                                                numOfPart * bmObj.points.Count() / nParts
                                             :
                                                bmObj.points.Count();

            points = new Point[itemTo-itemFrom];
            int k = 0;
            for (int i = itemFrom; i < itemTo; i++)
            {
                this.points[k++] = new Point(bmObj.points[i]);
            }
        }

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
    public class Progress
    {
        [DataMember]
        public double percent { get; set; }
        public Progress(int parPercent)
        {
            percent = parPercent;
        }
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
            UriTemplate = "startCalculation/")]

            //Ellipse[] getData(DhBmObj data);
            Ellipse[] startCalculation(DhBmObj data);

            //[OperationContract]
            //[WebInvoke(Method = "GET",
            //RequestFormat = WebMessageFormat.Json,
            //ResponseFormat = WebMessageFormat.Json,
            //    //BodyStyle = WebMessageBodyStyle.Wrapped,
            //UriTemplate = "getProgress/")]
            //Progress[]  getProgress();

            //[OperationContract]
            //[WebInvoke(Method = "GET",
            //RequestFormat = WebMessageFormat.Json,
            //ResponseFormat = WebMessageFormat.Json,
            //    //BodyStyle = WebMessageBodyStyle.Wrapped,
            //UriTemplate = "getResult/")]
            //Ellipse[] getResult();
        }

         
        public class MyService : IMyService
        {
            
            public static readonly ILog log = LogManager.GetLogger(typeof(MyService));
            public static readonly ILog logCon = LogManager.GetLogger("ConsoleAppender");
            private int nTread_ = 4;
            private Ellipse[] ellipses_;
            private TheBestEllipseOfPoint[] theBestEllipseOfPoint_;
            private DhBmObj inData_;
            private Progress[] progress_;
            //struct TheBestEllipseOfPointStruct
            //{
            //    TheBestEllipseOfPointStruct
            //}
            //public Ellipse[] getData(DhBmObj data)

            public Ellipse[] startCalculation(DhBmObj data)
            {
                
               // Ellipse[] ellipses1 = new Ellipse[data.points.Count()];
               // Ellipse[] ellipses = new Ellipse[data.bm.points.Count()];
                inData_ = data;
                BmObj[] bmObjs = new BmObj[nTread_];
                progress_ = new Progress[nTread_];
                theBestEllipseOfPoint_ = new TheBestEllipseOfPoint[nTread_];
                Thread[] threads=new Thread[nTread_];
               // BackgroundWorker[] aBw = new BackgroundWorker[nTread_];
                for (int i = 0; i < nTread_; i++)
                {
                    
                    //aBw[i] = new BackgroundWorker();
                    //aBw[i].DoWork += startThread;
                    //aBw[i].WorkerReportsProgress = true;
                    //aBw[i].ProgressChanged += progressThread;
                    //aBw[i].RunWorkerCompleted += completedThread;
                    //progress_[i] = new Progress(0);
                    //bmObjs[i] = new BmObj(inData_.bm, nTread_, i + 1);
                    //theBestEllipseOfPoint_[i] = new TheBestEllipseOfPoint(inData_.dh.points, bmObjs[i].points, bmObjs[i].xAxis, bmObjs[i].yAxis, bmObjs[i].zAxis, bmObjs[i].xElPos, bmObjs[i].yElPos, bmObjs[i].zElPos);
                    //theBestEllipseOfPoint_[i].idxThreat = i;
                    //aBw[i].RunWorkerAsync(theBestEllipseOfPoint_[i]);
                    
                    threads[i] = new Thread(startThread);
                    bmObjs[i] = new BmObj(inData_.bm, nTread_, i + 1);
                    theBestEllipseOfPoint_[i] = new TheBestEllipseOfPoint(inData_.dh.points, bmObjs[i].points, bmObjs[i].xAxis, bmObjs[i].yAxis, bmObjs[i].zAxis, bmObjs[i].xElPos, bmObjs[i].yElPos, bmObjs[i].zElPos);
                    threads[i].IsBackground = true;
                    threads[i].Start(theBestEllipseOfPoint_[i]);
                    
                    


                }

                
                
                for (int i = 0; i < nTread_; i++)
                {
                    threads[i].Join();
                }
                ellipses_ = theBestEllipseOfPoint_[0].getTheBestEllipses().ToArray();
                for (int i = 1; i < nTread_; i++)
                {
                    ellipses_ = ellipses_.Concat(theBestEllipseOfPoint_[i].getTheBestEllipses()).ToArray();
                }

                //log.DebugFormat("Объявлем класс TheBestEllipseOfPoint: кол-во скважин - {0},кол-во ячеек БМ - {1},размер полуосей:{2}-{3}-{4} кол-во эллипсов вокруг: X - {5}," +
                //                "Y - {6}, Z - {7}", data.dh.points.Count(), data.bm.points.Count(), data.bm.xAxis, data.bm.yAxis, data.bm.zAxis, data.dh.xElPos, data.dh.yElPos, data.dh.zElPos);

                //TheBestEllipseOfPoint elSeek = new TheBestEllipseOfPoint(data.dh.points, data.bm.points, data.bm.xAxis, data.bm.yAxis, data.bm.zAxis, data.bm.xElPos, data.bm.yElPos, data.bm.zElPos);
                //log.Debug("Запускаем вычисление оптимальных эллипсов для каждой ячейки");
                //elSeek.ComputeTheBestEllipses();
                //ellipses_ = elSeek.getTheBestEllipses();

                for (int i = 0; i < data.bm.points.Count(); i++)
                {
                    if (
                          !(ellipses_[i].point.x == data.bm.points[i].x
                            || ellipses_[i].point.y == data.bm.points[i].y
                            || ellipses_[i].point.z == data.bm.points[i].z
                            )
                        )
                    {
                        log.Error("Координаты входной БМ не соответствуют координатам результата");
                        logCon.Error("Координаты входной БМ не соответствуют координатам результата");
                        return null;
                    }
                }

                log.InfoFormat("Возвращено в вызывающий модуль {0} ячеек БМ с параметрами эллипсов", ellipses_.Count());
                Console.WriteLine(String.Format("Возвращено в вызывающий модуль {0} ячеек БМ с параметрами эллипсов", ellipses_.Count()));
                return ellipses_;
                
            }

            static void startThread(object obj)
           // static void startThread(object sender, DoWorkEventArgs e) 
            {
                if (obj.GetType() != typeof(TheBestEllipseOfPoint))
                    return;
                TheBestEllipseOfPoint theBestEllipseOfPoint = obj as TheBestEllipseOfPoint;
                theBestEllipseOfPoint.ComputeTheBestEllipses();
            }
         
          
          

        }
}
