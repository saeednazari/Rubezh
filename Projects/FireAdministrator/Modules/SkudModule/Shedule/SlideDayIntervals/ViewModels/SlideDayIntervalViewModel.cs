﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalViewModel : BaseViewModel
	{
		public SKDSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalViewModel(SKDSlideDayInterval slideDayInterval)
		{
			SlideDayInterval = slideDayInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervals = new ObservableCollection<SlideDayIntervalPartViewModel>();
			foreach (var timeIntervalUID in slideDayInterval.TimeIntervalUIDs)
			{
				var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
				if (timeInterval != null)
				{
					var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
					TimeIntervals.Add(slideDayIntervalPartViewModel);
				}
			}
		}

		public ObservableCollection<SlideDayIntervalPartViewModel> TimeIntervals { get; private set; }

		SlideDayIntervalPartViewModel _selectedTimeInterval;
		public SlideDayIntervalPartViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Update()
		{
			OnPropertyChanged("SlideDayInterval");

			SlideDayInterval.TimeIntervalUIDs = new List<Guid>();
			foreach (var timeInterval in TimeIntervals)
			{
				SlideDayInterval.TimeIntervalUIDs.Add(timeInterval.SelectedTimeInterval.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault();
			SlideDayInterval.TimeIntervalUIDs.Add(timeInterval.UID);
			var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
			TimeIntervals.Add(slideDayIntervalPartViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideDayInterval.TimeIntervalUIDs.Add(SelectedTimeInterval.TimeInterval.UID);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null;
		}
	}
}