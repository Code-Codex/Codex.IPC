using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Codex.IPC.Implementation
{
    [ServiceContract]
    public interface IIPC 
    {
        [OperationContract]
        ResponseMessage Call(RequestMessage request);
    }

    [ServiceContract(CallbackContract = typeof(IIPCDuplexCallback))]
    public interface IIPCDuplex
    {
        [OperationContract(IsOneWay = true)]
        void Send(RequestMessage request);

        [OperationContract(IsOneWay = true)]
        void Subscribe(RequestMessage request);

        [OperationContract(IsOneWay = true)]
        void UnSubscribe(RequestMessage request);
    }

    public interface IIPCDuplexCallback
    {
        [OperationContract(IsOneWay = true)]
        void Reply(ResponseMessage response);
    }

}
