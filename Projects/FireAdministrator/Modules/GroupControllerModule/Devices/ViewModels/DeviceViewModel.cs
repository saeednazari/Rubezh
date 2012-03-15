﻿using System.Collections.ObjectModel;
using System.Linq;
using Controls.MessageBox;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public XDevice Device { get; private set; }
        public PropertiesViewModel PropertiesViewModel { get; private set; }

        public DeviceViewModel(XDevice xDevice, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);

            Children = new ObservableCollection<DeviceViewModel>();

            Source = sourceDevices;
            Device = xDevice;
            PropertiesViewModel = new PropertiesViewModel(xDevice);
        }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
        }

        public XDriver Driver
        {
            get { return Device.Driver; }
            set
            {
                Device.Driver = value;
            }
        }

        public string Address
        {
            get { return Device.Address; }
            set
            {
                if (Device.Parent.Children.Where(x => x != Device).Any(x => x.Address == value))
                {
                    MessageBoxService.Show("Устройство с таким адресом уже существует");
                }
                else
                {
                    Device.Address = value;
                    if (Driver.IsChildAddressReservedRange)
                    {
                        foreach (var deviceViewModel in Children)
                        {
                            deviceViewModel.OnPropertyChanged("Address");
                        }
                    }
                }
                OnPropertyChanged("Address");
            }
        }

        public bool CanEditAddress
        {
            get
            {
                if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != XDriverType.MRK_30)
                    return false;
                return Driver.CanEditAddress;
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");
            }
        }

        public bool CanAdd()
        {
            return (Driver.Children.Count > 0);
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new NewDeviceViewModel(this)))
            {
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        bool CanRemove()
        {
            return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.AutoChild == Driver.UID);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            Parent.IsExpanded = false;
            Parent.Device.Children.Remove(Device);
            Parent.Children.Remove(this);
            Parent.Update();
            Parent.IsExpanded = true;
            Parent = null;

            XManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.XDevicesChanged = true;
        }

        bool CanShowProperties()
        {
            return false;
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            if (ServiceFactory.UserDialogs.ShowModalWindow(new DeviceDetailsViewModel(Device)))
                ServiceFactory.SaveService.XDevicesChanged = true;

            OnPropertyChanged("PresentationZone");
        }

        public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
        public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
        public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
    }
}