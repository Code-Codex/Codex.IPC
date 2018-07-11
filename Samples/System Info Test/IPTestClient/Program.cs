using Codex.IPC;
using Codex.IPC.Client;
using Codex.IPC.Contracts;
using Codex.IPC.DataTypes;
using IPCTestCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using CommandLine;

namespace IPTestClient
{
   internal class Program
   {
      private static Thread _IPCClientThread;
      private static IPCDuplexClient _client;
      private static string _serverProcId;
      private static CounterType _counterType;
      private static ManualResetEvent resetEvent;

      private static int Main(string[] args)
      {
         _serverProcId = "IPCTestServer";
         bool exit = false;
         var result = Parser.Default.ParseArguments<CommandOptions>(args);
         result.WithParsed(op =>
         {
            switch (op.CounterType)
            {
               case 'c':
               case 'C':
                  _counterType = CounterType.CPU;
                  break;
               case 'm':
               case 'M':
                  _counterType = CounterType.MEMORY;
                  break;
               case 'a':
               case 'A':
                  _counterType = CounterType.CPU | CounterType.MEMORY;
                  break;
            }
         })
         .WithNotParsed(errors =>
         {
            Console.WriteLine("invalid command line params");
            exit = true;
         });

         if (exit)
         {
            return 1;
         }

         resetEvent = new ManualResetEvent(false);

         Console.WriteLine("Searching servers...\n");

         FindResponse findResponse = ClientHelper.FindServersAsync(_serverProcId, null).Result;

         if (findResponse != null)
         {
            List<(Codex.IPC.Interfaces.IConnectionOptions ConnectionOption, Dictionary<string, string> Scopes)> options = Helpers.GetConnectionOptions(findResponse);
            int index = 1;
            foreach ((Codex.IPC.Interfaces.IConnectionOptions ConnectionOption, Dictionary<string, string> Scopes) opt in options)
            {
               Console.WriteLine($"{index}. Host: {opt.ConnectionOption.HostName}.");
               Console.WriteLine("\tScopes:");
               foreach (KeyValuePair<string, string> scope in opt.Scopes)
               {
                  Console.WriteLine($"\t{scope.Key}: {scope.Value}.");
               }

               index++;
            }

            Console.Write("Select the index of server to connect to: ");
            if (int.TryParse(Console.ReadLine(), out int selectedIndex))
            {
               _IPCClientThread = new Thread(ClientThreadLoop);
               _IPCClientThread.Start(options[selectedIndex - 1].ConnectionOption);
            }
         }

         Console.ReadLine();
         resetEvent.Set();
         return 0;
      }

      private static void ClientThreadLoop(object connOptions)
      {
         // Construct InstanceContext to handle messages on callback interface
         InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
         _client = ClientHelper.GetDuplexClient(instanceContext, (ConnectionOptions)connOptions, BindingScheme.TCP);
         _client.Open();
         RequestMessage requestMessage = new RequestMessage();
         RegisterMessage registerMessage = new RegisterMessage { Counter = _counterType };
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
         CounterData data = response.GetBody<CounterData>();
         Console.WriteLine($"{data.Type,-10} - {data.Value}");
      }
   }
}
