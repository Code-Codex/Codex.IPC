using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    /// <summary>
    /// Message sent in response to a request.
    /// </summary>
    [DataContract]
    public class ResponseMessage:MessageBase
    {
        /// <summary>
        /// Header information of the message packet.
        /// </summary>
        [DataMember]
        public ResponseMessageHeader Header { get; set; }

        
        public ResponseMessage(RequestMessageHeader requestHeader)
        {
            Header = new ResponseMessageHeader(requestHeader);
        }

    }
}
