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
            Trace.TraceInformation("Creating service components..");
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _host = WebHost.StartHost();
            Trace.TraceInformation("Block optimisation service started");
        }

        protected override void OnStop()
        {
            _host.Stop();
            Trace.TraceInformation("Block optimisation service stoped");
        }
    }
}
