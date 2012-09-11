﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI;
using FiresecAPI.Models;
using System.IO;

namespace Firesec
{
	public partial class FiresecDriver
	{
		NativeFiresecClient NativeFiresecClient { get; set; }
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
		public ConfigurationConverter ConfigurationConverter { get; private set; }
		public Watcher Watcher { get; private set; }

		public FiresecDriver(int lastJournalNo)
		{
			FiresecSerializedClient = new FiresecSerializedClient();
			FiresecSerializedClient.Connect("adm", "");
			ConfigurationConverter = new ConfigurationConverter()
			{
				FiresecSerializedClient = FiresecSerializedClient
			};
			ConfigurationConverter.ConvertMetadataFromFiresec();
			Watcher = new Watcher(FiresecSerializedClient, true);
		}

		public DeviceConfigurationStates ConvertStates()
		{
			ConfigurationCash.DeviceConfigurationStates = new DeviceConfigurationStates();
			if (ConfigurationCash.DeviceConfiguration.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
				{
					var deviceState = new DeviceState()
					{
						UID = device.UID,
						PlaceInTree = device.PlaceInTree,
						Device = device
					};

					foreach (var parameter in device.Driver.Parameters)
						deviceState.Parameters.Add(parameter.Copy());

					device.DeviceState = deviceState;

					ConfigurationCash.DeviceConfigurationStates.DeviceStates.Add(deviceState);
				}
			}

			foreach (var zone in ConfigurationCash.DeviceConfiguration.Zones)
			{
				var zoneState = new ZoneState() { No = zone.No };
				zone.ZoneState = zoneState;
				ConfigurationCash.DeviceConfigurationStates.ZoneStates.Add(zoneState);
			}

			Watcher.OnStateChanged();
			Watcher.OnParametersChanged();

			return ConfigurationCash.DeviceConfigurationStates;
		}

		public void Convert()
		{
			ConfigurationConverter.Convert();
		}

		public Firesec.Models.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			ConfigurationConverter.ConvertBack(deviceConfiguration, includeSecurity);
			return ConfigurationConverter.FiresecConfiguration;
		}

		public OperationResult<bool> SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, true);
			return FiresecSerializedClient.SetNewConfig(firesecConfiguration);
		}

		public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceWriteConfig(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetPassword(firesecConfiguration, device.GetPlaceInTree(), password, (int)devicePasswordType);
		}

		public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceDatetimeSync(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetInformation(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceGetSerialList(firesecConfiguration, device.GetPlaceInTree());

			var operationResult = new OperationResult<List<string>>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (result.Result != null)
				operationResult.Result = result.Result.Split(';').ToList();
			return operationResult;
		}

		public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			fileName = Guid.NewGuid().ToString();
			Directory.CreateDirectory("Temp");
			fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceUpdateFirmware(firesecConfiguration, device.GetPlaceInTree(), fileName);
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			fileName = Guid.NewGuid().ToString();
			Directory.CreateDirectory("Temp");
			fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceVerifyFirmwareVersion(firesecConfiguration, device.GetPlaceInTree(), fileName);
		}

		public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceReadEventLog(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceAutoDetectChildren(firesecConfiguration, device.GetPlaceInTree(), fastSearch);

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (operationResult.HasError)
				return operationResult;

			if (result.Result == null)
				return new OperationResult<DeviceConfiguration>("Ошибка. Получена пустая конфигурация");

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceReadConfig(firesecConfiguration, device.GetPlaceInTree());

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (operationResult.HasError)
				return operationResult;

			if (result.Result == null)
				return new OperationResult<DeviceConfiguration>("Ошибка. Получена пустая конфигурация");

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			var result = FiresecSerializedClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());

			var operationResult = new OperationResult<List<DeviceCustomFunction>>()
			{
				HasError = result.HasError,
				Error = result.Error
			};

			operationResult.Result = DeviceCustomFunctionConverter.Convert(result.Result);
			return operationResult;
		}

		public OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceCustomFunctionExecute(firesecConfiguration, device.GetPlaceInTree(), functionName);
		}

		public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetGuardUsersList(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetGuardUsersList(firesecConfiguration, device.GetPlaceInTree(), users);
		}

		public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetMDS5Data(firesecConfiguration, device.GetPlaceInTree());
		}

		public void AddToIgnoreList(List<Guid> deviceGuids)
		{
			var devicePaths = new List<string>();
			foreach (var guid in deviceGuids)
			{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
				devicePaths.Add(device.PlaceInTree);
			}

			FiresecSerializedClient.AddToIgnoreList(devicePaths);
		}

		public void RemoveFromIgnoreList(List<Guid> deviceGuids)
		{
			var devicePaths = new List<string>();
			foreach (var guid in deviceGuids)
			{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
				devicePaths.Add(device.PlaceInTree);
			}

			FiresecSerializedClient.RemoveFromIgnoreList(devicePaths);
		}

		public void ResetStates(List<ResetItem> resetItems)
		{
			var firesecResetHelper = new FiresecResetHelper(FiresecSerializedClient);
			firesecResetHelper.ResetStates(resetItems);
		}

		public void SetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				int reguestId = 0;
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "SetZoneToGuard", localZoneNo.ToString(), ref reguestId);
			}
		}

		public void UnSetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == secPanelUID);
			if (device != null)
			{
				int reguestId = 0;
				FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "UnSetZoneFromGuard", localZoneNo.ToString(), ref reguestId);
			}
		}

		public void AddUserMessage(string message)
		{
			FiresecSerializedClient.AddUserMessage(message);
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			var device = ConfigurationCash.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				FiresecSerializedClient.ExecuteCommand(device.PlaceInTree, methodName);
			}
			var operationResult = new OperationResult<bool>()
			{
				Result = false,
				HasError = true,
				Error = "Не найдено устройство по идентификатору"
			};
			return operationResult;
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return FiresecSerializedClient.CheckHaspPresence();
		}

		public List<JournalRecord> ConvertJournal()
		{
			var journalRecords = new List<JournalRecord>();

			var document = FiresecSerializedClient.ReadEvents(0, 10000).Result;
			if (document == null || document.Journal == null || document.Journal.Count() == 0)
				return journalRecords;

			foreach (var innerJournalItem in document.Journal)
			{
				var journalRecord = JournalConverter.Convert(innerJournalItem);
				journalRecords.Add(journalRecord);
			}
			return journalRecords;
		}
	}
}