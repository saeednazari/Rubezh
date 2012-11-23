﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace DevicesModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		DeviceConfiguration RemoteDeviceConfiguration;
		Device LocalRootDevice;
		Device RemoteRootDevice;
        Device LocalRootClone;
        Device RemoteRootClone;

		public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration remoteDeviceConfiguration)
		{
			Title = "Сравнение конфигураций";
			ReplaceCommand = new RelayCommand(OnReplace);

			RemoteDeviceConfiguration = remoteDeviceConfiguration;
			RemoteDeviceConfiguration.Reorder();
			RemoteDeviceConfiguration.Update();
			RemoteDeviceConfiguration.InvalidateConfiguration();
			RemoteDeviceConfiguration.UpdateCrossReferences();

			foreach (var device in RemoteDeviceConfiguration.Devices)
			{
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			}

			LocalRootDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			RemoteRootDevice = RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);

            LocalRootClone = (Device)FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceUID).Clone();
            RemoteRootClone = (Device)RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID).Clone();

            LocalRootClone.Children = new List<Device>();
            if (LocalRootDevice.Children != null)
                foreach (var children in LocalRootDevice.Children)
                {
                    var childrenClone = (Device) children.Clone();
                    LocalRootClone.Children.Add(childrenClone);
                    if (children.Children != null)
                    {
                        var localchch =
                            LocalRootClone.Children.FirstOrDefault(
                                x =>
                                ((x.PresentationName == children.PresentationName) &&
                                 (x.AddressFullPath == children.AddressFullPath)));
                        localchch.Children = new List<Device>();
                        foreach (var chch in children.Children)
                        {
                            var chchClone = (Device)chch.Clone();
                            localchch.Children.Add(chchClone);
                        }
                    }
                }

            RemoteRootClone.Children = new List<Device>();
            if (RemoteRootDevice.Children != null)
                foreach (var children in RemoteRootDevice.Children)
                {
                    var childrenClone = (Device)children.Clone();
                    RemoteRootClone.Children.Add(childrenClone);
                    if (children.Children != null)
                    {
                        var remotechch =
                            RemoteRootClone.Children.FirstOrDefault(
                                x =>
                                ((x.PresentationName == children.PresentationName) &&
                                 (x.AddressFullPath == children.AddressFullPath)));
                        remotechch.Children = new List<Device>();
                        foreach (var chch in children.Children)
                        {
                            var chchClone = (Device)chch.Clone();
                            remotechch.Children.Add(chchClone);
                        }
                    }
                }

            IntoLocalDevice(LocalRootDevice, RemoteRootClone);
            IntoRemoteDevice(RemoteRootDevice, LocalRootClone);

            Sort(LocalRootClone);
            Sort(RemoteRootClone);

            LocalDevices = new DeviceTreeViewModel(LocalRootClone, FiresecManager.FiresecConfiguration.DeviceConfiguration, false);
            RemoteDevices = new DeviceTreeViewModel(RemoteRootClone, RemoteDeviceConfiguration, true);
		}
        private void IntoLocalDevice(Device localRootDevice , Device remoteRootDevice)
        {
            foreach (var local in localRootDevice.Children)
            {
                var remoteAndLocal =
                    remoteRootDevice.Children.FirstOrDefault(x => (x.PresentationName == local.PresentationName) && (x.AddressFullPath == local.AddressFullPath));
                if (remoteAndLocal == null)
                {
                    var remote = (Device)local.Clone();
                    remote.Children = new List<Device>();
                    remote.HasDifferences = true;
                    remoteAndLocal = remote;
                    remoteRootDevice.Children.Add(remote);
                }
                else
                {
                    if ((remoteAndLocal.Zone == null) && (local.Zone != null))
                    {
                        remoteAndLocal.Zone = local.Zone;
                        remoteAndLocal.HasDifferences = true;
                    }
                    else if ((remoteAndLocal.ZonesInLogic.Count == 0) && (local.ZonesInLogic.Count != 0))
                    {
                        remoteAndLocal.ZonesInLogic = local.ZonesInLogic;
                        remoteAndLocal.ZoneLogic = local.ZoneLogic;
                        remoteAndLocal.HasDifferences = true;
                    }
                    else
                    {
                        remoteAndLocal.HasDifferences = false;
                    }
                }

                if ((local.Children != null) && (local.Children.Count > 0))
                {
                    IntoLocalDevice(local, remoteAndLocal);
                }
            }
        }
        private void IntoRemoteDevice(Device remoteRootDevice, Device localRootDevice)
        {
            foreach (var remote in remoteRootDevice.Children)
            {
                var localAndRemote = localRootDevice.Children.FirstOrDefault(x => (x.PresentationName == remote.PresentationName) && (x.AddressFullPath == remote.AddressFullPath));
                if (localAndRemote == null)
                {
                    var local = (Device)remote.Clone();
                    local.Children = new List<Device>();
                    local.HasDifferences = true;
                    localAndRemote = local;
                    localRootDevice.Children.Add(local);
                }
                else
                {
                    if ((localAndRemote.Zone == null) && (remote.Zone != null))
                    {
                        localAndRemote.Zone = remote.Zone;
                        localAndRemote.HasDifferences = true;
                    }
                    else if ((localAndRemote.ZonesInLogic.Count == 0) && (remote.ZonesInLogic.Count != 0))
                    {
                        localAndRemote.ZonesInLogic = remote.ZonesInLogic;
                        localAndRemote.ZoneLogic = remote.ZoneLogic;
                        localAndRemote.HasDifferences = true;
                    }
                    else
                    {
                        localAndRemote.HasDifferences = false;
                    }
                }
                if ((remote.Children != null) && (remote.Children.Count > 0))
                {
                        IntoRemoteDevice(remote, localAndRemote);
                }
            }

			//foreach(var device in UnionRootDevice.Children)
			//{
			//    if (device.ZonesInLogic != null)
			//    foreach (var clause in device.ZoneLogic.Clauses)
			//        var zone = FiresecManager.Zones.FirstOrDefault(x => x.PresentationName == cl);					
			//}
        }

        void Sort(Device device)
        {
            if (device.Children != null)
            {
                device.Children = device.Children.OrderBy(x => x.AddressFullPath).ToList();
                device.Children = device.Children.OrderBy(x => x.PresentationName).ToList();
            }
                foreach (var child in device.Children)
                {
                    if (child.Children !=null)
                        Sort(child);
                }
        }

	    public DeviceTreeViewModel LocalDevices { get; private set; }
		public DeviceTreeViewModel RemoteDevices { get; private set; }

		public RelayCommand ReplaceCommand { get; private set; }
		void OnReplace()
		{
            LocalRootDevice.Parent.Children.Remove(LocalRootDevice);
		    LocalRootDevice.Parent.Children.Add(RemoteRootDevice);

		    var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == LocalRootDevice.UID);
		    if (deviceViewModel == null)
		    {
		        Logger.Error("DeviceConfigurationViewModel.OnReplace deviceViewModel = null");
		        return;
		    }

		    //deviceViewModel.CollapseChildren();
		    deviceViewModel.Children.Clear();

		    foreach (var device in RemoteRootDevice.Children)
		    {
                DevicesViewModel.Current.AddDevice(device, deviceViewModel);
		        BuildZones(device);
		    }
		    
            deviceViewModel.ExpandChildren();
		    FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
		    ServiceFactory.SaveService.FSChanged = true;
		    DevicesViewModel.UpdateGuardVisibility();
            FiresecManager.FiresecConfiguration.UpdateConfiguration();
            Close(true);
		}

        public void BuildZones(Device device)
        {
            if (device.Zone != null)
            {
                if (!FiresecManager.Zones.Any(x => (x.No == device.Zone.No)))
                {
                    FiresecManager.FiresecConfiguration.AddZone(device.Zone);
                    ZonesViewModel.Current.Zones.Add(new ZoneViewModel(device.Zone));
                }
                device.Zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.Zone.No);
                FiresecManager.FiresecConfiguration.AddDeviceToZone(device, device.Zone);
            }

            if ((device.ZonesInLogic != null) && (device.ZonesInLogic.Count > 0))
            {
                List<Zone> tempZonesInLogic = new List<Zone>();
                foreach (var zoneInLogic in device.ZonesInLogic)
                {
                    if (!FiresecManager.Zones.Any(x => (x.No == zoneInLogic.No)))
                    {
                        FiresecManager.FiresecConfiguration.AddZone(zoneInLogic);
                        ZonesViewModel.Current.Zones.Add(new ZoneViewModel(zoneInLogic));
                    }
                    tempZonesInLogic.Add(FiresecManager.Zones.FirstOrDefault(x => x.No == zoneInLogic.No));
                }
                device.ZonesInLogic = tempZonesInLogic;
                device.ZoneLogic.Clauses[0].Zones = tempZonesInLogic;
                device.ZoneLogic.Clauses[0].ZoneUIDs = new List<Guid>();
                foreach (var tempZoneInLogic in tempZonesInLogic)
                {
                    device.ZoneLogic.Clauses[0].ZoneUIDs.Add(tempZoneInLogic.UID);
                }
            }

            if(device.Children != null)
                foreach (var child in device.Children)
                {
                    BuildZones(child);
                }
        }
	}
}