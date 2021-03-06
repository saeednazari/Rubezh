﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKProcessor;
using FiresecClient;
using FiresecAPI.Models.Skud;

namespace SKDModule.ViewModels
{
	public class SKUDViewModel : ViewPartViewModel
	{
		public SKUDViewModel()
		{
			Filter = new EmployeeFilter();
			SelectedEmployee = Employees.FirstOrDefault();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			//RemoveCommand = new RelayCommand(OnRemove, CanEditDelete);
			//EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		EmployeeFilter filter;
		public EmployeeFilter Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				UpdateEmployees();
			}
		}

		ObservableCollection<EmployeeViewModel> employees;
		public ObservableCollection<EmployeeViewModel> Employees
		{
			get { return employees; }
			set
			{
				employees = value;
				OnPropertyChanged("Employees");
			}
		}

		EmployeeViewModel selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return selectedEmployee; }
			set
			{
				selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");
			}
		}

		void UpdateEmployees()
		{
			Employees = new ObservableCollection<EmployeeViewModel>();
			FiresecManager.GetEmployees(Filter).ToList().ForEach(x => Employees.Add(new EmployeeViewModel(x)));
		}


		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
			}
		}

		//public RelayCommand RemoveCommand { get; private set; }
		//void OnRemove()
		//{
		//    //TestDataHelper.RemoveEmployee(SelectedEmployee.Employee);
		//    Employees.Remove(SelectedEmployee);
		//    SelectedEmployee = Employees.FirstOrDefault();

		//}

		//public RelayCommand EditCommand { get; private set; }
		//void OnEdit()
		//{
		//    var employeeDelailsViewModel = new EmployeeDetailsViewModel(SelectedEmployee.Employee);
		//    DialogService.ShowModalWindow(employeeDelailsViewModel);
		//    var employee = employeeDelailsViewModel.Employee;
		//    //Employees.Add(new EmployeeViewModel(employee));
		//    //TestDataHelper.AddEmployee(employee);
		//}

		//bool CanEditDelete()
		//{
		//    return SelectedEmployee != null;
		//}
	}
}