namespace CoreService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BlockServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CoreServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BlockServiceProcessInstaller
            // 
            this.BlockServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BlockServiceProcessInstaller.Password = null;
            this.BlockServiceProcessInstaller.Username = null;
            // 
            // CoreServiceInstaller
            // 
            this.CoreServiceInstaller.Description = "Block optimization service ";
            this.CoreServiceInstaller.DisplayName = "Block Optimization Service ";
            this.CoreServiceInstaller.ServiceName = "BlockOptimizationService";
            this.CoreServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BlockServiceProcessInstaller,
            this.CoreServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BlockServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller CoreServiceInstaller;
    }
}