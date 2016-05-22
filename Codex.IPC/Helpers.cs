using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC
{
    internal static class Helpers
    {
        const int MAX_MSG_SIZE = 50000000;

        public static bool IsBindingScheme(this BindingScheme scheme, BindingScheme schemeToCheck)
        {
            return (scheme & schemeToCheck) == schemeToCheck;
        }

        public static string GetEndpointAddress(this BindingScheme scheme, string processID, bool isMex = false, string hostName = "localhost", int portNumber = Constants.TCP_PORT_NUMBER)
        {
            String transport = String.Empty;
            var serverHostName = hostName;
            switch (scheme)
            {
                case BindingScheme.NAMED_PIPE:
                    transport = "net.pipe";
                    break;
                case BindingScheme.TCP:
                    transport = "net.tcp";
                    serverHostName = $"{hostName}:{portNumber}";
                    break;
                case BindingScheme.HTTP:
                    transport = "http";
                    serverHostName = $"{hostName}:{portNumber}";
                    break;
            }

            if (isMex)
                return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/mex";
            else
                return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{processID}/IPCService";

        }


        public static Binding GetBinding(this BindingScheme scheme)
        {
            Binding binding = null;
            switch (scheme)
            {
                case BindingScheme.TCP:
                    {
                        binding = new NetTcpBinding(SecurityMode.None);
                        ((NetTcpBinding)binding).MaxBufferPoolSize = MAX_MSG_SIZE;
                        ((NetTcpBinding)binding).MaxReceivedMessageSize = MAX_MSG_SIZE;
                        break;
                    }
                case BindingScheme.NAMED_PIPE:
                    {
                        binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                        ((NetNamedPipeBinding)binding).MaxBufferPoolSize = MAX_MSG_SIZE;
                        ((NetNamedPipeBinding)binding).MaxReceivedMessageSize = MAX_MSG_SIZE;
                        break;
                    }
                case BindingScheme.HTTP:
                    {
                        binding = new NetHttpBinding(BasicHttpSecurityMode.None);
                        var httpBinding = ((NetHttpBinding)binding);
                        httpBinding.MaxBufferPoolSize = MAX_MSG_SIZE;
                        httpBinding.MaxReceivedMessageSize = MAX_MSG_SIZE;
                        httpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
                        httpBinding.MessageEncoding = NetHttpMessageEncoding.Text;
                        break;
                    }
            }

            return binding;
        }
    }
}
