using System.ServiceProcess;
using System.Threading;

namespace GoogleHosts
{
    public partial class ServiceMain : ServiceBase
    {
        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Hosts hosts = new Hosts();

            Thread thread = new Thread(hosts.MainProcess);

            thread.Start();
        }

        protected override void OnStop()
        {
        }
    }
}