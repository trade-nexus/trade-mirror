/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Trade Mirror provides an infrastructure for low latency trade copying
* services from master to child traders, and also trader to different
* channels including social media. It is a highly customizable solution
* with low-latency signal transmission capabilities. The tool can copy trades
* from sender and publish them to all subscribed receiver’s in real time
* across a local network or the internet. Trade Mirror is built using
* languages and frameworks that include C#, C++, WPF, WCF, Socket Programming,
* MySQL, NUnit and MT4 and MT5 MetaTrader platforms.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


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
