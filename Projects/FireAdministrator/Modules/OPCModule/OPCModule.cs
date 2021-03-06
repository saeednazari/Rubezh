﻿using System;
using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using OPCModule.Validation;
using OPCModule.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows;
using FiresecAPI;
using Common;

namespace OPCModule
{
	public class OPCModule : ModuleBase, IValidationModule
	{
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;

		public override void CreateViewModels()
		{
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
		}

		public override void Initialize()
		{
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("OPC сервер", "/Controls;component/Images/Tree.png", new List<NavigationItem>()
				{
					new NavigationItem<ShowOPCDeviceEvent, Guid>(OPCDevicesViewModel, "Устройства","/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowOPCZoneEvent, Guid>(OPCZonesViewModel, "Зоны","/Controls;component/Images/Zones.png", null, null, Guid.Empty),
				}),
			};
		}
		public override string Name
		{
			get { return "OPC сервер"; }
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			return Validator.Validate();
		}
		#endregion

		public override bool BeforeInitialize(bool firstTime)
		{
			try
			{
				LoadingService.DoStep("Загрузка драйвера устройств");
				var connectionResult = FiresecManager.InitializeFiresecDriver(false);
				if (connectionResult.HasError)
				{
					MessageBoxService.ShowError(connectionResult.Error);
					return false;
				}
				//LoadingService.DoStep("Синхронизация конфигурации");
				//FiresecManager.FiresecDriver.Synchronyze(false);
				//LoadingService.DoStep("Старт мониторинга");
				//FiresecManager.FiresecDriver.StartWatcher(false, false);
				//FiresecManager.FSAgent.Start();
				return true;
			}
			catch (FiresecException e)
			{
				Logger.Error(e, "OPCModule.BeforeInitialize");
				MessageBoxService.ShowException(e);
				return false;
			}
		}
	}
}