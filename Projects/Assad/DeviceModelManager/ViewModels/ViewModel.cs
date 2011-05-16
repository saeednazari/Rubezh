﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using Infrastructure.Common;

namespace DeviveModelManager
{
    public class ViewModel : BaseViewModel
    {
        public static string StaticVersion { get; set; }

        public ViewModel()
        {
            GenarateCommand = new RelayCommand(OnGenarateCommand);
            Version = "5";
        }

        public RelayCommand GenarateCommand { get; private set; }
        void OnGenarateCommand()
        {
            AssadTreeBuilder assadTreeBuilder = new AssadTreeBuilder();
            assadTreeBuilder.Build();
            TreeItem RootTreeItem = assadTreeBuilder.RootTreeItem;
            Devices = new ObservableCollection<TreeItem>();
            Devices.Add(RootTreeItem);
            return;
        }

        ObservableCollection<TreeItem> devices;
        public ObservableCollection<TreeItem> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        TreeItem selectedDevice;
        public TreeItem SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        string version;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                StaticVersion = version;
                OnPropertyChanged("Version");
            }
        }
    }
}
