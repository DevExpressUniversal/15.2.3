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
	public abstract class CircularLineSeries2D : CircularSeries2D, ILineSeries, IGeometryHolder {
		static readonly DependencyPropertyKey ActualLineThicknessPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLineThickness",
			typeof(int), typeof(CircularLineSeries2D), new PropertyMetadata());
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ActualLineThicknessProperty = ActualLineThicknessPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ClosedProperty = DependencyPropertyManager.Register("Closed",
			typeof(bool), typeof(CircularLineSeries2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty MarkerVisibleProperty = DependencyPropertyManager.Register("MarkerVisible",
			typeof(bool), typeof(CircularLineSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
		   typeof(CircularLineAnimationBase), typeof(CircularLineSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		public static readonly DependencyProperty LineStyleProperty = DependencyPropertyManager.Register("LineStyle",
			typeof(LineStyle), typeof(CircularLineSeries2D), new PropertyMetadata(LineStyleChanged));
		static readonly DependencyPropertyKey ActualLineStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLineStyle",
		   typeof(LineStyle), typeof(CircularLineSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty ActualLineStyleProperty = ActualLineStylePropertyKey.DependencyProperty;
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ActualLineThickness {
			get { return (int)GetValue(ActualLineThicknessProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularLineSeries2DClosed"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool Closed {
			get { return (bool)GetValue(ClosedProperty); }
			set { SetValue(ClosedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularLineSeries2DMarkerVisible"),
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
	DevExpressXpfChartsLocalizedDescription("CircularLineSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty
		]
		public CircularLineAnimationBase SeriesAnimation {
			get { return (CircularLineAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularLineSeries2DLineStyle"),
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
		static void LineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularLineSeries2D series = d as CircularLineSeries2D;
			if (series != null) {
				LineStyle newLineStyle = e.NewValue as LineStyle;
				LineStyle actualLineStyle = newLineStyle == null ? series.CreateDefaultLineStyle() : newLineStyle;
				series.SetActualLineStyle(e.OldValue as LineStyle, actualLineStyle);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		const int defaultLineThickness = 2;
		protected override LineStyle LegendMarkerLineStyle { get { return ActualLineStyle; } }
		protected internal override bool ArePointsVisible { get { return MarkerVisible; } }
		protected internal override bool IsAdditionalGeometryClosed { get { return Closed; } }
		public CircularLineSeries2D() {
			SetActualLineStyle(null, CreateDefaultLineStyle());
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
			return new LineGeometryStripCreator(Closed);
		}
		#endregion
		#region ILineSeries
		int ILineSeries.LineThickness { get { return ActualLineThickness; } }
		LineStyle ILineSeries.LineStyle { get { return ActualLineStyle; } }
		#endregion
		LineStyle CreateDefaultLineStyle() {
			LineStyle lineStyle = new LineStyle();
			lineStyle.Thickness = defaultLineThickness;
			lineStyle.LineJoin = PenLineJoin.Round;
			return lineStyle;
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
			CircularLineSeries2D circularSeries2D = series as CircularLineSeries2D;
			if (circularSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, ClosedProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, MarkerVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, circularSeries2D, LineStyleProperty);
				if (CopyPropertyValueHelper.IsValueSet(circularSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, circularSeries2D, SeriesAnimationProperty))
						SeriesAnimation = circularSeries2D.SeriesAnimation.CloneAnimation() as CircularLineAnimationBase;
			}
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalLineSeriesGeometry(this);
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			CircularLineAnimationBase lineAnimation = GetActualSeriesAnimation() as CircularLineAnimationBase;
			return lineAnimation == null ? null :
				lineAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), SeriesProgress.ActualProgress);
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new CircularLineZoomInAnimation();
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			UpdateActualLineThickness();
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(CircularLineZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularLineSpinAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularLineSpinZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(CircularLineUnwindAnimation)));
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is CircularLineAnimationBase))
				return;
			SeriesAnimation = (CircularLineAnimationBase)value;
		}		
	}
}
