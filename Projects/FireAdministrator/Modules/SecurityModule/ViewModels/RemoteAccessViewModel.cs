﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using SecurityModule.Events;

namespace SecurityModule.ViewModels
{
    public class RemoteAccessViewModel : BaseViewModel
    {
        public RemoteAccessViewModel(RemoteAccess remoteAccess)
        {
            RemoteAccessTypes = new List<RemoteAccessTypeViewModel>();
            foreach (RemoteAccessType remoteAccessType in Enum.GetValues(typeof(RemoteAccessType)))
            {
                RemoteAccessTypes.Add(new RemoteAccessTypeViewModel(remoteAccessType));
            }
            RemoteAccessTypes.Find(x => x.RemoteAccessType == remoteAccess.RemoteAccessType).IsActive = true;

            HostNameOrAddressList = new ObservableCollection<string>();
            if (remoteAccess.HostNameOrAddressList.IsNotNullOrEmpty())
            {
                remoteAccess.HostNameOrAddressList.ForEach(x => HostNameOrAddressList.Add(x));
            }

            Initialize();
        }

        void Initialize()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);

            ServiceFactory.Events.GetEvent<RemoteAccessTypeChecked>().Subscribe(OnRemoteAccessTypeChecked);
        }

        public List<RemoteAccessTypeViewModel> RemoteAccessTypes { get; private set; }
        public ObservableCollection<string> HostNameOrAddressList { get; private set; }

        string _selectedHostNameOrAddress;
        public string SelectedHostNameOrAddress
        {
            get { return _selectedHostNameOrAddress; }
            set
            {
                _selectedHostNameOrAddress = value;
                OnPropertyChanged("SelectedHostNameOrAddress");
            }
        }

        public RemoteAccess GetModel()
        {
            var remoteAccess = new RemoteAccess();
            remoteAccess.RemoteAccessType = RemoteAccessTypes.Find(x => x.IsActive).RemoteAccessType;
            if (remoteAccess.RemoteAccessType == RemoteAccessType.SelectivelyAllowed)
            {
                remoteAccess.HostNameOrAddressList = new List<string>(HostNameOrAddressList.ToList());
            }

            return remoteAccess;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var remoteMachineViewModel = new RemoteMachineViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(remoteMachineViewModel))
            {
                if (string.IsNullOrEmpty(remoteMachineViewModel.HostNameOrAddress) == false &&
                    HostNameOrAddressList.Any(x => x == remoteMachineViewModel.HostNameOrAddress) == false)
                    HostNameOrAddressList.Add(remoteMachineViewModel.HostNameOrAddress);
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            HostNameOrAddressList.Remove(SelectedHostNameOrAddress);
        }

        bool CanRemove()
        {
            return SelectedHostNameOrAddress != null;
        }

        public void OnRemoteAccessTypeChecked(RemoteAccessTypeViewModel remoteAccessTypeViewModel)
        {
            RemoteAccessTypes.FindAll(x => x != remoteAccessTypeViewModel).ForEach(x => x.IsActive = false);
        }
    }
}