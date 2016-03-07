using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WebServiceHost;

namespace CoreService
{
    public partial class CoreService : ServiceBase
    {
        private WebHost _host;
        public CoreService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("Block optimisation service started");
            _host = WebHost.StartHost();
        }

        protected override void OnStop()
        {
            _host.Stop();
            Trace.WriteLine("Block optimisation service stoped");
        }
    }
}
