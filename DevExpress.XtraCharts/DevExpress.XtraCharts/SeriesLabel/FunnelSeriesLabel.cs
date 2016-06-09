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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.GLGraphics;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum FunnelSeriesLabelPosition {
		LeftColumn,
		Left,
		Center,
		Right,
		RightColumn
	}
	[
	TypeConverter(typeof(FunnelSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FunnelSeriesLabel : SeriesLabelBase {
		const FunnelSeriesLabelPosition DefaultPosition = FunnelSeriesLabelPosition.Right;
		FunnelSeriesLabelPosition position = DefaultPosition;
		bool IsOutsidePosition { get { return position == FunnelSeriesLabelPosition.Left || position == FunnelSeriesLabelPosition.Right; } }
		bool IsColumnPosition { get { return position == FunnelSeriesLabelPosition.LeftColumn || position == FunnelSeriesLabelPosition.RightColumn; } }
		protected virtual bool IsFlatConnector { get { return true; } }
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return position != FunnelSeriesLabelPosition.Center; } }
		protected internal override bool ResolveOverlappingSupported { get { return false; } }
		protected internal override bool ResolveOverlappingEnabled { get { return false; } }
		protected internal override bool VerticalRotationSupported { get { return false; } }
		internal FunnelSeriesViewBase View { get { return (FunnelSeriesViewBase)Series.View; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesLabel.Position"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public FunnelSeriesLabelPosition Position {
			get { return position; }
			set {
				if (value != position) {
					SendNotification(new ElementWillChangeNotification(this));
					position = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override TextOrientation TextOrientation { get { return DefaulTextOrientation; } }
		public FunnelSeriesLabel()
			: base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Position":
					return ShouldSerializePosition();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePosition() {
			return position != DefaultPosition;
		}
		void ResetPosition() {
			Position = DefaultPosition;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePosition();
		}
		#endregion
		Size CalculateMaximumLabelSize(RefinedSeriesData seriesData) {
			Size maxSize = Size.Empty;
			if (seriesData.Count == 0)
				return maxSize;
			foreach (RefinedPointData pointData in seriesData)
				if (pointData.LabelViewData.Length > 0) {
					SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
					if (labelViewData != null) {
						Size labelSize = labelViewData.TextSize;
						if (labelSize.Width > maxSize.Width)
							maxSize.Width = labelSize.Width;
						if (labelSize.Height > maxSize.Height)
							maxSize.Height = labelSize.Height;
					}
				}
			int borderSize = Border.ActualThickness * 2;
			maxSize.Width += borderSize;
			maxSize.Height += borderSize;
			return maxSize;
		}
		BoundsAndAdditionalLenghtCalculator CreateBoundsAndAdditionalLenghtCalculator(RefinedSeriesData seriesData, Rectangle funnelBounds, Rectangle labelBounds) {
			FunnelWholeSeriesViewData funnelWholeViewData = seriesData.WholeViewData as FunnelWholeSeriesViewData;
			if (funnelWholeViewData == null)
				return null;
			List<RefinedPointData> filteredPointsData = funnelWholeViewData.FilteredPointsData;
			switch (position) {
				case FunnelSeriesLabelPosition.Right:
					return new RightBoundsAndAdditionalLenghtCalculator(filteredPointsData, funnelBounds, labelBounds, this);
				case FunnelSeriesLabelPosition.Left:
					return new LeftBoundsAndAdditionalLenghtCalculator(filteredPointsData, funnelBounds, labelBounds, this);
				default:
					ChartDebug.Fail("Unexpected label position");
					return null;
			}
		}
		Rectangle CorrectFunnelBoundsForColumnPosition(RefinedSeriesData seriesData, Rectangle funnelBounds, Rectangle labelBounds) {
			Size maxLabelSize = CalculateMaximumLabelSize(seriesData);
			int correctionWidth = labelBounds.Left - funnelBounds.Left + maxLabelSize.Width + LineLength;
			if (correctionWidth > 0) {
				if (!((FunnelSeriesView)View).AlignToCenter) {
					int xOffset = correctionWidth;
					int deltaWidth = MathUtils.StrongRound((labelBounds.Width - funnelBounds.Width) / 2.0);
					if (xOffset > deltaWidth) {
						int dx = (xOffset - deltaWidth) / 2;
						xOffset = deltaWidth + dx;
						funnelBounds.Inflate(-dx, 0);
					}
					if (Position == FunnelSeriesLabelPosition.LeftColumn)
						funnelBounds.Offset(xOffset, 0);
					else
						funnelBounds.Offset(-xOffset, 0);
				}
				else
					funnelBounds.Inflate(-correctionWidth, 0);
			}
			return funnelBounds;
		}
		Rectangle CorrectFunnelBoundsForOutsidePosition(RefinedSeriesData seriesData, Rectangle funnelBounds, Rectangle labelBounds) {
			BoundsAndAdditionalLenghtCalculator calculator = CreateBoundsAndAdditionalLenghtCalculator(seriesData, funnelBounds, labelBounds);
			if (calculator != null)
				return calculator.CalculateBounds();
			return funnelBounds;
		}
		protected override ChartElement CreateObjectForClone() {
			return new FunnelSeriesLabel();
		}
		protected virtual DiagramPoint CalcLabelPositionAndConnectorPoints(FunnelSeriesPointLayout layout, ISimpleDiagramDomain domain,
			RectangleF labelsBounds, int labelWidth, double additionalLenght, out DiagramPoint startPoint, out DiagramPoint finishPoint) {
			switch (position) {
				case FunnelSeriesLabelPosition.Center:
					startPoint = finishPoint = new DiagramPoint((layout.LeftUpPoint.X + layout.RightUpPoint.X) / 2,
						(layout.RightUpPoint.Y + layout.RightDownPoint.Y) / 2);
					return startPoint;
				case FunnelSeriesLabelPosition.RightColumn:
					startPoint = new DiagramPoint(layout.RightUpPoint.X, layout.RightUpPoint.Y);
					finishPoint = new DiagramPoint(labelsBounds.Right - labelWidth, layout.RightUpPoint.Y);
					return finishPoint;
				case FunnelSeriesLabelPosition.LeftColumn:
					startPoint = new DiagramPoint(layout.LeftUpPoint.X, layout.LeftUpPoint.Y);
					finishPoint = new DiagramPoint(labelsBounds.Left + labelWidth, layout.LeftUpPoint.Y);
					return new DiagramPoint(labelsBounds.Left, layout.LeftUpPoint.Y);
				case FunnelSeriesLabelPosition.Left:
					startPoint = new DiagramPoint(layout.LeftUpPoint.X, layout.LeftUpPoint.Y);
					finishPoint = new DiagramPoint(layout.LeftUpPoint.X - LineLength - additionalLenght, layout.LeftUpPoint.Y);
					return new DiagramPoint(finishPoint.X - labelWidth, layout.LeftUpPoint.Y);
				case FunnelSeriesLabelPosition.Right:
					startPoint = new DiagramPoint(layout.RightUpPoint.X, layout.RightUpPoint.Y);
					finishPoint = new DiagramPoint(layout.RightUpPoint.X + LineLength + additionalLenght, layout.RightUpPoint.Y);
					return finishPoint;
				default:
					ChartDebug.Fail("Unknown funnel label position");
					startPoint = finishPoint = DiagramPoint.Zero;
					return finishPoint;
			}
		}
		protected virtual RectangleF CalcCorrectionLabelsBounds(ISimpleDiagramDomain domain) {
			return domain.LabelsBounds;
		}
		internal virtual Size CalculateMaximumSizeConsiderIndent(RefinedSeriesData seriesData) {
			if (!SeriesBase.ActualLabelsVisibility || position == FunnelSeriesLabelPosition.Center)
				return Size.Empty;
			Size maxSize = CalculateMaximumLabelSize(seriesData);
			maxSize.Width += LineLength;
			return maxSize;
		}
		internal Rectangle CorrectFunnelBounds(RefinedSeriesData seriesData, Rectangle funnelBounds, Rectangle labelBounds) {
			if (!SeriesBase.ActualLabelsVisibility || position == FunnelSeriesLabelPosition.Center)
				return funnelBounds;
			if (IsColumnPosition)
				return CorrectFunnelBoundsForColumnPosition(seriesData, funnelBounds, labelBounds);
			if (IsOutsidePosition)
				return CorrectFunnelBoundsForOutsidePosition(seriesData, funnelBounds, labelBounds);
			ChartDebug.Fail("Unexpected label position.");
			return funnelBounds;
		}
		protected internal override void CalculateLayout(SimpleDiagramSeriesLabelLayoutList labelsLayout, SeriesPointLayout pointLayout, TextMeasurer textMeasurer) {
			FunnelSeriesPointLayout funnelLayout = pointLayout as FunnelSeriesPointLayout;
			if (funnelLayout == null) {
				ChartDebug.Fail("FunnelSeriesPointLayout expected.");
				return;
			}
			SimpleDiagramDrawOptionsBase funnelDrawOptions = pointLayout.DrawOptions as SimpleDiagramDrawOptionsBase;
			if (funnelDrawOptions == null) {
				ChartDebug.Fail("SimpleDiagramDrawOptionsBase expected.");
				return;
			}
			if (pointLayout.LabelViewData.Length < 1)
				return;
			SeriesLabelViewData labelViewData = pointLayout.LabelViewData[0];
			ConnectorPainterBase connectorPainter = null;
			TextPainterBase textPainter;
			int labelWidth = labelViewData.TextSize.Width + Border.ActualThickness * 2;
			DiagramPoint connectorStartPoint, connectorFinishPoint;
			RectangleF labelsBounds = CalcCorrectionLabelsBounds(labelsLayout.Domain);
			DiagramPoint labelPosition = CalcLabelPositionAndConnectorPoints(funnelLayout, labelsLayout.Domain, labelsBounds, labelWidth, labelViewData.AdditionalLenght,
				out connectorStartPoint, out connectorFinishPoint);
			if (position == FunnelSeriesLabelPosition.Center)
				textPainter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, labelPosition);
			else {
				textPainter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, labelPosition, 0);
				if (ActualLineVisible)
					connectorPainter = new LineConnectorPainter(connectorStartPoint, connectorFinishPoint, 0, (ZPlaneRectangle)((TextPainter)textPainter).BoundsWithBorder, IsFlatConnector);
			}
			SeriesLabelLayout layout = new SeriesLabelLayout(funnelLayout.PointData, funnelDrawOptions.Color, textPainter, connectorPainter, ResolveOverlappingMode);
			labelsLayout.Add(layout);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			FunnelSeriesLabel label = obj as FunnelSeriesLabel;
			if (label == null)
				return;
			position = label.position;
		}
	}
	[
	TypeConverter(typeof(Funnel3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Funnel3DSeriesLabel : FunnelSeriesLabel {
		static DiagramPoint UnProject(SimpleDiagram3DDomain domain3D, DiagramPoint point) {
			return domain3D.UnProject(new DiagramPoint(point.X, domain3D.Bounds.Height + 2 * domain3D.Bounds.Top - point.Y, point.Z));
		}
		static double CalcCorrectionAngle(SimpleDiagram3DDomain domain3D, DiagramPoint point) {
			DiagramPoint axledPoint = GLHelper.InverseTransform(domain3D.ModelViewMatrix, new DiagramPoint(0, point.Y, 0));
			DiagramPoint surfacePoint = GLHelper.InverseTransform(domain3D.ModelViewMatrix, point);
			return Math.Atan2(axledPoint.X - surfacePoint.X, axledPoint.Z - surfacePoint.Z);
		}
		protected internal override bool ShadowSupported { get { return false; ; } }
		protected override bool IsFlatConnector { get { return false; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Shadow Shadow { get { return base.Shadow; } }
		public Funnel3DSeriesLabel() : base() {
		}
		void CalcConnectorPointsForInsidePosition(FunnelSeriesPointLayout layout, SimpleDiagram3DDomain domain3D,
			out DiagramPoint startPoint, out DiagramPoint finishPoint) {
			double y = domain3D.Height / 2 - (layout.RightUpPoint.Y + layout.RightDownPoint.Y) / 2;
			double radius = (layout.RightUpPoint.X + layout.RightDownPoint.X - layout.LeftUpPoint.X - layout.LeftDownPoint.X) / 4;
			double correctionAngle = CalcCorrectionAngle(domain3D, new DiagramPoint(0, y, -radius));
			startPoint = Funnel3DSeriesView.CalcSurfacePoint(y, radius, correctionAngle);
			double downRadius = (layout.RightDownPoint.X - layout.LeftDownPoint.X) / 2;
			double downY = domain3D.Height / 2 - layout.RightDownPoint.Y;
			DiagramPoint downPoint = Funnel3DSeriesView.CalcSurfacePoint(downY, downRadius, correctionAngle);
			int holeRadiusPercent = ((Funnel3DSeriesView)Series.View).HoleRadiusPercent;
			if (holeRadiusPercent >= 50 && holeRadiusPercent != 100) {
				bool isCorrectStartPoint = false;
				if (domain3D.Project(new DiagramPoint(0, 0, 0)).Y > domain3D.Project(new DiagramPoint(0, 1, 0)).Y) {
					if (startPoint.Y >= downPoint.Y)
						isCorrectStartPoint = true;
				}
				else
					if (startPoint.Y <= downPoint.Y)
						isCorrectStartPoint = true;
				if (isCorrectStartPoint)
					startPoint = Funnel3DSeriesView.CalcSurfacePoint(y, holeRadiusPercent * 0.01 * radius, correctionAngle);
			}
			finishPoint = startPoint;
		}
		void CalcConnectorPointsForOutsidePosition(FunnelSeriesPointLayout layout, SimpleDiagram3DDomain domain3D,
			out DiagramPoint startPoint, out DiagramPoint finishPoint, out DiagramPoint supportingPoint) {
			double radius = (layout.RightUpPoint.X - layout.LeftUpPoint.X) / 2;
			double y = domain3D.Bounds.Height / 2 - layout.RightUpPoint.Y;
			double correctionAngle;
			if (Position == FunnelSeriesLabelPosition.Left || Position == FunnelSeriesLabelPosition.LeftColumn)
				correctionAngle = CalcCorrectionAngle(domain3D, new DiagramPoint(1.0, y, 0));
			else
				correctionAngle = CalcCorrectionAngle(domain3D, new DiagramPoint(-1.0, y, 0));
			startPoint = Funnel3DSeriesView.CalcSurfacePoint(y, radius, correctionAngle);
			finishPoint = Funnel3DSeriesView.CalcSurfacePoint(y, radius + LineLength, correctionAngle);
			supportingPoint = radius + LineLength > domain3D.Depth / 2 ? Funnel3DSeriesView.CalcSurfacePoint(y, domain3D.Depth / 2, correctionAngle) : finishPoint;
		}
		internal override Size CalculateMaximumSizeConsiderIndent(RefinedSeriesData seriesData) {
			Size maxSize = base.CalculateMaximumSizeConsiderIndent(seriesData);
			maxSize.Height = MathUtils.StrongRound(maxSize.Height / 2.0);
			if (SeriesBase.ActualLabelsVisibility && (Position == FunnelSeriesLabelPosition.Left || Position == FunnelSeriesLabelPosition.Right))
				maxSize.Height += LineLength;
			return maxSize;
		}
		protected override DiagramPoint CalcLabelPositionAndConnectorPoints(FunnelSeriesPointLayout layout, ISimpleDiagramDomain domain,
			RectangleF labelsBounds, int labelWidth, double additionalLenght, out DiagramPoint startPoint, out DiagramPoint finishPoint) {
			startPoint = finishPoint = DiagramPoint.Zero;
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (domain3D == null)
				return finishPoint;
			DiagramPoint supportingPoint = DiagramPoint.Zero;
			if (Position == FunnelSeriesLabelPosition.Center)
				CalcConnectorPointsForInsidePosition(layout, domain3D, out startPoint, out finishPoint);
			else
				CalcConnectorPointsForOutsidePosition(layout, domain3D, out startPoint, out finishPoint, out supportingPoint);
			DiagramPoint anchorPoint = domain3D.Project(startPoint);
			switch (Position) {
				case FunnelSeriesLabelPosition.Center:
					return anchorPoint;
				case FunnelSeriesLabelPosition.Left:
					DiagramPoint p = domain3D.Project(finishPoint);
					finishPoint = UnProject(domain3D, new DiagramPoint(p.X, p.Y, domain3D.Project(supportingPoint).Z));
					return new DiagramPoint(p.X - labelWidth, p.Y);
				case FunnelSeriesLabelPosition.LeftColumn:
					finishPoint = UnProject(domain3D, new DiagramPoint(labelsBounds.Left + labelWidth, anchorPoint.Y, anchorPoint.Z));
					return new DiagramPoint(labelsBounds.Left, anchorPoint.Y);
				case FunnelSeriesLabelPosition.Right:
					p = domain3D.Project(finishPoint);
					finishPoint = UnProject(domain3D, new DiagramPoint(p.X, p.Y, domain3D.Project(supportingPoint).Z));
					return p;
				case FunnelSeriesLabelPosition.RightColumn:
					p = anchorPoint;
					p.X = labelsBounds.Right - labelWidth;
					finishPoint = UnProject(domain3D, p);
					return p;
				default:
					ChartDebug.Fail("Unknown funnel label position");
					return DiagramPoint.Zero;
			}
		}
		protected override RectangleF CalcCorrectionLabelsBounds(ISimpleDiagramDomain domain) {
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (domain3D == null)
				return domain.LabelsBounds;
			PointF zeroPoint = (PointF)domain3D.Project(DiagramPoint.Zero);
			return new RectangleF(zeroPoint.X - domain.LabelsBounds.Width / 2.0f,
				zeroPoint.Y - domain.LabelsBounds.Height / 2.0f, domain.LabelsBounds.Width, domain.LabelsBounds.Height);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class BoundsAndAdditionalLenghtCalculator {
		const int indent = 3;
		List<RefinedPointData> filteredPointsData;
		Rectangle labelBounds;
		Rectangle bounds;
		FunnelSeriesLabel label;
		float heightOfPolygon;
		float currentY;
		bool alignToCenter;
		bool isAutoSize;
		int actualPointDistance;
		public BoundsAndAdditionalLenghtCalculator(List<RefinedPointData> filteredPointsData, Rectangle bounds, Rectangle labelBounds, FunnelSeriesLabel label) {
			this.filteredPointsData = filteredPointsData;
			this.labelBounds = labelBounds;
			this.bounds = bounds;
			this.label = label;
			currentY = bounds.Top;
			int residue1, residue2;
			FunnelSeriesView view = (FunnelSeriesView)label.View;
			heightOfPolygon = view.CalculateHeightOfPolygon((int)(bounds.Height), filteredPointsData, out actualPointDistance, out residue1, out residue2);
			actualPointDistance = view.PointDistance;
			alignToCenter = view.AlignToCenter;
			isAutoSize = view.HeightToWidthRatioAuto;
		}
		void CalcAdditionalLenghtForTopPolygon(RefinedPointData seriesPointData, double normalValue, double upNormalValue, float halfOfLabelHeight) {
			if (halfOfLabelHeight <= actualPointDistance)
				return;
			float labelY = currentY - halfOfLabelHeight;
			float labelX = bounds.Width / 2 + GetWidth((float)(bounds.Width * normalValue / 2 + label.LineLength));
			float x11 = bounds.Width / 2 + GetWidth((float)(bounds.Width * normalValue / 2));
			float x2 = bounds.Width / 2 + GetWidth((float)(bounds.Width * upNormalValue / 2));
			float y1 = currentY - actualPointDistance;
			float y2 = y1 - heightOfPolygon;
			float x1 = (y1 - currentY) / (y2 - currentY) * (x2 - x11) + x11;
			float polygonY = (labelX - x1) / (x2 - x1) * (y2 - y1) + y1;
			if (labelY < polygonY && IsLabelXInFunnelPolygon(labelX, x1, x2)) {
				float newLabelX = (labelY - y1) / (y2 - y1) * (x2 - x1) + x1;
				if (!IsLabelXInFunnelPolygon(newLabelX, x1, x2))
					newLabelX = GetNewLabelX(x1, x2);
				if (seriesPointData.LabelViewData[0].AdditionalLenght < Math.Abs(newLabelX - labelX))
					seriesPointData.LabelViewData[0].AdditionalLenght = Math.Abs(newLabelX - labelX);
			}
		}
		void CalcAdditionalLenghtForBottomPolygon(RefinedPointData pointData, double normalValue, double downNormalValue, float halfOfLabelHeight) {
			float labelY = currentY + halfOfLabelHeight;
			float labelX = bounds.Width / 2 + GetWidth((float)(bounds.Width * normalValue / 2 + label.LineLength));
			float x1 = bounds.Width / 2 + GetWidth((float)(bounds.Width * normalValue / 2));
			float y1 = currentY;
			float y2 = currentY + heightOfPolygon;
			float dx = bounds.Width / 2 + GetWidth((float)(bounds.Width * downNormalValue / 2)) - x1;
			float dy = y2 + actualPointDistance - y1;
			float x2 = (y2 - y1) / dy * dx + x1;
			float polygonY = (labelX - x1) / (x2 - x1) * (y2 - y1) + y1;
			if (labelY > polygonY && IsLabelXInFunnelPolygon(labelX, x1, x2)) {
				float newLabelX = (labelY - y1) / (y2 - y1) * (x2 - x1) + x1;
				if (!IsLabelXInFunnelPolygon(newLabelX, x1, x2))
					newLabelX = GetNewLabelX(x1, x2);
				if (pointData.LabelViewData[0].AdditionalLenght < Math.Abs(newLabelX - labelX))
					pointData.LabelViewData[0].AdditionalLenght = Math.Abs(newLabelX - labelX);
			}
		}
		void CorrectAdditionalLength(RectangleF correctBounds, RectangleF bounds) {
			for (int i = 0; i < filteredPointsData.Count; i++) {
				IFunnelPoint funnelPoint = filteredPointsData[i].RefinedPoint;
				if (funnelPoint != null) {
					SeriesLabelViewData labelViewData = filteredPointsData[i].LabelViewData[0];
					if (labelViewData.AdditionalLenght != 0 && correctBounds.Width * 0.5 * funnelPoint.NormalizedValue + label.LineLength + labelViewData.AdditionalLenght > correctBounds.Width * 0.5) {
						double length = correctBounds.Width * 0.5 - correctBounds.Width * 0.5 * funnelPoint.NormalizedValue - label.LineLength;
						if (length > 0)
							labelViewData.AdditionalLenght = MathUtils.StrongRound(length);
						else
							labelViewData.AdditionalLenght = 0;
					}
				}
			}
		}
		int CalculateAdditionalLength() {
			float currentWidth = 0;
			float width = 0;
			IFunnelPoint previousFunnelPoint = null;
			for (int i = 0; i < filteredPointsData.Count; i++) {
				IFunnelPoint funnelPoint = filteredPointsData[i].RefinedPoint;
				if (funnelPoint != null) {
					SeriesLabelViewData labelViewData = filteredPointsData[i].LabelViewData[0];
					float halfOfLabelHeight = labelViewData.TextSize.Height / 2 + label.Border.ActualThickness + indent;
					if (previousFunnelPoint != null)
						CalcAdditionalLenghtForTopPolygon(filteredPointsData[i], funnelPoint.NormalizedValue, previousFunnelPoint.NormalizedValue, halfOfLabelHeight);
					if (i < filteredPointsData.Count - 1) {
						IFunnelPoint nextFunnelPoint = filteredPointsData[i + 1].RefinedPoint;
						if (nextFunnelPoint != null)
							CalcAdditionalLenghtForBottomPolygon(filteredPointsData[i], funnelPoint.NormalizedValue, nextFunnelPoint.NormalizedValue, halfOfLabelHeight);
					}
					currentY += (heightOfPolygon + actualPointDistance);
					if (isAutoSize) {
						double labelWidth = labelViewData.TextSize.Width + label.Border.ActualThickness * 2 + label.LineLength;
						if ((funnelPoint.NormalizedValue * bounds.Width + 2 * (labelWidth + labelViewData.AdditionalLenght)) > bounds.Width) {
							if (alignToCenter)
								currentWidth = (float)((bounds.Width - 2 * labelWidth) / (funnelPoint.NormalizedValue + 2 * labelViewData.AdditionalLenght / bounds.Width));
							else
								currentWidth = (float)((bounds.Width - labelWidth) / ((1 + funnelPoint.NormalizedValue) / 2 + labelViewData.AdditionalLenght / bounds.Width));
							if (currentWidth == 0)
								return 0;
							if (currentWidth < width || width == 0)
								width = currentWidth;
						}
					}
					previousFunnelPoint = funnelPoint;
				}
			}
			return (int)width;
		}
		double CorrectInflation(double dx, SeriesLabelViewData labelViewData, double normalValue) {
			double inflation = dx;
			double newWidth = bounds.Width - 2 * dx;
			if (labelViewData.AdditionalLenght != 0 && (newWidth / 2 - newWidth * normalValue / 2) < (labelViewData.AdditionalLenght + label.LineLength)) {
				double labelWidth = labelViewData.TextSize.Width + label.Border.ActualThickness * 2;
				double width2 = alignToCenter ? labelBounds.Width - 2 * labelWidth : labelBounds.Width - labelWidth;
				if (width2 / 2 * normalValue + label.LineLength < width2 / 2)
					inflation = (bounds.Width - width2) / 2;
				else {
					double factor = alignToCenter ? normalValue : 1 + normalValue;
					inflation = ((normalValue * bounds.Width / 2 + labelWidth + label.LineLength) - labelBounds.Width / 2) / factor;
				}
				return inflation;
			}
			return inflation;
		}
		void CalculateParams(out int deltaX, out int offset) {
			double deltaXDouble = 0;
			double offsetDouble = 0;
			double maxCorrection = 0;
			double maxOffset = (labelBounds.Width - bounds.Width) * 0.5;
			for (int i = 0; i < filteredPointsData.Count; i++) {
				IFunnelPoint funnelPoint = filteredPointsData[i].RefinedPoint;
				if (funnelPoint != null) {
					SeriesLabelViewData labelViewData = filteredPointsData[i].LabelViewData[0];
					double labelWidth = labelViewData.TextSize.Width + label.Border.ActualThickness * 2 + label.LineLength;
					double dx = (funnelPoint.NormalizedValue * bounds.Width / 2 + labelWidth + labelViewData.AdditionalLenght) - labelBounds.Width / 2;
					if (!alignToCenter) {
						double currentOffset = 0;
						double currentDx = 0;
						if (dx < maxOffset)
							currentOffset = dx;
						else {
							currentDx = (dx - maxOffset) / (1 + funnelPoint.NormalizedValue);
							currentDx = CorrectInflation(currentDx, labelViewData, funnelPoint.NormalizedValue);
							currentOffset = maxOffset + currentDx;
						}
						if (currentOffset + currentDx > maxCorrection) {
							maxCorrection = currentOffset + currentDx;
							deltaXDouble = currentDx;
							offsetDouble = currentOffset;
						}
					}
					else {
						if (funnelPoint.NormalizedValue > 0) {
							dx /= funnelPoint.NormalizedValue;
							dx = CorrectInflation(dx, labelViewData, funnelPoint.NormalizedValue);
							if (dx > deltaXDouble)
								deltaXDouble = dx;
						}
					}
				}
			}
			deltaX = (int)deltaXDouble;
			offset = (int)offsetDouble;
		}
		protected abstract float GetWidth(float width);
		protected abstract bool IsLabelXInFunnelPolygon(float labelX, float x1, float x2);
		protected abstract float GetNewLabelX(float x1, float x2);
		protected abstract int GetOffset(int offset);
		public Rectangle CalculateBounds() {
			if (heightOfPolygon == 0)
				return Rectangle.Empty;
			if (isAutoSize) {
				int width = CalculateAdditionalLength();
				foreach (RefinedPointData pointData in filteredPointsData) {
					if (pointData.LabelViewData[0].AdditionalLenght != 0)
						pointData.LabelViewData[0].AdditionalLenght = pointData.LabelViewData[0].AdditionalLenght * width / bounds.Width;
				}
				int offset = bounds.Width - width;
				if (alignToCenter) {
					bounds.Offset(offset / 2, 0);
					bounds.Width = width;
				}
				else {
					if (label.Position == FunnelSeriesLabelPosition.Left)
						bounds.Offset(offset, 0);
					bounds.Width = width;
				}
				return bounds;
			}
			else {
				CalculateAdditionalLength();
				int deltaX, offset;
				CalculateParams(out deltaX, out offset);
				RectangleF initialBounds = bounds;
				bounds.Offset(GetOffset(offset), 0);
				bounds.Inflate(-deltaX, 0);
				CorrectAdditionalLength(bounds, initialBounds);
				return bounds;
			}
		}
	}
	public class RightBoundsAndAdditionalLenghtCalculator : BoundsAndAdditionalLenghtCalculator {
		public RightBoundsAndAdditionalLenghtCalculator(List<RefinedPointData> filteredPointsData, Rectangle bounds, Rectangle labelBounds, FunnelSeriesLabel label)
			: base(filteredPointsData, bounds, labelBounds, label) { }
		protected override float GetWidth(float width) {
			return width;
		}
		protected override bool IsLabelXInFunnelPolygon(float labelX, float x1, float x2) {
			return labelX < x1 || labelX < x2;
		}
		protected override float GetNewLabelX(float x1, float x2) {
			return Math.Max(x1, x2);
		}
		protected override int GetOffset(int offset) {
			return -offset;
		}
	}
	public class LeftBoundsAndAdditionalLenghtCalculator : BoundsAndAdditionalLenghtCalculator {
		public LeftBoundsAndAdditionalLenghtCalculator(List<RefinedPointData> filteredPointsData, Rectangle bounds, Rectangle labelBounds, FunnelSeriesLabel label)
			: base(filteredPointsData, bounds, labelBounds, label) { }
		protected override float GetWidth(float width) {
			return -width;
		}
		protected override bool IsLabelXInFunnelPolygon(float labelX, float x1, float x2) {
			return labelX > x1 || labelX > x2;
		}
		protected override float GetNewLabelX(float x1, float x2) {
			return Math.Min(x1, x2);
		}
		protected override int GetOffset(int offset) {
			return offset;
		}
	}
}
