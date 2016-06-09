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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Map.Kml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
namespace DevExpress.Map.Native {
	public class KmlExporter<TItem> : XmlExporterBase<TItem> where TItem : IMapItemCore {
		const string KmlFormatVersion = "2.2";
		void AddColorStyle(Color color, XmlNode parent) {
			XmlElement colorStyle = AppendNewElementToParent(parent, KmlTokens.Color);
			colorStyle.InnerText = CoreUtils.ColorToBGRHex(color, true);
		}
		void AddPointData(CoordPoint point, XmlNode parent) {
			XmlElement pointElement = AppendNewElementToParent(parent, KmlTokens.Point);
			XmlElement coordinatesElement = AppendNewElementToParent(pointElement, KmlTokens.Coordinates);
			coordinatesElement.InnerText = PointToString(point);
		}
		void AddLinesData(IList<CoordPoint> points, XmlNode parent, bool isRing = false) {
			if (points.Count == 0) return;
			XmlElement lineStringElement = AppendNewElementToParent(parent, isRing ? KmlTokens.LinearRing : KmlTokens.LineString);
			XmlElement coordinatesElement = AppendNewElementToParent(lineStringElement, KmlTokens.Coordinates);
			StringBuilder pointsLine = new StringBuilder();
			pointsLine.Append(PointToString(points[0]));
			for (int i = 1; i < points.Count; i++) {
				pointsLine.Append(CoordinatesSeparator);
				pointsLine.Append(PointToString(points[i]));
			}
			if (isRing && points[0] != points[points.Count - 1]) {
				pointsLine.Append(CoordinatesSeparator);
				pointsLine.Append(PointToString(points[0]));
			}
			coordinatesElement.InnerText = pointsLine.ToString();
		}
		XmlElement AppendPolygonData(IPolygonCore polygon, XmlNode parent) {
			XmlElement polygonElement = AppendNewElementToParent(parent, KmlTokens.Polygon);
			XmlElement outerBoundaryElement = AppendNewElementToParent(polygonElement, KmlTokens.OuterBoundaryIs);
			AddLinesData(GetPolygonPoints(polygon).ToList(), outerBoundaryElement, true);
			return polygonElement;
		}
		void AddMultiGeometry(IPathCore path, XmlNode parent) {
			XmlElement multiGeometryElement = AppendNewElementToParent(parent, KmlTokens.MultiGeometry);
			for (int i = 0; i < path.SegmentCount; i++) {
				IPathSegmentCore segment = path.GetSegment(i);
				XmlElement polygonElement = AppendPolygonData(segment, multiGeometryElement);
				for (int j = 0; j < segment.InnerContourCount; j++) {
					XmlElement innerBoundaryElement = AppendNewElementToParent(polygonElement, KmlTokens.InnerBoundaryIs);
					IPointContainerCore innerBoundary = segment.GetInnerCountour(j);
					AddLinesData(GetPolygonPoints(innerBoundary).ToList(), innerBoundaryElement, true);
				}
			}
		}
		void AddWidthStyle(double width, XmlNode parent) {
			XmlElement widthStyle = AppendNewElementToParent(parent, KmlTokens.Width);
			widthStyle.InnerText = width.ToString(PointsExportCulture);
		}
		void AddImageStyle(IMapPointerStyleCore item, XmlNode parent) {
			string imageHref = GetExportImageHref(item);
			if(String.IsNullOrEmpty(imageHref)) 
				return;
			XmlElement iconStyleElement = AppendNewElementToParent(parent, KmlTokens.IconStyle);
			XmlElement iconElement = AppendNewElementToParent(iconStyleElement, KmlTokens.Icon);
			XmlElement iconHref = AppendNewElementToParent(iconElement, KmlTokens.HRef);
			iconHref.InnerText = imageHref;
		}
		void AddLabelStyle(IMapItemCore item, XmlNode parent) {
			XmlElement labelStyle = CreateElement(KmlTokens.LabelStyle);
			if (!item.TextColor.IsEmpty)
				AddColorStyle(item.TextColor, labelStyle);
			if (!IsEmptyElement(labelStyle))
				AddElementToParent(parent, labelStyle);
		}
		void AddLineStyle(IMapShapeStyleCore item, XmlNode parent) {
			XmlElement lineStyle = CreateElement(KmlTokens.LineStyle);
			if (!item.Stroke.IsEmpty)
				AddColorStyle(item.Stroke, lineStyle);
			if (item.StrokeWidth > 0)
				AddWidthStyle(item.StrokeWidth, lineStyle);
			if (!IsEmptyElement(lineStyle))
				AddElementToParent(parent, lineStyle);
		}
		void AddPolygonStyle(IMapShapeStyleCore item, XmlNode parent) {
			XmlElement polyStyle = CreateElement(KmlTokens.PolyStyle);
			if (!item.Fill.IsEmpty)
				AddColorStyle(item.Fill, polyStyle);
			if (!IsEmptyElement(polyStyle))
				AddElementToParent(parent, polyStyle);
		}
		internal void ProcessPointerStyle(IMapPointerStyleCore item, XmlNode styleElement) {
			if (item == null) return; 
			AddImageStyle(item, styleElement);
			AddLabelStyle(item, styleElement);
		}
		internal void ProcessShapeStyle(IMapShapeStyleCore item, XmlNode styleElement) {
			if (item == null) return;
			AddLabelStyle(item, styleElement);
			AddLineStyle(item, styleElement);
			AddPolygonStyle(item, styleElement);
		}
		internal void StyleProcessing(TItem item, int index) {
			XmlElement styleElement = AppendNewElementToParent(DocumentNode, KmlTokens.Style);
			styleElement.SetAttribute(KmlTokens.Id, string.Format("id_{0}", index));
			ProcessPointerStyle(item as IMapPointerStyleCore, styleElement);
			ProcessShapeStyle(item as IMapShapeStyleCore, styleElement);
		}
		internal void ProcessItem(IPointCore pointCore, XmlNode placemarkElement) {
			if (pointCore != null)
				AddPointData(pointCore.Location, placemarkElement);
		}
		internal void ProcessItem(ISupportIntermediatePoints item, XmlNode placemarkElement) {
			if (item != null)
				AddLinesData(item.Vertices, placemarkElement);
		}
		internal void ProcessItem(IPolygonCore polygon, XmlNode placemarkElement) {
			if (polygon != null)
				AppendPolygonData(polygon, placemarkElement);
		}
		internal void ProcessItem(IPathCore path, XmlNode parent) {
			if (path != null)
				AddMultiGeometry(path, parent);
		}
		internal void ProcessItemVisibility(TItem item, XmlNode placemarkElement) {
			XmlElement visability = AppendNewElementToParent(placemarkElement, KmlTokens.Visibility);
			visability.InnerText = "1";
		}
		internal void ProcessItemStyle(XmlNode placemarkElement, int index) {
			XmlElement stylePlacemark = AppendNewElementToParent(placemarkElement, KmlTokens.StyleUrl);
			stylePlacemark.InnerText = string.Format("#id_{0}", index);
		}
		internal void PlacemarkProcessing(TItem item, int index) {
			XmlElement placemarkElement = AppendNewElementToParent(DocumentNode, KmlTokens.Placemark);
			if (!String.IsNullOrEmpty(item.Text)) {
				XmlElement nameElement = AppendNewElementToParent(placemarkElement, KmlTokens.Name);
				nameElement.InnerText = item.Text;
			}
			ProcessItem(item as IPointCore, placemarkElement);
			ProcessItem(item as ISupportIntermediatePoints, placemarkElement);
			ProcessItem(item as IPolygonCore, placemarkElement);
			ProcessItem(item as IPathCore, placemarkElement);
			ProcessItemVisibility(item, placemarkElement);
			ProcessItemStyle(placemarkElement, index);
		}
		internal void Processing(IEnumerable<TItem> items) {
			int counter = 0;
			foreach (TItem item in items)
				StyleProcessing(item, ++counter);
			counter = 0;
			foreach (TItem item in items)
				PlacemarkProcessing(item, ++counter);
		}
		protected override void ExportCore(XmlDocument userDoc, IEnumerable<TItem> items) {
			base.ExportCore(userDoc, items);
			XmlElement kmlElement = AppendNewElementToParent(Doc, KmlTokens.Kml);
			kmlElement.SetAttribute(KmlTokens.KmlNamespaceAttribute, KmlTokens.KmlNamespaceName + KmlFormatVersion);
			XmlElement elDocument = Doc.CreateElement(KmlTokens.Document);
			kmlElement.AppendChild(elDocument);
			DocumentNode = elDocument;
			Processing(items);
		}
	}
}
