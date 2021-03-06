﻿using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class JournalDescriptionStateViewModel : BaseViewModel
    {
        public JournalDescriptionStateViewModel(JournalDescriptionState xEvent)
        {
            JournalDescriptionState = xEvent;
        }

        public JournalDescriptionState JournalDescriptionState { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
