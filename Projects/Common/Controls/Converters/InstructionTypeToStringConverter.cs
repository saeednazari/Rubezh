﻿using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace Controls.Converters
{
    internal class InstructionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((InstructionType) value).ToDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}