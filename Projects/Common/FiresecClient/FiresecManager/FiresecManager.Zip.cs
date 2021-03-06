﻿using System.IO;
using System.Text;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;
using XFiresecAPI;
using FiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void LoadFromZipFile(string fileName, out bool isFullConfiguration)
		{
			isFullConfiguration = false;
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			zipFile.ExtractAll(unzipFolderPath);

			var zipConfigurationItemsCollectionFileName = Path.Combine(unzipFolderPath, "ZipConfigurationItemsCollection.xml");
			if (!File.Exists(zipConfigurationItemsCollectionFileName))
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollectionFileName file not found");
				return;
			}
			var zipConfigurationItemsCollection = ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(zipConfigurationItemsCollectionFileName);
			if (zipConfigurationItemsCollection == null)
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollection == null");
				return;
			}

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems)
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "SecurityConfiguration.xml":
							SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(configurationFileName);
							isFullConfiguration = true;
							break;
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName);
							break;
						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationFileName);
							break;
						case "DriversConfiguration.xml":
							FiresecConfiguration.DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationFileName);
							break;
						case "DeviceConfiguration.xml":
							FiresecConfiguration.DeviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationFileName);
							break;
						case "DeviceLibraryConfiguration.xml":
							DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<DeviceLibraryConfiguration>(configurationFileName);
							break;
						case "XDeviceConfiguration.xml":
							XManager.DeviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(configurationFileName);
							break;
						case "XDeviceLibraryConfiguration.xml":
							XManager.DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<XDeviceLibraryConfiguration>(configurationFileName);
							break;
						case "SKDConfiguration.xml":
							SKDManager.SKDConfiguration = ZipSerializeHelper.DeSerialize<SKDConfiguration>(configurationFileName);
							break;
						case "SKDLibraryConfiguration.xml":
							SKDManager.SKDLibraryConfiguration = ZipSerializeHelper.DeSerialize<SKDLibraryConfiguration>(configurationFileName);
							break;
						case "LayoutsConfiguration.xml":
							LayoutsConfiguration = ZipSerializeHelper.DeSerialize<LayoutsConfiguration>(configurationFileName);
							break;
					}
				}
			}

			zipFile.Dispose();
		}
	}
}