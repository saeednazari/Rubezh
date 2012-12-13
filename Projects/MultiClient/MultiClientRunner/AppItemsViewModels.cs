﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using MuliclientAPI;
using System.Diagnostics;

namespace MultiClientRunner
{
    public class AppItemsViewModels : BaseViewModel
    {
        public AppItemsViewModels()
        {
            AppItems = new ObservableCollection<AppItem>();
        }

        public ObservableCollection<AppItem> AppItems { get; private set; }

        AppItem _selectedAppItem;
        public AppItem SelectedAppItem
        {
            get { return _selectedAppItem; }
            set
            {
                foreach (var appItem in AppItems)
                {
                    var windowSize = appItem.GetWindowSize();
                    Trace.WriteLine(windowSize.Left);
                }

                if (_selectedAppItem != value)
                {
                    WindowSize windowSize = null;
                    if (_selectedAppItem != null)
                    {
                        windowSize = _selectedAppItem.GetWindowSize();
                    }

                    _selectedAppItem = value;
                    OnPropertyChanged("SelectedAppItem");
                    foreach (var appItem in AppItems)
                    {
                        if (appItem != value)
                        {
                            appItem.Hide();
                        }
                    }
                    if (value != null)
                    {
                        value.Show(windowSize);
                    }
                }
            }
        }
    }
}