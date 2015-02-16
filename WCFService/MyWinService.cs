using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceProcess.Design;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using log4net;
using log4net.Config;

namespace WindowsService
{
    public class WFCService : System.ServiceProcess.ServiceBase
    {

        
        public static readonly ILog log = LogManager.GetLogger(typeof(WFCService));
        public static readonly ILog logCon = LogManager.GetLogger("ConsoleAppender");
        
        private ProjectInstaller projectInstaller2;
        public WFCService()
        {
            this.ServiceName = "MyWFCService";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            log4net.Config.XmlConfigurator.Configure();
            this.projectInstaller2 = new ProjectInstaller();
        }

        public static void Main(string[] args)
        {
            WFCService service = new WFCService();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press Enter key to stop program");
                Console.Read();
                
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }

            // SOAP архитектура
            //Type ServiceType = typeof(MyService.MyService);
            //Uri ServiceURI = new Uri("http://localhost:8089/");
            //ServiceHost host = new ServiceHost(ServiceType, ServiceURI);
            //host.Open();

        }


        protected override void OnStart(string[] args)
        {
            string URI = "http://localhost:8089/";
            Type ServiceType=typeof(MyService.MyService);
            Uri ServiceURI = new Uri(URI);
            WebServiceHost host = new WebServiceHost(ServiceType, ServiceURI);
           
            host.Open();
            log.Info("Запустили сервис");
            logCon.Info("Запустили сервис");
        }

        protected override void OnStop()
        {
            log.Info("Останавливаем сервис");
            logCon.Info("Останавливаем сервис");
        }

      
        //private void eLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        //{

        //}



    }
}
