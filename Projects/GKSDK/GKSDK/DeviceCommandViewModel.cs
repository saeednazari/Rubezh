﻿using XFiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace GKSDK
{
	public class DeviceCommandViewModel : BaseViewModel
	{
		XDevice Device;

		public DeviceCommandViewModel(XDevice device)
		{
			ExecuteCommand = new RelayCommand(OnExecute);
			Device = device;
		}

		public string ConmmandName { get; private set; }

		public RelayCommand ExecuteCommand { get; private set; }
		void OnExecute()
		{
		}
	}
}