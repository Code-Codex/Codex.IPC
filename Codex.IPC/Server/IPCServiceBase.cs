using Codex.IPC.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Codex.IPC.Contracts;

namespace Codex.IPC.Server
{
   /// <summary>
   /// This singleton class represent the IPC service.
   /// </summary>
   /// <remarks>
   /// This implements both simplex and duplex interfaces.
   /// </remarks>
   public abstract class IPCServiceBase : IIPC, IIPCDuplex
   {
      private ConcurrentDictionary<string, IIPCDuplexCallback> _subscriptions;


      public IPCServiceBase()
      {
         _subscriptions = new ConcurrentDictionary<string, IIPCDuplexCallback>();
      }

      /// <summary>
      /// Call message from the client requesting information.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      /// <returns>Response</returns>
      public abstract ResponseMessage Call(RequestMessage request);

      /// <summary>
      /// Send message from the client.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      /// <remarks>
      /// This should be used either when you need a one way notification.
      /// </remarks>
      public abstract void Post(RequestMessage request);

      /// <summary>
      /// Send message from the client.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      /// <remarks>
      /// This should be used either when you need a one way notification or an out of band reply.
      /// </remarks>
      public abstract void Send(RequestMessage request);

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
         request.Header.MessageType = (int)MessageType.SUBSCRIBE;
         if (!_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
            _subscriptions.TryAdd(request.Header.ProcessID.ToString(), callback);
      }


      /// <summary>
      /// UnSubscribe message from the client.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      public void UnSubscribe(RequestMessage request)
      {
         request.Header.MessageType = (int)MessageType.UNSUBSCRIBE;
         IIPCDuplexCallback notUsed = null;
         if (_subscriptions.ContainsKey(request.Header.ProcessID.ToString()))
            _subscriptions.TryRemove(request.Header.ProcessID.ToString(), out notUsed);
      }

      /// <summary>
      /// Reply from the server to the client.
      /// </summary>
      /// <param name="clientID">Unique ID for the clients response channel.</param>
      /// <param name="response">Response</param>
      public void SendReply(string clientID, ResponseMessage response)
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
               IIPCDuplexCallback notUsed = null;
               _subscriptions.TryRemove(clientID, out notUsed);
            }
         }
      }

      /// <summary>
      /// Broadcast a message to all clients.
      /// </summary>
      /// <param name="response">Response message</param>
      /// <remarks>Only works with duplex clients</remarks>
      public void Broadcast(ResponseMessage response)
      {
         IIPCDuplexCallback notUsed = null;
         foreach (var replyChannel in _subscriptions)
         {
            try
            {
               replyChannel.Value.Reply(response);
            }
            catch (Exception ex)
            {
               Trace.WriteLine($"Reply to {response.Header.RequestHeader.ProcessID} failed with error: {ex.Message}");
               _subscriptions.TryRemove(replyChannel.Key, out notUsed);
            }
         }
      }
   }
}
