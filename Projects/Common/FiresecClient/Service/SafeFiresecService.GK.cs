﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common;

namespace FiresecClient
{
    public partial class SafeFiresecService
    {
		static bool IsGKAsAService = GlobalSettingsHelper.GlobalSettings.IsGKAsAService;

		public void GKWriteConfiguration(XDevice device, bool writeFileToGK = false)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.BaseUID, writeFileToGK), "GKWriteConfiguration");
			}
			else
			{
				GkDescriptorsWriter.WriteConfig(device, writeFileToGK);
				if (!String.IsNullOrEmpty(GkDescriptorsWriter.Error))
				{
					LoadingService.IsCanceled = true; 
					MessageBoxService.ShowError(GkDescriptorsWriter.Error); 
					return;
				}
				FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
				AddGKMessage("Запись конфигурации в прибор", "", XStateClass.Info, device, true);
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKReadConfiguration(device.BaseUID), "GKReadConfiguration");
			}
			{
				AddGKMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device, true);
				var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
				descriptorReader.ReadConfiguration(device);
				return new OperationResult<XDeviceConfiguration> { HasError = !string.IsNullOrEmpty(descriptorReader.Error), Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
			}
		}

		public void GKUpdateFirmware(XDevice device, string fileName)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.GKUpdateFirmware(device.BaseUID, fileName), "GKUpdateFirmware");
			}
			else
			{
				AddGKMessage("Обновление ПО прибора", "", XStateClass.Info, device, true);
				FirmwareUpdateHelper.Update(device, fileName);
			}
		}

		public bool GKSyncronyseTime(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device.BaseUID); }, "GKSyncronyseTime");
			}
			else
			{
				AddGKMessage("Синхронизация времени", "", XStateClass.Info, device, true);
				return DeviceBytesHelper.WriteDateTime(device);
			}
		}

		public string GKGetDeviceInfo(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device.BaseUID); }, "GKGetDeviceInfo");
			}
			else
			{
				AddGKMessage("Запрос информации об устройсве", "", XStateClass.Info, device, true);
				return DeviceBytesHelper.GetDeviceInfo(device);
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(device.BaseUID); }, "GKGetJournalItemsCount");
			}
			else
			{
				var sendResult = SendManager.Send(device, 0, 6, 64);
				if (sendResult.HasError)
				{
					return new OperationResult<int>("Ошибка связи с устройством");
				}
				var journalParser = new JournalParser(device, sendResult.Bytes);
				var result = journalParser.JournalItem.GKJournalRecordNo.Value;
				return new OperationResult<int>() { Result = result };
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device.BaseUID, no); }, "GKReadJournalItem");
			}
			else
			{
				var data = BitConverter.GetBytes(no).ToList();
				var sendResult = SendManager.Send(device, 4, 7, 64, data);
				if (sendResult.HasError)
				{
					return new OperationResult<JournalItem>("Ошибка связи с устройством");
				}
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<JournalItem>() { Result = journalParser.JournalItem };
			}
		}

		public OperationResult<bool> GKSetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(device.BaseUID); }, "SetSingleParameter");
			}
			else
			{
				var error = ParametersHelper.SetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
		}

		public OperationResult<bool> GKGetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKGetSingleParameter(device.BaseUID); }, "GetSingleParameter");
			}
			else
			{
				var error = ParametersHelper.GetSingleParameter(device);
				return new OperationResult<bool>() { HasError = !string.IsNullOrEmpty(error), Error = error, Result = true };
			}
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(device.BaseUID, stateBit); }, "GKExecuteDeviceCommand");
			}
			else
			{
				Watcher.SendControlCommand(device, stateBit);
				AddGKMessage("Команда оператора", stateBit.ToDescription(), XStateClass.Info, device);
			}
		}

		public void GKReset(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKReset(xBase.BaseUID, GetObjectType(xBase)); }, "GKReset");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.Reset);
				AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, xBase);
			}
		}

		public void GKResetFire1(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire1(zone.UID); }, "GKResetFire1");
			}
			else
			{
				Watcher.SendControlCommand(zone, 0x02);
				AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKResetFire2(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire2(zone.UID); }, "GKResetFire2");
			}
			else
			{
				Watcher.SendControlCommand(zone, 0x03);
				AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(xBase.BaseUID, GetObjectType(xBase)); }, "GKSetAutomaticRegime");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
				AddGKMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetManualRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetManualRegime(xBase.BaseUID, GetObjectType(xBase)); }, "GKSetManualRegime");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
				AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOn");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
				AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOn(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOn");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
				AddGKMessage("Команда оператора", "Включить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOnNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOnNow(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOnNow");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
				AddGKMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOff(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOff(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOff");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
				AddGKMessage("Команда оператора", "Выключить", XStateClass.Info, xBase);
			}
		}

		public void GKTurnOffNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOffNow(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOffNow");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
				AddGKMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
			}
		}

		public void GKStop(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStop(xBase.BaseUID, GetObjectType(xBase)); }, "GKStop");
			}
			else
			{
				Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
				AddGKMessage("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
			}
		}

		public void AddGKMessage(string message, string description, XStateClass stateClass, XBase xBase, bool isAdministrator = false)
		{
			Guid uid = Guid.Empty;
			var journalItemType = JournalItemType.System;
			if (xBase != null)
			{
				if (xBase is XDevice)
				{
					uid = (xBase as XDevice).UID;
					journalItemType = JournalItemType.Device;
				}
				if (xBase is XZone)
				{
					uid = (xBase as XZone).UID;
					journalItemType = JournalItemType.Zone;
				}
				if (xBase is XDirection)
				{
					uid = (xBase as XDirection).UID;
					journalItemType = JournalItemType.Direction;
				}
				if (xBase is XDelay)
				{
					uid = (xBase as XDelay).UID;
					journalItemType = JournalItemType.Delay;
				}
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
				Description = description,
				ObjectUID = uid,
				ObjectStateClass = XStateClass.Norm,
				UserName = FiresecManager.CurrentUser.Name,
				SubsystemType = XSubsystemType.System
			};
			if (xBase != null)
			{
				journalItem.ObjectName = xBase.PresentationName;
				journalItem.GKObjectNo = (ushort)xBase.GKDescriptorNo;
			}

			if (isAdministrator)
			{
				SafeOperationCall(() => { FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
			}
			else
			{
				GKDBHelper.Add(journalItem);
				OnNewJournalItems(journalItem);
			}
		}

		public void GKAddMessage(string name, string description)
		{
			if (IsGKAsAService)
			{
			}
			else
			{
				AddGKMessage(name, description, XStateClass.Norm, null, true);
			}
		}

		public event Action<List<JournalItem>> NewJournalItems;
		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			if (NewJournalItems != null)
				NewJournalItems(journalItems);
		}
		void OnNewJournalItems(JournalItem journalItem)
		{
			var journalItems = new List<JournalItem>() { journalItem };
			if (NewJournalItems != null)
				NewJournalItems(journalItems);
		}

		XBaseObjectType GetObjectType(XBase xBase)
		{
			if (xBase is XDevice)
				return XBaseObjectType.Deivce;
			if (xBase is XZone)
				return XBaseObjectType.Zone;
			if (xBase is XDirection)
				return XBaseObjectType.Direction;
			if (xBase is XDelay)
				return XBaseObjectType.Delay;
			if (xBase is XPim)
				return XBaseObjectType.Pim;
			return XBaseObjectType.Deivce;
		}
    }
}