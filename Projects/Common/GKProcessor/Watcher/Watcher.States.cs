﻿using System.Collections.Generic;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using GKProcessor.Events;
using XFiresecAPI;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecClient;
using Infrastructure.Common.Services;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsAnyDBMissmatch = false;

		void GetAllStates()
		{
			IsAnyDBMissmatch = false;

			StartProgress("Опрос объектов ГК", GkDatabase.BinaryObjects.Count);
			foreach (var binaryObject in GkDatabase.BinaryObjects)
			{
				bool result = GetState(binaryObject.BinaryBase);
				if (!result)
				{
					if (binaryObject.Device != null && binaryObject.Device.Driver.DriverType == XDriverType.GK)
					{
						binaryObject.Device.DeviceState.IsConnectionLost = true;
						break;
					}
				}
				DoProgress("Опрос объекта ГК " + binaryObject.BinaryBase.BinaryInfo.ToString());
			}
			StopProgress();

			if (IsAnyDBMissmatch)
			{
				foreach (var binaryObject in GkDatabase.BinaryObjects)
				{
					var baseState = binaryObject.BinaryBase.GetXBaseState();
					baseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
					baseState.IsGKMissmatch = true;
				}
			}
			CheckTechnologicalRegime();

			ApplicationService.Invoke(() => { ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
		}

		bool GetState(XBinaryBase binaryBase)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(binaryBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != 68)
			{
				IsAnyDBMissmatch = true;
				ApplicationService.Invoke(() => { binaryBase.GetXBaseState().IsGKMissmatch = true; });
				return false;
			}
			ConnectionChanged(true);
			var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
			CheckDBMissmatch(binaryBase, binaryObjectState);
			ApplicationService.Invoke(() =>
			{
				var binaryState = binaryBase.GetXBaseState();
				binaryState.StateBits = binaryObjectState.States;
				binaryState.AdditionalStates = binaryObjectState.AdditionalStates;
				binaryState.AdditionalStateProperties = binaryObjectState.AdditionalStateProperties;
				binaryState.OnDelay = binaryObjectState.OnDelay;
				binaryState.HoldDelay = binaryObjectState.HoldDelay;
				binaryState.OffDelay = binaryObjectState.OffDelay;
				binaryState.LastDateTime = DateTime.Now;
			});
			return true;
		}

		void CheckAdditionalStates(BinaryObjectBase binaryObject)
		{
			if (binaryObject is DeviceBinaryObject)
			{
				var deviceBinaryObject = binaryObject as DeviceBinaryObject;
				if (deviceBinaryObject.Device.Driver.DriverType == XDriverType.GK || deviceBinaryObject.Device.Driver.DriverType == XDriverType.KAU)
				{
					GetState(binaryObject.BinaryBase);
				}
			}
		}

		void CheckDBMissmatch(XBinaryBase binaryBase, BinaryObjectState binaryObjectState)
		{
			bool isMissmatch = false;
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				if (device.Driver.DriverTypeNo != binaryObjectState.TypeNo)
					isMissmatch = true;

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.Driver.DriverType != XDriverType.GK && device.Driver.DriverType != XDriverType.KAU && device.Driver.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != binaryObjectState.PhysicalAddress)
					isMissmatch = true;

				if (device.GetNearestDatabaseNo() != binaryObjectState.AddressOnController)
					isMissmatch = true;
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				if (binaryObjectState.TypeNo != 0x100)
					isMissmatch = true;
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				if (binaryObjectState.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (binaryBase is XDelay)
			{
				var delay = binaryBase as XDelay;
				if (binaryObjectState.TypeNo != 0x101)
					isMissmatch = true;
			}

			if (binaryBase.GetBinaryDescription() != binaryObjectState.Description)
				isMissmatch = true;

			binaryBase.GetXBaseState().IsRealMissmatch = isMissmatch;
			if (isMissmatch)
			{
				IsAnyDBMissmatch = true;
			}
		}

		void CheckServiceRequired(XBinaryBase binaryBase, JournalItem journalItem)
		{
			if (journalItem.Name != "Запыленность")
				return;

			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				bool isDusted = journalItem.YesNo == JournalYesNoType.Yes;
				ApplicationService.Invoke(() => { device.DeviceState.IsService = isDusted; });
			}
		}

		#region TechnologicalRegime
		void CheckTechnologicalRegime()
		{
			if (IsInTechnologicalRegime(GkDatabase.RootDevice))
			{
				foreach (var binaryObject in GkDatabase.BinaryObjects)
				{
					var baseState = binaryObject.BinaryBase.GetXBaseState();
					baseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
					baseState.IsInTechnologicalRegime = true;
				}
			}
			else
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					if (IsInTechnologicalRegime(kauDatabase.RootDevice))
					{
						var allChildren = XManager.GetAllDeviceChildren(kauDatabase.RootDevice);
						allChildren.Add(kauDatabase.RootDevice);
						foreach (var device in allChildren)
						{
							var baseState = device.GetXBaseState();
							baseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
							baseState.IsInTechnologicalRegime = true;
						}
					}
				}
			}
		}

		bool IsInTechnologicalRegime(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 1, 1, null, true, false, 2000);
			if (!sendResult.HasError)
			{
				if (sendResult.Bytes.Count > 0)
				{
					var version = sendResult.Bytes[0];
					if (version > 127)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
}