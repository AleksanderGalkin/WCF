using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace MyService    
{
    [DataContract]
    public class s1 
    {
        [DataMember]public int id {get;set;}
        [DataMember]public string name{get;set;}
        [DataMember]public double[] val{get;set;}
    };

    
        [ServiceContract]
        public interface IMyService
        {
            [OperationContract]
            [WebInvoke(Method = "POST",   
            RequestFormat = WebMessageFormat.Json,   
            ResponseFormat = WebMessageFormat.Json,
          //  BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "GetData/")]
            s1[] GetData(s1[] data);

            //[OperationContract]
            //[WebInvoke(Method = "GET",
            //RequestFormat = WebMessageFormat.Json,
            //ResponseFormat = WebMessageFormat.Json,
            //    // BodyStyle = WebMessageBodyStyle.Wrapped,
            //UriTemplate = "GetData/{id}")]
            //s1 GetData(string id);

            //[OperationContract]
            //[WebInvoke(Method="POST",
            //RequestFormat = WebMessageFormat.Json,
            //ResponseFormat = WebMessageFormat.Json,
            //    // BodyStyle = WebMessageBodyStyle.Wrapped,
            //UriTemplate = "PostData/")]
            //void GetData(s1[] data);
        }

        public class MyService : IMyService
        {
            public s1[] GetData(s1[] data)
            {
                Console.WriteLine(data.Length);
                //s1[] ret_data=new s1[data.Count];
                //for (int i=0;i<data.Count;i++)
                //{
                //    ret_data[i].id = data[i].id;
                //    ret_data[i].name = data[i].name;
                //    ret_data[i].val = data[i].val + 1.5;
                //}

                Console.WriteLine("Возврат данных");
                return data;
            }
        }
}
