﻿using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace GKModule.Converters
{
	public class AlarmTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XAlarmType)value)
			{
				case XAlarmType.NPT:
					return "DarkRed";

				case XAlarmType.Fire1:
					return "Red";

				case XAlarmType.Fire2:
					return "Red";

				case XAlarmType.Attention:
					return "Orange";

				case XAlarmType.Failure:
					return "Yellow";

				case XAlarmType.Ignore:
					return "Wheat";

				case XAlarmType.Info:
					return "SkyBlue";

				case XAlarmType.Service:
					return "SkyBlue";

				case XAlarmType.Auto:
					return "Yellow";

				default:
					return "Transparent";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}