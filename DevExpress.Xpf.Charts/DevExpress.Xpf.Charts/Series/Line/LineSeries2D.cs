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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel))
	]
	public class LineSeries2D : MarkerSeries2D, ISupportMarker2D, ILineSeries, IGeometryHolder {
		static readonly DependencyPropertyKey ActualLineThicknessPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLineThickness",
		typeof(int), typeof(LineSeries2D), new PropertyMetadata());
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualLineThicknessProperty = ActualLineThicknessPropertyKey.DependencyProperty;
		public static readonly DependencyProperty MarkerModelProperty = DependencyPropertyManager.Register("MarkerModel",
			typeof(Marker2DModel), typeof(LineSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MarkerSizeProperty = DependencyPropertyManager.Register("MarkerSize",
			typeof(int), typeof(LineSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSizeValidation);
		public static readonly DependencyProperty MarkerVisibleProperty = DependencyPropertyManager.Register("MarkerVisible",
			typeof(bool), typeof(LineSeries2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty LineStyleProperty = DependencyPropertyManager.Register("LineStyle", 
			typeof(LineStyle), typeof(LineSeries2D), new PropertyMetadata(LineStyleChanged));
		static readonly DependencyPropertyKey ActualLineStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLineStyle",
			typeof(LineStyle), typeof(LineSeries2D), new PropertyMetadata(ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ActualLineStyleProperty = ActualLineStylePropertyKey.DependencyProperty;
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Marker2DAnimationBase), typeof(LineSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
			typeof(Line2DAnimationBase), typeof(LineSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ActualLineThickness {
			get { return (int)GetValue(ActualLineThicknessProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MarkerModel {
			get { return (Marker2DModel)GetValue(MarkerModelProperty); }
			set { SetValue(MarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DMarkerVisible"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool MarkerVisible {
			get { return (bool)GetValue(MarkerVisibleProperty); }
			set { SetValue(MarkerVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DLineStyle"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle LineStyle {
			get { return (LineStyle)GetValue(LineStyleProperty); }
			set { SetValue(LineStyleProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LineStyle ActualLineStyle {
			get { return (LineStyle)GetValue(ActualLineStyleProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DAnimationBase PointAnimation {
			get { return (Marker2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Line2DAnimationBase SeriesAnimation {
			get { return (Line2DAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		static void LineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LineSeries2D series = d as LineSeries2D;
			if (series != null) {
				LineStyle newLineStyle = e.NewValue as LineStyle;
				LineStyle actualLineStyle = newLineStyle == null ? series.CreateDefaultLineStyle() : newLineStyle;
				series.SetActualLineStyle(e.OldValue as LineStyle, actualLineStyle);
			}
		}
		internal const int DefaultLineThickness = 2;
		protected override LineStyle LegendMarkerLineStyle { get { return ActualLineStyle; } }
		protected internal override bool ArePointsVisible { get { return MarkerVisible; } }
		protected override int PixelsPerArgument { get { return this.MarkerSize; } }
		protected override bool HasInvisibleMarkers { get { return !MarkerVisible; } }
		public LineSeries2D() {
			DefaultStyleKey = typeof(LineSeries2D);
			SetActualLineStyle(null, CreateDefaultLineStyle());
		}
		LineStyle CreateDefaultLineStyle() {
			LineStyle lineStyle = new LineStyle();
			lineStyle.Thickness = DefaultLineThickness;
			lineStyle.LineJoin = PenLineJoin.Round;
			return lineStyle;
		}
		void SetActualLineStyle(LineStyle oldValue, LineStyle newValue) {
			this.SetValue(ActualLineStylePropertyKey, newValue);
			CommonUtils.SubscribePropertyChangedWeakEvent(oldValue, newValue, this);
			UpdateActualLineThickness();
		}
		void UpdateActualLineThickness() {
			this.SetValue(ActualLineThicknessPropertyKey, VisualSelectionHelper.GetLineThickness(ActualLineStyle.Thickness, IsSelected));
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		#region ILineSeries
		int ILineSeries.LineThickness { get { return ActualLineThickness; } }
		LineStyle ILineSeries.LineStyle { get { return ActualLineStyle; } }
		#endregion
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new LineGeometryStripCreator(false);
		}
		protected override Series CreateObjectForClone() {
			return new LineSeries2D();
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = base.PerformWeakEvent(managerType, sender, e);
			if (managerType == typeof(PropertyChangedWeakEventManager) && (sender is LineStyle)) {
				UpdateActualLineThickness();
				UpdateAdditionalGeometryAppearance();
				success = true;
			}
			return success;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			LineSeries2D lineSeries2D = series as LineSeries2D;
			if (lineSeries2D != null) {
				if (CopyPropertyValueHelper.IsValueSet(lineSeries2D, MarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, lineSeries2D, MarkerModelProperty))
						MarkerModel = lineSeries2D.MarkerModel.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, lineSeries2D, MarkerSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, lineSeries2D, MarkerVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, lineSeries2D, LineStyleProperty);
				if (CopyPropertyValueHelper.IsValueSet(lineSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, lineSeries2D, PointAnimationProperty))
						PointAnimation = lineSeries2D.PointAnimation.CloneAnimation() as Marker2DAnimationBase;
				if (CopyPropertyValueHelper.IsValueSet(lineSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, lineSeries2D, SeriesAnimationProperty))
						SeriesAnimation = lineSeries2D.SeriesAnimation.CloneAnimation() as Line2DAnimationBase;
			}
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return MarkerModel;
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			Line2DAnimationBase lineAnimation = GetActualSeriesAnimation() as Line2DAnimationBase;
			return lineAnimation == null ? null :
				lineAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), IsAxisXReversed, IsAxisYReversed, mapping.Rotated, SeriesProgress.ActualProgress);
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			UpdateActualLineThickness();
		}
		protected override int CalculateMarkerSize(PaneMapping mapping, RefinedPoint pointInfo) {
			return MarkerSize;
		}
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return MarkerVisible ? base.CreateSeriesPointLayout(mapping, pointItem) : null;
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalLineSeriesGeometry(this);
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			if (pointItem.PointItemPresentation.CanLayout)
				base.CompletePointLayout(pointItem);
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Marker2DFadeInAnimation();
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new Line2DStretchFromNearAnimation();
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			FillMarkerAnimationKinds(animationKinds);			
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Line2DSlideFromLeftAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DSlideFromRightAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DSlideFromTopAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DSlideFromBottomAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DUnwrapVerticallyAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DUnwrapHorizontallyAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DBlowUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DStretchFromNearAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DStretchFromFarAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Line2DUnwindAnimation)));
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		protected internal override SeriesPointItem[] CreateSeriesPointItems(RefinedPoint refinedPoint, SeriesPointData seriesPointData) {
			return new SeriesPointItem[] { new SeriesPointItem(this, seriesPointData) };
		}
		protected internal override bool IsPointItemHidden(SeriesPointItem pointItem) {
			return !MarkerVisible;
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Marker2DAnimationBase))
				return;
			PointAnimation = value as Marker2DAnimationBase;			
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is Line2DAnimationBase))
				return;
			SeriesAnimation = value as Line2DAnimationBase;
		}
	}
}
