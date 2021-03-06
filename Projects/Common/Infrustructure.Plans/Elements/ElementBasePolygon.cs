﻿using System.Runtime.Serialization;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolygon : ElementBaseShape
	{
		public ElementBasePolygon()
		{
		}

		public override ElementType Type
		{
			get { return ElementType.Polygon; }
		}
		protected override void SetDefault()
		{
			base.SetDefault();
			Points.Add(new Point(0, 0));
			Points.Add(new Point(50, 0));
			Points.Add(new Point(50, 50));
			Points.Add(new Point(0, 50));
		}
	}
}