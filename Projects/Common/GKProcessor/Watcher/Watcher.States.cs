﻿using System.Collections.Generic;
using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecClient;
using Infrastructure.Common.Services;
using FiresecAPI;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsDBMissmatchDuringMonitoring = false;
		EventDescription DBMissmatchDuringMonitoringReason;

		bool _isJournalAnyDBMissmatch = false;
		bool IsJournalAnyDBMissmatch
		{
			get { return _isJournalAnyDBMissmatch; }
			set
			{
				if (_isJournalAnyDBMissmatch != value)
				{
					_isJournalAnyDBMissmatch = value;
					var journalItem = new JournalItem()
					{
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						StateClass = XStateClass.Unknown,
						Name = value ? EventName.База_данных_прибора_не_соответствует_базе_данных_ПК.ToDescription(): EventName.База_данных_прибора_соответствует_базе_данных_ПК.ToDescription()
					};
					AddJournalItem(journalItem);
				}
			}
		}

		void GetAllStates()
		{
			IsDBMissmatchDuringMonitoring = false;
			GKProgressCallback progressCallback = GKProcessorManager.StartProgress("Опрос объектов ГК", "", GkDatabase.Descriptors.Count, false, GKProgressClientType.Monitor);

			foreach (var descriptor in GkDatabase.Descriptors)
			{
				LastUpdateTime = DateTime.Now;
				GetState(descriptor.XBase);
				if (!IsConnected)
				{
					break;
				}
				GKProcessorManager.DoProgress(descriptor.XBase.DescriptorPresentationName, progressCallback);

				WaitIfSuspending();
				if (IsStopping)
					return;
			}
			GKProcessorManager.StopProgress(progressCallback);

			CheckTechnologicalRegime();
			NotifyAllObjectsStateChanged();
			IsJournalAnyDBMissmatch = IsDBMissmatchDuringMonitoring;
		}

		void CheckDelays()
		{
			foreach (var device in XManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.AllParents.Any(x => x.DriverType == XDriverType.RSR2_KAU))
				{
					CheckDelay(device);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				CheckDelay(direction);
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				CheckDelay(pumpStation);
			}
			foreach (var delay in XManager.Delays)
			{
				CheckDelay(delay);
			}
		}

		void CheckDelay(XBase xBase)
		{
			bool mustGetState = false;
			switch (xBase.BaseState.StateClass)
			{
				case XStateClass.TurningOn:
					mustGetState = (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.On:
					mustGetState = xBase.BaseState.ZeroHoldDelayCount < 10 && (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.TurningOff:
					mustGetState = (DateTime.Now - xBase.BaseState.LastDateTime).TotalMilliseconds > 500;
					break;
			}
			if (mustGetState)
			{
				var onDelay = xBase.BaseState.OnDelay;
				var holdDelay = xBase.BaseState.HoldDelay;
				var offDelay = xBase.BaseState.OffDelay;

				if (MeasureDeviceInfos.Any(x => x.Device.UID == xBase.BaseUID))
				{
					GetDelays(xBase);
				}
				else
				{
					GetState(xBase, true);
				}

				if (onDelay != xBase.BaseState.OnDelay || holdDelay != xBase.BaseState.HoldDelay || offDelay != xBase.BaseState.OffDelay)
					OnObjectStateChanged(xBase);

				if (xBase.BaseState.StateClass == XStateClass.On && holdDelay == 0)
					xBase.BaseState.ZeroHoldDelayCount++;
				else
					xBase.BaseState.ZeroHoldDelayCount = 0;
			}
		}

		bool GetDelays(XBase xBase)
		{
			SendResult sendResult = null;
			var expectedBytesCount = 68;
			if (xBase.KauDatabaseParent != null)
			{
				sendResult = SendManager.Send(xBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(xBase.KAUDescriptorNo));
				expectedBytesCount = 32;
			}
			else
			{
				sendResult = SendManager.Send(xBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(xBase.GKDescriptorNo));
				expectedBytesCount = 68;
			}

			if (sendResult.HasError || sendResult.Bytes.Count != expectedBytesCount)
			{
				ConnectionChanged(false);
				return false;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, xBase);

			xBase.BaseState.LastDateTime = DateTime.Now;
			xBase.BaseState.OnDelay = descriptorStateHelper.OnDelay;
			xBase.BaseState.HoldDelay = descriptorStateHelper.HoldDelay;
			xBase.BaseState.OffDelay = descriptorStateHelper.OffDelay;
			return true;
		}

		void GetState(XBase xBase, bool delaysOnly = false)
		{
			var sendResult = SendManager.Send(xBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(xBase.GKDescriptorNo));
			if (sendResult.HasError || sendResult.Bytes.Count != 68)
			{
				ConnectionChanged(false);
				return;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, xBase);
			CheckDBMissmatch(xBase, descriptorStateHelper);

			xBase.BaseState.LastDateTime = DateTime.Now;
			if (!delaysOnly)
			{
				xBase.BaseState.StateBits = descriptorStateHelper.StateBits;
				xBase.BaseState.AdditionalStates = descriptorStateHelper.AdditionalStates;
			}
			xBase.BaseState.OnDelay = descriptorStateHelper.OnDelay;
			xBase.BaseState.HoldDelay = descriptorStateHelper.HoldDelay;
			xBase.BaseState.OffDelay = descriptorStateHelper.OffDelay;
		}

		void CheckDBMissmatch(XBase xBase, DescriptorStateHelper descriptorStateHelper)
		{
			bool isMissmatch = false;
			if (xBase is XDevice)
			{
				var device = xBase as XDevice;
				if (device.Driver.DriverTypeNo != descriptorStateHelper.TypeNo)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_устройства;
				}

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != XDriverType.GK && device.DriverType != XDriverType.KAU && device.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != descriptorStateHelper.PhysicalAddress)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_физический_адрес_устройства;
				}

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_адрес_на_контроллере;
				}
			}
			if (xBase is XZone)
			{
				if (descriptorStateHelper.TypeNo != 0x100)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_для_зоны;
				}
			}
			if (xBase is XDirection)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_для_направления;
				}
			}
			if (xBase is XPumpStation)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_для_НС;
				}
			}
			if (xBase is XDelay)
			{
				if (descriptorStateHelper.TypeNo != 0x101)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_для_Задержки;
				}
			}
			if (xBase is XPim)
			{
				if (descriptorStateHelper.TypeNo != 0x107)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_тип_для_ПИМ;
				}
			}

			var stringLength = Math.Min(xBase.PresentationName.Length, 32);
			var description = xBase.PresentationName.Substring(0, stringLength);
			if (description.TrimEnd(' ') != descriptorStateHelper.Description)
			{
				isMissmatch = true;
				DBMissmatchDuringMonitoringReason = EventDescription.Не_совпадает_описание_компонента;
			}

			xBase.BaseState.IsDBMissmatchDuringMonitoring = isMissmatch;
			if (isMissmatch)
			{
				IsDBMissmatchDuringMonitoring = true;
			}
		}

		#region TechnologicalRegime
		bool CheckTechnologicalRegime()
		{
			var isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(GkDatabase.RootDevice);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.XBase.BaseState.IsInTechnologicalRegime = isInTechnologicalRegime;
			}

			if (!isInTechnologicalRegime)
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					var isKAUInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(kauDatabase.RootDevice);
					var allChildren = XManager.GetAllDeviceChildren(kauDatabase.RootDevice);
					foreach (var device in allChildren)
					{
						device.BaseState.IsInTechnologicalRegime = isKAUInTechnologicalRegime;
					}
				}
			}
			return isInTechnologicalRegime;
		}
		#endregion

		void NotifyAllObjectsStateChanged()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
			foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
			{
				OnObjectStateChanged(device);
			}
			foreach (var device in XManager.GetAllDeviceChildren(gkDevice))
			{
				if (device.Driver.IsGroupDevice || device.DriverType == XDriverType.KAU_Shleif || device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					OnObjectStateChanged(device);
				}
			}
			foreach (var zone in XManager.Zones)
			{
				if (zone.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(zone);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(direction);
				}
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(pumpStation);
				}
			}
			foreach (var delay in XManager.Delays)
			{
				if (delay.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(delay);
				}
			}
			foreach (var pim in XManager.Pims)
			{
				if (pim.GkDatabaseParent == gkDevice)
				{
					OnObjectStateChanged(pim);
				}
			}
		}
	}
}