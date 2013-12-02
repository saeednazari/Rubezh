﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsConnected = true;
		int ConnectionLostCount = 0;
		object connectionChangedLocker = new object();

		public void ConnectionChanged(bool isConnected)
		{
			lock (connectionChangedLocker)
			{
				if (!isConnected)
				{
					ConnectionLostCount++;
					if (ConnectionLostCount < 5)
						return;
				}
				else
					ConnectionLostCount = 0;

				if (IsConnected != isConnected)
				{
					var journalItem = new JournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						Name = isConnected ? "Восстановление связи с прибором" : "Потеря связи с прибором"
					};
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						//GetAllStates(false);
					}
				}
                var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
				if (gkDevice == null)
				{
					var uidEquality = XManager.Devices.Any(x => x.UID == GkDatabase.RootDevice.UID);
					Logger.Error("JournalWatcher ConnectionChanged gkDevice = null " + uidEquality.ToString() + " " + GkDatabase.RootDevice.UID.ToString());
					return;
				}

				foreach (var childDevice in XManager.GetAllDeviceChildren(gkDevice))
				{
					if (childDevice != null)
					{
						childDevice.DeviceState.IsConnectionLost = !isConnected;
					}
				}
				foreach (var zoneState in XManager.GetAllGKZoneStates(gkDevice.DeviceState))
				{
					zoneState.IsConnectionLost = !isConnected;
				}
				foreach (var directionState in XManager.GetAllGKDirectionStates(gkDevice.DeviceState))
				{
					directionState.IsConnectionLost = !isConnected;
				}
			}
		}
	}
}