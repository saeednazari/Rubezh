﻿using System;
using System.Windows;
using System.Windows.Data;
using FiresecAPI.Models;

namespace InstructionsModule.Converters
{
    public class InstructionTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (InstructionType) value == InstructionType.Details ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}