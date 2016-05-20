using Codex.IPC.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
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
        /// <param name="processID">Base address for the process.</param>
        /// <param name="hostName">Host name for the machine.</param>
        public void Start(ManualResetEvent resetEvent, string processID, BindingScheme scheme, string hostName = "localhost", int tcpPort = Constants.TCP_PORT_NUMBER, int httpPort = Constants.HTTP_PORT_NUMBER)
        {
            List<Uri> baseAddresses = new List<Uri>();
            if (scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
            {
                baseAddresses.Add(new Uri(BindingScheme.NAMED_PIPE.GetEndpointAddress(processID, false, hostName, tcpPort)));
            }

            if (scheme.IsBindingScheme(BindingScheme.TCP))
            {
                baseAddresses.Add(new Uri(BindingScheme.TCP.GetEndpointAddress(processID, false, hostName, tcpPort)));
            }

            if (scheme.IsBindingScheme(BindingScheme.HTTP))
            {
                baseAddresses.Add(new Uri(BindingScheme.HTTP.GetEndpointAddress(processID, false, hostName, httpPort)));
            }
            using (var host = new ServiceHost(IPCService.Instance, baseAddresses.ToArray()))
            {

                // Check to see if the service host already has a ServiceMetadataBehavior
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                // If not, add one
                if (smb == null)
                    smb = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(smb);

                // Setup the bindings 
                if (scheme.IsBindingScheme(BindingScheme.TCP))
                {
                    var tcpBinding = (NetTcpBinding)BindingScheme.TCP.GetBinding();
                    host.AddServiceEndpoint(typeof(IIPC), tcpBinding, "");
                    host.AddServiceEndpoint(typeof(IIPCDuplex), tcpBinding, "");

                    host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), BindingScheme.TCP.GetEndpointAddress(processID, true, hostName, tcpPort));
                }

                if (scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
                {
                    var namedPipeBinding = (NetNamedPipeBinding)BindingScheme.NAMED_PIPE.GetBinding();
                    host.AddServiceEndpoint(typeof(IIPC), namedPipeBinding, "");
                    host.AddServiceEndpoint(typeof(IIPCDuplex), namedPipeBinding, "");
                    host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), BindingScheme.NAMED_PIPE.GetEndpointAddress(processID, true, hostName, tcpPort));
                }

                if (scheme.IsBindingScheme(BindingScheme.HTTP))
                {
                    var httpBinding = (NetHttpBinding)BindingScheme.HTTP.GetBinding();
                    host.AddServiceEndpoint(typeof(IIPC), httpBinding, "");
                    host.AddServiceEndpoint(typeof(IIPCDuplex), httpBinding, "");
                    host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), BindingScheme.HTTP.GetEndpointAddress(processID, true, hostName, httpPort));
                }

                host.Open();
                Console.WriteLine("Service up and running at:");
                foreach (var ea in host.Description.Endpoints)
                {
                    Console.WriteLine(ea.Address);
                }

                resetEvent.WaitOne();
                host.Close();
            }
        }

    }
}
