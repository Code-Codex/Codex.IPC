using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Codex.IPC.Implementation
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IIPC
    {
        [OperationContract]
        ResponseMessage Call(RequestMessage request);

        [OperationContract(IsOneWay =true)]
        void Send(RequestMessage request);
    }

}
