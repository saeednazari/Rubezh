﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Controls.Converters;

namespace GKModule.ViewModels
{
	public class DeviceTooltipViewModel : BaseViewModel
	{
		public XState State { get; private set; }
		public XDevice Device { get; private set; }

		public DeviceTooltipViewModel(XDevice device)
		{
			Device = device;
			State = device.State;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
		}

		public void OnStateChanged()
		{
			OnPropertyChanged("State");
			StateClasses.Clear();
			foreach (var stateClass in State.StateClasses)
			{
				StateClasses.Add(new XStateClassViewModel(State.Device, stateClass));
			}
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
	}
}