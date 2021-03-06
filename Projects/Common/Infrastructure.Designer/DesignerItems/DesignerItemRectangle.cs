﻿using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;
using Infrastructure.Designer.Adorners;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace Infrastructure.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			: base(element)
		{
			SetResizeChrome(new ResizeChromeRectangle(this));
			if (Element is ElementRectangle)
			{
				Title = "Прямоугольник";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementEllipse)
			{
				Title = "Эллипс";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementTextBlock)
			{
				Title = "Надпись";
				Group = LayerGroupService.ElementAlias;
			}
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementRectangle)
				return new RectanglePropertiesViewModel(Element as ElementRectangle);
			if (Element is ElementEllipse)
				return new EllipsePropertiesViewModel(Element as ElementEllipse);
			if (Element is ElementTextBlock)
				return new TextBlockPropertiesViewModel(Element as ElementTextBlock);
			return base.CreatePropertiesViewModel();
		}
	}
}
