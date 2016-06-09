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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum PieSeriesLabelPosition {
		Inside,
		Outside,
		TwoColumns,
		Radial,
		Tangent
	}
	[
	TypeConverter(typeof(PieSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PieSeriesLabel : SeriesLabelBase {
		static float CalcRadialLabelAngle(double lineAngle) {
			return (float)(Math.Round(-lineAngle * 180.0 / Math.PI + 90.0));
		}
		static float CalculateCorrection(float size, float maxSize) {
			float correction = size - maxSize / 2.0f;
			return correction > 0.0f ? correction : 0.0f;
		}
		static float ApplyCorrection(float value, float factor) {
			return value * factor + value * (1.0f - factor) / 2.0f;
		}
		internal static bool IsTwoColumnsLabelAlignedToRight(double lineAngle) {
			return Math.Cos(lineAngle) >= 0;
		}
		const int DefaultColumnIndent = 0;
		PieSeriesLabelPosition position;
		int columnIndent = DefaultColumnIndent;
		protected PieSeriesViewBase View {
			get {
				PieSeriesViewBase view = Series.View as PieSeriesViewBase;
				ChartDebug.Assert(view != null, "view can't be null");
				return view;
			}
		}
		protected bool IsInside { get { return IsInsidePosition(ActualPosition); } }
		protected virtual PieSeriesLabelPosition DefaultPosition { get { return PieSeriesLabelPosition.Outside; } }
		protected override bool Rotated { get { return ActualPosition == PieSeriesLabelPosition.Radial || ActualPosition == PieSeriesLabelPosition.Tangent; } }
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return !IsInside; } }
		protected internal override bool ResolveOverlappingEnabled { get { return Position == PieSeriesLabelPosition.TwoColumns || Position == PieSeriesLabelPosition.Outside; } }
		protected internal override bool VerticalRotationSupported { get { return false; } }
		internal PieSeriesLabelPosition ActualPosition { get { return IsPositionSupported(position) ? position : DefaultPosition; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesLabel.Position"),
		TypeConverter(typeof(PieSeriesLabelPositionTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public PieSeriesLabelPosition Position {
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
#if !SL
	DevExpressXtraChartsLocalizedDescription("PieSeriesLabelColumnIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PieSeriesLabel.ColumnIndent"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int ColumnIndent {
			get { return columnIndent; }
			set {
				if (value != columnIndent) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPieSeriesLabelColumnIndent));
					SendNotification(new ElementWillChangeNotification(this));
					columnIndent = value;
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
		public PieSeriesLabel()
			: base() {
			position = DefaultPosition;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Position":
					return ShouldSerializePosition();
				case "ColumnIndent":
					return ShouldSerializeColumnIndent();
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
		bool ShouldSerializeColumnIndent() {
			return columnIndent != DefaultColumnIndent;
		}
		void ResetColumnIndent() {
			ColumnIndent = DefaultColumnIndent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePosition() || ShouldSerializeColumnIndent();
		}
		#endregion
		Size CalculateMaximumSizeForInternalLabels(RefinedSeriesData seriesData, Size pieSize) {
			float factor = (float)(1.0 - View.CalculateActualHolePercent(seriesData.RefinedSeries, new SizeF(pieSize.Width, pieSize.Height)) / 100.0);
			SizeF actualBoundsForCompare = new SizeF(pieSize.Width * factor, pieSize.Height * factor);
			float widthCorrection, heightCorrection;
			Size maxSize = CalculateMaximumLabelSize(seriesData);
			if (ActualPosition == PieSeriesLabelPosition.Inside) {
				widthCorrection = CalculateCorrection(maxSize.Width, actualBoundsForCompare.Width);
				heightCorrection = CalculateCorrection(maxSize.Height, actualBoundsForCompare.Height);
			}
			else {
				int maxDimension = Math.Max(maxSize.Width, maxSize.Height);
				widthCorrection = CalculateCorrection(maxDimension, actualBoundsForCompare.Width);
				heightCorrection = CalculateCorrection(maxDimension, actualBoundsForCompare.Height);
			}
			return new Size(MathUtils.StrongRound(ApplyCorrection(widthCorrection, factor)),
							MathUtils.StrongRound(ApplyCorrection(heightCorrection, factor)));
		}
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
		protected bool IsInsidePosition(PieSeriesLabelPosition labelPosition) {
			return labelPosition == PieSeriesLabelPosition.Inside || labelPosition == PieSeriesLabelPosition.Radial || labelPosition == PieSeriesLabelPosition.Tangent;
		}
		protected override ChartElement CreateObjectForClone() {
			return new PieSeriesLabel();
		}
		protected virtual GRealPoint2D CalculateAnchorPointAndAngles(ISimpleDiagramDomain domain, PieSeriesPointLayout pieLayout, ref RectangleF labelsBounds, out double lineAngle, out double crossAngle) {
			lineAngle = pieLayout.Pie.HalfAngle;
			crossAngle = lineAngle + Math.PI / 2.0;
			RectangleF pieBounds = pieLayout.PieBounds;
			if (!pieBounds.AreWidthAndHeightPositive())
				return new GRealPoint2D();
			Ellipse actualEllipse = new Ellipse(pieLayout.Pie.CalculateCenter(pieLayout.BasePoint), pieBounds.Width / 2, pieBounds.Height / 2);
			GRealPoint2D finishPoint = actualEllipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
			if (!IsInside)
				return finishPoint;
			return pieLayout.PositiveValuesCount > 1 ?
				new GRealPoint2D((actualEllipse.Center.X + finishPoint.X) / 2.0, (actualEllipse.Center.Y + finishPoint.Y) / 2.0) :
				actualEllipse.Center;
		}
		protected internal Size CalculateMaximumSize(RefinedSeriesData seriesData, Size pieSize) {
			if (!SeriesBase.ActualLabelsVisibility)
				return Size.Empty;
			if (IsInside)
				return CalculateMaximumSizeForInternalLabels(seriesData, pieSize);
			Size maxSize = CalculateMaximumLabelSize(seriesData);
			if (ActualPosition == PieSeriesLabelPosition.TwoColumns) {
				maxSize.Height = (int)MathUtils.StrongRound(maxSize.Height / 2.0);
				maxSize.Width += columnIndent;
			}
			maxSize.Width += LineLength;
			maxSize.Height += LineLength;
			return maxSize;
		}
		protected internal override void CalculateLayout(SimpleDiagramSeriesLabelLayoutList labelsLayout, SeriesPointLayout pointLayout, TextMeasurer textMeasurer) {
			PieSeriesPointLayout pieLayout = pointLayout as PieSeriesPointLayout;
			if (pieLayout == null)
				return;
			RefinedPointData pointData = pieLayout.PointData;
			SimpleDiagramDrawOptionsBase pieDrawOptions = pointData.DrawOptions as SimpleDiagramDrawOptionsBase;
			IPiePoint piePoint = pointData.RefinedPoint;
			if (pieDrawOptions == null || piePoint == null || piePoint.NormalizedValue == 0)
				return;
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			ISimpleDiagramDomain domain = labelsLayout.Domain;
			Size textSize = labelViewData.TextSize;
			RectangleF labelsBounds = domain.LabelsBounds;
			double lineAngle, crossAngle;
			GRealPoint2D anchorPoint = CalculateAnchorPointAndAngles(domain, pieLayout, ref labelsBounds, out lineAngle, out crossAngle);
			if ((anchorPoint.X == 0 && anchorPoint.Y == 0) ||
				(!IsInside && Double.IsNaN(lineAngle)) ||
			   double.IsNaN(anchorPoint.X) ||
			   double.IsNaN(anchorPoint.Y))
				return;
			Point point = new Point((int)Math.Round(anchorPoint.X), (int)Math.Round(anchorPoint.Y));
			TextPainterBase textPainter;
			ConnectorPainterBase connectorPainter = null;
			bool actualResolveOverlapping = ResolveOverlappingMode != ResolveOverlappingMode.None && ResolveOverlappingSupported;
			switch (ActualPosition) {
				case PieSeriesLabelPosition.Radial:
					textPainter = new RotatedTextPainterOnCircleRadial(CalcRadialLabelAngle(lineAngle),
						point, labelViewData.Text, textSize, this, false, true);
					break;
				case PieSeriesLabelPosition.Tangent:
					textPainter = new RotatedTextPainterOnCircleTangent(CalcRadialLabelAngle(crossAngle - Math.PI / 2.0),
						point, labelViewData.Text, textSize, this, false, true);
					break;
				case PieSeriesLabelPosition.Inside:
					textPainter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, new DiagramPoint(anchorPoint.X, anchorPoint.Y));
					break;
				default:
					DiagramPoint startConnectorPoint = new DiagramPoint(anchorPoint.X, anchorPoint.Y);
					DiagramPoint finishConnectorPoint = new DiagramPoint(anchorPoint.X + (float)(Math.Cos(lineAngle) * LineLength),
																anchorPoint.Y - (float)(Math.Sin(lineAngle) * LineLength));
					if (ActualPosition == PieSeriesLabelPosition.Outside) {
						textPainter = labelViewData.CreateTextPainterAccordingAllowedBoundsForLabels(this, textMeasurer, finishConnectorPoint, lineAngle, labelsLayout.Domain.Bounds);
						if (ActualLineVisible && textPainter != null)
							if (actualResolveOverlapping)
								connectorPainter = new OriginBaseLineConnectorPainter(((ZPlaneRectangle)domain.ElementBounds).Center,
									startConnectorPoint, finishConnectorPoint, lineAngle, (ZPlaneRectangle)((TextPainter)textPainter).BoundsWithBorder, true);
							else
								if (textPainter != null)
									connectorPainter = new LineConnectorPainter(startConnectorPoint, finishConnectorPoint,
										lineAngle, (ZPlaneRectangle)((TextPainter)textPainter).BoundsWithBorder, true);
					}
					else {
						DiagramPoint connectorFinishPoint = new DiagramPoint();
						if (IsTwoColumnsLabelAlignedToRight(lineAngle)) {
							textPainter = labelViewData.CreateTextPainterAccordingAllowedBoundsForLabels(this, textMeasurer, finishConnectorPoint, 0.0, labelsLayout.Domain.Bounds);
							if (textPainter != null) {
								ZPlaneRectangle bounds = (ZPlaneRectangle)((TextPainter)textPainter).BoundsWithBorder;
								double dx = labelsBounds.Right - bounds.Right;
								connectorFinishPoint = new DiagramPoint(bounds.Left + dx, finishConnectorPoint.Y);
								textPainter.Offset(dx, 0);
							}
						}
						else {
							textPainter = labelViewData.CreateTextPainterAccordingAllowedBoundsForLabels(this, textMeasurer, finishConnectorPoint, Math.PI, labelsLayout.Domain.Bounds);
							if (textPainter != null) {
								ZPlaneRectangle bounds = (ZPlaneRectangle)((TextPainter)textPainter).BoundsWithBorder;
								double dx = labelsBounds.Left - bounds.Left;
								connectorFinishPoint = new DiagramPoint(bounds.Right + dx, finishConnectorPoint.Y);
								textPainter.Offset(dx, 0);
							}
						}
						if (ActualLineVisible && textPainter != null)
							connectorPainter = new BrokenLineConnectorPainter(startConnectorPoint, finishConnectorPoint, connectorFinishPoint, lineAngle);
					}
					break;
			}
			if (textPainter != null) {
				PieSeriesLabelLayout layout = new PieSeriesLabelLayout(pointData, pieDrawOptions.Color, textPainter, connectorPainter, ResolveOverlappingMode, -lineAngle);
				labelsLayout.Add(layout);
			}
		}
		protected internal virtual bool IsPositionSupported(PieSeriesLabelPosition labelPosition) {
			return true;
		}
		protected internal override bool CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			return mode == ResolveOverlappingMode.None || mode == ResolveOverlappingMode.Default;
		}
		protected internal override ResolveOverlappingMode GetResolveOverlappingMode(Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList) {
			return Position == PieSeriesLabelPosition.TwoColumns ? ResolveOverlappingMode.Default : ResolveOverlappingMode.None;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PieSeriesLabel label = obj as PieSeriesLabel;
			if (label != null) {
				position = label.position;
				columnIndent = label.columnIndent;
			}
		}
	}
	[
	TypeConverter(typeof(Pie3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Pie3DSeriesLabel : PieSeriesLabel {
		protected internal override bool ShadowSupported { get { return false; } }
		protected internal override bool ResolveOverlappingEnabled { get { return Position == PieSeriesLabelPosition.TwoColumns; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Pie3DSeriesLabelShadow"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Shadow Shadow { get { return base.Shadow; } }
		public Pie3DSeriesLabel()
			: base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new Pie3DSeriesLabel();
		}
		protected override GRealPoint2D CalculateAnchorPointAndAngles(ISimpleDiagramDomain domain, PieSeriesPointLayout pieLayout, ref RectangleF labelsBounds, out double lineAngle, out double crossAngle) {
			lineAngle = 0.0;
			crossAngle = 0.0;
			float inflateSize = -pieLayout.Pie.MajorSemiaxis * Math.Min(PieGraphicsCommand.FacetPercent, 1.0f - pieLayout.Pie.HoleFraction);
			RectangleF rect = GraphicUtils.InflateRect(pieLayout.PieBounds, inflateSize, inflateSize);
			if (!rect.AreWidthAndHeightPositive())
				return new GRealPoint2D();
			Ellipse realEllipse = new Ellipse(pieLayout.Pie.CalculateCenter(pieLayout.BasePoint), rect.Width / 2, rect.Height / 2);
			GRealPoint2D finishPoint = realEllipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
			DiagramPoint labelPoint = new DiagramPoint(finishPoint.X - realEllipse.Center.X, realEllipse.Center.Y - finishPoint.Y);
			if (IsInside)
				if (pieLayout.PositiveValuesCount > 1) {
					labelPoint.X /= 2.0;
					labelPoint.Y /= 2.0;
				}
				else {
					labelPoint.X = 0.0;
					labelPoint.Y = 0.0;
				}
			labelPoint.X += (realEllipse.Center.X - pieLayout.BasePoint.X);
			labelPoint.Y -= (realEllipse.Center.Y - pieLayout.BasePoint.Y);
			return Project(domain, labelPoint, pieLayout.Pie.MajorSemiaxis, ref labelsBounds, out lineAngle, out crossAngle);
		}
		protected GRealPoint2D Project(ISimpleDiagramDomain domain, DiagramPoint labelPoint, float majorSemiaxis, ref RectangleF labelsBounds, out double lineAngle, out double crossAngle) {
			lineAngle = 0.0;
			crossAngle = 0.0;
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (domain3D == null)
				return new GRealPoint2D();
			DiagramPoint p1 = domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y));
			DiagramPoint p2 = domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y, 1.0));
			double depth = majorSemiaxis * View.DepthPercent * 0.02;
			double z = p2.Z < p1.Z ? depth * 0.5 : -depth * 0.5;
			DiagramPoint anchorPoint = domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y, z));
			PointF crossPoint = (PointF)domain3D.Project(new DiagramPoint(labelPoint.Y, -labelPoint.X, z));
			PointF zeroPoint = (PointF)domain3D.Project(new DiagramPoint(0.0, 0.0, z));
			lineAngle = Math.Atan2((double)(zeroPoint.Y - anchorPoint.Y), (double)(anchorPoint.X - zeroPoint.X));
			crossAngle = Math.Atan2((double)(zeroPoint.Y - crossPoint.Y), (double)(crossPoint.X - zeroPoint.X));
			zeroPoint = (PointF)domain3D.Project(DiagramPoint.Zero);
			labelsBounds = new RectangleF(zeroPoint.X - labelsBounds.Width / 2.0f,
				zeroPoint.Y - labelsBounds.Height / 2.0f, labelsBounds.Width, labelsBounds.Height);
			return new GRealPoint2D(anchorPoint.X, anchorPoint.Y);
		}
	}
}
