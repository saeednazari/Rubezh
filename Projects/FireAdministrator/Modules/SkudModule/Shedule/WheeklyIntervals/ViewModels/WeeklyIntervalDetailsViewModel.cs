﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalDetailsViewModel(SKDWeeklyInterval weeklyInterval = null)
		{
			if (weeklyInterval == null)
			{
				Title = "Новый понедельный график";
				weeklyInterval = new SKDWeeklyInterval()
				{
					Name = "Понедельный график",
				};
				foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.TimeIntervalUID = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault().UID;
				}
			}
			else
			{
				Title = "Редактирование понедельногор графика";
			}
			WeeklyInterval = weeklyInterval;
			Name = WeeklyInterval.Name;
			Description = WeeklyInterval.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		protected override bool Save()
		{
			WeeklyInterval.Name = Name;
			WeeklyInterval.Description = Description;
			return true;
		}
	}
}