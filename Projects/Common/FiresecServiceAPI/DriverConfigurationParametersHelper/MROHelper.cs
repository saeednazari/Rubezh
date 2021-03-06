﻿using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MROHelper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO);
			driver.HasConfigurationProperties = true;

			var property1 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x82,
				Name = "Количество повторов",
				Caption = "Количество повторов",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 255
			};
			driver.Properties.Add(property1);

			var property2 = new DriverProperty()
			{
				IsAUParameter = true,
				No = 0x88,
				Name = "AU_Delay",
				Caption = "Задержка включения МРО, с",
				Default = "0",
				DriverPropertyType = DriverPropertyTypeEnum.IntType,
				Min = 0,
				Max = 1200
			};
			driver.Properties.Add(property2);

			//ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x80, "источник воспроизведения (только чтение)", 1, "память", "линейный вход");
		}
	}
}