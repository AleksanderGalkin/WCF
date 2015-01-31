using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MyService    
{

        [ServiceContract]
        public interface IMyService
        {
            [OperationContract]
            string GetData(string name);
        }

        public class MyService : IMyService
        {
            public string GetData(string name)
            {
                switch (name)
                {
                    case "Galkin":
                        return "Aleksandr";
                    case "Galkina":
                        return "Viktoria";
                    default:
                        return name + " is undefined";
                }
            }
        }
}
