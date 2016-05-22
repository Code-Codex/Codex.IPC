using System.ServiceModel;
using Codex.IPC.DataTypes;

namespace Codex.IPC.Contracts
{
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