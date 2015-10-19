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
using Codex.IPC;

namespace IPTestClient
{

    class Program
    {
        static Thread _IPCClientThread;
        static IPCDuplexClient _client;
        static int _serverProcId;
        static CounterType _counterType;

        static void Main(string[] args)
        {
            _serverProcId = int.Parse(args[0]);
            _counterType = (CounterType)int.Parse(args[1]);

            if(args.Count() == 3)
                _counterType |= (CounterType)int.Parse(args[2]);
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
            _client = ClientHelper.GetDuplexClient(instanceContext,_serverProcId.ToString(), BindingScheme.TCP,"localhost",64000);
            _client.Open();
            var requestMessage = new RequestMessage();
            var registerMessage = new RegisterMessage() { Counter = _counterType };
            Trace.WriteLine(registerMessage.Counter.ToString());
            requestMessage.SetBody<RegisterMessage>(registerMessage);
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
