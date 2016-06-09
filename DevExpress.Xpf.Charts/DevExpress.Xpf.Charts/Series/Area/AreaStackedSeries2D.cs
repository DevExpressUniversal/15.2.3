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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Charts.ModelSupport;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel)),
	]
	public class AreaStackedSeries2D : MarkerSeries2D, ISupportTransparency, ISupportSeriesBorder, IStackedView, IGeometryHolder {
		public static readonly DependencyProperty TransparencyProperty = DependencyPropertyManager.Register("Transparency",
			typeof(double), typeof(AreaStackedSeries2D), new FrameworkPropertyMetadata(0.0, ChartElementHelper.Update), AreaSeries2D.ValidateTransparency);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(AreaStacked2DFadeInAnimation), typeof(AreaStackedSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
		   typeof(Area2DAnimationBase), typeof(AreaStackedSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		public static readonly DependencyProperty BorderProperty = DependencyPropertyManager.Register("Border",
			typeof(SeriesBorder), typeof(AreaStackedSeries2D), new PropertyMetadata(null, BorderChanged));
		static readonly DependencyPropertyKey ActualBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder",
			typeof(SeriesBorder), typeof(AreaStackedSeries2D), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualBorderProperty = ActualBorderPropertyKey.DependencyProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaStackedSeries2DTransparency"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double Transparency {
			get { return (double)GetValue(TransparencyProperty); }
			set { SetValue(TransparencyProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ColorEach {
			get { return base.ColorEach; }
			set { base.ColorEach = value; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaStackedSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AreaStacked2DFadeInAnimation PointAnimation {
			get { return (AreaStacked2DFadeInAnimation)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaStackedSeries2DSeriesAnimation"),
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
	DevExpressXpfChartsLocalizedDescription("AreaStackedSeries2DBorder"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesBorder Border {
			get { return (SeriesBorder)GetValue(BorderProperty); }
			set { SetValue(BorderProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SeriesBorder ActualBorder {
			get { return (SeriesBorder)GetValue(ActualBorderProperty); }
		}
		static void BorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AreaStackedSeries2D series = d as AreaStackedSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorderPropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, e.NewValue as SeriesBorder, series);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		const int DefaultLineThickness = 1;
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal override bool HasAdditionalGeometryBottomStrip { get { return true; } }
		protected internal override VisualSelectionType SupportedLegendMarkerSelectionType { get { return VisualSelectionType.Hatch | base.SupportedLegendMarkerSelectionType; } }
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected override Type PointInterfaceType { get { return typeof(IStackedPoint); } }
		protected override int PixelsPerArgument { get { return 40; } }
		protected override bool HasInvisibleMarkers { get { return true; } }
		public AreaStackedSeries2D() {
			DefaultStyleKey = typeof(AreaStackedSeries2D);
			this.SetValue(ActualBorderPropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness));
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new StackedAreaGeometryStripCreator();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			AreaStackedSeries2D areaStackedSeries2D = series as AreaStackedSeries2D;
			if (areaStackedSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, areaStackedSeries2D, TransparencyProperty);
				if (CopyPropertyValueHelper.IsValueSet(areaStackedSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaStackedSeries2D, PointAnimationProperty))
						PointAnimation = areaStackedSeries2D.PointAnimation.CloneAnimation() as AreaStacked2DFadeInAnimation;
				if (CopyPropertyValueHelper.IsValueSet(areaStackedSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, areaStackedSeries2D, SeriesAnimationProperty))
						SeriesAnimation = areaStackedSeries2D.SeriesAnimation.CloneAnimation() as Area2DAnimationBase;
				CopyPropertyValueHelper.CopyPropertyValue(this, areaStackedSeries2D, BorderProperty);
			}
		}
		protected override Series CreateObjectForClone() {
			return new AreaStackedSeries2D();
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return ColorUtils.GetNotTransparentBrush(brush);
		}
		protected internal override SolidColorBrush MixColor(Color color) {
			return ColorUtils.GetTransparentBrush(ColorUtils.MixWithDefaultColor(color), Transparency);
		}		
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return null;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			if (pointItem.PointItemPresentation.CanLayout)
				base.CompletePointLayout(pointItem);
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return new CircleMarker2DModel();
		}
		protected override int CalculateMarkerSize(PaneMapping mapping, RefinedPoint pointInfo) {
			return 10;
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			yield return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalStackedAreaSeriesGeometry(this);
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new AreaStacked2DFadeInAnimation();
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new Area2DDropFromFarAnimation();
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			Area2DAnimationBase areaAnimation = GetActualSeriesAnimation() as Area2DAnimationBase;
			return areaAnimation == null ? null :
				areaAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), IsAxisXReversed, IsAxisYReversed, mapping.Rotated, SeriesProgress.ActualProgress);
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is SeriesBorder) {
				UpdateAdditionalGeometryAppearance();
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(AreaStacked2DFadeInAnimation)));
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			SeriesWithMarkerHelper.FillAreaAnimationKinds(animationKinds);
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this, true);
		}
		protected override double GetMaxValue(RefinedPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected internal override bool IsPointItemHidden(SeriesPointItem pointItem) {
			return true;
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is AreaStacked2DFadeInAnimation))
				return;
			PointAnimation = value as AreaStacked2DFadeInAnimation;		   
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is Area2DAnimationBase))
				return;
			SeriesAnimation = value as Area2DAnimationBase;			
		}
	}
}
