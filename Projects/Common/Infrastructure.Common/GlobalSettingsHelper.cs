﻿using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;

namespace Infrastructure.Common
{
	public static class GlobalSettingsHelper
	{
		static string FileName = AppDataFolderHelper.GetGlobalSettingsFileName();
		public static GlobalSettings GlobalSettings { get; set; }

		static GlobalSettingsHelper()
		{
			Load();
		}

		public static void Load()
		{
			try
			{
				GlobalSettings = new GlobalSettings();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(GlobalSettings));
						GlobalSettings = (GlobalSettings)dataContractSerializer.ReadObject(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public static void Save()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(GlobalSettings));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					dataContractSerializer.WriteObject(fileStream, GlobalSettings);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}