using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Client
{
    //[ServiceContract]
    //public interface IMyService
    //{
    //    [OperationContract]
    //    string GetData(string name);
    //}
    public class ClientApplication
    {
        static void Main(string[] args)
        {
            //string URI = "http://localhost:8089/";
            //EndpointAddress address = new EndpointAddress(URI);
            //BasicHttpBinding binding = new BasicHttpBinding();
            //ChannelFactory<IMyService> factory = new ChannelFactory<IMyService>(binding, address);
            //IMyService service = factory.CreateChannel();
            MyServiceClient service = new MyServiceClient();
            string buf;
            while ((buf = Console.ReadLine()).Length>0)
            {
                Console.WriteLine(buf.Length);
                Console.WriteLine(buf);
                Console.WriteLine(service.GetData(buf));
                
            }
        }
    }
}
