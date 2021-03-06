﻿using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XZonePolygonAdorner : BasePolygonAdorner
	{
		private ZonesViewModel _zonesViewModel;
		public XZonePolygonAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonXZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXZone(element);
			return element;
		}
	}
}