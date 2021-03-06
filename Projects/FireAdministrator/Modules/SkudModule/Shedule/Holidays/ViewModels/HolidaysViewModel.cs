﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Ribbon;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class HolidaysViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public HolidaysViewModel()
		{
			Menu = new HolidaysMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			RegisterShortcuts();
			SetRibbonItems();

			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in SKDManager.SKDConfiguration.Holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
			SelectedHoliday = Holidays.FirstOrDefault();
		}

		public ObservableCollection<HolidayViewModel> Holidays { get; private set; }

		HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged("SelectedHoliday");
			}
		}

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
				var holidayViewModel = Holidays.FirstOrDefault(x => x.Holiday.UID == holidayUID);
				if (holidayViewModel != null)
				{
					SelectedHoliday = holidayViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel();
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SKDManager.SKDConfiguration.Holidays.Add(holidayDetailsViewModel.Holiday);
				var holidayViewModel = new HolidayViewModel(holidayDetailsViewModel.Holiday);
				Holidays.Add(holidayViewModel);
				SelectedHoliday = holidayViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return Holidays.Count < 100;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.SKDConfiguration.Holidays.Remove(SelectedHoliday.Holiday);
			Holidays.Remove(SelectedHoliday);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SelectedHoliday.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedHoliday != null;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}
	}
}