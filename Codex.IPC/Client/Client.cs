using Codex.IPC.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Client
{
   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public interface IIPCChannel : IIPC, System.ServiceModel.IClientChannel
   {
   }

   [ServiceContract]
   public interface IIPC: Codex.IPC.Contracts.IIPC
   {
      /// <summary>
      /// Acynchronous request response.
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
      Task<ResponseMessage> CallAsync(RequestMessage request);
   }

   [System.Diagnostics.DebuggerStepThroughAttribute()]
   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public partial class IPCClient : System.ServiceModel.ClientBase<IIPC>, IIPC
   {

      public IPCClient()
      {
      }

      public IPCClient(string endpointConfigurationName) :
              base(endpointConfigurationName)
      {
      }

      public IPCClient(string endpointConfigurationName, string remoteAddress) :
              base(endpointConfigurationName, remoteAddress)
      {
      }

      public IPCClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
              base(endpointConfigurationName, remoteAddress)
      {
      }

      public IPCClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
              base(binding, remoteAddress)
      {
      }

      public Codex.IPC.DataTypes.ResponseMessage Call(Codex.IPC.DataTypes.RequestMessage request)
      {
         return base.Channel.Call(request);
      }

      public Task<Codex.IPC.DataTypes.ResponseMessage> CallAsync(Codex.IPC.DataTypes.RequestMessage request)
      {
         return base.Channel.CallAsync(request);
      }
   }

   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public interface IIPCDuplexChannel : Codex.IPC.Contracts.IIPCDuplex, System.ServiceModel.IClientChannel
   {
   }

   [System.Diagnostics.DebuggerStepThroughAttribute()]
   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public partial class IPCDuplexClient : System.ServiceModel.DuplexClientBase<Codex.IPC.Contracts.IIPCDuplex>, Codex.IPC.Contracts.IIPCDuplex
   {

      public IPCDuplexClient(System.ServiceModel.InstanceContext callbackInstance) :
              base(callbackInstance)
      {
      }

      public IPCDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) :
              base(callbackInstance, endpointConfigurationName)
      {
      }

      public IPCDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
              base(callbackInstance, endpointConfigurationName, remoteAddress)
      {
      }

      public IPCDuplexClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
              base(callbackInstance, endpointConfigurationName, remoteAddress)
      {
      }

      public IPCDuplexClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
              base(callbackInstance, binding, remoteAddress)
      {
      }

      public void Send(Codex.IPC.DataTypes.RequestMessage request)
      {
         base.Channel.Send(request);
      }

      public void Subscribe(Codex.IPC.DataTypes.RequestMessage request)
      {
         base.Channel.Subscribe(request);
      }

      public void UnSubscribe(Codex.IPC.DataTypes.RequestMessage request)
      {
         base.Channel.UnSubscribe(request);
      }
   }
}
