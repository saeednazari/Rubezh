﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPropertiesViewModel : SaveCancelDialogViewModel
	{
		public class IPObject
		{
			public string IP { get; set; }
		}
		public Layout Layout { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public LayoutPropertiesViewModel(Layout layout, LayoutUsersViewModel layoutUsersViewModel)
		{
			Title = "Свойства элемента: Шаблон интерфейса ОЗ";
			Layout = layout ?? new Layout();
			LayoutUsersViewModel = layoutUsersViewModel;
			LayoutUsersViewModel.Update(Layout);
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = Layout.Caption;
			Description = Layout.Description;
			SplitterColor = Layout.SplitterColor;
			SplitterSize = Layout.SplitterSize;
			BorderColor = Layout.BorderColor;
			BorderThickness = Layout.BorderThickness;
			BackgroundColor = Layout.BackgroundColor;
			Padding = Layout.Padding;
			IPs = new ObservableCollection<IPObject>();
			Layout.IPs.ForEach(ip => IPs.Add(new IPObject() { IP = ip }));
		}

		private string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		private Color _splitterColor;
		public Color SplitterColor
		{
			get { return _splitterColor; }
			set
			{
				_splitterColor = value;
				OnPropertyChanged(() => SplitterColor);
			}
		}
		private int _splitterSize;
		public int SplitterSize
		{
			get { return _splitterSize; }
			set
			{
				_splitterSize = value;
				OnPropertyChanged(() => SplitterSize);
			}
		}

		private int _borderThickness;
		public int BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
			}
		}
		private Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		private int _padding;
		public int Padding
		{
			get { return _padding; }
			set
			{
				_padding = value;
				OnPropertyChanged(() => Padding);
			}
		}


		public ObservableCollection<IPObject> IPs { get; private set; }

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}
		protected override bool Save()
		{
			Layout.Caption = Caption;
			Layout.Description = Description;
			LayoutUsersViewModel.Save();
			Layout.SplitterColor = SplitterColor;
			Layout.SplitterSize = SplitterSize;
			Layout.BorderColor = BorderColor;
			Layout.BorderThickness = BorderThickness;
			Layout.BackgroundColor = BackgroundColor;
			Layout.Padding = Padding;
			Layout.IPs = IPs.Select(item => item.IP).ToList();
			return base.Save();
		}
	}
}