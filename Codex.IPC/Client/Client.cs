using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codex.IPC.Contracts;

namespace Codex.IPC.Client
{
   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public interface IIPCChannel : IIPC, System.ServiceModel.IClientChannel
   {
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
   }

   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public interface IIPCDuplexChannel : IIPCDuplex, System.ServiceModel.IClientChannel
   {
   }

   [System.Diagnostics.DebuggerStepThroughAttribute()]
   [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
   public partial class IPCDuplexClient : System.ServiceModel.DuplexClientBase<IIPCDuplex>, IIPCDuplex
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
