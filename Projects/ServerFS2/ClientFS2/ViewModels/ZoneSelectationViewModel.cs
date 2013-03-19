﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
    public class ZoneSelectationViewModel : SaveCancelDialogViewModel
    {
        Device Device;

        public ZoneSelectationViewModel(Device device)
        {
            Title = "Выбор зоны устройства " + device.PresentationAddressAndName;
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit, CanEdit);

            Device = device;
            IsGuardDevice = (device.Driver.DeviceType == DeviceType.Sequrity);

            Zones = new ObservableCollection<ZoneViewModel>();
            foreach (var zone in from zone in FiresecManager.Zones orderby zone.No select zone)
            {
                var isGuardZone = (zone.ZoneType == ZoneType.Guard);
                if (isGuardZone ^ IsGuardDevice)
                    continue;
                var zoneViewModel = new ZoneViewModel(zone);
                Zones.Add(zoneViewModel);
            }
            if (Device.Zone != null)
                SelectedZone = Zones.FirstOrDefault(x => x.Zone == Device.Zone);
        }

        public bool IsGuardDevice { get; private set; }
        public ObservableCollection<ZoneViewModel> Zones { get; private set; }

        private ZoneViewModel _selectedZone;
        public ZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;
                OnPropertyChanged("SelectedZone");
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        private void OnCreate()
        {
            //var createZoneEventArg = new CreateZoneEventArg();
            //createZoneEventArg.Zone = new Zone();
            //if (IsGuardDevice)
            //    createZoneEventArg.Zone.ZoneType = ZoneType.Guard;
            //else
            //    createZoneEventArg.Zone.ZoneType = ZoneType.Fire;
            //ServiceFactory.Events.GetEvent<CreateZoneEvent>().Publish(createZoneEventArg);
            //if (!createZoneEventArg.Cancel)
            //{
            //    var zoneViewModel = new ZoneViewModel(createZoneEventArg.Zone);
            //    Zones.Add(zoneViewModel);
            //    SelectedZone = zoneViewModel;
            //}
        }

        public RelayCommand EditCommand { get; private set; }
        private void OnEdit()
        {
            //ServiceFactory.Events.GetEvent<EditZoneEvent>().Publish(SelectedZone.Zone.UID);
            //SelectedZone.Update(SelectedZone.Zone);
            //OnPropertyChanged("Zones");
        }
        private bool CanEdit()
        {
            return SelectedZone != null;
        }

        protected override bool CanSave()
        {
            return SelectedZone != null;
        }

        protected override bool Save()
        {
            FiresecManager.FiresecConfiguration.AddDeviceToZone(Device, SelectedZone.Zone);
            return base.Save();
        }
    }
}