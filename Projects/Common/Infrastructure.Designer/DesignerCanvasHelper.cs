﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrastructure.Common;
using System.Windows.Input;

namespace Infrastructure.Designer
{
	public static class DesignerCanvasHelper
	{
		public static MenuItem BuildMenuItem(string header, string imageSourceUri, ICommand command)
		{
			var menuItem = new MenuItem();

			Image image = new Image();
			image.Width = 16;
			image.VerticalAlignment = VerticalAlignment.Center;
			BitmapImage sourceImage = new BitmapImage();
			sourceImage.BeginInit();
			sourceImage.UriSource = new Uri(imageSourceUri);
			sourceImage.EndInit();
			image.Source = sourceImage;

			menuItem.Icon = image;
			menuItem.Header = header;
			menuItem.Command = command;

			return menuItem;
		}
	}
}