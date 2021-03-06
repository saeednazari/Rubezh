﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDeviceConfiguration DeviceConfiguration { get; set; }
		public static XDriversConfiguration DriversConfiguration { get; set; }
        public static XDeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }

		static XManager()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			DriversConfiguration = new XDriversConfiguration();
			Delays = new List<XDelay>();
			Pims = new List<XPim>();
		}

		public static List<XDevice> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}
		public static List<XZone> Zones
		{
			get { return DeviceConfiguration.Zones; }
		}
		public static List<XDirection> Directions
		{
			get { return DeviceConfiguration.Directions; }
		}
		public static List<XPumpStation> PumpStations
		{
			get { return DeviceConfiguration.PumpStations; }
		}
		public static List<XDriver> Drivers
		{
			get { return DriversConfiguration.XDrivers; }
		}
		public static List<XParameterTemplate> ParameterTemplates
		{
			get { return XManager.DeviceConfiguration.ParameterTemplates; }
		}

		public static List<XDelay> Delays { get; set; }
		public static List<XPim> Pims { get; set; }

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new XDeviceConfiguration();
			var driver = Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
			DeviceConfiguration.RootDevice = new XDevice()
			{
				DriverUID = driver.UID
			};
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			UpdateConfigurationHelper.Update(DeviceConfiguration);
		}

		public static void Prepare()
		{
			UpdateConfigurationHelper.PrepareDescriptors(DeviceConfiguration);
		}

		public static ushort GetKauLine(XDevice device)
		{
			ushort lineNo = 0;
			if (device.Driver.IsKauOrRSR2Kau)
			{
				var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
				if (modeProperty != null)
				{
					return modeProperty.Value;
				}
			}
			return lineNo;
		}

		public static string GetIpAddress(XDevice device)
		{
			XDevice gkDevice = null;
            switch (device.DriverType)
            {
                case XDriverType.GK:
                    gkDevice = device;
                    break;

                case XDriverType.KAU:
				case XDriverType.RSR2_KAU:
                    gkDevice = device.Parent;
                    break;

                default:
                    {
                        Logger.Error("XManager.GetIpAddress Получить IP адрес можно только у ГК или в КАУ");
                        throw new Exception("Получить IP адрес можно только у ГК или в КАУ");
                    }
            }
			var ipAddress = gkDevice.GetGKIpAddress();
			return ipAddress;
		}

		public static bool IsManyGK()
		{
			return DeviceConfiguration.Devices.Where(x => x.DriverType == XDriverType.GK).Count() > 1;
		}
	}
}