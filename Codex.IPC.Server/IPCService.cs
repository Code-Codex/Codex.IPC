using Codex.IPC.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Codex.IPC.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class IPCService : IIPC, IIPCDuplex
    {
        Dictionary<string, IIPCDuplexCallback> _subscriptions;
        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved = delegate { };
        private static object _syncRoot = new object();
        private static IPCService _service = null;

        public static IPCService Instance
        {
            get
            {
                if (_service == null)
                {
                    lock (_syncRoot)
                    {
                        if (_service == null)
                            _service = new IPCService();
                    }
                }
                return _service;
            }
        }

        private IPCService()
        {
            _subscriptions = new Dictionary<string, IIPCDuplexCallback>();
        }

        public ResponseMessage Call(RequestMessage request)
        {
            var arg = new MessageRecievedEventArgs(request);
            OnMessageRecieved(null, arg);
            return arg.Response;
        }

        public void Send(RequestMessage request)
        {
            IIPCDuplexCallback callback = OperationContext.Current.GetCallbackChannel<IIPCDuplexCallback>();
            var response = Call(request);
            if (callback != null)
                callback.Reply(response);
        }

        public void Subscribe(RequestMessage request)
        {
            IIPCDuplexCallback callback = OperationContext.Current.GetCallbackChannel<IIPCDuplexCallback>();
            if (!_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
                _subscriptions.Add(request.Header.ProcessID.ToString(), callback);
            OnMessageRecieved(null, new MessageRecievedEventArgs(request));
        }

        public void UnSubscribe(RequestMessage request)
        {
            if (_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
                _subscriptions.Remove(request.Header.ProcessID.ToString());
            OnMessageRecieved(null, new MessageRecievedEventArgs(request));
        }

        public void SendReply(string serverID,ResponseMessage response)
        {
            if (_subscriptions.ContainsKey(serverID))
            {
                var replyChannel = _subscriptions[serverID];

                try
                {
                    replyChannel.Reply(response);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Reply to {response.Header.RequestHeader.ProcessID} failed with error: {ex.Message}");
                    _subscriptions.Remove(serverID);
                }
            }

        }

        public void Broadcast(ResponseMessage response)
        {
            List<string> invalidChannels = new List<string>();
            foreach (var replyChannel in _subscriptions)
            {
                try
                {
                    replyChannel.Value.Reply(response);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Reply to {response.Header.RequestHeader.ProcessID} failed with error: {ex.Message}");
                    invalidChannels.Add(replyChannel.Key);
                }
            }

            foreach (var item in invalidChannels)
                _subscriptions.Remove(item);
        }

        public class MessageRecievedEventArgs : EventArgs
        {
            public ResponseMessage Response { get; set; }
            public RequestMessage Request { get; set; }

            public MessageRecievedEventArgs(RequestMessage request)
            {
                Request = request;
                Response = new ResponseMessage();
                Response.Header = new ResponseMessageHeader(Request.Header);
            }
        }
    }
}
