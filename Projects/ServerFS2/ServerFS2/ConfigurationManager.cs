﻿using System.IO;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;

namespace ServerFS2
{
	public static class ConfigurationManager
	{
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static DriversConfiguration DriversConfiguration { get; set; }

		public static void Load()
		{
			var serverConfigName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var folderName = AppDataFolderHelper.GetFolder("Server2");
			var configFileName = Path.Combine(folderName, "Config.fscp");
			var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(configFileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			if (!Directory.Exists(folderName))
			{
				Directory.CreateDirectory(folderName);
				File.Copy(serverConfigName, configFileName);

				zipFile.ExtractAll(unzipFolderPath);
			}

			var configurationFileName = Path.Combine(unzipFolderPath, "DeviceConfiguration.xml");
			DeviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationFileName);

			configurationFileName = Path.Combine(unzipFolderPath, "DriversConfiguration.xml");
			DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationFileName);
			DriversConfiguration.Drivers.ForEach(x => x.Properties.RemoveAll(z => z.IsAUParameter));
			DriverConfigurationParametersHelper.CreateKnownProperties(DriversConfiguration.Drivers);
			Update();
		}

		private static void Update()
		{
			DeviceConfiguration.Update();
			DeviceConfiguration.Reorder();
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					continue;
				}
			}
			DeviceConfiguration.InvalidateConfiguration();
			DeviceConfiguration.UpdateCrossReferences();
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.UpdateHasExternalDevices();
			}
		}
	}
}