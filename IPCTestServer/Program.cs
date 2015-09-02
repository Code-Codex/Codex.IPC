using Codex.IPC.Implementation;
using Codex.IPC.Server;
using Codex.IPC.Shmem;
using IPCTestCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPCTestServer
{
    class Program
    {
        static Thread _IPCServerThread, _replyThread;
        static Dictionary<int, Tuple<RequestMessageHeader, CounterType>> _clientProcIds = new Dictionary<int, Tuple<RequestMessageHeader, CounterType>>();
        static PerformanceCounter _cpuCounter;
        static PerformanceCounter _ramCounter;


        static void Main(string[] args)
        {
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
            var host = new Server();
            IPCService.Instance.OnMessageRecieved += IPCService_OnMessageRecieved;
            host.Start(resetEvent, Process.GetCurrentProcess().Id.ToString());
        }

        static void ReplyThreadLoop()
        {
            while (true)
            {
                if (_clientProcIds.Count > 0)
                {
                    foreach (var client in _clientProcIds)
                    {
                        var response = new ResponseMessage();
                        response.Header = new ResponseMessageHeader(client.Value.Item1);
                        if ((client.Value.Item2 & CounterType.CPU) == CounterType.CPU)
                        {
                            var reply = new CounterData() { Type = CounterType.CPU, Value = getCurrentCpuUsage() };
                            response.SetBody<CounterData>(reply);
                            IPCService.Instance.SendReply(response);
                        }
                        if ((client.Value.Item2 & CounterType.MEMORY) == CounterType.MEMORY)
                        {
                            var reply = new CounterData() { Type = CounterType.MEMORY, Value = getAvailableRAM() };
                            response.SetBody<CounterData>(reply);
                            IPCService.Instance.SendReply(response);
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }


        private static void IPCService_OnMessageRecieved(object sender, IPCService.MessageRecievedEventArgs e)
        {
            try
            {
                Trace.WriteLine($"Message recieved from {e.Request.Header.ProcessID}: {e.Request.Header.MessageType}");
                if (e.Request.Header.MessageType == 0)
                {
                    var body = e.Request.GetBody<RegisterMessage>();
                    Trace.WriteLine($"Counter type: {body.Counter}");
                    if (!_clientProcIds.ContainsKey(e.Request.Header.ProcessID))
                        _clientProcIds[e.Request.Header.ProcessID] = new Tuple<RequestMessageHeader, CounterType>(e.Request.Header, body.Counter);
                }
                else
                    _clientProcIds.Remove(e.Request.Header.ProcessID);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

    }
}
