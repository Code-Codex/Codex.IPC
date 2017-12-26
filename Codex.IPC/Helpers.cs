using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;
using Codex.IPC.Contracts;
using Codex.IPC.DataTypes;

namespace Codex.IPC
{
   internal static class Helpers
   {
      public static void InitializeHost(this ServiceHost host, ConnectionOptions options)
      {
         // Check to see if the service host already has a ServiceMetadataBehavior
         var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
         // If not, add one
         if (smb == null)
         {
            smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
         }

         // Check to see if the service host already has a ServiceDiscoveryBehavior
         var sdiscb = host.Description.Behaviors.Find<ServiceDiscoveryBehavior>();
         // If not, add one
         if (sdiscb == null)
         {
            sdiscb = new ServiceDiscoveryBehavior();
            host.Description.Behaviors.Add(sdiscb);
         }

         // Check to see if the service host already has a ServiceDebugBehavior
         var sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
         // If not, add one
         if (sdb == null)
         {
            sdb = new ServiceDebugBehavior
            {
               IncludeExceptionDetailInFaults = true
            };
            host.Description.Behaviors.Add(sdb);
         }



         List<ServiceEndpoint> contractEndpoints = new List<ServiceEndpoint>();
         // Setup the bindings 
         if (options.Scheme.IsBindingScheme(BindingScheme.TCP))
         {
            var tcpBinding = (NetTcpBinding)BindingScheme.TCP.GetBinding(options);
            host.AddServiceEndpoint(typeof(IIPC), tcpBinding, "");
            host.AddServiceEndpoint(typeof(IIPCDuplex), tcpBinding, "");
            contractEndpoints.Add(host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), BindingScheme.TCP.GetEndpointAddress(options, true)));
         }

         if (options.Scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
         {
            var namedPipeBinding = (NetNamedPipeBinding)BindingScheme.NAMED_PIPE.GetBinding(options);
            host.AddServiceEndpoint(typeof(IIPC), namedPipeBinding, "");
            host.AddServiceEndpoint(typeof(IIPCDuplex), namedPipeBinding, "");
            contractEndpoints.Add(host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), BindingScheme.NAMED_PIPE.GetEndpointAddress(options, true)));
         }

         if (options.Scheme.IsBindingScheme(BindingScheme.HTTP))
         {
            var httpBinding = (NetHttpBinding)BindingScheme.HTTP.GetBinding(options);
            host.AddServiceEndpoint(typeof(IIPC), httpBinding, "");
            host.AddServiceEndpoint(typeof(IIPCDuplex), httpBinding, "");
            contractEndpoints.Add(host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), BindingScheme.HTTP.GetEndpointAddress(options, true)));
         }

         host.AddServiceEndpoint(new UdpDiscoveryEndpoint(UdpDiscoveryEndpoint.DefaultIPv4MulticastAddress));

         var discoveryBehavior = new EndpointDiscoveryBehavior();
         discoveryBehavior.Scopes.Add(new Uri($"id:{options.ProcessID}"));
         foreach (var endpoint in contractEndpoints)
         {
            endpoint.EndpointBehaviors.Add(discoveryBehavior);
         }
      }
      public static List<Uri> GetBaseAddresses(this ConnectionOptions options)
      {
         List<Uri> baseAddresses = new List<Uri>();

         if (options.Scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
         {
            baseAddresses.Add(new Uri(BindingScheme.NAMED_PIPE.GetEndpointAddress(options, false)));
         }

         if (options.Scheme.IsBindingScheme(BindingScheme.TCP))
         {
            baseAddresses.Add(new Uri(BindingScheme.TCP.GetEndpointAddress(options, false)));
         }

         if (options.Scheme.IsBindingScheme(BindingScheme.HTTP))
         {
            baseAddresses.Add(new Uri(BindingScheme.HTTP.GetEndpointAddress(options, false)));
         }

         return baseAddresses;
      }

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
            return $"{transport}://{serverHostName}/Codex/{options.ProcessID}/mex";
         else
            return $"{transport}://{serverHostName}/Codex/{options.ProcessID}/IPCService";

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
