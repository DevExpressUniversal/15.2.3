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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class XYSeries : Series {
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush", 
			typeof(SolidColorBrush), typeof(XYSeries), new PropertyMetadata(null, ChartElementHelper.UpdateWithClearDiagramCache));
		public static readonly DependencyProperty ColorEachProperty = DependencyPropertyManager.Register("ColorEach", 
			typeof(bool), typeof(XYSeries), new PropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
		protected static void UpdateAdditionalGeometryAppearance(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYSeries series = d as XYSeries;
			if (series != null)
				series.UpdateAdditionalGeometryAppearance();
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeriesBrush"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public SolidColorBrush Brush {
			get { return (SolidColorBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYSeriesColorEach"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool ColorEach {
			get { return (bool)GetValue(ColorEachProperty); }
			set { SetValue(ColorEachProperty, value); }
		}
		ChartContentPresenter additionalGeometryHolder;
		SecondaryAxisX2D axisX;
		SecondaryAxisY2D axisY;
		protected AdditionalLineSeriesGeometry AdditionalGeometry {
			get {
				return AdditionalGeometryHolder != null ? AdditionalGeometryHolder.Content as AdditionalLineSeriesGeometry : null;
			}
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal IPane ActualPane { get { return Diagram as IPane; } }
		protected internal virtual bool IsAxisXReversed { get { return false; } }
		protected internal virtual bool IsAxisYReversed { get { return false; } }
		protected internal virtual bool HasAdditionalGeometryBottomStrip { get { return false; } }
		protected internal virtual bool IsAdditionalGeometryClosed { get { return false; } }
		protected internal override bool ActualColorEach { get { return ColorEach; } }
		protected internal virtual Brush AdditionalGeometrySelectionOpacityMask { get { return null; } }
		protected internal override Color BaseColor {
			get {
				SolidColorBrush brush = Brush;
				return brush == null ? base.BaseColor : brush.Color;
			}
		}
		internal ChartContentPresenter AdditionalGeometryHolder {
			get { return additionalGeometryHolder; }
			set { additionalGeometryHolder = value; }
		}
		internal SecondaryAxisX2D AxisXInternal { get { return axisX; } set { axisX = value; } }
		internal SecondaryAxisY2D AxisYInternal { get { return axisY; } set { axisY = value; } }
		protected override Type PointInterfaceType {
			get { return typeof(IXYPoint); }
		}
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.XYView; } }
		protected void UpdateAdditionalGeometryAppearance() {
			AdditionalLineSeriesGeometry additionalGeometry = AdditionalGeometry;
			if (additionalGeometry != null)
				additionalGeometry.Update(false);
		}
		protected void UpdateAdditionalGeometry() {
			AdditionalLineSeriesGeometry additionalGeometry = AdditionalGeometry;
			if (additionalGeometry != null)
				additionalGeometry.Update(true);
		}
		protected internal virtual Transform CreateSeriesAnimationTransform(IMapping mapping) {
			return null;
		}
		protected internal virtual IMapping CreateDiagramMapping() { return null; }
		protected internal virtual Point CalculateToolTipPoint(SeriesPointItem pointItem, bool inLabel) {
			return new Point();
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			if (AdditionalGeometry != null)
				AdditionalGeometry.SelectedState = isSelected;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			XYSeries xySeries = series as XYSeries;
			if (xySeries != null) {
				if (xySeries.Brush != null)
					Brush = xySeries.Brush.CloneCurrentValue();
				CopyPropertyValueHelper.CopyPropertyValue(this, xySeries, ColorEachProperty);
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, xySeries, XYDiagram2D.SeriesAxisXProperty);
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, xySeries, XYDiagram2D.SeriesAxisYProperty);
				CopyPropertyValueHelper.CopyPropertyValueByRef(this, xySeries, XYDiagram2D.SeriesPaneProperty);
			}
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IXYPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IXYPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return ((IXYPoint)point).Value;
		}
	}
}
