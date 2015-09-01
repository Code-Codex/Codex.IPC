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
    [DataContract]
    public class ResponseMessage
    {
        [DataMember]
        public ResponseMessageHeader Header { get; set; }

    }
}
