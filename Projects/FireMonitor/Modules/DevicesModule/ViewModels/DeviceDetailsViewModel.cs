﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public DeviceDetailsViewModel(string deviceId)
        {
            _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceId);
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
            deviceState.StateChanged += new Action(deviceState_StateChanged);
            _deviceControlViewModel = new DeviceControlViewModel(_device);

            Title = _device.Driver.ShortName + " " + _device.DottedAddress;
            CloseCommand = new RelayCommand(OnClosing);
        }

        Device _device;
        DeviceControls.DeviceControl _deviceControl;
        DeviceControlViewModel _deviceControlViewModel;

        public Driver Driver
        {
            get { return _device.Driver; }
        }

        void deviceState_StateChanged()
        {
            DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);

            if (_deviceControl != null)
            {
                _deviceControl.StateId = deviceState.State.Id.ToString();
            }

            OnPropertyChanged("DeviceControlContent");
        }

        public object DeviceControlContent
        {
            get
            {
                _deviceControl = new DeviceControls.DeviceControl();
                _deviceControl.DriverId = _device.Driver.Id;

                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                _deviceControl.StateId = deviceState.State.Id.ToString();

                _deviceControl.Width = 50;
                _deviceControl.Height = 50;

                return _deviceControl;
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (_device.Parent != null)
                {
                    return _device.Parent.Driver.Name;
                }
                return null;
            }
        }

        public string PresentationZone
        {
            get { return _device.GetPersentationZone(); }
        }

        public List<string> SelfStates
        {
            get
            {
                List<string> selfStates = new List<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                    foreach (var state in deviceState.States)
                    {
                        if (state.IsActive)
                            selfStates.Add(state.DriverState.Name);
                    }
                return selfStates;
            }
        }

        public List<string> ParentStates
        {
            get
            {
                List<string> parentStates = new List<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.ParentStringStates != null)
                    foreach (var parentState in deviceState.ParentStringStates)
                    {
                        parentStates.Add(parentState);
                    }
                return parentStates;
            }
        }

        public List<string> Parameters
        {
            get
            {
                List<string> parameters = new List<string>();
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                if (deviceState.Parameters != null)
                    foreach (var parameter in deviceState.Parameters)
                    {
                        if (parameter.Visible)
                        {
                            if (string.IsNullOrEmpty(parameter.Value))
                                continue;
                            if (parameter.Value == "<NULL>")
                                continue;
                            parameters.Add(parameter.Caption + " - " + parameter.Value);
                        }
                    }
                return parameters;
            }
        }

        public State State
        {
            get
            {
                DeviceState deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == _device.Id);
                return deviceState.State;
            }
        }

        public event Action Closing;
        void OnClosing()
        {
            if (Closing != null)
                Closing();
        }

        public RelayCommand CloseCommand { get; private set; }
    }

}
