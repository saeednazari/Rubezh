﻿using System;
using System.Linq;
using Firesec.Journals;
using FiresecAPI.Models;
using FiresecService.Service;

namespace FiresecService.Database
{
	public static class JournalConverter
	{
		public static JournalRecord Convert(journalType innerJournalRecord)
		{
			var journalRecord = new JournalRecord()
			{
				OldId = int.Parse(innerJournalRecord.IDEvents),
				DeviceTime = ConvertTime(innerJournalRecord.Dt),
				SystemTime = ConvertTime(innerJournalRecord.SysDt),
				ZoneName = innerJournalRecord.ZoneName,
				Description = innerJournalRecord.EventDesc,
				DeviceName = innerJournalRecord.CLC_Device,
				PanelName = innerJournalRecord.CLC_DeviceSource,
				DeviceDatabaseId = innerJournalRecord.IDDevices,
				PanelDatabaseId = innerJournalRecord.IDDevicesSource,
				User = innerJournalRecord.UserInfo,
				Detalization = innerJournalRecord.CLC_Detalization,
				SubsystemType = EnumsConverter.StringToSubsystemType(innerJournalRecord.IDSubSystem),
				StateType = (StateType)int.Parse(innerJournalRecord.IDTypeEvents),
			};

			Device device = null;
			if (string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId) == false)
			{
				device = ServiceCash.RunningManagers.FirstOrDefault().ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(
					 x => x.DatabaseId == journalRecord.DeviceDatabaseId);
			}
			else
			{
				device = ServiceCash.RunningManagers.FirstOrDefault().ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(
					   x => x.DatabaseId == journalRecord.PanelDatabaseId);
			}
			if (device != null)
			{
				journalRecord.DeviceCategory = (int)device.Driver.Category;
			}
			else
			{
				journalRecord.DeviceCategory = (int)DeviceCategoryType.None;
			}

			return journalRecord;
		}

		public static DateTime ConvertTime(string firesecTime)
		{
			return new DateTime(
				int.Parse(firesecTime.Substring(0, 4)),
				int.Parse(firesecTime.Substring(4, 2)),
				int.Parse(firesecTime.Substring(6, 2)),
				int.Parse(firesecTime.Substring(9, 2)),
				int.Parse(firesecTime.Substring(12, 2)),
				int.Parse(firesecTime.Substring(15, 2))
			);
		}
	}
}