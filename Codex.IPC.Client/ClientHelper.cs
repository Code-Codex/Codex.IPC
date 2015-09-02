using Codex.IPC.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC
{
    public static class ClientHelper
    {
        public enum CommType
        {
            REMOTE,
            LOCAL
        }
        public static string GetServerAddress(string processID, CommType commType = CommType.LOCAL, string hostName = "localhost",int portNumber=-1)
        {
            var transport = commType == CommType.LOCAL ? "net.pipe" : "net.tcp";
            var serverHostName = hostName;
            if (commType == CommType.REMOTE)
                serverHostName = $"{hostName}:{portNumber}";

            return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/IPCService";
        }

        public static IPCClient GetClient(string processID, CommType commType = CommType.LOCAL, string hostName = "localhost", int portNumber = -1)
        {
            var endpointName = commType == CommType.LOCAL ? "NamedPipeBinding_IIPCDuplex" : "";
            return new IPCClient("NamedPipeBinding_IIPCDuplex", GetServerAddress(processID, commType, hostName));
        }

        public static IPCDuplexClient GetDuplexClient(InstanceContext context, string processID, CommType commType = CommType.LOCAL, string hostName = "localhost", int portNumber = -1)
        {
            var endpointName = commType == CommType.LOCAL ? "NamedPipeBinding_IIPCDuplex" : "";
           return new IPCDuplexClient(context, "NamedPipeBinding_IIPCDuplex", GetServerAddress(processID,commType,hostName));
        }
    }
}
