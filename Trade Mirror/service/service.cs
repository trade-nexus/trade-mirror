﻿
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

// This WCF sample implements the List-based Publish-Subscribe Design Pattern.

using System;
using System.ServiceModel;
using System.Diagnostics;

namespace Microsoft.ServiceModel.Samples
{
    // Create a service contract and define the service operations.
    // NOTE: The service operations must be declared explicitly.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples", SessionMode=SessionMode.Required, CallbackContract=typeof(ISampleClientContract))]
    public interface ISampleContract
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        //void Subscribe();
        bool Subscribe(string userName, string password);
        [OperationContract(IsOneWay = false, IsTerminating=true)]
        void Unsubscribe();
        [OperationContract(IsOneWay = true)]
        void PublishPriceChange(string signalInformation);
    }

    public interface ISampleClientContract
    {
        [OperationContract(IsOneWay = true)]
        void PriceChange(string signalInformation);
    }

    public class PriceChangeEventArgs : EventArgs
    {
        public string SignalInformation;
    }

    // The Service implementation implements your service contract.
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class TradeMirrorService : ISampleContract
    {
        public static event PriceChangeEventHandler PriceChangeEvent;
        public delegate void PriceChangeEventHandler(object sender, PriceChangeEventArgs e);

        ISampleClientContract callback = null;

        PriceChangeEventHandler priceChangeHandler = null;

        //Clients call this service operation to subscribe.
        //A price change event handler is registered for this client instance.

        //public void Subscribe()
        public bool Subscribe(string userName, string password)
        {
            if(userName == "umerazizmalik" && password == "abdulaziz")
            {
                callback = OperationContext.Current.GetCallbackChannel<ISampleClientContract>();
                priceChangeHandler = new PriceChangeEventHandler(PriceChangeHandler);
                PriceChangeEvent += priceChangeHandler;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Clients call this service operation to unsubscribe.
        //The previous price change event handler is deregistered.

        public void Unsubscribe()
        {
            PriceChangeEvent -= priceChangeHandler;
        }

        //Information source clients call this service operation to report a price change.
        //A price change event is raised. The price change event handlers for each subscriber will execute.

        public void PublishPriceChange(string signalInformation)
        {
            PriceChangeEventArgs e = new PriceChangeEventArgs();
            e.SignalInformation = signalInformation;
            PriceChangeEvent(this, e);
        }

        //This event handler runs when a PriceChange event is raised.
        //The client's PriceChange service operation is invoked to provide notification about the price change.

        public void PriceChangeHandler(object sender, PriceChangeEventArgs e)
        {
            callback.PriceChange(e.SignalInformation);
        }

    }

}

