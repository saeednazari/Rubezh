﻿using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class RolesMenuViewModel : BaseViewModel
    {
        public RolesMenuViewModel(RelayCommand addCommand, RelayCommand deleteCommand, RelayCommand editCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
    }
}