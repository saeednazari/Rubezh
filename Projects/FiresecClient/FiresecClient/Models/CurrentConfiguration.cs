﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecClient.Models;

namespace FiresecClient
{
    public class CurrentConfiguration
    {
        public List<Device> Devices { get; set; }
        public Device RootDevice { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Direction> Directions { get; set; }
        public Firesec.Metadata.config Metadata { get; set; }

        public void FillAllDevices()
        {
            Devices = new List<Device>();
            RootDevice.Parent = null;
            Devices.Add(RootDevice);
            AddChild(RootDevice);
        }

        void AddChild(Device parentDevice)
        {
            foreach (var device in parentDevice.Children)
            {
                device.Parent = parentDevice;
                Devices.Add(device);
                AddChild(device);
            }
        }

        public void SetUnderlyingZones()
        {
            foreach (var device in Devices)
            {
                device.UderlyingZones = new List<string>();
            }

            foreach (var device in Devices)
            {
                if (string.IsNullOrEmpty(device.ZoneNo) == false)
                {
                    device.AddUnderlyingZone(device.ZoneNo);
                }
            }
        }
    }
}
