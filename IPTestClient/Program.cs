using Codex.IPC.Implementation;
using Codex.IPC.Server;
using Codex.IPC.Shmem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPTestClient
{
    class Program
    {
        static Thread _IPCThread;
        static Mutex _locMutex;
        static void Main(string[] args)
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            _IPCThread = new Thread(ThreadLoop);
            _IPCThread.Start(resetEvent);
            //Console.ReadLine();
            Mutex.TryOpenExisting("bauerTestMutext", out _locMutex);
            _locMutex.WaitOne();
            _locMutex.ReleaseMutex();
            Console.WriteLine("Mutex released");
            ShmemServer server = ShmemServer.Instance;
            server.Initialize(5024);
            var currProcess = Process.GetCurrentProcess();
            var otherProcesses = Process.GetProcessesByName(currProcess.ProcessName).Where(x => x.Id != currProcess.Id).Select(x => x.Id);
            foreach (var proc in otherProcesses)
            {
                var processAddress = $"net.pipe://localhost/Design_Time_Addresses/Codex/{proc}/IPCService";
                //Console.WriteLine(processAddress);
                using (var client = new Codex.IPC.Client.IPCClient("NamedPipeBinding_IIPC", processAddress))
                {
                    var req = new RequestMessage();
                    req.Header = new RequestMessageHeader();
                    req.Header.ProcessID = currProcess.Id;                  
                    for (int i = 0; i < 1; i++)
                    {
                        req.Header.MessageType = 0;
                        const int temp = 523;
                        var shmemInfo = new ShmemLocation() { Offset = (temp * i), Count = 5 };
                        req.SetBody<ShmemLocation>(shmemInfo);
                        var data = Enumerable.Range(1, 5).Select(x => new RandomStruct(DateTime.Now.AddDays(-x),Guid.NewGuid())).ToArray();
                        
                        server.SetData<RandomStruct>(ShmemServer.GetShmemName(currProcess.Id), shmemInfo.Offset,data);
                        Trace.WriteLine("Sending data:");
                        foreach (var item in data)
                        {
                            Trace.WriteLine(item.ToString());
                        }
                        client.Send(req);
                    }
                }
            }
            Console.ReadLine();
            resetEvent.Set();
            server.Dispose();
        }

        static void ThreadLoop(object mrevent)
        {
            ManualResetEvent resetEvent = (ManualResetEvent)mrevent;
            var host = new Server();
            host.OnMessageRecieved += IPCService_OnMessageRecieved;
            host.Start(resetEvent, Process.GetCurrentProcess().Id.ToString());
        }

        private static void IPCService_OnMessageRecieved(object sender, IPCService.MessageRecievedEventArgs e)
        {
            try {
                Trace.WriteLine($"Message recieved from {e.Request.Header.ProcessID}: {e.Request.Header.MessageType}");
                var body = e.Request.GetBody<ShmemLocation>();
                Trace.WriteLine($"Offset: {body.Offset}, Count: {body.Count}");
                var dataColl = ShmemClient.GetData<RandomStruct>(ShmemServer.GetShmemName(e.Request.Header.ProcessID), body.Offset, body.Count);

                foreach (var item in dataColl)
                {
                    Trace.WriteLine(item.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        [Serializable]
        public struct RandomStruct
        {
            public DateTime TimeStamp { get; set; }
            public Guid GUID { get; set; }

            public RandomStruct(DateTime stamp, Guid uid)
            {
                TimeStamp = stamp;
                GUID = uid;
            }

            public override string ToString()
            {
                return $"Timestamp : {TimeStamp}, GUID : {GUID.ToString()}";
            }
        }

        [Serializable]
        public struct ShmemLocation
        {
            public long Offset { get; set; }
            public int Count { get; set; }
            
        }
    }
}
