using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Codex.IPC.DataTypes;

namespace Codex.IPC
{
   internal static class Helpers
   {

      public static bool IsBindingScheme(this BindingScheme scheme, BindingScheme schemeToCheck)
      {
         return (scheme & schemeToCheck) == schemeToCheck;
      }

      public static string GetEndpointAddress(this BindingScheme scheme, ConnectionOptions options, bool isMex = false)
      {
         String transport = String.Empty;
         var serverHostName = options.HostName;
         var port = scheme == BindingScheme.HTTP ? options.HTTPPort : options.TCPPort;
         switch (scheme)
         {
            case BindingScheme.NAMED_PIPE:
               transport = "net.pipe";
               break;
            case BindingScheme.TCP:
               transport = "net.tcp";
               serverHostName = $"{options.HostName}:{port}";
               break;
            case BindingScheme.HTTP:
               transport = "http";
               serverHostName = $"{options.HostName}:{port}";
               break;
         }

         if (isMex)
            return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{options.ProcessID}/mex";
         else
            return $"{transport}://{serverHostName}/Design_Time_Addresses/Codex/{options.ProcessID}/IPCService";

      }


      public static Binding GetBinding(this BindingScheme scheme, ConnectionOptions options)
      {
         Binding binding = null;
         switch (scheme)
         {
            case BindingScheme.TCP:
               {
                  binding = new NetTcpBinding(SecurityMode.None);
                  var tcpBinding = ((NetTcpBinding)binding);
                  tcpBinding.MaxBufferPoolSize = Constants.MAX_MSG_SIZE;
                  tcpBinding.MaxReceivedMessageSize = Constants.MAX_MSG_SIZE;
                  break;
               }
            case BindingScheme.NAMED_PIPE:
               {
                  binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                  var npBinding = ((NetNamedPipeBinding)binding);
                  npBinding.MaxBufferPoolSize = Constants.MAX_MSG_SIZE;
                  npBinding.MaxReceivedMessageSize = Constants.MAX_MSG_SIZE;
                  break;
               }
            case BindingScheme.HTTP:
               {
                  binding = new NetHttpBinding(BasicHttpSecurityMode.None);
                  var httpBinding = ((NetHttpBinding)binding);
                  httpBinding.MaxBufferPoolSize = Constants.MAX_MSG_SIZE;
                  httpBinding.MaxReceivedMessageSize = Constants.MAX_MSG_SIZE;
                  httpBinding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
                  httpBinding.MessageEncoding = NetHttpMessageEncoding.Text;
                  break;
               }
         }

         if (binding != null)
         {
            binding.CloseTimeout = options.CloseTimeout;
            binding.OpenTimeout = options.OpenTimeout;
            binding.ReceiveTimeout = options.ReceiveTimeout;
            binding.SendTimeout = options.SendTimeout;
         }

         return binding;
      }
   }
}
