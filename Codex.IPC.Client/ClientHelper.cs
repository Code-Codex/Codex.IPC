using Codex.IPC.Client;
using Codex.IPC.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC
{
    /// <summary>
    /// Collection of helper functions for creating IPC clients.
    /// </summary>
    public static class ClientHelper
    {

        /// <summary>
        /// Gets a basic request response client
        /// </summary>
        /// <param name="processID">Process identifier for the server on the host machine.</param>
        /// <param name="scheme">Binding scheme to use.</param>
        /// <param name="hostName">Name of the machine the server is located on.</param>
        /// <param name="portNumber">Port on which the server is listening.</param>
        /// <returns>Client object.</returns>
        public static IPCClient GetClient(string processID, BindingScheme scheme = BindingScheme.NAMED_PIPE, string hostName = "localhost", int portNumber = -1)
        {
            return new IPCClient(scheme.GetBinding(), new EndpointAddress(scheme.GetEndpointAddress(processID, false, hostName, portNumber)));
        }

        /// <summary>
        /// Gets a duplex client
        /// </summary>
        /// <param name="processID">Process identifier for the server on the host machine.</param>
        /// <param name="scheme">Binding scheme to use.</param>
        /// <param name="hostName">Name of the machine the server is located on.</param>
        /// <param name="portNumber">Port on which the server is listening.</param>
        /// <returns>Client object.</returns>
        public static IPCDuplexClient GetDuplexClient(InstanceContext context, string processID, BindingScheme scheme = BindingScheme.NAMED_PIPE, string hostName = "localhost", int portNumber = -1)
        {
           return new IPCDuplexClient(context, scheme.GetBinding(), new EndpointAddress(scheme.GetEndpointAddress(processID, false, hostName, portNumber)));
        }
    }
}
