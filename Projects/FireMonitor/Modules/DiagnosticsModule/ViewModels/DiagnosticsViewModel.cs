﻿using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using FiresecAPI;
using System.Diagnostics;
using System.Text;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            Test1Command = new RelayCommand(OnTest1);
            Test2Command = new RelayCommand(OnTest2);
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
            var stringBuilder = new StringBuilder();
            foreach (var device in FiresecManager.Devices)
            {
                if (device.PlaceInTree == null)
                    stringBuilder.AppendLine(device.PresentationAddress);
            }
            Text = stringBuilder.ToString();
        }

        public RelayCommand Test2Command { get; private set; }
        void OnTest2()
        {
        }
    }
}