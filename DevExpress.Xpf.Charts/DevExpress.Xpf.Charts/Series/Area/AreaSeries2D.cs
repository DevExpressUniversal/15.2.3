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
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel))
	]
	public class AreaSeries2D : MarkerSeries2D, ISupportTransparency, ISupportMarker2D, ISupportSeriesBorder, IGeometryHolder {
		public static readonly DependencyProperty MarkerModelProperty = DependencyPropertyManager.Register("MarkerModel",
			typeof(Marker2DModel), typeof(AreaSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MarkerSizeProperty = DependencyPropertyManager.Register("MarkerSize",
			typeof(int), typeof(AreaSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSizeValidation);
		public static readonly DependencyProperty MarkerVisibleProperty = DependencyPropertyManager.Register("MarkerVisible",
			typeof(bool), typeof(AreaSeries2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty TransparencyProperty = DependencyPropertyManager.Register("Transparency", 
			typeof(double), typeof(AreaSeries2D), new PropertyMetadata(0.0, ChartElementHelper.Update), ValidateTransparency);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Marker2DAnimationBase), typeof(AreaSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
			typeof(Area2DAnimationBase), typeof(AreaSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		public static readonly DependencyProperty BorderProperty = DependencyPropertyManager.Register("Border",
			typeof(SeriesBorder), typeof(AreaSeries2D), new PropertyMetadata(null, BorderChanged));
		static readonly DependencyPropertyKey ActualBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder",
			typeof(SeriesBorder), typeof(AreaSeries2D), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualBorderProperty = ActualBorderPropertyKey.DependencyProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DMarkerModel"),
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
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DMarkerSize"),
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
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DMarkerVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool MarkerVisible {
			get { return (bool)GetValue(MarkerVisibleProperty); }
			set { SetValue(MarkerVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DTransparency"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double Transparency {
			get { return (double)GetValue(TransparencyProperty); }
			set { SetValue(TransparencyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DPointAnimation"),
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
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Area2DAnimationBase SeriesAnimation {
			get { return (Area2DAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaSeries2DBorder"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesBorder Border {
			get { return (SeriesBorder)GetValue(BorderProperty); }
			set { SetValue(BorderProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SeriesBorder ActualBorder {
			get { return (SeriesBorder)GetValue(ActualBorderProperty); }
		}
		static void BorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AreaSeries2D series = d as AreaSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorderPropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, e.NewValue as SeriesBorder, series);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		internal static bool ValidateTransparency(object transparency) {
			return (double)transparency >= 0 && (double)transparency <= 1;
		}				
		const int DefaultLineThickness = 1;
		protected override bool ShouldSortPoints { get { return true; } }
		protected internal override bool ArePointsVisible { get { return MarkerVisible; } }
		protected internal override bool HasAdditionalGeometryBottomStrip { get { return true; } }
		protected internal override VisualSelectionType SupportedLegendMarkerSelectionType { get { return VisualSelectionType.Hatch | base.SupportedLegendMarkerSelectionType; } }
		protected override int PixelsPerArgument { get { return this.MarkerSize; } }
		protected override bool HasInvisibleMarkers { get { return !MarkerVisible; } }
		public AreaSeries2D() {
			DefaultStyleKey = typeof(AreaSeries2D);
			this.SetValue(ActualBorderPropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness));
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new AreaGeometryStripCreator();
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is SeriesBorder) {
				UpdateAdditionalGeometryAppearance();
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected override Series CreateObjectForClone() {
			return new AreaSeries2D();
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return ColorUtils.GetNotTransparentBrush(brush);
		}
		protected internal override SolidColorBrush MixColor(Color color) {
			return ColorUtils.GetTransparentBrush(ColorUtils.MixWithDefaultColor(color), Transparency);
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return MarkerModel;
		}
		protected override int CalculateMarkerSize(PaneMapping mapping, RefinedPoint pointInfo) {
			return MarkerSize;
		}
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return MarkerVisible ? base.CreateSeriesPointLayout(mapping, pointItem) : null;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			if (pointItem.PointItemPresentation.CanLayout)
				base.CompletePointLayout(pointItem);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			AreaSeries2D areaSeries2D = series as AreaSeries2D;
			if (areaSeries2D != null) {
				if (CopyPropertyValueHelper.IsValueSet(areaSeries2D, MarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaSeries2D, MarkerModelProperty))
						MarkerModel = areaSeries2D.MarkerModel.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, areaSeries2D, MarkerSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, areaSeries2D, MarkerVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, areaSeries2D, TransparencyProperty);
				if (CopyPropertyValueHelper.IsValueSet(areaSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaSeries2D, PointAnimationProperty))
						PointAnimation = areaSeries2D.PointAnimation.CloneAnimation() as Marker2DAnimationBase;
				if (CopyPropertyValueHelper.IsValueSet(areaSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaSeries2D, SeriesAnimationProperty))
						SeriesAnimation = areaSeries2D.SeriesAnimation.CloneAnimation() as Area2DAnimationBase;
				CopyPropertyValueHelper.CopyPropertyValue(this, areaSeries2D, BorderProperty);
			}
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalAreaSeriesGeometry(this);
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Marker2DFadeInAnimation();
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new Area2DStretchFromNearAnimation();
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			Area2DAnimationBase areaAnimation = GetActualSeriesAnimation() as Area2DAnimationBase;
			return areaAnimation == null ? null :
				areaAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), IsAxisXReversed, IsAxisYReversed, mapping.Rotated, SeriesProgress.ActualProgress);
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			FillMarkerAnimationKinds(animationKinds);			
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			SeriesWithMarkerHelper.FillAreaAnimationKinds(animationKinds);
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
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
			if (value != null && !(value is Area2DAnimationBase))
				return;
			SeriesAnimation = value as Area2DAnimationBase;			
		}
	}
}
