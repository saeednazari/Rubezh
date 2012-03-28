﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Controls.MessageBox;
using GroupControllerModule.Models;
using Infrastructure;
using Infrastructure.Common;
using XFiresecAPI;

namespace GroupControllerModule.ViewModels
{
    public class ZonesViewModel : RegionViewModel, IEditingViewModel
    {
        public ZoneDevicesViewModel ZoneDevices { get; set; }

        public ZonesViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            ZoneDevices = new ZoneDevicesViewModel();

            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in XManager.DeviceConfiguration.Zones
                orderby zone.No
                select new ZoneViewModel(zone));

            if (Zones.Count > 0)
                SelectedZone = Zones[0];
        }

        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                if (value != null)
                    ZoneDevices.Initialize(value.XZone);
                else
                    ZoneDevices.Clear();

                OnPropertyChanged("SelectedZone");
            }
        }

        bool CanEditDelete()
        {
            return SelectedZone != null;
        }

        bool CanDeleteAll()
        {
            return Zones.Count > 0;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var zoneDetailsViewModel = new ZoneDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel))
            {
                XManager.DeviceConfiguration.Zones.Add(zoneDetailsViewModel.XZone);
                Zones.Add(new ZoneViewModel(zoneDetailsViewModel.XZone));

                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.XZone.PresentationName);
            if (dialogResult == MessageBoxResult.Yes)
            {
                XManager.DeviceConfiguration.Zones.Remove(SelectedZone.XZone);
                Zones.Remove(SelectedZone);
                SelectFirstZone();
                ZoneDevices.UpdateAvailableDevices();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.XZone);
            if (ServiceFactory.UserDialogs.ShowModalWindow(zoneDetailsViewModel))
            {
                SelectedZone.XZone = zoneDetailsViewModel.XZone;
                SelectedZone.Update();

                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        void SelectFirstZone()
        {
            if (Zones.Count > 0)
                SelectedZone = Zones[0];
            else
                SelectedZone = null;
        }

        public override void OnShow()
        {
            SelectedZone = SelectedZone;
            ServiceFactory.Layout.ShowMenu(new ZonesMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}