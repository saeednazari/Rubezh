﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Firesec;

namespace DevicesModule.Converters
{
    public class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            State state = (State)value;
            string icon = StateToIcon(state);
            if (icon != null)
            {
                return "../Icons/" + icon + ".ico";
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

        string StateToIcon(State state)
        {
            switch (state.StateType)
            {
                case StateType.Alarm:
                    return "DS_Critical";

                case StateType.Warning:
                    return "DS_Warning";

                case StateType.Failure:
                    return "DS_Error";

                case StateType.Service:
                    return "DS_ServiceRequired";

                case StateType.Off:
                    return "DS_Mute";

                case StateType.Unknown:
                    return "DS_Unknown";

                case StateType.Info:
                    return "DS_Normal";

                case StateType.Norm:
                    return null;

                default:
                    return null;
            }
        }
    }
}