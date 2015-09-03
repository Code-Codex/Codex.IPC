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
        /// <summary>
        /// Start the IPC server for this process.
        /// </summary>
        /// <param name="resetEvent">Reset event to gracefully shutdown the server.</param>
        /// <param name="baseAddress">Base address for the process.</param>
        /// <param name="hostName">Host name for the machine.</param>
        public void Start(ManualResetEvent resetEvent, string baseAddress, string hostName = "localhost",int port = 64000)
        {
            var processAddress = $"Design_Time_Addresses/Codex/{baseAddress}";
            Uri[] baseAddresses = { new Uri($"net.pipe://{hostName}/{processAddress}"), new Uri($"net.tcp://{hostName}:{port}/{processAddress}") };
            using (var host = new ServiceHost(IPCService.Instance, baseAddresses))
            {
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
