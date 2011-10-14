﻿using System;
using System.Windows.Data;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.Converters
{
    public class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StateType stateType = (StateType) value;
            string icon = EnumsConverter.StateToIcon(stateType);
            if (icon != null)
            {
                return FiresecClient.FileHelper.GetIconFilePath(icon + ".ico");
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}