using Codex.IPC.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Codex.IPC.Server
{
    public class IPCService : IIPC
    {

        public static event EventHandler<MessageRecievedEventArgs> OnMessageRecieved = delegate { };

        public ResponseMessage Call(RequestMessage request)
        {
            var arg = new MessageRecievedEventArgs(request);
            OnMessageRecieved(null,arg);
            return arg.Response;
        }

        public void Send(RequestMessage request)
        {
            Call(request);
        }

        public class MessageRecievedEventArgs:EventArgs
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
