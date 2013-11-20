﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock.Layout;
using Infrastructure.Common;

namespace LayoutModule.ViewModels
{
	public class LayoutDesignerViewModel : BaseViewModel
	{
		public static LayoutDesignerViewModel Instance { get; private set; }

		private Layout _layout;
		private XmlLayoutSerializer _serializer;
		private bool _loading;
		private bool _currentLayoutChanged;
		public LayoutDesignerViewModel(LayoutElementsViewModel layoutElementsViewModel)
		{
			_currentLayoutChanged = false;
			_loading = false;
			Instance = this;
			LayoutElementsViewModel = layoutElementsViewModel;
			Update();
			LayoutParts = new ObservableCollection<LayoutPartViewModel>();
		}

		public LayoutElementsViewModel LayoutElementsViewModel { get; private set; }

		private ObservableCollection<LayoutPartViewModel> _layoutParts;
		public ObservableCollection<LayoutPartViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				if (_layoutParts != null)
					_layoutParts.CollectionChanged -= LayoutPartsChanged;
				_layoutParts = value;
				_layoutParts.CollectionChanged += LayoutPartsChanged;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		private LayoutPartViewModel _activeLayoutPart;
		public LayoutPartViewModel ActiveLayoutPart
		{
			get { return _activeLayoutPart; }
			set
			{
				_activeLayoutPart = value;
				OnPropertyChanged(() => ActiveLayoutPart);
			}
		}

		private DockingManager _manager;
		public DockingManager Manager
		{
			get { return _manager; }
			set
			{
				if (Manager != null)
				{
					Manager.DocumentClosing -= LayoutPartClosing;
					Manager.LayoutConfigurationChanged -= LayoutConfigurationChanged;
				}
				_manager = value;
				if (_serializer != null)
				{
					_serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
					_serializer = null;
				}
				if (_manager != null)
				{
					Manager.LayoutConfigurationChanged += LayoutConfigurationChanged;
					Manager.DocumentClosing += LayoutPartClosing;
					Manager.LayoutUpdateStrategy = new LayoutUpdateStrategy();
					_serializer = new XmlLayoutSerializer(Manager);
					_serializer.LayoutSerializationCallback += LayoutSerializationCallback;
				}
			}
		}

		public void Update(Layout layout)
		{
			using (new WaitWrapper())
			{
				if (_layout != null)
					SaveLayout();
				_layout = layout;
				_currentLayoutChanged = false;
				if (_layout != null)
				{
					var layoutParts = new ObservableCollection<LayoutPartViewModel>();
					foreach (var layoutPart in _layout.Parts)
						layoutParts.Add(new LayoutPartViewModel(layoutPart));
					LayoutParts = layoutParts;
					_loading = true;
					Manager.Layout = new LayoutRoot();
					if (!string.IsNullOrEmpty(_layout.Content))
						using (var tr = new StringReader(_layout.Content))
							_serializer.Deserialize(tr);
					_loading = false;
					ActiveLayoutPart = LayoutParts.FirstOrDefault();
				}
			}
		}
		public void Update()
		{

		}
		public void SaveLayout()
		{
			if (_layout != null && _currentLayoutChanged)
				using (var tw = new StringWriter())
				{
					_serializer.Serialize(tw);
					_layout.Content = tw.ToString();
				}
		}

		private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(e.Model.ContentId))
				e.Content = LayoutParts.FirstOrDefault(item => item.UID == Guid.Parse(e.Model.ContentId));
		}
		private void LayoutConfigurationChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				ServiceFactory.SaveService.LayoutsChanged = true;
				_currentLayoutChanged = true;
			}
		}
		private void LayoutPartClosing(object sender, DocumentClosingEventArgs e)
		{
			var layoutPartViewModel = e.Document.Content as LayoutPartViewModel;
			if (layoutPartViewModel != null)
			{
				LayoutParts.Remove(layoutPartViewModel);
				e.Cancel = true;
			}
		}
		private void LayoutPartsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (LayoutPartViewModel layoutPartViewModel in e.NewItems)
					{
						_layout.Parts.Add(layoutPartViewModel.LayoutPart);
						layoutPartViewModel.LayoutPartDescriptionViewModel.Count++;
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (LayoutPartViewModel layoutPartViewModel in e.OldItems)
					{
						_layout.Parts.Remove(layoutPartViewModel.LayoutPart);
						layoutPartViewModel.LayoutPartDescriptionViewModel.Count--;
					}
					break;
			}
		}

		public void AddLayoutPart(LayoutPartDescriptionViewModel layoutPartDescriptionViewModel, bool dragging)
		{
			var layoutPartViewModel = new LayoutPartViewModel(layoutPartDescriptionViewModel);
			LayoutParts.Add(layoutPartViewModel);
			ActiveLayoutPart = layoutPartViewModel;
			if (dragging)
				Manager.StartDragging(layoutPartViewModel);
		}
	}
}