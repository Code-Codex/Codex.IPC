using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Codex.IPC.Implementation
{
    /// <summary>
    /// Request - Response contract.
    /// </summary>
    [ServiceContract]
    public interface IIPC
    {
        /// <summary>
        /// Blocking request response.
        /// </summary>
        /// <param name="request">
        /// Request message containing 
        /// the information regarding the request.
        /// </param>
        /// <returns>
        /// Response message which contains
        /// the output in the body.
        /// </returns>
        [OperationContract]
        ResponseMessage Call(RequestMessage request);
    }

    /// <summary>
    /// Duplex contract
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IIPCDuplexCallback))]
    public interface IIPCDuplex
    {
        /// <summary>
        /// Non blocking message sending.
        /// </summary>
        /// <param name="request">
        /// Request message containing 
        /// the information regarding the request.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void Send(RequestMessage request);

        /// <summary>
        /// Subscribe to broad cast or callback messages.
        /// </summary>
        /// <param name="request">
        /// Request message containing 
        /// the information regarding the request.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void Subscribe(RequestMessage request);

        /// <summary>
        /// Un-Subscribe from broad cast or callback messages.
        /// </summary>
        /// <param name="request">
        /// Request message containing 
        /// the information regarding the request.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void UnSubscribe(RequestMessage request);
    }

    /// <summary>
    /// Call back interface for duplex link.
    /// </summary>
    public interface IIPCDuplexCallback
    {
        /// <summary>
        /// Reply from the server
        /// </summary>
        /// <param name="request">
        /// Response message which contains
        /// the output in the body.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void Reply(ResponseMessage response);
    }

}
