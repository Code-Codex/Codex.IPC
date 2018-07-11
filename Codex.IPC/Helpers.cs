using Codex.IPC.Contracts;
using Codex.IPC.DataTypes;
using Codex.IPC.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace Codex.IPC
{
   public static class Helpers
   {
      internal static void InitializeHost(this ServiceHost host, ServerOptions options)
      {
         // Check to see if the service host already has a ServiceMetadataBehavior
         ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
         // If not, add one
         if (smb == null)
         {
            smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
         }

         if (options.EnableDiscovery)
         {
            // Check to see if the service host already has a ServiceDiscoveryBehavior
            ServiceDiscoveryBehavior sdiscb = host.Description.Behaviors.Find<ServiceDiscoveryBehavior>();
            // If not, add one
            if (sdiscb == null)
            {
               sdiscb = new ServiceDiscoveryBehavior();
               host.Description.Behaviors.Add(sdiscb);
            }
         }

         // Check to see if the service host already has a ServiceDebugBehavior
         ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
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
            NetTcpBinding tcpBinding = (NetTcpBinding)BindingScheme.TCP.GetBinding(options);
            host.AddServiceEndpoint(typeof(IIPC), tcpBinding, "");
            host.AddServiceEndpoint(typeof(IIPCDuplex), tcpBinding, "");
            contractEndpoints.Add(host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), BindingScheme.TCP.GetEndpointAddress(options, true)));
         }

         if (options.Scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
         {
            NetNamedPipeBinding namedPipeBinding = (NetNamedPipeBinding)BindingScheme.NAMED_PIPE.GetBinding(options);
            host.AddServiceEndpoint(typeof(IIPC), namedPipeBinding, "");
            host.AddServiceEndpoint(typeof(IIPCDuplex), namedPipeBinding, "");
            contractEndpoints.Add(host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), BindingScheme.NAMED_PIPE.GetEndpointAddress(options, true)));
         }

         if (options.EnableDiscovery)
         {
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint(UdpDiscoveryEndpoint.DefaultIPv4MulticastAddress));

            EndpointDiscoveryBehavior discoveryBehavior = new EndpointDiscoveryBehavior();
            discoveryBehavior.Scopes.Add(new Uri($"id:{options.ProcessID}".ToLower()));
            foreach (KeyValuePair<string, string> scope in options.Scopes)
            {
               discoveryBehavior.Scopes.Add(new Uri($"{scope.Key}:{scope.Value}".ToLower()));
            }

            foreach (ServiceEndpoint endpoint in contractEndpoints)
            {
               endpoint.EndpointBehaviors.Add(discoveryBehavior);
            }
         }
      }
      internal static List<Uri> GetBaseAddresses(this IConnectionOptions options)
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

         return baseAddresses;
      }

      internal static bool IsBindingScheme(this BindingScheme scheme, BindingScheme schemeToCheck)
      {
         return (scheme & schemeToCheck) == schemeToCheck;
      }

      internal static string GetEndpointAddress(this BindingScheme scheme, IConnectionOptions options, bool isMex = false)
      {
         string transport = string.Empty;
         string serverHostName = options.HostName;
         switch (scheme)
         {
            case BindingScheme.NAMED_PIPE:
               transport = "net.pipe";
               break;
            case BindingScheme.TCP:
               transport = "net.tcp";
               serverHostName = $"{options.HostName}:{options.TCPPort}";
               break;
         }

         if (isMex)
         {
            return $"{transport}://{serverHostName}/Codex/{options.ProcessID}/mex";
         }
         else
         {
            return $"{transport}://{serverHostName}/Codex/{options.ProcessID}/IPCService";
         }
      }

      internal static Binding GetBinding(this BindingScheme scheme, IConnectionOptions options)
      {
         Binding binding = null;
         switch (scheme)
         {
            case BindingScheme.TCP:
               {
                  binding = new NetTcpBinding(SecurityMode.None);
                  NetTcpBinding tcpBinding = ((NetTcpBinding)binding);
                  tcpBinding.MaxBufferPoolSize = Constants.MAX_MSG_SIZE;
                  tcpBinding.MaxReceivedMessageSize = Constants.MAX_MSG_SIZE;
                  break;
               }
            case BindingScheme.NAMED_PIPE:
               {
                  binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                  NetNamedPipeBinding npBinding = ((NetNamedPipeBinding)binding);
                  npBinding.MaxBufferPoolSize = Constants.MAX_MSG_SIZE;
                  npBinding.MaxReceivedMessageSize = Constants.MAX_MSG_SIZE;
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


      /// <summary>
      /// Get the options to connect to the server based on the exposed endpoints
      /// </summary>
      public static List<IConnectionOptions> GetConnectionOptions(IEnumerable<EndpointAddress> endpointAddresses)
      {
         List<IConnectionOptions> options = new List<IConnectionOptions>();
         IEnumerable<IGrouping<string, EndpointAddress>> groupedEndpoints = endpointAddresses.GroupBy(x => x.Uri.Host);
         foreach (IGrouping<string, EndpointAddress> hostGroup in groupedEndpoints)
         {
            IEnumerable<IGrouping<string, EndpointAddress>> processGroup = hostGroup.GroupBy(x => getProcessID(x.Uri));
            foreach (IGrouping<string, EndpointAddress> grp in processGroup)
            {
               ConnectionOptions connOption = new ConnectionOptions(grp.Key);
               connOption.HostName = hostGroup.Key;
               var portSchemeMap = grp.Select(x => new { scheme = GetEnumFromDescription<BindingScheme>(x.Uri.Scheme), port = x.Uri.Port });
               connOption.Scheme = portSchemeMap.Select(x => x.scheme).Aggregate((x, y) => x | y);
               foreach (var map in portSchemeMap)
               {
                  if (map.scheme == BindingScheme.TCP)
                  {
                     connOption.TCPPort = (uint)map.port;
                  }
               }
               options.Add(connOption);
            }
         }

         return options;
      }


      /// <summary>
      /// Get the options to connect to the server based on the discovery result
      /// </summary>
      public static List<IConnectionOptions> GetConnectionOptions(FindResponse searchResponse)
      {
         return Helpers.GetConnectionOptions(searchResponse.Endpoints.Select(x => x.Address));
      }

      /// <summary>
      /// Parse the process identifier from the endpoint URI
      /// </summary>
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
         {
            return null;
         }
         else
         {
            return (Attribute.GetCustomAttribute(info, typeof(T))) as T;
         }
      }


      /// <summary>
      /// Gets the custom attributes
      /// </summary>
      public static T[] GetCustomAttributes<T>(this object value, AttributeTargets target) where T : Attribute
      {
         MemberInfo info = getMemberInfoForAttribute(value, target);
         if (info == null)
         {
            return null;
         }
         else
         {
            return (Attribute.GetCustomAttributes(info, typeof(T))) as T[];
         }
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
         Type type = typeof(T);
         if (!type.IsEnum)
         {
            throw new ArgumentException();
         }

         FieldInfo[] fields = type.GetFields();
         var field = fields
                         .SelectMany(f => f.GetCustomAttributes(
                             typeof(DescriptionAttribute), false),
                             (f, a) => new { Field = f, Att = a })
                             .SingleOrDefault(a => string.Equals(((DescriptionAttribute)a.Att).Description, description, StringComparison.OrdinalIgnoreCase));
         if (field == null)
         {
            throw new ArgumentException($"Enum with description ({description}) not found");
         }

         return (T)field.Field.GetRawConstantValue();
      }
   }
}
