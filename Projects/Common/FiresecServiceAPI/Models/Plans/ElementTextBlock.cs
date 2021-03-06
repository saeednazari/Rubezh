﻿using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementTextBlock : ElementBaseRectangle, IElementTextBlock, IPrimitive
	{
		public ElementTextBlock()
		{
			Stretch = false;
			TextAlignment = 0;
			BorderThickness = 0;
			BackgroundColor = Colors.Transparent;
		}

		[DataMember]
		public string Text { get; set; }
		[DataMember]
		public Color ForegroundColor { get; set; }
		[DataMember]
		public double FontSize { get; set; }
		[DataMember]
		public bool FontItalic { get; set; }
		[DataMember]
		public bool FontBold { get; set; }
		[DataMember]
		public string FontFamilyName { get; set; }
		[DataMember]
		public bool Stretch { get; set; }
		[DataMember]
		public int TextAlignment { get; set; }

		public override ElementBase Clone()
		{
			ElementTextBlock elementBase = new ElementTextBlock();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			var elementTextBlock = (ElementTextBlock)element;
			elementTextBlock.Text = Text;
			elementTextBlock.ForegroundColor = ForegroundColor;
			elementTextBlock.FontSize = FontSize;
			elementTextBlock.FontFamilyName = FontFamilyName;
			elementTextBlock.Stretch = Stretch;
			elementTextBlock.TextAlignment = TextAlignment;
			elementTextBlock.FontBold = FontBold;
			elementTextBlock.FontItalic = FontItalic;
		}


		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.TextBlock; }
		}

		#endregion

		protected override void SetDefault()
		{
			Text = "Надпись";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			FontItalic = false;
			FontBold = false;
			base.SetDefault();
			Height = 22;
			Width = 52;
		}
	}
}