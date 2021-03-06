﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace InstructionsModule.ViewModels
{
	public class InstructionsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public InstructionsViewModel()
		{
			Menu = new InstructionsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			DeleteAllCommand = new RelayCommand(OnDeleteAll, CanRemoveAll);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Instructions = new ObservableCollection<InstructionViewModel>();

			foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
			{
				if (instruction.InstructionType == InstructionType.Details)
				{
					instruction.Devices = new List<Guid>(instruction.Devices.Where(deviceGuid => FiresecManager.Devices.Any(x => x.UID == deviceGuid)));
					instruction.ZoneUIDs = new List<Guid>(instruction.ZoneUIDs.Where(zoneUID => FiresecManager.Zones.Any(x => x.UID == zoneUID)));
				}
				Instructions.Add(new InstructionViewModel(instruction));
			}

			SelectedInstruction = Instructions.FirstOrDefault();
		}

		ObservableCollection<InstructionViewModel> _instructions;
		public ObservableCollection<InstructionViewModel> Instructions
		{
			get { return _instructions; }
			set
			{
				_instructions = value;
				OnPropertyChanged("Instructions");
			}
		}

		InstructionViewModel _selectedInstruction;
		public InstructionViewModel SelectedInstruction
		{
			get { return _selectedInstruction; }
			set
			{
				_selectedInstruction = value;
				OnPropertyChanged("SelectedInstruction");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var instructionDetailsViewModel = new InstructionDetailsViewModel();
			if (DialogService.ShowModalWindow(instructionDetailsViewModel))
			{
                FiresecManager.SystemConfiguration.Instructions.Add(instructionDetailsViewModel.Instruction);
                var instructionViewModel = new InstructionViewModel(instructionDetailsViewModel.Instruction);
                Instructions.Add(instructionViewModel);
                SelectedInstruction = instructionViewModel;
				ServiceFactory.SaveService.InstructionsChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var instructionDetailsViewModel = new InstructionDetailsViewModel(SelectedInstruction.Instruction);
			if (DialogService.ShowModalWindow(instructionDetailsViewModel))
			{
				SelectedInstruction.Update();
				ServiceFactory.SaveService.InstructionsChanged = true;
			}
		}

		bool CanEditRemove()
		{
			return SelectedInstruction != null;
		}

		bool CanRemoveAll()
		{
			return (Instructions.IsNotNullOrEmpty());
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecManager.SystemConfiguration.Instructions.Remove(SelectedInstruction.Instruction);
			Instructions.Remove(SelectedInstruction);
			SelectedInstruction = Instructions.FirstOrDefault();
			ServiceFactory.SaveService.InstructionsChanged = true;
		}

		public RelayCommand DeleteAllCommand { get; private set; }
		void OnDeleteAll()
		{
			SelectedInstruction = null;
			Instructions.Clear();
			FiresecManager.SystemConfiguration.Instructions.Clear();
			ServiceFactory.SaveService.InstructionsChanged = true;
		}

		#region ISelectable<Guid> Members
		public void Select(Guid instructionUID)
		{
            if (instructionUID != Guid.Empty)
                SelectedInstruction = Instructions.FirstOrDefault(x => x.Instruction.UID == instructionUID);
		}
		#endregion

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Удалить все инструкции", DeleteAllCommand, "/Controls;component/Images/BDeleteAll.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}