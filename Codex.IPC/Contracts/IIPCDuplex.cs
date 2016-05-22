using System.ServiceModel;
using Codex.IPC.DataTypes;

namespace Codex.IPC.Contracts
{
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
}