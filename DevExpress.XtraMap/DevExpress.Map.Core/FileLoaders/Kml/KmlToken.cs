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

using System;
namespace DevExpress.Map.Kml {
	public static class KmlTokens {
		public const string KmlOldNamespaceName = "http://earth.google.com/kml/";
		public const string KmlNamespaceName = "http://www.opengis.net/kml/";
		public const string KmlNamespaceAttribute = "xmlns";
		public const string Kml = "kml";
		public const string Document = "Document";
		public const string Folder = "Folder";
		public const string Id = "id";
		public const string Placemark = "Placemark";
		public const string Point = "Point";
		public const string LineString = "LineString";
		public const string LinearRing = "LinearRing";
		public const string Polygon = "Polygon";
		public const string MultiGeometry = "MultiGeometry";
		public const string Icon = "Icon";
		public const string Name = "name";
		public const string OuterBoundaryIs = "outerBoundaryIs";
		public const string InnerBoundaryIs = "innerBoundaryIs";
		public const string Address = "address";
		public const string PhoneNumber = "phoneNumber";
		public const string Description = "description";
		public const string Style = "Style";
		public const string StyleMap = "StyleMap";
		public const string StyleUrl = "styleUrl";
		public const string IconStyle = "IconStyle";
		public const string LabelStyle = "LabelStyle";
		public const string LineStyle = "LineStyle";
		public const string PolyStyle = "PolyStyle";
		public const string BalloonStyle = "BalloonStyle";
		public const string ListStyle = "ListStyle";
		public const string Pair = "Pair";
		public const string Key = "key";
		public const string HotSpot = "hotSpot";
		public const string XUnits = "xunits";
		public const string YUnits = "yunits";
		public const string X = "x";
		public const string Y = "y";
		public const string Scale = "scale";
		public const string Heading = "heading";
		public const string HRef = "href";
		public const string Color = "color";
		public const string ColorMode = "colorMode";
		public const string Width = "width";
		public const string Fill = "fill";
		public const string Outline = "outline";
		public const string BgColor = "bgColor";
		public const string TextColor = "textColor";
		public const string Text = "text";
		public const string DisplayMode = "displayMode";
		public const string ListItemType = "listItemType";
		public const string ItemIcon = "ItemIcon";
		public const string IconItemMode = "iconItemMode";
		public const string Coordinates = "coordinates";
		public const string Visibility = "visibility";
	}
	public static class KmlValueTokens {
		public const string Pixels = "pixels";
		public const string InsetPixels = "insetPixels";
		public const string Fraction = "fraction";
		public const string Normal = "normal";
		public const string Random = "random";
	}
}
