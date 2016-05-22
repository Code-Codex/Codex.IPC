using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.DataTypes
{
    /// <summary>
    /// Header information for the response packet.
    /// </summary>
    [DataContract]
    public class ResponseMessageHeader
    {
        /// <summary>
        /// Header information for the request packet.
        /// </summary>
        [DataMember]
        public RequestMessageHeader RequestHeader { get; set; }

        public ResponseMessageHeader(RequestMessageHeader requestHeader)
        {
            RequestHeader = requestHeader;
        }

    }
}
