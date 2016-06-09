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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class BarSeries2DBase : XYSeries2D, IBarSeriesView {
		public static readonly DependencyProperty BarWidthProperty = DependencyPropertyManager.Register("BarWidth", typeof(double),
			typeof(BarSeries2DBase), new PropertyMetadata(0.6, BarWidthPropertyChanged), new ValidateValueCallback(BarWidthValidation));
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Bar2DAnimationBase), typeof(BarSeries2DBase), new PropertyMetadata(null, PointAnimationPropertyChanged));
		static bool BarWidthValidation(object barWidth) {
			return (double)barWidth > 0;
		}
		static void BarWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "SideBySideProperties"));
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double BarWidth {
			get { return (double)GetValue(BarWidthProperty); }
			set { SetValue(BarWidthProperty, value); }
		}
		[
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Bar2DAnimationBase PointAnimation {
			get { return (Bar2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		const double minBarPixelWidth = 3.0;
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram2D); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Hatch; } }
		protected internal override bool ArePointsVisible { get { return true; } }
		protected internal override bool LabelsResolveOverlappingSupported { get { return true; } }
		protected internal override ResolveOverlappingMode LabelsResolveOverlappingMode {
			get {
				ResolveOverlappingMode mode = ActualLabel.ResolveOverlappingMode;
				return (mode == ResolveOverlappingMode.JustifyAllAroundPoint || mode == ResolveOverlappingMode.JustifyAroundPoint) ? ResolveOverlappingMode.Default : mode;
			}
		}
		protected virtual bool ShouldAnimateClipBounds { get { return false; } }
		protected override int PixelsPerArgument { get { return 40; } }
		double GetArgument(RefinedPoint refinedPoint) {
			return refinedPoint.Argument + GetDisplayOffset(refinedPoint);
		}
		bool CheckValuesByRange(IMinMaxValues values) {
			return !Double.IsNaN(values.Max) && !Double.IsNaN(values.Min);
		}
		Bar2D CalculateBar(PaneMapping mapping, RefinedPoint refinedPoint, double lowValue, double highValue) {
			if (double.IsNaN(lowValue))
				lowValue = 0;
			if (double.IsNaN(highValue))
				highValue = 0;
			double argument = GetArgument(refinedPoint);
			double halfBarWidth = GetBarWidth(refinedPoint) / 2;
			double fixedOffset = GetFixedOffset(refinedPoint);
			if (IsAxisXReversed)
				fixedOffset = -fixedOffset;
			IAxisMapping argumentMapping = mapping.AxisXMapping;
			IAxisMapping valueMapping = mapping.AxisYMapping;
			double left = argumentMapping.GetRoundedAxisValue(argument - halfBarWidth) + fixedOffset;
			double right = argumentMapping.GetRoundedAxisValue(argument + halfBarWidth) + fixedOffset;
			double top = valueMapping.GetRoundedAxisValue(highValue);
			double bottom = valueMapping.GetRoundedAxisValue(lowValue);
			Render2DHelper.CorrectBounds(ref left, ref right);
			Render2DHelper.CorrectBounds(ref top, ref bottom);
			if (top == bottom)
				CorrectZeroHeight(ref top, ref bottom);
			Render2DHelper.CorrectBoundsByMinDistance(ref left, ref right, minBarPixelWidth);
			return new Bar2D(argument, lowValue, highValue, fixedOffset, left, right, top, bottom);
		}
		protected Bar2D? CalculateBar(PaneMapping mapping, RefinedPoint refinedPoint, bool isLabelInCenter) {
			IMinMaxValues values = new MinMaxValues(GetLowValue(refinedPoint), GetHighValue(refinedPoint));
			if (isLabelInCenter) {
				values = MathUtils.CorrectMinMaxByRange(((IAxisData)ActualAxisY).VisualRange, values);
				if (!CheckValuesByRange(values))
					return null;
			}
			return CalculateBar(mapping, refinedPoint, values.Min, values.Max);
		}
		protected virtual double GetLowValue(RefinedPoint refinedPoint) {
			return 0;
		}
		protected virtual double GetHighValue(RefinedPoint refinedPoint) {
			return ((ISideBySidePoint)refinedPoint).Value;
		}
		protected virtual void CorrectZeroHeight(ref double top, ref double bottom) {
		}
		protected Rect CalculateSeriesPointBounds(PaneMapping mapping, RefinedPoint refinedPoint, double lowValue, double highValue) {
			return CalculateBar(mapping, refinedPoint, lowValue, highValue).Bounds;
		}
		protected Point GetLabelAnchorPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool isLabelInCenter, Bar2D bar) {
			RefinedPoint refinedPoint = pointItem.RefinedPoint;
			double x = GetDisplayOffset(refinedPoint) == 0 ? mapping.AxisXMapping.GetRoundedAxisValue(GetArgument(refinedPoint)) + GetFixedOffset(refinedPoint) : (bar.Left + bar.Right) / 2;
			if (isLabelInCenter) {
				double minY = Math.Min(bar.Top, bar.Bottom);
				double maxY = Math.Max(bar.Top, bar.Bottom);
				double centerY = minY + (maxY - minY) / 2;
				Point centerPoint = new Point(x, centerY);
				return centerPoint;
			}
			else {
				double outsideY = GetPointValue(pointItem.ValueLevel, bar);
				bool isDownwardBar = IsDownwardBar(refinedPoint);
				Point anchorPoint = new Point(x, outsideY);
				if (pointItem.Model != null)
					anchorPoint = pointItem.Model.CorrectOutsideLabelLocation(anchorPoint, isDownwardBar);
				anchorPoint = transform.Transform(anchorPoint);
				return anchorPoint;
			}
		}
		protected abstract double GetBarWidth(RefinedPoint refinedPoint);
		protected abstract int GetFixedOffset(RefinedPoint refinedPoint);
		protected abstract double GetDisplayOffset(RefinedPoint refinedPoint);
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BarSeries2DBase barSeries2D = series as BarSeries2DBase;
			if (barSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, barSeries2D, BarWidthProperty);
				if (CopyPropertyValueHelper.IsValueSet(barSeries2D, PointAnimationProperty) && CopyPropertyValueHelper.VerifyValues(this, barSeries2D, PointAnimationProperty))
					PointAnimation = barSeries2D.PointAnimation.CloneAnimation() as Bar2DAnimationBase;
			}
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Bar2DGrowUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DDropInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DBounceAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DSlideFromLeftAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DSlideFromRightAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DSlideFromTopAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DSlideFromBottomAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DWidenAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Bar2DFadeInAnimation)));
		}
		protected virtual Rect CalculateSeriesPointBounds(PaneMapping mapping, RefinedPoint refinedPoint) {
			return CalculateSeriesPointBounds(mapping, refinedPoint, GetLowValue(refinedPoint), GetHighValue(refinedPoint));
		}
		protected virtual Rect? CalculateSeriesPointClipBounds(PaneMapping mapping, SeriesPointItem pointItem, Rect viewport, Rect bounds) {
			if (pointItem.Model != null) {
				XYDiagram2D xyDiagram2D = Diagram as XYDiagram2D;
				if (xyDiagram2D != null && xyDiagram2D.IsNavigationEnabled) {
					Point clipBoundsLeftTop = new Point(viewport.Left, viewport.Top);
					Point clipBoundsRightBottom = xyDiagram2D.Rotated ? new Point(viewport.Bottom, viewport.Right) : new Point(viewport.Right, viewport.Bottom);
					return new Rect(clipBoundsLeftTop, clipBoundsRightBottom);
				}
				Rect clipBounds = pointItem.Model.CalculateCorrectedBounds(viewport, bounds, xyDiagram2D.Rotated);
				if (!clipBounds.IsEmpty)
					return clipBounds;
			}
			return null;
		}
		protected virtual bool IsDownwardBar(RefinedPoint refinedPoint) {
			ISideBySidePoint barPoint = refinedPoint;
			return IsAxisYReversed ? barPoint.Value > 0 : barPoint.Value < 0;
		}
		protected virtual double GetLabelAngle(RangeValueLevel valueLevel) {
			return -Math.PI / 2;
		}
		protected virtual double GetPointValue(RangeValueLevel valueLevel, Bar2D bar) {
			return bar.Top;
		}
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return CreatePointItemLayout(mapping, pointItem);
		}
		protected internal XYSeriesLabel2DLayout CalculateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform, bool isLabelInCenter) {
			Bar2D? bar = CalculateBar(mapping, labelItem.PointItem.RefinedPoint, isLabelInCenter);
			if (!bar.HasValue)
				return null;
			Point anchorPoint = GetLabelAnchorPoint(labelItem.PointItem, mapping, transform, isLabelInCenter, bar.Value);
			if (isLabelInCenter) {
				Point anchorPointWithoutModelCorrection = transform.Transform(anchorPoint);
				if (labelItem.PointItem.Model != null)
					anchorPoint = labelItem.PointItem.Model.CorrectCenterLabelLocation(anchorPoint);
				anchorPoint = transform.Transform(anchorPoint);
				GRect2D validRectangle = GraphicsUtils.ConvertRect(transform.TransformBounds(bar.Value.Bounds));
				if (validRectangle.IsEmpty) {
					int dx = validRectangle.Width > 0 ? 0 : 1;
					int dy = validRectangle.Height > 0 ? 0 : 1;
					validRectangle = validRectangle.Inflate(new Thickness(0, 0, dx, dy));
				}
				return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, validRectangle, anchorPointWithoutModelCorrection);
			}
			else {
				double angle = GetLabelAngle(labelItem.PointItem.ValueLevel);
				bool isDownwardBar = IsDownwardBar(labelItem.RefinedPoint);
				if (isDownwardBar)
					angle = -angle;
				if (mapping.Rotated)
					angle += Math.PI / 2;
				return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, labelItem.Label.Indent, angle, GRect2D.Empty);
			}
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Bar2DGrowUpAnimation();
		}
		protected internal virtual bool IsNegativeBar(SeriesPointItem pointItem) {
			return ((IValuePoint)pointItem.RefinedPoint).Value < 0;
		}
		protected internal virtual bool ShouldFlipNegativeBar(SeriesPointItem pointItem) {
			return false;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			BarSeries2DPointLayout layout = pointItem.Layout as BarSeries2DPointLayout;
			if (layout == null || pointItem.RefinedPoint == null)
				return;
			bool shouldFlipBar = ShouldFlipNegativeBar(pointItem) && IsDownwardBar(pointItem.RefinedPoint);
			Rect barBounds = layout.InitialBounds;
			Rect? clipBounds = layout.InitialClipBounds;
			Bar2DAnimationBase animation = GetActualPointAnimation() as Bar2DAnimationBase;
			if (animation != null) {
				double progress = pointItem.PointProgress.ActualProgress;
				if (ShouldAnimateClipBounds && clipBounds.HasValue) {
					Rect initailClipBounds = clipBounds.Value;
					Rect newClipBounds = animation.CreateAnimatedBarBounds(initailClipBounds, layout.Viewport, ((IValuePoint)pointItem.RefinedPoint).Value < 0, IsAxisXReversed, IsAxisYReversed, ((XYDiagram2D)Diagram).Rotated, progress);
					double newBarBoundsWidth = initailClipBounds.Width == 0 ? 0 : barBounds.Width * newClipBounds.Width / initailClipBounds.Width;
					double newBarBoundsHeight = initailClipBounds.Height == 0 ? 0 : barBounds.Height * newClipBounds.Height / initailClipBounds.Height;
					double deltaX = initailClipBounds.X - barBounds.X;
					double x = barBounds.Width == 0 ? 0 : newClipBounds.X - (deltaX * newBarBoundsWidth / barBounds.Width);
					double deltaY = initailClipBounds.Y - barBounds.Y;
					double y = barBounds.Height == 0 ? 0 : newClipBounds.Y - (deltaY * newBarBoundsHeight / barBounds.Height);
					barBounds = new Rect(x, y, newBarBoundsWidth, newBarBoundsHeight);
					clipBounds = newClipBounds;
				}
				else 
					barBounds = animation.CreateAnimatedBarBounds(barBounds, layout.Viewport, IsNegativeBar(pointItem), IsAxisXReversed, IsAxisYReversed, ((XYDiagram2D)Diagram).Rotated, progress);
			}
			Transform seriesPointTransform = null;
			if (shouldFlipBar)
				seriesPointTransform = new ScaleTransform() { CenterY = barBounds.Height / 2, ScaleY = -1 };
			else
				seriesPointTransform = new MatrixTransform() { Matrix = Matrix.Identity };
			if (clipBounds.HasValue) {
				Rect actualClipBounds = clipBounds.Value;
				actualClipBounds = new Rect(new Point(actualClipBounds.Left - barBounds.Left, actualClipBounds.Top - barBounds.Top),
											new Point(actualClipBounds.Right - barBounds.Left, actualClipBounds.Bottom - barBounds.Top));
				clipBounds = seriesPointTransform.TransformBounds(actualClipBounds);
			}
			layout.Complete(barBounds, seriesPointTransform, clipBounds);
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			Bar2D? bar = CalculateBar(mapping, pointItem.RefinedPoint, true);
			if (!bar.HasValue)
				return new Point(0.0, 0.0);
			Point anchorPoint = GetLabelAnchorPoint(pointItem, mapping, transform, true, bar.Value);
			if (pointItem.Model != null)
				anchorPoint = pointItem.Model.CorrectCenterLabelLocation(anchorPoint);
			return transform.Transform(anchorPoint);
		}
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			if (pointItem == null || pointItem.RefinedPoint == null)
				return null;
			Rect barBounds = CalculateSeriesPointBounds(mapping, pointItem.RefinedPoint);
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			return new BarSeries2DPointLayout(viewport, barBounds, CalculateSeriesPointClipBounds(mapping, pointItem, viewport, barBounds));
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxBarRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo.Pane, snapMode);
		}
		internal double GetPointValue(RefinedPoint refinedPoint) {
			return GetHighValue(refinedPoint);
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Bar2DAnimationBase))
				return;
			PointAnimation = value as Bar2DAnimationBase;
		}
	}
	public abstract class BarSeries2D : BarSeries2DBase {		
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(Bar2DModel), typeof(BarSeries2D), new PropertyMetadata(ModelPropertyChanged));
		static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarSeries2D series = d as BarSeries2D;
			if (series != null)
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as Bar2DModel, e.NewValue as Bar2DModel, series);
			ChartElementHelper.Update(d, e);
		}
		protected internal override bool IsLabelConnectorItemVisible { get { return GetSeriesLabelPosition() == Bar2DLabelPosition.Outside && ActualLabel.ConnectorVisible; } }
		protected internal override bool InFrontOfAxes {
			get {
				Bar2DModel model = Model;
				return model != null && model.ActualInFrontOfAxes;
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BarSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Bar2DModel Model {
			get { return (Bar2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		protected abstract Bar2DLabelPosition GetSeriesLabelPosition();		
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			if (labelItem.RefinedPoint == null || labelItem.RefinedPoint.SeriesPoint.AnimatedValues == null || labelItem.RefinedPoint.SeriesPoint.AnimatedValues[0] == 0)
				return null;
			Bar2DLabelPosition position = GetSeriesLabelPosition();
			XYSeriesLabel2DLayout layout;
			if (position == Bar2DLabelPosition.Auto) {
				layout = CalculateSeriesLabelLayout(labelItem, mapping, transform, true);
				if (layout != null) {
					labelItem.ShowConnector = false;
					BarSeries2DPointLayout barLayout = labelItem.PointItem.Layout as BarSeries2DPointLayout;
					if (barLayout != null && layout.Bounds.Height > barLayout.InitialBounds.Height) {
						layout = CalculateSeriesLabelLayout(labelItem, mapping, transform, false);
						labelItem.ShowConnector = true;
					}
				}
			}
			else {
				layout = CalculateSeriesLabelLayout(labelItem, mapping, transform, position == Bar2DLabelPosition.Center);
				labelItem.ShowConnector = position != Bar2DLabelPosition.Center;
			}
			labelItem.ShowConnector = this.ActualLabel.ConnectorVisible;
			return layout;
		}		
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				CustomBar2DModel model = sender as CustomBar2DModel;
				if (model != null) {
					ChartElementHelper.Update(this);
					return true;
				}
			}
			return base.PerformWeakEvent(managerType, sender, e);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BarSeries2D barSeries2D = series as BarSeries2D;
			if (barSeries2D != null) {
				if (CopyPropertyValueHelper.IsValueSet(barSeries2D, ModelProperty) && CopyPropertyValueHelper.VerifyValues(this, barSeries2D, ModelProperty))
					Model = barSeries2D.Model.CloneModel();
			}
		}	 
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return Model;
		}
		protected internal override bool ShouldFlipNegativeBar(SeriesPointItem pointItem) {
			Bar2DModel model = Model;
			return model == null || model.ActualFlipNegativeBars;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public struct Bar2D {
		public readonly double Argument;
		public readonly double LowValue;
		public readonly double HighValue;
		public readonly double FixedOffset;
		public readonly double Left;
		public readonly double Right;
		public readonly double Top;
		public readonly double Bottom;
		public readonly Rect Bounds;
		public Bar2D(double argument, double lowValue, double highValue, double fixedOffset, double left, double right, double top, double bottom) {
			Argument = argument;
			LowValue = lowValue;
			HighValue = highValue;
			FixedOffset = fixedOffset;
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
			Bounds = new Rect(new Point(Left, Top), new Point(Right, Bottom));
		}
	}
}
