using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using MyService;

namespace Server
{
    class Program
    {
        const string URI = "http://localhost:8089/";
        static void Main(string[] args)
        {
            Type ServiceType=typeof(MyService.MyService);
            Uri ServiceURI = new Uri(URI);
            ServiceHost host = new ServiceHost(ServiceType, ServiceURI);
            host.Open();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Сервис информации о имени человека");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Для закрытия сервиса нажмите Enter");
            Console.Read();

        }
    }

}

