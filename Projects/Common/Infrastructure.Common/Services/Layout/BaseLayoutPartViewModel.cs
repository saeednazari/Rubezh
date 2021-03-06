﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public abstract class BaseLayoutPartViewModel : BaseViewModel
	{
		public abstract ILayoutProperties Properties { get; }
		public abstract IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages { get; }
	}
}
