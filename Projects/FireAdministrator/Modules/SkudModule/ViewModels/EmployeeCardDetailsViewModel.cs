﻿using System.Collections.ObjectModel;
using FiresecAPI.Models.Skud;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Properties;

namespace SKDModule.ViewModels
{
	public class EmployeeCardDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeeCardViewModel EmployeeCardViewModel { get; private set; }
		public EmployeeCardDetails Card { get; private set; }
		public ObservableCollection<EmployeePosition> Positions { get; private set; }
		public ObservableCollection<EmployeeDepartment> Departments { get; private set; }
		public ObservableCollection<EmployeeGroup> Groups { get; private set; }

		public EmployeeCardDetailsViewModel(EmployeeCardViewModel employeeCardViewModel = null)
		{
			EmployeeCardViewModel = employeeCardViewModel;
			Title = Resources.EmployeeCardTitle;
			SaveCaption = Resources.SaveText;
			Initialize();
		}

		private void Initialize()
		{
			Card = EmployeeCardViewModel == null ? new EmployeeCardDetails() { Id = -1 } : FiresecManager.GetEmployeeCard(EmployeeCardViewModel.EmployeeCard);

			Positions = new ObservableCollection<EmployeePosition>(FiresecManager.GetEmployeePositions());
			Positions.Insert(0, new EmployeePosition() { Id = 0 });
			Departments = new ObservableCollection<EmployeeDepartment>(FiresecManager.GetEmployeeDepartments());
			Departments.Insert(0, new EmployeeDepartment() { Id = 0 });
			Groups = new ObservableCollection<EmployeeGroup>(FiresecManager.GetEmployeeGroups());
			Groups.Insert(0, new EmployeeGroup() { Id = 0 });
		}

		public string Group { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }

		protected override bool Save()
		{
			bool result = base.Save();
			if (result)
			{
				result = FiresecManager.SaveEmployeeCard(Card);
				if (result)
				{
					if (EmployeeCardViewModel == null)
						EmployeeCardViewModel = new ViewModels.EmployeeCardViewModel(new EmployeeCard());
					EmployeeCardViewModel.EmployeeCard.Id = Card.Id;
					EmployeeCardViewModel.EmployeeCard.ClockNumber = Card.ClockNumber;
					EmployeeCardViewModel.EmployeeCard.FirstName = Card.FirstName;
					EmployeeCardViewModel.EmployeeCard.LastName = Card.LastName;
					EmployeeCardViewModel.EmployeeCard.SecondName = Card.SecondName;
					EmployeeCardViewModel.EmployeeCard.Department = Department;
					EmployeeCardViewModel.EmployeeCard.Group = Group;
					EmployeeCardViewModel.EmployeeCard.Position = Position;
				}
			}
			return result;
		}
		protected override bool CanSave()
		{
			return base.CanSave();
		}
	}
}