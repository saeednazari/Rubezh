﻿using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RM_4_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RM_4,
				UID = new Guid("3cb0e7fb-670f-4f32-8123-4b310aee1db8"),
				Name = "РМ-4",
				ShortName = "РМ-4",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}