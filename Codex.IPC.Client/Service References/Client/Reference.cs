﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Codex.IPC.Client {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Client.IIPC")]
    public interface IIPC {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIPC/Call", ReplyAction="http://tempuri.org/IIPC/CallResponse")]
        Codex.IPC.Implementation.ResponseMessage Call(Codex.IPC.Implementation.RequestMessage request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIPC/Call", ReplyAction="http://tempuri.org/IIPC/CallResponse")]
        System.Threading.Tasks.Task<Codex.IPC.Implementation.ResponseMessage> CallAsync(Codex.IPC.Implementation.RequestMessage request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IIPC/Send")]
        void Send(Codex.IPC.Implementation.RequestMessage request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IIPC/Send")]
        System.Threading.Tasks.Task SendAsync(Codex.IPC.Implementation.RequestMessage request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IIPCChannel : Codex.IPC.Client.IIPC, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IPCClient : System.ServiceModel.ClientBase<Codex.IPC.Client.IIPC>, Codex.IPC.Client.IIPC {
        
        public IPCClient() {
        }
        
        public IPCClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IPCClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IPCClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IPCClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Codex.IPC.Implementation.ResponseMessage Call(Codex.IPC.Implementation.RequestMessage request) {
            return base.Channel.Call(request);
        }
        
        public System.Threading.Tasks.Task<Codex.IPC.Implementation.ResponseMessage> CallAsync(Codex.IPC.Implementation.RequestMessage request) {
            return base.Channel.CallAsync(request);
        }
        
        public void Send(Codex.IPC.Implementation.RequestMessage request) {
            base.Channel.Send(request);
        }
        
        public System.Threading.Tasks.Task SendAsync(Codex.IPC.Implementation.RequestMessage request) {
            return base.Channel.SendAsync(request);
        }
    }
}
