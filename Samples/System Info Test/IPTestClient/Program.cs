using Codex.IPC.DataTypes;
using Codex.IPC.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IPCTestCommon;
using Codex.IPC;
using Codex.IPC.Contracts;
using System.ServiceModel.Discovery;

namespace IPTestClient
{

   class Program
   {
      static Thread _IPCClientThread;
      static IPCDuplexClient _client;
      static string _serverProcId;
      static CounterType _counterType;

      static void Main(string[] args)
      {
         _serverProcId = "IPCTestServer";
         if (args.Any())
            _counterType = (CounterType)int.Parse(args[0]);
         else
            _counterType = CounterType.CPU;

         if (args.Length == 3)
            _counterType |= (CounterType)int.Parse(args[2]);
         ManualResetEvent resetEvent = new ManualResetEvent(false);

         var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint(UdpDiscoveryEndpoint.DefaultIPv4MulticastAddress));
         var findCriteria = FindCriteria.CreateMetadataExchangeEndpointCriteria(typeof(Codex.IPC.Contracts.IIPC));
         findCriteria.Scopes.Add(new Uri($"id:{_serverProcId}"));
         var findResponse = discoveryClient.Find(findCriteria);

         if (findResponse != null)
         {
            if (findResponse.Endpoints != null)
            {
               foreach (var ep in findResponse.Endpoints)
                  Console.WriteLine(ep.Address.Uri.ToString());
            }
         }

         _IPCClientThread = new Thread(ClientThreadLoop);
         _IPCClientThread.Start(resetEvent);



         Console.ReadLine();
         resetEvent.Set();
      }


      static void ClientThreadLoop(object mrevent)
      {
         ManualResetEvent resetEvent = (ManualResetEvent)mrevent;
         // Construct InstanceContext to handle messages on callback interface
         InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
         _client = ClientHelper.GetDuplexClient(instanceContext, new ConnectionOptions(_serverProcId), BindingScheme.TCP);
         _client.Open();
         var requestMessage = new RequestMessage();
         var registerMessage = new RegisterMessage { Counter = _counterType };
         Trace.WriteLine(registerMessage.Counter.ToString());
         requestMessage.SetBody(registerMessage);
         _client.Subscribe(requestMessage);
         resetEvent.WaitOne();
         _client.Close();

      }
   }


   public class CallbackHandler : IIPCDuplexCallback
   {
      public void Reply(ResponseMessage response)
      {
         var data = response.GetBody<CounterData>();
         Console.WriteLine($"{data.Type} - {data.Value}");
      }
   }
}
