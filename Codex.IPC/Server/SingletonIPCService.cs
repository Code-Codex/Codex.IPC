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
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
   public class SingleonIPCService : IPCServiceBase
   {
      public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved = delegate { };
      private static object _syncRoot = new object();
      private static SingleonIPCService _service = null;

      /// <summary>
      /// Singleton instance of the service.
      /// </summary>
      public static SingleonIPCService Instance
      {
         get
         {
            if (_service == null)
            {
               lock (_syncRoot)
               {
                  if (_service == null)
                     _service = new SingleonIPCService();
               }
            }
            return _service;
         }
      }

      private SingleonIPCService()
         :base()
      {
      }

      /// <summary>
      /// Call message from the client requesting information.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      /// <returns>Response</returns>
      public override ResponseMessage Call(RequestMessage request)
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
      /// This should be used either when you need a one way notification.
      /// </remarks>
      public override void Post(RequestMessage request)
      {
         var arg = new MessageRecievedEventArgs(request);
         OnMessageRecieved(null, arg);
      }

      /// <summary>
      /// Subscribe message from the client.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      /// <remarks>
      /// The client subscribes to the events from the server.
      /// This may include client specific events or general broadcasts.
      /// </remarks>
      public override void Subscribe(RequestMessage request)
      {
         base.Subscribe(request);
         OnMessageRecieved(null, new MessageRecievedEventArgs(request));
      }


      /// <summary>
      /// UnSubscribe message from the client.
      /// </summary>
      /// <param name="request">Object representing the requested information</param>
      public override void UnSubscribe(RequestMessage request)
      {
         base.UnSubscribe(request);
         OnMessageRecieved(null, new MessageRecievedEventArgs(request));
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
