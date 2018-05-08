using Codex.IPC.Client;
using Codex.IPC.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
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
      /// <param name="options">Connection Options</param>
      /// <returns>Client object.</returns>
      public static IPCClient GetClient(ConnectionOptions options, BindingScheme scheme = BindingScheme.NAMED_PIPE)
      {
         return new IPCClient(scheme.GetBinding(options), new EndpointAddress(scheme.GetEndpointAddress(options, false)));
      }

      /// <summary>
      /// Gets a duplex client
      /// </summary>
      /// <param name="options">Connection Options</param>
      /// <returns>Client object.</returns>
      public static IPCDuplexClient GetDuplexClient(InstanceContext context, ConnectionOptions options, BindingScheme scheme = BindingScheme.NAMED_PIPE)
      {
         return new IPCDuplexClient(context, scheme.GetBinding(options), new EndpointAddress(scheme.GetEndpointAddress(options, false)));
      }

      /// <summary>
      /// Finds the list of servers with the provided filter criteria
      /// </summary>
      /// <param name="serverId">Identifier for the server</param>
      /// <param name="filterCriteria">filter criteria</param>
      /// <returns>Find results</returns>
      public static async Task<FindResponse> FindServersAsync(string serverId, Dictionary<string,string> filterCriteria)
      {
         var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint(UdpDiscoveryEndpoint.DefaultIPv4MulticastAddress));
         var findCriteria = FindCriteria.CreateMetadataExchangeEndpointCriteria(typeof(IIPC));
         findCriteria.Scopes.Add(new Uri($"id:{serverId}"));
         if(filterCriteria != null)
         {
            foreach(var entry in filterCriteria)
               findCriteria.Scopes.Add(new Uri($"{entry.Key}:{entry.Value}"));
         }

         return await discoveryClient.FindTaskAsync(findCriteria);
      }
   }
}
