using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    [DataContract]
    public class RequestMessageHeader
    {
        [DataMember]
        public int ProcessID { get; set; }

        [DataMember]
        public int MessageType { get; set; }
    }
}
