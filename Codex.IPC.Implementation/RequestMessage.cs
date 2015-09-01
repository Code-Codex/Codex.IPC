using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    [DataContract]
    public class RequestMessage:MessageBase
    {
        [DataMember]
        public RequestMessageHeader Header { get; set; }

        public RequestMessage()
        {
            Header = new RequestMessageHeader();
            Header.ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
        }
    }
}
