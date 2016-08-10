using System.ServiceModel;
using System.Threading.Tasks;
using Codex.IPC.DataTypes;

namespace Codex.IPC.Contracts
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

      /// <summary>
      /// Non blocking message sending.
      /// </summary>
      /// <param name="request">
      /// Request message containing 
      /// the information regarding the request.
      /// </param>
      [OperationContract(IsOneWay = true)]
      void Post(RequestMessage request);
   }
}
