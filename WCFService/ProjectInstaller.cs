﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;

namespace WindowsService
{
    [RunInstaller(true)]

    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            // Attach the 'AfterRollback' event.
            this.AfterRollback += new InstallEventHandler(MyInstaller_AfterRollBack);
        }


    }
}

namespace WindowsService
{
    partial class ProjectInstaller
    {

        private System.ComponentModel.IContainer components = null;

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code


        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            //
            // serviceProcessInstaller1
            //
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            //
            // serviceInstaller1
            //
            this.serviceInstaller1.Description = "WCF сервис через Windows Service";
            this.serviceInstaller1.DisplayName = "WCFService";
            this.serviceInstaller1.ServiceName = "WCFService";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            //
            // ProjectInstaller
            //
            this.Installers.AddRange(new System.Configuration.Install.Installer[] 
                                           {
                                            this.serviceProcessInstaller1,
                                            this.serviceInstaller1
                                           });

        }

        #endregion

        // Event handler for 'AfterRollback' event.
        private void MyInstaller_AfterRollBack(object sender, InstallEventArgs e)
        {
            Console.WriteLine("Что-то пошло не так. Устанавливать службу необходимо с правами администратора.");
        }


        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}