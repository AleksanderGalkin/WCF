using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using tServiceREST;

namespace MyService    
{
    public class Point
    {
        public double x;
        public double y;
        public double z;
        public double cr;
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
    public class Ellipse
    {
        [DataMember] public Point point { get; set; }
        [DataMember] public double trDipDir { get; set; }
        [DataMember] public double trDip { get; set; }

        public enum Method { Variance, Median }
        private double? criterion_ { get; set; }
        private int axisEllipseA_ { get; set; }
        private int axisEllipseB_ { get; set; }
        private int axisEllipseC_ { get; set; }

         public Ellipse(Point pPoint, int axisEllipseA, int axisEllipseB, int axisEllipseC)
        {
            point = pPoint;
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
                    sumSquareOfIndication +=  Math.Pow (arrayOfPoints[i].cr, 2);
                    sumOfIndication += arrayOfPoints[i].cr;
                    nIndication++;
                }

            }
            criterion_ = (sumSquareOfIndication / nIndication) - Math.Pow(sumOfIndication / nIndication, 2); // For a while Variance only
            return 0;
        }

        public double? getCriterion()
        {
            return criterion_;
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

            Ellipse[] getData(DhObj data);
        }

        public class MyService : IMyService
        {
            public Ellipse[] getData(DhObj data)
            {
                
               // Ellipse[] ellipses1 = new Ellipse[data.points.Count()];
                Ellipse[] ellipses = new Ellipse[data.points.Count()];


                TheBestEllipseOfPoint elSeek = new TheBestEllipseOfPoint(data.points, data.xElPos, data.yElPos, data.zElPos);
                elSeek.setTheBestEllipses();
                ellipses=elSeek.getTheBestEllipses();

                //for (int i = 0; i < ellipses1.Count(); i++)
                //{
                //    //ellipses1[i] = new Ellipse(data.points[i], 15, 10, 10);
                //    ellipses1[i] = ellipses[i];
                //    d1[i] = ellipses[i].trDipDir;
                //    d2[i] = ellipses[i].trDip;
                //   // ellipses1[i].trDip = 14.174204666693806;
                //   // ellipses1[i].trDipDir = 211.06298167815987;

                //}

                Console.WriteLine("Обработка");
                Console.WriteLine("Отправленно "+ellipses.Count().ToString()+" эллипсов");
                return ellipses;
            }
        }
}
