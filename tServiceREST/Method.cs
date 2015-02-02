using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace MyService    
{
    public struct s1 { public int a; public int b; };    
    
        [ServiceContract]
        public interface IMyService
        {
            [OperationContract]
            [WebInvoke(Method = "GET",   
            RequestFormat = WebMessageFormat.Json,   
            ResponseFormat = WebMessageFormat.Json,
           // BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "GetData/{id}")]
            s1[] GetData(string id);
        }

        public class MyService : IMyService
        {
            public s1[] GetData(string id)
            {

                //switch (name)
                //{
                //    case "Galkin":
                //        return "Aleksandr";
                //    case "Galkina":
                //        return "Viktoria";
                //    default:
                //        return name + " is undefined";
                //}

                s1[] names = new[] { new s1 { a = 1, b = 2 }, new s1 { a = 4, b = Convert.ToInt32(id) } };
                return  names;
            }
        }
}
