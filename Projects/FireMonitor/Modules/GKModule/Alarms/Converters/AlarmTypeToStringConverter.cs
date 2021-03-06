﻿using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class AlarmTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XAlarmType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}