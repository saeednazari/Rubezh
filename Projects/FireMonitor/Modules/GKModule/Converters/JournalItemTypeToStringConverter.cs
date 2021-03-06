﻿using System;
using System.Windows.Data;
using GKProcessor;
using FiresecAPI;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class JournalItemTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((JournalItemType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}