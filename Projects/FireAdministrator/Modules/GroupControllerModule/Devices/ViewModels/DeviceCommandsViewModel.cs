﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Converter;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Windows.Input;
using Common;
using System;
using System.Diagnostics;

namespace GKModule.Models
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration);
            WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);

			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			GetAllParametersCommand = new RelayCommand(OnGetAllParameters);
			SetAllParametersCommand = new RelayCommand(OnSetAllParameters, CanSetAllParameters);
            GetSingleParameterCommand = new RelayCommand(OnGetSingleParameter, CanGetSetSingleParameter);
            SetSingleParameterCommand = new RelayCommand(OnSetSingleParameter, CanSetSingleParameter);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = DeviceBytesHelper.GetDeviceInfo(SelectedDevice.Device);
			if (!string.IsNullOrEmpty(result))
			{
				MessageBoxService.Show(result);
			}
			else
			{
				MessageBoxService.Show("Ошибка при запросе информации об устройстве");
			}
		}
		bool CanShowInfo()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.DriverType == XDriverType.KAU ||
				SelectedDevice.Device.Driver.DriverType == XDriverType.GK));
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			DeviceBytesHelper.WriteDateTime(SelectedDevice.Device);
		}
		bool CanSynchroniseTime()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand ReadJournalCommand { get; private set; }
		void OnReadJournal()
		{
			var journalViewModel = new JournalViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(journalViewModel);
		}
		bool CanReadJournal()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (ValidateConfiguration())
			{
				BinConfigurationWriter.WriteConfig();
			}
			else
			{
				if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") == System.Windows.MessageBoxResult.Yes)
				{
					BinConfigurationWriter.WriteConfig();
				}
			}
		}
        bool CanWriteConfig()
        {
            return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
        }

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var device = SelectedDevice.Device;
			if (device.Driver.DriverType != XDriverType.KAU)
				return;
			BinConfigurationReader.ReadConfiguration(device);
		}

		public RelayCommand GetAllParametersCommand { get; private set; }
		void OnGetAllParameters()
		{
			ParametersHelper.GetAllParameters();
		}

		public RelayCommand SetAllParametersCommand { get; private set; }
		void OnSetAllParameters()
		{
			ParametersHelper.SetAllParameters();
		}
		bool CanSetAllParameters()
		{
			return CanGetSetSingleParameter() && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
		}

		public RelayCommand GetSingleParameterCommand { get; private set; }
		void OnGetSingleParameter()
		{
			ParametersHelper.GetSingleParameter(SelectedDevice.Device);
		}

        public RelayCommand SetSingleParameterCommand { get; private set; }
        void OnSetSingleParameter()
		{
            ParametersHelper.SetSingleParameter(SelectedDevice.Device);
		}
		bool CanSetSingleParameter()
		{
			return CanGetSetSingleParameter() && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
		}

        bool CanGetSetSingleParameter()
        {
            if(SelectedDevice == null)
                return false;
            if(SelectedDevice.Device.Driver.DriverType == XDriverType.System)
                return false;
            if(SelectedDevice.Device.Driver.IsGroupDevice)
                return false;
            return true;
        }

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
			MessageBoxService.Show("Функция не реализована");
		}
		bool CanUpdateFirmwhare()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == XDriverType.KAU && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors("GK"))
			{
				if (validationResult.CannotSave("GK") || validationResult.CannotWrite("GK"))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}
	}
}