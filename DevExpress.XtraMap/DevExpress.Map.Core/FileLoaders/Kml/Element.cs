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
	public abstract class Container : Feature {  
	}
	[FormatElement(KmlTokens.Document)]
	public class Document : Container {  
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
		}
	}
	[FormatElement(KmlTokens.Folder)]
	public class Folder : Container {  
	}
	public abstract class Feature : Element {
		public string Name { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public string Description { get; set; }
		public Uri StyleUrl { get; set; }
		public StyleSelector StyleSelector { get; set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			Name = parser.ParseStringElement(xml, KmlTokens.Name, string.Empty);
			Address = parser.ParseStringElement(xml, KmlTokens.Address, string.Empty);
			PhoneNumber = parser.ParseStringElement(xml, KmlTokens.PhoneNumber, string.Empty);
			Description = parser.ParseStringElement(xml, KmlTokens.Description, string.Empty);
			StyleUrl = parser.ParseStyleUrlElement(xml, KmlTokens.StyleUrl, null);
			StyleSelector = parser.CreateAndParseElement(xml, KmlTokens.Style) as Style;
			if (StyleSelector == null)
				StyleSelector = parser.CreateAndParseElement(xml, KmlTokens.StyleMap) as StyleMap;
		}
	}
	[FormatElement(KmlTokens.Placemark)]
	public class Placemark : Feature {
		public Geometry Geometry { get; private set; }
		public Placemark() {
		}
		protected string[] SupportedChildElementNames { 
			get { return new string[] { KmlTokens.LineString, KmlTokens.Point, KmlTokens.Polygon, KmlTokens.MultiGeometry }; }
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			Geometry = parser.CreateAndParseElement(xml, SupportedChildElementNames) as Geometry;
		}
	}
	[FormatElement(KmlTokens.Polygon)]
	public class Polygon : Geometry {
		public OuterBoundary OuterBoundaryIs { get; set; }
		public List<InnerBoundary> InnerBoundaryIs { get; protected set; }
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			OuterBoundaryIs = parser.CreateAndParseElement(xml, KmlTokens.OuterBoundaryIs) as OuterBoundary;
			InnerBoundaryIs = parser.CreateAndParseInnerBoundaryIs(xml);
		}
	}
	public abstract class CoordinatesGeometry : Geometry {
		LatLonPointCollection coordinates;
		public LatLonPointCollection Coordinates {
			get {
				if (coordinates == null)
					coordinates = new LatLonPointCollection();
				return coordinates;
			}
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			coordinates = parser.CreateAndParseElement(xml, KmlTokens.Coordinates) as LatLonPointCollection;
		}
	}
	[FormatElement(KmlTokens.LinearRing)]
	public class LinearRing : CoordinatesGeometry {
	}
	[FormatElement(KmlTokens.Point)]
	public class Point : CoordinatesGeometry {
	}
	[FormatElement(KmlTokens.LineString)]
	public class LineString : CoordinatesGeometry {
	}
	[FormatElement(KmlTokens.MultiGeometry)]
	public class MultiGeometry : Geometry {
		List<Geometry> geometryList;
		protected string[] SupportedChildElementNames {
			get { return new string[] { KmlTokens.LineString, KmlTokens.Point, KmlTokens.Polygon, KmlTokens.MultiGeometry }; }
		}
		public List<Geometry> GeometryList {
			get { 
				if (geometryList == null)
					geometryList = new List<Geometry>();
				return geometryList;
			}
		}
		public override void Parse(KmlModelParserBase parser, XElement xml) {
			base.Parse(parser, xml);
			geometryList = parser.CreateAndParseGeometries(xml, SupportedChildElementNames);
		}
	}
}
