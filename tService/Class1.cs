﻿using System;
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
            string GetData();
        }

        public class MyService : IMyService
        {
            public string GetData()
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
                return "{'name':'Aleksandr'}";
            }
        }
}
