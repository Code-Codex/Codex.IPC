using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    [DataContract]
    public class ResponseMessageHeader
    {
        [DataMember]
        public RequestMessageHeader RequestHeader { get; set; }

        public ResponseMessageHeader(RequestMessageHeader requestHeader)
        {
            RequestHeader = requestHeader;
        }

    }
}
