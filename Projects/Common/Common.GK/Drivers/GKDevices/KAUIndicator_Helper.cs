﻿using System;
using XFiresecAPI;

namespace Common.GK
{
    public class KAUIndicator_Helper
    {
        public static XDriver Create()
        {
            var driver = new XDriver()
            {
                DriverTypeNo = 0x103,
                DriverType = XDriverType.KAUIndicator,
				UID = new Guid("17A2B7D1-CB62-4AF7-940E-BC30B004B0D0"),
                Name = "Индикатор КАУ",
				ShortName = "Индикатор КАУ",
                CanEditAddress = false,
                HasAddress = false,
                IsAutoCreate = true,
                MinAddress = 1,
                MaxAddress = 1,
				IsDeviceOnShleif = false
            };

            var modeProperty = new XDriverProperty()
            {
                No = 0,
                Name = "Mode",
                Caption = "Режим работы",
                ToolTip = "Режим работы индикатора",
                Default = 1,
                DriverPropertyType = XDriverPropertyTypeEnum.EnumType,
                IsInternalDeviceParameter = true
            };
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Выключено", Value = 0 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Включено", Value = 1 });
			modeProperty.Parameters.Add(new XDriverPropertyParameter() { Name = "Мерцает", Value = 2 });
            driver.Properties.Add(modeProperty);

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 1,
                    Name = "OnDuration",
                    Caption = "Продолжительность горения для режима 2",
                    ToolTip = "Продолжительность горения для режима 2",
                    Default = 0,
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    IsInternalDeviceParameter = true
                }
                );

            driver.Properties.Add(
                new XDriverProperty()
                {
                    No = 2,
                    Name = "OffDuration",
                    Caption = "Продолжительность гашения для режима 2",
                    ToolTip = "Продолжительность гашения для режима 2",
                    DriverPropertyType = XDriverPropertyTypeEnum.IntType,
                    Default = 0,
                    IsInternalDeviceParameter = true
                }
                );

            return driver;
        }
    }
}