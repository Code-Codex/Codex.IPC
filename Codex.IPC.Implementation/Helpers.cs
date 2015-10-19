using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    public static class Helpers
    {
        const int MAX_MSG_SIZE = 50000000;
        public static bool IsBindingScheme(this BindingScheme scheme, BindingScheme schemeToCheck)
        {
            return (scheme & schemeToCheck) == schemeToCheck;
        }

        public static string GetEndpointAddress(this BindingScheme scheme, string processID, bool isMex = false, string hostName = "localhost", int portNumber = 64000)
        {
            String transport = String.Empty;
            var serverHostName = hostName;
            if (scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
            {
                transport = "net.pipe";
                if (isMex)
                    return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/mex";
                else
                    return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/IPCService";
            }

            if (scheme.IsBindingScheme(BindingScheme.TCP))
            {
                transport = "net.tcp";
                serverHostName = $"{hostName}:{portNumber}";
                if (isMex)
                    return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/mex";
                else
                    return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/IPCService";
            }

            return String.Empty;
        }


        public static Binding GetBinding(this BindingScheme scheme)
        {
            if (scheme==BindingScheme.TCP)
            {
                var binding = new NetTcpBinding(SecurityMode.None);
                binding.MaxBufferPoolSize = MAX_MSG_SIZE;
                binding.MaxReceivedMessageSize = MAX_MSG_SIZE;
                return binding;
            }

            if (scheme==BindingScheme.NAMED_PIPE)
            {
                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                binding.MaxBufferPoolSize = MAX_MSG_SIZE;
                binding.MaxReceivedMessageSize = MAX_MSG_SIZE;
                return binding;
            }

            return null;
        }
    }
}
