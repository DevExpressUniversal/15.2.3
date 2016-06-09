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
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Utils;
using System;
using System.Windows;
namespace DevExpress.Xpf.Map.Native {
	public abstract class XpfSvgPointConverterBase : ISvgPointConverter {
		readonly MapCoordinateSystem coordinateSystem;
		readonly VectorLayerBase layer;
		readonly CoordBounds itemsBounds;
		readonly CoordBounds sceneBounds;
		SvgPoint startPoint;
		SvgSize boundsRatio;
		protected MapCoordinateSystem CoordinateSystem { get { return coordinateSystem; } }
		protected VectorLayerBase Layer { get { return layer; } }
		protected CoordBounds ItemsBounds { get { return itemsBounds; } }
		protected CoordBounds SceneBounds { get { return sceneBounds; } }
		protected internal SvgPoint StartPoint { get { return startPoint; } set { startPoint = value; } }
		protected internal SvgSize BoundsRatio { get { return boundsRatio; } set { boundsRatio = value; } }
		protected XpfSvgPointConverterBase(MapCoordinateSystem coordinateSystem, VectorLayerBase layer) {
			this.coordinateSystem = coordinateSystem;
			this.layer = layer;
			this.sceneBounds = GetSceneBounds();
			ResetBoundsRatio();
		}
		protected XpfSvgPointConverterBase(MapCoordinateSystem coordinateSystem, VectorLayerBase layer, CoordBounds itemsBounds)
			: this(coordinateSystem, layer) {
			this.itemsBounds = itemsBounds;
		}
		protected void ResetBoundsRatio() {
			this.boundsRatio = new SvgSize(1.0, 1.0);
		}
		protected void ResetStartPoint() {
			this.startPoint = new SvgPoint(0.0, 0.0);
		}
		protected SvgPoint GetPrintingPoint(SvgPoint point) {
			return new SvgPoint(point.X * this.boundsRatio.Width, point.Y * this.boundsRatio.Height);
		}
		protected SvgSize GetPrintingSize(SvgSize size) {
			return new SvgSize(size.Width * this.boundsRatio.Width, size.Height * this.boundsRatio.Height);
		}
		protected Rect CalculatePrintingBounds(CoordPoint boundaryPoint1, CoordPoint boundaryPoint2) {
			CoordPoint leftTopPoint = GetLeftTopPoint(boundaryPoint1, boundaryPoint2);
			CoordPoint rightBottomPoint = GetRightBottomPoint(boundaryPoint1, boundaryPoint2);
			SvgPoint leftTopSvg = leftTopPoint != null ? CoordToSvgPointInternal(leftTopPoint) : SvgPoint.Empty;
			SvgPoint rightBottomSvg = rightBottomPoint != null ? CoordToSvgPointInternal(rightBottomPoint) : SvgPoint.Empty;
			leftTopSvg = CorrectLeftTopSvgBorder(leftTopPoint, rightBottomPoint, leftTopSvg, rightBottomSvg);
			rightBottomSvg = CorrectRightBottomSvgBorder(leftTopPoint, rightBottomPoint, leftTopSvg, rightBottomSvg);
			return new Rect(new Point(leftTopSvg.X, leftTopSvg.Y), new Point(rightBottomSvg.X, rightBottomSvg.Y));
		}
		protected void CalculateStartPointAndRatio(Rect svgPrintingBounds) {
			this.boundsRatio = new SvgSize(svgPrintingBounds.Width / this.itemsBounds.Width, svgPrintingBounds.Height / this.itemsBounds.Height);
			if(double.IsInfinity(this.boundsRatio.Width) || double.IsInfinity(this.boundsRatio.Height))
				ResetBoundsRatio();
			this.startPoint = new SvgPoint(svgPrintingBounds.Left - this.itemsBounds.X1 * this.boundsRatio.Width, svgPrintingBounds.Top - ItemsBounds.Y2 * BoundsRatio.Height);
		}
		protected abstract CoordPoint GetLeftTopPoint(CoordPoint point1, CoordPoint point2);
		protected abstract CoordPoint GetRightBottomPoint(CoordPoint point1, CoordPoint point2);
		protected abstract SvgPoint CorrectLeftTopSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg);
		protected abstract SvgPoint CorrectRightBottomSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg);
		protected abstract SvgPoint CoordToSvgPointInternal(CoordPoint point);
		protected internal virtual CoordBounds GetSceneBounds() {
			CoordBounds layerBounds = Layer != null ? Layer.GetBoundingRect() : CoordBounds.Empty;
			if(!layerBounds.IsEmpty)
				return layerBounds;
			return CoordinateSystem.BoundingBox;
		}
		public abstract void SetPrintingBounds(CoordPoint boundaryPoint1, CoordPoint boundaryPoint2);
		public abstract SvgPoint CoordToSvgPoint(CoordPoint point);
		public abstract CoordPoint SvgToCoordPoint(SvgPoint point);
		public abstract SvgSize CoordToSvgSize(CoordPoint location, SvgSize size);
		public abstract SvgSize SvgToCoordSize(SvgPoint point, SvgSize size);
	}
	public class XpfSvgGeoPointConverter : XpfSvgPointConverterBase {
		readonly Rect sceneSvgBounds;
		MapControl Map { get { return Layer != null ? Layer.Map : null; } }
		public XpfSvgGeoPointConverter(MapCoordinateSystem coordinateSystem, VectorLayerBase layer)
			: base(coordinateSystem, layer) {
			this.sceneSvgBounds = CalculateSceneSvgBounds(SceneBounds);
			StartPoint = new SvgPoint(sceneSvgBounds.Left, sceneSvgBounds.Top);
		}
		public XpfSvgGeoPointConverter(MapCoordinateSystem coordinateSystem, VectorLayerBase layer, CoordBounds itemsBounds)
			: base(coordinateSystem, layer, itemsBounds) {
			this.sceneSvgBounds = CalculateSceneSvgBounds(SceneBounds);
			StartPoint = new SvgPoint(sceneSvgBounds.Left, sceneSvgBounds.Top);
		}
		bool IsCorrectRectBounds(Rect rectBounds) {
			return !double.IsNaN(rectBounds.Left) && !double.IsNaN(rectBounds.Top) && !double.IsNaN(rectBounds.Right) && !double.IsNaN(rectBounds.Bottom);
		}
		Rect CalculateSceneSvgBounds(CoordBounds bounds) {
			SvgPoint p1 = CoordToSvgPoint(new GeoPoint(bounds.Y1, bounds.X1));
			SvgPoint p2 = CoordToSvgPoint(new GeoPoint(bounds.Y2, bounds.X2));
			return new Rect(new Point(p1.X, p1.Y), new Point(p2.X, p2.Y));
		}
		SvgSize CalculatePrintingSize(SvgSize maxSize, SvgSize realSize) {
			double widthRatio = realSize.Width / maxSize.Width;
			double heightRatio = realSize.Height / maxSize.Height;
			if(widthRatio > 1.0 || heightRatio > 1.0) {
				double indexRatio = widthRatio > heightRatio ? widthRatio : heightRatio;
				return new SvgSize(realSize.Width / indexRatio, realSize.Height / indexRatio);
			}
			return realSize;
		}
		CoordPoint CheckCorrectCoordBorder(CoordPoint coordPoint) {
			if(SceneBounds.IsEmpty || coordPoint != null && SceneBounds.IsPointInBounds(coordPoint))
				return coordPoint;
			return null;
		}
		Rect CorrectLeftTopPrintingBounds(Rect svgPrintingBounds, SvgSize realPrintingSize) {
			if(svgPrintingBounds.Left >= sceneSvgBounds.Left && svgPrintingBounds.Top >= sceneSvgBounds.Top)
				return svgPrintingBounds;
			double left = svgPrintingBounds.Left;
			double top = svgPrintingBounds.Top;
			if(left < sceneSvgBounds.Left) left = sceneSvgBounds.Left;
			if(top < sceneSvgBounds.Top) top = sceneSvgBounds.Top;
			Rect validPrintingRect = new Rect(new Point(left, top), new Point(svgPrintingBounds.Right, svgPrintingBounds.Bottom));
			SvgSize validPrintingSize = CalculateValidPrintingSize(validPrintingRect, realPrintingSize);
			return new Rect(new Point(svgPrintingBounds.Right - validPrintingSize.Width, svgPrintingBounds.Bottom - validPrintingSize.Height), new Point(svgPrintingBounds.Right, svgPrintingBounds.Bottom));
		}
		Rect CorrectRightBottomPrintingBounds(Rect svgPrintingBounds, SvgSize realPrintingSize) {
			if(svgPrintingBounds.Right <= sceneSvgBounds.Right && svgPrintingBounds.Bottom <= sceneSvgBounds.Bottom)
				return svgPrintingBounds;
			double right = svgPrintingBounds.Right;
			double bottom = svgPrintingBounds.Bottom;
			if(right > sceneSvgBounds.Right) right = sceneSvgBounds.Right;
			if(bottom > sceneSvgBounds.Bottom) bottom = sceneSvgBounds.Bottom;
			Rect validPrintingRect = new Rect(new Point(svgPrintingBounds.Left, svgPrintingBounds.Top), new Point(right, bottom));
			SvgSize validPrintingSize = CalculateValidPrintingSize(validPrintingRect, realPrintingSize);
			return new Rect(new Point(svgPrintingBounds.Left, svgPrintingBounds.Top), new Point(svgPrintingBounds.Left + validPrintingSize.Width, svgPrintingBounds.Top + validPrintingSize.Height));
		}
		Rect CorrectPrintingBounds(Rect svgPrintingBounds) {
			if(!IsCorrectRectBounds(sceneSvgBounds))
				return svgPrintingBounds;
			SvgSize realPrintingSize = new SvgSize(svgPrintingBounds.Right - svgPrintingBounds.Left, svgPrintingBounds.Bottom - svgPrintingBounds.Top);
			svgPrintingBounds = CorrectLeftTopPrintingBounds(svgPrintingBounds, realPrintingSize);
			svgPrintingBounds = CorrectRightBottomPrintingBounds(svgPrintingBounds, realPrintingSize);
			return svgPrintingBounds;
		}
		SvgSize CalculateValidPrintingSize(Rect svgPrintingBounds, SvgSize realPrintingSize) {
			SvgSize maxPrintingSize = new SvgSize(svgPrintingBounds.Right - svgPrintingBounds.Left, svgPrintingBounds.Bottom - svgPrintingBounds.Top);
			return CalculatePrintingSize(maxPrintingSize, realPrintingSize);
		}
		protected override CoordPoint GetLeftTopPoint(CoordPoint point1, CoordPoint point2) {
			if(point1 != null && point2 != null)
				return new GeoPoint(Math.Max(point1.GetY(), point2.GetY()), Math.Min(point1.GetX(), point2.GetX()));
			return point1;
		}
		protected override CoordPoint GetRightBottomPoint(CoordPoint point1, CoordPoint point2) {
			if(point1 != null && point2 != null)
				return new GeoPoint(Math.Min(point1.GetY(), point2.GetY()), Math.Max(point1.GetX(), point2.GetX()));
			return point2;
		}
		protected override SvgPoint CorrectLeftTopSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg) {
			if(leftTopBorder != null)
				return leftTopSvg;
			if(rightBottomBorder != null)
				return new SvgPoint(rightBottomSvg.X - ItemsBounds.Width, rightBottomSvg.Y - ItemsBounds.Height);
			return new SvgPoint(sceneSvgBounds.Left, sceneSvgBounds.Top);
		}
		protected override SvgPoint CorrectRightBottomSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg) {
			if(rightBottomBorder != null)
				return rightBottomSvg;
			return new SvgPoint(leftTopSvg.X + ItemsBounds.Width, leftTopSvg.Y + ItemsBounds.Height);
		}
		protected override SvgPoint CoordToSvgPointInternal(CoordPoint point) {
			MapSizeCore mapSizeInPixels = MathUtils.CalcMapSizeInPixels(1.0, new MapSizeCore(Map.InitialMapSize.Width, Map.InitialMapSize.Height));
			Point screenPoint = CoordinateSystem.CoordToScreenPointIdentity(Map.ViewportInPixels, point, new Size(mapSizeInPixels.Width, mapSizeInPixels.Height));
			return new SvgPoint(screenPoint.X, screenPoint.Y);
		}
		protected internal override CoordBounds GetSceneBounds() {
			return CoordinateSystem.BoundingBox;
		}
		public override void SetPrintingBounds(CoordPoint boundaryPoint1, CoordPoint boundaryPoint2) {
			boundaryPoint1 = CheckCorrectCoordBorder(boundaryPoint1);
			boundaryPoint2 = CheckCorrectCoordBorder(boundaryPoint2);
			Rect svgPrintingBounds = CorrectPrintingBounds(CalculatePrintingBounds(boundaryPoint1, boundaryPoint2));
			CalculateStartPointAndRatio(svgPrintingBounds);
		}
		public override SvgPoint CoordToSvgPoint(CoordPoint point) {
			return CoordToSvgPointInternal(point) - StartPoint;
		}
		public override CoordPoint SvgToCoordPoint(SvgPoint point) {
			SvgPoint printingPoint = GetPrintingPoint(point);
			Point screenPoint = new Point(printingPoint.X + StartPoint.X, printingPoint.Y + StartPoint.Y);
			Rect viewport = CoordinateSystem.CalculateViewport(1.0, CoordinateSystem.Center, Map.ViewportInPixels, Map.InitialMapSize);
			MapUnit unit = CoordinateSystem.ScreenPointToMapUnit(screenPoint, viewport, Map.ViewportInPixels);
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		public override SvgSize CoordToSvgSize(CoordPoint location, SvgSize sourceSize) {
			Size coordSize = CoordinateSystem.KilometersToGeoSize((GeoPoint)location, new Size(sourceSize.Width, sourceSize.Height));
			CoordPoint oppositeLocation = location.Offset(coordSize.Width, coordSize.Height);
			SvgPoint oppositePoint = CoordToSvgPoint(oppositeLocation);
			SvgPoint point = CoordToSvgPoint(location);
			return new SvgSize(oppositePoint.X - point.X, point.Y - oppositePoint.Y);
		}
		public override SvgSize SvgToCoordSize(SvgPoint point, SvgSize sourceSize) {
			CoordPoint location = SvgToCoordPoint(point);
			SvgPoint oppositePoint = new SvgPoint(point.X + sourceSize.Width, point.Y - sourceSize.Height);
			CoordPoint oppositeLocation = SvgToCoordPoint(oppositePoint);
			Size coordSize = new Size(oppositeLocation.GetX() - location.GetX(), oppositeLocation.GetY() - location.GetY());
			Size size = CoordinateSystem.GeoToKilometersSize((GeoPoint)location, coordSize);
			return new SvgSize(size.Width, size.Height);
		}
	}
	public class XpfSvgCartesianPointConverter : XpfSvgPointConverterBase {
		readonly double maxOrdinate;
		double maxPrintingOrdinate;
		public XpfSvgCartesianPointConverter(MapCoordinateSystem coordinateSystem, VectorLayerBase layer)
			: base(coordinateSystem, layer) {
			this.maxOrdinate = SceneBounds.Y1 + SceneBounds.Y2;
			RecalculatePrintingOrdinate();
			StartPoint = new SvgPoint(SceneBounds.X1, SceneBounds.Y2);
		}
		public XpfSvgCartesianPointConverter(MapCoordinateSystem coordinateSystem, VectorLayerBase layer, CoordBounds itemsBounds)
			: base(coordinateSystem, layer, itemsBounds) {
			this.maxOrdinate = ItemsBounds.Y1 + ItemsBounds.Y2;
			RecalculatePrintingOrdinate();
			ResetStartPoint();
			UpdateMapBounds(itemsBounds);
		}
		void UpdateMapBounds(CoordBounds bounds) {
			CoordinateSystem.UpdateBoundingBox(bounds);
			CoordinateSystem.SetNeedUpdateBoundingBox(true);
		}
		void RecalculatePrintingOrdinate() {
			this.maxPrintingOrdinate = this.maxOrdinate * BoundsRatio.Height;
		}
		protected override CoordPoint GetLeftTopPoint(CoordPoint point1, CoordPoint point2) {
			if(point1 != null && point2 != null)
				return new CartesianPoint(Math.Min(point1.GetX(), point2.GetX()), Math.Max(point1.GetY(), point2.GetY()));
			return point1;
		}
		protected override CoordPoint GetRightBottomPoint(CoordPoint point1, CoordPoint point2) {
			if(point1 != null && point2 != null)
				return new CartesianPoint(Math.Max(point1.GetX(), point2.GetX()), Math.Min(point1.GetY(), point2.GetY()));
			return point2;
		}
		protected override SvgPoint CorrectLeftTopSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg) {
			if(leftTopBorder != null)
				return leftTopSvg;
			return new SvgPoint(rightBottomSvg.X - ItemsBounds.Width, rightBottomSvg.Y + ItemsBounds.Height);
		}
		protected override SvgPoint CorrectRightBottomSvgBorder(CoordPoint leftTopBorder, CoordPoint rightBottomBorder, SvgPoint leftTopSvg, SvgPoint rightBottomSvg) {
			if(rightBottomBorder != null)
				return rightBottomSvg;
			return new SvgPoint(leftTopSvg.X + ItemsBounds.Width, leftTopSvg.Y - ItemsBounds.Height);
		}
		protected override SvgPoint CoordToSvgPointInternal(CoordPoint point) {
			return new SvgPoint(point.GetX(), point.GetY());
		}
		public override void SetPrintingBounds(CoordPoint boundaryPoint1, CoordPoint boundaryPoint2) {
			if(boundaryPoint1 == null && boundaryPoint2 == null) {
				ResetBoundsRatio();
				ResetStartPoint();
				return;
			}
			Rect svgPrintingBounds = CalculatePrintingBounds(boundaryPoint1, boundaryPoint2);
			CalculateStartPointAndRatio(svgPrintingBounds);
			RecalculatePrintingOrdinate();
			UpdateMapBounds(new CoordBounds(svgPrintingBounds.Left, svgPrintingBounds.Top, svgPrintingBounds.Right, svgPrintingBounds.Bottom));
		}
		public override SvgPoint CoordToSvgPoint(CoordPoint point) {
			SvgPoint position = CoordToSvgPointInternal(point);
			return new SvgPoint(position.X - StartPoint.X, this.maxPrintingOrdinate - position.Y - StartPoint.Y);
		}
		public override CoordPoint SvgToCoordPoint(SvgPoint point) {
			SvgPoint printingPoint = GetPrintingPoint(point);
			return new CartesianPoint(printingPoint.X + StartPoint.X, this.maxPrintingOrdinate - printingPoint.Y + StartPoint.Y);
		}
		public override SvgSize CoordToSvgSize(CoordPoint location, SvgSize sourceSize) {
			return sourceSize;
		}
		public override SvgSize SvgToCoordSize(SvgPoint point, SvgSize sourceSize) {
			return GetPrintingSize(sourceSize);
		}
	}
}
