using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
   public static class Helpers
   {
      internal static void InitializeHost(this ServiceHost host, ConnectionOptions options)
      {
         // Check to see if the service host already has a ServiceMetadataBehavior
         var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
         // If not, add one
         if (smb == null)
         {
            smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
         }

         if (options.EnableDiscovery)
         {
            // Check to see if the service host already has a ServiceDiscoveryBehavior
            var sdiscb = host.Description.Behaviors.Find<ServiceDiscoveryBehavior>();
            // If not, add one
            if (sdiscb == null)
            {
               sdiscb = new ServiceDiscoveryBehavior();
               host.Description.Behaviors.Add(sdiscb);
            }
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
      internal static List<Uri> GetBaseAddresses(this ConnectionOptions options)
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

      internal static bool IsBindingScheme(this BindingScheme scheme, BindingScheme schemeToCheck)
      {
         return (scheme & schemeToCheck) == schemeToCheck;
      }

      internal static string GetEndpointAddress(this BindingScheme scheme, ConnectionOptions options, bool isMex = false)
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

      internal static Binding GetBinding(this BindingScheme scheme, ConnectionOptions options)
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

      public static List<ConnectionOptions> GetConnectionOptions(List<EndpointAddress> endpointAddresses)
      {
         List<ConnectionOptions> options = new List<ConnectionOptions>();
         var groupedEndpoints = endpointAddresses.GroupBy(x => x.Uri.Host);
         foreach (var hostGroup in groupedEndpoints)
         {
            var processGroup = hostGroup.GroupBy(x => getProcessID(x.Uri));
            foreach (var grp in processGroup)
            {
               var connOption = new ConnectionOptions(grp.Key);
               connOption.HostName = hostGroup.Key;
               var portSchemeMap = grp.Select(x => new { scheme = GetEnumFromDescription<BindingScheme>(x.Uri.Scheme), port = x.Uri.Port });
               connOption.Scheme = portSchemeMap.Select(x => x.scheme).Aggregate((x, y) => x | y);
               foreach(var map in portSchemeMap)
               {
                  if (map.scheme == BindingScheme.TCP)
                     connOption.TCPPort = (uint)map.port;
                  else if (map.scheme == BindingScheme.HTTP)
                     connOption.HTTPPort = (uint)map.port;
               }
               options.Add(connOption);
            }
         }

         return options;
      }

      private static string getProcessID(Uri endpointURI)
      {
         return endpointURI.Segments[2].Substring(0, endpointURI.Segments[2].Length - 1);
      }

      private static MemberInfo getMemberInfoForAttribute(object value, AttributeTargets target)
      {
         MemberInfo info = null;
         switch (target)
         {
            case AttributeTargets.Class:
               info = value.GetType();
               break;
            case AttributeTargets.Field:
               info = value.GetType().GetField(value.ToString());
               break;
         }
         return info;
      }

      /// <summary>
      /// Gets the custom attribute
      /// </summary>
      public static T GetCustomAttribute<T>(this object value, AttributeTargets target) where T : Attribute
      {
         MemberInfo info = getMemberInfoForAttribute(value, target);
         if (info == null)
            return null;
         else
            return (Attribute.GetCustomAttribute(info, typeof(T))) as T;
      }


      /// <summary>
      /// Gets the custom attributes
      /// </summary>
      public static T[] GetCustomAttributes<T>(this object value, AttributeTargets target) where T : Attribute
      {
         MemberInfo info = getMemberInfoForAttribute(value, target);
         if (info == null)
            return null;
         else
            return (Attribute.GetCustomAttributes(info, typeof(T))) as T[];
      }

      /// <summary>
      /// Gets the description from an Enum
      /// </summary>
      public static string GetDescription(this Enum value)
      {
         DescriptionAttribute attribute = value.GetCustomAttribute<DescriptionAttribute>(AttributeTargets.Field);
         return attribute == null ? value.ToString() : attribute.Description;
      }

      /// <summary>
      /// This method looks for the description attribute on the 
      /// enum and finds the enum with the matching description.
      /// </summary>
      /// <typeparam name="T">Type of the struct</typeparam>
      /// <param name="description">Description text for the enum value</param>
      /// <returns>Enum value</returns>
      public static T GetEnumFromDescription<T>(this string description) where T : struct, IConvertible
      {
         var type = typeof(T);
         if (!type.IsEnum)
            throw new ArgumentException();
         FieldInfo[] fields = type.GetFields();
         var field = fields
                         .SelectMany(f => f.GetCustomAttributes(
                             typeof(DescriptionAttribute), false),
                             (f, a) => new { Field = f, Att = a })
                             .SingleOrDefault(a => String.Equals(((DescriptionAttribute)a.Att).Description,description,StringComparison.OrdinalIgnoreCase));
         if (field == null)
            throw new ArgumentException($"Enum with description ({description}) not found");
         return (T)field.Field.GetRawConstantValue();
      }
   }
}
