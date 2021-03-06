﻿using FiresecAPI.XModels;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceParameterTemplateViewModel : BaseViewModel
	{
		public XDeviceParameterTemplate DeviceParameterTemplate { get; private set; }
		public DeviceParameterViewModel DeviceParameterViewModel { get; private set; }

		public DeviceParameterTemplateViewModel(XDeviceParameterTemplate deviceParameterTemplate)
		{
			DeviceParameterTemplate = deviceParameterTemplate;
			DeviceParameterViewModel = new DeviceParameterViewModel(deviceParameterTemplate.XDevice);
		}

		public XDriver Driver
		{
			get { return DeviceParameterTemplate.XDevice.Driver; }
		}
	}
}