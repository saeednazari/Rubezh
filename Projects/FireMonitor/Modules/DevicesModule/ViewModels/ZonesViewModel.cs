﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using DevicesModule.Events;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel
    {
        public ZonesViewModel()
        {
            ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        void OnZoneStateChanged(string zoneNo)
        {
            if (FiresecManager.CurrentStates.ZoneStates.Any(x => x.No == zoneNo))
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
                if (Zones.Any(x => x.No == zoneNo))
                {
                    ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.No == zoneNo);
                    zoneViewModel.State = zoneState.State;
                }
            }
        }

        void OnDeviceStateChanged(string id)
        {
            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Id == id))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == id);
                if (plainDevices != null)
                {
                    if (plainDevices.Any(x => x.Device.Id == id))
                    {
                        DeviceViewModel deviceViewModel = plainDevices.FirstOrDefault(x => x.Device.Id == id);
                        deviceViewModel.Update();
                        //deviceViewModel.MainState = deviceState.State;
                    }
                }
            }
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>();

            foreach (Zone zone in FiresecManager.CurrentConfiguration.Zones)
            {
                ZoneViewModel zoneViewModel = new ZoneViewModel();
                zoneViewModel.Initialize(zone);
                Zones.Add(zoneViewModel);
            }

            FiresecManager.CurrentStates.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
        }

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x=>x.No == zoneNo);
            ZoneViewModel zoneViewModel = Zones.FirstOrDefault(x => x.No == zoneNo);
            zoneViewModel.State = zoneState.State;
        }

        public void Select(string zoneNo)
        {
            if (string.IsNullOrEmpty(zoneNo) == false)
            {
                if (Zones.Any(x => x.No == zoneNo))
                {
                    SelectedZone = Zones.FirstOrDefault(x => x.No == zoneNo);
                }
            }
        }

        ObservableCollection<ZoneViewModel> zones;
        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return zones; }
            set
            {
                zones = value;
                OnPropertyChanged("Zones");
            }
        }

        ZoneViewModel selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return selectedZone; }
            set
            {
                selectedZone = value;
                InitializeDevices();
                OnPropertyChanged("SelectedZone");
            }
        }

        void InitializeDevices()
        {
            plainDevices = new List<DeviceViewModel>();
            Devices = new ObservableCollection<DeviceViewModel>();

            Device rooDevice = FiresecManager.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice, Devices);
            rootDeviceViewModel.IsExpanded = true;
            plainDevices.Add(rootDeviceViewModel);
            Devices.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                if ((device.UderlyingZones.Contains(SelectedZone.No) == false) &&
                    (device.ZoneNo != SelectedZone.No))
                    continue;

                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                plainDevices.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        public List<DeviceViewModel> plainDevices;

        ObservableCollection<DeviceViewModel> devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        public override void Dispose()
        {
        }
    }
}
