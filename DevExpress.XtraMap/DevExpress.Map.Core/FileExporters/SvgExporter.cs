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
using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
namespace DevExpress.Map.Native {
	public enum SvgEntityTypes {
		Unknown = 0,
		Point = 1,
		Line = 2,
		Polyline = 3,
		Polygon = 4,
		Rectangle = 5,
		Ellipse = 6,
		Path = 7
	}
	public class SvgExporter<TItem> : XmlExporterBase<TItem> where TItem : IMapItemCore {
		const string SvgFormatVersion = "1.2";
		internal const int DefaultWidth = 360;
		internal const int DefaultHeight = 360;
		internal const double StrokeScaleFactor = 0.35;
		public double ScaleFactor { get; set; }
		public SvgSize CanvasSize { get; set; }
		public ISvgPointConverter PointConverter { get; set; }
		public SvgExporter() {
			ScaleFactor = 1.0;
		}
		SvgEntityTypes GetEntityByType(IMapItemCore item) {
			bool isIntermediatePoints = item is ISupportIntermediatePoints;
			bool isPointContainer = item is IPointContainerCore;
			if (isIntermediatePoints && !isPointContainer) return SvgEntityTypes.Line;
			if (isIntermediatePoints && isPointContainer) return SvgEntityTypes.Polyline;
			if (item is IPolygonCore && !(item is ISupportRectangle)) return SvgEntityTypes.Polygon;
			if (item is IPathCore) return SvgEntityTypes.Path;
			if (item is IEllipseCore) return SvgEntityTypes.Ellipse;
			if (item is IRectangleCore) return SvgEntityTypes.Rectangle;
			if (item is IPointCore) return SvgEntityTypes.Point;
			return SvgEntityTypes.Unknown;
		}
		XmlElement CreateSvgEntity(string name) {
			return AppendNewElementToParent(DocumentNode, name);
		}
		void AddLineCoordinatesToElement(IList<CoordPoint> points, XmlElement element) {
			if (points == null || points.Count != 2) return;
			SvgPoint svgFirstPoint = CoordPointToSvgPoint(points[0]);
			SvgPoint svgSecondPoint = CoordPointToSvgPoint(points[1]);
			element.SetAttribute(SvgTokens.X1, svgFirstPoint.ToStringX(PointsExportCulture));
			element.SetAttribute(SvgTokens.Y1, svgFirstPoint.ToStringY(PointsExportCulture));
			element.SetAttribute(SvgTokens.X2, svgSecondPoint.ToStringX(PointsExportCulture));
			element.SetAttribute(SvgTokens.Y2, svgSecondPoint.ToStringY(PointsExportCulture));
		}
		void AddPointsToElement(IList<CoordPoint> points, XmlElement element, bool isRing = false) {
			if (points == null || points.Count == 0) return;
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
			element.SetAttribute(SvgTokens.Points, pointsLine.ToString());
		}
		void AddPathSegmentsToElement(IPathCore path, XmlElement element) {
			if (path == null || path.SegmentCount == 0) return;
			StringBuilder commandsLine = new StringBuilder();
			for (int i = 0; i < path.SegmentCount; i++) {
				string segmentCommands = CreateSegmentCommands(path.GetSegment(i));
				commandsLine.Append(segmentCommands);
			}
			element.SetAttribute(SvgTokens.Commands, commandsLine.ToString());
			element.SetAttribute(SvgTokens.FillRule, SvgTokens.Everodd);
		}
		void AddRectangleToElement(CoordPoint location, double width, double height, XmlElement element) {
			SvgPoint point = CoordPointToSvgPoint(location);
			SvgSize size = CoordSizeToSvgSize(location, new SvgSize(width, height));
			element.SetAttribute(SvgTokens.X, point.ToStringX(PointsExportCulture));
			element.SetAttribute(SvgTokens.Y, point.ToStringY(PointsExportCulture));
			element.SetAttribute(SvgTokens.Width, size.Width.ToString(PointsExportCulture));
			element.SetAttribute(SvgTokens.Height, size.Height.ToString(PointsExportCulture));
		}
		void AddEllipseToElement(CoordPoint location, double width, double height, XmlElement element) {
			SvgSize size = CoordSizeToSvgSize(location, new SvgSize(width, height));
			double rx = size.Width / 2.0;
			double ry = size.Height / 2.0;
			SvgPoint point = CoordPointToSvgPoint(location) + new SvgPoint(rx, ry);
			element.SetAttribute(SvgTokens.Cx, point.ToStringX(PointsExportCulture));
			element.SetAttribute(SvgTokens.Cy, point.ToStringY(PointsExportCulture));
			element.SetAttribute(SvgTokens.Rx, rx.ToString(PointsExportCulture));
			element.SetAttribute(SvgTokens.Ry, ry.ToString(PointsExportCulture));
		}
		void AddStylesToElement(IMapShapeStyleCore shapeStyle, XmlElement element) {
			if (shapeStyle == null) return;
			element.SetAttribute(SvgTokens.Fill, GetColorString(shapeStyle.Fill));
			element.SetAttribute(SvgTokens.Stroke, GetColorString(shapeStyle.Stroke));
			if (shapeStyle.StrokeWidth >= 0)
				element.SetAttribute(SvgTokens.StrokeWidth, ConvertStrokeWidth(shapeStyle.StrokeWidth).ToString(PointsExportCulture));
		}
		protected override void ExportCore(XmlDocument userDoc, IEnumerable<TItem> items) {
			base.ExportCore(userDoc, items);
			XmlElement svgElement = AppendNewElementToParent(Doc, SvgTokens.Svg);
			svgElement.SetAttribute(SvgTokens.Version, SvgFormatVersion);
			svgElement.SetAttribute(SvgTokens.SvgNamespaceAttribute, SvgTokens.SvgNamespaceName);
			svgElement.SetAttribute(SvgTokens.Width, GetCanvasWidth().ToString());
			svgElement.SetAttribute(SvgTokens.Height, GetCanvasHeight().ToString());
			XmlElement groupElelment = AppendNewElementToParent(svgElement, SvgTokens.Group);
			groupElelment.SetAttribute(SvgTokens.Transform, String.Format(CultureInfo.InvariantCulture, "{0}({1} {2})", SvgTokens.Scale, ScaleFactor, ScaleFactor));
			DocumentNode = groupElelment;
			Processing(items);
		}
		internal string GetColorString(Color color) {
			return color.IsEmpty ? SvgTokens.None : String.Format("#{0}", CoreUtils.ColorToRGBHex(color, false));
		}
		internal string CreateCommandsLine(IList<CoordPoint> points) {
			if (points.Count <= 1) return String.Empty;
			StringBuilder commandsLine = new StringBuilder();
			commandsLine.Append(String.Format("M{0} ", PointToString(points[0], " ")));
			for (int i = 1; i < points.Count - 1; i++)
				commandsLine.Append(String.Format("L{0} ", PointToString(points[i], " ")));
			int lastIndex = points.Count - 1;
			if (points[lastIndex] != points[0])
				commandsLine.Append(String.Format("L{0} ", PointToString(points[lastIndex], " ")));
			commandsLine.Append("Z ");
			return commandsLine.ToString();
		}
		internal string CreateSegmentCommands(IPathSegmentCore segment) {
			if (segment.PointCount <= 1) return String.Empty;
			StringBuilder commandsLine = new StringBuilder();
			IList<CoordPoint> points = GetPolygonPoints(segment).ToList();
			commandsLine.Append(CreateCommandsLine(points));
			for (int j = 0; j < segment.InnerContourCount; j++) {
				IPointContainerCore innerContour = segment.GetInnerCountour(j);
				IList<CoordPoint> innerPoints = GetPolygonPoints(innerContour).ToList();
				commandsLine.Append(CreateCommandsLine(innerPoints));
			}
			return commandsLine.ToString();
		}
		internal void ProcessLine(ISupportIntermediatePoints intermediatePoints, IMapShapeStyleCore shapeStyle) {
			if (intermediatePoints == null) return;
			if (intermediatePoints.Vertices.Count > 2) {
				ProcessPolyline(intermediatePoints, shapeStyle);
			} else {
				XmlElement lineElement = CreateSvgEntity(SvgTokens.Line);
				AddLineCoordinatesToElement(intermediatePoints.Vertices, lineElement);
				AddStylesToElement(shapeStyle, lineElement);
			}
		}
		internal void ProcessPolyline(ISupportIntermediatePoints intermediatePoints, IMapShapeStyleCore shapeStyle) {
			if (intermediatePoints == null) return;
			XmlElement polylineElement = CreateSvgEntity(SvgTokens.Polyline);
			AddPointsToElement(intermediatePoints.Vertices, polylineElement);
			AddStylesToElement(shapeStyle, polylineElement);
		}
		internal void ProcessPolygon(IPolygonCore polygon, IMapShapeStyleCore shapeStyle) {
			if (polygon == null) return;
			XmlElement polylineElement = CreateSvgEntity(SvgTokens.Polygon);
			IList<CoordPoint> points = GetPolygonPoints(polygon).ToList();
			AddPointsToElement(points, polylineElement, true);
			AddStylesToElement(shapeStyle, polylineElement);
		}
		internal void ProcessPath(IPathCore path, IMapShapeStyleCore shapeStyle) {
			if (path == null) return;
			XmlElement pathElement = CreateSvgEntity(SvgTokens.Path);
			AddPathSegmentsToElement(path, pathElement);
			AddStylesToElement(shapeStyle, pathElement);
		}
		internal void ProcessRectangle(ISupportRectangle supportRectangle, IMapShapeStyleCore shapeStyle) {
			if (supportRectangle == null) return;
			XmlElement pathElement = CreateSvgEntity(SvgTokens.Rectangle);
			AddRectangleToElement(supportRectangle.Location, supportRectangle.Width, supportRectangle.Height, pathElement);
			AddStylesToElement(shapeStyle, pathElement);
		}
		internal void ProcessEllipse(ISupportRectangle supportRectangle, IMapShapeStyleCore shapeStyle) {
			if (supportRectangle == null) return;
			XmlElement pathElement = CreateSvgEntity(SvgTokens.Ellipse);
			AddEllipseToElement(supportRectangle.Location, supportRectangle.Width, supportRectangle.Height, pathElement);
			AddStylesToElement(shapeStyle, pathElement);
		}
		internal void Processing(IEnumerable<TItem> items) {
			foreach (TItem item in items) {
				switch (GetEntityByType(item)) {
					case SvgEntityTypes.Line:
						ProcessLine(item as ISupportIntermediatePoints, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Polyline:
						ProcessPolyline(item as ISupportIntermediatePoints, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Polygon:
						ProcessPolygon(item as IPolygonCore, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Path:
						ProcessPath(item as IPathCore, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Rectangle:
						ProcessRectangle(item as ISupportRectangle, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Ellipse:
						ProcessEllipse(item as ISupportRectangle, item as IMapShapeStyleCore); break;
					case SvgEntityTypes.Point: break;
				}
			}
		}
		internal SvgPoint CoordPointToSvgPoint(CoordPoint coordPoint) {
			return PointConverter != null ? PointConverter.CoordToSvgPoint(coordPoint) : new SvgPoint(coordPoint.GetX(), coordPoint.GetY());
		}
		internal SvgSize CoordSizeToSvgSize(CoordPoint coordPoint, SvgSize coordSize) {
			return PointConverter != null ? PointConverter.CoordToSvgSize(coordPoint, coordSize) : coordSize;
		}
		internal int GetCanvasWidth() {
			double width = CanvasSize.Width > 0 ? CanvasSize.Width : DefaultWidth;
			return (int)Math.Ceiling(width * ScaleFactor);
		}
		internal int GetCanvasHeight() {
			double height = CanvasSize.Height > 0 ? CanvasSize.Height : DefaultHeight;
			return (int)Math.Ceiling(height * ScaleFactor);
		}
		internal double ConvertStrokeWidth(double strokeWidth) {
			return strokeWidth * StrokeScaleFactor * ScaleFactor;
		}
		protected internal override string PointToString(CoordPoint point) {
			return this.PointToString(point, ",");
		}
		protected internal override string PointToString(CoordPoint point, string pointsSeparator) {
			return CoordPointToSvgPoint(point).ToString(PointsExportCulture, pointsSeparator);
		}
	}
}
