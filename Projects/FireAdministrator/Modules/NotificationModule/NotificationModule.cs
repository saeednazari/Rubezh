﻿using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using NotificationModule.ViewModels;

namespace NotificationModule
{
	public class NotificationModule : ModuleBase
	{
		NotificationViewModel NotificationViewModel;

		public override void CreateViewModels()
		{
			NotificationViewModel = new NotificationViewModel();
		}

		public override void Initialize()
		{
			NotificationViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowNotificationEvent>(NotificationViewModel, "Уведомления", "/Controls;component/Images/music.png"),
			};
		}

		public override string Name
		{
			get { return "Уведомления"; }
		}
	}
}