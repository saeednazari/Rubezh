﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;

namespace Infrastructure.Common.Windows
{
	public static class DialogService
	{
		public static Window GetActiveWindow()
		{
			if (!Application.Current.Dispatcher.CheckAccess())
				return (Window)Application.Current.Dispatcher.Invoke((Func<Window>)GetActiveWindow);
			var window = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
			return window ?? ApplicationService.ApplicationWindow;
		}

		public static bool ShowModalWindow(WindowBaseViewModel model, bool allowsTransparency = true)
		{
			try
			{
				WindowBaseView win = new WindowBaseView(model);
				win.AllowsTransparency = allowsTransparency;
				PrepareWindow(model);
				bool? result = win.ShowDialog();
				return result.HasValue ? result.Value : false;
			}
			catch (Exception e)
			{
				Logger.Error(e, "DialogService.ShowModalWindow");
			}
			return false;
		}
		public static void ShowWindow(WindowBaseViewModel model)
		{
			if (!FindWindowIdentity(model))
			{
				WindowBaseView win = new WindowBaseView(model);
				PrepareWindow(model);
				win.Show();
			}
		}

		private static List<IWindowIdentity> _openedWindows = new List<IWindowIdentity>();
		private static bool FindWindowIdentity(WindowBaseViewModel model)
		{
			var identityModel = model as IWindowIdentity;
			if (identityModel != null)
			{
				WindowBaseViewModel openedWindow = (WindowBaseViewModel)_openedWindows.FirstOrDefault(x => x.Guid == identityModel.Guid);
				if (openedWindow != null && openedWindow.Surface != null)
				{
					openedWindow.Surface.Activate();
					return true;
				}
				model.Closed += (s, e) => _openedWindows.Remove((IWindowIdentity)s);
				_openedWindows.Add(identityModel);
			}
			return false;
		}
		private static void PrepareWindow(WindowBaseViewModel model)
		{
			SetWindowProperty(model);
			UpdateWindowSize(model);
		}
		private static void SetWindowProperty(WindowBaseViewModel model)
		{
			model.Surface.Owner = GetActiveWindow();
			model.Surface.ShowInTaskbar = model.Surface.Owner == null;
			model.Surface.WindowStartupLocation = model.Surface.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
		}
		private static void UpdateWindowSize(WindowBaseViewModel model)
		{
			try
			{
				string key = model.GetType().AssemblyQualifiedName;
				var isSaveSize = model.GetType().GetCustomAttributes(typeof(SaveSizeAttribute), true).Length > 0;
				if (isSaveSize && Properties.Settings.Default.WindowSizeState != null && Properties.Settings.Default.WindowSizeState.ContainsKey(key))
				{
					var val = Properties.Settings.Default.WindowSizeState[key];
					var values = val.Split(';');
					Rect rect = new Rect()
					{
						X = Int32.Parse(values[0]),
						Y = Int32.Parse(values[1]),
						Width = Int32.Parse(values[2]),
						Height = Int32.Parse(values[3])
					};
					model.Surface.Top = rect.X;
					model.Surface.Left = rect.Y;
					model.Surface.Width = rect.Width;
					model.Surface.Height = rect.Height;
					model.Surface.WindowStartupLocation = WindowStartupLocation.Manual;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DialogService.UpdateWindowSize");
			}
			model.Closed -= SaveWindowSize;
			model.Closed += SaveWindowSize;
		}
		private static void SaveWindowSize(object sender, EventArgs e)
		{
			WindowBaseViewModel model = (WindowBaseViewModel)sender;
			string key = model.GetType().AssemblyQualifiedName;
			Rect rect = new Rect(model.Surface.Top, model.Surface.Left, model.Surface.Width, model.Surface.Height);
			var stringRect = rect.X.ToString() + ";" + rect.Y.ToString() + ";" + rect.Width.ToString() + ";" + rect.Height.ToString();
			if (Properties.Settings.Default.WindowSizeState == null)
				Properties.Settings.Default.WindowSizeState = new System.Collections.Specialized.StringDictionary();
			if (Properties.Settings.Default.WindowSizeState.ContainsKey(key))
				Properties.Settings.Default.WindowSizeState[key] = stringRect;
			else
				Properties.Settings.Default.WindowSizeState.Add(key, stringRect);
			Properties.Settings.Default.Save();
		}

		public static bool IsModal(this Window window)
		{
			return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		}
	}
}