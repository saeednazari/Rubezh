﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using System.Diagnostics;
using System.Windows;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FireMonitor.ViewModels;
using System.Windows.Threading;
using Infrastructure.Events;
using Infrastructure;

namespace FireMonitor
{
	public static class ProgressWatcher
	{
		static ProgressViewModel progressViewModel = new ProgressViewModel();
		static DispatcherTimer ClosingTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };

		public static void Run()
		{
			ClosingTimer.Tick += new EventHandler(ClosingTimer_Tick);
			FiresecManager.FiresecDriver.Watcher.Progress += new Func<int, string, int, int, bool>(Watcher_Progress);
		}

		static bool Watcher_Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);

			SafeCall(() =>
			{
				if (FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice.DeviceState.StateType == StateType.Unknown)
				{
					progressViewModel.ProgressItems.Add(comment);
					progressViewModel.SelectedProgressItem = progressViewModel.ProgressItems.LastOrDefault();
					if (!progressViewModel.IsShown)
					{
						DialogService.ShowWindow(progressViewModel);
						progressViewModel.IsShown = true;
						ClosingTimer.Start();
					}
				}
				Trace.WriteLine(stage.ToString() + " - " + comment + " - " + percentComplete.ToString() + " - " + bytesRW.ToString());
			});
			return true;
		}

		static void ClosingTimer_Tick(object sender, EventArgs e)
		{
			OnDevicesStateChanged(null);
		}

		static void OnDevicesStateChanged(object obj)
		{
			if (FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice.DeviceState.StateType != StateType.Unknown)
			{
				ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
				ClosingTimer.Stop();
				progressViewModel.Close();
			}
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}