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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Linq;
namespace DevExpress.Map.Kml.Model {
	public abstract class Element {
		public virtual void Parse(KmlModelParserBase parser, XElement xml) {
		}
	}
	public abstract class KmlObject : Element {
		public string ID { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			ID = parser.ParseStringAttribute(xml, KmlTokens.Id, null);
		}
	}
	public abstract class Geometry : KmlObject {
	}
	public class PlacemarkList : ElementList<Placemark> {
	}
	public class StyleList : ElementList<Style> {
	}
	public class StyleMapList : ElementList<StyleMap> {
	}
	public class ElementList<T> : List<T> where T : Element {
	}
	public class Root : Element {  
		public Root() {
			Styles = new StyleList();
			StyleMap = new StyleMapList();
			Placemarks = new PlacemarkList();
		}
		public StyleList Styles { get; private set; }
		public StyleMapList StyleMap { get; private set; }
		public PlacemarkList Placemarks { get; private set; }
		internal Style ResolveElementStyle(Feature feature) {
			if (feature == null)
				return null;
			if (feature.StyleSelector != null)
				return ResolveStyleBySelector(feature.StyleSelector);
			return ResolveStyleByUrl(feature.StyleUrl);
		}
		Style ResolveStyleBySelector(StyleSelector selector) {
			Style style = selector as Style;
			if (style != null) return style;
			StyleMap map = selector as StyleMap;
			if (map != null) {
				return  FindSharedStyleFromStyleMap(map.ID);
			}
			return null;
		}
		Style ResolveStyleByUrl(Uri url) {
			if (url == null) return null;
			string id = url.ToString();
			Style result = FindSharedStyle(id);
			if (result != null)
				return result;
			return FindSharedStyleFromStyleMap(id);
		}
		Style FindSharedStyleFromStyleMap(string mapId) {
			StyleMap map = StyleMap.Find((x) => x.ID == mapId);
			if (map == null) 
				return null;
			Uri url = map.GetStyleUrl(StyleState.Normal); 
			string styleId = url.ToString();
			return FindSharedStyle(styleId);
		}
		Style FindSharedStyle(string id) {
			return Styles.Find((x) => x.ID == id);
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct ColorABGR : IComparable<ColorABGR> {
		static readonly ColorABGR defaultColor = new ColorABGR(255, 255, 255, 255);
		public static ColorABGR Default { get { return defaultColor; } }
		public static ColorABGR Parse(string abgrColor) {
			if (abgrColor.Length != 8)
				return ColorABGR.Default;
			byte a = byte.Parse(abgrColor.Substring(0, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(abgrColor.Substring(2, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(abgrColor.Substring(4, 2), NumberStyles.HexNumber);
			byte r = byte.Parse(abgrColor.Substring(6, 2), NumberStyles.HexNumber);
			return new ColorABGR(a, b, g, r);
		}
		public static bool operator ==(ColorABGR color1, ColorABGR color2) {
			return color1.abgr == color2.abgr;
		}
		public static bool operator !=(ColorABGR color1, ColorABGR color2) {
			return !(color1 == color2);
		}
		public static bool Equals(ColorABGR color1, ColorABGR color2) {
			if (color1 != null && color2 != null)
				return color1 == color2;
			return false;
		}
		readonly UInt32 abgr;
		public byte A { get { return (byte)(abgr >> 0x18); } }
		public byte B { get { return (byte)(abgr >> 0x10); } }
		public byte G { get { return (byte)(abgr >> 8); } }
		public byte R { get { return (byte)(abgr); } }
		public ColorABGR(byte alpha, byte blue, byte green, byte red) {
			this.abgr = (UInt32)(alpha << 0x18 | blue << 0x10 | green << 8 | red);
		}
		public override bool Equals(object obj) {
			ColorABGR? nullableValue = obj as ColorABGR?;
			return nullableValue.HasValue && ColorABGR.Equals(this, nullableValue.Value);
		}
		public override int GetHashCode() {
			return this.abgr.GetHashCode();
		}
		public int CompareTo(ColorABGR other) {
			return this.abgr.CompareTo(other.abgr);
		}
	}
	public enum ColorMode { Normal, Random }
	[FormatElement(KmlTokens.Coordinates)]
	public class LatLonPointCollection : Element {
		List<LatLonPoint> coordinates;
		public LatLonPoint this[int index] { get { return GetItemByIndex(index); } }
		protected LatLonPoint GetItemByIndex(int index) {
			if (index < 0 || index >= Count) return LatLonPoint.Empty;
			return coordinates[index];
		}
		public int Count { get { return coordinates.Count; } }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			coordinates = parser.ParseCoordinates(xml);
		}
	}
	public struct LatLonPoint {
		static readonly LatLonPoint empty = new LatLonPoint();
		public static LatLonPoint Empty { get { return empty; } }
		double latitude;
		double longitude;
		public double Latitude { get { return latitude; } }
		public double Longitude { get { return longitude; } }
		public LatLonPoint(double latitude, double longitude) {
			this.latitude = latitude;
			this.longitude = longitude;
		}
	}
	public enum Unit { Fraction = 0, Pixels = 1, InsetPixels = 2 };
	[FormatElement(KmlTokens.Icon)]
	public class Icon : Element {
		public Uri Href { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			Href = parser.ParseUriElement(xml, KmlTokens.HRef, null);
		}
	}
	[FormatElement(KmlTokens.HotSpot)]
	public class HotSpot : Element {
		public const Unit DefaultUnit = Unit.Fraction;
		public const double DefaultX = 0.5;
		public const double DefaultY = 0.5;
		public double X { get; set; }
		public double Y { get; set; }
		public Unit XUnits { get; set; }
		public Unit YUnits { get; set; }
		public HotSpot() {
			X = DefaultX;
			Y = DefaultY;
			XUnits = DefaultUnit;
			YUnits = DefaultUnit;
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			X = parser.ParseDoubleAttribute(xml, KmlTokens.X, DefaultX);
			Y = parser.ParseDoubleAttribute(xml, KmlTokens.Y, DefaultY);
			XUnits = parser.ParseUnit(xml, KmlTokens.XUnits, DefaultUnit);
			YUnits = parser.ParseUnit(xml, KmlTokens.YUnits, DefaultUnit);
		}
	}
	public abstract class LineRingContainerElement : Element {
		LinearRing ring;
		public LinearRing LinearRing {
			get {
				if (ring == null)
					ring = new LinearRing();
				return ring;
			}
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			ring = parser.CreateAndParseElement(xml, KmlTokens.LinearRing) as LinearRing;
		}
	}
	[FormatElement(KmlTokens.InnerBoundaryIs)]
	public class InnerBoundary : LineRingContainerElement {
	}
	[FormatElement(KmlTokens.OuterBoundaryIs)]
	public class OuterBoundary : LineRingContainerElement {
	}
}
