﻿using System;
using System.Windows;
using Common;
using FireAdministrator.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Initialize(new LayoutService(), new ProgressService(), new ValidationService());
			var assembly = GetType().Assembly;
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(assembly, "DataTemplates/Dictionary.xaml"));
			if (ServiceFactory.LoginService.ExecuteConnect())
			{
				try
				{
					CreateModules();

					LoadingService.ShowLoading("Чтение конфигурации", 4);
					LoadingService.AddCount(GetModuleCount() + 6);

					LoadingService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					LoadingService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration("Administrator/Configuration");

					GKDriversCreator.Create();
					BeforeInitialize(true);

					//LoadingService.DoStep("Старт полинга сервера");
					//FiresecManager.StartPoll(true);

					LoadingService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Adm_ViewConfig) == false)
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					else if (Application.Current != null)
					{
						var shell = new AdministratorShellViewModel();
						ServiceFactory.MenuService = new MenuService((vm) => ((MenuViewModel)shell.Toolbar).ExtendedMenu = vm);
						RunShell(shell);
					}
					LoadingService.Close();

					AterInitialize();

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosed);
					MutexHelper.KeepAlive();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.Initialize");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return;
			}
		}

		private void OnConfigurationChanged(object obj)
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
			ServiceFactory.ContentService.Invalidate();
			InitializeModules();
			LoadingService.Close();
		}
		private void OnConfigurationClosed(object obj)
		{
			ServiceFactory.ContentService.Close();
		}

		private void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			if (Application.Current != null)
				Application.Current.Shutdown();
		}
	}
}