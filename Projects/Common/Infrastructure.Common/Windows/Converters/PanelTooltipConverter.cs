﻿using System;
using System.Windows.Data;

namespace Infrastructure.Common.Windows.Converters
{
	public class PanelTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var isLeftPanel = parameter == null || System.Convert.ToBoolean(parameter);
			var isVisible = System.Convert.ToBoolean(value);
			return isLeftPanel ?
				(isVisible ? "Свернуть дерево" : "Развернуть планы") :
				(isVisible ? "Свернуть планы" : "Развернуть дерево");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
