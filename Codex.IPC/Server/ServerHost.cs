using Codex.IPC.DataTypes;
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
   public class ServerHost

   {
      /// <summary>
      /// Start the IPC server from the instance of the provided service type.
      /// </summary>
      /// <param name="serviceClassType">Type of the ervice class to instantiate</param>
      /// <param name="resetEvent">Reset event to gracefully shutdown the server.</param>
      /// <param name="options">Connections options for the server.</param>
      public void Start(Type serviceClassType, ManualResetEvent resetEvent, ServerOptions options)
      {
         List<Uri> baseAddresses = options.GetBaseAddresses();
         using (var host = new ServiceHost(serviceClassType, baseAddresses.ToArray()))
         {
            host.InitializeHost(options);

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


      /// <summary>
      /// Start the IPC server from the instance
      /// </summary>
      /// <param name="serviceInstance">Inaance of the service class</param>
      /// <param name="resetEvent">Reset event to gracefully shutdown the server.</param>
      /// <param name="options">Connections options for the server.</param>
      public void Start(IPCServiceBase serviceInstance, ManualResetEvent resetEvent, ServerOptions options)
      {
         List<Uri> baseAddresses = options.GetBaseAddresses();
         using (var host = new ServiceHost(serviceInstance, baseAddresses.ToArray()))
         {
            host.InitializeHost(options);

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
