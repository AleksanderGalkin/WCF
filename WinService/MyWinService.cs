using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceProcess.Design;
using System.ComponentModel;


namespace WindowsService
{
    public class WindowsService : System.ServiceProcess.ServiceBase
    {
        
        private ProjectInstaller projectInstaller2;
        private System.Diagnostics.EventLog eLog;

        public WindowsService()
        {
            this.ServiceName = "MyService";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.eLog = new System.Diagnostics.EventLog();
            this.projectInstaller2 = new WindowsService.ProjectInstaller();
            ((System.ComponentModel.ISupportInitialize)(this.eLog)).BeginInit();
            // 
            // eLog
            // 
            this.eLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eLog_EntryWritten);
            ((System.ComponentModel.ISupportInitialize)(this.eLog)).EndInit();

        }

        public static void Main()
        {

            System.ServiceProcess.ServiceBase.Run(new WindowsService());
        }

 
        protected override void OnStart(string[] args)
         {
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
             if (!EventLog.SourceExists("MyExampleService"))
             {
               EventLog.CreateEventSource("MyExampleService", "MyExampleService");
             }
             eLog.Source = "MyExampleService";
             eLog.WriteEntry(log);
           }
           catch{}
        }

         [RunInstallerAttribute(true)]
         public class ProjectInstaller : System.Configuration.Install.Installer
         {
             public ProjectInstaller()
                 : base()
             {
                 InitializeComponent();
             }

             private void InitializeComponent()
             {

             }
         }

         private void projectInstaller1_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
         {

         }

         private void eLog_EntryWritten(object sender, EntryWrittenEventArgs e)
         {

         }
  


    }
}
