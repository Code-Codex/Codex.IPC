using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Codex.IPC.Server
{
    public class Server
    {
        public event EventHandler<IPCService.MessageRecievedEventArgs> OnMessageRecieved = delegate { };

        public void Start(ManualResetEvent resetEvent, string baseAddress, string hostName = "localhost")
        {
            var processAddress = $"Design_Time_Addresses/Codex/{baseAddress}";
            Uri[] baseAddresses = { new Uri($"http://{hostName}:8733/{processAddress}"), new Uri($"net.pipe://{hostName}/{processAddress}") };
            using (var host = new ServiceHost(typeof(Codex.IPC.Server.IPCService), baseAddresses))
            {
                IPCService.OnMessageRecieved += (s, e) => OnMessageRecieved(s, e);
                host.Open();

                Console.WriteLine("Service up and running at:");
                foreach (var ea in host.Description.Endpoints)
                {
                    Trace.WriteLine(ea.Address);
                }

                resetEvent.WaitOne();
                host.Close();
            }
        }

    }
}
