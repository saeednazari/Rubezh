﻿using System;
using System.Windows.Data;
using FiresecAPI;
using XFiresecAPI;

namespace Controls.Converters
{
	public class XStateTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XStateBit)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateBit)value;
		}
	}
}