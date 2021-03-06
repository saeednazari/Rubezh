﻿using System;
using XFiresecAPI;

namespace GKProcessor
{
	public static class RSR2_HandDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0xD8,
				DriverType = XDriverType.RSR2_HandDetector,
				UID = new Guid("151881A2-0A39-4609-870F-4A84B2F8A4C8"),
				Name = "Ручной извещатель ИПР513-11 RSR2",
				ShortName = "ИПР RSR2",
				HasZone = true,
				IsPlaceable = true
			};

			GKDriversHelper.AddAvailableStateBits(driver, XStateBit.Fire2);
			GKDriversHelper.AddAvailableStateClasses(driver, XStateClass.Fire2);

			return driver;
		}
	}
}