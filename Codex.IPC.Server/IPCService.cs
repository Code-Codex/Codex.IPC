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
    /// <summary>
    /// This singleton class represent the IPC service.
    /// </summary>
    /// <remarks>
    /// This implements both simplex and duplex interfaces.
    /// </remarks>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class IPCService : IIPC, IIPCDuplex
    {
        Dictionary<string, IIPCDuplexCallback> _subscriptions;
        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved = delegate { };
        private static object _syncRoot = new object();
        private static IPCService _service = null;

        /// <summary>
        /// Singleton instance of the service.
        /// </summary>
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

        /// <summary>
        /// Call message from the client requesting information.
        /// </summary>
        /// <param name="request">Object representing the requested information</param>
        /// <returns>Response</returns>
        public ResponseMessage Call(RequestMessage request)
        {
            var arg = new MessageRecievedEventArgs(request);
            OnMessageRecieved(null, arg);
            return arg.Response;
        }

        /// <summary>
        /// Send message from the client.
        /// </summary>
        /// <param name="request">Object representing the requested information</param>
        /// <remarks>
        /// This should be used either when you need a one way notification or an out of band reply.
        /// </remarks>
        public void Send(RequestMessage request)
        {
            IIPCDuplexCallback callback = OperationContext.Current.GetCallbackChannel<IIPCDuplexCallback>();
            var response = Call(request);
            if (callback != null)
                callback.Reply(response);
        }

        /// <summary>
        /// Subscribe message from the client.
        /// </summary>
        /// <param name="request">Object representing the requested information</param>
        /// <remarks>
        /// The client subscribes to the events from the server.
        /// This may include client specific events or general broadcasts.
        /// </remarks>
        public void Subscribe(RequestMessage request)
        {
            IIPCDuplexCallback callback = OperationContext.Current.GetCallbackChannel<IIPCDuplexCallback>();
            if (!_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
                _subscriptions.Add(request.Header.ProcessID.ToString(), callback);
            OnMessageRecieved(null, new MessageRecievedEventArgs(request));
        }


        /// <summary>
        /// UnSubscribe message from the client.
        /// </summary>
        /// <param name="request">Object representing the requested information</param>
        public void UnSubscribe(RequestMessage request)
        {
            if (_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
                _subscriptions.Remove(request.Header.ProcessID.ToString());
            OnMessageRecieved(null, new MessageRecievedEventArgs(request));
        }

        /// <summary>
        /// Reply from the server to the client.
        /// </summary>
        /// <param name="clientID">Unique ID for the clients response channel.</param>
        /// <param name="response">Response</param>
        public void SendReply(string clientID,ResponseMessage response)
        {
            if (_subscriptions.ContainsKey(clientID))
            {
                var replyChannel = _subscriptions[clientID];

                try
                {
                    replyChannel.Reply(response);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Reply to {response.Header.RequestHeader.ProcessID} failed with error: {ex.Message}");
                    _subscriptions.Remove(clientID);
                }
            }
        }

        /// <summary>
        /// Broadcast a message to all clients.
        /// </summary>
        /// <param name="response">Response message</param>
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
                Response = new ResponseMessage(Request.Header);
            }
        }
    }
}
