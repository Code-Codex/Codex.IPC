using Codex.IPC.Implementation;
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

namespace IPTestClient
{

    class Program
    {
        static Thread _IPCClientThread;
        static IPCDuplexClient _client;
        static int _serverProcId;

        static void Main(string[] args)
        {
            _serverProcId = int.Parse(args[0]);
            ManualResetEvent resetEvent = new ManualResetEvent(false);

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
            var processAddress = $"net.pipe://localhost/Design_Time_Addresses/Codex/{_serverProcId}/IPCService";
            _client = new IPCDuplexClient(instanceContext, "NamedPipeBinding_IIPCDuplex", processAddress);
            _client.Open();
            var requestMessage = new RequestMessage();
            requestMessage.SetBody<RegisterMessage>(new RegisterMessage() { Counter =  CounterType.MEMORY });
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
            Trace.WriteLine($"{data.Type} - {data.Value}");
        }
    }
}
