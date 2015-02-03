using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceProcess.Design;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WindowsService
{
    public class WFCService : System.ServiceProcess.ServiceBase
    {

        private ProjectInstaller projectInstaller2;
        private System.Diagnostics.EventLog eLog;

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
            this.eLog = new System.Diagnostics.EventLog();
            this.projectInstaller2 = new ProjectInstaller();
            ((System.ComponentModel.ISupportInitialize)(this.eLog)).BeginInit();
            // 
            // eLog
            // 
            this.eLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eLog_EntryWritten);
            ((System.ComponentModel.ISupportInitialize)(this.eLog)).EndInit();

        }

        public static void Main(string[] args)
        {
            
                        //WFCService service = new WFCService();
                        //WFCService.Run
                        //// Put a breakpoint on the following line to always catch
                        //// your service when it has finished its work
                        //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            

            WFCService service = new WFCService();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }

            //    System.ServiceProcess.ServiceBase.Run(new WFCService());

            
            //Console.WriteLine("asdasd");
            //Type ServiceType = typeof(MyService.MyService);
            //Uri ServiceURI = new Uri("http://localhost:8089/");
            //ServiceHost host = new ServiceHost(ServiceType, ServiceURI);
            //host.Open();
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.WriteLine("Сервис информации о имени человека");
            //Console.ForegroundColor = ConsoleColor.Gray;
            //Console.WriteLine("Для закрытия сервиса нажмите Enter");
            //Console.Read();

        }


        protected override void OnStart(string[] args)
        {
            string URI = "http://localhost:8089/";
            Type ServiceType=typeof(MyService.MyService);
            Uri ServiceURI = new Uri(URI);
            WebServiceHost host = new WebServiceHost(ServiceType, ServiceURI);
            host.Open();
            AddLog("start");
        }

        protected override void OnStop()
        {
            AddLog("stop");
        }

        public void AddLog(string log)
        {
            try
            {
                if (!EventLog.SourceExists("MyWFCService"))
                {
                    EventLog.CreateEventSource("MyWFCService", "MyWFCService");
                }
                eLog.Source = "MyWFCService";
                eLog.WriteEntry(log);
            }
            catch { }
        }
        private void eLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }



    }
}
