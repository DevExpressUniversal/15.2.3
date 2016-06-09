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
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class FunnelSeriesViewBase : SimpleDiagramSeriesViewBase {
		const double DefaultHeightToWidthRatio = 1.0;
		const int DefaultPointDistance = 0;
		double ratio = DefaultHeightToWidthRatio;
		int pointDistance = DefaultPointDistance;
		protected internal virtual bool ActualHeightToWidthRatioAuto {
			get { return false; }
		}
		protected internal override bool ShouldSortPoints { get { return false; } }
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return false;
			}
		}
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.SimpleView; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewBaseHeightToWidthRatio"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesViewBase.HeightToWidthRatio"),
		XtraSerializableProperty
		]
		public double HeightToWidthRatio {
			get { return this.ratio; }
			set {
				if (value != ratio) {
					SendNotification(new ElementWillChangeNotification(this));
					ratio = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewBasePointDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesViewBase.PointDistance"),
		XtraSerializableProperty
		]
		public int PointDistance {
			get { return pointDistance; }
			set {
				if (value != pointDistance) {
					if (!Loading)
						ValidatePointDistance(value);
					SendNotification(new ElementWillChangeNotification(this));
					pointDistance = value;
					RaiseControlChanged();
				}
			}
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "HeightToWidthRatio")
				return ShouldSerializeHeightToWidthRatio();
			if (propertyName == "PointDistance")
				return ShouldSerializePointDistance();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeHeightToWidthRatio() {
			return ratio != DefaultHeightToWidthRatio;
		}
		void ResetHeightToWidthRatio() {
			HeightToWidthRatio = DefaultHeightToWidthRatio;
		}
		bool ShouldSerializePointDistance() {
			return pointDistance != DefaultPointDistance;
		}
		void ResetPointDistance() {
			PointDistance = DefaultPointDistance;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeHeightToWidthRatio() ||
				ShouldSerializePointDistance();
		}
		#endregion        
		PointOrder GetPointOrder(int pointIndex, int pointsCount) {
			if (pointsCount == 1)
				return PointOrder.Single;
			else if (pointIndex == 0)
				return PointOrder.First;
			else if (pointIndex == pointsCount - 1)
				return PointOrder.Last;
			return PointOrder.Middle;
		}
		void ValidatePointDistance(int val) {
			if (val < 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointDistance));
		}
		VariousPolygon GetPolygonForLegendMarker(Rectangle bounds) {
			float heightOfSmallPolygon = bounds.Height / 2;
			float secondY = bounds.Y + heightOfSmallPolygon;
			float widthOfSmallPolygon = bounds.Width / 3;
			float secondX = bounds.X + widthOfSmallPolygon;
			float thirdX = secondX + widthOfSmallPolygon;
			LineStrip points = new LineStrip();
			points.Add(new GRealPoint2D(bounds.X, bounds.Y));
			points.Add(new GRealPoint2D(bounds.Right, bounds.Y));
			points.Add(new GRealPoint2D(thirdX, secondY));
			points.Add(new GRealPoint2D(thirdX, bounds.Bottom));
			points.Add(new GRealPoint2D(secondX, bounds.Bottom));
			points.Add(new GRealPoint2D(secondX, secondY));
			return new VariousPolygon(points, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height));
		}
		protected Rectangle CorrectBoundsBySizeAndRatio(Rectangle bounds) {
			if (ActualHeightToWidthRatioAuto)
				return bounds;
			int newHeight = (int)(bounds.Width * HeightToWidthRatio);
			int newWidth = bounds.Width;
			if (newHeight > bounds.Height) {
				newHeight = bounds.Height;
				newWidth = MathUtils.Ceiling(newHeight / HeightToWidthRatio);
			}
			int dx = MathUtils.StrongRound((bounds.Width - newWidth) / 2.0);
			int dy = MathUtils.StrongRound((bounds.Height - newHeight) / 2.0);
			return new Rectangle(bounds.X + dx, bounds.Y + dy, newWidth, newHeight);
		}
		protected override NormalizedValuesCalculator CreateNormalizedCalculator(ISeries series) {
			return new FunnelNormalizedValuesCalculator(series);
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			RenderLegendMarker(renderer, GetPolygonForLegendMarker(bounds), (SimpleDiagramDrawOptionsBase)seriesPointDrawOptions, selectionState);
		}
		protected override PatternDataProvider CreatePatternDataProvider(PatternConstants patternConstant) {
			return new FunnelPointPatternDataProvider(patternConstant);
		}
		protected abstract GraphicsCommand CreateGraphicsCommand(FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds);
		protected abstract void Render(IRenderer renderer, FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds);
		protected abstract void RenderLegendMarker(IRenderer renderer, VariousPolygon polygon, SimpleDiagramDrawOptionsBase drawOptions, SelectionState selectionState);
		protected virtual PointF CalculateBasePoint(ISimpleDiagramDomain domain, Rectangle correctedBounds) {
			return new PointF(correctedBounds.X + correctedBounds.Width / 2.0f, correctedBounds.Y);
		}   
		protected internal virtual int CalculateHeightOfPolygon(int height, List<RefinedPointData> filteredPointsData, out int actualPointDistance, out int residueHeightOfPolygon, out int residuePointDistance) {
			int count = filteredPointsData.Count;
			int tempPointDistance = actualPointDistance = pointDistance == 0 ? 0 : pointDistance + 1;
			residueHeightOfPolygon = residuePointDistance = 0;
			if (count == 0 || height < count)
				return 0;
			int heightOfPolygon;			
			if (pointDistance != 0 && count != 1 && (height - count) < ((count - 1) * tempPointDistance)) {
				actualPointDistance = (height - count) / (count - 1);
				residuePointDistance = (height - count) % (count - 1);
				heightOfPolygon = 1;
			}
			else {
				int dividend = height - (count - 1) * tempPointDistance;
				heightOfPolygon = dividend / count;
				residueHeightOfPolygon = dividend % count;
			}			
			return heightOfPolygon;
		}
		protected internal virtual DiagramPoint? CalculateRelativeToolTipPosition(SeriesPointLayout pointLayout) {
			return null;
		}
		protected internal override PointOptions CreatePointOptions() {
			return new FunnelPointOptions();
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return CreateGraphicsCommand((FunnelSeriesPointLayout)pointLayout, (SimpleDiagramDrawOptionsBase)drawOptions, mappingBounds);
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			Render(renderer, (FunnelSeriesPointLayout)pointLayout, (SimpleDiagramDrawOptionsBase)drawOptions, mappingBounds);
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected internal override object[] GenerateRandomValues(int indexOfRandomPoint, int randomPointCount, ScaleType valueScaleType) {
			int startOfMaxDiapason = (randomPointCount - 1) * 2;
			double value = Math.Round(random.NextDouble() * 2 + startOfMaxDiapason - indexOfRandomPoint * 2, 1);
			return new object[] { value };
		}
		protected internal override IEnumerable<SeriesPointLayout> CalculatePointsLayout(ISimpleDiagramDomain domain, RefinedSeriesData seriesData) {
			List<SeriesPointLayout> pointsLayout = new List<SeriesPointLayout>();
			FunnelWholeSeriesViewData funnelWholeViewData = seriesData.WholeViewData as FunnelWholeSeriesViewData;
			if (funnelWholeViewData == null)
				return pointsLayout;
			List<RefinedPointData> filteredPointsData = funnelWholeViewData.FilteredPointsData;
			int count = filteredPointsData.Count;
			if (count == 0 || seriesData.Count == 0)
				return pointsLayout;
			Rectangle correctedBounds = CorrectBoundsBySizeAndRatio(domain.ElementBounds);
			if (!correctedBounds.AreWidthAndHeightPositive())
				return pointsLayout;
			IList<RefinedPoint> refinedPoints = new List<RefinedPoint>();
			foreach (var pointData in filteredPointsData)
				refinedPoints.Add(pointData.RefinedPoint);
			Funnel2DLayoutCalculator funnel2DLayoutCalculator = new Funnel2DLayoutCalculator(new GRealSize2D(correctedBounds.Width, correctedBounds.Height), refinedPoints, pointDistance == 0 ? pointDistance : pointDistance + 1);
			List<FunnelPointInfo> pointInfos = funnel2DLayoutCalculator.Calculate();
			if (pointInfos != null) {
				for (int i = 0; i < pointInfos.Count; i++) {
					PointOrder pointOrder = GetPointOrder(i, pointInfos.Count);
					FunnelPointInfo pointInfo = pointInfos[i];
					PointF basePoint = CalculateBasePoint(domain, correctedBounds);
					LineStrip points = new LineStrip();
					points.Add(new GRealPoint2D(MathUtils.StrongRound(correctedBounds.X + pointInfo.TopLeftPoint.X), MathUtils.StrongRound(basePoint.Y + pointInfo.TopLeftPoint.Y)));
					points.Add(new GRealPoint2D(MathUtils.StrongRound(correctedBounds.X + pointInfo.TopRightPoint.X), MathUtils.StrongRound(basePoint.Y + pointInfo.TopRightPoint.Y)));
					points.Add(new GRealPoint2D(MathUtils.StrongRound(correctedBounds.X + pointInfo.BottomRightPoint.X), MathUtils.StrongRound(basePoint.Y + pointInfo.BottomRightPoint.Y)));
					points.Add(new GRealPoint2D(MathUtils.StrongRound(correctedBounds.X + pointInfo.BottomLeftPoint.X), MathUtils.StrongRound(basePoint.Y + pointInfo.BottomLeftPoint.Y)));
					RectangleF rect = new RectangleF(
						correctedBounds.X,
						(float)(basePoint.Y + pointInfo.TopLeftPoint.Y),
						correctedBounds.Width,
						(float)Math.Floor(pointInfo.BottomLeftPoint.Y - pointInfo.TopLeftPoint.Y) + 1);
					FunnelSeriesPointLayout layout = new FunnelSeriesPointLayout(filteredPointsData[i], points, rect, funnelWholeViewData.NegativeValuePresents, pointOrder);
					filteredPointsData[i].ToolTipRelativePosition = CalculateRelativeToolTipPosition(layout);
					pointsLayout.Add(layout);
				}
			}
			return pointsLayout;
		}
		protected internal override WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			List<RefinedPointData> filteredPointsData = new List<RefinedPointData>();
			bool negativeValuePresents = false;
			foreach (RefinedPointData pointData in seriesData) {
				IFunnelPoint point = pointData.RefinedPoint;
				if (point != null && !point.IsEmpty) {
					negativeValuePresents |= point.Value < 0;
					if (!Double.IsNaN(((IFunnelPoint)point).NormalizedValue))
						filteredPointsData.Add(pointData);
				}
			}
			return new FunnelWholeSeriesViewData(filteredPointsData, negativeValuePresents);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FunnelSeriesViewBase view = obj as FunnelSeriesViewBase;
			if (view == null)
				return;
			this.ratio = view.ratio;
			this.pointDistance = view.pointDistance;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			FunnelSeriesViewBase view = (FunnelSeriesViewBase)obj;
			return 
				ratio == view.ratio &&
				pointDistance == view.pointDistance; 
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public enum PointOrder {
		First,
		Middle,
		Last,
		Single
	}
	public class FunnelWholeSeriesViewData : SimpleDiagramWholeSeriesViewData {
		List<RefinedPointData> filteredPointsData;
		bool negativeValuePresents;
		public List<RefinedPointData> FilteredPointsData { get { return filteredPointsData; } }
		public override bool NegativeValuePresents { get { return negativeValuePresents; } }
		public FunnelWholeSeriesViewData(List<RefinedPointData> filteredPointsData, bool negativeValuePresents) {
			this.filteredPointsData = filteredPointsData;
			this.negativeValuePresents = negativeValuePresents;
		}
	}
	public class FunnelSeriesPointLayout : SeriesPointLayout {
		readonly LineStrip points;
		readonly VariousPolygon polygon;
		readonly bool isNegativeValuePresents;
		readonly PointOrder pointOrder;
		public VariousPolygon Polygon { get { return polygon; } }
		public bool IsNegativeValuePresents { get { return isNegativeValuePresents; } }
		public GRealPoint2D LeftUpPoint { get { return points[0]; } }
		public GRealPoint2D RightUpPoint { get { return points[1]; } }
		public GRealPoint2D RightDownPoint { get { return points[2]; } }
		public GRealPoint2D LeftDownPoint { get { return points[3]; } }
		public PointOrder PointOrder { get { return pointOrder; } }		
		public FunnelSeriesPointLayout(RefinedPointData pointData, LineStrip points, RectangleF rectangle, bool isNegativeValuePresents, PointOrder pointOrder)
			: base(pointData) {
			this.points = points;
			polygon = new VariousPolygon(points, rectangle);
			this.isNegativeValuePresents = isNegativeValuePresents;
			this.pointOrder = pointOrder;
		}
		void CorrectWidth(int correction) {
			points[1] = new GRealPoint2D(points[1].X + correction, points[1].Y);
			points[2] = new GRealPoint2D(points[2].X + correction, points[2].Y);
		}
		public bool CheckWidth() {
			if (points[0].X == points[1].X && points[2].X == points[3].X)
				return false;
			return true;
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer region = base.CalculateHitRegion();
			if (CheckWidth())
				region.Union(new HitRegion(polygon));
			else {
				CorrectWidth(1);
				region.Union(new HitRegion(polygon));
				CorrectWidth(-1);
			}
			return region;
		}
	}
}
