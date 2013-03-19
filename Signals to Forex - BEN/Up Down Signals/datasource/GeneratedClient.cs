﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "http://UpDownSingnalsServer.Services", ConfigurationName = "IUpDownSignals", CallbackContract = typeof(IUpDownSignalsCallback), SessionMode = System.ServiceModel.SessionMode.Required)]
public interface IUpDownSignals
{

    [System.ServiceModel.OperationContractAttribute(Action = "http://UpDownSingnalsServer.Services/IUpDownSignals/Subscribe", ReplyAction = "http://UpDownSingnalsServer.Services/IUpDownSignals/SubscribeResponse")]
    string Subscribe(string userName);

    [System.ServiceModel.OperationContractAttribute(Action = "http://UpDownSingnalsServer.Services/IUpDownSignals/Unsubscribe", ReplyAction = "http://UpDownSingnalsServer.Services/IUpDownSignals/UnsubscribeResponse")]
    bool Unsubscribe(string userName);

    [System.ServiceModel.OperationContractAttribute(Action = "http://UpDownSingnalsServer.Services/IUpDownSignals/PublishNewSignal", ReplyAction = "http://UpDownSingnalsServer.Services/IUpDownSignals/PublishNewSignalResponse")]
    void PublishNewSignal(string signalInformation);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IUpDownSignalsCallback
{

    [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://UpDownSingnalsServer.Services/IUpDownSignals/NewSignal")]
    void NewSignal(string signalInformation);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IUpDownSignalsChannel : IUpDownSignals, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class UpDownSignalsClient : System.ServiceModel.DuplexClientBase<IUpDownSignals>, IUpDownSignals
{

    public UpDownSignalsClient(System.ServiceModel.InstanceContext callbackInstance) :
        base(callbackInstance)
    {
    }

    public UpDownSignalsClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) :
        base(callbackInstance, endpointConfigurationName)
    {
    }

    public UpDownSignalsClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
        base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }

    public UpDownSignalsClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }

    public UpDownSignalsClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(callbackInstance, binding, remoteAddress)
    {
    }

    public string Subscribe(string userName)
    {
        return base.Channel.Subscribe(userName);
    }

    public bool Unsubscribe(string userName)
    {
        return base.Channel.Unsubscribe(userName);
    }

    public void PublishNewSignal(string signalInformation)
    {
        base.Channel.PublishNewSignal(signalInformation);
    }
}
