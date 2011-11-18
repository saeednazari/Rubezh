﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class NewDeviceRangeViewModel : SaveCancelDialogContent
    {
        public NewDeviceRangeViewModel(DeviceViewModel parent)
        {
            Title = "Новоые устройства";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Drivers
                       where ((_parent.Driver.AvaliableChildren.Contains(driver.UID) && driver.HasAddress))
                       select driver;
            }
        }

        Driver _selectedDriver;
        public Driver SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                UpdateAddressRange();
                OnPropertyChanged("SelectedDriver");
            }
        }

        string _startAddress;
        public string StartAddress
        {
            get { return _startAddress; }
            set
            {
                _startAddress = value;
                OnPropertyChanged("StartAddress");
            }
        }

        string _endAddress;
        public string EndAddress
        {
            get { return _endAddress; }
            set
            {
                _endAddress = value;
                OnPropertyChanged("EndAddress");
            }
        }

        Device ParentAddressSystemDevice
        {
            get
            {
                Device parentAddressSystemDevice = _parent;
                while (parentAddressSystemDevice.Driver.UseParentAddressSystem)
                {
                    parentAddressSystemDevice = parentAddressSystemDevice.Parent;
                }
                return parentAddressSystemDevice;
            }
        }

        void AddChildAddressSystemDevice(List<Device> childAddressSystemDevices, Device parentDevice)
        {
            foreach (var childDevice in parentDevice.Children)
            {
                if (childDevice.Driver.UseParentAddressSystem)
                {
                    childAddressSystemDevices.Add(childDevice);
                    AddChildAddressSystemDevice(childAddressSystemDevices, childDevice);
                }
            }
        }

        List<Device> ChildAddressSystemDevices
        {
            get
            {
                var childAddressSystemDevices = new List<Device>();
                AddChildAddressSystemDevice(childAddressSystemDevices, ParentAddressSystemDevice);
                return childAddressSystemDevices;
            }
        }

        List<int> AvaliableAddresses
        {
            get { return NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice); }
        }

        void UpdateAddressRange()
        {
            var avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            int maxIndex = -1;
            for (int i = 0; i < avaliableAddresses.Count; ++i)
            {
                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == avaliableAddresses[i]))
                    maxIndex = i;
            }

            int startAddress;
            int endAddress;

            if (maxIndex == -1)
            {
                startAddress = endAddress = avaliableAddresses[0];
                if (avaliableAddresses.Count > 1)
                    endAddress = avaliableAddresses[1];
            }
            else
            {
                startAddress = endAddress = avaliableAddresses[maxIndex];

                if (avaliableAddresses.Count > maxIndex + 1)
                    startAddress = endAddress = avaliableAddresses[maxIndex + 1];

                if (avaliableAddresses.Count > maxIndex + 2)
                    endAddress = avaliableAddresses[maxIndex + 2];
            }

            StartAddress = AddressConverter.IntToStringAddress(SelectedDriver, startAddress);
            EndAddress = AddressConverter.IntToStringAddress(SelectedDriver, endAddress);
        }

        void CreateDevices()
        {
            int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
            int endAddress = AddressConverter.StringToIntAddress(SelectedDriver, EndAddress);

            var avaliableAddresses = NewDeviceHelper.GetAvaliableAddresses(SelectedDriver, ParentAddressSystemDevice);

            if (startAddress < endAddress)
            {
                ;
            }
            if (startAddress < avaliableAddresses.First())
            {
                ;
            }
            if (endAddress > avaliableAddresses.Last())
            {
                ;
            }

            for (int i = 0; i < avaliableAddresses.Count; ++i)
            {
                int address = avaliableAddresses[i];
                if (ParentAddressSystemDevice.Children.Any(x => x.IntAddress == address))
                {
                }

                if (ChildAddressSystemDevices.Any(x => x.IntAddress == address))
                {
                }
            }

            for (int i = 0; i < avaliableAddresses.Count; ++i)
            {
                if (avaliableAddresses[i] >= startAddress && avaliableAddresses[i] <= endAddress)
                {
                    Device device = _parent.AddChild(SelectedDriver, avaliableAddresses[i]);
                    AddDevice(device, _parentDeviceViewModel);

                    if (SelectedDriver.IsChildAddressReservedRange)
                        i += SelectedDriver.ChildAddressReserveRangeCount - 1;
                }
            }
        }

        void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device, _parentDeviceViewModel.Source);
            deviceViewModel.Parent = parentDeviceViewModel;
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }
        }

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

        protected override void Save(ref bool cancel)
        {
            CreateDevices();

            _parentDeviceViewModel.Update();
            FiresecManager.DeviceConfiguration.Update();
        }
    }
}