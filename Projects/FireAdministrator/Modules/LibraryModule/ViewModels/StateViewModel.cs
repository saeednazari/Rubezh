﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public static readonly string DefaultClassId = "8";

        public StateViewModel(DeviceLibrary.Models.State state, DeviceViewModel parent)
        {
            SetFrames(state);
            ParentDevice = parent;
            Class = state.Class;
            if (state.Code != null)
            {
                InnerState = ParentDevice.Driver.States.FirstOrDefault(x => x.Code == state.Code);
            }

            Initialize();
        }

        public StateViewModel(InnerState innerState, DeviceViewModel parent)
        {
            SetDefaultFrame();
            ParentDevice = parent;
            Class = innerState.State.Id.ToString();
            InnerState = innerState;

            Initialize();
        }

        public StateViewModel(string classId, DeviceViewModel parent)
        {
            SetDefaultFrame();
            ParentDevice = parent;
            Class = classId;

            Initialize();
        }

        void Initialize()
        {
            RemoveStateCommand = new RelayCommand(OnRemoveState);
        }

        public InnerState InnerState { get; private set; }
        public DeviceViewModel ParentDevice { get; private set; }

        public string Class { get; private set; }

        public string Code
        {
            get
            {
                if (InnerState != null) return InnerState.Code;
                return null;
            }
        }

        public bool IsAdditional
        {
            get
            {
                return InnerState != null;
            }
        }

        public string ClassName
        {
            get
            {
                return new State() { Id = int.Parse(Class) }.ToString();
            }
        }

        public string Name
        {
            get
            {
                var name = new State() { Id = int.Parse(Class) }.ToString();
                if (IsAdditional)
                {
                    name += ". ";
                    var state = ParentDevice.Driver.States.FirstOrDefault(x => x.Code == Code);
                    if (state == null) name += "Unknown";
                    name += state.Name;
                }
                return name;
            }
        }

        public string AdditionalName
        {
            get
            {
                var state = ParentDevice.Driver.States.FirstOrDefault(x => x.Code == Code);
                if (state == null) return "Unknown";
                return state.Name;
            }
        }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if (ParentDevice.SelectedState != this)
                {
                    ParentDevice.SelectedState = this;
                }

                OnPropertyChanged("IsChecked");
            }
        }

        public CanvasesPresenter CanvasesPresenter
        {
            get { return new CanvasesPresenterViewModel(this).CanvasesPresenter; }
        }

        ObservableCollection<FrameViewModel> _frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get
            {
                return _frames;
            }

            private set
            {
                _frames = value;
                OnPropertyChanged("Frames");
            }
        }

        FrameViewModel _selectedFrame;
        public FrameViewModel SelectedFrame
        {
            get
            {
                return _selectedFrame;
            }

            set
            {
                _selectedFrame = value;
                OnPropertyChanged("SelectedFrame");
            }
        }

        void SetDefaultFrame()
        {
            Frames = new ObservableCollection<FrameViewModel>();
            Frames.Add(new FrameViewModel(this));
        }

        void SetFrames(DeviceLibrary.Models.State state)
        {
            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var frame in state.Frames)
            {
                Frames.Add(new FrameViewModel(this, frame));
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            if (Class == DefaultClassId)
            {
                MessageBox.Show("Невозможно удалить базовый рисунок");
            }
            else
            {
                var dialogResult = MessageBox.Show("Удалить выбранное состояние?",
                                                    "Окно подтверждения",
                                                    MessageBoxButton.OKCancel,
                                                    MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    ParentDevice.States.Remove(this);
                }
            }
        }
    }
}
