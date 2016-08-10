﻿using Codex.IPC.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codex.IPC.Contracts;

namespace Codex.IPC.Server
{
   public class Server

   {
      /// <summary>
      /// Start the IPC server for this process.
      /// </summary>
      /// <param name="resetEvent">Reset event to gracefully shutdown the server.</param>
      /// <param name="processID">Base address for the process.</param>
      /// <param name="options">Connections options for the server.</param>
      /// <param name="scheme">Connection schemes that can be used in the server, this is a flags enum so multiple schemes can be provided.</param>
      public void Start(ManualResetEvent resetEvent, string processID, ConnectionOptions options, BindingScheme scheme)
      {
         List<Uri> baseAddresses = new List<Uri>();

         if (scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
         {
            baseAddresses.Add(new Uri(BindingScheme.NAMED_PIPE.GetEndpointAddress(processID, options, false)));
         }

         if (scheme.IsBindingScheme(BindingScheme.TCP))
         {
            baseAddresses.Add(new Uri(BindingScheme.TCP.GetEndpointAddress(processID, options, false)));
         }

         if (scheme.IsBindingScheme(BindingScheme.HTTP))
         {
            baseAddresses.Add(new Uri(BindingScheme.HTTP.GetEndpointAddress(processID, options, false)));
         }
         using (var host = new ServiceHost(IPCService.Instance, baseAddresses.ToArray()))
         {

            // Check to see if the service host already has a ServiceMetadataBehavior
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
            {
               smb = new ServiceMetadataBehavior();
               host.Description.Behaviors.Add(smb);
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

            // Setup the bindings 
            if (scheme.IsBindingScheme(BindingScheme.TCP))
            {
               var tcpBinding = (NetTcpBinding)BindingScheme.TCP.GetBinding(options);
               host.AddServiceEndpoint(typeof(IIPC), tcpBinding, "");
               host.AddServiceEndpoint(typeof(IIPCDuplex), tcpBinding, "");
               host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), BindingScheme.TCP.GetEndpointAddress(processID, options, true));
            }

            if (scheme.IsBindingScheme(BindingScheme.NAMED_PIPE))
            {
               var namedPipeBinding = (NetNamedPipeBinding)BindingScheme.NAMED_PIPE.GetBinding(options);
               host.AddServiceEndpoint(typeof(IIPC), namedPipeBinding, "");
               host.AddServiceEndpoint(typeof(IIPCDuplex), namedPipeBinding, "");
               host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), BindingScheme.NAMED_PIPE.GetEndpointAddress(processID, options, true));
            }

            if (scheme.IsBindingScheme(BindingScheme.HTTP))
            {
               var httpBinding = (NetHttpBinding)BindingScheme.HTTP.GetBinding(options);
               host.AddServiceEndpoint(typeof(IIPC), httpBinding, "");
               host.AddServiceEndpoint(typeof(IIPCDuplex), httpBinding, "");
               host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), BindingScheme.HTTP.GetEndpointAddress(processID, options, true));
            }

            host.Open();
            Trace.WriteLine("Service up and running at:");
            foreach (var ea in host.Description.Endpoints)
            {
               Trace.WriteLine(ea.Address);
            }

            resetEvent.WaitOne();
            host.Close();
         }
      }

   }
}
