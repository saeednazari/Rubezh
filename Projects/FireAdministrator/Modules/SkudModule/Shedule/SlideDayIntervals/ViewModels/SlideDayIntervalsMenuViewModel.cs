﻿using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalsMenuViewModel : BaseViewModel
	{
		public SlideDayIntervalsMenuViewModel(SlideDayIntervalsViewModel context)
		{
			Context = context;
		}

		public SlideDayIntervalsViewModel Context { get; private set; }
	}
}