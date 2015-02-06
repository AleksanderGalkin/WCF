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
    public class DhEllipse
    {
        [DataMember] public Point Point { get; set; }
        [DataMember] public double dipdirection { get; set; }
        [DataMember] public double dip { get; set; }
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

            DhEllipse[] getData(DhObj data);
        }

        public class MyService : IMyService
        {
            public DhEllipse[] getData(DhObj data)
            {
                DhEllipse[] ellipses = new DhEllipse[data.points.Count()];
                for (int i = 0; i < ellipses.Count(); i++)
                    ellipses[i] = new DhEllipse();
                TheBestEllipseOfPoint elSeek = new TheBestEllipseOfPoint(data.points, data.xElPos, data.yElPos, data.zElPos);
                elSeek.setTheBestEllipses();
                
                Console.WriteLine("Обработка");
                Console.WriteLine("Отправленно "+ellipses.Count().ToString()+" эллипсов");
                return ellipses;
            }
        }
}
