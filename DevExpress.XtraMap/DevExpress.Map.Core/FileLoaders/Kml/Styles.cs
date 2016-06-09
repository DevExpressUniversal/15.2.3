#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using DevExpress.Data.Svg;
using DevExpress.Map.Native;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
namespace DevExpress.Map.Kml.Model {
	public enum StyleState { Normal, Highlight };
	public enum DisplayMode { Default, Hide };
	public enum ListItemType { Check, CheckOffOnly, CheckHideChildren, RadioFolder };
	public enum IconItemMode { Open, Closed, Error, Fetching0, Fetching1, Fetching2 };
	public abstract class StyleSelector : KmlObject {
	}
	[FormatElement(KmlTokens.Style)] 
	public class Style : StyleSelector {
		public IconStyle IconStyle { get; set; }
		public LabelStyle LabelStyle { get; set; }
		public LineStyle LineStyle { get; set; }
		public PolyStyle PolyStyle { get; set; }
		public BalloonStyle BalloonStyle { get; set; }
		public ListStyle ListStyle { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			IconStyle = parser.CreateAndParseElement(xml, KmlTokens.IconStyle) as IconStyle;
			LabelStyle = parser.CreateAndParseElement(xml, KmlTokens.LabelStyle) as LabelStyle;
			LineStyle = parser.CreateAndParseElement(xml, KmlTokens.LineStyle) as LineStyle;
			PolyStyle = parser.CreateAndParseElement(xml, KmlTokens.PolyStyle) as PolyStyle;
			BalloonStyle = parser.CreateAndParseElement(xml, KmlTokens.BalloonStyle) as BalloonStyle;
			ListStyle = parser.CreateAndParseElement(xml, KmlTokens.ListStyle) as ListStyle;
		}
	}
	[FormatElement(KmlTokens.StyleMap)] 
	public class StyleMap : StyleSelector {
		List<Pair> pairs;
		public List<Pair> PairList {
			get {
				if (pairs == null)
					pairs = new List<Pair>();
				return pairs;
			}
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			this.pairs = parser.CreateAndParsePairs(xml);
		}
		public Uri GetStyleUrl(StyleState key) { 
			Pair item = pairs.Find((x) => x.Key == key);
			return item != null ? item.StyleUrl : null;
		}
	}
	[FormatElement(KmlTokens.Pair)] 
	public class Pair : KmlObject {
		public StyleState Key { get; set; }
		public Uri StyleUrl { get; set; }
		public Style Style { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Key = parser.ParseStyleState(xml, KmlTokens.Key, StyleState.Normal);
			StyleUrl = parser.ParseStyleUrlElement(xml, KmlTokens.StyleUrl, null);
			Style = parser.CreateAndParseElement(xml, KmlTokens.Style) as Style;
		}
	}
	public abstract class SubStyle : KmlObject {
	}
	public abstract class ColorStyle : SubStyle {
		public const ColorMode DefaultColorMode = ColorMode.Normal;
		public ColorABGR Color { get; set; }
		public ColorMode ColorMode { get; set; }
		protected ColorStyle() {
			Color = ColorABGR.Default;
			ColorMode = DefaultColorMode;
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Color = parser.ParseColorABGR(xml, KmlTokens.Color, ColorABGR.Default);
			ColorMode = parser.ParseColorMode(xml, KmlTokens.ColorMode, DefaultColorMode);
		}
	}
	[FormatElement(KmlTokens.IconStyle)] 
	public class IconStyle : ColorStyle {
		public const int DefaultHeading = 0;
		public const double DefaultScale = 1.0;
		public static IconStyle Default { get { return new IconStyle() { Scale = DefaultScale }; } }
		HotSpot hotSpot;
		Icon icon;
		public double Scale { get; set; }
		public double Heading { get; set; } 
		public Icon Icon { get { return icon; } }
		public HotSpot HotSpot { get { return hotSpot;  }  }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Scale = parser.ParseDoubleElement(xml, KmlTokens.Scale, DefaultScale);
			Heading = parser.ParseIntElement(xml, KmlTokens.Heading, DefaultHeading);
			this.icon = parser.CreateAndParseElement(xml, KmlTokens.Icon) as Icon;
			this.hotSpot = parser.CreateAndParseElement(xml, KmlTokens.HotSpot) as HotSpot;
		}
	}
	[FormatElement(KmlTokens.LabelStyle)]
	public class LabelStyle : ColorStyle {
		public const double DefaultScale = 1.0;
		public static LabelStyle Default { get { return new LabelStyle() { Scale = DefaultScale }; } }
		public double Scale { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Scale = parser.ParseDoubleElement(xml, KmlTokens.Scale, DefaultScale);
		}
	}
	[FormatElement(KmlTokens.LineStyle)]
	public class LineStyle : ColorStyle {
		public const double DefaultWidth = 1.0;
		public double Width { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Width = parser.ParseDoubleElement(xml, KmlTokens.Width, DefaultWidth);
		}
	}
	[FormatElement(KmlTokens.PolyStyle)]
	public class PolyStyle : ColorStyle {
		public const bool DefaultFill = true;
		public const bool DefaultOutline = true;
		public bool Fill { get; set; }
		public bool Outline { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Fill = parser.ParseBooleanElement(xml, KmlTokens.Fill, DefaultFill);
			Outline = parser.ParseBooleanElement(xml, KmlTokens.Outline, DefaultOutline);
		}
	}
	[FormatElement(KmlTokens.BalloonStyle)]
	public class BalloonStyle : SubStyle {
		public static readonly ColorABGR DefaultTextColor = new ColorABGR(255, 0, 0, 0);
		public ColorABGR BgColor { get; set; }
		public ColorABGR TextColor { get; set; }
		public string Text { get; set; }
		public DisplayMode DisplayMode { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			BgColor = parser.ParseColorABGR(xml, KmlTokens.BgColor, ColorABGR.Default);
			TextColor = parser.ParseColorABGR(xml, KmlTokens.TextColor, DefaultTextColor);
			Text = parser.ParseStringElement(xml, KmlTokens.Text, string.Empty);
			DisplayMode = parser.ParseDisplayMode(xml, KmlTokens.DisplayMode, DisplayMode.Default);
		}
	}
	[FormatElement(KmlTokens.ListStyle)]
	public class ListStyle : SubStyle {
		public const ListItemType DefaultListItemType = ListItemType.Check;
		public ColorABGR BgColor { get; set; }
		public ListItemType ListItemType { get; set; }
		List<ItemIcon> itemIconList;
		public List<ItemIcon> ItemIconList {
			get {
				if (itemIconList == null)
					itemIconList = new List<ItemIcon>();
				return itemIconList;
			}
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			BgColor = parser.ParseColorABGR(xml, KmlTokens.BgColor, ColorABGR.Default);
			ListItemType = parser.ParseListItemType(xml, KmlTokens.ListItemType, DefaultListItemType);
			itemIconList = parser.CreateAndParseItemIcons(xml);
		}
	}
	[FormatElement(KmlTokens.ItemIcon)]
	public class ItemIcon : KmlObject {
		public const IconItemMode DefaultIconItemMode = IconItemMode.Open;
		public IconItemMode IconItemMode { get; set; }
		public Uri Href { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			IconItemMode = parser.ParseIconItemMode(xml, KmlTokens.IconItemMode, DefaultIconItemMode);
			Href = parser.ParseUriElement(xml, KmlTokens.HRef, null);
		}
	}
}
