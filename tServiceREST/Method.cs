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
    public class point
    {
        public double x;
        public double y;
        public double z;
        public double cr;
    }
    [DataContract]
    public class dhObj
    {
        [DataMember] public point[] points { get; set; }
        [DataMember] public int xElPos { get; set; }
        [DataMember] public int yElPos { get; set; }
        [DataMember] public int zElPos { get; set; }
 
    };

    
    [DataContract]
    public class dhEllipse
    {
        [DataMember] public point Point { get; set; }
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

            dhEllipse[] GetData(dhObj data);
        }

        public class MyService : IMyService
        {
            public dhEllipse[] GetData(dhObj data)
            {
                dhEllipse[] ellipses = new dhEllipse[data.points.Count()];
                for (int i = 0; i < ellipses.Count(); i++)
                    ellipses[i] = new dhEllipse();
                theBestEllipseOfPoint elSeek = new theBestEllipseOfPoint(data.points, data.xElPos, data.yElPos, data.zElPos);
                Console.WriteLine("Обработка");
                Console.WriteLine("Отправленно "+ellipses.Count().ToString()+" эллипсов");
                return ellipses;
            }
        }
}
