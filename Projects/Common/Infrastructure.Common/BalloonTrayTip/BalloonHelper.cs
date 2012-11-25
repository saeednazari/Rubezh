﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows;
using System.Drawing;

namespace Infrastructure.Common.BalloonTrayTip
{
    public class BalloonHelper
    {
        public static void Show(string title, string text, System.Windows.Media.Brush clr)
        {
            var balloonToolTipViewModel = new BalloonToolTipViewModel(title, text, clr);
            DialogService.ShowTrayWindow(balloonToolTipViewModel);
        }

        public static void ShowConflagration(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.Tomato);
        }

        public static void ShowWarning(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.Goldenrod);
        }

        public static void ShowNotification(string title, string text)
        {
            Show(title, text, System.Windows.Media.Brushes.CornflowerBlue);
        }
    }
}