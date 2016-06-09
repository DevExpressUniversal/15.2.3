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
	public abstract class CircularAreaSeries2D : CircularSeries2D, ISupportTransparency, ISupportSeriesBorder, IGeometryHolder {
		public static readonly DependencyProperty MarkerVisibleProperty = DependencyPropertyManager.Register("MarkerVisible",
			typeof(bool), typeof(CircularAreaSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
		   typeof(CircularAreaAnimationBase), typeof(CircularAreaSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		public static readonly DependencyProperty TransparencyProperty = DependencyPropertyManager.Register("Transparency",
		   typeof(double), typeof(CircularAreaSeries2D), new PropertyMetadata(0.0, ChartElementHelper.Update), AreaSeries2D.ValidateTransparency);
		public static readonly DependencyProperty BorderProperty = DependencyPropertyManager.Register("Border",
		   typeof(SeriesBorder), typeof(CircularAreaSeries2D), new PropertyMetadata(null, BorderChanged));
		static readonly DependencyPropertyKey ActualBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder",
		   typeof(SeriesBorder), typeof(CircularAreaSeries2D), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualBorderProperty = ActualBorderPropertyKey.DependencyProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularAreaSeries2DMarkerVisible"),
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
	DevExpressXpfChartsLocalizedDescription("CircularAreaSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public CircularAreaAnimationBase SeriesAnimation {
			get { return (CircularAreaAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularAreaSeries2DTransparency"),
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
	DevExpressXpfChartsLocalizedDescription("CircularAreaSeries2DBorder"),
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
			CircularAreaSeries2D series = d as CircularAreaSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorderPropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(defaultBorderThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, e.NewValue as SeriesBorder, series);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		const int defaultBorderThickness = 1;
		protected internal override bool ArePointsVisible { get { return MarkerVisible; } }
		protected internal override bool IsAdditionalGeometryClosed { get { return true; } }
		protected internal override Brush AdditionalGeometrySelectionOpacityMask { get { return VisualSelectionHelper.SelectionOpacityMask; } }
		protected internal override VisualSelectionType SupportedLegendMarkerSelectionType { get { return VisualSelectionType.Hatch | base.SupportedLegendMarkerSelectionType; } }
		public CircularAreaSeries2D() {
			this.SetValue(ActualBorderPropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(defaultBorderThickness));
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return new RadarAreaGeometryStripCreator(true);
		}
		#endregion
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is SeriesBorder) {
				UpdateAdditionalGeometryAppearance();
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			CircularAreaSeries2D circularSeries2D = series as CircularAreaSeries2D;
			if (circularSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, MarkerVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, TransparencyProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, BorderProperty);
				if (CopyPropertyValueHelper.IsValueSet(circularSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, circularSeries2D, SeriesAnimationProperty))
						SeriesAnimation = circularSeries2D.SeriesAnimation.CloneAnimation() as CircularAreaAnimationBase;
			}
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalAreaSeriesGeometry(this);
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			CircularAreaAnimationBase areaAnimation = GetActualSeriesAnimation() as CircularAreaAnimationBase;
			return areaAnimation == null ? null :
				areaAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), SeriesProgress.ActualProgress);
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new CircularAreaZoomInAnimation();
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return ColorUtils.GetNotTransparentBrush(brush);
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(CircularAreaZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularAreaSpinAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularAreaSpinZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularAreaUnwindAnimation)));
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is CircularAreaAnimationBase))
				return;
			SeriesAnimation = (CircularAreaAnimationBase)value;
		}		
	}
}
