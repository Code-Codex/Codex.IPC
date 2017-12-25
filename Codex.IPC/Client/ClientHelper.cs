using Codex.IPC.Client;
using Codex.IPC.DataTypes;
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
   }
}
