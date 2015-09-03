using Codex.IPC.Client;
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
        /// Represents if the server is local or remote.
        /// </summary>
        public enum ServerLocation
        {
            REMOTE,
            LOCAL
        }

        /// <summary>
        /// Generates a server endpoint address.
        /// </summary>
        /// <param name="processID">Process identifier for the server on the host machine.</param>
        /// <param name="serverLoc">Location type of the server.</param>
        /// <param name="hostName">Name of the machine the server is located on.</param>
        /// <param name="portNumber">Port on which the server is listening.</param>
        /// <returns>String representing the endpoint address.</returns>
        public static string GetServerAddress(string processID, ServerLocation serverLoc = ServerLocation.LOCAL, string hostName = "localhost",int portNumber=-1)
        {
            var transport = serverLoc == ServerLocation.LOCAL ? "net.pipe" : "net.tcp";
            var serverHostName = hostName;
            if (serverLoc == ServerLocation.REMOTE)
                serverHostName = $"{hostName}:{portNumber}";

            return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/IPCService";
        }

        /// <summary>
        /// Gets a basic request response client
        /// </summary>
        /// <param name="processID">Process identifier for the server on the host machine.</param>
        /// <param name="serverLoc">Location type of the server.</param>
        /// <param name="hostName">Name of the machine the server is located on.</param>
        /// <param name="portNumber">Port on which the server is listening.</param>
        /// <returns>Client object.</returns>
        public static IPCClient GetClient(string processID, ServerLocation serverLoc = ServerLocation.LOCAL, string hostName = "localhost", int portNumber = -1)
        {
            var endpointName = serverLoc == ServerLocation.LOCAL ? "NamedPipeBinding_IIPCDuplex" : "";
            return new IPCClient("NamedPipeBinding_IIPCDuplex", GetServerAddress(processID, serverLoc, hostName));
        }

        /// <summary>
        /// Gets a duplex client
        /// </summary>
        /// <param name="processID">Process identifier for the server on the host machine.</param>
        /// <param name="serverLoc">Location type of the server.</param>
        /// <param name="hostName">Name of the machine the server is located on.</param>
        /// <param name="portNumber">Port on which the server is listening.</param>
        /// <returns>Client object.</returns>
        public static IPCDuplexClient GetDuplexClient(InstanceContext context, string processID, ServerLocation serverLoc = ServerLocation.LOCAL, string hostName = "localhost", int portNumber = -1)
        {
            var endpointName = serverLoc == ServerLocation.LOCAL ? "NamedPipeBinding_IIPCDuplex" : "";
           return new IPCDuplexClient(context, "NamedPipeBinding_IIPCDuplex", GetServerAddress(processID,serverLoc,hostName));
        }
    }
}
