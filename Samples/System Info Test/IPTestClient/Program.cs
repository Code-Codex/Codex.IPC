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
      static ManualResetEvent resetEvent;

      static void Main(string[] args)
      {
         _serverProcId = "IPCTestServer";
         if (args.Any())
            _counterType = (CounterType)int.Parse(args[0]);
         else
            _counterType = CounterType.CPU;

         if (args.Length == 3)
            _counterType |= (CounterType)int.Parse(args[2]);

         resetEvent = new ManualResetEvent(false);

         Console.WriteLine("Searching servers...\n");

         var findResponse = ClientHelper.FindServersAsync(_serverProcId, null).Result;

         if (findResponse != null)
         {
            var options = Helpers.GetConnectionOptions(findResponse.Endpoints.Select(x => x.Address).ToList());
            int index = 1;
            foreach(var opt in options)
            {
               Console.WriteLine($"{index}. Host: {opt.HostName}");
               index++;
            }

            Console.Write("Select the index of server to connect to: ");
            if (int.TryParse(Console.ReadLine(), out int selectedIndex))
            {
               _IPCClientThread = new Thread(ClientThreadLoop);
               _IPCClientThread.Start(options[selectedIndex-1]);
            }
         }

         Console.ReadLine();
         resetEvent.Set();
      }


      static void ClientThreadLoop(object connOptions)
      {
         // Construct InstanceContext to handle messages on callback interface
         InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
         _client = ClientHelper.GetDuplexClient(instanceContext, (ConnectionOptions)connOptions, BindingScheme.TCP);
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
