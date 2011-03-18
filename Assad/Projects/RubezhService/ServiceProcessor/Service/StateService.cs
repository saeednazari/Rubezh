﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using ServiseProcessor;
using Common;
using ServiceApi;

namespace ServiseProcessor
{
    class StateService : IStateService
    {
        static ICallback callback;

        public void Initialize()
        {
            callback = OperationContext.Current.GetCallbackChannel<ICallback>();
        }

        public static void Notify(string message)
        {
            if (callback != null)
                callback.Notify(message);
        }

        public static void DeviceChanged(Device device)
        {
            if (callback != null)
            {
                callback.DeviceChanged(device);
            }
        }

        public static void ZoneChanged(Zone zone)
        {
            if (callback != null)
            {
                callback.ZoneChanged(zone);
            }
        }

        public Configuration GetConfiguration()
        {
            return Services.Configuration;
        }

        public void ExecuteCommand(string devicePath, string command)
        {
            Device device;
            try
            {
                device = Services.Configuration.Devices.First(x => x.Path == devicePath);
            }
            catch
            {
                device = null;
            }
            if (device != null)
            {
                Processor.ExecuteCommand(device, command);
            }
        }

        public void SetConfiguration(Configuration data)
        {
            Processor.SetNewConfig(data);
        }
    }
}