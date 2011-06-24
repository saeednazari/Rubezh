﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using System.Windows;
using FiresecClient.Models;

namespace SecurityModule.ViewModels
{
	public class NewUserViewModel : DialogContent
	{
		public NewUserViewModel(User newUser)
		{
			Title = "Новый пользователь";
			AddCommand = new RelayCommand(OnAdd);
			CancelCommand = new RelayCommand(OnCancel);
			_user = newUser;
		}

		private User _user;

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

		string _fullName;
		public string FullName
		{
			get { return _fullName; }
			set
			{
				_fullName = value;
				OnPropertyChanged("FullName");
			}
		}
		public string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			_user.Name = Name;
			_user.FullName = FullName;
			Close(true);
		}

		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
		}

		public void Initialize()
		{

		}
	}
}
