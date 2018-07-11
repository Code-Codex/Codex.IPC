using Codex.IPC.DataTypes;
using Codex.IPC.Server;
using IPCTestCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codex.IPC;
using CommandLine;

namespace IPCTestServer
{
   class Program
   {
      static Thread _IPCServerThread, _replyThread;
      static Dictionary<string, Tuple<RequestMessageHeader, CounterType>> _clientProcIds = new Dictionary<string, Tuple<RequestMessageHeader, CounterType>>();
      static PerformanceCounter _cpuCounter;
      static PerformanceCounter _ramCounter;
      private static object _syncLock = new object();
      public static CommandOptions _options;


      static int Main(string[] args)
      {
         bool exit = false;
         var result = Parser.Default.ParseArguments<CommandOptions>(args);
         result.WithParsed(op => _options = op)
               .WithNotParsed(errors =>
               {
                  Console.WriteLine("invalid command line params");
                  exit = true;
               });

         if (exit)
            return 1;

         ManualResetEvent resetEvent = new ManualResetEvent(false);
         _cpuCounter = new PerformanceCounter();

         _cpuCounter.CategoryName = "Processor";
         _cpuCounter.CounterName = "% Processor Time";
         _cpuCounter.InstanceName = "_Total";
         _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

         _IPCServerThread = new Thread(ServerThreadLoop);
         _IPCServerThread.Start(resetEvent);

         _replyThread = new Thread(ReplyThreadLoop);
         _replyThread.Start();

         Console.ReadLine();
         resetEvent.Set();
         return 0;
      }

      public static float getCurrentCpuUsage()
      {
         return _cpuCounter.NextValue();
      }

      public static float getAvailableRAM()
      {
         return _ramCounter.NextValue();
      }

      static void ServerThreadLoop(object mrevent)
      {
         ManualResetEvent resetEvent = (ManualResetEvent)mrevent;
         var host = new ServerHost();
         SingleonIPCService.Instance.OnMessageRecieved += IPCService_OnMessageRecieved;
         BindingScheme schemes = BindingScheme.NAMED_PIPE;
         
         foreach (var sch in _options.BindingScheme)
         {
            switch(sch)
            {
               case 't':
                  schemes |= BindingScheme.TCP;
                  break;
               case 'p':
                  schemes |= BindingScheme.NAMED_PIPE;
                  break;

            }
         }

         host.Start(SingleonIPCService.Instance, resetEvent, new ConnectionOptions(_options.ServerName) { Scheme = schemes, EnableDiscovery = true });
      }

      static void ReplyThreadLoop()
      {
         while (true)
         {
            if (_clientProcIds.Count > 0)
            {
               lock (_syncLock)
               {
                  var cpu = getCurrentCpuUsage();
                  var ram = getAvailableRAM();
                  foreach (var client in _clientProcIds)
                  {
                     var response = new ResponseMessage(client.Value.Item1);
                     if ((CounterType.CPU & client.Value.Item2) == CounterType.CPU)
                     {
                        var reply = new CounterData() { Type = CounterType.CPU, Value = cpu };
                        response.SetBody<CounterData>(reply);
                        SingleonIPCService.Instance.SendReply(response.Header.RequestHeader.ProcessID.ToString(), response);
                     }
                     if ((CounterType.MEMORY & client.Value.Item2) == CounterType.MEMORY)
                     {
                        var reply = new CounterData() { Type = CounterType.MEMORY, Value = ram };
                        response.SetBody<CounterData>(reply);
                        SingleonIPCService.Instance.SendReply(response.Header.RequestHeader.ProcessID.ToString(), response);
                     }
                  }
               }
            }
            Thread.Sleep(1000);
         }
      }


      private static void IPCService_OnMessageRecieved(object sender, SingleonIPCService.MessageRecievedEventArgs e)
      {
         try
         {
            Console.WriteLine($"Message received from {e.Request.Header.ProcessID}: {e.Request.Header.MessageType}");
            if (e.Request.Header.MessageType == (int)MessageType.SUBSCRIBE)
            {
               var body = e.Request.GetBody<RegisterMessage>();
               Console.WriteLine($"Counter type: {body.Counter}");
               if (!_clientProcIds.ContainsKey(e.Request.Header.ProcessID))
               {
                  lock (_syncLock)
                  {
                     _clientProcIds[e.Request.Header.ProcessID] =
                        new Tuple<RequestMessageHeader, CounterType>(e.Request.Header, body.Counter);
                  }
               }
            }
            else
            {
               _clientProcIds.Remove(e.Request.Header.ProcessID);
            }

         }
         catch (Exception ex)
         {
            Trace.WriteLine(ex.Message);
         }
      }

   }
}
