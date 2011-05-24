﻿using System;
using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace LibraryModule.ViewModels
{
    class StatesListViewModel : DialogContent
    {
        public StatesListViewModel()
        {
            Title = "Список состояний";
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            Initialize();
            AddCommand = new RelayCommand(OnAdd);
            AddCommand = new RelayCommand(OnAdd);
        }

        private readonly DeviceViewModel _selectedDevice;

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        private ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();
            for (var stateId = 0; stateId < 9; stateId++)
            {
                if (_selectedDevice.States.FirstOrDefault(x => (x.Id == Convert.ToString(stateId)) && (!x.IsAdditional)) != null) continue;
                var frames = new ObservableCollection<FrameViewModel> { new FrameViewModel(Helper.EmptyFrame, 300, 0) };
                var stateViewModel = new StateViewModel(Convert.ToString(stateId), _selectedDevice, false, frames);
                States.Add(stateViewModel);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedState == null) return;
            _selectedDevice.States.Add(SelectedState);
            States.Remove(SelectedState);
            _selectedDevice.SortStates();
            LibraryViewModel.Current.Update();
        }
    }
}
